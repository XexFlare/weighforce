using System.Collections.Generic;

namespace WeighForce
{
    public class DayInfo
    {
        public string Day { get; set; }
        public int Trucks { get; set; }
    }
    public class ChartInfo
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public double Sub { get; set; }
    }
    public class Dashboard
    {
        public List<ChartInfo> DailyDispatches { get; set; }
        public List<ChartInfo> DailyReceivals { get; set; }
        public List<ChartInfo> TopDispatches { get; set; }
        public List<ChartInfo> TopReceivals { get; set; }
        public List<ChartInfo> DailyDispatchedTons { get; set; }
        public List<ChartInfo> DailyReceivedTons { get; set; }
    }

    public class DispatchReport
    {
        public List<string> Labels { get; set; }
        public List<int> Dispatched { get; set; }
        public List<int> Received { get; set; }
    }
}