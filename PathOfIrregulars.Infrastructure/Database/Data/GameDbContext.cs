using Microsoft.EntityFrameworkCore;
using PathOfIrregulars.Infrastructure.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PathOfIrregulars.Infrastructure.Database.Data
{
public class GameDbContext : DbContext
    {

        public DbSet<Models.UserProfile> Players { get; set; }
        public DbSet <Models.Deck> Decks { get; set; }
        public DbSet<Models.DeckCard> DeckCards { get; set; }

        public DbSet<Match> Matches => Set<Match>();
        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<DeckCard>()
                .HasKey(dc => new { dc.DeckId, dc.CardId });
        }


        public GameDbContext(DbContextOptions<GameDbContext> options)
    : base(options) { }
    }
}
