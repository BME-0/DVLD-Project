using BassemCore.Validation;
using DVLD_BusinessLayer; // استدعاء طبقة الأعمال للتعامل مع قاعدة البيانات
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DVLD_ClientTier
{
    /// <summary>
    /// Interaction logic for UC_ChangePassword.xaml
    /// </summary>
    public partial class UC_ChangePassword : UserControl
    {
        private int _userID;

        // المُنشئ (Constructor) الافتراضي
        public UC_ChangePassword()
        {
            InitializeComponent();

            // جلب معرف المستخدم الحالي تلقائياً من الكلاس العام إن وجد
            if (ClsGlobal.CurrentUser != null)
            {
                _userID = ClsGlobal.CurrentUser.UserID;
            }
        }

        // مُنشئ بديل في حال أردت تمرير الـ UserID مباشرة
        public UC_ChangePassword(int userID)
        {
            InitializeComponent();
            _userID = userID;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // جلب القيم بالطريقة الصحيحة بغض النظر عما إذا كان الحقل في وضع الإخفاء أو الإظهار
            string currentPasswordValue = GetPassword(txtCurrentPassword, txtCurrentPasswordVisible);
            string newPasswordValue = GetPassword(txtNewPassword, txtNewPasswordVisible);
            string confirmPasswordValue = GetPassword(txtConfirmPassword, txtConfirmPasswordVisible);

            // 1. التحقق من تعبئة جميع الحقول
            if (string.IsNullOrWhiteSpace(currentPasswordValue) ||
                string.IsNullOrWhiteSpace(newPasswordValue) ||
                string.IsNullOrWhiteSpace(confirmPasswordValue))
            {
                MessageBox.Show("Please fill in all password fields.", "Missing Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. التحقق من تطابق كلمة السر الجديدة مع تأكيد كلمة السر
            if (newPasswordValue != confirmPasswordValue)
            {
                MessageBox.Show("New password and confirmation password do not match!", "Mismatch Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 3. التحقق من صحة كلمة السر الحالية للمستخدم
            bool isCurrentPasswordCorrect = ClsUser.IsPasswordCorrect(_userID, currentPasswordValue);

            if (!isCurrentPasswordCorrect)
            {
                MessageBox.Show("Current password is incorrect!", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Error);
                txtCurrentPassword.Focus();
                return;
            }

            // 4. جلب كائن المستخدم وتحديث كلمة المرور
            ClsUser user = ClsUser.Find(_userID);
            if (user != null)
            {
                user.Password = newPasswordValue;

                // حفظ التعديلات في قاعدة البيانات
                if (user.Save())
                {
                    MessageBox.Show("Password updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    // تفريغ الحقول بعد النجاح
                    txtCurrentPassword.Clear();
                    txtCurrentPasswordVisible.Clear();
                    txtNewPassword.Clear();
                    txtNewPasswordVisible.Clear();
                    txtConfirmPassword.Clear();
                    txtConfirmPasswordVisible.Clear();
                }
                else
                {
                    MessageBox.Show("Failed to update password in database.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Could not find the current user data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // أزرار إظهار وإخفاء كلمة المرور
        private void btnToggleCurrent_Click(object sender, RoutedEventArgs e)
        {
            TogglePasswordVisibility(txtCurrentPassword, txtCurrentPasswordVisible, btnToggleCurrent);
        }

        private void btnToggleNew_Click(object sender, RoutedEventArgs e)
        {
            TogglePasswordVisibility(txtNewPassword, txtNewPasswordVisible, btnToggleNew);
        }

        private void btnToggleConfirm_Click(object sender, RoutedEventArgs e)
        {
            TogglePasswordVisibility(txtConfirmPassword, txtConfirmPasswordVisible, btnToggleConfirm);
        }

        // دالة مساعدة لتبديل حالة العرض بين PasswordBox و TextBox
        private void TogglePasswordVisibility(PasswordBox passBox, TextBox textBox, Button toggleBtn)
        {
            if (passBox.Visibility == Visibility.Visible)
            {
                textBox.Text = passBox.Password; // نقل النص المخفي إلى النص الظاهر
                passBox.Visibility = Visibility.Collapsed;
                textBox.Visibility = Visibility.Visible;
                toggleBtn.Content = "👁️‍🗨️"; // تغيير شكل الأيقونة للإشارة إلى أن الباسورد مكشوف
            }
            else
            {
                passBox.Password = textBox.Text; // نقل النص الظاهر إلى الـ PasswordBox
                textBox.Visibility = Visibility.Collapsed;
                passBox.Visibility = Visibility.Visible;
                toggleBtn.Content = "👁️"; // إعادة شكل الأيقونة الأصلية
            }
        }

        // دالة مساعدة للحصول على القيمة الصحيحة (سواء كانت مكتوبة في PasswordBox أو TextBox)
        private string GetPassword(PasswordBox passBox, TextBox textBox)
        {
            return passBox.Visibility == Visibility.Visible ? passBox.Password : textBox.Text;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ClsInputValidation.ValidateTextInput(e, ClsInputValidation.eValidationType.AlphaNumeric);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        // دالة عامة يمكنك استدعاؤها من أي فورم لإجراء عملية تغيير كلمة المرور برمجياً
        public bool ChangePassword(string currentPassword, string newPassword)
        {
            ClsUser user = ClsUser.Find(_userID);
            if (user != null)
            {
                // التحقق من صحة كلمة المرور الحالية
                if (!ClsUser.IsPasswordCorrect(_userID, currentPassword))
                {
                    return false; // كلمة المرور الحالية غير صحيحة
                }

                user.Password = newPassword;

                // الحفظ
                return user.Save();
            }
            return false; // المستخدم غير موجود
        }
    }
}