
namespace PathOfIrregulars.Domain.Entities

{
  public class Player
    {
        public string Name { get; set; }
        public List<Card> Hand { get; set; }
        public List<Card> Deck { get; set; }
        public List<Card> Graveyard { get; set; }

        public Lane[] Lanes { get; set; }

        public int totalPower { get; set; } = 0;
        public int wonRounds { get; set; } = 0;
        public bool hasPassed { get; set; }

        public Card DrawCard()
        {
            if (Deck.Count == 0)
            {
                return null;
                //lose game
            }
            var drawnCard = Deck[0];
            Deck.RemoveAt(0);
            Hand.Add(drawnCard);
       
            return drawnCard;
        }

        public void ShuffleDeck()
        {
            var rnd = new Random();
            Deck = Deck.OrderBy(x => rnd.Next()).ToList();
        }   

        public Card SelectCard()
        {
            for (int i = 0; i < Hand.Count; i++)
            {
                Console.WriteLine($"{i}: {Hand[i].Name} (Power: {Hand[i].Power})");
            }

            Console.WriteLine("Select a card to play");

            int selectedIndex;

          
            string? input = Console.ReadLine();

            while (!int.TryParse(input, out selectedIndex) || selectedIndex < 0 || selectedIndex >= Hand.Count)
            {
                Console.WriteLine("Invalid selection. Try again:");
                input = Console.ReadLine();
            }

            return Hand[selectedIndex];
        }


        public bool Pass()
        {
            hasPassed = true;
          
            return hasPassed;
        }

        public int CalculateTotalPower()
        {
            int totalPower = 0;
            foreach (var lane in Lanes)
            {
                totalPower += lane.CalculateLanePower();
            }
            return totalPower;
        }



    }
}
