using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PathOfIrregulars.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EffectTrigger
    {
        OnPlay,
        OnTurnStart,
        OnTurnEnd,
        OnDeath,
        OnLaneEnter,
        OnLaneLeave,
        OnEquip
    }
};
