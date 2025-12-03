
using PathOfIrregulars.Application.Services;
using PathOfIrregulars.Domain.Entities;
using System.Data.Common;
using System.Numerics;



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
      
        private EffectRegistry registry { get; set; } = new EffectRegistry();

        private CardService cardService { get; set; }

        public void StartGame(Player p1, Player p2)
        {

            cardService = new CardService(registry);


            PlayerOne = p1;
            PlayerTwo = p2;

   
      


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

            //MulliganPhase
            InitiateGameLoop();

        }



public void InitiateGameLoop()
{
    StartRound();

    while (true)
    {
        StartTurn(ActivePlayer);
        EndTurn();

        if (PlayerOne.HasPassed && PlayerTwo.HasPassed)
        {
            EndRound();
            
            if (PlayerOne.WonRounds == 2)
            {
                EndGame(PlayerOne);
                return;
            }
            if (PlayerTwo.WonRounds == 2)
            {
                EndGame(PlayerTwo);
                return;
            }

            StartRound(); 
        }
    }
}

       public void InitializeFloorTest()
        {
            floorTest = new FloorTest();
            Log("Floor test initialized.");
        }

        public void SetTurnOrder()
        {
            var rnd = new Random();
            if (rnd.Next(0, 2) == 0)
            {
                ActivePlayer = PlayerOne;

            }
            else
            {
                ActivePlayer = PlayerTwo;
            }

        }

        public void StartTurn(Player player)
        {
            if (player != ActivePlayer)
            {
                Log($"It's not {player.Name}'s turn.");
                return;
            }


            Log($"Turn {TurnCount} started for {player.Name}");

       

            Log($" Total Power: {player.TotalPower}");


            //temporary pass, will later make into a own api call (TODO)
            Console.WriteLine("Would you like to Pass?");
            var input = Console.ReadLine();
            if (input != null && input.ToLower() == "pass")
            {
                player.HasPassed = true;
                Log($"{player.Name} has passed.");
               
               EndTurn();
                return;
            }

            var cardToPlay = player.SelectCard();
            Lane? lane = null;

            if (cardToPlay.Type == Domain.Enums.CardType.Climber)
            {
             lane = player.SelectLane();
            }


            var result = cardService.PlayCard(cardToPlay, this, lane);

            return;


        }

        public void StartRound()
        {
            PlayerOne.HasPassed = false;
            PlayerTwo.HasPassed = false;
            Log("New round started.");

            


            if (PlayerOne.WonRounds > PlayerTwo.WonRounds)
            {
                ActivePlayer = PlayerTwo;
            }
            else if (PlayerTwo.WonRounds > PlayerOne.WonRounds)
            {
                ActivePlayer = PlayerOne;
            }
            else
            {
                SetTurnOrder();
            }

            for (int i = 0; i < 4; i ++)
                {
                if (PlayerOne.Hand.Count < 10)
                {
                    DrawCard(PlayerOne);
                }
                if (PlayerTwo.Hand.Count < 10)
                {
                    DrawCard(PlayerTwo);
                }
            }

            //set new floor test
          





        }

        public void EndTurn()
        {//calculate effects that happen at end of turn

            ActivePlayer.CalculateTotalPower();
            Console.WriteLine(ActivePlayer);

            TurnCount++;

            if (PlayerOne.HasPassed && PlayerTwo.HasPassed) {
                Log($"Both players have passed, Starting new Round");
                return;
            }

            if (Opponent.HasPassed)             {
                Log($"{Opponent.Name} has passed. Skipping turn switch.");

                return;
            }
        


            ActivePlayer = Opponent;
 

        }

        public void EndRound()
        {
    PlayerOne.CalculateTotalPower();
    PlayerTwo.CalculateTotalPower();
            Log($"Round ended. {PlayerOne.Name} has {PlayerOne.TotalPower} Power. {PlayerTwo.Name} has {PlayerTwo.TotalPower} Power");
            if (PlayerOne.TotalPower > PlayerTwo.TotalPower)
            {
                PlayerOne.WonRounds++;
                Log($"{PlayerOne.Name} wins the round!");
                ResetField();
            }
            else if (PlayerTwo.TotalPower > PlayerOne.TotalPower)
            {
                PlayerTwo.WonRounds++;
                Log($"{PlayerTwo.Name} wins the round!");
                ResetField();
            }
            else
            {
                Log("The round is a tie!");
                ResetField();
            }


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

        public void ResetField()
        {

            PlayerOne.Reset();
            PlayerTwo.Reset();
          
        }

  // CARD FUNCTIONS


        public void SelectEnemyTarget(Player player)
            {
            Log($"{player.Name}, select a target card from opponent's field:");
            var opponentLanes = Opponent.Lanes;
         
            var allCards = new List<Card>();
            foreach (var lane in opponentLanes)
            {
                allCards.AddRange(lane.CardsInLane);
            }
            for (int i = 0; i < allCards.Count; i++)
            {
                Log($"{i + 1}: {allCards[i].Name} (Power: {allCards[i].Power})");
            }

            if (allCards.Count == 0)
            {
                Log("No cards available to target.");
                return;
            }
            var input = Console.ReadLine();
            if (int.TryParse(input, out int choice) && choice > 0 && choice <= allCards.Count)
            {
                var selectedCard = allCards[choice - 1];
                Log($"{player.Name} selected {selectedCard.Name} as target.");
                TargetCard = selectedCard;
            }
            else
            {
                Log("Invalid selection. No target selected.");
                
            }
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


     

        
        public void Log(string message)
        {
            Logs.Add(message);
            Console.WriteLine(message);
        }
    }
}
