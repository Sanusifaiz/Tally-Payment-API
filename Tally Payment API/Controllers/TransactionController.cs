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

        public readonly ICardTokenRepository _cardTokenRepo;



        public TransactionController(ILogger<TransactionController> logger,
            ITransactionRepository transactionRepository, IPaymentDataEncryption paymentDataEncryption,
            IUserPaymentRepository userPaymentRepository, IConfiguration Configuration, ICardTokenRepository cardTokenRepository)
        {
            _logger = logger;
            _transactionRepo = transactionRepository;
            _userRepo = userPaymentRepository;
            _paymentEncryption = paymentDataEncryption;
            configuration = Configuration;
            _cardTokenRepo = cardTokenRepository;
            // _savecard = saveCard;
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

                    var response = await client.ExecutePostAsync(request);
                    if (response.StatusCode.ToString() == "OK")
                    {
                        //update paymentlink details
                        paymentLinkDetails.Status = (int)Status.Processing;
                        _userRepo.Save();

                        var responseobj = JsonConvert.DeserializeObject<InitiatePaymentResponseForNigerianMasterAndVerveCard>(response.Content);

                        if (responseobj.status == "success" && responseobj.message == "AUTH_SUGGESTION")
                        {
                            if (responseobj.data.suggested_auth == "PIN")
                            {
                                return new ResponseMessage { Status = "Ok", Message = "Please Enter your Card Pin", Data2 = responseobj.data.suggested_auth };
                            }
                            else if (responseobj.data.suggested_auth == "NOAUTH_INTERNATIONAL")
                            {
                                return new ResponseMessage { Status = "Ok", Message = "Please Enter your billing details", Data2 = responseobj.data.suggested_auth };
                            }
                            else if (responseobj.data.suggested_auth == "AVS_VBVSECURECODE")
                            {
                                return new ResponseMessage { Status = "Ok", Message = "3D Secured Payment", Data2 = responseobj.data.suggested_auth };

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
                                paymentLinkUniqueString = paymentLinkDetails.RandomString,
                                PaymentFrontEndUrl = card.FrontEndUrl,
                                SaveCard = card.SaveCard
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
                var response = await client.ExecutePostAsync(request);
                if (response.StatusCode.ToString() == "OK")
                {
                    var responseobj = JsonConvert.DeserializeObject<VerifyPaymentFinalResponse>(response.Content);



                    transaction.status = responseobj.data.status;
                    transaction.chargeResponseCode = responseobj.data.chargecode;
                    //transaction.chargeResponseMessage = responseobj.data.chargeResponseMessage;
                    transaction.ResponseCode = responseobj.data.chargecode;
                    transaction.ResponseMessage = responseobj.data.vbvmessage;
                    transaction.CardDetails = responseobj.data.card.brand + ":" + responseobj.data.card.cardBIN + ":"
                        + responseobj.data.card.expirymonth + ":" + responseobj.data.card.expiryyear + ":"
                        + responseobj.data.card.issuing_country + ":" + responseobj.data.card.last4digits;
                    if (responseobj.data.meta.Count != 0)
                    {
                        transaction.metaname = responseobj.data.meta[0].metaname;
                        transaction.metavalue = responseobj.data.meta[0].metavalue;
                    }

                    transaction.Amount = responseobj.data.amount;
                    transaction.charged_amount = responseobj.data.chargedamount;

                    _transactionRepo.Save();

                    //update userpayment table
                    var paymentLinkDetails = _userRepo.GetUserPaymentByUniqueString(transaction.paymentLinkUniqueString);

                    if (responseobj.data.status == "successful" && responseobj.data.amount >= paymentLinkDetails.Amount && responseobj.data.chargecode == "00")
                    {
                        paymentLinkDetails.Status = (int)Status.Paid;
                        paymentLinkDetails.outData = JsonConvert.SerializeObject(responseobj);
                        _userRepo.Save();
                    }
                    else
                    {
                        paymentLinkDetails.Status = (int)Status.Failed;
                        paymentLinkDetails.outData = JsonConvert.SerializeObject(responseobj);
                        _userRepo.Save();
                    }



                    string SaveCard = "Card Not Saved";
                    //save Card Details as token
                    if (transaction.SaveCard)
                    {
                        if (responseobj.data.card.card_tokens.Count != 0) ;
                        {
                            // saveCard = saveCardDetails.SaveCard(responseobj.data.custemail, responseobj.data.custphone, responseobj.data.card.card_tokens[0].embedtoken);
                            var cardTokenObj = new CardTokenTable
                            {
                                email = responseobj.data.custemail,
                                PhoneNumber = responseobj.data.custphone,
                                embedtoken = responseobj.data.card.card_tokens[0].embedtoken,
                                expirymonth = responseobj.data.card.expirymonth,
                                expiryyear = responseobj.data.card.expiryyear,
                                cardBIN = responseobj.data.card.cardBIN,
                                last4digits = responseobj.data.card.last4digits,
                                type = responseobj.data.card.type

                            };

                            if (_cardTokenRepo.AddCardToken(cardTokenObj))
                            {
                                SaveCard = "Card Saved";
                            };
                        }
                    }

                    var respObj = new VerifyPaymentResponseMessage
                    {
                        StatusCode = responseobj.data.chargecode,
                        ResponseMessage = responseobj.data.vbvmessage,
                        TransactionRef = responseobj.data.txRef,
                        FlutterwaveRef = responseobj.data.flwRef,
                        amount = responseobj.data.amount,
                        charged_amount = responseobj.data.chargedamount,
                        status = responseobj.data.status,
                        authModelUsed = responseobj.data.authModelUsed,
                        CardSaveStatus = SaveCard
                    };

                    return new ResponseMessage { Status = "OK", Message = responseobj.message, Data2 = respObj };
                }
                return new ResponseMessage { Status = "error", Message = "Error Verifying Payment" };
            }
            catch (Exception ex)
            {
                return new ResponseMessage { Status = "error", Message = "An Error Occurred", Data2 = ex };

            }
        }


        [HttpPost, Route("VerifyVBVSecuredayment")]
        [ProducesResponseType(201, Type = typeof(ResponseMessage))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseMessage> VerifyVBVSecuredayment([FromBody] VbvResponseObj Response)
        {
            try
            {
                if (Response.txRef != null)
                {
                    var response = await VerifyCardPayment(Response.txRef);
                    if (response.Status == "OK")
                    {
                        //var responseobj = JsonConvert.DeserializeObject<VerifyPaymentResponseMessage>(response.D);

                        return new ResponseMessage { Status = "OK", Message = response.Message, Data2 = response.Data2 };
                    }
                    return new ResponseMessage { Status = "error", Message = "Error Verifying Payment" };

                }
                return new ResponseMessage { Status = "error", Message = "Error Verifying Payment" };

            }
            catch (Exception ex)
            {

                return new ResponseMessage { Status = "error", Message = "An Error Occurred", Data2 = ex };

            }

        }

        [HttpPost, Route("ChargeWithToken")]
        [ProducesResponseType(201, Type = typeof(ResponseMessage))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseMessage> ChargeWithToken([FromBody] TokenChargeObj Request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //get paymentLink details
                    var paymentLinkDetails = _userRepo.GetUserPaymentByUniqueString(Request.UniqueString);

                    if (paymentLinkDetails == null)
                    {
                        return new ResponseMessage { Status = "Error", Message = "Use a Valid PaymentLink" };

                    }
                    //build transaction reference
                    var tranxRef = "TALDPFK-" + Guid.NewGuid().ToString().Substring(0, 10) + DateTime.Now.Ticks;
                    Request.txRef = tranxRef;
                    Request.SECKEY = configuration["Keys:FltwvSecretKey"];

                    //getCard Token with email
                    var CardToken = _cardTokenRepo.GetCardTokenById(Request.CardId).embedtoken;

                    if (CardToken == null)
                    {
                        return new ResponseMessage { Status = "Error", Message = "Card does not exist" };
                    }
                    Request.token = CardToken;


                    // add transaction to db
                    _transactionRepo.AddTransaction(new Transactions
                    {
                        transactionRef = tranxRef,
                        Amount = Request.amount,
                        PayerEmail = Request.email,
                        Currency = Request.currency,
                        PayerName = paymentLinkDetails.Payer,
                        status = "Pending"
                    });

                    var FltwvUrl = string.Format(configuration["Url:FlutterwaveBaseUrlLive"] + "/flwv3-pug/getpaidx/api/tokenized/charge");
                    var client = new RestClient(FltwvUrl);
                    var request = new RestRequest();
                    var body = Request;

                    request.AddJsonBody(body);

                    var response = client.Post(request);
                    if (response.StatusCode.ToString() == "OK")
                    {
                        var responseobj = JsonConvert.DeserializeObject<TokenChargeResponse>(response.Content);
                        if (responseobj.status == "success")
                        {
                            var Transaction = _transactionRepo.GetTransaction(tranxRef);


                            Transaction.orderRef = responseobj.data.orderRef;
                            Transaction.flwRef = responseobj.data.flwRef;
                            Transaction.ResponseCode = responseobj.data.vbvrespcode;
                            Transaction.ResponseMessage = responseobj.data.vbvrespmessage;
                            Transaction.authModelUsed = responseobj.data.authModelUsed;
                            Transaction.PaymentType = responseobj.data.paymentType;
                            Transaction.Amount = responseobj.data.amount;
                            Transaction.charged_amount = responseobj.data.charged_amount;
                            Transaction.appfee = responseobj.data.appfee;
                            Transaction.merchantfee = responseobj.data.merchantfee;
                            Transaction.chargeResponseCode = responseobj.data.chargeResponseCode;
                            Transaction.chargeResponseMessage = responseobj.data.chargeResponseMessage;
                            Transaction.narration = responseobj.data.narration;
                            Transaction.authurl = responseobj.data.authurl;
                            Transaction.status = responseobj.data.status;
                            Transaction.Currency = responseobj.data.currency;
                            Transaction.Created = responseobj.data.createdAt;
                            Transaction.Updated = responseobj.data.updatedAt;
                            Transaction.PayerEmail = paymentLinkDetails.Payer;
                            Transaction.PayerName = paymentLinkDetails.FeesBearer;
                            Transaction.PayerPhone = null;
                            Transaction.UserId = paymentLinkDetails.UserId;
                            Transaction.paymentLinkUniqueString = paymentLinkDetails.RandomString;
                            _transactionRepo.Save();

                            //update user payment table
                            paymentLinkDetails.Status = (int)Status.Paid;
                            paymentLinkDetails.outData = JsonConvert.SerializeObject(responseobj);
                            _userRepo.Save();

                            //build response object
                            var respObj = new VerifyPaymentResponseMessage
                            {
                                StatusCode = responseobj.data.chargeResponseCode,
                                ResponseMessage = responseobj.data.vbvrespmessage,
                                TransactionRef = responseobj.data.txRef,
                                FlutterwaveRef = responseobj.data.flwRef,
                                amount = responseobj.data.amount,
                                charged_amount = responseobj.data.charged_amount,
                                status = responseobj.data.status,
                                CardSaveStatus = ""
                            };

                            return new ResponseMessage { Status = "OK", Message = responseobj.message, Data2 = respObj };


                        }

                        paymentLinkDetails.Status = (int)Status.Failed;
                        paymentLinkDetails.outData = JsonConvert.SerializeObject(responseobj);
                        _userRepo.Save();


                        return new ResponseMessage { Status = "OK", Message = responseobj.message, Data2 = responseobj.data };



                    }
                    return new ResponseMessage { Status = "Error", Message = "Error Occured while charging with token" };
                }

                return new ResponseMessage { Status = "Error", Data2 = ModelState };
            }
            catch (Exception ex)
            {

                return new ResponseMessage { Status = "error", Message = "An Error Occurred", Data2 = ex };

            }
        }
    }
}
