namespace Tally_Payment_API.Models
{
    public class VbvResponseObj : InitiateFinalResponse
    {
        public chargeToken chargeToken { get; set; }
    }

    public class chargeToken
    {
        public string user_token { get; set; }
        public string embed_token { get; set; }
    }
}