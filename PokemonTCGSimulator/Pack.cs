using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace PokemonTCGSimulator
{
    public class Pack{

        private string APP_PATH = AppDomain.CurrentDomain.BaseDirectory;

        public string name;
        public List<Card> cards;
        public int size;
        private Random rn;

        public Pack(string name){
            this.cards = new List<Card>();
            this.rn = new Random();
            this.name = name;
            this.size = 0;
            if(!Init()){
                Console.WriteLine("Problem z inicjalizacją boostera.");
                Console.WriteLine("Sprawdź czy napewno taki booster istnieje.");
            }
        }

        public List<Card> Open(){

            List<Card> opened = new List<Card>();
            
            var query = from c in cards where c.type == "Common" select c;
            List<Card> common = query.ToList<Card>();
            query = from c in cards where c.type == "Uncommon" select c;
            List<Card> uncommon = query.ToList<Card>();
            query = from c in cards where c.type == "Rare" select c;
            List<Card> rare = query.ToList<Card>();
            query = from c in cards where c.type == "UltraRare" select c;
            List<Card> uRare = query.ToList<Card>();
            query = from c in cards where c.type == "Energy" select c;
            List<Card> energy = query.ToList<Card>();
            
            opened.Add(energy[rn.Next(0, energy.Count-1)]);

            bool rares = false;

            if(rn.Next(0, 100) < 50)
                rares = true;

            if(rares){

                for(int i = 1; i < 8; i++){
                    if(rn.Next(0, 100) < 25)
                        opened.Add(uncommon[rn.Next(0, uncommon.Count)]);
                    else
                        opened.Add(common[rn.Next(0, common.Count)]);
                    
                }

                if(rn.NextDouble()*100 < 1.5)
                    opened.Add(uRare[rn.Next(0, uRare.Count)]);
                else
                    opened.Add(rare[rn.Next(0, rare.Count)]);
                

                if(rn.NextDouble()*100 < 1.5)
                    opened.Add(uRare[rn.Next(0, uRare.Count)]);
                else
                    opened.Add(rare[rn.Next(0, rare.Count)]);
                
            }else{

                for(int i = 1; i < 9; i++){
                    if(rn.Next(0, 100) < 25)
                        opened.Add(uncommon[rn.Next(0, uncommon.Count)]);
                    else
                        opened.Add(common[rn.Next(0, common.Count)]);
                    
                }

                if(rn.NextDouble()*100 < 1.5)
                    opened.Add(uRare[rn.Next(0, uRare.Count)]);
                else
                    opened.Add(rare[rn.Next(0, rare.Count)]);
                
            }

            return opened;

        }

        private bool Init() {

            if (Directory.Exists(APP_PATH + @"Boosters\" + name))
            {

                string pp = APP_PATH + @"Boosters\" + name;

                string[] json = File.ReadAllLines(pp + @"\pokemons.json");

                foreach(string j in json)
                {
                    var card = JsonConvert.DeserializeObject<Card>(j);
                    cards.Add(card);
                    size++;
                }

                return true;

            }

            return false;
        }

    }
}