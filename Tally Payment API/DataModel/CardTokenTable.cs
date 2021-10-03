using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tally_Payment_API.DataModel
{
    public class CardTokenTable
    {
        [Key]
        public int Id { get; set; }
        public string email { get; set; }
        public string PhoneNumber { get; set; }
        public string embedtoken { get; set; }
        public string expirymonth { get; set; }
        public string expiryyear { get; set; }
        public string cardBIN { get; set; }
        public string last4digits { get; set; }
        public string type { get; set; }
    }
}
