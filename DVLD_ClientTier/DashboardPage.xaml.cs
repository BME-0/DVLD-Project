using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DVLD_BusinessLayer;

namespace DVLD_ClientTier
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        protected MainWindow MainWin => Window.GetWindow(this) as MainWindow;

        public DashboardPage()
        {
            InitializeComponent();
            LoadPeopleDashBoard();
        }

        private void btnManagePeople_Click(object sender, RoutedEventArgs e)
        {
            MainWin?.MainFrame.Navigate(new ManagePeoplePage());

            //MessageBox.Show("📌 اختصار سريع لـ: [ Manage People ]", "معاينة الاختصار", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnDrivers_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("📌 اختصار سريع لـ: [ Drivers ]", "معاينة الاختصار", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            MainWin?.MainFrame.Navigate(new ManageUserPage());

            //MessageBox.Show("📌 اختصار سريع لـ: [ Users ]", "معاينة الاختصار", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnLocalLicense_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("📌 اختصار سريع لـ: [ Local Licenses ]", "معاينة الاختصار", MessageBoxButton.OK, MessageBoxImage.Information);

            frmDrivingLicenseApplicationInfo applicationInfo = new frmDrivingLicenseApplicationInfo(1);

            applicationInfo.ShowDialog();
        }

        private void btnRetakeTest_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("📌 اختصار سريع لـ: [ Retake Test / Applications ]", "معاينة الاختصار", MessageBoxButton.OK, MessageBoxImage.Information);
        }

       


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


            LoadPeopleDashBoard();
        }


    }
}