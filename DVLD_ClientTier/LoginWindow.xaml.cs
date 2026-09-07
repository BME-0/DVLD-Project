using DVLD_BusinessLayer;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DVLD_ClientTier
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        #region Fields & Properties

        private bool _isPasswordVisible = false;

        #endregion

        #region Constructor

        public LoginWindow()
        {
            InitializeComponent();
            LoadStoredCredentials();
        }

        #endregion

        #region Credentials Management

        /// <summary>
        /// استرجاع بيانات الدخول المحفوظة مسبقاً إذا كان خيار تذكرني مفعلاً
        /// </summary>
        private void LoadStoredCredentials()
        {
            string username = string.Empty;
            string password = string.Empty;

            if (ClsGlobal.GetStoredCredential(ref username, ref password))
            {
                txtUsername.Text = username;
                txtPassword.Password = password;
                txtVisiblePassword.Text = password;
                chkRememberMe.IsChecked = true;
            }
        }

        #endregion

        #region Window Controls & Mouse Events

        /// <summary>
        /// تمكين سحب النافذة بالماوس لعدم وجود Title Bar تقليدي
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        /// <summary>
        /// إغلاق التطبيق بالكامل من زر الـ X
        /// </summary>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region Password Visibility & Syncing Events

        /// <summary>
        /// مزامنة خانة الباسورد السرية مع الخانة الظاهرة
        /// </summary>
        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!_isPasswordVisible)
            {
                txtVisiblePassword.Text = txtPassword.Password;
            }
        }

        private void TxtVisiblePassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isPasswordVisible)
            {
                txtPassword.Password = txtVisiblePassword.Text;
            }
        }

        /// <summary>
        /// حدث الضغط على زر العين لإظهار أو إخفاء كلمة المرور
        /// </summary>
        private void BtnShowHidePassword_Click(object sender, RoutedEventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;

            if (_isPasswordVisible)
            {
                txtVisiblePassword.Text = txtPassword.Password;
                txtPassword.Visibility = Visibility.Collapsed;
                txtVisiblePassword.Visibility = Visibility.Visible;
                btnShowHidePassword.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2563EB"));
            }
            else
            {
                txtPassword.Password = txtVisiblePassword.Text;
                txtVisiblePassword.Visibility = Visibility.Collapsed;
                txtPassword.Visibility = Visibility.Visible;
                btnShowHidePassword.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));
            }
        }

        #endregion

        #region Authentication & Login Logic

        /// <summary>
        /// حدث الضغط على زر تسجيل الدخول
        /// </summary>
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = _isPasswordVisible ? txtVisiblePassword.Text.Trim() : txtPassword.Password.Trim();
            bool rememberMe = chkRememberMe.IsChecked ?? false;

            // التحقق من أن الحقول ليست فارغة
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both username and password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUsername.Focus();
                return;
            }

            // البحث عن المستخدم في قاعدة البيانات
            ClsUser user = ClsUser.FindByUsernameAndPassword(username, password);

            if (user != null)
            {
                // التحقق مما إذا كان الحساب مفعلاً
                if (!user.IsActive)
                {
                    MessageBox.Show("This user is not active, please contact admin.", "Inactive Account", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtUsername.Focus();
                    return;
                }

                // حفظ المستخدم الحالي في الـ Global لتتمكن من استخدامه في باقي الشاشات
                ClsGlobal.CurrentUser = user;

                // معالجة خاصية تذكرني (حفظ أو مسح الملف)
                if (rememberMe)
                {
                    ClsGlobal.RememberUsernameAndPassword(username, password);
                }
                else
                {
                    ClsGlobal.RememberUsernameAndPassword(string.Empty, string.Empty);
                }

                // الانتقال إلى الـ MainWindow وإغلاق نافذة تسجيل الدخول الحالية
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid Username or Password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                txtUsername.Focus();
                txtUsername.SelectAll();
            }
        }

        #endregion
    }
}