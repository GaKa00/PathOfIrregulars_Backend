using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfIrregulars.Domain.ValueObjects
{
    public sealed class EffectResult
    {
        public bool Success { get; }
        public string Message { get; }
        public List<GameEvent> Events { get; }

        public EffectResult(bool success, string message, List<GameEvent> events = null)
        {
            Success = success;
            Message = message;
            Events = events ?? new();
        }

        public static EffectResult Ok(string message) => new(true, message);
        public static EffectResult Fail(string message) => new(false, message);
    }
}
