using PathOfIrregulars.Domain;
using PathOfIrregulars.Domain.Enums;

namespace PathOfIrregulars.Application
{
    public class MatchDto
    {

        public Guid MatchId { get; set; }
        public GameState GameState { get; set; } 
        public int TurnCount { get; set; }
        public string ActivePlayer { get; set; }
        public List<string> Logs { get; set; } = new();
        public PlayerStateDto PlayerOne { get; set; } = new();
        public PlayerStateDto PlayerTwo { get; set; } = new();
    }
}
