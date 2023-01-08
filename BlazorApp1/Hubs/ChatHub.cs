using Microsoft.AspNetCore.SignalR;
using System.Text;

namespace GenericWebConsole.Server.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
    public class ConsoleHub : Hub
    {

        ///公用物件

        //Rabbit端(在program.cs設定)
        public static ChenRabbitMqHelper? chenRabbitMqHelper;
        //用戶端
        public static IHubCallerClients? TheClients;

        // 將被遠端Razor呼叫的函數
        public void SendMqData(DateTime dateTime, string cmd)
        {
            //
            Log("嘗試轉發命令：" + cmd);

            //發訊息to RabbitMQ
            var dataBytes = Encoding.UTF8.GetBytes(cmd);
            if (chenRabbitMqHelper != null)
            {
                chenRabbitMqHelper.SendData(dataBytes, "CmdInput");
                Log("發出命令：" + cmd);
            }
            else
            {
                Log("chenRabbitMqHelper未設定");
            }
            //
        }

        //配合timer求設定參數
        public static async void ReadMqData(object? state)
        {
            /// 讀取cmdResponse
            if (chenRabbitMqHelper != null)
            {
                var dataBytes = chenRabbitMqHelper.ReadData("CmdResponse");

                //轉送Client
                if (TheClients != null && dataBytes.Length != 0)
                {
                    string context = Encoding.UTF8.GetString(dataBytes.ToArray());
                    await TheClients.All.SendAsync("UserReadCmdResult", context);
                    Log("收到CmdResponse：" + context);
                }
            }
            else
            {
                Log("嘗試收CmdResponse，但chenRabbitMqHelper未設定");
            }
        }

        //發日誌
        public static void Log(string log)
        {
            //轉送Client
            if (TheClients != null)
            {
                TheClients.All.SendAsync("GetWebsiteLog", DateTime.Now, log);
            }
        }

        // 取得Client物件
        public override Task OnConnectedAsync()
        {
            //
            Log("新Hub連接");

            //
            if (TheClients == null)
            {
                TheClients = this.Clients;
            }

            return base.OnConnectedAsync();
        }
    }
}