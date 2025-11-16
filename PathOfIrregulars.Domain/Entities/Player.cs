
namespace PathOfIrregulars.Domain.Entities

{
  public class Player
    {
        public string Name { get; set; }
        public List<Card> Hand { get; set; }
        public List<Card> Deck { get; set; }
        public List<Card> Graveyard { get; set; }

        public int wonRounds { get; set; } = 0;
        public bool hasPassed { get; set; }

        public Card DrawCard()
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

        public bool Pass()
        {
            hasPassed = true;
          
            return hasPassed;
        }

    }
}
