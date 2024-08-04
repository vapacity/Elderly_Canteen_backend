using System;
namespace Elderly_Canteen.Tools
{
    public class WeekCalculator
    {
        public static int CalculateWeekNumber(DateTime baseDate, DateTime currentDate)
        {
            // 计算两个日期之间的时间间隔
            TimeSpan timeSpan = currentDate - baseDate;

            // 计算完整周数，使用整除
            int weekNumber = (int)(timeSpan.TotalDays / 7);

            // 如果需要从1开始计数而不是从0开始计数，增加1
            return weekNumber + 1;
        }

        public static void Main()
        {
            // 基准时间（可以是任意指定的日期）
            DateTime baseDate = new DateTime(2024, 1, 1);

            // 当前时间
            DateTime currentDate = DateTime.Now;

            // 计算当前周数
            int weekNumber = CalculateWeekNumber(baseDate, currentDate);

            Console.WriteLine($"当前周数: {weekNumber}");
        }
    }

}
