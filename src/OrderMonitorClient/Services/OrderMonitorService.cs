using OrderMonitorClient.Models;
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
                    options =>
                    {
                        options.AccessTokenProvider = async () =>
                        {
                            var result = await _tokenProvider.RequestAccessToken();
                            if (result.TryGetToken(out var token))
                            {
                                return token.Value;
                            }
                            else
                            {
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
            //todo: retry with exponential backoff
            try
            {
                await _hubConnection.StartAsync();
            }
            catch (Exception)
            {
                Console.WriteLine("Could not connect to SignalR Hub");
            }
        }

        public async Task<List<OrderMonitorListModel>> ListOrdersAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<OrderMonitorListModel>>($"{_apiRoot}/orders/monitor");

            return result;
        }

        public async Task PlaceOrderAsync(OrderCreateModel model)
        {
            var result = await _httpClient.PostAsJsonAsync($"{_apiRoot}/orders", model);
        }


    }
}
