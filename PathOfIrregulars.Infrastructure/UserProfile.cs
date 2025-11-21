using PathOfIrregulars.Domain.Entities;

namespace PathOfIrregulars.Infrastructure.Profiles
{
    public class UserProfile
    {
        public string UserId { get; set; }
        public string Username { get; set; }

        public Dictionary<string, List<string>> SavedDecks { get; set; }
            = new();
    }

}