using PathOfIrregulars.Domain.Entities;

namespace PathOfIrregulars.Infrastructure.Database.Models
{
    public class UserProfile
    {
     
            public Guid Id { get; set; }
            public string Username { get; set; }
            public string PasswordHash { get; set; }
            public int Elo { get; set; } = 1000;
            public DateTime CreatedAt { get; set; }
        

        public Dictionary<string, List<Deck>> SavedDecks { get; set; }
            = new();
    }

}