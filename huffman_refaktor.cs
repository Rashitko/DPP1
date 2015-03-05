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
        /// Kdyz nema jedineho syna vraci true.
        /// </summary>
        /// <returns></returns>
        public bool IsLeaf()
        {
            return (Left == null) && (Right == null);
        }

        /// <summary>
        /// True o sobe Node rekne jestli bude v Huffmanskem strome nalevo od druheho Nodeu.
        /// </summary>
        /// <param name="druhy"></param>
        /// <returns></returns>
        public bool WillBeOnLeft(Node that)
        {
            // if (that.Freq > this.Freq)
            // {
            //     return true;
            // }
            // else if (that.Freq < this.Freq)
            // {
            //     return false;
            // }
            // else if (that.IsLeaf() && !(this.IsLeaf()))
            // {
            //     return false;
            // }
            // else if (this.IsLeaf() && !(that.IsLeaf()))
            // {
            //     return true;
            // }
            // else if ((this.IsLeaf()) && (that.IsLeaf()) && (this.Symbol < that.Symbol))
            // {
            //     return true;
            // }
            // else if ((this.IsLeaf()) && (that.IsLeaf()) && (this.Symbol > that.Symbol))
            // {
            //     return false;
            // }
            // else if (this.SeqNum < that.SeqNum)
            // {
            //     return true;
            // }
            // else
            // {
            //     return false;
            // }

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

        public HuffmanTree(SortedDictionary<int, List<Node>> freqToNodes)
        {
            root = BuildHuffmanTree(freqToNodes);
        }

        private Node BuildHuffmanTree(SortedDictionary<int, List<Node>> HuffmanskyLes)
        {
            int ZbyvaZpracovat = HuffmanskyLes.Sum(item => item.Value.Count);
            while (ZbyvaZpracovat != 1)
            {
                Node u = GetAndDelNextNode(HuffmanskyLes);
                Node v = GetAndDelNextNode(HuffmanskyLes);
                Node father = CreateFather(u, v);
                AppendToDict(HuffmanskyLes, father);
                ZbyvaZpracovat--;
            }
            return HuffmanskyLes[HuffmanskyLes.Keys.ElementAt(0)][0];
        }

        private void AppendToDict(SortedDictionary<int, List<Node>> freqToNodes, Node u)
        {
            if (!freqToNodes.ContainsKey(u.Freq))
            {
                freqToNodes.Add(u.Freq, new List<Node>());
            }
            freqToNodes[u.Freq].Add(u);
        }

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

        private Node CreateFather(Node u, Node v)
        {
            Node left = u.WillBeOnLeft(v) ? u : v;
            Node right = u.WillBeOnLeft(v) ? v : u;
            int newFreq = u.Freq + v.Freq;
            byte symbol = left.Freq == right.Freq ? u.Symbol : left.Symbol;
            return NodeCreator.CreateNode(left, right, newFreq, symbol);
        }
       
        public void VypisStrom()
        {
            // VypisStrom(this.root);
        }

        public void VypisStrom2()
        {
            VypisStrom2(this.root, "");
        }
        
        public void VypisStrom2(Node vrch, string pre)
        {
            bool bylVlevo = false;

            if (vrch.IsLeaf()) {
                if ((vrch.Symbol >= 32) && (vrch.Symbol <= 0x7E))
                {
                    Console.Write(" ['{0}':{1}]\n", (char) vrch.Symbol, vrch.Freq);
                    return;
                }
                else
                {
                    Console.Write(" [{0}:{1}]\n", vrch.Symbol, vrch.Freq);
                }
                return;
            }
            else
            {
                // bylVlevo = true;
            }
                
            if (!bylVlevo)
            {
                Console.Write("{0,4} -+- ", vrch.Freq);
                bylVlevo = true;
            }
            pre = pre + "      ";
            if (bylVlevo)
            {
                VypisStrom2(vrch.Right, pre + "|  ");
                Console.Write("{0}|\n", pre);
                Console.Write("{0}`- ", pre);
                VypisStrom2(vrch.Left, pre + "   ");
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
                SortedDictionary<int, List<Node>> Nodey = new SortedDictionary<int, List<Node>>();
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
                            //   Nodey.Add(prvky[a]);
                        }
                        else
                        {
                            prvky[a].Freq++;
                        }
                    }
                }

                for (int i = 0; i < vstup.Length % 0x4000; i++)
                {
                    a =(byte) vstup.ReadByte();
                    if (prvky[a] == null)
                    {
                        prvky[a] = NodeCreator.CreateNode(null, null, 1, (byte)a);
                        //   Nodey.Add(prvky[a]);
                    }
                    else
                    {
                        prvky[a].Freq++;
                    }
                }

                for (int i = 0; i < 256; i++)
                {
                    if (prvky[i]!= null)
	                {
                        if (Nodey.ContainsKey(prvky[i].Freq))
                        {
                            Nodey[prvky[i].Freq].Add(prvky[i]);
                    }
                    else Nodey.Add(prvky[i].Freq, new List<Node>() { prvky[i] });
                    }
                }
                foreach (KeyValuePair<int,List<Node>> item in Nodey)
                {
                    item.Value.Sort();
                }
                return Nodey;
            }
        }

    }

    class Program
    {
        static SortedDictionary<int, List<Node>> Nodey;
        static HuffmanTree Huffman;
     //   static Stopwatch sw = new Stopwatch();

        static void Main(string[] args)
        {
       //     sw.Start();

            if (args.Length != 1)
            {
                Console.Write("Argument Error");
                Environment.Exit(0);
            }
            Nodey = Nacitacka.PrectiSoubor(args[0]);


            if ((Nodey != null) && (Nodey.Count != 0))
            {
                Huffman = new HuffmanTree(Nodey);
                Huffman.VypisStrom();
                //Console.Write("\n");
                Huffman.VypisStrom2();
                Console.Write("\n");
            }

      /*      sw.Stop();
            string ExecutionTimeTaken = string.Format("Minutes :{0}\nSeconds :{1}\n Mili seconds :{2}", sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.Elapsed.TotalMilliseconds);
            Console.Write(ExecutionTimeTaken);
            Console.ReadKey();

            Console.ReadKey(); */
        }
    }
}
