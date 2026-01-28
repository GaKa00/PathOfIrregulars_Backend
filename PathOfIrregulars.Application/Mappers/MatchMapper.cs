using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace PathOfIrregulars.Application.Mappers
{
    public static class MatchMapper
    {
        public static MatchDto ToDto(Guid matchId, Match match)
        {
            return new MatchDto
            {
                MatchId = matchId,
                GameState = match.GameState,
                TurnCount = match.TurnCount,
                ActivePlayer = match.ActivePlayer.Name,
                Logs = match.Logs.ToList(),

                PlayerOne = PlayerStateDto.From(match.PlayerOne),
                PlayerTwo = PlayerStateDto.From(match.PlayerTwo)
            };
        }
    }

}
