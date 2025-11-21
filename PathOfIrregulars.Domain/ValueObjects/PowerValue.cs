//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PathOfIrregulars.Domain.ValueObjects
//{
//    public sealed class PowerValue
//    {
//        public int Value { get; private set; }

//        public PowerValue(int value)
//        {
//            if (value < 0) throw new ArgumentException("Power cannot be negative.");
//            Value = value;
//        }

//        public int Increase(int amount) => new(Value + amount);
//        public int Decrease(int amount) => new(Math.Max(0, Value - amount));
//    }
//}
