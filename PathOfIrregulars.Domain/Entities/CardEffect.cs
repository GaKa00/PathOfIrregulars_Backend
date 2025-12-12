using PathOfIrregulars.Domain.Enums;
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
        [JsonPropertyName("EffectTrigger")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EffectTrigger Trigger { get; set; }

        public string EffectId { get; set; }

        public Dictionary<string, int> Parameters { get; set; } = new();

        public int GetAmount() =>
            Parameters.TryGetValue("amount", out var val) ? val : 0;
    }

}
