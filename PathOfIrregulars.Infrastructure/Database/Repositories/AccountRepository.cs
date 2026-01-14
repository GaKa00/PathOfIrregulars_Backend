using Microsoft.EntityFrameworkCore;
using PathOfIrregulars.Application.Contracts;
using PathOfIrregulars.Infrastructure.Database.Data;
using PathOfIrregulars.Infrastructure.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfIrregulars.Infrastructure.Database.Repositories
{
    public class AccountRepository
    {
        private readonly GameDbContext _context;

        public AccountRepository(GameDbContext context)
        {
            _context = context;
        }

        public AccountDto GetAccount(Guid accountId)
        {
            var account = _context.Players
                .Include(p => p.SavedDecks)
                .FirstOrDefault(p => p.Id == accountId)
                ?? throw new InvalidOperationException("Account not found");

            var deckIds = account.SavedDecks
               .SelectMany(kvp => kvp.Value)
               .Select(deck => deck.Id)
               .ToList();

            return new AccountDto
            {
                Id = account.Id,
                Username = account.Username,
                Rating = account.Elo,
                CreatedAt = account.CreatedAt,
                DeckIds = deckIds
            };
        }
    }

}
