using PathOfIrregulars.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfIrregulars.Application.Services
{
    public class EffectDefinition
    {
        public bool RequiresTarget { get; init; }
        public  HashSet<TargetType> AllowedTargets { get; init; } = new HashSet<TargetType>();
        public HashSet<EffectTrigger> AllowedTriggers { get; init; } = new HashSet<EffectTrigger>();
        public bool AllowZeroAmount { get; init; }
        public int? MaxAmount { get; init; }
        public bool IsCancelable { get; init; }


    }
}
