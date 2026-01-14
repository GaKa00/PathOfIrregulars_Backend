using PathOfIrregulars.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfIrregulars.Application.Contracts
{
   public interface ICardRepository
    {
        Card GetCard(string id);
        IEnumerable<Card> GetAllCards();
    }
}
