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
    public class WSEPSupplyAdapter : ISupplyAdapter
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;
        private readonly Logger _logger;
        
        public WSEPSupplyAdapter()
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
        public async Task<Result<int>> SupplyProducts(string storeName, string[] itemsNames, string userAddress)
        {
            int transactionId = -1;
            Dictionary<string, string> dictionary = new Dictionary<string, string>()
            {
                {"action_type", "supply"},
                {"store", storeName},
                {"address", userAddress},
            };

            HttpContent content = new FormUrlEncodedContent(dictionary);
            HttpResponseMessage responseMessage = await _httpClient.PostAsync(_url, content);

            if (!responseMessage.IsSuccessStatusCode)
            {
                string message = $"Connection error with the supply system {responseMessage.StatusCode}";
                _logger.Error(message);
                return Result.Fail<int>($"Connection error with the supply system {responseMessage.StatusCode}");
            }

            string responseContent = await responseMessage.Content.ReadAsStringAsync();
            if (!int.TryParse(responseContent, out transactionId))
            {
                _logger.Error($"Invalid transaction id from supply system {responseContent}");
                return Result.Fail<int>($"Supply error");
            }
            
            return Result.Ok(transactionId);
        }

        public async Task<Result> CheckSupplyInfo(int transactionId)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>()
            {
                {"action_type", "cancel_supply"},
                {"transaction_id", $"{transactionId}"},
            };

            HttpContent content = new FormUrlEncodedContent(dictionary);
            HttpResponseMessage responseMessage = await _httpClient.PostAsync(_url, content);
            
            if (!responseMessage.IsSuccessStatusCode)
            {
                string message = $"Connection error with the supply system {responseMessage.StatusCode}";
                _logger.Error(message);
                return Result.Fail($"Connection error with the supply system {responseMessage.StatusCode}");
            }

            string responseContent = await responseMessage.Content.ReadAsStringAsync();
            if (!int.TryParse(responseContent, out transactionId))
            {
                _logger.Error($"Invalid transaction id from supply system {responseContent}");
                return Result.Fail<int>($"Supply error");
            }

            Result refundRes;
            if (transactionId == -1)
            {
                refundRes = Result.Fail<Result>("Order wasn't canceled");
            } else if (transactionId == 1)
            {
                refundRes = Result.Ok(transactionId);
            }
            else
            {
                _logger.Error($"Invalid value return from supply system for refund {transactionId}");
                refundRes = Result.Fail("Cancel order error");
            }

            return refundRes;
        }
    }
}