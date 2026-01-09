using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PathOfIrregulars.Infrastructure.Database
{
public class GameDbContext : DbContext
    {

        public DbSet<Domain.Entities.Player> Players { get; set; }
        public DbSet

        public GameDbContext(DbContextOptions<GameDbContext> options)
    : base(options) { }
    }
}
