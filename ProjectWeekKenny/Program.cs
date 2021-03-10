using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace ProjectWeekKenny
{
    /* todo:
     *      Error handeling,
     *      card colors,
     *      Afwerking,
     *      Vereenvoudigen,
     *      Wauw effect
     */
    enum MenuUitgelogd
    {
        aanmaken = 1,
        inloggen = 2,
        exit = 3
    }

    enum MenuIngelogd
    {
        bewerken = 1,
        verwijderen = 2,
        spelen = 3,
        logout = 4,
        exit = 5
    }
    enum GamesMenu
    {
        blackjack = 1,
        slotmachien = 2,
        memory = 3,
        exit = 4
    }

    
    class Program
    {
        private RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();

        static void Main(string[] args)
        {
            // -- game instellingen

            string bestandsnaam = "Gegevens.txt";
            int startBudget = 200;
            char seperator = '#';
            char currency = '$';

            // -- wachtwoord instellingen

            string encryptKey = "ZYXWVUTSRQPONMLKJIHGFEDCBAzyxwvutsrqponmlkjihgfedcba9876543210";
            int aantalHLetters = 1; // aantal hoofdletters
            int aantalKLetters = 1; // aantal kleinletters
            int aantalCijfers = 1;
            int aantalVreemdeLetters = 1;
            int minLengte = 8;
            int maxLengte = 20;

            // -- system vars

            int budget = 0;
            bool geldigWachtwoord;
            bool ingelogd = false;
            bool exit = false;
            string errorMsg = null;
            string userName = null;
            string wachtwoord = "";
            string currentDir = Directory.GetCurrentDirectory();
            MenuUitgelogd keuzeUitgelogd = MenuUitgelogd.exit;
            MenuIngelogd keuzeIngelogd = MenuIngelogd.exit;
            DateTime tijdGestart = DateTime.UtcNow;
            TimeSpan tijdOnline;
            string huidigeTijd;

            do
            {
                do // eerste menu
                {
                    Console.Clear();
                    Console.WriteLine("Welkom in Project 0!\n\n");
                    Console.Write(  "1. Gebruiker aanmaken\n" +
                                    "2. Inloggen\n" +
                                    "3. Exit\n\n");

                    keuzeUitgelogd = (MenuUitgelogd)InputInt("Keuze: ");

                    if (keuzeUitgelogd != MenuUitgelogd.exit)
                    {
                        do 
                        {
                            Console.Clear();
                            if (errorMsg != null)
                                Console.Write($"{errorMsg}\n");

                            if (userName == null)
                                userName = InputStr("Gebruikersnaam:\t", true, false);
                            else
                                Console.WriteLine($"Gebruikersnaam:\t{userName}");

                            wachtwoord = InputStr("Wachtwoord:\t");
                            errorMsg = CheckWachtwoord(wachtwoord, minLengte, maxLengte, aantalHLetters, aantalKLetters, aantalCijfers, aantalVreemdeLetters);

                            if (errorMsg == null)
                                geldigWachtwoord = true;
                            else
                                geldigWachtwoord = false;
                        } while (!geldigWachtwoord);

                        bool nogEens = false;
                        do
                        {
                            switch (keuzeUitgelogd)
                            {
                                case MenuUitgelogd.aanmaken:
                                    {
                                        if (SearchDataInRecord(bestandsnaam, userName) == userName)
                                        {
                                            string voirgeUserName = userName;
                                            do
                                            {
                                                Console.WriteLine("Usernaam bestaat al, gelieve een ander te kiezen");
                                                userName = InputStr("Usernaam: ", true);
                                            } while (voirgeUserName == userName);
                                        }
                                        if (WriteDataInRecord(bestandsnaam, userName, seperator, true, EncryptStr(wachtwoord, encryptKey), Convert.ToString(startBudget)))
                                            Console.WriteLine("Account toegevoegd");
                                        else
                                            Console.WriteLine("Niet gelukt account aan te maken");
                                        Console.ReadLine();
                                        ingelogd = true;
                                        nogEens = false;
                                        exit = false;
                                    }
                                    break;
                                case MenuUitgelogd.inloggen:
                                    {
                                        string gevondenWachtwoord = SearchDataInRecord(bestandsnaam, userName, 1);
                                        switch (gevondenWachtwoord)
                                        {
                                            case null: // file niet gevonden
                                            case "0": // record niet gevonden
                                                {
                                                    if (InputBool("Username niet gevonden.\nAanmaken? (j/n)\n"))
                                                    {
                                                        keuzeUitgelogd = MenuUitgelogd.aanmaken;
                                                        nogEens = true;
                                                    }
                                                }
                                                break;
                                            default: // record gevonden
                                                {
                                                    if (wachtwoord == EncryptStr(gevondenWachtwoord, encryptKey))
                                                    {
                                                        nogEens = false;
                                                        exit = false;
                                                        ingelogd = true;
                                                    }
                                                    else
                                                    {
                                                        if (InputBool(  "Account gevonden maar wachtwoord komt niet overeen.\n" +
                                                                        "Nieuwe account aanmaken? (j/n)\n"))
                                                        {
                                                            keuzeUitgelogd = MenuUitgelogd.aanmaken; // 
                                                            nogEens = true;
                                                        }
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                    break;
                                case MenuUitgelogd.exit:
                                    {
                                        nogEens = false;
                                        ingelogd = false;
                                        exit = true;
                                    }
                                    break;
                            }
                        } while (nogEens);
                    }
                    else
                    {
                        exit = true;
                        ingelogd = false;
                    }

                } while (!ingelogd && !exit);

                if (ingelogd && !exit)
                {
                    
                    do // tweede menu 
                    {
                        Console.Clear();
                        tijdOnline = DateTime.UtcNow - tijdGestart;
                        huidigeTijd = DateTime.UtcNow.ToString("dddd MM yyyy - H:mm ");

                        budget = Convert.ToInt32(SearchDataInRecord(bestandsnaam, userName, 2));
                        Console.WriteLine($"Welkom {userName}!\t Uw budget: {budget}{currency}\n");
                        Console.WriteLine($"{huidigeTijd}\t{Math.Round(tijdOnline.TotalMinutes,0)} minuten {Math.Round(tijdOnline.TotalSeconds, 0)} seconden online\n");
                        Console.Write(  "1. Gebruiker bewerken\n" +
                                        "2. Gebruiker verwijderen\n" +
                                        "3. Spelen\n" +
                                        "4. Uitloggen\n" +
                                        "5. Exit\n\n");

                        keuzeIngelogd = (MenuIngelogd)InputInt("Keuze: ");

                        switch (keuzeIngelogd)
                        {

                            case MenuIngelogd.bewerken:
                                {
                                    char antw;
                                    bool accountVeranderd = false;
                                    string vorigeUsername = userName;
                                    do
                                    {
                                        Console.Clear();
                                        Console.WriteLine("BEWERKEN:\n" +
                                                            "1. Naam\n" +
                                                            "2. Wachtwoord\n" +
                                                            "3. Exit\n");
                                        antw = InputChr("Keuze: ");
                                        switch (antw)
                                        {
                                            case '1':
                                                {
                                                    Console.Clear();
                                                    Console.WriteLine($"Huidige naam: {userName}");
                                                    userName = InputStr("Nieuwe naam: ");
                                                    accountVeranderd = true;
                                                }
                                                break;
                                            case '2':
                                                {
                                                    geldigWachtwoord = false;

                                                    do
                                                    {
                                                        Console.Clear();
                                                        wachtwoord = InputStr("Nieuw wachtwoord: ");
                                                        errorMsg = CheckWachtwoord(wachtwoord, minLengte, maxLengte, aantalHLetters, aantalKLetters, aantalCijfers, aantalVreemdeLetters);
                                                        if (errorMsg == null)
                                                            geldigWachtwoord = true;
                                                        else
                                                            geldigWachtwoord = false;
                                                    } while (!geldigWachtwoord);
                                                    accountVeranderd = true;
                                                }
                                                break;
                                            case '3':
                                                break;
                                            default:
                                                {
                                                    Console.WriteLine("Verkeerde keuze!");
                                                    Console.ReadLine();
                                                }
                                                break;
                                        }

                                    } while (antw != '3');


                                    if (accountVeranderd)
                                    {
                                        budget = Convert.ToInt32(SearchDataInRecord(bestandsnaam, vorigeUsername, 2));
                                        bool vorigeIsWeg = DeleteRecordInFile(bestandsnaam, vorigeUsername);
                                        bool nieuweIsWeg = WriteDataInRecord(bestandsnaam, userName, seperator, true, EncryptStr(wachtwoord, encryptKey), Convert.ToString(budget));
                                        if (vorigeIsWeg && nieuweIsWeg) Console.WriteLine("Account aangepast!");
                                        else Console.WriteLine("Account niet gelukt om aan te passen");
                                        Console.ReadLine();
                                    }

                                }
                                break;
                            case MenuIngelogd.verwijderen:
                                {
                                    bool accBijgewerkt = DeleteRecordInFile(bestandsnaam, userName);
                                    if (accBijgewerkt)
                                        Console.WriteLine("Account verwijderd");
                                    else
                                        Console.WriteLine("Account niet gelukt om te verwijderen");
                                    Console.ReadLine();
                                    ingelogd = false;
                                    exit = false;
                                }
                                break;
                            case MenuIngelogd.spelen:
                                {
                                    ingelogd = true;
                                    exit = false;
                                }
                                break;
                            case MenuIngelogd.exit:
                                {
                                    ingelogd = false;
                                    exit = true;
                                }
                                break;
                            default:
                                    ingelogd = false;
                                break;
                        }
                        if (!ingelogd) Console.ReadLine();
                    } while (keuzeIngelogd == MenuIngelogd.bewerken);
                }


                if (!exit && ingelogd)
                {
                    budget = Convert.ToInt32(SearchDataInRecord(bestandsnaam, userName, 2));
                    GamesMenu gameKeuze;

                    do
                    {
                        tijdOnline = DateTime.UtcNow - tijdGestart;
                        huidigeTijd = DateTime.UtcNow.ToString("dddd MM yyyy - H:mm ");
                        Console.Clear();
                        Console.WriteLine($"Welkom {userName}!\tScore: {budget}{currency}\n\n");
                        Console.WriteLine($"{huidigeTijd}\t{Math.Round(tijdOnline.TotalMinutes, 0)} minuten {Math.Round(tijdOnline.TotalSeconds, 0)} seconden online\n");
                        gameKeuze = (GamesMenu)InputInt(    $"1. Blackjack\n" +
                                                            $"2. Slot machien\n" +
                                                            $"3. Geheugen\n" +
                                                            $"4. Save & exit\n");

                        switch (gameKeuze)
                        {
                            case GamesMenu.blackjack:
                                {
                                    BlackJack myBlackJack = new BlackJack(budget);
                                    budget = myBlackJack.Spelen();
                                }
                                break;
                            case GamesMenu.slotmachien:
                                {
                                    SlotMachien mySlotmachien = new SlotMachien(budget);
                                    budget = mySlotmachien.Spelen();
                                }
                                break;
                            case GamesMenu.memory:
                                {
                                    Memory myMemory = new Memory(budget);
                                    budget = myMemory.Spelen();
                                }
                                break;
                            case GamesMenu.exit:
                                {
                                    bool deleteRec = DeleteRecordInFile(bestandsnaam, userName);
                                    if (deleteRec)
                                    {
                                        bool accBijgewerkt = WriteDataInRecord(bestandsnaam, userName, seperator, true, EncryptStr(wachtwoord, encryptKey), Convert.ToString(budget));
                                        if (accBijgewerkt)
                                            Console.WriteLine("Gegevens bewaard");
                                    }
                                }
                                break;
                        }
                    } while (gameKeuze != GamesMenu.exit);
                }
            } while (!exit);
            Console.WriteLine("Tot nog eens!");
            Console.ReadKey();
            
        }



        static bool DeleteRecordInFile
            (
                string bestandsnaam,
                string recordKey,
                char seperator = '#'
            )
        {
            string searchMsg = SearchDataInRecord(bestandsnaam, recordKey);
            switch (searchMsg)
            {
                case "0": // file found but not record
                case null:return false; // file not found
                default:
                    {
                        string[] accReader = File.ReadAllLines(bestandsnaam);
                        string[] newFile = new string[accReader.GetUpperBound(0)];
                        int count = 0;
                        foreach (string accReaderLine in accReader)
                        {
                            string[] recGegevens = accReaderLine.Split(seperator);
                            if (recGegevens[0] != recordKey)
                                newFile[count++] = accReaderLine;
                        }
                        File.Delete(bestandsnaam);
                        File.WriteAllLines(bestandsnaam, newFile);
                        return true;
                    }
            }
        }
        static bool WriteDataInRecord
            (
                string bestandsnaam,
                string recordKey,
                char seperator = '#',
                bool createIfFileNotFound = false,
                params string[] dataToAdd
            )
        {
            string recordToAdd = recordKey;
            for (int i = 0; i < dataToAdd.Length; i++)
                recordToAdd += seperator + dataToAdd[i];

            if (File.Exists(bestandsnaam))
            {
                FileStream appendFile = File.Open(bestandsnaam, FileMode.Append);
                StreamWriter writer = new StreamWriter(appendFile);
                writer.WriteLine(recordToAdd);
                writer.Close();
                return true;
            }
            else
            {
                if (createIfFileNotFound)
                {
                    using (StreamWriter writer = new StreamWriter(bestandsnaam))
                        writer.WriteLine(recordToAdd);
                    return true;
                }
                else
                    return false;
            }
        }
        static string SearchDataInRecord
            (
                string bestandsnaam, 
                string recordKey, 
                int cellToReturn = 0, 
                char seperator = '#' 
            )   /* returns: 
                 *  null when file not found or 
                 *  "0" when record not found
                 */
        {
            if (File.Exists(bestandsnaam))
            {
                string[] accReader = File.ReadAllLines(bestandsnaam);

                foreach (string accReaderLine in accReader)
                {
                    string[] recGegevens = accReaderLine.Split(seperator);
                    if (recGegevens[0] == recordKey)
                        if (cellToReturn <= recGegevens.GetUpperBound(0))
                            return recGegevens[cellToReturn];
                        else
                            return "-1";
                }
            }
            else
                return null;
            return "0";
        }
        static string EncryptStr
            (
                string toEncrypt, 
                string encryptKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
            )
        {
            const string unEncryted = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string encrypted = "";

            for (int i = 0; i < toEncrypt.Length; i++)
            {
                int indexChar = unEncryted.IndexOf(toEncrypt[i]);
                if ((indexChar != -1) && (indexChar <= encryptKey.Length))
                {
                    encrypted += encryptKey[indexChar];
                }else
                {
                    encrypted += toEncrypt[i];
                }
            }
            return encrypted;
        }

        static string CheckWachtwoord      // returns null when all ok
            (   string strToCheck, 
                params int[] toCheck
            )
        /*  
         *  params:
            min lengte, 
            max lengte, 
            aantal hoofdletters, 
            aantal kleine letters,
            aantal cijfers,
            aantal vreemde letters
        */

        {
            bool geldig = true;
            string errorMsg = null;

            int[] gevondenLetter =
                {
                    CountCharInRange(strToCheck, 'A', 'Z'),
                    CountCharInRange(strToCheck, 'a', 'z'),
                    CountCharInRange(strToCheck, '0', '9'),
                    CountCharInRange(strToCheck, 23, 126) - CountCharInRange(strToCheck, 'A', 'Z') -
                    CountCharInRange(strToCheck, 'a', 'z') - CountCharInRange(strToCheck, '0', '9')
                };

            geldig = true;
            for (int i = 0; i < toCheck.Length; i++)
            {
                if (!geldig) break;
                switch(i)
                {
                    case 0:
                        {
                            if ((strToCheck.Length < toCheck[i]) || (strToCheck.Length > toCheck[i+1]))
                            {
                                errorMsg = $"Ongeldige lengte\nGeldige lengte is min. {toCheck[i]} max. {toCheck[i+1]}";
                                geldig = false;
                            }
                        }
                        break;
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        {
                            if (gevondenLetter[i-2] < toCheck[i])
                            {
                                geldig = false;
                                switch (i)
                                {
                                    case 2:
                                        {
                                            errorMsg =  $"te weinig aantal hoofdletters ({gevondenLetter[i - 2]})\n" +
                                                        $"min aantal hoofdletters is {toCheck[i]}\n";
                                        }
                                        break;
                                    case 3:
                                        {
                                            errorMsg =  $"te weinig aantal kleineletters ({gevondenLetter[i - 2]})\n" +
                                                        $"min aantal kleineletters is {toCheck[i]}\n";
                                        }
                                        break;
                                    case 4:
                                        {
                                            errorMsg =  $"te weinig aantal cijfers ({gevondenLetter[i - 2]})\n" +
                                                        $"min aantal cijfers is {toCheck[i]}\n";
                                        }
                                        break;

                                    case 5:
                                        {
                                            errorMsg =  $"ongeldige aantal vreemde tekens ({gevondenLetter[i - 2]})\n" +
                                                        $"min aantal vreemde tekens is {toCheck[i]}\n";
                                        }
                                        break;
                                    default: break;
                                }
                            }
                            
                        }
                        break;
                    
                    default:break;
                }
            }

            return errorMsg;
        }

        static int InputInt(string tekst = "Getal: ")
        {
            Console.Write(tekst);
            return int.Parse(Console.ReadLine());
        }
        static string InputStr(string text = "String: ", bool numbersLetters = false, bool consoleClear = false)
        {
            if (numbersLetters)
            {
                string input;
                bool inputOK = true;
                do
                {
                    if (consoleClear)
                    {
                        Console.Clear();
                    }
                    Console.Write(text);
                    input = Console.ReadLine();
                    inputOK = true;
                    for (int i = 0; i < input.Length; i++)
                    {
                        if (    !Enumerable.Range('0', '9').Contains(input[i]) &&
                                !Enumerable.Range('A', 'Z').Contains(input[i]) &&
                                !Enumerable.Range('a', 'z').Contains(input[i])
                                        )
                        {
                            
                            if (input[i] == ' ')
                            {
                                Console.WriteLine("Spatie niet toegelaten, enkel cijfers en letters");
                            }
                            else
                            {
                                Console.WriteLine($"{input[i]} niet toegelaten, enkel cijfers en letters");
                            }
                            Console.ReadLine();
                            inputOK = false;
                            break;
                        }
                        
                    }

                } while (!inputOK);
                return input;
            }else
            {
                if (consoleClear)
                {
                    Console.Clear();
                }
                Console.Write(text);
                return Console.ReadLine();
            }
        }
        static int CountCharInRange(string txtToSearch, char startChar, char endChar)
        {
            int count = 0;
            foreach (char c in txtToSearch)
                if (c >= startChar && c <= endChar) count++;
            return count;
        }
        static int CountCharInRange(string txtToSearch, int startChar, int endChar)
        {
            int count = 0;
            foreach (char c in txtToSearch)
                if (c >= startChar && c <= endChar) count++;
            return count;
        }
        static int CountChar(string txtToCheck, char charToLook)
        {
            int count = 0;
            foreach (char c in txtToCheck)
                if (c == charToLook) count++;
            return count;
        }
        static int RandomInt(int einde, int start = 1)
        {
            Random rnd = new Random();
            return rnd.Next(start, einde);
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
        static char InputChr(string tekst = "Keuze: ")
        {
            Console.Write(tekst);
            ConsoleKeyInfo keyStrike = Console.ReadKey(true);
            return keyStrike.KeyChar;

        }
    }

}
