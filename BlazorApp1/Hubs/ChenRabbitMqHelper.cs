using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace GenericWebConsole.Server.Hubs
{
    /// <summary>
    /// 配合連接RabbitMQ，主要分三部份功能
    /// (1) 在物件建構式時，連接RabbitMQ主機並運用帳密登入。
    /// (2) 提供Send功能
    /// (3) 提供Read功能
    /// </summary>
    public class ChenRabbitMqHelper
    {
        ///公用
        ConnectionFactory connFactory = new ConnectionFactory();

        /// <summary>
        /// 建構式
        /// </summary>
        public ChenRabbitMqHelper
            (string hostName = "localhost", int port = 5672, 
            string uid = "rmqadmin", string pwd = "rmqpasswdofadmin")
        {
            ///準備連線參數
            connFactory.HostName = hostName;
            connFactory.Port = port;
            connFactory.UserName = uid;
            connFactory.Password = pwd;
        }

        public void SendData(ReadOnlyMemory<byte> dataBytes, string queueName )
        {
            //建立連線
            using var connection = connFactory.CreateConnection();
            using var channel = connection.CreateModel();

            //宣告 queues
            channel.QueueDeclare(queueName, false, false, false, null);

            //發訊息
            channel.BasicPublish(String.Empty, queueName, null, dataBytes);
        }

        public ReadOnlyMemory<byte> ReadData(string queueName)
        {
            //建立連線
            using var connection = connFactory.CreateConnection();
            using var channel = connection.CreateModel();

            //宣告 queues
            channel.QueueDeclare(queueName, false, false, false, null);

            //設定參數
            var consumer = new EventingBasicConsumer(channel);
            var result = channel.BasicGet(queueName, true);
            ReadOnlyMemory<byte> dataBytes = null;
            if (result != null) { dataBytes = result.Body; }
            return dataBytes;
        }
    }
}
