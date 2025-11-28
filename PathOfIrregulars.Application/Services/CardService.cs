using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathOfIrregulars.Domain.Entities;
using PathOfIrregulars.Domain.ValueObjects;

namespace PathOfIrregulars.Application.Services
{
    public class CardService
    {
        private readonly EffectRegistry _registry;

        public CardService(EffectRegistry registry) => _registry = registry;

        public EffectResult PlayCard(Card card, GameContext context, Lane? lane)
        {

            if (!context.ActivePlayer.Hand.Contains(card))
            {
                return EffectResult.Fail($"{card.Name} is not in {context.ActivePlayer.Name}'s hand.");
            }


            context.ActivePlayer.Hand.Remove(card);
            context.Log($"{context.ActivePlayer.Name} plays {card.Name} in lane {lane.LaneType}.");


            lane.CardsInLane.Add(card);

            foreach (var effect in card.CardEffects)
            {
                Console.WriteLine("In effect loop!");
                var result = _registry.Execute(
     effect.EffectId,
     card,
     context,
     effect.GetAmount()
 );

                Console.WriteLine("executing effect" + effect.EffectId);
                context.Log(result.Message);

                if (!result.Success)
                {
                    return result;
                }
            }

            if (card.Power != null)
            {
                lane.AddPower(card.Power);
                Console.WriteLine(lane.Power);
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
