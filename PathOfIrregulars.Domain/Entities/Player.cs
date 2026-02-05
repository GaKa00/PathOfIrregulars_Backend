
namespace PathOfIrregulars.Domain.Entities

{
  public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CardInstance> Hand { get; set; }
        public List<CardInstance> Deck { get; set; }
        public List<CardInstance> Graveyard { get; set; }

        public Lane[] Lanes { get; set; }

        public int TotalPower => CalculateTotalPower();
        public int WonRounds { get; set; } = 0;
        public bool HasPassed { get; set; }

        public CardInstance DrawCard()
        {
            if (Deck.Count == 0)
            {
                return null;
               
            }
            var drawnCard = Deck[0];
            Deck.RemoveAt(0);
            Hand.Add(drawnCard);
       
            return drawnCard;
        }

        public void ShuffleDeck(Random rng)
        {
            for (int i = Deck.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (Deck[i], Deck[j]) = (Deck[j], Deck[i]);
            }
        }

        // Select card to play from hand- will later take id as parameter
        public CardInstance SelectCard()
        {
            for (int i = 0; i < Hand.Count; i++)
            {
                Console.WriteLine($"{i}: {Hand[i].Definition.Name} (Power: {Hand[i].Power})");
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
        // Select lane to play card in
        public Lane SelectLane()
        {
            for (int i = 0; i < Lanes.Length; i++)
            {
                Console.WriteLine($"{i}: {Lanes[i].LaneType} (Power: {Lanes[i].CalculateLanePower()})");
            }
            Console.WriteLine("Select a lane to play in:");
            int selectedIndex;
            string? input = Console.ReadLine();
            while (!int.TryParse(input, out selectedIndex) || selectedIndex < 0 || selectedIndex >= Lanes.Length)
            {
                Console.WriteLine("Invalid selection. Try again:");
                input = Console.ReadLine();
            }
            return Lanes[selectedIndex];

        }


        //public bool Pass()
        //{
        //    hasPassed = true;

        //    return hasPassed;
        //}

        // Calculate total power across all lanes for player
        public int CalculateTotalPower()
        {
            int totalPower = 0;
            foreach (var lane in Lanes)
            {
                totalPower += lane.CalculateLanePower();
            }
            Console.WriteLine("Total Power (from player class): " + totalPower);
            return totalPower;
        }

        // reset player for new round
        public void Reset()
        {
            foreach (var lane in Lanes)
            {
                foreach (var card in lane.CardsInLane.ToList())
                {
                    Graveyard.Add(card);
                }

                lane.CardsInLane.Clear();
              
            }

            HasPassed = false;
        }

    }
}
