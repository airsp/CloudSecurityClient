using System;
using System.Diagnostics;
using System.Threading;
using System.Timers;

namespace CloudSecurityClient.Services
{
    public class BackgroundMonitor
    {
        private readonly TelemetryService _telemetryService;
        private readonly Timer _monitorTimer;
        private PerformanceCounter _networkCounter;

        public BackgroundMonitor(TelemetryService telemetryService)
        {
            _telemetryService = telemetryService;
            _monitorTimer = new Timer(TimeSpan.FromMinutes(1).TotalMilliseconds);
            _monitorTimer.Elapsed += MonitorElapsed;

            try
            {
                _networkCounter = new PerformanceCounter("Network Interface", "Bytes Total/sec",
                    GetPrimaryNetworkInterface());
            }
            catch
            {
                _networkCounter = null;
            }
        }

        public void Start()
        {
            _monitorTimer.Start();
        }

        public void Stop()
        {
            _monitorTimer.Stop();
        }

        private void MonitorElapsed(object sender, ElapsedEventArgs e)
        {
            // 监控异常进程
            CheckSuspiciousProcesses();

            // 记录网络活动
            if (_networkCounter != null)
            {
                float networkUsage = _networkCounter.NextValue();
                if (networkUsage > 1024 * 1024) // 1MB/s
                {
                    _telemetryService.LogEvent("HighNetworkUsage",
                        $"High network activity: {networkUsage / 1024:F2} KB/s");
                }
            }
        }

        private void CheckSuspiciousProcesses()
        {
            try
            {
                var suspiciousNames = new[] { "mimikatz", "powersploit", "cobaltstrike", "metasploit" };
                foreach (var process in Process.GetProcesses())
                {
                    foreach (var name in suspiciousNames)
                    {
                        if (process.ProcessName.Contains(name, StringComparison.OrdinalIgnoreCase))
                        {
                            _telemetryService.LogEvent("SuspiciousProcess",
                                $"Suspicious process detected: {process.ProcessName}",
                                "SecurityMonitor");
                        }
                    }
                }
            }
            catch { /* 忽略监控错误 */ }
        }

        private string GetPrimaryNetworkInterface()
        {
            // 简化实现 - 实际中应选择活动接口
            var category = new PerformanceCounterCategory("Network Interface");
            return category.GetInstanceNames().FirstOrDefault();
        }
    }
}