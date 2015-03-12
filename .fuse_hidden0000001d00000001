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
        private static int nodesCount = 0;
        public static Node CreateNode(Node left, Node right, int freq, byte symbol)
        {
            nodesCount++;
            return new Node(nodesCount, left, right, freq, symbol);
        }
    }

    class Node: IComparable<Node>
    {
	    public int SeqNum {get; private set;}        
        public Node Left {get; private set;}
        public Node Right {get; private set;}
	    public int Freq {get; set;}
        public byte Symbol {get; private set;}
        
        public Node(int seqNum, Node left, Node right, int freq, byte symbol)
        {
	    this.SeqNum = seqNum;
            this.Left = left;
            this.Right = right;
            this.Freq = freq;
            this.Symbol = symbol;
        }

        /// <summary>
        /// Kdyz nema jedineho syna vraci true
        /// </summary>
        /// <returns></returns>
        public bool IsLeaf()
        {
            return (Left == null) && (Right == null);
        }

        /// <summary>
        /// Check if node will be on the left of the provided node 
        /// </summary>
        /// <param name="that">Provided node</param>
        /// <returns>True if this node is on the left</returns>
        public bool WillBeOnLeft(Node that)
        {
            if (that.Freq != this.Freq)
            {
                return that.Freq > this.Freq;
            }
            
            bool exactlyOneLeaf = that.IsLeaf() != this.IsLeaf();
            if (exactlyOneLeaf)
            {
                return this.IsLeaf();
            }
            
            bool bothLeafs = that.IsLeaf() && this.IsLeaf();
            if (bothLeafs)
            {
                return that.Symbol > this.Symbol;
            }
            
            return that.SeqNum > this.SeqNum;
        }

        /// <summary>Creates string representation of path to node and node itself</summary>
        /// <returns>String representation of path to node and node itself</returns>
        public override string ToString()
        {
            const string CONTROL_SYMBOL_FORMAT = " [{0}:{1}]\n";
            const string PRINTABLE_SYMBOL_FORMAT = " ['{0}':{1}]\n";
            if ( !char.IsControl((char) Symbol))
            {
                return string.Format(PRINTABLE_SYMBOL_FORMAT, (char) this.Symbol, this.Freq);
            }
            return string.Format(CONTROL_SYMBOL_FORMAT, this.Symbol, this.Freq);
        }


        #region IComparable Members

        public int CompareTo(Node that)
        {
            if (this == that)
            {
                return 0;
            }
            else if (this.WillBeOnLeft(that))
            {
                return -1;
            }
            else 
            {
                return 1;
            }
            
        }

        #endregion
    }

    class HuffmanTree
    {
        private Node root;

        ///<summary>Creates Huffman tree from provided dictionary. Dictionary must not be empty</summary>
        public HuffmanTree(SortedDictionary<int, List<Node>> freqToNodes)
        {
            root = BuildHuffmanTree(freqToNodes);
        }

        /// <summary>Creates SortedDictionary where key is frequency and value is the list of nodes with frequencies equal to key</summary>
        /// <param name="nodes">Array of nodes</param>
        /// <returns>SortedDictionary where key is frequency and value is the list of nodes</returns>
        public static SortedDictionary<int, List<Node>> CreateFreqToNodesFromNodes(Node[] nodes)
        {
            SortedDictionary<int, List<Node>> freqToNodes = new SortedDictionary<int, List<Node>>();
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i]!= null)
                {
                    HuffmanTree.AppendToDict(freqToNodes, nodes[i]);
                }
            }
            return freqToNodes;
        }

        ///<summary>Creates Huffman tree from provided SortedDictionary where key is frequency and value is the list of nodes</summary>
        ///<param name="freqToNodes">SortedDictionary where key is frequency and value is the list of nodes</param>
        ///<returns>Root of the created tree</returns>
        private Node BuildHuffmanTree(SortedDictionary<int, List<Node>> freqToNodes)
        {
            int leftNodes = freqToNodes.Sum(item => item.Value.Count);
            while (leftNodes > 1)
            {
                Node u = GetAndDelNextNode(freqToNodes);
                Node v = GetAndDelNextNode(freqToNodes);
                Node father = CreateFather(u, v);
                AppendToDict(freqToNodes, father);
                leftNodes--;
            }
            return freqToNodes[freqToNodes.Keys.ElementAt(0)][0];
        }

        ///<summary>Appends provided node to dictionary</summary>
        ///<param name="freqToNodes">Dictionary where the node should be inserted</param>
        ///<param name="u">Node to be inserted</param>
        public static void AppendToDict(SortedDictionary<int, List<Node>> freqToNodes, Node u)
        {
            if (!freqToNodes.ContainsKey(u.Freq))
            {
                freqToNodes.Add(u.Freq, new List<Node>());
            }
            freqToNodes[u.Freq].Add(u);
        }

        ///<summary>Extractes node with the lowest frequency from the dictionary</summary>
        ///<param name="freqToNodes">Dictionary from which the node should be extracted</param>
        ///<returns>Extracted node with the lowest frequency</returns>
        private Node GetAndDelNextNode(SortedDictionary<int, List<Node>> freqToNodes)
        {
            int lowestFreq = freqToNodes.Keys.First();
            Node next = freqToNodes[lowestFreq].First();
            freqToNodes[lowestFreq].Remove(next);
            if (freqToNodes[lowestFreq].Count == 0) {
                freqToNodes.Remove(lowestFreq);
            }
            return next;
        }

        ///<summary>Creates father of two nodes, whose frequency is equal to sum of the frequencies of his sons</summary>
        ///<param name="u">Node</param>
        ///<param name="v">Node</param>
        ///<returns>Created father</returns>
        private Node CreateFather(Node u, Node v)
        {
            Node left = u.WillBeOnLeft(v) ? u : v;
            Node right = u.WillBeOnLeft(v) ? v : u;
            int newFreq = u.Freq + v.Freq;
            byte symbol = left.Freq == right.Freq ? u.Symbol : left.Symbol;
            return NodeCreator.CreateNode(left, right, newFreq, symbol);
        }
       
        ///<summary>Prints Huffman tree</summary>
        public void PrintTree()
        {
            PrintTreeWithPrefix(root, "");
        }
        
        ///<summary>Prints Huffman tree with root in provided node with provided prefix</summary>
        ///<param name="u">Root from which to start</params>
        ///<param name="prefix">Prefix</params>
        private void PrintTreeWithPrefix(Node u, string prefix)
        {
            if (u.IsLeaf()) {
                Console.Write(u.ToString());
            } else {
                const string PADDING = "      ";
                const string ADDITION_TO_LEFT_PREFIX = "   ";
                const string ADDITION_TO_RIGHT_PREFIX = "|  ";
                const string CURRENT_NODE_OUTPUT_FORMAT = "{0,4} -+- ";
                const string SON_PATH_FORMAT = "{0}|\n";
                const string NEW_NODE_PREFIX_FORMAT = "{0}`- ";
                prefix = prefix + PADDING;

                Console.Write(CURRENT_NODE_OUTPUT_FORMAT, u.Freq);

                PrintTreeWithPrefix(u.Right, prefix + ADDITION_TO_RIGHT_PREFIX);

                Console.Write(SON_PATH_FORMAT, prefix);
                Console.Write(NEW_NODE_PREFIX_FORMAT, prefix);

                PrintTreeWithPrefix(u.Left, prefix + ADDITION_TO_LEFT_PREFIX);
            }
        }
    }

    class FileReader
    {
        private const int NODES_SIZE = 256;
        private const int BUFFER_SIZE = 16384;

        ///<summary>Checks if the file with provided name can be read</summary>
        ///<param name="fileName">File name</params>
        ///<returns>True if the file can be read</returns>
        public static bool IsFileReadable(string fileName)
        {
            try 
                {
                using (FileStream stream = new FileStream(fileName, FileMode.Open)) 
                {
                    return stream.CanRead;
                }
            }
            catch 
            { 
                return false; 
            }
        }

        ///<summary>Creates dictionary based on content of file. File must be readable</summary>
        ///<param name="fileName">File name</params>
        ///<returns>SortedDictionary where key is frequency and value is the list of nodes</returns>
        public static SortedDictionary<int, List<Node>> GetFreqToNodesFromFile(string fileName)
        {
            Node[] nodes = ReadNodesFromFile(fileName);
            return HuffmanTree.CreateFreqToNodesFromNodes(nodes);
        }

        ///<summary>Create nodes array from file. File must be readable</summary>
        ///<param name="fileName">File name</params>
        ///<returns>Loaded array of nodes</returns>
        private static Node[] ReadNodesFromFile(string fileName)
        {
            byte[] rawBytes = File.ReadAllBytes(fileName);
            Node[] nodes = new Node[NODES_SIZE];
            foreach (byte readByte in rawBytes)
            {
                AppendByteToNodes(readByte, nodes);
            }
            return nodes;
        }

        ///<summary>If node with given symbol does not exist new one is created, otherwise frequency of existing node is incremeted</summary>
        ///<param name="symbol">Symbol</params>
        ///<param name="nodes">Nodes</params>
        private static void AppendByteToNodes(byte symbol, Node[] nodes)
        {
            if (nodes[symbol] == null)
            {
                nodes[symbol] = NodeCreator.CreateNode(null, null, 1, (byte)symbol);
            }
           else
            {
                nodes[symbol].Freq++;
            }
        }

    }

    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length != 1)
            {
                Console.Write("Argument Error");
                Environment.Exit(0);
            }
            string fileName = args[0];
            if (!FileReader.IsFileReadable(fileName)) 
            {
                Console.Write("File Error");
                Environment.Exit(0);
            }
            SortedDictionary<int, List<Node>> freqToNodes = FileReader.GetFreqToNodesFromFile(fileName);
            bool freqToNodesNotEmpty = (freqToNodes != null) && (freqToNodes.Count != 0);
            if (freqToNodesNotEmpty)
            {
                const string NEW_LINE = "\n";
                HuffmanTree huffmanTree = new HuffmanTree(freqToNodes);
                huffmanTree.PrintTree();
                Console.Write(NEW_LINE);
            }

        }
    }
}
