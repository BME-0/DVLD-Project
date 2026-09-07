using System.Windows;

namespace DVLD_ClientTier
{
    public partial class frmDrivingLicenseApplicationInfo : Window
    {
        // Constructor يتقبل رقم الطلب المحلي ليقوم بتمريره للـ User Control
        public frmDrivingLicenseApplicationInfo(int localDrivingLicenseApplicationID)
        {
            InitializeComponent();

            // استدعاء دالة التحميل الموجودة داخل الـ User Control
            ctrlDrivingLicenseApplicationInfo1.LoadInfo(localDrivingLicenseApplicationID);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}