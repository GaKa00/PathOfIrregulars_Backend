using PathOfIrregulars.API.Contracts;
using System.Collections.Generic;

namespace PathOfIrregulars.API.Contracts;

public class AccountDto
{
    public AccountDto() { }

    public AccountDto(int id, string username, int elo, List<DeckDto> decks)
    {
        Id = id;
        Username = username;
        Elo = elo;
        Decks = decks ?? new List<DeckDto>();
    }

    public int Id { get; set; }
    public string? Username { get; set; }
    public int Elo { get; set; } = 1000;
    public List<DeckDto> Decks { get; set; } = new List<DeckDto>();
}
