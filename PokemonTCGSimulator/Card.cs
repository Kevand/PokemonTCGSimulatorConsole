namespace PokemonTCGSimulator
{
    public struct Card{
        public int id;
        public string type;
        public string image;
        public string booster;
        public Card(int id, string type, string image, string booster){
            this.id = id;
            this.type = type;
            this.image = image;
            this.booster = booster;
        }

    }
}