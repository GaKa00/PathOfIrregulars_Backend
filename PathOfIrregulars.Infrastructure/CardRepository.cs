using System.Text.Json;
using PathOfIrregulars.Domain.Entities;

namespace PathOfIrregulars.Infrastructure;

public class CardRepository
{
    private List<Card> _cards;

    public CardRepository()
    {
        _cards = new List<Card>();
        LoadCardsFromJson();
    }

    private void LoadCardsFromJson()
    {
        string json = File.ReadAllText("Data/Cards/cards.json");
        _cards = JsonSerializer.Deserialize<List<Card>>(json)
            ?? new List<Card>();
    }

    public Card GetCardByName(string name)
    {
        return _cards.FirstOrDefault(c => c.Name == name)
               ?? throw new InvalidOperationException($"Card {name} not found.");
    }

    public IEnumerable<Card> GetAllCards() => _cards;
}
