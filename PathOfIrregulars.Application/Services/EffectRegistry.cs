using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathOfIrregulars.Domain.Entities;
using PathOfIrregulars.Domain.ValueObjects;

namespace PathOfIrregulars.Application.Services
{
   public class EffectRegistry
    {

        private Dictionary<string, Func<Card, GameContext, int?, EffectResult>> effectRegistry;



        public EffectRegistry()
        {
            effectRegistry = new Dictionary<string, Func<Card, GameContext, int?, EffectResult>>
            {

               

                //draw a card
                ["DrawCard"] = (card, context, amount) =>
                {
                    for (int i = 0; i < (amount ?? 1); i++)
                    {
                        context.ActivePlayer.DrawCard();

                    }

                    return EffectResult.Ok($"{context.ActivePlayer.Name} drew {amount} cards.");
                },

                //deal damage to target
                ["DealDamageToTarget"] = (card, context, damage) =>
                {
                    // TODO: later select between all targets
                    context.SelectEnemyTarget(context.ActivePlayer);
                    var target = context.TargetCard;
                    if (target == null)
                    {
                        return EffectResult.Fail("No target selected for DealDamage effect.");
                    }

                    target.DecreasePower(damage ?? 1);

                    if (target.Power == 0)
                    {
                        context.Opponent.Lanes.FirstOrDefault(l => l.CardsInLane.Contains(target))?.CardsInLane.Remove(target);
                        context.Opponent.Graveyard.Add(target);
                        context.Log($"{target.Name} has been destroyed and sent to the graveyard.");
                    }
                    return EffectResult.Ok($"{card.Name} dealt {damage} damage to {target.Name}.");
               
                },

                ["BuffCardToTarget"] = (card, context, buffAmount) =>
                {
                    var targetCard = context.TargetCard;
                    if (targetCard == null)
                    {
                        return EffectResult.Fail("No target selected for BuffCard effect.");
                    }
                    targetCard.IncreasePower(buffAmount ?? 1);
                    return EffectResult.Ok($"{card.Name} buffed {targetCard.Name} by {buffAmount}.");
                },

                ["DealDamageToSelf"] = (card, context, damage) =>
                {
                    if (!card.HasPower)
                    {
                        return EffectResult.Fail($"{card.Name} has no power to deal damage to itself.");
                    }
                    card.DecreasePower(damage ?? 1);
                    return EffectResult.Ok($"{card.Name} dealt {damage} damage to itself.");
                },

                ["BuffSelf"] = (card, context, buffAmount) =>
                {
                    if (card.Type != Domain.Enums.CardType.Climber)
                    {
                        return EffectResult.Fail($"{card.Name} is not a Climber, and therefore can't gain power.");
                    }
                    card.IncreasePower(buffAmount ?? 1);
                    return EffectResult.Ok($"{card.Name} buffed itself by {buffAmount}.");
                }
            };

        }

        public EffectResult Execute(string effectId, Card card, GameContext context, int? value = null)
        {
            if (effectRegistry.TryGetValue(effectId, out var effectFunc))
                return effectFunc(card, context, value);
            Console.WriteLine( "Func" + effectFunc(card, context, value));

            return EffectResult.Fail($"Effect '{effectId}' not found.");
        }

    }

}
