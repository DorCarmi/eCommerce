using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using eCommerce.Business;
using eCommerce.Common;
using NLog;

namespace eCommerce.Adapters
{
    public class WSEPPaymentAdapter : IPaymentAdapter
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;
        private readonly Logger _logger;
        
        public WSEPPaymentAdapter()
        {
            _httpClient = new HttpClient();
            _url = "https://cs-bgu-wsep.herokuapap.com/";
            LogManager.GetCurrentClassLogger();
        }
        
        public async Task<bool> VerifyConnection()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>()
            {
                {"action_type", "handshake"},
            };

            HttpContent content = new FormUrlEncodedContent(dictionary);
            HttpResponseMessage responseMessage = await _httpClient.PostAsync(_url, content);

            return responseMessage.IsSuccessStatusCode;
        }
        public async Task<Result<int>> Charge(double price, string paymentInfoUserName, string paymentInfoIdNumber, string paymentInfoCreditCardNumber,
            string paymentInfoCreditCardExpirationDate, string paymentInfoThreeDigitsOnBackOfCard)
        {
            int transactionId = -1;
            DateTime creditCardExpirationDate = DateTime.Parse(paymentInfoCreditCardExpirationDate);
            Dictionary<string, string> dictionary = new Dictionary<string, string>()
            {
                {"action_type", "pay"},
                {"card_number", paymentInfoCreditCardNumber},
                {"month", creditCardExpirationDate.Month.ToString()},
                {"year", creditCardExpirationDate.Year.ToString()},
                {"holder", paymentInfoUserName},
                {"ccv", paymentInfoThreeDigitsOnBackOfCard},
                {"id", paymentInfoIdNumber},
                {"price", $"{price}"}
            };

            HttpContent content = new FormUrlEncodedContent(dictionary);
            HttpResponseMessage responseMessage = await _httpClient.PostAsync(_url, content);

            if (!responseMessage.IsSuccessStatusCode)
            {
                string message = $"Connection error with the payment system {responseMessage.StatusCode}";
                _logger.Error(message);
                return Result.Fail<int>($"Connection error with the payment system {responseMessage.StatusCode}");
            }

            string responseContent = await responseMessage.Content.ReadAsStringAsync();
            if (!int.TryParse(responseContent, out transactionId))
            {
                _logger.Error($"Invalid transaction id from payment system {responseContent}");
                return Result.Fail<int>($"Payment error");
            }
            
            return Result.Ok(transactionId);
        }

        public Task<bool> CheckPaymentInfo(string paymentInfoUserName, string paymentInfoIdNumber, string paymentInfoCreditCardNumber,
            string paymentInfoCreditCardExpirationDate, string paymentInfoThreeDigitsOnBackOfCard)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Result> Refund(int transactionId)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>()
            {
                {"action_type", "cancel_pay"},
                {"transaction_id", $"{transactionId}"},
            };

            HttpContent content = new FormUrlEncodedContent(dictionary);
            HttpResponseMessage responseMessage = await _httpClient.PostAsync(_url, content);
            
            if (!responseMessage.IsSuccessStatusCode)
            {
                string message = $"Connection error with the payment system {responseMessage.StatusCode}";
                _logger.Error(message);
                return Result.Fail($"Connection error with the payment system {responseMessage.StatusCode}");
            }

            string responseContent = await responseMessage.Content.ReadAsStringAsync();
            if (!int.TryParse(responseContent, out transactionId))
            {
                _logger.Error($"Invalid transaction id from payment system {responseContent}");
                return Result.Fail<int>($"Payment error");
            }

            Result refundRes;
            if (transactionId == -1)
            {
                refundRes = Result.Fail<Result>("Not refund");
            } else if (transactionId == 1)
            {
                refundRes = Result.Ok(transactionId);
            }
            else
            {
                _logger.Error($"Invalid value return from payment system for refund {transactionId}");
                refundRes = Result.Fail("Refund error");
            }

            return refundRes;
        }
    }
}