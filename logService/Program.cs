using logService.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace logService
{
    public class Program
    {
        static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static Config OConfig = Config.GetConfig();

        public static IFreeSql mysqlLazy;

        public static List<int> typeList = new List<int>();
        public static void Main(string[] args)
        {
            Initailize();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls($"http://*:{OConfig.Port}");
                    webBuilder.UseKestrel();
                    webBuilder.UseStartup<Startup>();
                });

        public static void Initailize()
        {
            try
            {
                mysqlLazy = new FreeSql.FreeSqlBuilder()
                                        .UseConnectionString(FreeSql.DataType.MySql, OConfig.DBConnectionString)
                                        .UseAutoSyncStructure(true)//自动迁移实体的结构到数据库 ,生成环境慎用
                                        .UseMonitorCommand(
                                            cmd => Logger.Info($"线程[{Thread.CurrentThread.ManagedThreadId}]:[{cmd.CommandText}]") //监听SQL命令对象，在执行前
                                            )
                                        .Build();

                typeList = mysqlLazy.Select<TypeInfo>().ToList(a => a.Type);

                foreach (var m in OConfig.LogQueue)
                {
                    Task.Factory.StartNew(() =>
                    {
                        QueueConsumer queueConsumer = new QueueConsumer();
                        queueConsumer.RabbitmqReceive(m);
                    }, TaskCreationOptions.LongRunning);
                }

                Logger.Info("版本 1.0.0 程序初始化成功");
            }
            catch (Exception exc)
            {
                Logger.Error($"版本 1.0.0 程序初始化失败:{exc}");
                throw new Exception(exc.ToString());
            }
        }

    }
}
