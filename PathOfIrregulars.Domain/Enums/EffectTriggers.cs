using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfIrregulars.Domain.Enums
{
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
