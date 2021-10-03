using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tally_Payment_API.Models
{
    public class TokenChargeObj
    {

        public string SECKEY { get; set; }
        public string token { get; set; }
        [Required]
        public string email { get; set; }
        public string currency { get; set; }
        public string country { get; set; }
        [Required]
        public double amount { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string IP { get; set; }
        public string narration { get; set; }
        public string txRef { get; set; }
        public List<MetaObj> meta { get; set; }
        public string device_fingerprint { get; set; }
        public string payment_plan { get; set; }
        public int CardId { get; set; }
        public string UniqueString { get; set; }
    }
}
