using Newtonsoft.Json;
using NLog;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace logService
{
    public class Rabbitmq
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static IModel mqchannel = null;

        public IModel MqConnection()
        {
            if (mqchannel == null || !mqchannel.IsOpen)
            {
                ConnectionFactory factory = new ConnectionFactory
                {
                    UserName = Program.OConfig.RabbitmqConnect.UserName,
                    Password = Program.OConfig.RabbitmqConnect.Password,
                    HostName = Program.OConfig.RabbitmqConnect.HostName,
                    Port = Program.OConfig.RabbitmqConnect.Port,
                    VirtualHost = Program.OConfig.RabbitmqConnect.VirtualHost,
                    AutomaticRecoveryEnabled = true
                };
                IConnection connection = factory.CreateConnection();
                mqchannel = connection.CreateModel();
            }
            return mqchannel;
        }

        public void RabbitmqWrite(RabbitmqConnectInfo connect, string queue, object info)
        {
            int index = 0;
            while (index < 3)
            {
                try
                {
                    ConnectionFactory factory = new ConnectionFactory
                    {
                        UserName = connect.UserName,
                        Password = connect.Password,
                        HostName = connect.HostName,
                        Port = connect.Port,
                        VirtualHost = connect.VirtualHost,
                        AutomaticRecoveryEnabled = true
                    };
                    using (IConnection connection = factory.CreateConnection())
                    {
                        using (IModel channel = connection.CreateModel())
                        {
                            channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, null);
                            string input = JsonConvert.SerializeObject(info, Formatting.None);
                            byte[] sendBytes = Encoding.UTF8.GetBytes(input);
                            IBasicProperties properties = channel.CreateBasicProperties();
                            properties.DeliveryMode = 2;
                            channel.BasicPublish("", queue, properties, sendBytes);
                        }
                    }
                    return;
                }
                catch (Exception exc)
                {
                    Logger.Error($"RabbitmqWrite:[{connect.HostName}][{index}][{exc}]");
                    index++;
                }
            }
        }

        public void Write2Exchange(RabbitmqConnectInfo connect, string exchangeName, List<string> queues, object info)
        {
            int index = 0;
            while (index < 3)
            {
                try
                {
                    ConnectionFactory factory = new ConnectionFactory
                    {
                        UserName = connect.UserName,
                        Password = connect.Password,
                        HostName = connect.HostName,
                        Port = connect.Port,
                        VirtualHost = connect.VirtualHost,
                        AutomaticRecoveryEnabled = true
                    };
                    using (IConnection connection = factory.CreateConnection())
                    {
                        using (IModel channel = connection.CreateModel())
                        {
                            BindExchange.Bind(channel, exchangeName, queues);
                            string input = JsonConvert.SerializeObject(info, Formatting.None);
                            byte[] sendBytes = Encoding.UTF8.GetBytes(input);
                            IBasicProperties properties = channel.CreateBasicProperties();
                            properties.DeliveryMode = 2;
                            channel.BasicPublish(exchangeName, "", properties, sendBytes);
                        }
                    }
                    return;
                }
                catch (Exception exc)
                {
                    Logger.Error($"Write2Exchange:[{connect.HostName}][{index}][{exc}]");
                    index++;
                }
            }
        }
    }

    public class BindExchange
    {
        private static bool isbind;

        public static void Bind(IModel channel, string exchangeName, List<string> queues)
        {
            if (!isbind)
            {
                channel.ExchangeDeclare(exchangeName, "fanout", durable: true);
                queues.ForEach((Action<string>)delegate (string m)
                {
                    channel.QueueDeclare(m, durable: true, exclusive: false, autoDelete: false, null);
                    channel.QueueBind(m, exchangeName, "");
                });
                isbind = true;
            }
        }
    }
}
