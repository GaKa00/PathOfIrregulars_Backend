using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfIrregulars.Infrastructure.Database.Models
{
   public class Deck
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int AccountId { get; set; }
        public List<Card> Cards { get; set; } = new();
    }
}
