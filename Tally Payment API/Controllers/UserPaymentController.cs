using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tally_Payment_API.DataModel;
using Tally_Payment_API.Models;
using Tally_Payment_API.Repository.IRepository;


namespace Tally_Payment_API.Controllers
{
    [ApiController]
    [Route("api/v1/UserPayments")]
    public class UserPaymentController : ControllerBase
    {


        private readonly ILogger<UserPaymentController> _logger;
        private readonly IUserPaymentRepository _userRepo;
        public UserPaymentController(ILogger<UserPaymentController> logger, IUserPaymentRepository userRepo)
        {
            _logger = logger;
            _userRepo = userRepo;
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
        /// Generate Random link for User payment
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPaymentLink")]
        [ProducesResponseType(201, Type = typeof(ResponseMessage))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ResponseMessage GetPaymentLink([FromBody] UserRequestModel user)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    // generate random string for payment url

                    var randomString = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(
                        user.UserId + user.Payer + DateTime.Now + user.Amount + Guid.NewGuid().ToString().Substring(10))); ;


                    var inputObject = new UserPaymentModel
                    {
                        UserId = user.UserId,
                        Amount = user.Amount,
                        Means = user.Means,
                        From = user.From,
                        FeesBearer = user.FeesBearer,
                        Payer = user.Payer,
                        Currency = user.Currency,
                        Expiry = user.Expiry,
                        RandomString = randomString,
                    };

                    // create new user field in database
                    var AddUserPaymentToDb = _userRepo.CreateUserPayment(inputObject);

                    if (AddUserPaymentToDb)
                    {
                        //generate payment url
                        var PaymentLink = $"https://tally.dipfek?payL=" + randomString;

                        //update db with paymenturl 
                        var getUserpaymentDetails = _userRepo.GetUserPaymentByUniqueString(randomString);

                        getUserpaymentDetails.PaymentLink = PaymentLink;
                        getUserpaymentDetails.Created = DateTime.UtcNow;

                        var update = _userRepo.Save();

                        //response object
                        var response = new Data
                        {
                            userId = user.UserId,
                            amount = user.Amount,
                            from = user.From,
                            created = getUserpaymentDetails.Created,
                            paymentLink = getUserpaymentDetails.PaymentLink
                        };

                        if (update)
                        {
                            return new ResponseMessage { Status = "Ok", Data = response };
                        }

                        return new ResponseMessage { Status = "Error", Message = "An Error Occured while generating payment link" };
                    }

                    return new ResponseMessage { Status = "Error", Message = "An Error Occured while generating payment link" };


                }
                catch (Exception e)
                {
                    _logger.LogError($"{e}");
                    return new ResponseMessage { Status = "Error", Message = "An Error Occured", Data2 = e };
                }
            }
            return new ResponseMessage { Status = "Error", Message = "Enter all Required feilds correctly" };


        }

        /// <summary>
        /// GetPaymentDetails
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPaymentDetails")]
        [ProducesResponseType(200, Type = typeof(ResponseMessage))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ResponseMessage GetPaymentDetails(string UniqueString)
        {
            try
            {
                var output = _userRepo.GetUserPaymentByUniqueString(UniqueString);
                if(output != null)
                {
                    return new ResponseMessage { Status = "Ok", Data2 = output };
                }

                return new ResponseMessage { Status = "Error", Message = "Not Found" };
            }
            catch (Exception e)
            {
                _logger.LogError($"{e}");
                return new ResponseMessage { Status = "Error", Message = "Enter all Required feilds correctly", Data2 = e };
            }
        }
    }
}
