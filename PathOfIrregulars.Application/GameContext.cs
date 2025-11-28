
using PathOfIrregulars.Domain.Entities;


namespace PathOfIrregulars.Application
{
    public class GameContext
    {

        public List<string> Logs { get; private set; } = new();
        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }

        public Player ActivePlayer { get; private set; }
        public Player Opponent => ActivePlayer == PlayerOne ? PlayerTwo : PlayerOne;

        public int TurnCount { get; private set; } = 1;

        public Card? TargetCard { get; set; }

        public bool HasGameEnded { get; private set; } = false;
        public Player? Winner { get; private set; }

        public FloorTest floorTest { get; set; } 

        public void StartGame(Player p1, Player p2)
        {

            PlayerOne = p1;
            PlayerTwo = p2;
            ActivePlayer = p1;

            p1.ShuffleDeck();
            p2.ShuffleDeck();   


            for (int i = 1; i < 10; i++)
            {
                DrawCard(PlayerOne);
                Log("p1 " + PlayerOne.Hand.Count);

                DrawCard(PlayerTwo);
                Log("p2" + PlayerTwo.Hand.Count);
            }




            Log("Game started between " + PlayerOne.Name + " and " + PlayerTwo.Name);
        }

        public void StartTurn()
        {
            Log($"Turn {TurnCount} started for {ActivePlayer.Name}");

            // decide who starts , by seeing who has the lowest score.
            //set new floor test
            //draw 4 cards or until 10 cards in hand
        }

        public void startRound()
        {
            PlayerOne.hasPassed = false;
            PlayerTwo.hasPassed = false;
            Log("New round started.");


        }

        public void EndTurn()
        {//calculate effects that happen at end of turn
            //calculate points

            ActivePlayer = Opponent;
            
            TurnCount++;
        }

        public void EndRound()
        {
            PlayerOne.hasPassed = true;
            PlayerTwo.hasPassed = true;

           
        }

        public void EndGame(Player winner)
        {
            Winner = winner;
            HasGameEnded = true;
        }


        public static List<Card> GetAdjacentCards(Card card, Lane lane) {

            var cards = lane.CardsInLane;
            var index = cards.IndexOf(card);
            var adjacentCards = new List<Card>();

            if ( index == -1)
            {
                return cards;
            }

            if (index > 0)
            {
                adjacentCards.Add(cards[index - 1]);
            }
            if (index < cards.Count - 1)
            {
                adjacentCards.Add(cards[index + 1]);
            }
            return cards;

        }

        public Card DrawCard(Player player)
        {
            if (player.Deck.Count == 0)
            {
                Log("Deck is empty. Cannot draw a card.");
               
                return null;
            }
            var drawnCard = player.Deck[0];
            player.Deck.RemoveAt(0);
            player.Hand.Add(drawnCard);
            Log($"{drawnCard.Name} drawn to hand.");
            return drawnCard;
        }


     

        //targetmultiplecards?
        public void Log(string message)
        {
            Logs.Add(message);
            Console.WriteLine(message);
        }
    }
}
