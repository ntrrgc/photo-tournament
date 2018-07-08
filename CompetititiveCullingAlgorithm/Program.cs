using CompetititiveCullingAlgorithm;
using Shields.GraphViz.Components;
using Shields.GraphViz.Models;
using Shields.GraphViz.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace TournamentSort
{
    public partial class Program
    {
        abstract class Node<T> where T : IComparable<T>
        {
            public abstract T BestItem();
            public BracketNode<T> Parent { get; set; }
        }

        class LeafNode<T> : Node<T> where T : IComparable<T>
        {
            public LeafNode(T item)
            {
                Item = item;
            }

            private T Item { get; }

            public override T BestItem()
            {
                return Item;
            }
        }

        class BracketNode<T> : Node<T> where T : IComparable<T>
        {
            public BracketNode(Node<T> competitorA, Node<T> competitorB)
            {
                CompetitorA = competitorA;
                CompetitorB = competitorB;
                BestCompetitor = null;

                Debug.Assert(competitorA.Parent == null);
                Debug.Assert(competitorB.Parent == null);
                competitorA.Parent = competitorB.Parent = this;
            }

            public Node<T> CompetitorA { get; private set; }
            public Node<T> CompetitorB { get; private set;  }
            public Node<T> BestCompetitor { get; private set; }

            public void ReplaceWinnerChildBracketWithNode(Node<T> replacement)
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

            public override T BestItem()
            {
                if (BestCompetitor == null)
                {
                    BestCompetitor = CompetitorA.BestItem().CompareTo(CompetitorB.BestItem()) > 0 ? CompetitorA : CompetitorB;
                }
                return BestCompetitor.BestItem();
            }

            public Node<T> CompetitorOtherThan(Node<T> node)
            {
                if (node == CompetitorA)
                    return CompetitorB;
                else if (node == CompetitorB)
                    return CompetitorA;
                else
                    throw new Exception("Unknown node provided.");
            }
        }

        static void PlotBrackets<T>(Node<T> topMostRoot) where T : IComparable<T>
        {
            Graph graph = Graph.Directed;

            Dictionary<Node<T>, NodeId> nodeIdCache = new Dictionary<Node<T>, NodeId>();
            int bracketCounter = 0;
            string NodeToString(Node<T> node)
            {
                if (node is LeafNode<T>)
                    return node.BestItem().ToString();
                else
                    return $"B{++bracketCounter}";
            }
            NodeId NodeToNodeId(Node<T> node)
            {
                if (!nodeIdCache.ContainsKey(node))
                    nodeIdCache[node] = new NodeId(NodeToString(node));
                return nodeIdCache[node];
            }

            void Recurse(Node<T> root)
            {
                if (root is BracketNode<T>)
                {
                    BracketNode<T> node = root as BracketNode<T>;

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

        static List<T> CalculateTopN<T>(List<T> items, int totalPlaces) where T : IComparable<T>
        {
            totalPlaces = Math.Min(totalPlaces, items.Count);
            if (items.Count == 0)
                return new List<T>();

            Dictionary<T, LeafNode<T>> itemToLeafNode = new Dictionary<T, LeafNode<T>>();
            IReadOnlyList<Node<T>> baseLevel = items.Select(item => itemToLeafNode[item] = new LeafNode<T>(item)).ToList();
            while (baseLevel.Count > 1)
            {
                List<Node<T>> newLevel = new List<Node<T>>();
                int indexPairingEnd = 2 * (baseLevel.Count / 2);
                for (int i = 0; i < indexPairingEnd; i += 2)
                {
                    newLevel.Add(new BracketNode<T>(baseLevel[i], baseLevel[i + 1]));
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
            Node<T> rootNode = baseLevel[0];
            for (int place = 1; place <= totalPlaces; place++)
            {
                T winnerItem = rootNode.BestItem();
                rankingWinners.Add(winnerItem);
                //System.Console.WriteLine($"Winner: {winnerItem} Comparisons: {comparisonCount}");

                if (place != totalPlaces)
                {
                    /* When there is a winner, its dropped from the competition so that we have a 2nd place winner and so on. */
                    Node<T> winnerNode = itemToLeafNode[winnerItem];
                    Debug.Assert(winnerNode.Parent != null);

                    Node<T> replacement = winnerNode.Parent.CompetitorOtherThan(winnerNode);
                    /* Unparent the replacement node, its bracket is no more. */
                    replacement.Parent = null; 
                    BracketNode<T> grandparent = winnerNode.Parent.Parent;
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

        private static int comparisonCount = 0;

        class PhotoCandidate : IComparable<PhotoCandidate>
        {
            public PhotoCandidate(int n)
            {
                N = n;
            }

            public int N { get; }

            public int CompareTo(PhotoCandidate other)
            {
                comparisonCount++;
                return N - other.N;
            }

            public override string ToString() => $"{N}";
        }

        static void Stress()
        {
            var rng = new Random();
            for (int i = 0; i < 100000; i++)
            {
                var winners = CalculateTopN(Enumerable.Range(1, rng.Next(0, 20)).ToList().Shuffle().Select(x => new PhotoCandidate(x)).ToList(), 7);
                for (int j = 0; j < winners.Count - 1; j++)
                    Debug.Assert(winners[j].CompareTo(winners[j + 1]) > 0);
            }
            System.Console.WriteLine("Success.");
        }

        static void PerformanceTest()
        {
            var comparisonCounts = new List<int>();
            for (int i = 0; i < 100000; i++)
            {
                comparisonCount = 0;
                CalculateTopN(Enumerable.Range(1, 300).ToList().Shuffle().Select(x => new PhotoCandidate(x)).ToList(), 24);
                comparisonCounts.Add(comparisonCount);
            }
            Console.WriteLine($"Min comparisons: {comparisonCounts.Min()}");
            Console.WriteLine($"Max comparisons: {comparisonCounts.Max()}");
            Console.WriteLine($"Avg comparisons: {comparisonCounts.Average()}");
        }

        static async Task<int> Miau()
        {
            Console.WriteLine("start");
            Console.WriteLine($"returned {await Guau()} from Guau");

            return 5;
        }

        private static async Task<int> Guau()
        {
            await Task.Yield();
            return 2;
        }

        private static IEnumerator<string> Test()
        {
            yield return "begin";
        }

        private static TaskCompletionSource<string> helpFromMT;

        private static async Task<string> Steps()
        {
            helpFromMT = new TaskCompletionSource<string>();
            var ret = await helpFromMT.Task;
            Console.WriteLine(ret);
            await Task.Delay(300);
            return "end";
        }

        [STAThread]
        static void Main(string[] args)
        {
            //CalculateTopN(Enumerable.Range(1, new Random().Next(0, 70)).ToList().Shuffle().Select(x => new PhotoCandidate(x)).ToList(), 24);
            //PerformanceTest();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }
}
