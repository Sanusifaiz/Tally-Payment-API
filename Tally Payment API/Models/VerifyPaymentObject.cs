using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tally_Payment_API.Models
{
    public class VerifyPaymentObject
    {
        public string txref { get; set; }
        public string SECKEY { get; set; }
    }
}
