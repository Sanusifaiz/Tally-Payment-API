using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tally_Payment_API.Models
{
    public class ValidatePyamentObject
    {
        public string PBFPubKey { get; set; }
        public string transaction_reference { get; set; }
        public string otp { get; set; }
    }
}
