using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding.Binders;
using Tally_Payment_API.DataModel;

namespace Tally_Payment_API.Repository.IRepository
{
    public class UserPaymentRepo : IUserPaymentRepository
    {
        private readonly DataContext _db;

        public UserPaymentRepo(DataContext db)
        {
            _db = db;
        }


        public bool CreateUserPayment(UserPaymentModel userPayment)
        {
            
            _db.userPaymentModels.Add(userPayment);
            return Save();
        }

        public bool DeleteUserPayment(UserPaymentModel userPayment)
        {
            throw new NotImplementedException();
        }

        public UserPaymentModel GetUserPayment(string UserId)
        {
            var output = _db.userPaymentModels.SingleOrDefault(a => a.UserId == UserId);

            return output;
        }

        public UserPaymentModel GetUserPaymentByUniqueString(string UniqueString)
        {
            return _db.userPaymentModels.SingleOrDefault(a => a.RandomString == UniqueString);
        }

        public ICollection<IUserPaymentRepository> GetUserPaymentRepositories()
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        public bool UpdateUserPayment(UserPaymentModel userPayment)
        {
            _db.userPaymentModels.Update(userPayment);
            return Save();
        }
    }
}
