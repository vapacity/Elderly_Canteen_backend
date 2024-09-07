namespace Elderly_Canteen.Data.Entities
{
    public class SystemLog
    {
        public int LogId { get; set; }       // 主键
        public string LogLevel { get; set; } // 日志级别：safe, warning, dangerous
        public string Message { get; set; }  // 日志消息
        public DateTime CreatedAt { get; set; } // 创建时间
    }

}
