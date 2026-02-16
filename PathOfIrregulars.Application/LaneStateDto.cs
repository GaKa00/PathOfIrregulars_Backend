using PathOfIrregulars.Domain.Entities;
using PathOfIrregulars.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfIrregulars.Application
{
    public class LaneStateDto
    {
        public LaneType LaneType { get; set; }
        public List<CardStateDto> Cards { get; set; }

        public static LaneStateDto From(Lane lane)
        {
            return new LaneStateDto
            {
                LaneType = lane.LaneType,
                Cards = lane.CardsInLane.Select(CardStateDto.From).ToList()
            };
        }
    }

}
