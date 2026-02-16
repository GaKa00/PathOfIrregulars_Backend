using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfIrregulars.Domain.Entities
{
    public class Board
    {
        public List<Lane> PlayerOneLanes { get; set; }
        public List<Lane> PlayerTwoLanes { get; set; }

        public IEnumerable<CardInstance> CardsInPlay
            => PlayerOneLanes.SelectMany(l => l.CardsInLane)
            .Concat(PlayerTwoLanes.SelectMany(l => l.CardsInLane));
    }
}
