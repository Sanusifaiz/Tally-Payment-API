using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tally_Payment_API.Models
{
    public class CardDetails
    {
        [Required]
        public string PBFPubKey { get; set; }
        [Required]
        public string cardno { get; set; }
        [Required]
        [StringLength(3)]
        public string cvv { get; set; }
        [Required]
        [StringLength(2)]
        public string expirymonth { get; set; }
        [Required]
        [StringLength(2)]
        public string expiryyear { get; set; }
        
        public string currency { get; set; }
        public string country { get; set; }
        [Required]
        public string amount { get; set; }
        [Required]
        public string email { get; set; }
        public string phonenumber { get; set; }
        public string pin { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string IP { get; set; }
        public string txRef { get; set; }
        public List<Meta> meta { get; set; }
        public string redirect_url { get; set; }
        public string device_fingerprint { get; set; }
        public string suggested_auth { get; set; }
        public string billingzip { get; set; }
        public string billingcity { get; set; }
        public string billingaddress { get; set; }
        public string billingstate { get; set; }
        public string billingcountry { get; set; }
        public string FrontEndUrl { get; set; }
        public bool SaveCard { get; set; }


    }

    public  class Meta
    {
        public string metaname { get; set; }
        public string metavalue { get; set; }
    }
}
