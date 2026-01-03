using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfIrregulars.Domain.Enums
{
  public enum TargetType
    {
        None,
        Self,
        OwnCard,
        EnemyCard,
        AnyCard,
        OwnLane,
        EnemyLane,
        AnyLane,
        Player,
        EnemyPlayer,
       
    }
}
