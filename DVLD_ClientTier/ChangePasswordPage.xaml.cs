using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DVLD_BusinessLayer;

namespace DVLD_ClientTier
{
    /// <summary>
    /// Interaction logic for ChangePasswordPage.xaml
    /// </summary>
    public partial class ChangePasswordPage : Page
    {
        private int _userID;

        // المُنشئ (Constructor) الافتراضي
        public ChangePasswordPage()
        {
            InitializeComponent();

            // جلب معرف المستخدم الحالي تلقائياً من الكلاس العام إن وجد
            
            if (ClsGlobal.CurrentUser != null)
            {
                _userID = ClsGlobal.CurrentUser.UserID;
                LoadUserInfo();
            }
            
        }

        // مُنشئ بديل في حال أردت تمرير الـ UserID مباشرة عند فتح الصفحة
        public ChangePasswordPage(int userID)
        {
            InitializeComponent();
            _userID = userID;
            LoadUserInfo();
        }

        // دالة لجلب معلومات المستخدم وعرضها في الـ Labels المخصصة داخل الكارد
        private void LoadUserInfo()
        {
            
            ClsUser user = ClsUser.Find(_userID);
            if (user != null)
            {
                lblUserID.Text = user.UserID.ToString();
                lblUserName.Text = user.UserName;
            }
            
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
            BassemCore.Validation.ClsInputValidation.ValidateTextInput(e, BassemCore.Validation.ClsInputValidation.eValidationType.AlphaNumeric);
        }

        // دالة عامة يمكنك استدعاؤها إجراء عملية تغيير كلمة المرور برمجياً عند الحاجة
        public bool ChangePassword(string currentPassword, string newPassword)
        {
            
            ClsUser user = ClsUser.Find(_userID);
            if (user != null)
            {
                if (!ClsUser.IsPasswordCorrect(_userID, currentPassword))
                {
                    return false; 
                }

                user.Password = newPassword;
                return user.Save();
            }
            
            return false;
        }
    }
}