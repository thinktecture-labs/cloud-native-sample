using Gateway.Models;
using Microsoft.AspNetCore.Components;
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
        private readonly HttpClient _httpClient;
        private HubConnection _hubConnection;

        public event EventHandler<OrderEventArgs> OrderListChanged;

        public OrderMonitorService(NavigationManager navigationManager, HttpClient httpClient)
        {
            _navigationManager = navigationManager;
            _httpClient = httpClient;
        }

        public async Task InitAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("/notifications/notificationHub"))
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
            var result = await _httpClient.GetFromJsonAsync<List<OrderMonitorListModel>>("/orders/monitor");

            return result;
        }
    }
}
