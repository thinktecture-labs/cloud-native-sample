using Gateway.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;

namespace OrderMonitorClient.Services
{
    public class OrderEventArgs : EventArgs
    {
        public string Id { get; set; }
    }

    public class OrderMonitorService
    {
        private readonly NavigationManager _navigationManager;
        private readonly IAccessTokenProvider _tokenProvider;
        private readonly HttpClient _httpClient;
        private readonly string _apiRoot;

        private HubConnection _hubConnection;

        public event EventHandler<OrderEventArgs> OrderListChanged;


        public OrderMonitorService(NavigationManager navigationManager, IAccessTokenProvider tokenProvider, IConfiguration config, HttpClient httpClient)
        {
            _navigationManager = navigationManager;
            _tokenProvider = tokenProvider;
            _apiRoot = config["ApiRoot"];
            _httpClient = httpClient;
        }

        public async Task InitAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(
                    _navigationManager.ToAbsoluteUri($"{_apiRoot}/notifications/notificationHub"),
                    options => {
                        options.AccessTokenProvider = async () => {
                            var result = await _tokenProvider.RequestAccessToken();
                            if (result.TryGetToken(out var token)) {
                                return token.Value;
                            }
                            else {
                                return string.Empty;
                            }
                        };
                    })
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<string>("onOrderProcessed", (orderId) =>
            {
                OrderListChanged?.Invoke(this, new OrderEventArgs { Id = orderId });
            });

            await _hubConnection.StartAsync();
        }

        public async Task<List<OrderMonitorListModel>> ListOrdersAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<OrderMonitorListModel>>($"{_apiRoot}/orders/monitor");

            return result;
        }

        public async Task AddOrderAsync()
        {
            var rnd = new Random();
            const int min = 1;
            const int max = 20;

            var model = new OrderCreateModel
            {
                Positions = new List<OrderPositionCreateModel>{
                    new OrderPositionCreateModel
                    {
                        ProductId = "b3b749d1-fd02-4b47-8e3c-540555439db6",
                        Quantity = rnd.Next(min, max)
                    },
                    new OrderPositionCreateModel
                    {
                        ProductId = "aaaaaaaa-fd02-4b47-8e3c-540555439db6",
                        Quantity = rnd.Next(min, max)
                    },
                }
            };
            var result = await _httpClient.PostAsJsonAsync($"{_apiRoot}/orders", model);
            
        }

        private class OrderCreateModel
        {
            public List<OrderPositionCreateModel> Positions { get; set; }
        }

        private class OrderPositionCreateModel
        {
            public string ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
