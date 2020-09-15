using logService.Model;
using Newtonsoft.Json;
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace logService
{
    internal class QueueConsumer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void RabbitmqReceive(string queue)
        {
            try
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
                IModel channel = connection.CreateModel();
                channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, null);
                EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue, autoAck: false, consumer);
                consumer.Received += delegate (object ch, BasicDeliverEventArgs ea)
                {
                    try
                    {
                        string @string = Encoding.UTF8.GetString(ea.Body.ToArray());
                        if (Program.OConfig.LogQueue.Contains(queue))
                        {
                            Analysis(@string, channel, ea);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Rabbitmq:" + ex.ToString());
                    }
                };
            }
            catch (Exception exc)
            {
                Logger.Error("RabbitmqReceive:[" + exc.Message + "]");
            }
        }

        public void Analysis(string body, IModel channel, BasicDeliverEventArgs ea)
        {
            try
            {
                try
                {
                    Logger.Info("日志队列接收到消息 [" + body + "]");

                    var info = JsonConvert.DeserializeObject<LogInfo>(body);

                    if (Program.typeList.Contains(info.Type))
                    {
                        var log = Program.mysqlLazy.GetGuidRepository<LogInfo>(null, oldname => $"{oldname}_{DateTime.Now:yyyy}");
                        log.Insert(info);

                        channel.BasicAck(ea.DeliveryTag, multiple: false);
                    }
                    else
                    {
                        Logger.Info($"日志队列 mq:[未找到此日志类型:[{info.Type}]]");

                        //添加此类型
                        var repo = Program.mysqlLazy.GetGuidRepository<TypeInfo>();
                        repo.Insert(new TypeInfo()
                        {
                            Type = info.Type,
                            Name = info.Name,
                            ServiceName = info.ServiceName
                        });

                        Program.typeList.Add(info.Type);
                        //添加此类型end

                        var log = Program.mysqlLazy.GetGuidRepository<LogInfo>(null, oldname => $"{oldname}_{DateTime.Now:yyyy}");
                        log.Insert(info);

                        channel.BasicAck(ea.DeliveryTag, multiple: false);
                    }
                }
                catch (Exception exc)
                {
                    Logger.Error("日志队列 mq:[" + exc.ToString() + "]");
                    channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
                    System.Threading.Thread.Sleep(5 * 1000);
                }
            }
            catch (Exception exc)
            {
                Logger.Error("日志队列 mq:[" + exc.ToString() + "]");
            }
        }
    }
}
