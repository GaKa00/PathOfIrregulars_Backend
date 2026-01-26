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
    }
}
