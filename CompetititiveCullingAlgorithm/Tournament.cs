﻿using Shields.GraphViz.Components;
using Shields.GraphViz.Models;
using Shields.GraphViz.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static TournamentSort.Program;

namespace CompetititiveCullingAlgorithm
{
    [Serializable()]
    class Tournament<T>
    {
        #region Node tree
        abstract class Node
        {
            /** Traverse the tree filling outList with the items that will be compared next, in order
             * (first in the list is compared the first and so on).
             * In the case of the items being photos which take 2 seconds to load, using this lists it's 
             * possible to preload them in advance so that while the user thinks about a comparison, the
             * photos for the next comparison are being loaded. */
            public abstract void PopulateItemsWorthPreloading(List<T> outList);
            public abstract void ForEachLeafNode(Action<LeafNode> action);
            public abstract Task<LeafNode> BestNodeAsync(IAsyncComparator<T> comparator);
            public BracketNode Parent { get; set; }
        }

        class LeafNode : Node
        {
            public LeafNode(T item)
            {
                Item = item;
            }

            public T Item { get; }

            public override Task<LeafNode> BestNodeAsync(IAsyncComparator<T> comparator)
            {
                return Task.FromResult(this);
            }

            public override void ForEachLeafNode(Action<LeafNode> action)
            {
                action.Invoke(this);
            }

            public override void PopulateItemsWorthPreloading(List<T> outList)
            {
                // If I'm called it's because my parent BracketNode doesn't know if I'm better than my
                // sibling, hence our pair will be shown ande therefore it's worth preloading.
                outList.Add(Item);
            }
        }

        class BracketNode : Node
        {
            public BracketNode(Node competitorA, Node competitorB)
            {
                CompetitorA = competitorA;
                CompetitorB = competitorB;
                BestCompetitor = null;

                Debug.Assert(competitorA.Parent == null);
                Debug.Assert(competitorB.Parent == null);
                competitorA.Parent = competitorB.Parent = this;
            }

            public Node CompetitorA { get; private set; }
            public Node CompetitorB { get; private set; }
            public Node BestCompetitor { get; private set; }

            public void ReplaceWinnerChildBracketWithNode(Node replacement)
            {
                /* This method is called at least on the grandparent of the winner, because the parent of the 
                 * winner is replaced entirely by the winner. */
                Debug.Assert(BestCompetitor != null);
                Debug.Assert(replacement.Parent == null);

                if (BestCompetitor == CompetitorA)
                    CompetitorA = replacement;
                else
                    CompetitorB = replacement;
                replacement.Parent = this;

                OnDroppedCompetitor();
            }

            private void OnDroppedCompetitor()
            {
                /* Need a rematch in this bracket, as well on parent brackets. */
                BestCompetitor = null;
                if (Parent != null)
                    Parent.OnDroppedCompetitor();
            }

            public async override Task<LeafNode> BestNodeAsync(IAsyncComparator<T> comparator)
            {
                if (BestCompetitor == null)
                {
                    T itemA = (await CompetitorA.BestNodeAsync(comparator)).Item;
                    T itemB = (await CompetitorB.BestNodeAsync(comparator)).Item;
                    BestCompetitor = (await comparator.CompareAsync(itemA, itemB)) > 0 ? CompetitorA : CompetitorB;
                }
                return await BestCompetitor.BestNodeAsync(comparator);
            }

            public Node CompetitorOtherThan(Node node)
            {
                if (node == CompetitorA)
                    return CompetitorB;
                else if (node == CompetitorB)
                    return CompetitorA;
                else
                    throw new Exception("Unknown node provided.");
            }

            public override void PopulateItemsWorthPreloading(List<T> outList)
            {
                if (BestCompetitor == null)
                {
                    CompetitorA.PopulateItemsWorthPreloading(outList);
                    CompetitorB.PopulateItemsWorthPreloading(outList);
                }
            }

            public override void ForEachLeafNode(Action<LeafNode> action)
            {
                CompetitorA.ForEachLeafNode(action);
                CompetitorB.ForEachLeafNode(action);
            }
        }

        static void PlotBrackets(IAsyncComparator<T> comparator, Node topMostRoot)
        {
            Graph graph = Graph.Directed;

            Dictionary<Node, NodeId> nodeIdCache = new Dictionary<Node, NodeId>();
            int bracketCounter = 0;
            string NodeToString(Node node)
            {
                if (node is LeafNode)
                    return node.BestNodeAsync(comparator).ToString();
                else
                    return $"B{++bracketCounter}";
            }
            NodeId NodeToNodeId(Node node)
            {
                if (!nodeIdCache.ContainsKey(node))
                    nodeIdCache[node] = new NodeId(NodeToString(node));
                return nodeIdCache[node];
            }

            void Recurse(Node root)
            {
                if (root is BracketNode)
                {
                    BracketNode node = root as BracketNode;

                    Debug.Assert(node.CompetitorA.Parent == node);
                    Debug.Assert(node.CompetitorB.Parent == node);

                    graph = graph.Add(EdgeStatement.For(
                        NodeToNodeId(node),
                        NodeToNodeId(node.CompetitorA)));
                    graph = graph.Add(EdgeStatement.For(
                        NodeToNodeId(node),
                        NodeToNodeId(node.CompetitorB)));

                    Recurse(node.CompetitorA);
                    Recurse(node.CompetitorB);
                }
            }

            Recurse(topMostRoot);

            IRenderer renderer = new Renderer(@"C:\Program Files (x86)\Graphviz2.38\bin");
            using (Stream file = File.Create("graph.png"))
            {
                AsyncHelper.RunSync(async () => await renderer.RunAsync(
                    graph, file,
                    RendererLayouts.Dot,
                    RendererFormats.Png,
                    CancellationToken.None));
            }
        }

        #endregion

        public delegate void NewWinnerEventHandler(int place, T item);
        public event NewWinnerEventHandler NewWinnerEvent;

        public IAsyncComparator<T> Comparator { get; }
        public List<T> Items { get; }
        public int TotalPlaces { get; }
        private Node rootNode;
        List<T> rankingWinners = new List<T>();

        
        public struct SavedState
        {
            
        }

        internal Tournament(IAsyncComparator<T> comparator, List<T> items, int totalPlaces)
        {
            totalPlaces = Math.Min(totalPlaces, items.Count);
            Debug.Assert(items.Count > 0);
            Comparator = comparator;
            Items = items;
            TotalPlaces = totalPlaces;

            IReadOnlyList<Node> baseLevel = Items.Select(item => new LeafNode(item)).ToList();
            while (baseLevel.Count > 1)
            {
                List<Node> newLevel = new List<Node>();
                int indexPairingEnd = 2 * (baseLevel.Count / 2);
                for (int i = 0; i < indexPairingEnd; i += 2)
                {
                    newLevel.Add(new BracketNode(baseLevel[i], baseLevel[i + 1]));
                }
                if (indexPairingEnd < baseLevel.Count)
                {
                    // Odd number of competitors in this level, so the last one does not compete (against others of this level, that is)
                    newLevel.Add(baseLevel[indexPairingEnd]);
                }

                // Rinse and repeat until we have a full tree.
                baseLevel = newLevel;
            }

            rootNode = baseLevel[0]; // The only remaining node is the root node
        }

        public async Task<List<T>> CalculateTopN()
        {
            for (int place = 1; place <= TotalPlaces; place++)
            {
                LeafNode winnerNode = await rootNode.BestNodeAsync(Comparator);
                T winnerItem = winnerNode.Item;
                NewWinnerEvent(place, winnerItem);
                rankingWinners.Add(winnerItem);
                //System.Console.WriteLine($"Winner: {winnerItem} Comparisons: {comparisonCount}");

                if (place != TotalPlaces)
                {
                    /* When there is a winner, its dropped from the competition so that we have a 2nd place winner and so on. */

                    Debug.Assert(winnerNode.Parent != null);

                    Node replacement = winnerNode.Parent.CompetitorOtherThan(winnerNode);
                    /* Unparent the replacement node, its bracket is no more. */
                    replacement.Parent = null;
                    BracketNode grandparent = winnerNode.Parent.Parent;
                    if (grandparent != null)
                    {
                        /* Replace the now unneeded parent bracket node with the competitor node. */
                        grandparent.ReplaceWinnerChildBracketWithNode(replacement);
                    }
                    else
                    {
                        Debug.Assert(winnerNode.Parent == rootNode);
                        rootNode = replacement;
                    }
                }

                //PlotBrackets(rootNode);
            }

            return rankingWinners;
        }

        public List<T> PredictItemsWorthPreloading()
        {
            List<T> ret = new List<T>();
            rootNode.PopulateItemsWorthPreloading(ret);
            return ret;
        }
    }
}
