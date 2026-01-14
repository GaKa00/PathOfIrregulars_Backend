using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfIrregulars.Infrastructure.Database.Models
{
    public class DeckCard
    {
        public Guid DeckId { get; set; }
        public string CardId { get; set; } 
        public int Quantity { get; set; }
    }
}
