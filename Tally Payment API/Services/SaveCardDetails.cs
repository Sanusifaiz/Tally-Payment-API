using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tally_Payment_API.DataModel;
using Tally_Payment_API.Models;
using Tally_Payment_API.Repository.IRepository;

namespace Tally_Payment_API.Services
{
    public class SaveCardDetails
    {
        private protected ICardTokenRepository _cardTokenRepo;

     

        public SaveCardDetails(ICardTokenRepository CardTokenRepo)
        {
            _cardTokenRepo = CardTokenRepo;
        }

        public string SaveCard(string email, string phone, string token)
        {
            if (email != null && token != null)
            {
                var cardTokenObj = new CardTokenTable
                {
                    email = email,
                    PhoneNumber = phone,
                    embedtoken = token
                };
                
                if (_cardTokenRepo.AddCardToken(cardTokenObj))
                {
                    return "Card Saved";
                }
                return "Card Not Saved";
            }
            return "Card Not Saved";
        }
    }
}
