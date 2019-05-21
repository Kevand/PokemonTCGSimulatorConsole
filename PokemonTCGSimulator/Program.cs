using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using ConsoleImages;
using System.Linq;
using KevandConsole;

namespace PokemonTCGSimulator
{

    class Program{

        private static string APP_PATH = AppDomain.CurrentDomain.BaseDirectory;

        public static bool opened;
        public static List<Card> equipment;

        public static void Main(){

            equipment = new List<Card>();

            if (!File.Exists("save.tcgs"))
                File.Create("save.tcgs").Close();

            opened = true;

            while(opened){

                kc.cls();

                int menuChoice;

                kc.wl("MENU");
                kc.wl("-------------------------------------------------");
                kc.wl("1. Otwórz booster");
                kc.wl("2. Ekwipunek");
                try{
                    menuChoice = Convert.ToInt32(kc.rl());
                }
                catch
                {
                    menuChoice = 1;
                }

                switch (menuChoice)
                {
                    case 1:
                        OpenPack();
                        break;
                    case 2:
                        OpenEquipment();
                        break;
                }

            }

        }

        public static void OpenPack()
        {

            List<Card> opened = new List<Card>();

            //Na początku czyścimy konsole;
            kc.cls();
            //Reszta;
            GenerateCards();
            kc.wl("Wybierz booster: ");
            kc.wl("--------------------");
            int cn = 0;
            string[] choice = new string[10];
            try
            {
                foreach (var dir in Directory.GetDirectories("Boosters/"))
                {
                    string dirName = dir.Split('/')[1];
                    choice[cn] = dirName;
                    kc.wl(cn + 1 + ". " + dirName);
                    cn++;
                }
            }
            catch
            {
                kc.wl("Brak boosterów");
                kc.rk();
                return;
            }

            string name = kc.rl();
            //inicjalizaja paczki
            Pack pack = new Pack(choice[Convert.ToInt32(name) - 1]);
            kc.cls();
            try
            {
                CImage.ConsoleWriteImage(new System.Drawing.Bitmap(APP_PATH + @"Boosters\" + pack.name + @"\cover.png"));
            }
            catch
            {}

            kc.wl("--------------------------------");
            kc.wl("");
            kc.ww("Ilość kart w zestawie:");
            kc.wl(pack.size, ConsoleColor.Red);
            kc.ww("Ile paczek chcesz otworzyć?[Max: 10]: ");
            int count = Convert.ToInt32(kc.rl());
            if (count <= 0)
                count = 1;
            else if (count > 10)
                count = 10;

            kc.wl("Prędkość otwierania? [Od 10 = najszybciej do 100 = najwoleniej] ");
            int initSpeed = Convert.ToInt32(kc.rl());
            if (initSpeed < 10)
                initSpeed = 10;
            else if (initSpeed > 100)
                initSpeed = 100;

            kc.cls();

            for (int x = 0; x < count; x++)
            {
                List<Card> cards = pack.Open();

                kc.wl("");
                kc.wl("Otwieramy booster...");
                kc.wl("");

                int speed = initSpeed;

                foreach (Card c in cards)
                {
                    opened.Add(c);
                    kc.ww(c.id + ". Typ:");
                    if (c.type == "Common")
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        kc.wl(c.type, ConsoleColor.Green);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    if (c.type == "Energy")
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        kc.wl(c.type, ConsoleColor.DarkGray);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    if (c.type == "Uncommon")
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        kc.wl(c.type, ConsoleColor.Magenta);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    if (c.type == "Rare")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        kc.wl(c.type, ConsoleColor.Red);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    if (c.type == "UltraRare")
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        kc.wl(c.type, ConsoleColor.Yellow);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Thread.Sleep(speed);
                    speed += initSpeed;
                }

            }

            foreach(Card c in opened)
            {
                string json = JsonConvert.SerializeObject(c);
                File.AppendAllText("save.tcgs", json + Environment.NewLine);
            }

            kc.rk();
        }

        public static void OpenEquipment()
        {

            equipment = LoadEquipment();
            var query = from c in equipment orderby c.id ascending select c;

            bool opened = true;

            while (opened)
            {
                kc.cls();

                equipment = query.ToList();

                if (equipment.Count == 0)
                {
                    kc.wl("Ekiwipunek jest pusty.");
                }
                else
                {

                    foreach (Card c in equipment)
                    {
                        kc.wl(c.booster + " | " + c.id + ". " + c.type);
                    }

                }

                kc.wl("Tryb sortowania: 1. Po ID 2. Po rzadkości 3. Po boosterze. Aby wyjść kliknij [Q]");

                var choice = kc.rk();

                switch (choice)
                {
                    case ConsoleKey.D1:
                        query = from c in equipment orderby c.id ascending select c;
                        break;
                    case ConsoleKey.D2:
                        query = from c in equipment orderby c.type.Length descending select c;
                        break;
                    case ConsoleKey.D3:
                        query = from c in equipment orderby c.booster ascending select c;
                        break;
                    case ConsoleKey.Q:
                        opened = false;
                        break;
                }

            }

            
        }

        public static void GenerateCards() {

            try
            {
                //dla każdego boostera
                foreach (var dir in Directory.GetDirectories(APP_PATH + @"Boosters\"))
                {
                    //bierzemy informacje o nim
                    DirectoryInfo d = new DirectoryInfo(dir);
                    //jeżeli nie ma w nim plików (domyślnie json'ów odpowiadających za karty)
                    if (!File.Exists(d.FullName + @"\pokemons.json"))
                    {
                        kc.wl("Generowanie bazy kart... ");
                        //generuj json

                        string pth = d.FullName + @"\pokemons.json";

                        //tworzymy plik bazy
                        File.Create(pth).Close();

                        foreach (var d2 in d.GetDirectories())
                        {
                            //wypisujemy jakie pokemony ładuje gra
                            kc.wl("Pokemony " + d2.Name);

                            foreach (var file in d2.GetFiles())
                            {
                                //serializacja do json
                                Card c = new Card(
                                    Convert.ToInt32(Path.GetFileNameWithoutExtension(file.Name)),
                                    Path.GetFileName(Path.GetDirectoryName(file.FullName)),
                                    file.Name,
                                    d.Name);
                                string json = JsonConvert.SerializeObject(c);
                                File.AppendAllText(pth, json + Environment.NewLine);
                            }

                        }
                    }
                }
            }
            catch
            {
                return;
            }

            kc.cls();
                    
        }

        private static List<Card> LoadEquipment()
        {
            List<Card> eq = new List<Card>();

            string[] json = File.ReadAllLines("save.tcgs");

            foreach(string s in json)
            {
                Card crd = JsonConvert.DeserializeObject<Card>(s);
                eq.Add(crd);
            }
            

            return eq;
        }

    }

}