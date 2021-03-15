using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWeekKenny
{
    /* 
     *      bron: https://codereview.stackexchange.com/questions/60314/console-blackjack-game
    */
    public enum Achterkant
    {
        Harten,
        Ruit,
        Schuppen,
        Klavers
    }

    public enum Voorkant
    {
        Aas,
        Twee,
        Drie,
        Vier,
        Vijf,
        Zes,
        Zeven,
        Acht,
        Negen,
        Tien,
        Boer,
        Dame,
        Koning,
    }


    public class Kaart
    {
        public Achterkant Achterkant { get; set; }
        public Voorkant Voorkant { get; set; }
        public int Waarde { get; set; }
    }

    public class Boek
    {
        private List<Kaart> kaarten;
        public Boek()
        {
            this.Opstarten();
        }
        public void Opstarten()
        {
            kaarten = new List<Kaart>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    kaarten.Add(new Kaart() { Achterkant = (Achterkant)i, Voorkant = (Voorkant)j });
                    if (j <= 8)
                        kaarten[kaarten.Count - 1].Waarde = j + 1;
                    else
                        kaarten[kaarten.Count - 1].Waarde = 10;
                }
            }
        }
        public void Schudden()
        {
            Random rnd = new Random();
            int teller = kaarten.Count;
            while (teller > 1)
            {
                int r = rnd.Next(--teller + 1);
                Kaart kaart = kaarten[r];
                kaarten[r] = kaarten[teller];
                kaarten[teller] = kaart;
            }
        }

        public Kaart TrekEenKaart()
        {
            if (kaarten.Count <= 0)
            {
                this.Opstarten();
                this.Schudden();
            }

            Kaart kaartTerugTeGeven = kaarten[kaarten.Count - 1];
            kaarten.RemoveAt(kaarten.Count - 1);
            return kaartTerugTeGeven;
        }
        public int AantalKaartenOver()
        {
            return kaarten.Count;
        }
        public void PrintBoek()
        {
            int i = 1;
            foreach (Kaart kaart in kaarten)
            {
                Console.WriteLine($"Kaart {i++}: | {kaart.Voorkant} | {kaart.Achterkant} | met waarde: {kaart.Waarde}");
            }
        }
    }

    public class BlackJack
    {
        private int budget;
        private int inzet;
        static List<Kaart> spelersHand;
        static List<Kaart> dealerHand;
        public BlackJack(int geld, int prijsPerBeurt = 10)
        {
            budget = geld;
            inzet = prijsPerBeurt;
        }

        public int Spelen()
        {
            bool nogEensSpelen = true;
            Boek boekKaarten = new Boek();

            Console.Clear();
            Console.WriteLine("Welkom bij Blackjack!\n");
            boekKaarten.Schudden();

            while ((budget > 0) && nogEensSpelen)
            {
                spelersHand = new List<Kaart>();
                spelersHand.Add(boekKaarten.TrekEenKaart());
                spelersHand.Add(boekKaarten.TrekEenKaart());
                int totSpeler = 0;
                bool nogEenKaart = false;

                do
                {
                    Console.Clear();
                    Console.WriteLine("Welkom bij Blackjack!\n");
                    Console.WriteLine("Jou kaarten zijn:\n");
                    totSpeler = 0;
                    int teller = 0;
                    int tellerAce = 0;
                    if (nogEenKaart) spelersHand.Add(boekKaarten.TrekEenKaart());
                    foreach (Kaart kaart in spelersHand)
                    {
                        if (kaart.Voorkant == Voorkant.Aas)
                        {
                            tellerAce++;
                            kaart.Waarde = 11;
                            //if (totSpeler + 11 < 22) kaart.Waarde = 11;
                            //else kaart.Waarde = 1;
                        }
                        totSpeler += kaart.Waarde;
                        Console.WriteLine($"Kaart {++teller}:\t{kaart.Voorkant}\t{kaart.Achterkant}\twaarde: {kaart.Waarde}");
                    }
                    for (int i = tellerAce; i > 0; i--)
                    {
                        if (totSpeler > 21) totSpeler -= 10;
                    }
                    Console.WriteLine($"Totaal:\t{totSpeler}\n");
                    if (totSpeler < 21) nogEenKaart = InputBool("Nog een kaart? j/n");
                    else Console.ReadLine();
                   
                } while ((totSpeler < 21) && nogEenKaart);

                if (totSpeler < 21)
                {
                    dealerHand = new List<Kaart>();
                    int totDealer = 0;
                    Console.WriteLine("Dealer kaarten:");

                    do
                    {
                        dealerHand.Add(boekKaarten.TrekEenKaart());
                        totDealer += dealerHand[dealerHand.Count-1].Waarde;
                        Console.WriteLine($"Kaart {dealerHand.Count}: {dealerHand[dealerHand.Count - 1].Voorkant} | {dealerHand[dealerHand.Count - 1].Achterkant} | waarde: {dealerHand[dealerHand.Count - 1].Waarde}");
                        Console.ReadKey();
                    } while (totDealer < 16);

                    Console.WriteLine($"Totaal: {totDealer}\n");
                    Console.WriteLine($"{totSpeler} vs {totDealer}");

                    if ((totSpeler>totDealer)||(totDealer > 21))
                    {
                        Console.WriteLine("U heeft 20$ gewonnen!");
                        budget += 20;
                    }
                    else if (totDealer == totSpeler)
                        Console.WriteLine("Gelijk!");
                    else 
                    { 
                        Console.WriteLine($"U heeft {inzet} verloren!");
                        budget -= inzet;
                    }
                }
                else if (totSpeler == 21)
                {
                    Console.WriteLine("U heeft 25$ gewonnen! proficiat.");
                    Console.ReadLine();
                    budget += 25;
                }
                else
                {
                    Console.WriteLine("U heeft meer dan 21! 10$ verloren");
                    nogEenKaart = false;
                    budget -= 10;
                }
                Console.WriteLine($"Uw budget: {budget}");
                nogEensSpelen = InputBool("Nog een potje BlackJack? j/n");
            } 
            if (budget < 10)
            {
                Console.WriteLine("Geen geld meer.");
                Console.ReadKey();
            }
            return budget;
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
