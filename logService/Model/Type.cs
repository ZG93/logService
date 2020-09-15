using FreeSql.DataAnnotations;

namespace logService.Model
{
    public class TypeInfo
    {
        /// <summary>
        /// 日志类型
        /// </summary>
        [Column(Name = "type", IsPrimary = true)]
        public int Type { get; set; }

        /// <summary>
        /// 日志类型名称
        /// </summary>
        [Column(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// 自身服务名称
        /// </summary>
        [Column(Name = "service_name")]
        public string ServiceName { get; set; }
    }
}
