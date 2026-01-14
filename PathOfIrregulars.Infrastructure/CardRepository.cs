using System.Text.Json;
using PathOfIrregulars.Domain.Entities;
using PathOfIrregulars.Application.Contracts;

namespace PathOfIrregulars.Infrastructure.Persistence
{
 
    public class CardDatabaseFile
    {
        public List<Card> Cards { get; set; } = new();
    }

    public class CardRepository : ICardRepository
    {
        private Dictionary<string, Card> _cardsById;

        public CardRepository(string jsonPath = "cards.json")
        {
            LoadCardsFromJson(jsonPath);
        }

        private void LoadCardsFromJson(string jsonPath)
        {
            if (!File.Exists(jsonPath))
                throw new FileNotFoundException($"cards.json not found at: {jsonPath}");

            string json = File.ReadAllText(jsonPath);

            var db = JsonSerializer.Deserialize<CardDatabaseFile>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (db == null || db.Cards == null)
                throw new Exception("Cards.json is empty or malformed.");

            // store by Id
            _cardsById = db.Cards.ToDictionary(c => c.Id);
        }

        public Card GetCardById(string id)
        {
            if (!_cardsById.TryGetValue(id, out var card))
                throw new InvalidOperationException($"Card ID '{id}' not found.");

            return card.Clone();  
        }

     
        public IEnumerable<Card> GetAll() => _cardsById.Values;

        public Card GetCard(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Card> GetAllCards()
        {
            throw new NotImplementedException();
        }
    }
}