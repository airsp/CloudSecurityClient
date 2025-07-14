using CloudSecurityClient.Services;
using System.Collections.Generic;
using System.Windows;
using CloudSecurityClient.Models;

namespace CloudSecurityClient.Views
{
    public partial class MainView : Window
    {
        private readonly List<SecurityEvent> _events = new List<SecurityEvent>();

        public MainView()
        {
            InitializeComponent();
            LoadEvents();
        }

        private void LoadEvents()
        {
            // 模拟事件数据
            _events.AddRange(new[]
            {
                new SecurityEvent { EventType = "Login", EventTime = DateTime.Now.AddMinutes(-10), Details = "用户登录成功" },
                new SecurityEvent { EventType = "ConfigChange", EventTime = DateTime.Now.AddMinutes(-5), Details = "设置更新" },
                new SecurityEvent { EventType = "Telemetry", EventTime = DateTime.Now.AddMinutes(-2), Details = "发送遥测数据" }
            });

            dgEvents.ItemsSource = _events;
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            App.TelemetryService.LogEvent("UserAction", "手动刷新事件日志");
            LoadEvents();
            txtStatus.Text = "数据已刷新";
        }

        private void BtnTestEvent_Click(object sender, RoutedEventArgs e)
        {
            App.TelemetryService.LogEvent("TestEvent", "手动触发的测试事件");
            LoadEvents();
            txtStatus.Text = "测试事件已发送";
        }

        private void MenuLogout_Click(object sender, RoutedEventArgs e)
        {
            App.AuthService.Logout();
            new LoginView().Show();
            this.Close();
        }

        private void MenuWipeData_Click(object sender, RoutedEventArgs e)
        {
            SecureStorage.WipeSensitiveData();
            MessageBox.Show("本地敏感数据已清除", "操作成功");
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuShowTray_Click(object sender, RoutedEventArgs e)
        {
            // 系统托盘实现
            var trayView = new TrayIconView();
            trayView.Show();
            this.WindowState = WindowState.Minimized;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = WindowState.Minimized;
        }
    }
}