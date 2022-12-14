using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;

namespace BlazorApp1.Data
{
    public class DosService
    {
        private List<string> addrs = new List<string>();

        public string dos(string addr)
        {
            if (!addrs.Contains(addr))
            {
                addrs.Add(addr);
                return "Starting Dos : " + addr;
            }
            else
            {
                addrs.Remove(addr);
                return "Stopping Dos : " + addr;
            }
        }

        private NavigationManager Navigation;
        private HubConnection? hubConnection;
        private List<string> messages = new List<string>();
        private string? userInput;
        private string? messageInput;

/*        protected override async Task OnInitializedAsync()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(Navigation.ToAbsoluteUri("/chathub"))
            .Build();

            await hubConnection.StartAsync();
        }
*/
        private async Task Send()
        {
            if (hubConnection is not null)
            {
                await hubConnection.SendAsync("SendMessage", userInput, messageInput);
            }
        }
        public bool IsConnected =>
            hubConnection?.State == HubConnectionState.Connected;

        public async ValueTask DisposeAsync()
        {
            if (hubConnection is not null)
            {
                await hubConnection.DisposeAsync();
            }
        }
    }
}