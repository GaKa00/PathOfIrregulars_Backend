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
   public class EffectRegistry
    {

        private readonly Dictionary<string, Func<Card, GameContext, int?, EffectResult>> _executors;

       
        private readonly Dictionary<string, EffectDefinition> _definitions;


        public EffectDefinition GetDefinition(string effectId)
        {
            if (!_definitions.TryGetValue(effectId, out var def))
                throw new InvalidOperationException(
                    $"Effect definition '{effectId}' not found."
                );

            return def;
        }

        public EffectRegistry()
        {
            _executors = new();
            _definitions = new();

            RegisterEffects();
        }

        private void Register(string id,
            EffectDefinition definition,
            Func<Card, GameContext, int?, EffectResult> executor)
        {
            _executors[id] = executor;
            _definitions[id] = definition;
        }



        private void RegisterEffects()
        {
            Register(
                 "DrawCard",
                 new EffectDefinition
                 {
                     RequiresTarget = false,
                     AllowedTriggers = { EffectTrigger.OnPlay },
                     MaxAmount = 5
                 },
                 (card, context, amount) =>
                 {
                     int drawCount = amount ?? 1;

                     for (int i = 0; i < drawCount; i++)
                         context.ActivePlayer.DrawCard();

                     return EffectResult.Ok(
                         $"{context.ActivePlayer.Name} drew {drawCount} card(s)."
                     );
                 }
             );

            //deal damage to target
            Register(
                     "DealDamageToTarget",
                      new EffectDefinition
                      {
                          RequiresTarget = true,
                          AllowedTargets = { TargetType.EnemyCard, TargetType.OwnCard },
                          AllowedTriggers = { EffectTrigger.OnPlay, EffectTrigger.OnDeath, EffectTrigger.OnEquip },
                          AllowZeroAmount = false,
                          IsCancelable = true,
                          MaxAmount = 100
                      },
                      (card, context, damage) =>
                        {
                            context.SelectTargetCard();
                            var target = context.TargetCard;

                            if (target == null)
                                return EffectResult.Fail("No target selected.");

                            int dmg = damage ?? 1;
                            target.DecreasePower(dmg);

                            if (target.Power == 0)
                            {
                            //TODO: checked if target is in opponent's lane or active player's lane
                                var lane = context.Opponent.Lanes.FirstOrDefault(l => l.CardsInLane.Contains(target));
                                lane?.CardsInLane.Remove(target);
                                context.ResolveTrigger(EffectTrigger.OnDeath, target);
                                context.Opponent.Graveyard.Add(target);
                                context.Log($"{target.Name} was destroyed.");
                            }
                            return EffectResult.Ok(
                                $"{card.Name} dealt {dmg} damage to {target.Name}."
                            );
                        }
                        );

            Register(
                "BuffCardToTarget",
                new EffectDefinition
                {
                    RequiresTarget = true,
                    AllowedTargets = { TargetType.EnemyCard, TargetType.OwnCard },
                    AllowedTriggers = { EffectTrigger.OnPlay, EffectTrigger.OnDeath, EffectTrigger.OnEquip },
                    AllowZeroAmount = false,
                    IsCancelable = true,
                    MaxAmount = 100
                },
                (card, context, buffAmount) =>
                {
                    context.SelectTargetCard();
                    var targetCard = context.TargetCard;
                    if (targetCard == null)
                    {
                        return EffectResult.Fail("No target selected for BuffCard effect.");
                    }
                    targetCard.IncreasePower(buffAmount ?? 1);
                    return EffectResult.Ok($"{card.Name} buffed {targetCard.Name} by {buffAmount}.");
                }
                );

            Register(
                "DealDamageToSelf",
                new EffectDefinition
                {
                    RequiresTarget = false,
                    AllowedTriggers = { EffectTrigger.OnPlay, EffectTrigger.OnTurnStart },
                    MaxAmount = 100
                },
                (card, context, damage) =>
                {
                    if (!card.HasPower)
                    {
                        return EffectResult.Fail($"{card.Name} has no power to deal damage to itself.");
                    }
                    card.DecreasePower(damage ?? 1);

                   
                    if (card.Power == 0)
                    {
                        context.ActivePlayer.Lanes.FirstOrDefault(l => l.CardsInLane.Contains(card))?.CardsInLane.Remove(card);
                        context.ResolveTrigger(EffectTrigger.OnDeath, card, owner: context.ActivePlayer);
                        context.ActivePlayer.Graveyard.Add(card);
                        context.Log($"{card.Name} has been destroyed and sent to the graveyard.");
                    }
                    return EffectResult.Ok($"{card.Name} dealt {damage} damage to itself.");
                }

                );

            Register("BuffSelf",
                new EffectDefinition
                {
                    RequiresTarget = false,
                    AllowedTriggers = { EffectTrigger.OnPlay, EffectTrigger.OnTurnStart },
                    MaxAmount = 100
                },
                (card, context, buffAmount) =>
             {
                 if (card.Type != Domain.Enums.CardType.Climber)
                 {
                     return EffectResult.Fail($"{card.Name} is not a Climber, and therefore can't gain power.");
                 }
                 card.IncreasePower(buffAmount ?? 1);
                 return EffectResult.Ok($"{card.Name} buffed itself by {buffAmount}.");
             }
             );

            Register(
                "HealTarget",
                new EffectDefinition
                {
                    RequiresTarget = true,
                    AllowedTriggers = { EffectTrigger.OnPlay },
                    MaxAmount = 100
                },
                (card, context, healAmount) =>
                {
                    context.SelectTargetCard();
                    var targetCard = context.TargetCard;
                    if (targetCard == null)
                    {
                        return EffectResult.Fail("No target selected for HealTarget effect.");
                    }
                    targetCard.IncreasePower(healAmount ?? 1);
                    return EffectResult.Ok($"{card.Name} healed {targetCard.Name} by {healAmount}.");
                }
                );

            Register(
                "KillTarget",
                new EffectDefinition
                {
                    RequiresTarget = true,
                    AllowedTriggers = { EffectTrigger.OnPlay },
                    MaxAmount = null
                },
                (card, context, _) =>
                {
                    context.SelectTargetCard();
                    var target = context.TargetCard;
                    if (target == null)
                        return EffectResult.Fail("No target selected.");
                    var lane = context.Opponent.Lanes.FirstOrDefault(l => l.CardsInLane.Contains(target));
                    lane?.CardsInLane.Remove(target);
                    context.ResolveTrigger(EffectTrigger.OnDeath, target);
                    context.Opponent.Graveyard.Add(target);
                    return EffectResult.Ok(
                        $"{card.Name} killed {target.Name}."
                    );
                }
                );
        }



    public EffectResult Execute(
       string effectId,
       Card card,
       GameContext context,
       int? value = null
   )
    {
        if (_executors.TryGetValue(effectId, out var effectFunc))
            return EffectResult.Fail($"Effect '{effectId}' not found.");

        return effectFunc(card, context, value);
    }

        public bool Has(string effectId)
        {
            return _executors.ContainsKey(effectId);
        }
    };



}


