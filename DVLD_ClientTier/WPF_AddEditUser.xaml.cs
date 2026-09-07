using DVLD_ClientTier;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DVLD_ClientTier
{
    public partial class WPF_AddEditUser : Window
    {
        // المُنشئ الافتراضي لحالة إضافة مستخدم جديد (AddNew)
        public WPF_AddEditUser()
        {
            InitializeComponent();

            this.Title = "Add / Edit New User";
            this.Icon = new BitmapImage(new Uri(@"H:\DVLD_Project\DVLD-Project\Icons\AddNewPerson.ico", UriKind.Absolute));
            // لو الـ UserControl يدعم Constructor افتراضي، يمكنك تمريره أو تهيئته داخله
        }

        // المُنشئ المخصص لحالة تعديل مستخدم موجود (Update) بناءً على الـ UserID
        public WPF_AddEditUser(int userID)
        {
            InitializeComponent();

            this.Title = "Add / Edit New User";
            this.Icon = new BitmapImage(new Uri(@"H:\DVLD_Project\DVLD-Project\Icons\EditPerson.ico", UriKind.Absolute));

            // استبدال محتوى الـ Grid بالـ UserControl المهيأ بوضع التعديل
            // أو إذا أردت تمرير الـ ID مباشرة للـ UserControl الموجود في الـ XAML:
            // (يمكنك تعديل UserControl الخاص بك ليقبل UserID عبر Constructor أو دالة Load)

            Content = new UC_AddEditUser(userID);
        }

        private void ctrlAddEditUser_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}