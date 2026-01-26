using PathOfIrregulars.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfIrregulars.Application.Services
{
   public class EffectContext
    {
        public Match Match { get; }
        public Player SourcePlayer { get; }
        public Player Opponent => Match.Opponent;

        public Card? TargetCard { get; set; }

        public EffectContext(Match match, Player sourcePlayer)
        {
            Match = match;
            SourcePlayer = sourcePlayer;
        }

        public Board Board => Match.Board;
        public void Log(string msg) => Match.Log(msg);
    }
}
