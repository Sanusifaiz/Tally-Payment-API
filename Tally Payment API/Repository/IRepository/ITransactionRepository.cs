using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tally_Payment_API.DataModel;

namespace Tally_Payment_API.Repository.IRepository
{
    public interface ITransactionRepository
    {
        ICollection<ITransactionRepository> GetTransactions();
        Transactions GetTransaction(string TransactionRef);
        ICollection<ITransactionRepository> GetUserTransactions(string UserId);
        bool AddTransaction(Transactions transaction);
        bool UpdateTransaction(Transactions transaction);
        bool Save();
    }
}
