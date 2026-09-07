using BassemCore.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DVLD_ClientTier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isSidebarExpanded = true;

        public MainWindow()
        {
            InitializeComponent();

            // تحميل شاشة الداشبورد افتراضياً عند تشغيل النظام
            MainFrame.Navigate(new DashboardPage());

            LoadUserInfo();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            // مخصص لتحديث التاريخ أو بيانات المستخدم عند تفعيل النافذة
        }

        // 1. زر طي/فتح القائمة الجانبية (Sidebar Toggle)
        private void btnToggleSidebar_Click(object sender, RoutedEventArgs e)
        {
            if (isSidebarExpanded)
            {
                SidebarColumn.Width = new GridLength(70);
                pnlLogoText.Visibility = Visibility.Collapsed;
                isSidebarExpanded = false;
            }
            else
            {
                SidebarColumn.Width = new GridLength(250);
                pnlLogoText.Visibility = Visibility.Visible;
                isSidebarExpanded = true;
            }
        }

        // 2. لوحة التحكم الرئيسية (Dashboard)
        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashboardPage());
        }

        // 3. طلب رخصة قيادة محلية (Local License)
        private void btnLocalLicense_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("📌 هذه الشاشة مخصصة لـ: [ Local License Application ]\nجاهزة لاستقبال الكود الخاص بك هنا.", "معاينة الشاشة", MessageBoxButton.OK, MessageBoxImage.Information);
            // قريباً استبدلها بـ: MainFrame.Navigate(new LocalLicensePage());
        }

        // 4. رخصة قيادة دولية (International License)
        private void btnInternationalLicense_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("📌 هذه الشاشة مخصصة لـ: [ International License ]\nجاهزة لاستقبال الكود الخاص بك هنا.", "معاينة الشاشة", MessageBoxButton.OK, MessageBoxImage.Information);
            // قريباً استبدلها بـ: MainFrame.Navigate(new InternationalLicensePage());
        }

        // 5. تجديد رخصة قيادة (Renew License)
        private void btnRenewLicense_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("📌 هذه الشاشة مخصصة لـ: [ Renew License ]\nجاهزة لاستقبال الكود الخاص بك هنا.", "معاينة الشاشة", MessageBoxButton.OK, MessageBoxImage.Information);
            // قريباً استبدلها بـ: MainFrame.Navigate(new RenewLicensePage());
        }

        // 6. إدارة الطلبات (Manage Applications)
        private void btnManageLocalApps_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ManageApplicationTypesPage());
            //MessageBox.Show("📌 هذه الشاشة مخصصة لـ: [ Manage Local Driving License Applications ]\nجاهزة لاستقبال الكود الخاص بك هنا.", "معاينة الشاشة", MessageBoxButton.OK, MessageBoxImage.Information);
            // قريباً استبدلها بـ: MainFrame.Navigate(new ManageLocalApplicationsPage());
        }

        private void btnManageTestTypes_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TestTypePage());
            //MessageBox.Show("📌 هذه الشاشة مخصصة لـ: [ Manage Test Types ]\nجاهزة لاستقبال الكود الخاص بك هنا.", "معاينة الشاشة", MessageBoxButton.OK, MessageBoxImage.Information);
            // قريباً استبدلها بـ: MainFrame.Navigate(new ManageLocalApplicationsPage());
        }

        // 7. الرخص المحجوزة (Detained Licenses)
        private void btnManageDetained_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("📌 هذه الشاشة مخصصة لـ: [ Detained Licenses / Manage Detained Licenses ]\nجاهزة لاستقبال الكود الخاص بك هنا.", "معاينة الشاشة", MessageBoxButton.OK, MessageBoxImage.Information);
            // قريباً استبدلها بـ: MainFrame.Navigate(new DetainedLicensesPage());
        }

        // 8. إدارة الأشخاص (Manage People)
        private void btnManagePeople_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ManagePeoplePage());

            //MessageBox.Show("📌 هذه الشاشة مخصصة لـ: [ Manage People ]\nجاهزة لاستقبال الكود الخاص بك هنا.", "معاينة الشاشة", MessageBoxButton.OK, MessageBoxImage.Information);
            // قريباً استبدلها بـ: MainFrame.Navigate(new ManagePeoplePage());
        }

        // 9. إدارة السائقين (Drivers)
        private void btnDrivers_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("📌 هذه الشاشة مخصصة لـ: [ Drivers Management ]\nجاهزة لاستقبال الكود الخاص بك هنا.", "معاينة الشاشة", MessageBoxButton.OK, MessageBoxImage.Information);
            // قريباً استبدلها بـ: MainFrame.Navigate(new DriversPage());
        }

        // 10. إدارة المستخدمين (Users)
        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ManageUserPage());

            //MessageBox.Show("📌 هذه الشاشة مخصصة لـ: [ Manage Users ]\nجاهزة لاستقبال الكود الخاص بك هنا.", "معاينة الشاشة", MessageBoxButton.OK, MessageBoxImage.Information);
            // قريباً استبدلها بـ: MainFrame.Navigate(new UsersPage());
        }

        // 11. معلومات المستخدم الحالي (Current User Info)
        private void btnCurrentUserInfo_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new UserInfoPage(ClsGlobal.CurrentUser.UserID));

            //MessageBox.Show("📌 هذه الشاشة مخصصة لـ: [ Current User Info / Profile ]\nجاهزة لاستقبال الكود الخاص بك هنا.", "معاينة الشاشة", MessageBoxButton.OK, MessageBoxImage.Information);
            // قريباً استبدلها بـ: MainFrame.Navigate(new UserInfoPage());
        }

        
        private void btnReplacementLicenses_Click(object sender, RoutedEventArgs e)
        {
            //MainFrame.Navigate(new UserInfoPage(ClsGlobal.CurrentUser.UserID));

            MessageBox.Show("📌 هذه الشاشة مخصصة لـ: [ Replacement Licenses / Profile ]\nجاهزة لاستقبال الكود الخاص بك هنا.", "معاينة الشاشة", MessageBoxButton.OK, MessageBoxImage.Information);
            // قريباً استبدلها بـ: MainFrame.Navigate(new UserInfoPage());
        }

        
        private void btnTestAppointments_Click(object sender, RoutedEventArgs e)
        {
            //MainFrame.Navigate(new UserInfoPage(ClsGlobal.CurrentUser.UserID));

            MessageBox.Show("📌 هذه الشاشة مخصصة لـ: [ Test Appointments / Profile ]\nجاهزة لاستقبال الكود الخاص بك هنا.", "معاينة الشاشة", MessageBoxButton.OK, MessageBoxImage.Information);
            // قريباً استبدلها بـ: MainFrame.Navigate(new UserInfoPage());
        }

        
        private void btnReleaseDetainedLicense_Click(object sender, RoutedEventArgs e)
        {
            //MainFrame.Navigate(new UserInfoPage(ClsGlobal.CurrentUser.UserID));

            MessageBox.Show("📌 هذه الشاشة مخصصة لـ: [ Release Detained License / Profile ]\nجاهزة لاستقبال الكود الخاص بك هنا.", "معاينة الشاشة", MessageBoxButton.OK, MessageBoxImage.Information);
            // قريباً استبدلها بـ: MainFrame.Navigate(new UserInfoPage());
        }

        

        // 12. تغيير كلمة المرور (Change Password)
        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ChangePasswordPage(ClsGlobal.CurrentUser.UserID));

            //MessageBox.Show("📌 هذه الشاشة مخصصة لـ: [ Change Password ]\nجاهزة لاستقبال الكود الخاص بك هنا.", "معاينة الشاشة", MessageBoxButton.OK, MessageBoxImage.Information);
            // قريباً استبدلها بـ: MainFrame.Navigate(new ChangePasswordPage());
        }

        // 13. تسجيل الخروج (Sign Out)
        private void btnSignOut_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                           "هل أنت متأكد من تسجيل الخروج؟",
                           "تأكيد الخروج",
                           MessageBoxButton.YesNo,
                           MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // 1. فتح نافذة تسجيل الدخول من جديد
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();

                // 2. إغلاق النافذة الرئيسية الحالية
                this.Close();
            }
        }

        #region User Info Helpers

        /// <summary>
        /// تحميل بيانات المستخدم الحالي وعرض اسمه في شريط الترحيب
        /// </summary>
        private void LoadUserInfo()
        {
            if (ClsGlobal.CurrentUser != null)
            {
                // يمكنك استخدام UserName أو FullName حسب تصميم الكلاس لديك في مشروع الـ DVLD
                txtWelcomeUser.Text = $"👋 Welcome Back, {ClsGlobal.CurrentUser.UserName}";

                // 2. عرض اليوم والتاريخ الحالي بالصيغة الإنجليزية (مثال: Friday, August 07, 2026)
                txtHeaderDate.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
                txtStatusDate.Text = txtHeaderDate.Text;

                txtTopUserName.Text = ClsGlobal.CurrentUser.PersonInfo.FullName;
                txtTopUserRole.Text = (ClsGlobal.CurrentUser.UserName.ToLower() == "admin") ? "System Admin" : "Standard User";
                txtStatusRole.Text = txtTopUserRole.Text;

                LoadUserAvatar();
            }
        }

        /// <summary>
        /// تحميل صورة المستخدم الحالي أو عرض الأيقونة التلقائية باستخدام الكلاس المساعد
        /// </summary>
        private void LoadUserAvatar()
        {
            try
            {
                // 1. جلب بيانات الشخص المرتبط بالمستخدم الحالي
                var person = ClsGlobal.CurrentUser?.PersonInfo;
                string imagePath = person?.ImagePath;

                // 2. محاولة تحميل الصورة بحرية دون قفل الملف
                var userImage = ClsImageHelper.LoadImageFreely(imagePath);

                if (userImage != null)
                {
                    // الحالة الأولى: توجد صورة حقيقية
                    imgUserAvatar.Source = userImage;

                    // إخفاء أيقونة الجنس لو كانت موجودة في الواجهة
                    if (txtUserEmoji != null)
                    {
                        txtUserEmoji.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    // الحالة الثانية: لا توجد صورة، نلغي مصدر الصورة
                    imgUserAvatar.Source = null;

                    // التأكد من إظهار عنصر الـ Emoji لأنه قد تم إخفاؤه مسبقاً
                    if (txtUserEmoji != null)
                    {
                        txtUserEmoji.Visibility = Visibility.Visible;
                    }

                    // تمرير الـ TextBlock والـ Person للكلاس المساعد لضبط الأيقونة المناسبة (ذكر/أنثى)
                    ClsImageHelper.SetDefaultGenderIcon(txtUserEmoji, person);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading user image: " + ex.Message);
            }
        }
        #endregion

    }
}