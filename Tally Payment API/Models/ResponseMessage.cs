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
        public string inData { get; set; }
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

    public class InitiatePaymentResponseMessage
    {
        public string StatusCode { get; set; }
        public string ResponseMessage { get; set; }
        public string authUrl { get; set; }
        public string authModelUsed { get; set; }
        public string TransactionRef { get; set; }
    }

    public class ValidatePaymentREsponseMessage : InitiatePaymentResponseMessage
    {
        public string FlutterwaveRef { get; set; }
        public double amount { get; set; }
        public double charged_amount { get; set; }
        public string status { get; set; }
    }

    public class VerifyPaymentResponseMessage : ValidatePaymentREsponseMessage 
    {

    }

    public class InitiatePaymentResponseForNigerianMasterAndVerveCard
    {
        public string status { get; set; }
        public string message { get; set; }
        public  suggestedAuth data { get; set; }
    }

    public class ValidatePaymentFinalResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public validatepaymentObj data { get; set; }
    }


    public class VerifyPaymentFinalResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public verifyPaymentObj data { get; set; }
    }

    public class verifyPaymentObj : InitiateFinalResponse
    {
        public string chargecode { get; set; }
        public double chargedamount { get; set; }
        public string vbvmessage { get; set; }
        public CardObj card { get; set; }

        public List<MetaObj> meta { get; set; }
      
    }

    public class MetaObj
    {
        public string metaname { get; set; }
        public string metavalue { get; set; }
    }

    public class CardObj
    {
        public string expirymonth { get; set; }
        public string expiryyear { get; set; }
        public string cardBIN { get; set; }
        public string last4digits { get; set; }
        public string brand { get; set; }
        public string issuing_country { get; set; }
        public string card_hash { get; set; }
        public string type { get; set; }
    }

    public class validatepaymentObj
    {
        public finalResponse data { get; set; }
        public ValidateFinalResponse tx { get; set; }
    }

    public class ValidateFinalResponse : InitiateFinalResponse
    {
        public chargeTokenObj chargeToken { get; set; }
    }

    public class chargeTokenObj
    {
        public string user_token { get; set; }
        public string embed_token { get; set; }
    }

    public class finalResponse
    {
        public string responsecode { get; set; }
        public string responsemessage { get; set; }
    }

    public class InitiatePaymentFinalResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public InitiateFinalResponse data { get; set; }
    }

    public class InitiateFinalResponse
    {
        public int id { get; set; }
        public string txRef { get; set; }
        public string orderRef { get; set; }
        public string flwRef { get; set; }
        public string redirectUrl { get; set; }
        public string device_fingerprint { get; set; }
        public string settlement_token { get; set; }
        public string cycle { get; set; }
        public double amount { get; set; }
        public double charged_amount { get; set; }
        public double appfee { get; set; }
        public double merchantfee { get; set; }
        public double merchantbearsfee { get; set; }
        public string chargeResponseCode { get; set; }
        public string raveRef { get; set; }
        public string chargeResponseMessage { get; set; }
        public string authModelUsed { get; set; }
        public string currency { get; set; }
        public string IP { get; set; }
        public string narration { get; set; }
        public string status { get; set; }
        public string vbvrespmessage { get; set; }
        public string authurl { get; set; }
        public string vbvrespcode { get; set; }
        public string acctvalrespmsg { get; set; }
        public string acctvalrespcode { get; set; }
        public string paymentType { get; set; }
        public string paymentPlan { get; set; }
        public string paymentPage { get; set; }
        public string paymentId { get; set; }
        public string fraud_status { get; set; }
        public string charge_type { get; set; }
        public int is_live { get; set; }
        public dynamic retry_attempt { get; set; }
        public dynamic getpaidBatchId { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public string deletedAt { get; set; }
        public int customerId { get; set; }
        public int AccountId { get; set; }
        //public customer customer { get; set; }
        public bool customercandosubsequentnoauth { get; set; }
      

    }

    public class customer
    {
        public int id { get; set; }
        public string phone { get; set; }
        public string fullName { get; set; }
        public string customertoken { get; set; }
        public string email { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public dynamic deletedAt { get; set; }
        public int AccountId { get; set; }

    }

    public class suggestedAuth
    {
        public string suggested_auth { get; set; }
    }
}
