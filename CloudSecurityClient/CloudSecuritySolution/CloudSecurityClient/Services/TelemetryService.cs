using CloudSecurityClient.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace CloudSecurityClient.Services
{
    public class TelemetryService
    {
        private readonly ApiClient _apiClient;
        private readonly Timer _telemetryTimer;
        private readonly List<SecurityEvent> _eventBuffer = new List<SecurityEvent>();

        public TelemetryService(ApiClient apiClient)
        {
            _apiClient = apiClient;
            _telemetryTimer = new Timer(TimeSpan.FromMinutes(5).TotalMilliseconds);
            _telemetryTimer.Elapsed += async (s, e) => await SendTelemetryAsync();
        }

        public void Start()
        {
            _telemetryTimer.Start();
        }

        public void Stop()
        {
            _telemetryTimer.Stop();
            SendTelemetryAsync().Wait(); // 发送剩余数据
        }

        public void LogEvent(string eventType, string details, string source = "Client")
        {
            lock (_eventBuffer)
            {
                _eventBuffer.Add(new SecurityEvent
                {
                    EventType = eventType,
                    EventTime = DateTime.UtcNow,
                    Details = details,
                    Source = source
                });
            }
        }

        private async Task SendTelemetryAsync()
        {
            List<SecurityEvent> eventsToSend;
            lock (_eventBuffer)
            {
                eventsToSend = new List<SecurityEvent>(_eventBuffer);
                _eventBuffer.Clear();
            }

            if (eventsToSend.Count == 0) return;

            var telemetry = new TelemetryData
            {
                MachineId = GetMachineId(),
                Events = eventsToSend,
                CpuUsage = GetCpuUsage(),
                MemoryUsage = GetMemoryUsage(),
                ActiveProcesses = GetActiveProcesses()
            };

            try
            {
                await _apiClient.PostAsync("api/telemetry", telemetry);
            }
            catch { /* 静默失败，下次重试 */ }
        }

        private string GetMachineId()
        {
            return Environment.MachineName + "_" + Environment.UserName;
        }

        private double GetCpuUsage()
        {
            using var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue(); // 首次调用返回0
            System.Threading.Thread.Sleep(500);
            return Math.Round(cpuCounter.NextValue(), 1);
        }

        private double GetMemoryUsage()
        {
            using var memCounter = new PerformanceCounter("Memory", "Available MBytes");
            float availableMB = memCounter.NextValue();
            using var totalMemCounter = new PerformanceCounter("Memory", "Committed Bytes");
            float totalMB = totalMemCounter.NextValue() / 1024 / 1024;
            return Math.Round((totalMB - availableMB) / totalMB * 100, 1);
        }

        private List<string> GetActiveProcesses()
        {
            return Process.GetProcesses()
                .Where(p => !string.IsNullOrEmpty(p.ProcessName))
                .GroupBy(p => p.ProcessName)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => g.Key)
                .ToList();
        }
    }
}