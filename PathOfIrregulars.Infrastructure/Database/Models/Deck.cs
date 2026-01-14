using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfIrregulars.Infrastructure.Database.Models
{
   public class Deck
    {

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public List<DeckCard> Cards { get; set; } = new();
    }
}
