using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfIrregulars.Application.Contracts
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Guid> DeckIds { get; set; } = new();
    }
}
