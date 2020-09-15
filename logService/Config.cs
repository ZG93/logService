using System.Collections.Generic;
using System.IO;
using System.Text;

namespace logService
{
    public class RabbitmqConnectInfo
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string HostName { get; set; }
        public string VirtualHost { get; set; }
        public int Port { get; set; }
    }

    public class Config
    {
        public const string CString = "application.json";
        public int Port { get; set; }
        public string DBConnectionString { get; set; }
        public RabbitmqConnectInfo RabbitmqConnect { get; set; }

        public List<string> LogQueue { get; set; } = new List<string>() { "log_service_msg_queue" };

        private static Config _config;
        public static Config GetConfig()
        {
            if (_config != null)
                return _config;

            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), CString)))
                throw new FileNotFoundException("没有发现配置文件。");

            var json = File.ReadAllText(CString, Encoding.UTF8);
            _config = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(json);
            return _config;
        }

        public static void Write(Config c)
        {
            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), CString), Newtonsoft.Json.JsonConvert.SerializeObject(c, Newtonsoft.Json.Formatting.Indented));
        }
    }
}
