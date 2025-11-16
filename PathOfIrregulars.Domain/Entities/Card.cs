using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using PathOfIrregulars.Domain.Enums;
using PathOfIrregulars.Domain.ValueObjects;

namespace PathOfIrregulars.Domain.Entities
{
   public class Card
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public CardType Type { get; set; }

        public int GetPower() => Power?.Value ?? 0;

        public PowerValue? Power { get; set; }
        public bool HasPower => Power != null;
        public List<CardEffect> CardEffects { get; set; } = new();

        public void IncreasePower(int amount)
        {
            if (Power == null)
                Power = new PowerValue(amount);
            else
                Power.Increase(amount);
        }

        public void DecreasePower(int amount)
        {
            Power?.Decrease(amount);
        }
    }
}






