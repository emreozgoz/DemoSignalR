using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System.Data;

namespace WEB.SignalRConnections
{
    public class SignalRConnection(NavigationManager NavManager)
    {
        //C#’ta, event (olay) bir nesnenin veya bileşenin diğer nesnelerle iletişim kurmasını sağlayan bir mekanizmadır.
        //Bir event, bir olayın gerçekleştiğini bildiren ve bu olaya tepki olarak bir veya daha fazla işlevin çalışmasını sağlayan bir mekanizmadır.

        //Action<T> sınıfından yeni bir değişken oluşturup, değer olarak bir method’un kendisini atayabiliriz.
        public event Action? ConnectionStateChanged;
        public string ConnectionState = string.Empty;

        //Bu Nav manager Blazor içerisinde yönlendirmeyi sağlayan bir class
        public readonly HubConnection hubConnection = new HubConnectionBuilder().WithUrl(NavManager.ToAbsoluteUri("https://localhost:7032/connect")).Build();

        public async Task StartConnection()
        {
            //Eğer herhangi bir bağlantı yok ise StartAsync metodu ile connectionı başlatıyor.
            if (hubConnection.State != HubConnectionState.Connected)
            {
                await hubConnection.StartAsync();
            }
            GetConnectionState();
        }

        public async Task CloseConnection()
        {
            //Eğer herhangi bir bağlantı var ise CloseConnection metodu ile connectionı bitiriyor.
            if (hubConnection.State == HubConnectionState.Connected)
            {
                await hubConnection.StopAsync();
            }
            GetConnectionState();
        }

        public void GetConnectionState()
        {
            switch (hubConnection.State)
            {
                case HubConnectionState.Connected:
                    Invoke("Connected");
                    break;
                case HubConnectionState.Connecting:
                    Invoke("Connecting...");
                    break;
                case HubConnectionState.Reconnecting:
                    Invoke("Reconnecting...");
                    break;
                case HubConnectionState.Disconnected:
                    Invoke("Disconnected...");
                    break;
                default:
                    ConnectionState = "Unknown error occured";
                    break;
            }
        }

        void Invoke(string message)
        {
            ConnectionState = message;
            ConnectionStateChanged?.Invoke();
        }
    }
}
