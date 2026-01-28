using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfIrregulars.Application
{
    // will track ongoing matches, take reponsibility for starting, updating , and ending matches - todo

   public class MatchStore
    {

        public static Dictionary<Guid, Match> OngoingMatches { get; } = new Dictionary<Guid, Match>();

        public Guid Create(Match match)
        {
            var id = Guid.NewGuid();
            OngoingMatches[id] = match;
            return id;
        }

        public Match? Get(Guid matchId)
        {
            OngoingMatches.TryGetValue(matchId, out var match);
            return match;
        }

        public void Remove(Guid matchId)
        {
            OngoingMatches.Remove(matchId);
        }


    }
}
