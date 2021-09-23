using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tally_Payment_API.DataModel
{
    public class UserPaymentModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public double Amount { get; set; }
        [MaxLength(3)]
        [Required]
        public string Currency { get; set; }

        public DateTime Created { get; set; }
        public string Status { get; set; }
        [Required]
        public string From { get; set; }
        [Required]
        public string Means { get; set; }
        public DateTime Expiry { get; set; }
        [Required]
        public string FeesBearer { get; set; }
        [Required]
        public string Payer { get; set; }
        public string Details { get; set; }
        public string PaymentLink { get; set; }
        public string RandomString { get; set; }
        public bool Sendmail { get; set; }
    }
}
