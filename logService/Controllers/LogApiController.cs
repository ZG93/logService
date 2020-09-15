using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using logService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace logService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogApiController : ControllerBase
    {
        static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        readonly string earlistTime = "2020-01-01 01:01:01";

        [HttpGet("search")]
        public JsonResult Search([FromQuery] int? type, [FromQuery] string orderid, [FromQuery] string user,
            [FromQuery] string stime, [FromQuery] string etime, [FromQuery] int page = 1, [FromQuery] int count = 20)
        {
            try
            {
                if (type == null)
                    return new JsonResult(new RetInfo<string>()
                    {
                        code = Code.NotFindType,
                        message = "未找到日志类型",
                    });

                var sql = Program.mysqlLazy.Select<LogInfo>().Where(m => m.Type == type);

                DateTime date = DateTime.Now;
                int enddate = Convert.ToDateTime(earlistTime).Year;

                while (date.Year >= enddate)
                {
                    var table = $"{nameof(LogInfo)}_{date:yyyy}";
                    sql = sql.AsTable((type, oldname) => table);

                    date = date.AddYears(-1);
                }

                if (orderid != null)
                    sql = sql.Where(m => m.OrderId == orderid);

                if (user != null)
                    sql = sql.Where(m => m.User == user);

                if (stime != null)
                {
                    var _stime = DateTime.ParseExact(stime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                    sql = sql.Where(m => m.OperationTime >= _stime);
                }

                if (etime != null)
                {
                    var _etime = DateTime.ParseExact(etime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                    sql = sql.Where(m => m.OperationTime <= _etime);
                }

                var info = new PageInfo
                {
                    total = sql.Count(),
                    count = count,
                    page = page
                };

                var totalPage = info.total / count;
                if (info.total % count > 0)
                    totalPage++;

                info.totalpage = totalPage;

                return new JsonResult(new RetInfo<SearchInfo>()
                {
                    code = Code.OK,
                    message = "success",
                    data = new SearchInfo()
                    {
                        pageInfo = info,
                        records = sql.Page(page, count).OrderByDescending(m => m.OperationTime).ToList()
                    }
                });
            }
            catch (Exception exc)
            {
                Logger.Error($"Search :[{exc}]");

                return new JsonResult(new RetInfo<string>()
                {
                    code = Code.SystemError,
                    message = "系统错误",
                });

            }
        }

        [HttpGet("queue/list")]
        public JsonResult Queues()
        {
            try
            {

                return new JsonResult(new RetInfo<List<string>>()
                {
                    code = Code.OK,
                    message = "success",
                    data = Program.OConfig.LogQueue
                });
            }
            catch (Exception exc)
            {
                Logger.Error($"queue/list :[{exc}]");

                return new JsonResult(new RetInfo<string>()
                {
                    code = Code.SystemError,
                    message = "系统错误",
                });

            }
        }

        [HttpGet("queue/register")]
        public JsonResult Register([FromQuery] string queueName)
        {
            try
            {
                if (queueName is null)
                {
                    throw new ArgumentNullException(nameof(queueName));
                }

                if (Program.OConfig.LogQueue.Contains(queueName))
                    throw new Exception($"已存在此队列:{queueName}");
                else
                {
                    Program.OConfig.LogQueue.Add(queueName);
                    Task.Factory.StartNew(() =>
                    {
                        QueueConsumer queueConsumer = new QueueConsumer();
                        queueConsumer.RabbitmqReceive(queueName);
                    }, TaskCreationOptions.LongRunning);
                    Config.Write(Program.OConfig);
                }

                return new JsonResult(new RetInfo<string>()
                {
                    code = Code.OK,
                    message = "success",
                    data = "注册成功"
                });
            }
            catch (Exception exc)
            {
                Logger.Error($"queue/list :[{exc}]");

                return new JsonResult(new RetInfo<string>()
                {
                    code = Code.SystemError,
                    message = "系统错误",
                    data = exc.Message
                });

            }
        }

    }
}
