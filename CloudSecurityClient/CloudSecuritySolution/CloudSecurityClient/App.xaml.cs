using CloudSecurityClient.Services;
using CloudSecurityClient.Utilities;
using CloudSecurityClient.Views;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Windows;

namespace CloudSecurityClient
{
    public partial class App : Application
    {
        public static IConfiguration Configuration { get; private set; }
        public static AuthService AuthService { get; private set; }
        public static ApiClient ApiClient { get; private set; }
        public static TelemetryService TelemetryService { get; private set; }
        public static BackgroundMonitor BackgroundMonitor { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            // 安全验证
            if (!SecurityValidator.VerifyAssemblyIntegrity())
            {
                MessageBox.Show("应用程序完整性验证失败，无法启动。", "安全错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown(1001);
                return;
            }

            // 加载配置
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            // 初始化服务
            ApiClient = new ApiClient(Configuration["ApiSettings:BaseUrl"]);
            AuthService = new AuthService(ApiClient);
            TelemetryService = new TelemetryService(ApiClient);
            BackgroundMonitor = new BackgroundMonitor(TelemetryService);

            // 尝试静默登录
            if (AuthService.TrySilentLogin())
            {
                StartMainApplication();
            }
            else
            {
                // 显示登录窗口
                var loginView = new LoginView();
                loginView.Show();
            }

            base.OnStartup(e);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            SecureStorage.WipeSensitiveData();
            BackgroundMonitor.Stop();
            TelemetryService.Stop();
        }

        public static void StartMainApplication()
        {
            // 启动后台服务
            BackgroundMonitor.Start();
            TelemetryService.Start();

            // 创建主窗口
            var mainView = new MainView();
            mainView.Show();

            // 关闭登录窗口（如果存在）
            foreach (Window window in Current.Windows)
            {
                if (window is LoginView) window.Close();
            }
        }
    }
}