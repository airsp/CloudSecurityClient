using System;
using System.Collections.Generic;

namespace CloudSecurityClient.Models
{
    // 遥测数据模型
    public class TelemetryData
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string MachineId { get; set; }
        public string UserId { get; set; }
        public List<SecurityEvent> Events { get; set; } = new List<SecurityEvent>();
        public double CpuUsage { get; set; }
        public double MemoryUsage { get; set; }
        public List<string> ActiveProcesses { get; set; } = new List<string>();
    }

    public class SecurityEvent
    {
        public string EventType { get; set; }
        public DateTime EventTime { get; set; }
        public string Details { get; set; }
        public string Source { get; set; }
    }
}