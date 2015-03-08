using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace HuffmanskeKapky
{

    class NodeCreator
    {
        static int nodesNumber = 0;

        public static Node CreateNode(Node left, Node right, int freq, byte symbol)
        {
            nodesNumber++;
            return new Node(nodesNumber, left, right, freq, symbol);
        }
    }
    class Node : IComparable<Node>
    {
        public int Freq { get; set; }
        public byte Symbol { get; private set; }
        public int SeqNum { get; private set; }
        public Node Left { get; private set; }
        public Node Right { get; private set; }

        public Node(int seqNum, Node left, Node right, int freq, byte symbol)
        {
            this.SeqNum = seqNum;
            this.Left = left;
            this.Right = right;
            this.Freq = Freq;
            this.Symbol = symbol;
        }

        /// <summary>
        /// Kdyz nema jedineho syna vraci true.
        /// </summary>
        /// <returns></returns>
        public bool IsLeaf()
        {
            return (Left == null) && (Right == null);
        }

        /// <summary>
        /// True o sobe vrchol rekne jestli bude v Huffmanskem strome nalevo od druheho vrcholu.
        /// </summary>
        /// <param name="druhy"></param>
        /// <returns></returns>
        public bool IsOnLeftOf(Node that)
        {
            if (that.Freq != this.Freq)
            {
                return that.Freq > this.Freq;
            }

            bool exactlyOneLeaf = that.IsLeaf() != this.IsLeaf();
            if (exactlyOneLeaf)
            {
                return IsLeaf();
            }

            bool bothLeafs = that.IsLeaf() && this.IsLeaf();
            if (bothLeafs)
            {
                return that.Symbol > this.Symbol;
            }

            return that.SeqNum > this.SeqNum;
        }


        #region IComparable Members

        public int CompareTo(Node that)
        {
            if (this == that)
            {
                return 0;
            }
            if (IsOnLeftOf(that))
            {
                return -1;
            }
            return 1;

        }

        #endregion
    }

    class HuffmanTree
    {
        private Node root;

        public HuffmanTree(SortedDictionary<int, List<Node>> nodes)
        {
            root = BuildHuffmanTree(nodes);
        }

        private Node BuildHuffmanTree(SortedDictionary<int, List<Node>> nodes)
        {
            int nodesLeft = countNodes(nodes);
            // continue untill you have only one node, which is the root of the tree
            while (nodesLeft > 1)
            {
                Node u = getAndDelNextNode(nodes);
                Node v = getAndDelNextNode(nodes);
                Node father = CreateFather(u, v);
                appendToNodesList(nodes, father);
                nodesLeft--;
            }
            return nodes[nodes.Keys.First()].First();
        }


        private Node getAndDelNextNode(SortedDictionary<int, List<Node>> nodes)
        {
            int lowestFreq = nodes.Keys.First();
            Node next = nodes[lowestFreq].First();
            nodes[lowestFreq].Remove(next);

            // if the list is empty, delete it from the dictionary
            if (nodes[lowestFreq].Count == 0)
            {
                nodes.Remove(lowestFreq);
            }
            return next;
        }

        private void appendToNodesList(SortedDictionary<int, List<Node>> nodes, Node father)
        {
            if (!nodes.ContainsKey(father.Freq))
            {
                nodes.Add(father.Freq, new List<Node>());
            }
            nodes[father.Freq].Add(father);
        }

        private Node CreateFather(Node u, Node v)
        {
            int newFreq = u.Freq + v.Freq;
            Node left = u.IsOnLeftOf(v) ? u : v;
            Node right = u.IsOnLeftOf(v) ? v : u;
            // if frequencies are the same, always pick the symbol of the node in the first parameter, otherwise
            // pick the symbol of the node who is on the left of the other given node
            byte symbol = left.Freq == right.Freq ? u.Symbol : left.Symbol;

            return NodeCreator.CreateNode(left, right, newFreq, symbol);
        }


        private int countNodes(SortedDictionary<int, List<Node>> nodes)
        {
            int result = 0;
            foreach (KeyValuePair<int, List<Node>> item in nodes)
            {
                result += item.Value.Count;
            }
            return result;
        }


        public void PrintTree()
        {
            PrintTreeWithPrefix(root, "");
        }

        private void PrintLeaf(Node leaf)
        {
            if ((leaf.Symbol >= 32) && (leaf.Symbol <= 0x7E))
            {
                Console.Write(" ['{0}':{1}]\n", (char)leaf.Symbol, leaf.Freq);
            }
            else
            {
                Console.Write(" [{0}:{1}]\n", leaf.Symbol, leaf.Freq);
            }
        }
        public void PrintTreeWithPrefix(Node u, string prefix)
        {
            if (u.IsLeaf())
            {
                PrintLeaf(u);
            }
            else
            {
                Console.Write("{0,4} -+- ", u.Freq);
                prefix = prefix + "      ";
                PrintTreeWithPrefix(u.Right, prefix + "|  ");
                Console.Write("{0}|\n", prefix);
                Console.Write("{0}`- ", prefix);
                PrintTreeWithPrefix(u.Left, prefix + "   ");
            }
        }
    }

    class Nacitacka
    {
        private static FileStream vstup;

        public static bool OtevrSoubor(string nazev)
        {
            try
            {
                vstup = new FileStream(nazev, FileMode.Open, FileAccess.Read);
                if (!(vstup.CanRead))
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                Console.Write("File Error");
                Environment.Exit(0);
                //    return false;
            }
            return true;
        }

        public static SortedDictionary<int, List<Node>> PrectiSoubor(string nazev)
        {

            if (!(OtevrSoubor(nazev))) return null;
            else
            {
                SortedDictionary<int, List<Node>> vrcholy = new SortedDictionary<int, List<Node>>();
                byte a = 0;

                Node[] prvky = new Node[256];
                byte[] bafr = new byte[0x4000];

                for (int i = 0; i < vstup.Length / 0x4000; i++)
                {
                    vstup.Read(bafr, 0, 16384);

                    for (int j = 0; j < 16384; j++)
                    {
                        a = bafr[j];
                        if (prvky[a] == null)
                        {
                            prvky[a] = NodeCreator.CreateNode(null, null, 1, (byte)a);
                            //   vrcholy.Add(prvky[a]);
                        }
                        else
                        {
                            prvky[a].Freq++;
                        }
                    }
                }

                for (int i = 0; i < vstup.Length % 0x4000; i++)
                {
                    a = (byte)vstup.ReadByte();
                    if (prvky[a] == null)
                    {
                        prvky[a] = NodeCreator.CreateNode(null, null, 1, (byte)a);
                        //   vrcholy.Add(prvky[a]);
                    }
                    else
                    {
                        prvky[a].Freq++;
                    }
                }

                for (int i = 0; i < 256; i++)
                {
                    if (prvky[i] != null)
                    {
                        if (vrcholy.ContainsKey(prvky[i].Freq))
                        {
                            vrcholy[prvky[i].Freq].Add(prvky[i]);
                        }
                        else vrcholy.Add(prvky[i].Freq, new List<Node>() { prvky[i] });
                    }
                }
                foreach (KeyValuePair<int, List<Node>> item in vrcholy)
                {
                    item.Value.Sort();
                }
                return vrcholy;
            }
        }

    }

    class Program
    {
        static SortedDictionary<int, List<Node>> vrcholy;
        static HuffmanTree Huffman;

        static void Main(string[] args)
        {

            if (args.Length != 1)
            {
                Console.Write("Argument Error");
                Environment.Exit(0);
            }
            vrcholy = Nacitacka.PrectiSoubor(args[0]);


            if ((vrcholy != null) && (vrcholy.Count != 0))
            {
                Huffman = new HuffmanTree(vrcholy);
                Huffman.PrintTree();
                Console.Write("\n");
            }

        }
    }
}