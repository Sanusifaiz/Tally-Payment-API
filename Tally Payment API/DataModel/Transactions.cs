using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tally_Payment_API.DataModel
{
    public class Transactions
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string transactionRef { get; set; }
        
        public string orderRef { get; set; }
   
        public string flwRef { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string authModelUsed { get; set; }
        public string PaymentType { get; set; }
        [Required]
        public double Amount { get; set; }
        [Required]
        public double charged_amount { get; set; }
        public double appfee{ get; set; }
        public double merchantfee { get; set; }
        public string chargeResponseCode { get; set; }
        public string chargeResponseMessage { get; set; }
        public string  narration { get; set; }
        public string  authurl { get; set; }
        public string status { get; set; }
        public string Currency { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
       
        public string PayerEmail { get; set; }
        public string PayerName { get; set; }
        public string PayerPhone { get; set; }
        public string UserId { get; set; }
        public string CardDetails { get; set; }
        public string metaname { get; set; }
        public string metavalue { get; set; }
        public string paymentLinkUniqueString { get; set; }
    }
}
