using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tally_Payment_API.DataModel;

namespace Tally_Payment_API.Repository.IRepository
{
    public interface IUserPaymentRepository
    {
        ICollection<IUserPaymentRepository> GetUserPaymentRepositories();
        UserPaymentModel GetUserPayment(string UserId);
        UserPaymentModel GetUserPaymentByUniqueString(string UniqueString);
        bool CreateUserPayment(UserPaymentModel userPayment);
        bool UpdateUserPayment(UserPaymentModel userPayment);
        bool DeleteUserPayment(UserPaymentModel userPayment);
        bool Save();
    }
}
