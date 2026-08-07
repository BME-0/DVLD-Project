using DVLD; // Business Layer Reference
using DVLD.Classes;
using DVLD_BusinessLayer;
using DVLD_PresentationLayer.Users;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

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

            LoadDashboardStats();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            LoadDashboardStats();
        }

        #region User Info Helpers

        /// <summary>
        /// تحميل بيانات المستخدم الحالي وعرض اسمه في شريط الترحيب
        /// </summary>
        private void LoadUserInfo()
        {
            if (clsGlobal.CurrentUser != null)
            {
                // يمكنك استخدام UserName أو FullName حسب تصميم الكلاس لديك في مشروع الـ DVLD
                txtWelcomeUser.Text = $"👋 Welcome Back, {clsGlobal.CurrentUser.UserName}";

                // 2. عرض اليوم والتاريخ الحالي بالصيغة الإنجليزية (مثال: Friday, August 07, 2026)
                txtHeaderDate.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
                txtStatusDate.Text = txtHeaderDate.Text;

                txtTopUserName.Text = clsGlobal.CurrentUser.PersonInfo.FullName;
                txtTopUserRole.Text = (clsGlobal.CurrentUser.UserName.ToLower() == "admin") ? "System Admin" : "Standard User";
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
                var person = clsGlobal.CurrentUser?.PersonInfo;
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

        private void LoadPeopleDashBoard()
        {
            try
            {
                // 1. جلب وعرض العدد الإجمالي مع تنسيق الأرقام (مثل 1,250)
                int totalPeople = ClsPerson.GetTotalPeopleCount();
                lblTotalPeopleCard.Text = totalPeople.ToString("N0");

                // 2. جلب وعرض مسجلي اليوم وتلوين المؤشر بناءً على القيمة
                int addedToday = ClsPerson.GetPeopleCountToday();

                if (addedToday > 0)
                {
                    lblTodayPeopleCard.Text = $"+{addedToday} Today";
                    lblTodayPeopleCard.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")); // أخضر للنشاط
                }
                else
                {
                    lblTodayPeopleCard.Text = "0 Today";
                    lblTodayPeopleCard.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#64748B")); // رمادي هادئ
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading statistics: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void LoadDashboardStats()
        {
            LoadUserInfo();


            LoadPeopleDashBoard();
        }

        // دالة يتم استدعاؤها فور نجاح تسجيل الدخول
        private void LoginControl_OnLoginSuccess(object sender, EventArgs e)
        {
            // إعادة القائمة الجانبية لحجمها الطبيعي وتفعيلها
            SidebarColumn.Width = new GridLength(250);
            btnToggleSidebar.IsEnabled = true;

            // العودة للوحة التحكم الرئيسية (Dashboard)
            ShowDashboardHome();
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
                    // ============================================================================
                    // تعديل هائم لجعل عنصر الـ UserControl يمتد ويمتلئ بالكامل داخل المساحة المتاحة
                    // ============================================================================
                    userControl.HorizontalAlignment = HorizontalAlignment.Stretch;
                    userControl.VerticalAlignment = VerticalAlignment.Stretch;
                    userControl.Width = double.NaN;  // إلغاء أي عرض ثابت مسجل برمجيًا
                    userControl.Height = double.NaN; // إلغاء أي ارتفاع ثابت مسجل برمجيًا
                    // ============================================================================

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
                ManagePeopleWindow managePeople = new ManagePeopleWindow();
                managePeople.Owner = this;
                managePeople.ShowDialog();
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
            try
            {
                Wpf_PersonWithCard personCardWindow = new Wpf_PersonWithCard();
                personCardWindow.Owner = this;
                personCardWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء فتح شاشة إدارة السائقين:\n{ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            //MessageBox.Show("شاشة إدارة المستخدمين (Users)", "تحت التطوير", MessageBoxButton.OK, MessageBoxImage.Information);

            try
            {
                Frm_ManageUsers manageUsers = new Frm_ManageUsers();
                manageUsers.Owner = this;
                manageUsers.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء فتح شاشة إدارة المستخدمين:\n{ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        // بيانات المستخدم الحالي
        private void btnCurrentUserInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. التحقق من أن هناك مستخدم مسجل الدخول بالفعل في النظام
                if (clsGlobal.CurrentUser != null)
                {
                    // 2. سحب الـ UserID الخاص بالمستخدم الحالي من الكلاس العام
                    int currentUserId = clsGlobal.CurrentUser.UserID;

                    // 3. إنشاء نسخة من الـ UserControl الخاص بعرض بيانات المستخدم وتمرير الـ ID له
                    // (تأكد من أنك قمت بإنشاء UC_ShowUserWithDetails في مشروعك)
                    UC_ShowUserWithDetails currentUserInfoUC = new UC_ShowUserWithDetails(currentUserId);

                    //WPF_ShowUserWithDetails ShowUser = new WPF_ShowUserWithDetails(currentUserId);
                    //ShowUser.ShowDialog();

                    // 4. استخدام الدالة المساعدة (الموجودة لديك بالفعل) لعرض الـ UserControl وإخفاء الداش بورد
                     NavigateToView(currentUserInfoUC);
                }
                else
                {
                    MessageBox.Show("لا يوجد مستخدم مسجل الدخول حالياً لعرض بياناته.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء فتح شاشة بيانات المستخدم:\n{ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // تغيير كلمة السر
        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("شاشة تغيير كلمة السر", "تغيير كلمة السر", MessageBoxButton.OK, MessageBoxImage.Information);

            try
            {
                // 1. التحقق من أن هناك مستخدم مسجل الدخول بالفعل في النظام
                if (clsGlobal.CurrentUser != null)
                {
                    // 2. سحب الـ UserID الخاص بالمستخدم الحالي من الكلاس العام
                    int currentUserId = clsGlobal.CurrentUser.UserID;

                    // 3. إنشاء نسخة من الـ UserControl الخاص بعرض بيانات المستخدم وتمرير الـ ID له
                    UC_ChangePassword currentUser = new UC_ChangePassword(currentUserId);


                    // 4. استخدام الدالة المساعدة (الموجودة لديك بالفعل) لعرض الـ UserControl وإخفاء الداش بورد
                     NavigateToView(currentUser);
                }
                else
                {
                    MessageBox.Show("لا يوجد مستخدم مسجل الدخول حالياً لعرض بياناته.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء فتح شاشة بيانات المستخدم:\n{ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        #endregion

        #region Authentication & System Actions

        /// <summary>
        /// تسجيل الخروج والعودة لشاشة تسجيل الدخول
        /// </summary>
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
        #endregion

        /// <summary>
        /// زر الـ X لإغلاق البرنامج بالكامل
        /// </summary>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

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