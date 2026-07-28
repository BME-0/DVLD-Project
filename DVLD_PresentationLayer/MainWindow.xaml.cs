using System;
using System.Windows;
using System.Windows.Controls;
using DVLD; // Business Layer Reference
using System.Windows.Interop;

namespace DVLD_PresentationLayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields & Properties

        // متغير لمتابعة حالة القائمة الجانبية (مفتوحة / مطوية)
        private bool _isSidebarCollapsed = false;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Navigation Helpers

        /// <summary>
        /// دالة مساعدة لنشر View جديد داخل ContentControl إن وجد في XAML
        /// </summary>
        private void NavigateToView(UserControl userControl)
        {
            try
            {
                // ابحث عن ContentControl المعرف في XAML باسم MainContentFrame
                ContentControl mainFrame = this.FindName("MainContentFrame") as ContentControl;
                ScrollViewer dashboardContent = this.FindName("pnlDashboardContent") as ScrollViewer;

                if (mainFrame != null)
                {
                    mainFrame.Content = userControl;
                    mainFrame.Visibility = Visibility.Visible;

                    if (dashboardContent != null)
                    {
                        dashboardContent.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء التنقل بين الشاشات:\n{ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// العودة للشاشة الرئيسية (Dashboard)
        /// </summary>
        private void ShowDashboardHome()
        {
            ContentControl mainFrame = this.FindName("MainContentFrame") as ContentControl;
            ScrollViewer dashboardContent = this.FindName("pnlDashboardContent") as ScrollViewer;

            if (mainFrame != null)
            {
                mainFrame.Content = null;
                mainFrame.Visibility = Visibility.Collapsed;
            }

            if (dashboardContent != null)
            {
                dashboardContent.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region UI & Navigation Events

        /// <summary>
        /// طي وتوسيع القائمة الجانبية (Sidebar)
        /// </summary>
        private void btnToggleSidebar_Click(object sender, RoutedEventArgs e)
        {
            if (_isSidebarCollapsed)
            {
                // توسيع القائمة الجانبية إلى العرض الطبيعي
                SidebarColumn.Width = new GridLength(250);
                pnlLogoText.Visibility = Visibility.Visible;
                _isSidebarCollapsed = false;
            }
            else
            {
                // طي القائمة الجانبية لتقديم مساحة أكبر للمحتوى الرئيسي
                SidebarColumn.Width = new GridLength(70);
                pnlLogoText.Visibility = Visibility.Collapsed;
                _isSidebarCollapsed = true;
            }
        }

        /// <summary>
        /// العودة للوحة التحكم الرئيسية (Dashboard)
        /// </summary>
        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            ShowDashboardHome();
        }

        #endregion

        #region People & Drivers Management

        /// <summary>
        /// فتح شاشة إدارة الأشخاص
        /// </summary>
        private void btnManagePeople_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // الخيار 1: إظهار النافذة كـ Dialog منبثقة فوق النافذة الرئيسية
                ManagePeopleWindow managePeople = new ManagePeopleWindow();
                managePeople.Owner = this;
                managePeople.ShowDialog();

                // الخيار 2: إذا قمت بتحويل الشاشة إلى UserControl وتريد عرضها داخل الـ Dashboard نفسه:
                // NavigateToView(new ManagePeopleControl());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء فتح شاشة إدارة الأشخاص:\n{ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// فتح شاشة إدارة السائقين
        /// </summary>
        private void btnDrivers_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("شاشة إدارة السائقين (Drivers)", "تحت التطوير", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region Applications & Licenses Management

        // إصدار رخصة محلية جديدة
        private void btnLocalLicense_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("شاشة إصدار رخصة جديدة محلياً", "تحت التطوير", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // إصدار رخصة دولية
        private void btnInternationalLicense_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("شاشة إصدار رخصة دولية", "تحت التطوير", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // تجديد الرخصة
        private void btnRenewLicense_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("شاشة تجديد الرخصة", "تحت التطوير", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // بدل فاقد / تالف
        private void btnReplacementLicense_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("شاشة بدل فاقد/تالف", "تحت التطوير", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // إعادة الاختبار
        private void btnRetakeTest_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("شاشة إعادة الاختبار", "تحت التطوير", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // إدارة الطلبات المحلية
        private void btnManageLocalApps_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("شاشة إدارة الطلبات المحلية", "تحت التطوير", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // إدارة الطلبات الدولية
        private void btnManageIntApps_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("شاشة إدارة الطلبات الدولية", "تحت التطوير", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region Detained Licenses Management

        // حجز رخصة
        private void btnDetainLicense_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("شاشة حجز رخصة", "تحت التطوير", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // فك حجز رخصة
        private void btnReleaseDetained_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("شاشة فك حجز رخصة", "تحت التطوير", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnReleaseDetainedLicense_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("شاشة فك الحجز", "تحت التطوير", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // إدارة الرخص المحجوزة
        private void btnManageDetained_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("شاشة إدارة الرخص المحجوزة", "تحت التطوير", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region Application Settings & Types

        // إدارة أنواع الطلبات
        private void btnManageAppTypes_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("شاشة إدارة أنواع الطلبات", "تحت التطوير", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // إدارة أنواع الاختبارات
        private void btnManageTestTypes_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("شاشة إدارة أنواع الاختبارات", "تحت التطوير", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region Users & Account Settings

        // إدارة المستخدمين
        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("شاشة إدارة المستخدمين (Users)", "تحت التطوير", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // بيانات المستخدم الحالي
        private void btnCurrentUserInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("بيانات المستخدم الحالي", "معلومات", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // تغيير كلمة السر
        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("شاشة تغيير كلمة السر", "تغيير كلمة السر", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region Authentication & System Actions

        /// <summary>
        /// تسجيل الخروج والعودة لشاشة تسجيل الدخول
        /// </summary>
        private void btnSignOut_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "هل أنت تأكد من تسجيل الخروج؟",
                "تأكيد الخروج",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // إغلاق النافذة الحالية
                this.Close();
            }
        }

        #endregion

        // اعتراض رسائل النظام لمنع النافذة من التصغير/الاستعادة عند سحبها وهي ماكسيميزد
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                hwndSource.AddHook(HwndHook);
            }
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOVE = 0xF010;

            if (msg == WM_SYSCOMMAND)
            {
                int command = wParam.ToInt32() & 0xFFF0;
                // إذا حاول المستخدم سحب النافذة وهي في وضع التكبير، نمنع حدوث الاستعادة والتصغير
                if (command == SC_MOVE && this.WindowState == WindowState.Maximized)
                {
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }
    }
}