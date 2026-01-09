using PathOfIrregulars.Domain.Entities;

namespace PathOfIrregulars.Infrastructure.Profiles
{
    public class UserProfile
    {
     
            public Guid Id { get; set; }
            public string Username { get; set; }
            public string PasswordHash { get; set; }
            public int Elo { get; set; } = 1000;
            public DateTime CreatedAt { get; set; }
        

        public Dictionary<string, List<string>> SavedDecks { get; set; }
            = new();
    }

}