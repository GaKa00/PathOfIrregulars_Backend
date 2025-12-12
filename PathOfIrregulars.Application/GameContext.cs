
using PathOfIrregulars.Application.Services;
using PathOfIrregulars.Domain.Entities;
using PathOfIrregulars.Domain.Enums;
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
        public Board Board { get;  private set; }

        public void SetupBoard()
        {
            Board = new Board
            {
                PlayerOneLanes = new List<Lane>(),
                PlayerTwoLanes = new List<Lane>()
            };

            // Copy lanes from players
            Board.PlayerOneLanes.AddRange(PlayerOne.Lanes);
            Board.PlayerTwoLanes.AddRange(PlayerTwo.Lanes);
        }

        public void StartGame(Player p1, Player p2)
        {

            cardService = new CardService(registry);
            PlayerOne = p1;
            PlayerTwo = p2;
            SetupBoard();



   
      


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

            HandleStartTurnEffects(ActivePlayer);
            HandleStartTurnEffects(Opponent);


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

            HandleEndTurnEffects(ActivePlayer);
            HandleEndTurnEffects(Opponent);

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


        //EFFECT HANDLERS

        public void HandleEndTurnEffects(Player player)
        {
            if (player?.Lanes == null) return;

            foreach (var lane in player.Lanes ?? Enumerable.Empty<Lane>())
            {
                if (lane?.CardsInLane == null) continue;

                foreach (var card in lane.CardsInLane)
                {
                    if (card.CardEffects == null) continue;

                    foreach (var effect in card.CardEffects)
                    {
                        if (effect.Trigger == EffectTrigger.OnTurnEnd)
                        {
                            registry.Execute(effect.EffectId, card, this, effect.GetAmount());
                        }
                    }
                }
            }
        }

        public void HandleStartTurnEffects(Player player)
        {
            if (player?.Lanes == null) return; 

            foreach (var lane in player.Lanes ?? Enumerable.Empty<Lane>())
            {
                if (lane?.CardsInLane == null) continue; 

                foreach (var card in lane.CardsInLane)
                {
                    if (card.CardEffects == null) continue; 

                    foreach (var effect in card.CardEffects)
                    {
                        if (effect.Trigger == EffectTrigger.OnTurnStart)
                        {
                            registry.Execute(effect.EffectId, card, this, effect.GetAmount());
                        }
                    }
                }
            }
        }
        public void EquipArtifact(Card artifact)
        {
            Log($"CardEffects count: {artifact.CardEffects.Count}");
            SelectTargetCard();
            var targetClimber = this.TargetCard;

            if (targetClimber == null)
            {
                Log("No valid climber selected to equip.");
                ActivePlayer.Graveyard.Add(artifact);
                return;
            }

            targetClimber.EquippedArtifacts.Add(artifact);
            artifact.EquippedTo = targetClimber;

            Log($"{artifact.Name} equipped to {targetClimber.Name}.");

       
            foreach (var effect in artifact.CardEffects.Where(e => e.Trigger == EffectTrigger.OnEquip))
            {
                registry.Execute(effect.EffectId, artifact, this, effect.GetAmount());
            }
        }

        public void HandleCardDeathEffect(Card card)
        {
           
            foreach (var effect in card.CardEffects.Where(e => e.Trigger == EffectTrigger.OnDeath))
            {
                registry.Execute(effect.EffectId, card, this, effect.GetAmount());
            }

        }

        // CARD FUNCTIONS
        public void SelectTargetCard()
        {

            Console.WriteLine("In aelect card target");
           
            foreach (var c in Board.CardsInPlay)
                Console.WriteLine($"{c.Name} - ID: {c.Id}");

            Console.WriteLine("Enter the ID of the card you want to target:");

            while (true)
            {
                var input = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Log("Invalid input. Enter a valid card ID.");
                    continue;
                }

                var card = Board.CardsInPlay.FirstOrDefault(c => c.Id == input);

                if (card == null)
                {
                    Log("No card found with that ID. Try again.");
                    continue;
                }

                TargetCard = card;
                Log($"{card.Name} has been targeted.");
                break;
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
