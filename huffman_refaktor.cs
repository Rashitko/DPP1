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
        public bool BudeNodeVlevo(Node druhy)
        {
            if (druhy.Freq > Freq)
            {
                return true;
            }
            else if (druhy.Freq < Freq)
            {
                return false;
            }
            else if (druhy.IsLeaf() && !(IsLeaf()))
            {
                return false;
            }
            else if (IsLeaf() && !(druhy.IsLeaf()))
            {
                return true;
            }
            else if ((IsLeaf()) && (druhy.IsLeaf()) && (Symbol < druhy.Symbol))
            {
                return true;
            }
            else if ((IsLeaf()) && (druhy.IsLeaf()) && (Symbol > druhy.Symbol))
            {
                return false;
            }
            else if (SeqNum < druhy.SeqNum)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        #region IComparable Members

        public int CompareTo(Node obj)
        {
            if (this == obj)
            {
                return 0;
            }
            else if (BudeNodeVlevo(obj))
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

    class strom
    {
        private Node koren;

        public strom(SortedDictionary<int, List<Node>> Nodey)
        {
            postavStrom(Nodey);
        }

        int pocetStromu = 0;

        private void postavStrom(SortedDictionary<int, List<Node>> HuffmanskyLes)
        {
            List<Node> seznam;
                Node pom1;
                Node pom3;
                Node novy;
                Node lichy = null;
                int ZbyvaZpracovat = 0;
                int rank;

                foreach (KeyValuePair<int,List<Node>> item in HuffmanskyLes)
                {
                    ZbyvaZpracovat += item.Value.Count;
                }

            if (ZbyvaZpracovat != 1) {
                pocetStromu = pocetStromu + 1;
            }

            while (ZbyvaZpracovat != 1)
            {
                seznam = HuffmanskyLes[HuffmanskyLes.Keys.ElementAt(0)];
                rank = HuffmanskyLes.Keys.ElementAt(0);

                if (lichy == null)
                {
                    for (int i = 0; i < seznam.Count - 1; i++)
                    {
                        pom1 = seznam[i];
                        pom3 = seznam[++i];

                        if (pom1.BudeNodeVlevo(pom3))
                        {
                            novy = NodeCreator.CreateNode(pom1, pom3, pom1.Freq + pom3.Freq, pom1.Symbol);
                        }
                        else novy = NodeCreator.CreateNode(pom3, pom1, pom1.Freq + pom3.Freq, pom1.Symbol);

                        if (HuffmanskyLes.ContainsKey(novy.Freq))
                        {
                            HuffmanskyLes[novy.Freq].Add(novy);
                        }
                        else HuffmanskyLes.Add(novy.Freq, new List<Node>() { novy });
                        
                        
                        ZbyvaZpracovat--;
                    }
                    if (seznam.Count % 2 == 1)
                    {
                        lichy = seznam[seznam.Count - 1];

                    }
                    else
                    {
                        lichy = null;
                    }

                }
                else 
                {
                    pom1 = seznam[0];
                    if (lichy.BudeNodeVlevo(pom1))
                    {
                        novy = NodeCreator.CreateNode(lichy, pom1, lichy.Freq + pom1.Freq, lichy.Symbol);
                    }
                    else novy = NodeCreator.CreateNode(pom1, lichy, pom1.Freq + lichy.Freq, pom1.Symbol);

                    if (HuffmanskyLes.ContainsKey(novy.Freq))
                    {
                        HuffmanskyLes[novy.Freq].Add(novy);
                    }
                    else HuffmanskyLes.Add(novy.Freq, new List<Node>() { novy });

                    ZbyvaZpracovat--;

                    for (int i = 1; i < seznam.Count - 1; i++)
                    {
                        pom1 = seznam[i];
                        pom3 = seznam[++i];

                        if (pom1.BudeNodeVlevo(pom3))
                        {
                            novy = NodeCreator.CreateNode(pom1, pom3, pom1.Freq + pom3.Freq, pom1.Symbol);
                        }
                        else novy = NodeCreator.CreateNode(pom3, pom1, pom1.Freq + pom3.Freq, pom1.Symbol);

                        if (HuffmanskyLes.ContainsKey(novy.Freq))
                        {
                            HuffmanskyLes[novy.Freq].Add(novy);
                        }
                        else HuffmanskyLes.Add(novy.Freq, new List<Node>() { novy });

                        ZbyvaZpracovat--;
                    }
                    if (seznam.Count % 2 == 0)
                    {
                        lichy = seznam[seznam.Count - 1];
                    }
                    else lichy = null;
                }
                HuffmanskyLes.Remove(rank);
            }
            koren = HuffmanskyLes[HuffmanskyLes.Keys.ElementAt(0)][0];
        }
       
        public void VypisStrom()
        {
            // VypisStrom(this.koren);
        }

        public void VypisStrom2()
        {
            VypisStrom2(this.koren, "");
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
        static strom Huffman;
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
                Huffman = new strom(Nodey);
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
