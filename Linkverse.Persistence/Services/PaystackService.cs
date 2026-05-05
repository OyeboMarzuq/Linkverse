using Linkverse.Application.Interfaces.IServices;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Persistence.Services
{
    public class PaystackService : IPaystackService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public PaystackService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<string?> InitializePaymentAsync(string email, decimal amount, string reference, string callbackUrl)
        {
            var requestBody = new
            {
                email = email,
                amount = (int)(amount * 100), // Kobo
                reference,
                callback_url = callbackUrl
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.paystack.co/transaction/initialize");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _config["Paystack:SecretKey"]);
            request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var result = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

            return response.IsSuccessStatusCode ? (string?)result.data.authorization_url : null;
        }

        public async Task<bool> VerifyPaymentAsync(string reference)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.paystack.co/transaction/verify/{reference}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _config["Paystack:SecretKey"]);

            var response = await _httpClient.SendAsync(request);
            var result = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

            return result.data.status == "success";
        }
    }
}