using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfIrregulars.Domain.Entities
{
    
        public class CardInstance
        {
            public string InstanceId { get; set; } = Guid.NewGuid().ToString();

            
            public Card Definition { get; set; }

           
            public int Power { get; set; }
            public bool IsDestroyed { get; set; }
            public bool IsUntargetable { get; set; }

            public CardInstance? EquippedTo { get; set; }
            public List<CardInstance> EquippedArtifacts { get; set; } = new();

            public bool HasPower => Power > 0;

            public void IncreasePower(int amount)
            {
                Power += amount;
            }

            public void DecreasePower(int amount)
            {
                Power = Math.Max(0, Power - amount);
            }
        }
    }


