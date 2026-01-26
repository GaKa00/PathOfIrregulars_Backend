using PathOfIrregulars.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfIrregulars.Application
{
    public class PlayerStateDto
    {
        public string Name { get; set; }
        public int HandSize { get; set; }
        public int DeckSize { get; set; }
        public int TotalPower { get; set; }
        public int WonRounds { get; set; }
        public bool HasPassed { get; set; }

        public List<LaneStateDto> Lanes { get; set; }

        public static PlayerStateDto From(Player player)
        {
            return new PlayerStateDto
            {
                Name = player.Name,
                HandSize = player.Hand.Count,
                DeckSize = player.Deck.Count,
                TotalPower = player.TotalPower,
                WonRounds = player.WonRounds,
                HasPassed = player.HasPassed,
                Lanes = player.Lanes.Select(LaneStateDto.From).ToList()
            };
        }
    }

}
