using PathOfIrregulars.Domain.Entities;
using PathOfIrregulars.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfIrregulars.Domain
{
    public class CardDefinition
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public CardType Type { get; set; }
        public int BasePower { get; set; }
        public List<CardEffect> Effects { get; set; }
    }

}
