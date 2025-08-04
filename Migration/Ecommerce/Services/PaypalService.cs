using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Ecommerce.Interfaces;
using Ecommerce.Models;
using Ecommerce.Models.DTOs;
using Microsoft.Extensions.Options;

namespace Ecommerce.Services
{
    public class PaypalService
    {
        private readonly HttpClient _httpClient;
        private readonly PaypalSettings _paypalSettings;
        private readonly IRepository<int, Product> _productRepository;

        public PaypalService(HttpClient httpClient, IOptions<PaypalSettings> paypalSettings, IRepository<int, Product> productRepository)
        {
            _httpClient = httpClient;
            _paypalSettings = paypalSettings.Value;
            _productRepository = productRepository;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var byteArray = Encoding.UTF8.GetBytes($"{_paypalSettings.ClientId}:{_paypalSettings.ClientSecret}");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var requestBody = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await _httpClient.PostAsync($"{_paypalSettings.BaseUrl}/v1/oauth2/token", requestBody);
            var json = await response.Content.ReadAsStringAsync();

            return JsonDocument.Parse(json).RootElement.GetProperty("access_token").GetString()!;
        }

        public async Task<string> CreateOrder(List<CartDto> cartItems, string currency = "USD")
        {
            var accessToken = await GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var productIds = cartItems.Select(c => c.ProductId).ToList();
            var allProducts = await _productRepository.GetAllAsync();
            var products = allProducts
                .Where(p => productIds.Contains(p.ProductId))
                .ToList();

            var items = new List<object>();
            double? total = 0;

            foreach (var cart in cartItems)
            {
                var product = products.FirstOrDefault(p => p.ProductId == cart.ProductId);
                if (product == null) continue;

                var lineTotal = product.Price * cart.Quantity;
                total += lineTotal;

                items.Add(new
                {
                    name = product.ProductName,
                    quantity = cart.Quantity.ToString(),
                    unit_amount = new
                    {
                        currency_code = currency,
                        value = product.Price.ToString()
                    }
                });
            }

            var order = new
            {
                intent = "CAPTURE",
                application_content = new
                {
                    user_action = "PAY_NOW",
                    return_url = "http://localhost:4200/payment-success",
                    cancel_url = "http://localhost:4200/payment-cancel"
                },

                purchase_units = new[]
                {
                    new
                    {
                        amount = new
                        {
                            currency_code = currency,
                            value = total.ToString(),
                            breakdown = new
                            {
                                item_total = new
                                {
                                    currency_code = currency,
                                    value = total.ToString()
                                }
                            }
                        },
                        items
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_paypalSettings.BaseUrl}/v2/checkout/orders", content);
            return await response.Content.ReadAsStringAsync();


        }

        public async Task<string> CaptureOrder(string orderId)
        {
            var accessToken = await GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var content = new StringContent("{}", Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                $"{_paypalSettings.BaseUrl}/v2/checkout/orders/{orderId}/capture",
                content
            );

            return await response.Content.ReadAsStringAsync();
        }

    }
}