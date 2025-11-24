using Microsoft.VisualBasic;
using PathOfIrregulars.Domain.Enums;
using PathOfIrregulars.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PathOfIrregulars.Domain.Entities
{
   public class Card
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CardType Type { get; set; }
        public int Power { get; set; }
        public bool HasPower => Power > 0;

        [JsonPropertyName("Effects")]
        public List<CardEffect> CardEffects { get; set; } = new();

        public void IncreasePower(int amount)
        {
         
                Power += amount;
        }

        public void DecreasePower(int amount)
        {
            Power = Math.Max(0, Power - amount);
        }

        public Card Clone()
        {
            return new Card
            {
                Id = this.Id,
                Name = this.Name,
                Type = this.Type,
                Power= this.Power,
                    
                CardEffects = this.CardEffects
                    ?.Select(e => new CardEffect
                    {
                        EffectId = e.EffectId,
                        Parameters = new Dictionary<string, int>(e.Parameters)
                    })
                    .ToList()
            };
        }
    }
}






