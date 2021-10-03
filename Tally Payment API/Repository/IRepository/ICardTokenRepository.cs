using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tally_Payment_API.DataModel;

namespace Tally_Payment_API.Repository.IRepository
{
    public interface ICardTokenRepository
    {

        CardTokenTable GetCardTokenById(int id);

        IEnumerable<CardTokenTable> GetCardTokensByEmail(string email);
        bool AddCardToken(CardTokenTable card);
        bool UpdateCardToken(CardTokenTable card);
        bool DeleteCardToken(int Id);
        bool Save();
    }
}
