using PathOfIrregulars.Domain.Entities;
using PathOfIrregulars.Domain.Enums;
using PathOfIrregulars.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfIrregulars.Application.Services
{
    public class CardService
    {
        private readonly EffectRegistry _registry;

        public CardService(EffectRegistry registry) => _registry = registry;

        public EffectResult PlayCard(Card card, GameContext context, Lane? lane)
        {
            if (!context.ActivePlayer.Hand.Contains(card))
                return EffectResult.Fail($"{card.Name} is not in {context.ActivePlayer.Name}'s hand.");

            context.ActivePlayer.Hand.Remove(card);

            if (card.Type == CardType.Climber)
            {
                if (lane == null)
                    return EffectResult.Fail("A climber must be played to a lane.");

                lane.CardsInLane.Add(card);
                context.Log($"{context.ActivePlayer.Name} plays {card.Name} in lane {lane.LaneType}.");

           
                lane.AddPower(card.Power);
            }
            else
            {
                // SPELL or ARTIFACT
                context.Log($"{context.ActivePlayer.Name} casts {card.Name}.");
            }

            // Execute effects
            foreach (var effect in card.CardEffects)
            {
                var result = _registry.Execute(
                    effect.EffectId,
                    card,
                    context,
                    effect.GetAmount()
                );

                context.Log(result.Message);

                if (!result.Success)
                    return result;
            }

            if (card.Type != CardType.Climber)
            {
                context.ActivePlayer.Graveyard.Add(card);
                context.Log($"{card.Name} was sent to the graveyard.");
            }

            return EffectResult.Ok($"{card.Name} played successfully.");
        }


        //public EffectResult HandleEndOfTurnEffects( Card card, GameContext context)
        //{

        //}

        //public EffectResult HandleOnDeathEffects( Card card, GameContext context)
        //{

        //}


        //public void EffectCycler(Card card , GameContext context, Lane? lane)
        //{
        //    if( card.CardTrigger == "OnPlay")






        //}
    }
}
