using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathOfIrregulars.Domain.Enums;

namespace PathOfIrregulars.Domain.Entities
{
    public class Lane
    {
        public List<CardInstance> CardsInLane { get; set; }
        public LaneType LaneType { get; set; }

        public Lane(LaneType type)
        {
            LaneType = type;
            CardsInLane = new List<CardInstance>();
        }

        public int Power { get; private set; }
    public void AddPower(  int amount)
    {
        Power += amount;
        }

        public void RemovePower(int amount)
        {
            Power += amount;
        }


        // Calculate total power in the lane
        public int CalculateLanePower()
        {
            return CardsInLane
                .Where(c => c.Definition.Power != null)
                .Sum(c => c.Power);
        }
    }



    }
