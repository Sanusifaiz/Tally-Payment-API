using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tally_Payment_API.Models
{
    public class Data
    {
        public string userId { get; set; }
        public double amount { get; set; }
        public DateTime created { get; set; }
        public string paymentLink { get; set; }
    }
    public class ResponseMessage
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public Data Data { get; set; }
        public object Data2 { get; set; }
    }

   
}
