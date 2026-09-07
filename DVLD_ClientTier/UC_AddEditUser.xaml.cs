using DVLD_BusinessLayer;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DVLD_ClientTier
{
    public partial class UC_AddEditUser : UserControl
    {
        // تعريف الحالات الخاصة بالشاشة (إضافة أو تعديل)
        public enum enMode { AddNew = 0, Update = 1 };
        private enMode _Mode;

        private int _UserID = -1;
        private ClsUser _User;

        // المُنشئ الافتراضي لحالة الإضافة (AddNew)
        public UC_AddEditUser()
        {
            InitializeComponent();
            _Mode = enMode.AddNew;
        }

        // المُنشئ المخصص لحالة التعديل (Update)
        public UC_AddEditUser(int userID)
        {
            InitializeComponent();
            _UserID = userID;
            _Mode = enMode.Update;
        }

        // حدث التحميل (Load) لتجهيز الشاشة بناءً على وضعها
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _ResetDefaultValues();

            if (_Mode == enMode.Update)
            {
                _LoadData();
            }
        }

        // إرجاع القيم الافتراضية
        private void _ResetDefaultValues()
        {
            if (_Mode == enMode.AddNew)
            {
                lblTitle.Text = "Add New User";
                this.Dispatcher.InvokeAsync(() => {
                    _User = new ClsUser();
                });

                // إخفاء حقل كلمة المرور الحالية وتسمية الحقل العادي بـ Password في حالة الإضافة
                pnlCurrentPassword.Visibility = Visibility.Collapsed;
                lblPasswordTitle.Text = "Password: ";
                txtUserName.IsEnabled = true;
            }
            else
            {
                lblTitle.Text = "Edit User Info";
                // إظهار حقل كلمة المرور الحالية وتسمية الحقل بـ New Password في حالة التعديل
                pnlCurrentPassword.Visibility = Visibility.Visible;
                lblPasswordTitle.Text = "New Password: ";
                txtUserName.IsEnabled = false; // قفل اسم المستخدم تماماً في التعديل
            }

            txtUserName.Clear();
            txtCurrentPassword.Clear();
            txtVisibleCurrentPassword.Clear();
            txtPassword.Clear();
            txtVisiblePassword.Clear();
            txtConfirmPassword.Clear();
            txtVisibleConfirmPassword.Clear();
            chkIsActive.IsChecked = true;
        }

        // تحميل بيانات المستخدم القديمة في حالة التعديل
        private void _LoadData()
        {
            _User = ClsUser.Find(_UserID);

            if (_User == null)
            {
                MessageBox.Show($"No User with ID = {_UserID} was found!", "User Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // تعبئة البيانات
            lblUserID.Text = _User.UserID.ToString();
            txtUserName.Text = _User.UserName;
            chkIsActive.IsChecked = _User.IsActive;

            // تحميل بيانات الشخص في كارت البحث وتعطيل البحث لعدم إمكانية تغيير الشخص المرتبط بمستخدم قديم
            if (_User.PersonID != -1)
            {
                ctrlPersonCardWithFilter.LoadPersonInfo(_User.PersonID);
                ctrlPersonCardWithFilter.IsEnabled = false; // منع تغيير الشخص للمستخدم الحالي
            }
        }

        // زر الانتقال من تبويب البيانات الشخصية إلى تبويب بيانات الدخول (Next)
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (_Mode == enMode.Update)
            {
                // في حالة التعديل، الانتقال متاح مباشرة
                tcUserInfo.SelectedIndex = 1;
                return;
            }

            // في حالة الإضافة الجديدة، يجب التأكد من اختيار شخص أولاً
            if (ctrlPersonCardWithFilter.PersonID != -1)
            {
                // التحقق عما إذا كان هذا الشخص له حساب مستخدم مسبقاً أم لا
                if (ClsUser.IsUserExistForPersonID(ctrlPersonCardWithFilter.PersonID))
                {
                    MessageBox.Show("Selected Person already has a user account, choose another one.", "Select another Person", MessageBoxButton.OK, MessageBoxImage.Error);
                    ctrlPersonCardWithFilter.FilterFocus();
                }
                else
                {
                    tcUserInfo.SelectedIndex = 1; // الانتقال للتبويب التالي
                }
            }
            else
            {
                MessageBox.Show("Please select a Person first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ctrlPersonCardWithFilter.FilterFocus();
            }
        }

        // زر الرجوع للتبويب السابق (Back)
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            tcUserInfo.SelectedIndex = 0;
        }

        private void txtUserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string enteredUserName = txtUserName.Text.Trim();

            // إذا كان الحقل فارغاً، نعيد اللون الطبيعي ونخرج
            if (string.IsNullOrEmpty(enteredUserName))
            {
                txtUserName.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CBD5E1")); // اللون العادي
                return;
            }

            // في حالة التعديل (Update)، إذا كان اسم المستخدم الجديد هو نفسه اسمه القديم، فهذا مسموح وعادي جداً
            if (_Mode == enMode.Update && _User != null && _User.UserName.Equals(enteredUserName, StringComparison.OrdinalIgnoreCase))
            {
                txtUserName.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3182CE"));
                txtUserName.ToolTip = null;
                return;
            }

            // التحقق هل اسم المستخدم موجود مسبقاً في قاعدة البيانات؟
            if (ClsUser.IsUserExist(enteredUserName))
            {
                // تغيير لون الإطار إلى الأحمر للتنبيه
                txtUserName.BorderBrush = Brushes.Red;
                txtUserName.ToolTip = "اسم المستخدم هذا موجود مسبقاً، الرجاء اختيار اسم آخر.";
            }
            else
            {
                // إرجاع اللون الطبيعي إذا كان الاسم متاحاً
                txtUserName.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3182CE"));
                txtUserName.ToolTip = null;
            }
        }

        // دالة موحدة للتحقق من الأخطاء وتلوين الحقل بالاعتماد على الـ PasswordBox أو الـ TextBox الظاهر
        private void _ValidatePassword(PasswordBox passwordBox, TextBox textBox, string errorMessage)
        {
            passwordBox.BorderBrush = Brushes.Red;
            textBox.BorderBrush = Brushes.Red;
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            if (passwordBox.Visibility == Visibility.Visible)
                passwordBox.Focus();
            else
                textBox.Focus();
        }

        // دالة مساعدة لتبديل رؤية الباسورد
        private void TogglePasswordVisibility(PasswordBox passwordBox, TextBox textBox, Button button)
        {
            var textBlock = button.Content as TextBlock;

            if (passwordBox.Visibility == Visibility.Visible)
            {
                // إظهار النص العادي وإخفاء الـ PasswordBox
                textBox.Text = passwordBox.Password;
                passwordBox.Visibility = Visibility.Collapsed;
                textBox.Visibility = Visibility.Visible;
                textBox.Focus();
                textBox.CaretIndex = textBox.Text.Length;
                if (textBlock != null) textBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3182CE"));
            }
            else
            {
                // إرجاع الوضع المشفر (PasswordBox)
                passwordBox.Password = textBox.Text;
                textBox.Visibility = Visibility.Collapsed;
                passwordBox.Visibility = Visibility.Visible;
                passwordBox.Focus();
                if (textBlock != null) textBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#718096"));
            }
        }

        private void btnToggleCurrentPassword_Click(object sender, RoutedEventArgs e)
        {
            TogglePasswordVisibility(txtCurrentPassword, txtVisibleCurrentPassword, btnToggleCurrentPassword);
        }

        private void btnTogglePassword_Click(object sender, RoutedEventArgs e)
        {
            TogglePasswordVisibility(txtPassword, txtVisiblePassword, btnTogglePassword);
        }

        private void btnToggleConfirmPassword_Click(object sender, RoutedEventArgs e)
        {
            TogglePasswordVisibility(txtConfirmPassword, txtVisibleConfirmPassword, btnToggleConfirmPassword);
        }

        // دالة مساعدة للحصول على قيمة الحقل سواء كان مكتوباً في الـ PasswordBox أو الـ TextBox الظاهر
        private string GetPasswordValue(PasswordBox passwordBox, TextBox textBox)
        {
            return passwordBox.Visibility == Visibility.Visible ? passwordBox.Password : textBox.Text;
        }

        // زر الحفظ (Save)
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string enteredUserName = txtUserName.Text.Trim();

            string currentPasswordValue = GetPasswordValue(txtCurrentPassword, txtVisibleCurrentPassword).Trim();
            string passwordValue = GetPasswordValue(txtPassword, txtVisiblePassword).Trim();
            string confirmPasswordValue = GetPasswordValue(txtConfirmPassword, txtVisibleConfirmPassword).Trim();

            // 1. التحقق من الحقول الإجبارية حسب المود
            if (_Mode == enMode.AddNew)
            {
                if (string.IsNullOrWhiteSpace(enteredUserName) ||
                    string.IsNullOrWhiteSpace(passwordValue) ||
                    string.IsNullOrWhiteSpace(confirmPasswordValue))
                {
                    MessageBox.Show("Please fill in all required fields.", "Missing Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // التحقق من تكرار اسم المستخدم عند الإضافة
                if (ClsUser.IsUserExist(enteredUserName))
                {
                    MessageBox.Show("اسم المستخدم مستخدم بالفعل، يرجى اختيار اسم آخر.", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtUserName.BorderBrush = Brushes.Red;
                    txtUserName.Focus();
                    return;
                }
            }
            else // Update
            {
                // في حالة التعديل، إذا أراد تغيير كلمة المرور
                if (!string.IsNullOrEmpty(passwordValue) || !string.IsNullOrEmpty(currentPasswordValue) || !string.IsNullOrEmpty(confirmPasswordValue))
                {
                    if (string.IsNullOrWhiteSpace(currentPasswordValue))
                    {
                        _ValidatePassword(txtCurrentPassword, txtVisibleCurrentPassword, "Please enter your current password.");
                        return;
                    }

                    // التحقق من صحة كلمة المرور الحالية
                    if (_User.Password != ClsEncryption.ComputeHash(currentPasswordValue))
                    {
                        _ValidatePassword(txtCurrentPassword, txtVisibleCurrentPassword, "Current password is incorrect.");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(passwordValue))
                    {
                        _ValidatePassword(txtPassword, txtVisiblePassword, "Please enter the new password.");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(confirmPasswordValue))
                    {
                        _ValidatePassword(txtConfirmPassword, txtVisibleConfirmPassword, "Please confirm the new password.");
                        return;
                    }
                }
            }

            // 2. التحقق من تطابق كلمة المرور الجديدة في حال كتابتها
            if (!string.IsNullOrEmpty(passwordValue))
            {
                if (passwordValue != confirmPasswordValue)
                {
                    _ValidatePassword(txtConfirmPassword, txtVisibleConfirmPassword, "Password and confirmation password do not match.");
                    return;
                }
            }

            // إرجاع الحدود للوضع الطبيعي في حال اجتياز التحقق بنجاح
            Brush defaultBorder = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CBD5E1"));
            txtCurrentPassword.BorderBrush = defaultBorder;
            txtVisibleCurrentPassword.BorderBrush = defaultBorder;
            txtPassword.BorderBrush = defaultBorder;
            txtVisiblePassword.BorderBrush = defaultBorder;
            txtConfirmPassword.BorderBrush = defaultBorder;
            txtVisibleConfirmPassword.BorderBrush = defaultBorder;

            // 3. تعبئة خصائص الكائن
            _User.PersonID = ctrlPersonCardWithFilter.PersonID;
            _User.UserName = enteredUserName;

            // إذا كتب كلمة مرور جديدة نعينها، وإلا نبقي القديمة كما هي في وضع التعديل
            if (!string.IsNullOrEmpty(passwordValue))
            {
                _User.Password = passwordValue;
            }

            _User.IsActive = chkIsActive.IsChecked ?? true;

            // 4. الحفظ في قاعدة البيانات
            if (_User.Save())
            {
                MessageBox.Show("Data Saved Successfully.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);

                // تغيير الواجهة إلى وضع التعديل وتحديث الـ ID المعروض
                _Mode = enMode.Update;
                lblTitle.Text = "Edit User Info";
                lblUserID.Text = _User.UserID.ToString();
                pnlCurrentPassword.Visibility = Visibility.Visible;
                lblPasswordTitle.Text = "New Password: ";
                txtUserName.IsEnabled = false;



                txtCurrentPassword.Clear();
                txtVisibleCurrentPassword.Clear();
                txtPassword.Clear();
                txtVisiblePassword.Clear();
                txtConfirmPassword.Clear();
                txtVisibleConfirmPassword.Clear();
            }
            else
            {
                MessageBox.Show("Error: Data Is Not Saved Successfully.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}