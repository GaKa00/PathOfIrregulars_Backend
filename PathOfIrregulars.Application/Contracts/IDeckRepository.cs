using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfIrregulars.Application.Contracts
{
    public interface IDeckRepository
    {
    DeckDto GetDeck(Guid deckId);
    }
}
