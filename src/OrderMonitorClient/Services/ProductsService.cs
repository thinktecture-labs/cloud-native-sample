using OrderMonitorClient.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace OrderMonitorClient.Services
{
    public class ProductsService
    {
        private readonly IAccessTokenProvider _tokenProvider;
        private readonly HttpClient _httpClient;
        private readonly string _apiRoot;

        public ProductsService(IAccessTokenProvider tokenProvider, IConfiguration config, HttpClient httpClient)
        {
            _tokenProvider = tokenProvider;
            _apiRoot = config["ApiRoot"];
            _httpClient = httpClient;
        }

        public async Task<List<ProductListModel>> GetProductsAsync()
        {
            var products =  await _httpClient.GetFromJsonAsync<List<ProductListModel>>($"{_apiRoot}/products");
            return products;
        }
    }
}
