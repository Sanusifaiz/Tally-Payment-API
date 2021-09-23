using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Tally_Payment_API.DataModel;

namespace Tally_Payment_API.Models
{
    public class UserRequestModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public double Amount { get; set; }
        [Required]
        public string Means { get; set; }
        [Required]
        public string From { get; set; }
        [Required]
        public string FeesBearer { get; set; }
        [Required]
        public string Payer { get; set; }
        [MinLength(3)]
        [MaxLength(3)]
        [Required]
        public string Currency { get; set; }
        public DateTime Expiry { get; set; }
    }
}
