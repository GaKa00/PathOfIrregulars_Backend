using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathOfIrregulars.Domain.Enums;

namespace PathOfIrregulars.Domain.Entities
{
    public class Lane
    {
        public List<Card> CardsInLane { get; set; }
        public LaneType LaneType { get; set; }
    }
}
