using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfIrregulars.Domain.Entities
{
   public class CardEffect
    {
        public string EffectId { get; set; }
        public Dictionary<string, object> parameters { get; set; }
    }
}
