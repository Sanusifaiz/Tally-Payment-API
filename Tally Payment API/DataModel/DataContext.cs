using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tally_Payment_API.DataModel
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<UserPaymentModel> userPaymentModels { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<CardTokenTable> CardTokenTable { get; set; }
    }
}
