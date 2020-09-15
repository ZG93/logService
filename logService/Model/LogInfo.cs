using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace logService.Model
{
    [Index("LogInfo_id", "id", true)]
    [Index("LogInfo_type", "type", false)]
    [Index("LogInfo_type_order_id", "type, order_id", false)]
    [Index("LogInfo_type_user", "type, user", false)]
    [Index("LogInfo_type_operation_time", "type, operation_time", false)]
    public class LogInfo
    {
        [Column(Name = "id", IsIdentity = true, IsPrimary = true)]
        public long Id { get; set; }

        /// <summary>
        /// 日志类型
        /// </summary>
        [Column(Name = "type")]
        [JsonProperty(PropertyName = "type")]
        public int Type { get; set; }

        /// <summary>
        /// 日志类型名称
        /// </summary>
        [Column(Name = "name")]
        [JsonProperty(PropertyName = "name")]
        //[Column(IsIgnore = true)]
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// 自身服务名称
        /// </summary>
        [Column(Name = "service_name")]
        [JsonProperty(PropertyName = "service_name")]
        //[Column(IsIgnore = true)]
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ServiceName { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [Column(Name = "user")]
        [JsonProperty(PropertyName = "user")]
        public string User { get; set; }

        /// <summary>
        /// 订单
        /// </summary>
        [Column(Name = "order_id")]
        [JsonProperty(PropertyName = "order_id")]
        public string OrderId { get; set; }

        /// <summary>
        /// 操作日志
        /// </summary>
        [Column(Name = "msg")]
        [JsonProperty(PropertyName = "msg")]
        [MaxLength(-1)]
        public string Msg { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Column(Name = "memo")]
        [JsonProperty(PropertyName = "memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        [Column(Name = "operation_time")]
        [JsonProperty(PropertyName = "operation_time")]
        public DateTime OperationTime { get; set; }

    }
}
