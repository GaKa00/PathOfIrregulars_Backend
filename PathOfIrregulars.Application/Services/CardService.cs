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

        public EffectResult PlayCard(Card card, GameContext context)
        {
            foreach (var effect in card.CardEffects)
            {
                var result = _registry.Execute(effect.EffectId, card, context);
           
            }

            return EffectResult.Ok($"{card.Name} played successfully.");
        }
    }
}
