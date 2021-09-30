using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding.Binders;
using Tally_Payment_API.DataModel;

namespace Tally_Payment_API.Repository.IRepository
{
    public class TransactionRepo : ITransactionRepository
    {
        private readonly DataContext _db;

        public TransactionRepo(DataContext db)
        {
            _db = db;
        }

        public bool AddTransaction(Transactions transaction)
        {
            _db.Transactions.Add(transaction);
            return Save();
        }

        public Transactions GetTransaction(string TransactionRef)
        {
            if (TransactionRef != null)
            {
               return _db.Transactions.SingleOrDefault(a => a.transactionRef == TransactionRef);
            }

            return null;
        }

        public ICollection<ITransactionRepository> GetTransactions()
        {
            throw new NotImplementedException();
        }

        public ICollection<ITransactionRepository> GetUserTransactions(string UserId)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        public bool UpdateTransaction(Transactions transaction)
        {
            throw new NotImplementedException();
        }
    }
}
