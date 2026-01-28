using Microsoft.EntityFrameworkCore;
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

        private readonly Dictionary<string, Func<CardInstance, Match, int?,  CardInstance?, EffectResult>> _executors;

       
        private readonly Dictionary<string, EffectDefinition> _definitions;

        // gets effect definition for selected effect
        public EffectDefinition GetDefinition(string effectId)
        {
            if (!_definitions.TryGetValue(effectId, out var def))
                throw new InvalidOperationException(
                    $"Effect definition '{effectId}' not found."
                );

            return def;
        }
        // builds registry and registers all effects
        public EffectRegistry()
        {
            _executors = new();
            _definitions = new();

            RegisterEffects();
        }

        // helper method to register effects into the registry
        private void Register(string id,
            EffectDefinition definition,
            Func<CardInstance, Match, int?, CardInstance?, EffectResult> executor)
        {
            _executors[id] = executor;
            _definitions[id] = definition;
        }



        private void RegisterEffects()
        {

            // drawcard - limited to max 5 cards, can be triggered on play only, and will only draw for active player (for now)
            Register(
                 "DrawCard",
                 new EffectDefinition
                 {
                     RequiresTarget = false,
                     AllowedTriggers = { EffectTrigger.OnPlay },
                     MaxAmount = 5
                 },
                 (card, context, amount, target) =>
                 {
                     int drawCount = amount ?? 1;

                     for (int i = 0; i < drawCount; i++)
                         context.ActivePlayer.DrawCard();

                     return EffectResult.Ok(
                         $"{context.ActivePlayer.Name} drew {drawCount} card(s)."
                     );
                 }
             );

            // deal damage to target card effect, requires a target that can be either enemy or own card, can be triggered on play, death or equip (in case of artifact), 
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
                      (card, context, damage, target) =>
                        {
                            
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
                                context.Log($"{target.Definition.Name} was destroyed.");
                            }
                            return EffectResult.Ok(
                                $"{card.Definition.Name} dealt {dmg} damage to {target.Definition.Name}."
                            );
                        }
                        );
            // buff target card effect, requires a target that can be either enemy or own card, can be triggered on play, death or equip (in case of artifact),
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
                (card, context, buffAmount, target) =>
                {
                    
                    if (target == null)
                    {
                        return EffectResult.Fail("No target selected for BuffCard effect.");
                    }
                    target.IncreasePower(buffAmount ?? 1);
                    return EffectResult.Ok($"{card.Definition.Name} buffed {target.Definition.Name} by {buffAmount}.");
                }
                );
            // deal damage to self effect, cannot target other cards, can be triggered on play or turn start (typically a setback for big power cards)
            Register(
                "DealDamageToSelf",
                new EffectDefinition
                {
                    RequiresTarget = false,
                    AllowedTriggers = { EffectTrigger.OnPlay, EffectTrigger.OnTurnStart },
                    MaxAmount = 100
                },
                (card, context, damage, target) =>
                {
                    if (!card.HasPower)
                    {
                        return EffectResult.Fail($"{card.Definition.Name} has no power to deal damage to itself.");
                    }
                    card.DecreasePower(damage ?? 1);

                   
                    if (card.Power == 0)
                    {
                        context.ActivePlayer.Lanes.FirstOrDefault(l => l.CardsInLane.Contains(card))?.CardsInLane.Remove(card);
                        context.ResolveTrigger(EffectTrigger.OnDeath, card, owner: context.ActivePlayer);
                        context.ActivePlayer.Graveyard.Add(card);
                        context.Log($"{card.Definition.Name} has been destroyed and sent to the graveyard.");
                    }
                    return EffectResult.Ok($"{card.Definition.Name} dealt {damage} damage to itself.");
                }

                );

            // buff self effect, cannot target other cards, can be triggered on play or turn start
            Register("BuffSelf",
                new EffectDefinition
                {
                    RequiresTarget = false,
                    AllowedTriggers = { EffectTrigger.OnPlay, EffectTrigger.OnTurnStart },
                    MaxAmount = 100
                },
                (card, context, buffAmount, target) =>
             {
                 if (card.Definition.Type != CardType.Climber)
                 {
                     return EffectResult.Fail($"{card.Definition.Name} is not a Climber, and therefore can't gain power.");
                 }
                 card.IncreasePower(buffAmount ?? 1);
                 return EffectResult.Ok($"{card.Definition.Name} buffed itself by {buffAmount}.");
             }
             );

            // heal target effect, requires a target that can be either enemy or own card, can be triggered on play
            Register(
                "HealTarget",
                new EffectDefinition
                {
                    RequiresTarget = true,
                    AllowedTriggers = { EffectTrigger.OnPlay },
                    MaxAmount = 100
                },
                (card, context, healAmount, target) =>
                {
                 
                    if (target == null)
                    {
                        return EffectResult.Fail("No target selected for HealTarget effect.");
                    }
                    target.IncreasePower(healAmount ?? 1);
                    return EffectResult.Ok($"{card.Definition.Name} healed {target.Definition.Name} by {healAmount}.");
                }
                );

            // kill target effect, requires a target that can be either enemy or own card, can be triggered on play
            Register(
                "KillTarget",
                new EffectDefinition
                {
                    RequiresTarget = true,
                    AllowedTriggers = { EffectTrigger.OnPlay },
                    MaxAmount = null
                },
                (card, context, _, target) =>
                {
                   ;
                    if (target == null)
                        return EffectResult.Fail("No target selected.");
                    var lane = context.Opponent.Lanes.FirstOrDefault(l => l.CardsInLane.Contains(target));
                    lane?.CardsInLane.Remove(target);
                    context.ResolveTrigger(EffectTrigger.OnDeath, target);
                    context.Opponent.Graveyard.Add(target);
                    return EffectResult.Ok(
                        $"{card.Definition.Name} killed {target.Definition.Name}."
                    );
                }
                );
        }

        // executes effect by id
        public EffectResult Execute(
       string effectId,
       CardInstance card,
       Match context,
       int? value = null,
       CardInstance? targetCard = null
   )
    {
        if (!_executors.TryGetValue(effectId, out var effectFunc))
            return EffectResult.Fail($"Effect '{effectId}' not found.");

        return effectFunc(card, context, value , targetCard);
    }


        public bool Has(string effectId)
        {
            return _executors.ContainsKey(effectId);
        }
    };



}


