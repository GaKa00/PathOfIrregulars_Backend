using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PathOfIrregulars.Domain.Entities
{
    public class CardEffect
    {
        public string EffectId { get; set; }
        public Dictionary<string, int> Parameters { get; set; } = new();

        public int GetAmount()
        {
            if (Parameters.TryGetValue("amount", out var v))
                return v;
            return 0;
        }
    }
}
