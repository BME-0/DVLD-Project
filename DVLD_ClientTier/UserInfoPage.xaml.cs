using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using DVLD_BusinessLayer;

namespace DVLD_ClientTier
{
    public partial class UserInfoPage : Page
    {
        private int _UserID = -1;
        private ClsUser _User = null; 

        // 1. المُنشئ الأول (يستقبل الـ UserID مباشرة عند فتح الصفحة)
        public UserInfoPage(int userID)
        {
            InitializeComponent();
            _UserID = userID;
            LoadUserInfo(_UserID);
        }

        // 2. مُنشئ افتراضي فارغ (إذا احتجت لإنشاء الكائن واستدعاء الدالة لاحقاً)
        public UserInfoPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ميثود أساسية لجلب بيانات المستخدم وتعبئتها في العناصر
        /// </summary>
        private void LoadUserInfo(int userID)
        {
            _UserID = userID;

            // === [ملاحظة: قم بإلغاء التعليق وتعديل الكود ليطابق كلاس الـ Business الخاص بك] ===
            
            _User = ClsUser.Find(_UserID);

            if (_User == null)
            {
                _ResetUserInfo();
                MessageBox.Show("No User with UserID = " + userID, "User Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // تعبئة بيانات تسجيل الدخول (Login Info)
            lblUserID.Text = _User.UserID.ToString();
            lblUserName.Text = _User.UserName;
            lblIsActive.Text = _User.IsActive ? "Active"  : "Not Active";

            // تعبئة البيانات الشخصية (Personal Info) (بافتراض أن User يرث أو يحتوي على بيانات Person)
            lblPersonID.Text = _User.PersonID.ToString();
            lblFirstName.Text = _User.PersonInfo.FirstName;
            lblSecondName.Text = _User.PersonInfo.SecondName;
            lblThirdName.Text = string.IsNullOrEmpty(_User.PersonInfo.ThirdName) ? "—" : _User.PersonInfo.ThirdName;
            lblLastName.Text = _User.PersonInfo.LastName;
            lblNationalNo.Text = _User.PersonInfo.NationalNo;
            lblDateOfBirthAndAge.Text = _User.PersonInfo.DateOfBirth.ToShortDateString(); // يمكنك دمج العمر هنا إذا أردت
            lblPhone1.Text = _User.PersonInfo.Phone1;
            lblPhone2.Text = string.IsNullOrEmpty(_User.PersonInfo.Phone2) ? "—" : _User.PersonInfo.Phone2;
            lblEmail.Text = string.IsNullOrEmpty(_User.PersonInfo.Email) ? "—" : _User.PersonInfo.Email;
            lblGender.Text = _User.PersonInfo.GenderName;
            lblCountryName.Text = _User.PersonInfo?.CountryName ?? "N/A";
            lblAddress.Text = _User.PersonInfo.Address;

            if (_User.IsActive)
            {
                lblIsActive.Foreground = Brushes.Green;
            }
            else
            {
                lblIsActive.Foreground = Brushes.Red;
            }

        }

        /// <summary>
        /// إعادة تعيين الحقول للقيم الافتراضية في حال عدم وجود بيانات
        /// </summary>
        private void _ResetUserInfo()
        {
            lblUserID.Text = "[????]";
            lblUserName.Text = "[????]";
            lblIsActive.Text = "[????]";

            lblPersonID.Text = "N/A";
            lblFirstName.Text = "N/A";
            lblSecondName.Text = "N/A";
            lblThirdName.Text = "—";
            lblLastName.Text = "N/A";
            lblNationalNo.Text = "N/A";
            lblDateOfBirthAndAge.Text = "N/A";
            lblPhone1.Text = "N/A";
            lblPhone2.Text = "—";
            lblEmail.Text = "—";
            lblGender.Text = "N/A";
            lblCountryName.Text = "N/A";
            lblAddress.Text = "N/A";
        }
    }
}