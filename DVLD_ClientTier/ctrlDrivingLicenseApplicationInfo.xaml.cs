using System.Windows;
using System.Windows.Controls;
using DVLD_BusinessLayer;

namespace DVLD_ClientTier
{
    public partial class ctrlDrivingLicenseApplicationInfo : UserControl
    {
        private ClsLocalDrivingLicenseApplication _localDrivingLicenseApplication;
        private int _localDrivingLicenseApplicationID = -1;

        public int LocalDrivingLicenseApplicationID
        {
            get { return _localDrivingLicenseApplicationID; }
        }

        public ctrlDrivingLicenseApplicationInfo()
        {
            InitializeComponent();
        }

        public void LoadInfo(int localDrivingLicenseApplicationID)
        {
            _localDrivingLicenseApplicationID = localDrivingLicenseApplicationID;
            _localDrivingLicenseApplication = ClsLocalDrivingLicenseApplication.Find(_localDrivingLicenseApplicationID);

            if (_localDrivingLicenseApplication == null)
            {
                ResetApplicationInfo();
                MessageBox.Show("No Application with ID = " + _localDrivingLicenseApplicationID, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _FillApplicationInfo();
        }

        private void _FillApplicationInfo()
        {
            txtLDLAppID.Text = _localDrivingLicenseApplication.LocalDrivingLicenseApplicationID.ToString();
            txtApplicationID.Text = _localDrivingLicenseApplication.ApplicationID.ToString();

            // يمكنك ربط اسم فئة الرخصة حسب الـ LicenseClassID (مثلاً Class 1 - Small Motorcycle إلخ)
             //txtLicenseClass.Text = ClsLicenseClass.Find(_localDrivingLicenseApplication.LicenseClassID).ClassName;
            txtLicenseClass.Text = _localDrivingLicenseApplication.LicenseClassID.ToString(); // مؤقتاً لحين ربط جدول الفئات

            txtPassedTests.Text = _localDrivingLicenseApplication.GetPassedTestCount().ToString();
            txtFees.Text = _localDrivingLicenseApplication.PaidFees.ToString("C2"); // تنسيق العملة
            txtStatus.Text = _localDrivingLicenseApplication.ApplicationStatus.ToString(); // يمكن تحويلها لنص مثل New, Cancelled, Completed

            // جلب اسم الشخص المتقدم بناءً على ApplicantPersonID من كلاس الـ Applications العام
            ClsPerson person = ClsPerson.Find(_localDrivingLicenseApplication.ApplicantPersonID);
            txtApplicantName.Text = person != null ? person.FullName : "[Unknown]";
            //txtApplicantName.Text = "Person ID: " + _localDrivingLicenseApplication.ApplicantPersonID; // مؤقتاً
        }

        public void ResetApplicationInfo()
        {
            _localDrivingLicenseApplicationID = -1;
            txtLDLAppID.Text = "[????]";
            txtLicenseClass.Text = "[????]";
            txtPassedTests.Text = "0";
            txtApplicationID.Text = "[????]";
            txtStatus.Text = "[????]";
            txtFees.Text = "[????]";
            txtApplicantName.Text = "[????]";
        }
    }
}