using PathOfIrregulars.Application.Services;
using PathOfIrregulars.Domain.Entities;
using PathOfIrregulars.Domain.Enums;
using System.Data.Common;
using System.Numerics;



namespace PathOfIrregulars.Application
{
    public class Match
    {

        public List<string> Logs { get; private set; } = new();
        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }

        public Player ActivePlayer { get; private set; }
        public Player Opponent => ActivePlayer == PlayerOne ? PlayerTwo : PlayerOne;

        public int TurnCount { get; private set; } = 1;


        public GameState GameState { get; set; }
        public Player? Winner { get; private set; }
        public bool HasGameEnded { get; private set; } = false;
        public Board Board { get;  private set; }

        public FloorTest floorTest { get; set; }
        public Card? TargetCard { get; set; }
      
        private EffectRegistry Registry { get; set; } = new EffectRegistry();

        private CardService cardService { get; set; }


        //helper method to setup board for easier access to lanes and cards in play
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
            // set initial game state
            GameState = GameState.GameStart;
            cardService = new CardService(Registry);
            PlayerOne = p1;
            PlayerTwo = p2;
            SetupBoard();
            SetTurnOrder();


            p1.ShuffleDeck();
            p2.ShuffleDeck();

            //draw opening hands
            for (int i = 1; i < 10; i++)
            {
              PlayerOne.DrawCard();
                Log("p1 " + PlayerOne.Hand.Count);

              PlayerTwo.DrawCard();
                Log("p2" + PlayerTwo.Hand.Count);
            }
            Log("Game started between " + PlayerOne.Name + " and " + PlayerTwo.Name);

            //MulliganPhase - TODO
            //InitiateGameLoop();

        }



public void InitiateGameLoop()
{
    StartRound();

            // as long as a winner hasn't been decided, keep playing turns
            while (true)
    {
        StartTurn(ActivePlayer);
        EndTurn();
                //  Check if both players have passed to end the round
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
        // Setup floor test - TODO
        public void InitializeFloorTest()
        {
            floorTest = new FloorTest();
            Log("Floor test initialized.");
        }

        //Randomizes starting player
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

            //set game state based on active player
            GameState = player == PlayerOne
        ? GameState.Player1Turn
        : GameState.Player2Turn;


            

            //var cardToPlay = player.SelectCard();
            ////temporary null, is later set by player input 
            //Lane? lane = null;

            //if (cardToPlay.Type == CardType.Climber)
            //{
            // lane = player.SelectLane();
            //}

            ////plays selected card  after lane is set
            //var result = cardService.PlayCard(cardToPlay, this, lane);

            return;
        }

    public void PlayCard(string playerName, string cardId, string? laneId, string? targetCardId)
        {
            var player = playerName == PlayerOne.Name ? PlayerOne : PlayerTwo;
            if (player != ActivePlayer)
            {
                Log($"It's not {player.Name}'s turn.");
                return;
            }
            var cardToPlay = player.Hand.FirstOrDefault(c => c.Definition.Id == cardId);

            if (cardToPlay == null)
            {
                Log($"{player.Name}  does not have card with ID {cardId} in hand.");
                return;
            }
            Lane? lane = null;
        
                if (Enum.TryParse<LaneType>(laneId, out var parsedLaneType))
                {
                    lane = player.Lanes.FirstOrDefault(l => l.LaneType == parsedLaneType);
                    if (lane == null)
                    {
                        Log($"{player.Name} does not have lane with ID {laneId}.");
                        return;
                    }
                }
                else
                {
                    Log($"{laneId} is not a valid LaneType.");
                    return;
                }

                var targetCard = targetCardId != null ? Board.CardsInPlay.FirstOrDefault(c => c.Definition.Id == targetCardId) : null;
                var result = cardService.PlayCard(cardToPlay, this, lane, targetCard);

            }

        

        public void StartRound()
        {// reset round state
            GameState = GameState.RoundStart;
            PlayerOne.HasPassed = false;
            PlayerTwo.HasPassed = false;
            Log("New round started.");

            //trigger start of round effects
            ResolveTrigger(EffectTrigger.OnTurnStart, owner:PlayerOne);

           ResolveTrigger(EffectTrigger.OnTurnStart, owner: PlayerTwo);


            //determine starting player
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
            // draw cards  up to 4, or until hand has 10 cards
            for (int i = 0; i < 4; i ++)
                {
                if (PlayerOne.Hand.Count < 10)
                {
                    PlayerOne.DrawCard();
                }
                if (PlayerTwo.Hand.Count < 10)
                {
                    PlayerTwo.DrawCard();
                }
            }

            //set new floor test - todo
        }

        public void EndTurn()
        {//calculate effects that happen at end of turn


            ActivePlayer.CalculateTotalPower();
            ResolveTrigger(EffectTrigger.OnTurnEnd, owner: ActivePlayer);
            ResolveTrigger(EffectTrigger.OnTurnEnd, owner: Opponent); 

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

            GameState = GameState.RoundEnd;
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
            GameState = GameState.GameEnd;
            Winner = winner;
            HasGameEnded = true;
        }





        // Resets the field by clearing lanes and destroying cards in play
        public void ResetField()
        {

            PlayerOne.Reset();
            PlayerTwo.Reset();
          
        }

        public void Pass(Player player)
        {
            player.HasPassed = true;
            Log($"{player.Name} has passed their turn.");
        }


        //EFFECT HANDLERS

        public void ResolveTrigger(
    EffectTrigger trigger,
    CardInstance? sourceCard = null,
    Player? owner = null
)
        {
            var cards = Board.CardsInPlay;

           
            if (sourceCard != null && !cards.Contains(sourceCard))
                cards = cards.Append(sourceCard);

            foreach (var card in cards)
            {
                foreach (var effect in card.Definition.CardEffects
                             .Where(e => e.Trigger == trigger))
                {
                    Registry.Execute(
                        effect.EffectId,
                        card,
                        this,
                        effect.GetAmount()
                    );
                }
            }
        }


        // Equip artifact to climber
        public void EquipArtifact(CardInstance artifact, CardInstance target)
        {
      
            
            var targetClimber = target;

            if (targetClimber == null)
            {
                Log("No valid climber selected to equip.");
                ActivePlayer.Graveyard.Add(artifact);
                return;
            }

            targetClimber.EquippedArtifacts.Add(artifact);
            artifact.EquippedTo = targetClimber;

            Log($"{artifact.Definition.Name} equipped to {targetClimber.Definition.Name}.");


            foreach (var effect in artifact.Definition.CardEffects.Where(e => e.Trigger == EffectTrigger.OnEquip))
            {
                Registry.Execute(effect.EffectId, artifact, this, effect.GetAmount());
            }
        }
        // utility method to get adjacent cards in a lane
       

        //public void HandleCardDeathEffect(Card card)
        //{

        //    foreach (var effect in card.CardEffects.Where(e => e.Trigger == EffectTrigger.OnDeath))
        //    {
        //        registry.Execute(effect.EffectId, card, this, effect.GetAmount());
        //    }

        //}

        // CARD FUNCTIONS
        //public void SelectTargetCard()
        //{

           
        //    foreach (var c in Board.CardsInPlay)
        //        Console.WriteLine($"{c.Name} - ID: {c.Id}");

        //    Console.WriteLine("Enter the ID of the card you want to target:");

        //    while (true)
        //    {
        //        var input = Console.ReadLine()?.Trim();

        //        if (string.IsNullOrWhiteSpace(input))
        //        {
        //            Log("Invalid input. Enter a valid card ID.");
        //            continue;
        //        }

        //        var card = Board.CardsInPlay.FirstOrDefault(c => c.Id == input);

        //        if (card == null)
        //        {
        //            Log("No card found with that ID. Try again.");
        //            continue;
        //        }

        //        TargetCard = card;
        //        Log($"{card.Name} has been targeted.");
        //        break;
        //    }
        //}
        
        public void Log(string message)
        {
            Logs.Add(message);
            Console.WriteLine(message);
        }
    }
}
