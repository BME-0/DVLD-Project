using System.Windows;
using System.Windows.Controls;
using DVLD_BusinessLayer;

namespace DVLD_ClientTier
{
    public partial class UC_ShowUserWithDetails : UserControl
    {
        public UC_ShowUserWithDetails()
        {
            InitializeComponent();
        }

        public UC_ShowUserWithDetails(int userId)
        {
            InitializeComponent();

            LoadUserInfo(userId);
        }

        // دالة مركزية لتمكين أي نافذة أو Dashboard من تمرير الـ UserID وعرض بياناته
        public void LoadUserInfo(int userID)
        {
            // 1. جلب كائن المستخدم من طبقة الـ Business
            ClsUser user = ClsUser.Find(userID);
            if (user == null)
            {
                MessageBox.Show("No User with UserID = " + userID, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 2.تمرير الـ PersonID لـ شاشة معلومات الشخص
            PersonCard.LoadPersonInfo(user.PersonID);

            // 3.تمرير الـ UserID لـ شاشة معلومات الحساب
            UserCard.LoadUserInfo(userID);
        }
    }
}