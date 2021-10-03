using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding.Binders;
using Tally_Payment_API.DataModel;

namespace Tally_Payment_API.Repository.IRepository
{
    public class CardTokenRepo : ICardTokenRepository
    {
        private readonly DataContext _db;

        public CardTokenRepo(DataContext db)
        {
            _db = db;
        }

        public bool AddCardToken(CardTokenTable card)
        {
            //check if card exists
            if (!_db.CardTokenTable.Any(a => a.embedtoken == card.embedtoken)) 
            {
                _db.CardTokenTable.Add(card);
                return Save();
            };

            
            return Save();
        }

        public bool DeleteCardToken(int Id)
        {
            var cardToken = GetCardTokenById(Id);
            _db.CardTokenTable.Remove(cardToken);

            return Save();
        }

     

        public CardTokenTable GetCardTokenById(int Id)
        {
            return _db.CardTokenTable.SingleOrDefault(a => a.Id == Id);
        }

        public IEnumerable<CardTokenTable> GetCardTokensByEmail(string email)
        {
            return _db.CardTokenTable.Where(a => a.email == email).ToList();
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        public bool UpdateCardToken(CardTokenTable card)
        {
            _db.CardTokenTable.Update(card);
            return Save();
        }

     
    }
}
