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

        public EffectResult PlayCard(CardInstance card, Match context, Lane? lane, CardInstance? target)
        {

            // Validate card is in hand
            if (!context.ActivePlayer.Hand.Contains(card))
                return EffectResult.Fail($"{card.Definition.Name} is not in {context.ActivePlayer.Name}'s hand.");

            context.ActivePlayer.Hand.Remove(card);


            // Handle card types
            if (card.Definition.Type == CardType.Climber)
            {
                if (lane == null)
                    return EffectResult.Fail("A climber must be played to a lane.");

                lane.CardsInLane.Add(card);
                context.Log($"{context.ActivePlayer.Name} plays {card.Definition.Name} in lane {lane.LaneType}.");

           
                lane.AddPower(card.Power);
            }
            else if (card.Definition.Type == CardType.Artifact)
            {
                context.EquipArtifact(card, target);

                return EffectResult.Ok($"{card.Definition.Name} equipped.");
            }

            // Execute effects by going through each effect on the card
            foreach (var effect in card.Definition.CardEffects)
            {
                var result = _registry.Execute(
                    effect.EffectId,
                    card,
                    context,
                    effect.GetAmount(),
                    target
                );

                context.Log(result.Message);

                if (!result.Success)
                    return result;
            }

            if (card.Definition.Type != CardType.Climber)
            {
                context.ActivePlayer.Graveyard.Add(card);
                context.Log($"{card.Definition.Name} was sent to the graveyard.");
            }

            return EffectResult.Ok($"{card.Definition.Name} played successfully.");
        }


       

    


    }
}
