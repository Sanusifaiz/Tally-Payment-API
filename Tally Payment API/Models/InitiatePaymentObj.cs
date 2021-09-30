using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tally_Payment_API.Models
{
    public class InitiatePaymentObj
    {
        public string PBFPubKey { get; set; }
        public string client { get; set; }
        public string alg { get; set; }
    }
}
