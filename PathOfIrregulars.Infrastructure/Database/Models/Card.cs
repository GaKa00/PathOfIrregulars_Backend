using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfIrregulars.Infrastructure.Database.Models
{
    public class Card
    {

        public int Id { get; set; }
        public string CardId { get; set; }
        public int DeckId { get; set; }
        public int Amount { get; set; }
    }
}
