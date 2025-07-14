using CloudSecurityClient.Services;
using System.Windows;

namespace CloudSecurityClient.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;
            string totp = txtTotp.Text;

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(totp))
            {
                MessageBox.Show("请输入所有字段", "输入错误",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = await App.AuthService.Authenticate(username, password, totp);
            if (result)
            {
                App.StartMainApplication();
                this.Close();
            }
            else
            {
                MessageBox.Show("登录失败，请检查凭据", "认证错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}