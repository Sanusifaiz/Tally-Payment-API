using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tally_Payment_API.DataModel;
using Tally_Payment_API.Models;
using Tally_Payment_API.Repository.IRepository;
using Tally_Payment_API.Services;

namespace Tally_Payment_API.Controllers
{
    [ApiController]
    [Route("api/v1/Transaction")]
    public class TransactionController : ControllerBase
    {


        private readonly ILogger<TransactionController> _logger;
        private readonly ITransactionRepository _transactionRepo;
        private readonly IUserPaymentRepository _userRepo;
        public readonly IPaymentDataEncryption _paymentEncryption;
        private readonly IConfiguration configuration;

        public TransactionController(ILogger<TransactionController> logger,
            ITransactionRepository transactionRepository, IPaymentDataEncryption paymentDataEncryption,
            IUserPaymentRepository userPaymentRepository, IConfiguration Configuration)
        {
            _logger = logger;
            _transactionRepo = transactionRepository;
            _userRepo = userPaymentRepository;
            _paymentEncryption = paymentDataEncryption;
            configuration = Configuration;
        }

        public enum Status
        {
            Pending = 0,
            Processing,
            Paid,
            Canceled,
            Refunded,
            Failed
        }

        public enum Means
        {
            Bank = 1,
            Card,
            Paypal
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("InitiateCardPayment/{uniqueString}")]
        [ProducesResponseType(201, Type = typeof(ResponseMessage))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseMessage> InitiateCardPayment([FromBody] CardDetails card, [FromRoute] string uniqueString)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //build transaction reference
                    var tranxRef = "TALDPFK-" + Guid.NewGuid().ToString().Substring(0, 10) + DateTime.Now.Ticks;
                    card.txRef = tranxRef;
                    //get PyamentLInk details by userId

                    var paymentLinkDetails = _userRepo.GetUserPaymentByUniqueString(uniqueString);

                    if (paymentLinkDetails == null)
                    {
                        return new ResponseMessage { Status = "error", Message = "Use a valid Payment Link" };
                    }
                    //get encryption key
                    var encryptionKey = _paymentEncryption.GetEncryptionKey(configuration["Keys:FltwvSecretKey"]);

                    //stringify card data
                    var CardData = JsonConvert.SerializeObject(card);
                    // encrypt Card Details
                    var encryptedCardDetails = _paymentEncryption.EncryptData(configuration["Keys:FltwvEncyptionKey"], CardData);

                    var FltwvUrl = string.Format(configuration["Url:FlutterwaveBaseUrlLive"] + "/flwv3-pug/getpaidx/api/charge");
                    var client = new RestClient(FltwvUrl);
                    var request = new RestRequest();
                    var body = new InitiatePaymentObj()
                    {
                        PBFPubKey = configuration["Keys:FltwvPublicKey"],
                        client = encryptedCardDetails,
                        alg = "3DES-24"
                    };

                    request.AddJsonBody(body);

                    var response = client.Post(request);
                    if (response.StatusCode.ToString() == "OK")
                    {
                        var responseobj = JsonConvert.DeserializeObject<InitiatePaymentResponseForNigerianMasterAndVerveCard>(response.Content);

                        if (responseobj.status == "success" && responseobj.message == "AUTH_SUGGESTION")
                        {
                            if (responseobj.data.suggested_auth == "PIN")
                            {
                                return new ResponseMessage { Status = "Ok", Message = "Please Enter your Card Pin", Data2 = responseobj.data.suggested_auth };
                            }
                            else if (responseobj.data.suggested_auth == "NOAUTH_INTERNATIONAL" || responseobj.data.suggested_auth == "AVS_VBVSECURECODE")
                            {
                                return new ResponseMessage { Status = "Ok", Message = "Please Enter your billing details", Data2 = responseobj.data.suggested_auth };
                            }
                        }
                        else if (responseobj.status == "success" && responseobj.message == "V-COMP")
                        {
                            var finalresponseobj = JsonConvert.DeserializeObject<InitiatePaymentFinalResponse>(response.Content);
                            // add transaction response to table


                            var transactionObj = new Transactions
                            {
                                transactionRef = finalresponseobj.data.txRef,
                                orderRef = finalresponseobj.data.orderRef,
                                flwRef = finalresponseobj.data.flwRef,
                                ResponseCode = finalresponseobj.data.vbvrespcode,
                                ResponseMessage = finalresponseobj.data.vbvrespmessage,
                                authModelUsed = finalresponseobj.data.authModelUsed,
                                PaymentType = finalresponseobj.data.paymentType,
                                Amount = finalresponseobj.data.amount,
                                charged_amount = finalresponseobj.data.charged_amount,
                                appfee = finalresponseobj.data.appfee,
                                merchantfee = finalresponseobj.data.merchantfee,
                                chargeResponseCode = finalresponseobj.data.chargeResponseCode,
                                chargeResponseMessage = finalresponseobj.data.chargeResponseMessage,
                                narration = finalresponseobj.data.narration,
                                authurl = finalresponseobj.data.authurl,
                                status = finalresponseobj.data.status,
                                Currency = finalresponseobj.data.currency,
                                Created = finalresponseobj.data.createdAt,
                                Updated = finalresponseobj.data.updatedAt,
                                PayerEmail = paymentLinkDetails.Payer,
                                PayerName = paymentLinkDetails.FeesBearer,
                                PayerPhone = null,
                                UserId = paymentLinkDetails.UserId,
                                paymentLinkUniqueString = paymentLinkDetails.RandomString
                            };


                            if (_transactionRepo.AddTransaction(transactionObj))
                            {
                                var respObj = new InitiatePaymentResponseMessage
                                {
                                    StatusCode = finalresponseobj.data.chargeResponseCode,
                                    ResponseMessage = finalresponseobj.data.chargeResponseMessage,
                                    authModelUsed = finalresponseobj.data.authModelUsed,
                                    authUrl = finalresponseobj.data.authurl,
                                    TransactionRef = finalresponseobj.data.txRef
                                };
                                return new ResponseMessage { Status = "Ok", Message = finalresponseobj.data.chargeResponseMessage, Data2 = respObj };
                            };
                        }


                        return new ResponseMessage { Status = "error", Message = "Error Initiating payment" };
                    }

                    return new ResponseMessage { Status = "error", Message = "Error Initiating payment" };

                }

                return new ResponseMessage { Status = "error", Data2 = ModelState };
            }
            catch (Exception ex)
            {
                return new ResponseMessage { Status = "error", Message = "An Error Occurred", Data2 = ex };
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="trxnRef"></param>
        /// <returns></returns>
        [HttpPost, Route("ValidateCardPayment/{trxnRef}")]
        [ProducesResponseType(201, Type = typeof(ResponseMessage))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseMessage> ValidateCardPayment(string trxnRef, string otp)
        {
            try
            {
                // get transaction details
                var transaction = _transactionRepo.GetTransaction(trxnRef);

                if (transaction == null)
                {
                    return new ResponseMessage { Status = "error", Message = "Transaction does not exist, Enter a valid Transaction refrence" };
                }

                var FltwvUrl = string.Format(configuration["Url:FlutterwaveBaseUrlLive"] + "/flwv3-pug/getpaidx/api/validatecharge");
                var client = new RestClient(FltwvUrl);
                var request = new RestRequest();
                var body = new ValidatePyamentObject
                {
                    PBFPubKey = configuration["Keys:FltwvPublicKey"],
                    transaction_reference = transaction.flwRef,
                    otp = otp
                };

                request.AddJsonBody(body);
                var response = client.Post(request);
                if (response.StatusCode.ToString() == "OK")
                {
                    var responseobj = JsonConvert.DeserializeObject<ValidatePaymentFinalResponse>(response.Content);

                    if (responseobj.status == "success")
                    {
                        // update transaction table 

                        transaction.status = responseobj.data.tx.status;
                        transaction.chargeResponseCode = responseobj.data.tx.chargeResponseCode;
                        transaction.chargeResponseMessage = responseobj.data.tx.chargeResponseMessage;
                        transaction.ResponseCode = responseobj.data.data.responsecode;
                        transaction.ResponseMessage = responseobj.data.data.responsemessage;

                        _userRepo.Save();

                        var respObj = new ValidatePaymentREsponseMessage
                        {
                            StatusCode = responseobj.data.tx.chargeResponseCode,
                            ResponseMessage = responseobj.data.data.responsemessage,
                            TransactionRef = responseobj.data.tx.txRef,
                            FlutterwaveRef = responseobj.data.tx.flwRef
                        };

                        return new ResponseMessage { Status = "OK", Message = responseobj.message, Data2 = respObj };

                    }

                    return new ResponseMessage { Status = "Error", Message = responseobj.message, Data2 = responseobj.data };
                }
                return new ResponseMessage { Status = "Error", Message = "Error Validating Payment" };
            }

            catch (Exception ex)
            {

                return new ResponseMessage { Status = "error", Message = "An Error Occurred", Data2 = ex };
            }

        }

        [HttpPost, Route("VerifyCardPayment/{trxnRef}")]
        [ProducesResponseType(201, Type = typeof(ResponseMessage))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseMessage> VerifyCardPayment(string trxnRef)
        {
            try
            {
                // get transaction details
                var transaction = _transactionRepo.GetTransaction(trxnRef);

                if (transaction == null)
                {
                    return new ResponseMessage { Status = "error", Message = "Transaction does not exist, Enter a valid Transaction refrence" };
                }

                var FltwvUrl = string.Format(configuration["Url:FlutterwaveBaseUrlLive"] + "/flwv3-pug/getpaidx/api/v2/verify");
                var client = new RestClient(FltwvUrl);
                var request = new RestRequest();
                var body = new VerifyPaymentObject
                {
                    txref = trxnRef,
                    SECKEY = configuration["Keys:FltwvSecretKey"]
                };

                request.AddJsonBody(body);
                var response = client.Post(request);
                if (response.StatusCode.ToString() == "OK")
                {
                    var responseobj = JsonConvert.DeserializeObject<VerifyPaymentFinalResponse>(response.Content);
                    // update transaction table 

                    transaction.status = responseobj.data.status;
                    transaction.chargeResponseCode = responseobj.data.chargecode;
                    //transaction.chargeResponseMessage = responseobj.data.chargeResponseMessage;
                    transaction.ResponseCode = responseobj.data.chargecode;
                    transaction.ResponseMessage = responseobj.data.vbvmessage;
                    transaction.CardDetails = responseobj.data.card.brand + ":" + responseobj.data.card.cardBIN + ":"
                        + responseobj.data.card.expirymonth + ":" + responseobj.data.card.expiryyear + ":"
                        + responseobj.data.card.issuing_country + ":" + responseobj.data.card.last4digits;
                    transaction.metaname = responseobj.data.meta[0].metaname;
                    transaction.metavalue = responseobj.data.meta[0].metavalue;
                    transaction.Amount = responseobj.data.amount;
                    transaction.charged_amount = responseobj.data.chargedamount;

                    _userRepo.Save();

                    var respObj = new VerifyPaymentResponseMessage
                    {
                        StatusCode = responseobj.data.chargecode,
                        ResponseMessage = responseobj.data.vbvmessage,
                        TransactionRef = responseobj.data.txRef,
                        FlutterwaveRef = responseobj.data.flwRef,
                        amount = responseobj.data.amount,
                        charged_amount = responseobj.data.charged_amount,
                        status = responseobj.data.status
                    };

                    return new ResponseMessage { Status = "OK", Message = responseobj.message, Data2 = respObj };
                }
                return new ResponseMessage { };
            }
            catch (Exception ex)
            {

                return new ResponseMessage { Status = "error", Message = "An Error Occurred", Data2 = ex };

            }
        }
    }
}
