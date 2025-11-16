using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfIrregulars.Domain.ValueObjects
{
    public class GameEvent
    {
        public string Type { get; init; }
        public string Source { get; init; }
        public string Target { get; init; }
        public int? Value { get; init; }

        public GameEvent(string type, string source, string target, int? value = null)
        {
            Type = type;
            Source = source;
            Target = target;
            Value = value;
        }
    }
}
