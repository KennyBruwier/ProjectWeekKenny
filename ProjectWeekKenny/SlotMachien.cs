using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWeekKenny
{
    public class SlotMachien
    {
        private int budget;
        private char currency = '$';
        public SlotMachien(int geld)
        {
            budget = geld;
        }
        public int Spelen()
        {
            int inzet = 5;
            bool nogEens;
            Random RandomNumber = new Random();

            do
            {
                Console.Clear();
                Console.WriteLine("Welkom bij het slotmachien\n");

                if (budget <= 0)
                {
                    Console.WriteLine("Geen geld meer.");
                    Console.Read();
                    break;
                }

                Console.WriteLine($"Geld: {budget}{currency}");
                Console.WriteLine($"Inzet: {inzet}{currency}");
                Console.WriteLine("Slots laten draaien? j/n");
                string antw = Console.ReadLine();

                if (antw == "n")
                    break;
                else
                {
                    budget -= 5;
                    int[] sloten = new int[9];
                    for (int i = 0; i < sloten.Length; i++)
                    {
                        sloten[i] = RandomNumber.Next(0, 6); 
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        Console.Write($"[{dST(sloten[i])} - {dST(sloten[i+1])} - {dST(sloten[i+2])}]\t<druk een key>\n"); Console.ReadKey();
                    }

                    int vorigeScore = budget;
                    for (int i = 0; i < 3; i++)
                    {
                        if (sloten[i] == sloten[i+1] && sloten[i+1] == sloten[i+2])
                        {
                            switch (sloten[i])
                            {
                                case 0: budget += 3; break;
                                case 1: budget += 5; break;
                                case 2: budget += 7; break;
                                case 3: budget += 10; break;
                                case 4: budget += 20; break;
                                case 5: budget += 50; break;
                                case 6: budget += 100; break;
                                default: break;
                            }
                        }
                    }
                    // diagonaal score moet er nog bij

                    if (vorigeScore < budget)
                        Console.WriteLine($"Je hebt {budget - vorigeScore}{currency} gewonnen!");
                    else
                        Console.WriteLine($"Jammer, je hebt niets gewonnen.");
                }
                nogEens = InputBool("Nog eens spelen? j/n");
            } while (nogEens);

            return budget;
        }
        static char dST(int Value, string Tekens = "☻♠♣♦♥A7", char Error = '◘')
        // display Slot Teken
        {
            if (Value <= Tekens.Length) return Tekens[Value];
            else return Error;
        }
        static bool InputBool(string tekst = "j/n", bool Cyes = true, bool Cno = false)
        {
            ConsoleKeyInfo keyStrike = new ConsoleKeyInfo();

            Console.WriteLine(tekst);
            keyStrike = Console.ReadKey(true);

            switch (Char.ToLower(keyStrike.KeyChar))
            {
                case 'y':
                case 'j': return Cyes;
                case 'n': return Cno;
            }
            return false;

        }
    }
}
