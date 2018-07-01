using Shields.GraphViz.Components;
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
    class Tournament<T> where T: IAsyncComparable<T>
    {
        public List<T> Items { get; }
        public int TotalPlaces { get; }

        public delegate void NewWinnerEventHandler(int place, T item);
        public event NewWinnerEventHandler NewWinnerEvent;
        
        internal Tournament(List<T> items, int totalPlaces)
        {
            totalPlaces = Math.Min(totalPlaces, items.Count);

            Items = items;
            TotalPlaces = totalPlaces;
        }

        abstract class Node
        {
            public abstract Task<T> BestItemAsync();
            public BracketNode Parent { get; set; }
        }

        class LeafNode : Node
        {
            public LeafNode(T item)
            {
                Item = item;
            }

            private T Item { get; }

            public override Task<T> BestItemAsync()
            {
                return Task.FromResult(Item);
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

            public async override Task<T> BestItemAsync()
            {
                if (BestCompetitor == null)
                {
                    T itemA = await CompetitorA.BestItemAsync();
                    T itemB = await CompetitorB.BestItemAsync();
                    BestCompetitor = (await itemA.CompareToAsync(itemB)) > 0 ? CompetitorA : CompetitorB;
                }
                return await BestCompetitor.BestItemAsync();
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
        }

        static void PlotBrackets(Node topMostRoot)
        {
            Graph graph = Graph.Directed;

            Dictionary<Node, NodeId> nodeIdCache = new Dictionary<Node, NodeId>();
            int bracketCounter = 0;
            string NodeToString(Node node)
            {
                if (node is LeafNode)
                    return node.BestItemAsync().ToString();
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

        public async Task<List<T>> CalculateTopN()
        {
            if (Items.Count == 0)
                return new List<T>();

            Dictionary<T, LeafNode> itemToLeafNode = new Dictionary<T, LeafNode>();
            IReadOnlyList<Node> baseLevel = Items.Select(item => itemToLeafNode[item] = new LeafNode(item)).ToList();
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

            List<T> rankingWinners = new List<T>();
            Node rootNode = baseLevel[0];
            for (int place = 1; place <= TotalPlaces; place++)
            {
                T winnerItem = await rootNode.BestItemAsync();
                NewWinnerEvent(place, winnerItem);
                rankingWinners.Add(winnerItem);
                //System.Console.WriteLine($"Winner: {winnerItem} Comparisons: {comparisonCount}");

                if (place != TotalPlaces)
                {
                    /* When there is a winner, its dropped from the competition so that we have a 2nd place winner and so on. */
                    Node winnerNode = itemToLeafNode[winnerItem];
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
    }
}
