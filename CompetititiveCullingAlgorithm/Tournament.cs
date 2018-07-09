using Shields.GraphViz.Components;
using Shields.GraphViz.Models;
using Shields.GraphViz.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using static TournamentSort.Program;

namespace CompetititiveCullingAlgorithm
{
    public class Tournament<T>
    {
        #region Node tree
        [DataContract]
        abstract class Node
        {
            public abstract void ForEachLeafNode(Action<LeafNode> action);
            public abstract Task<LeafNode> BestNodeAsync(Tournament<T> tournament,
                IAsyncComparator<T> comparator, CancellationToken cancellationToken);
            public abstract Node DeepClone();
            public abstract int CountUnanswered();
            [DataMember]
            public BracketNode Parent { get; set; }
        }

        [DataContract]
        class LeafNode : Node
        {
            public LeafNode(T item)
            {
                Item = item;
            }

            [DataMember()]
            public readonly T Item;

            public override Task<LeafNode> BestNodeAsync(Tournament<T> tournament, IAsyncComparator<T> comparator, CancellationToken cancellationToken)
            {
                return Task.FromResult(this);
            }

            public override void ForEachLeafNode(Action<LeafNode> action)
            {
                action.Invoke(this);
            }

            public override Node DeepClone()
            {
                return new LeafNode(Item);
            }

            public override int CountUnanswered()
            {
                return 0;
            }
        }

        [DataContract]
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

            [DataMember()]
            public Node CompetitorA { get; private set; }
            [DataMember()]
            public Node CompetitorB { get; private set; }
            [DataMember()]
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

            public async override Task<LeafNode> BestNodeAsync(Tournament<T> tournament, IAsyncComparator<T> comparator, CancellationToken cancellationToken)
            {
                if (BestCompetitor == null)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    T itemA = (await CompetitorA.BestNodeAsync(tournament, comparator, cancellationToken)).Item;
                    cancellationToken.ThrowIfCancellationRequested();
                    T itemB = (await CompetitorB.BestNodeAsync(tournament, comparator, cancellationToken)).Item;
                    cancellationToken.ThrowIfCancellationRequested();
                    BestCompetitor = (await comparator.CompareAsync(itemA, itemB, cancellationToken)) > 0 ? CompetitorA : CompetitorB;
                    tournament.NextWinnerStepsDone++;
                    tournament.GlobalStepsDone++;
                }
                return await BestCompetitor.BestNodeAsync(tournament, comparator, cancellationToken);
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

            public override void ForEachLeafNode(Action<LeafNode> action)
            {
                CompetitorA.ForEachLeafNode(action);
                CompetitorB.ForEachLeafNode(action);
            }

            public override Node DeepClone()
            {
                BracketNode clone = new BracketNode(CompetitorA.DeepClone(), CompetitorB.DeepClone());
                if (BestCompetitor == CompetitorA)
                    clone.BestCompetitor = clone.CompetitorA;
                else if (BestCompetitor == CompetitorB)
                    clone.BestCompetitor = clone.CompetitorB;
                return clone;
            }

            public override int CountUnanswered()
            {
                return (BestCompetitor == null ? 1 : 0) + CompetitorA.CountUnanswered() + CompetitorB.CountUnanswered();
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
                    return ((LeafNode)node).Item.ToString();
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

        [DataContract()]
        public class SavedState
        {
            public SavedState(Tournament<T> tournament)
            {
                TotalPlaces = tournament.TotalPlaces;
                RootNode = tournament.rootNode.DeepClone();
                RankingWinners = new List<T>(tournament.rankingWinners);
                NumItems = tournament.NumItems;
                GlobalStepsDone = tournament.GlobalStepsDone;
                NextWinnerStepsDone = tournament.NextWinnerStepsDone;
                NextWinnerStepsMax = tournament.NextWinnerStepsMax;
            }

            public Tournament<T> Instantiate()
            {
                return new Tournament<T>
                {
                    TotalPlaces = TotalPlaces,
                    rootNode = RootNode.DeepClone(),
                    rankingWinners = RankingWinners,
                    NumItems = NumItems,
                    GlobalStepsDone = GlobalStepsDone,
                    NextWinnerStepsDone = NextWinnerStepsDone,
                    NextWinnerStepsMax = NextWinnerStepsMax,
                };
            }

            #region Serialization methods
            private static List<Type> SerializationKnownTypes { get { return new List<Type> { typeof(BracketNode), typeof(LeafNode) }; } }

            public void SaveToFile(string path)
            {
                var serializer = new DataContractSerializer(typeof(SavedState), new DataContractSerializerSettings
                {
                    KnownTypes = SerializationKnownTypes,
                    PreserveObjectReferences = true
                });
                var file = File.Open(path, FileMode.Create);
                try
                {
                    var writer = XmlWriter.Create(file, new XmlWriterSettings
                    {
                        OmitXmlDeclaration = true,
                        Encoding = Encoding.UTF8,
                        Indent = true
                    });
                    serializer.WriteObject(writer, this);
                    writer.Close();
                }
                finally
                {
                    file.Close();
                }
            }

            public static SavedState LoadFromFile(string path)
            {
                var serializer = new DataContractSerializer(typeof(SavedState), new DataContractSerializerSettings
                {
                    KnownTypes = SerializationKnownTypes,
                    PreserveObjectReferences = true
                });
                var file = File.OpenRead(path);
                try
                {
                    var reader = XmlDictionaryReader.CreateTextReader(file, new XmlDictionaryReaderQuotas());
                    var ret = (SavedState) serializer.ReadObject(reader);
                    reader.Close();
                    return ret;
                }
                finally
                {
                    file.Close();
                }
            }
            #endregion

            [DataMember()]
            private readonly int TotalPlaces;
            [DataMember()]
            private readonly Node RootNode;
            [DataMember()]
            private readonly List<T> RankingWinners = new List<T>();
            [DataMember()]
            private int NumItems;
            [DataMember()]
            private int GlobalStepsDone;
            [DataMember()]
            private int NextWinnerStepsMax;
            [DataMember()]
            private int NextWinnerStepsDone;
        }

        public int TotalPlaces;
        private Node rootNode;
        private List<T> rankingWinners = new List<T>();
        public int NumItems;

        public IReadOnlyCollection<T> RankingWinners { get => rankingWinners.AsReadOnly(); }

        public int NextWinnerStepsMax { get; private set; }
        public int NextWinnerStepsDone { get; private set; }

        public int GlobalStepsDone { get; private set; } = 0;
        public int GlobalStepsMax
        {
            get
            {
                var n = NumItems;
                var k = TotalPlaces;
                return (int)Math.Round(n - 1 + (k - 1) * Math.Log(n, 2));
            }
        }

        private Tournament()
        {
            // for instantiation by SavedState only
        }

        internal Tournament(List<T> items, int totalPlaces)
        {
            totalPlaces = Math.Min(totalPlaces, items.Count);
            Debug.Assert(items.Count > 0);
            NumItems = items.Count;
            TotalPlaces = totalPlaces;

            IReadOnlyList<Node> baseLevel = items.Select(item => new LeafNode(item)).ToList();
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

            NextWinnerStepsDone = 0;
            NextWinnerStepsMax = rootNode.CountUnanswered();
        }

        public async Task<List<T>> CalculateTopN(IAsyncComparator<T> comparator, CancellationToken cancellationToken)
        {
            for (int place = 1; place <= TotalPlaces; place++)
            {
                LeafNode winnerNode = await rootNode.BestNodeAsync(this, comparator, cancellationToken);
                T winnerItem = winnerNode.Item;
                NewWinnerEvent?.Invoke(place, winnerItem);
                rankingWinners.Add(winnerItem);
                //System.Console.WriteLine($"Winner: {winnerItem} Comparisons: {comparisonCount}");

                NextWinnerStepsDone = 0;
                NextWinnerStepsMax = rootNode.CountUnanswered();

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

        public SavedState SaveState()
        {
            return new SavedState(this);
        }

        class RiggedAsyncComparator : IAsyncComparator<T>
        {
            public RiggedAsyncComparator(List<int> decisionPath, List<List<T>> itemsShownByDepthLevel, CancellationTokenSource cancellationSource)
            {
                this.decisionPath = decisionPath;
                this.itemsShownByDepthLevel = itemsShownByDepthLevel;
                this.cancellationSource = cancellationSource;
            }
            List<int> decisionPath;
            private readonly List<List<T>> itemsShownByDepthLevel;
            private readonly CancellationTokenSource cancellationSource;
            int nextDecisionIndex = 0;

            public Task<int> CompareAsync(T item, T other, CancellationToken cancellationToken)
            {
                if (!itemsShownByDepthLevel[nextDecisionIndex].Contains(item))
                    itemsShownByDepthLevel[nextDecisionIndex].Add(item);
                if (!itemsShownByDepthLevel[nextDecisionIndex].Contains(other))
                    itemsShownByDepthLevel[nextDecisionIndex].Add(other);

                if (nextDecisionIndex < decisionPath.Count)
                {
                    return Task.FromResult(decisionPath[nextDecisionIndex++]);
                }
                else
                {
                    cancellationSource.Cancel();
                    return Task.FromCanceled<int>(cancellationToken);
                }
            }
        }

        public List<T> PredictItemsWorthPreloading()
        {
            const int maxDepth = 3;
            List<List<T>> itemsShownByDepthLevel = Enumerable.Range(0, maxDepth).Select(_ => new List<T>()).ToList();
            List<List<int>> decisionPaths = new List<List<int>>
            {
                new List<int> { -1, -1 },
                new List<int> { -1,  1 },
                new List<int> {  1, -1 },
                new List<int> {  1,  1 },
            };

            SavedState startState = SaveState();
            foreach (List<int> decisionPath in decisionPaths)
            {
                var cancellationSource = new CancellationTokenSource();
                var task = startState.Instantiate().CalculateTopN(new RiggedAsyncComparator(decisionPath, itemsShownByDepthLevel, cancellationSource),
                    cancellationSource.Token);
                Debug.Assert(task.Status == TaskStatus.RanToCompletion || task.Status == TaskStatus.Canceled);
            }
            return itemsShownByDepthLevel.SelectMany(x => x).ToList();
        }

    }
}
