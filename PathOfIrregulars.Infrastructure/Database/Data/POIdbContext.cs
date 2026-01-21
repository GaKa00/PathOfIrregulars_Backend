using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfIrregulars.Infrastructure.Database.Data
{
    public class POIdbContext : DbContext
    {
        public POIdbContext(DbContextOptions<POIdbContext> options) : base(options)
        {
        }
        public DbSet<Models.Deck> Decks { get; set; }
        public DbSet<Models.Card> Cards { get; set; }
        public DbSet<Models.Account> Accounts { get; set; }
        public DbSet<Models.Match> Matches { get; set; }
    }
}
