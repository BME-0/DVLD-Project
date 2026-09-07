using DVLD_ClientTier;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace DVLD_ClientTier
{
    public partial class Frm_ChangePassword : Form
    {
        private int _id = -1;

        // خاصية للوصول للـ UserControl بسهولة
        public UC_ChangePassword ChangePass => elementHost1.Child as UC_ChangePassword;

        // المُنشئ الافتراضي
        public Frm_ChangePassword()
        {
            InitializeComponent();
        }

        // مُنشئ مخصص لو أردت تمرير الـ UserID عند فتح الفورم
        public Frm_ChangePassword(int userID) : this()
        {
            _id = userID;

            // لو الـ UserControl يحتاج يستقبل الـ ID، يمكنك تمريره له هنا لو أردت
        }

        private void Element1_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }

        // دالة داخل الفورم لتنفيذ أو استدعاء تغيير الباسورد برمجياً مع إظهار رسائل النجاح والفشل
        public void ExecutePasswordChange(string currentPass, string newPass)
        {
            // التأكد أن الـ UserControl محمل وغير فارغ
            if (ChangePass != null)
            {
                bool result = ChangePass.ChangePassword(currentPass, newPass);

                if (result)
                {
                    MessageBox.Show("Password updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to update password. Please check your current password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("UserControl is not loaded yet.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}