using PathOfIrregulars.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfIrregulars.Application
{
    public class CardStateDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Power { get; set; }

        public static CardStateDto From(Card card)
        {
            return new CardStateDto
            {
                Id = card.Id,
                Name = card.Name,
                Power = card.Power
            };
        }
    }

}
