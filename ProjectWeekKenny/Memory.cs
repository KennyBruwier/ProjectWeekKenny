using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWeekKenny
{
    public class Memory
    {
        private int budget;
        private char currency = '$';
        private string Tekens = "♠♣♦♥A";
        public Memory(int geld)
        {
            budget = geld;
        }
        public int Spelen()
        {
            bool nogeens = true;
            int inzet = 20;
            int dubbels = 2;
            
            Random RandomNumber = new Random();
            List<char> kaarten = new List<char>();

            for (int i = 0; i < dubbels; i++)
            {
                for (int j = 0; j < Tekens.Length; j++)
                {
                    kaarten.Add(Tekens[j]);
                }
            }
            do
            {
                List<char> kaartenGeschud = ShuffleCharList<char>(kaarten);
                char[] kaartenRaden = new char[kaartenGeschud.Count];

                Console.Clear();
                Console.WriteLine("\nWelkom bij MEMORY\n");

                if (budget <= 0)
                {
                    Console.WriteLine("Geen geld meer.");
                    Console.Read();
                    break;
                }

                Console.WriteLine($"Geld = {budget}{currency}");
                Console.WriteLine($"Inzet = {inzet}{currency}");
                nogeens = InputBool("Spelen? j/n");

                if (nogeens)
                {
                    bool geldigeGok = false;
                    bool stoppen = false;
                    int juisteGok = 0;

                    for (int i = 0; i < kaartenGeschud.Count; i++)
                    {
                        Console.Write($"\t {i} ");
                    }
                    Console.WriteLine();
                    for (int i = 0; i < kaartenGeschud.Count; i++)
                    {
                        Console.Write($"\t[{kaartenGeschud[i]}]");
                        kaartenRaden[i] = ' ';
                    }

                    Console.WriteLine("\n\t ONTHOUD DIT!");
                    System.Threading.Thread.Sleep(5000);

                    do
                    {
                        Console.Clear();
                        Console.WriteLine("\nWelkom bij MEMORY\n");
                        for (int i = 0; i < kaartenGeschud.Count; i++)
                        {
                            Console.Write($"\t {i} ");
                        }
                        Console.WriteLine();
                        for (int i = 0; i < kaartenGeschud.Count; i++)
                        {
                            Console.Write($"\t[{kaartenRaden[i]}]");
                        }
                        Console.WriteLine("\n");
                        Console.WriteLine("<'q' = exit>");
                        char cGok1 = InputChr("Kaart 1:");
                        char cGok2 = ' ';
                        if (cGok1 != 'q')
                        {
                            Console.WriteLine();
                            cGok2 = InputChr("Kaart 2:");
                            Console.WriteLine();
                            if (cGok2 == 'q') stoppen = true;
                        }
                        else stoppen = true;
                        if (!stoppen)
                        {
                            int gok1;
                            int gok2;
                            int.TryParse(cGok1.ToString(), out gok1);
                            int.TryParse(cGok2.ToString(), out gok2);

                            if ((gok1 > kaartenGeschud.Count) || (gok2 > kaartenGeschud.Count)) geldigeGok = false; else geldigeGok = true;
                            if (geldigeGok)
                            {
                                if (kaartenGeschud[gok1] == kaartenGeschud[gok2])
                                {
                                    juisteGok++;
                                    kaartenRaden[gok1] = kaartenGeschud[gok1];
                                    kaartenRaden[gok2] = kaartenGeschud[gok1];
                                    if (juisteGok == kaartenGeschud.Count / dubbels)
                                    {
                                        budget += 30;
                                        stoppen = true;
                                        Console.WriteLine("Proficiat! U heeft alles goed geraden!");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Goed gedaan! nog {kaartenGeschud.Count / dubbels - juisteGok} kaarten te gaan!");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("fout!");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Ongeldige gok!");
                            }
                            Console.ReadLine();
                        }
                    } while (!stoppen);
                    nogeens = InputBool("Nog eens? j/n");
                }
            } while (nogeens);
            return budget;
        }
        static bool InputBool(string tekst = "j/n", bool Cyes = true, bool Cno = false)
        {
            //ConsoleKeyInfo keyStrike = new ConsoleKeyInfo();

            Console.WriteLine(tekst);
            ConsoleKeyInfo keyStrike = Console.ReadKey(true);

            switch (Char.ToLower(keyStrike.KeyChar))
            {
                case 'y':
                case 'j': return Cyes;
                case 'n': return Cno;

            }
            return false;

        }
        static char InputChr(string tekst = "Keuze: ")
        {
            Console.Write(tekst);
            ConsoleKeyInfo keyStrike = Console.ReadKey(true);
            return keyStrike.KeyChar;

        }
        static List<char> ShuffleCharList<T>(List<char> list)
        // bron: https://stackoverflow.com/questions/12137127/how-to-shuffle-a-list
        {
            Random rnd = new Random();
            List<char> newList = new List<char>();
            newList = list.OrderBy(x => rnd.Next()).ToList();
            return newList;
        }
    }
}
