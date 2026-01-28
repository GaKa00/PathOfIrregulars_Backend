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

        public static CardStateDto From(CardInstance card)
        {
            return new CardStateDto
            {
                Id = card.Definition.Id,
                Name = card.Definition.Name,
                Power = card.Power
            };
        }
    }

}
