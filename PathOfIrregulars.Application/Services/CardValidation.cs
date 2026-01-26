using PathOfIrregulars.Domain.Entities;
using PathOfIrregulars.Domain.Enums;
namespace PathOfIrregulars.Application.Services
{
    internal class CardValidation
    {
        public static bool ValidatePlay(Card card, Match context, Lane lane)
        {
            //check if activeplayer played card

            //check if card is in hand

            if (context.ActivePlayer.Hand.Contains(card))
            {
                return false;
            }

            // check that card is not already in play

            if (context.Board.CardsInPlay.Contains(card))
            {
                return false;
            }

            //check that game and round is not yet ended

            if (context.GameState == GameState.RoundEnd || context.GameState == GameState.GameEnd)
            {
                return false;
            }

            //check that climber card is going to a lane (if not discard effect)

            if (card.Type == Domain.Enums.CardType.Climber && lane == null) {
                return false;
            }
            //check that lane is not full (4 cards)

            if (lane.CardsInLane.Count >= 4)
            {
                return false;
            }

            //for spell and artifact, check that lane is null

            if ((card.Type == CardType.Spell || card.Type == CardType.Artifact) && lane != null) {
            }

            return true;
        }


        public static bool ValidateTarget(
            Card sourceCard,
            Card? targetCard,
            Match context
        )
        {
            //Target must exist
            if (targetCard == null)
                return false;

            //  Target must be in play
            if (!context.Board.CardsInPlay.Contains(targetCard))
                return false;

            //  Target must not be destroyed 
            if (targetCard.IsDestroyed)
                return false;

            // Target must be targetable
          
            if (targetCard.IsUntargetable)
                return false;

        
            // Artifacts can only be equipped to climbers
            if (sourceCard.Type == CardType.Artifact &&
                targetCard.Type != CardType.Climber)
                return false;


            return true;
        }



        public static bool ValidateEffect(
          CardEffect effect,
          Card sourceCard,
          Card? targetCard,
          Match context,
          EffectRegistry registry
      )
        {


            var definition = registry.GetDefinition(effect.EffectId);
            // 1. Effect must exist in registry
            if (!registry.Has(effect.EffectId))
                return false;

            // 2. Trigger must match current game event
            switch (effect.Trigger)
            {
                case EffectTrigger.OnPlay:
                    if (context.GameState != GameState.Player1Turn)
                        return false;
                    break;

             

                case EffectTrigger.OnTurnStart:
                    if (context.GameState != GameState.Player1Turn)
                        return false;
                    break;

                case EffectTrigger.OnTurnEnd:
                    if (context.GameState != GameState.Player1Turn)
                        return false;
                    break;
            }

            // 3. Amount validation (if applicable)
            if (effect.Parameters.TryGetValue("amount", out var amount))
            {
                if (amount <= 0)
                    return false;

                // Optional future safety cap
                if (amount > 100)
                    return false;
            }

            // 4. Target required but missing
            if ( definition.RequiresTarget && targetCard == null)
                return false;

            // 5. Delegate target legality
            if (definition.RequiresTarget)
            {
                if (!ValidateTarget(sourceCard, targetCard, context))
                    return false;
            }

            return true;
        }


        public static bool ValidateGameState(Match context)
        {

            //check that game is not over

            if (context.GameState == GameState.GameEnd)
            {
                return false;
            }
            //check that round is not over
            if (context.GameState == GameState.RoundEnd)
            {
                return false;
            }
    
            return true;
        }
    

    }

}
