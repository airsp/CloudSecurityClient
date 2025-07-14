using System.Windows;
using System.Windows.Forms;

namespace CloudSecurityClient.Views
{
    public partial class TrayIconView : Window
    {
        private NotifyIcon _notifyIcon;

        public TrayIconView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath),
                Text = "云安全监控",
                Visible = true
            };

            _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("打开控制台", null, (s, e) => ShowMainWindow());
            _notifyIcon.ContextMenuStrip.Items.Add("退出", null, (s, e) => ExitApplication());

            _notifyIcon.DoubleClick += (s, e) => ShowMainWindow();
        }

        private void ShowMainWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainView mainView)
                {
                    mainView.WindowState = WindowState.Normal;
                    mainView.Show();
                    mainView.Activate();
                    break;
                }
            }
            this.Close();
        }

        private void ExitApplication()
        {
            _notifyIcon.Visible = false;
            Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
        }
    }
}