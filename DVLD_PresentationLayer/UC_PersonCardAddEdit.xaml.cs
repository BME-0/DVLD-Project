using DVLD_BusinessLayer; // تأكد من استيراد كلاسات الـ Business والـ DataAccess
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DVLD_PresentationLayer
{
    public partial class UC_PersonCardAddEdit : UserControl
    {
        // 1. تحديد وضع الكنترول (إضافة جديد = 0، تعديل الحالي = 1)
        private enum enMode { AddNew = 0, Update = 1 }
        private enMode _Mode = enMode.AddNew;

        public enum enNameLanguage { Undecided, Arabic, English }
        private enNameLanguage _NameLanguageMode = enNameLanguage.Undecided;

        // 2. المتغيرات الخاصة بالبيانات والصور
        private int _PersonID = -1;                // لحفظ الـ ID للشخص الحالي
        private ClsPerson _Person;                 // كائن الـ Business Layer
        private string _SelectedImagePath = "";    // لحفظ مسار الصورة المؤقت

        // 3. خصائص (Properties) عامة
        public int PersonID => _PersonID;
        public ClsPerson SelectedPerson => _Person;

        public UC_PersonCardAddEdit()
        {
            InitializeComponent();

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                return; // اخرج فوراً لو إحنا جوه الفيجوال استوديو ديزاينر
            }

            _FillCountriesComboBox();
            ResetPersonInfo();
        }

        private void _FillCountriesComboBox()
        {
            // حماية: التأكد من أن الـ ComboBox نفسه تم تحميله وليس null
            if (cbCountries == null) return;

            DataTable dtCountries = ClsCountry.GetAllCountries();

            // حماية: التأكد من أن الداتابيز رجعت بيانات فعلاً ولم ترجع null
            if (dtCountries != null)
            {
                cbCountries.Items.Clear(); // تصفير العناصر لمنع التكرار
                foreach (DataRow row in dtCountries.Rows)
                {
                    if (row["CountryName_En"] != null)
                    {
                        cbCountries.Items.Add(row["CountryName_En"].ToString());
                    }
                }
            }

            // تحديد دولة افتراضية آمنة
            int index = cbCountries.Items.IndexOf("Egypt");
            if (index != -1)
            {
                cbCountries.SelectedIndex = index;
            }
        }

        public void ResetPersonInfo()
        {
            _PersonID = -1;
            _Person = new ClsPerson();
            _Mode = enMode.AddNew;
            lblPersonID.Text = "????";

            // حماية: وضع علامة استفهام أو التحقق من وجود الكنترول قبل إسناد قيمة له
            if (txtFirstName != null) txtFirstName.Text = "";
            if (txtSecondName != null) txtSecondName.Text = "";
            if (txtThirdName != null) txtThirdName.Text = "";
            if (txtLastName != null) txtLastName.Text = "";
            if (txtNationalNo != null) txtNationalNo.Text = "";

            if (dtpDateOfBirth != null)
                dtpDateOfBirth.SelectedDate = DateTime.Now.AddYears(-18);

            if (txtPhone1 != null) txtPhone1.Text = "";
            if (txtPhone2 != null) txtPhone2.Text = "";
            if (txtEmail != null) txtEmail.Text = "";
            if (txtAddress != null) txtAddress.Text = "";

            if (cbCountries != null)
            {
                int index = cbCountries.Items.IndexOf("Egypt");
                if (index != -1) cbCountries.SelectedIndex = index;
            }

            if (rbMale != null) rbMale.IsChecked = true;
            if (txtNationalNo != null) txtNationalNo.IsEnabled = true;

            _SelectedImagePath = "";
            _LoadDefaultGenderIcon();
        }

        public void LoadPersonInfo(int personID)
        {
            _PersonID = personID;
            _Person = ClsPerson.Find(personID);

            if (_Person == null)
            {
                MessageBox.Show($"No Person with ID = {personID} was found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ResetPersonInfo();
                return;
            }

            _Mode = enMode.Update;

            lblPersonID.Text = _PersonID.ToString();

            // حماية: وضع البيانات في العناصر بشكل آمن
            if (txtFirstName != null) txtFirstName.Text = _Person.FirstName;
            if (txtSecondName != null) txtSecondName.Text = _Person.SecondName;
            if (txtThirdName != null) txtThirdName.Text = _Person.ThirdName;
            if (txtLastName != null) txtLastName.Text = _Person.LastName;
            if (txtNationalNo != null) txtNationalNo.Text = _Person.NationalNo;

            if (dtpDateOfBirth != null) dtpDateOfBirth.SelectedDate = _Person.DateOfBirth;

            if (txtPhone1 != null) txtPhone1.Text = _Person.Phone1;
            if (txtPhone2 != null) txtPhone2.Text = _Person.Phone2;
            if (txtEmail != null) txtEmail.Text = _Person.Email;
            if (txtAddress != null) txtAddress.Text = _Person.Address;

            if (txtNationalNo != null) txtNationalNo.IsEnabled = false;

            // جلب الدولة بشكل آمن ومحمي من الـ null
            if (cbCountries != null)
            {
                var country = ClsCountry.Find(_Person.NationalityCountryID);
                if (country != null)
                {
                    cbCountries.SelectedItem = country.CountryName_En;
                }
            }

            if (rbFemale != null && rbMale != null)
            {
                if (_Person.Gender)
                    rbFemale.IsChecked = true;
                else
                    rbMale.IsChecked = true;
            }

                  // التعامل مع الصورة الشخصية بشكل آمن داخل try-catch
            if (!string.IsNullOrEmpty(_Person.ImagePath) /*&& File.Exists(_Person.ImagePath)*/)
            {
                _SelectedImagePath = _Person.ImagePath;

                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(_SelectedImagePath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    if (imgPerson != null)
                    {
                        imgPerson.ImageSource = bitmap;
                    }
                }
                catch
                {
                    _LoadDefaultGenderIcon(); // تراجع آمن في حال فشل قراءة ملف الصورة
                }
            }
            else
            {
                _LoadDefaultGenderIcon();
            }
        }

        private void _LoadDefaultGenderIcon()
        {
            // حماية: التأكد من أن الكنترول ليس نال قبل التعامل معه
            if (imgPerson != null)
            {
                imgPerson.ImageSource = null;
            }

            if (txtGenderIcon != null && rbFemale != null)
            {
                if (rbFemale.IsChecked == true)
                    txtGenderIcon.Text = "👩";
                else
                    txtGenderIcon.Text = "🧔";
            }
        }

        private void rbGender_Checked(object sender, RoutedEventArgs e)
        {
            // 🛡️ حماية ذهبية: لو الكنترول لسه بيتبني والعناصر الأساسية نال في الذاكرة، اخرج فوراً ومتنفذش الكود
            if (txtGenderIcon == null || imgPerson == null || rbFemale == null)
                return;

            // عند تغيير الجنس، لو مفيش صورة مرفوعة، غير الأيقونة التعبيرية الافتراضية
            if (string.IsNullOrEmpty(_SelectedImagePath))
            {
                _LoadDefaultGenderIcon();
            }
        }

        private void btnSetImage_Click(object sender, RoutedEventArgs e)
        {
            // تأكد من إضافة using Microsoft.Win32; في أول الملف لاستخدام OpenFileDialog
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            // بنحدد أنواع الصور المسموح برفعها فقط
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                _SelectedImagePath = openFileDialog.FileName; // بنحتفظ بالمسار الجديد اللي اختاره

                try
                {
                    // بنعرض الصورة فوراً على الكارت بشكل آمن
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(_SelectedImagePath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    if (imgPerson != null)
                    {
                        imgPerson.ImageSource = bitmap;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message, "Image Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnRemoveImage_Click(object sender, RoutedEventArgs e)
        {
            _SelectedImagePath = "";
            _LoadDefaultGenderIcon();
        }

        private void txtNameFields_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 1. منع الأرقام
            Regex regex = new Regex(@"[\d٠-٩]");
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
                return;
            }

            // 2. التحقق من اللغة بشكل آمن
            if (_NameLanguageMode != enNameLanguage.Undecided)
            {
                bool isArabic = (e.Text[0] >= 0x0600 && e.Text[0] <= 0x06FF);
                bool isEnglish = ((e.Text[0] >= 'A' && e.Text[0] <= 'Z') || (e.Text[0] >= 'a' && e.Text[0] <= 'z'));

                if (_NameLanguageMode == enNameLanguage.Arabic && isEnglish) e.Handled = true; // لو اللغة عربي ومنعنا إنجليزي
                if (_NameLanguageMode == enNameLanguage.English && isArabic) e.Handled = true; // لو اللغة إنجليزي ومنعنا عربي
            }
        }
        // 1. دالة لتحديد اتجاه حقل التكست بناءً على أول حرف مكتوب
        private void _AdjustTextDirection(TextBox textBox)
        {
            // 🛡️ حماية: لو الـ TextBox الممرر لسه null في الذاكرة اخرج فوراً
            if (textBox == null) return;

            if (string.IsNullOrEmpty(textBox.Text))
            {
                // الوضع الافتراضي للحقل وهو فارغ (يسار)
                textBox.FlowDirection = FlowDirection.LeftToRight;
                return;
            }

            // فحص أول حرف في الـ TextBox
            char firstChar = textBox.Text[0];

            // إذا كان أول حرف يقع في نطاق الحروف العربية
            if (firstChar >= 0x0600 && firstChar <= 0x06FF)
            {
                textBox.FlowDirection = FlowDirection.RightToLeft;
            }
            else
            {
                textBox.FlowDirection = FlowDirection.LeftToRight;
            }
        }

        // 2. دالة ذكية لتغيير لغة الكيبورد تلقائياً للجهاز بالكامل
        private void _ChangeInputLanguage(DependencyObject targetElement, string cultureCode)
        {
            // 🛡️ حماية: لو العنصر المستهدف null اخرج فوراً
            if (targetElement == null) return;

            try
            {
                // الاستدعاء الاستاتيكي الصحيح بدون استخدام Current
                InputLanguageManager.SetInputLanguage(targetElement, CultureInfo.GetCultureInfo(cultureCode));
            }
            catch
            {
                // حماية في حال لم تكن اللغة مثبتة على نظام التشغيل
            }
        }

        private bool IsValidGmail(string email)
        {
            // Regex يشترط وجود أي نصوص قبل @gmail.com
            string pattern = @"^[^@\s]+@gmail\.com$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        private void ValidateEmail()
        {
            // 🛡️ حماية كبرى: لو الكنترول لسه بيتبني و txtEmail نال في الذاكرة، اخرج فوراً
            if (txtEmail == null) return;

            string email = txtEmail.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                // لو الحقل فاضي، رجع الحدود للرمادي الافتراضي وشيل التول تيب
                txtEmail.BorderBrush = System.Windows.Media.Brushes.DarkGray;
                txtEmail.ToolTip = null;
                return;
            }

            if (!IsValidGmail(email))
            {
                // 1. تغيير لون الحدود للون الأحمر
                txtEmail.BorderBrush = System.Windows.Media.Brushes.Red;

                // 2. إظهار رسالة الخطأ عند الوقوف بالماوس
                txtEmail.ToolTip = "Invalid format! Email must be a @gmail.com address.";
            }
            else
            {
                // 3. لو الإيميل سليم، رجع الحدود للون الأخضر وشيل الخطأ
                txtEmail.BorderBrush = System.Windows.Media.Brushes.Green;
                txtEmail.ToolTip = null;
            }
        }

        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateEmail();
        }
        // 3. دالة تفحص لغة الحقل الأول وتثبت لغة الكيبورد للباقي
        private void _HandleLanguageAutoSwitch(TextBox currentTextBox)
        {
            // 🛡️ حماية: التأكد من أن الـ TextBox ليس null
            if (currentTextBox == null) return;

            // نعدل اتجاه النص للحقل الحالي فوراً
            _AdjustTextDirection(currentTextBox);

            if (string.IsNullOrEmpty(currentTextBox.Text)) return;

            char firstChar = currentTextBox.Text[0];

            // لو بيكتب عربي: حول كيبورد الـ TextBox ده لعربي
            if (firstChar >= 0x0600 && firstChar <= 0x06FF)
            {
                _ChangeInputLanguage(currentTextBox, "ar-EG");
            }
            // لو بيكتب إنجليزي: حول كيبورد الـ TextBox ده لإنجليزي
            else if ((firstChar >= 'A' && firstChar <= 'Z') || (firstChar >= 'a' && firstChar <= 'z'))
            {
                _ChangeInputLanguage(currentTextBox, "en-US");
            }
        }

        // دالة مساعدة لحفظ الصورة الشخصية في مجلد المشروع الخاص بالـ DVLD
        private bool _HandlePersonImage()
        {
            if (_Person == null) return false;

            // أ) لو مفيش تغيير في مسار الصورة (نفس الصورة القديمة هي الجديدة)
            if (_Person.ImagePath == _SelectedImagePath)
                return true;

            // ب) لو المستخدم اختار إزالة الصورة الحالية تماماً (حذف)
            if (string.IsNullOrEmpty(_SelectedImagePath))
            {
                if (!string.IsNullOrEmpty(_Person.ImagePath) && File.Exists(_Person.ImagePath))
                {
                    try
                    {
                        File.Delete(_Person.ImagePath); // حذف الصورة القديمة من الهارد
                    }
                    catch { }
                }
                _Person.ImagePath = "";
                return true;
            }

            // ج) لو المستخدم اختار صورة جديدة تماماً (تغيير)
            // 🌟 هنا بنمسح الصورة القديمة أولاً عشان ما نكررش ملفات على الهارد 🌟
            if (!string.IsNullOrEmpty(_Person.ImagePath) && File.Exists(_Person.ImagePath))
            {
                try
                {
                    File.Delete(_Person.ImagePath); // حذف الملف القديم فوراً قبل رفع الجديد
                }
                catch { }
            }

            // د) نسخ الصورة الجديدة للمجلد المخصص
            string targetFolder = @"H:\Projects\DVLD\DVLD_Images\People"; // غير المسار للمكان اللي تحبه

            if (!Directory.Exists(targetFolder))
            {
                try { Directory.CreateDirectory(targetFolder); }
                catch { return false; }
            }

            string ext = Path.GetExtension(_SelectedImagePath);
            string newFileName = Guid.NewGuid().ToString() + ext;
            string targetFilePath = Path.Combine(targetFolder, newFileName);

            try
            {
                File.Copy(_SelectedImagePath, targetFilePath, true);
                _Person.ImagePath = targetFilePath; // تحديث الكائن بالمسار الجديد
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error copying image file: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        private void txtAddress_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 🛡️ حماية: لمنع الـ ArgumentOutOfRangeException لو النص المكتوب فارغاً
            if (string.IsNullOrEmpty(e.Text)) return;

            // لو اللغة لسه مش متحددة، سيب المستخدم يكتب اللي عايزه
            if (_NameLanguageMode == enNameLanguage.Undecided) return;

            // لو اللغة متحددة عربي، امنع الإنجليزي والعكس
            bool isArabic = (e.Text[0] >= 0x0600 && e.Text[0] <= 0x06FF);
            bool isEnglish = ((e.Text[0] >= 'A' && e.Text[0] <= 'Z') || (e.Text[0] >= 'a' && e.Text[0] <= 'z'));

            if (_NameLanguageMode == enNameLanguage.Arabic && isEnglish)
            {
                e.Handled = true;
                // 🛡️ تصحيح الـ ToolTip: يظهر فقط عند محاولة كتابة لغة خاطئة لتنبيه المستخدم
                txtAddress.ToolTip = "Language mismatch! You must type in Arabic.";
            }
            else if (_NameLanguageMode == enNameLanguage.English && isArabic)
            {
                e.Handled = true;
                txtAddress.ToolTip = "Language mismatch! You must type in English.";
            }
            else
            {
                // مسح التول تيب عند الكتابة الصحيحة
                txtAddress.ToolTip = null;
            }
        }

        private void txtNationalNo_LostFocus(object sender, RoutedEventArgs e)
        {
            // 🛡️ حماية: لو الـ TextBox لسبب ما غير متاح في الذاكرة
            if (txtNationalNo == null) return;

            // 1. لو بنعدل بيانات (Update)، مش محتاجين نتأكد من التكرار
            if (_Mode == enMode.Update) return;

            string nationalNo = txtNationalNo.Text.Trim();
            if (string.IsNullOrEmpty(nationalNo)) return;

            // 2. التحقق من الداتابيز
            if (ClsPerson.IsPersonExistByNationalNo(nationalNo))
            {
                // 3. لو موجود، ظهر الخطأ
                txtNationalNo.BorderBrush = System.Windows.Media.Brushes.Red;
                txtNationalNo.ToolTip = "This National Number already exists!";

                MessageBox.Show("This National Number is already registered for another person.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                // 🛡️ الأسلوب الاحترافي في WPF لإعادة التركيز للحقل وتجنب الـ UI Frozen
                Dispatcher.BeginInvoke(new Action(() => txtNationalNo.Focus()));
            }
            else
            {
                // 4. لو الرقم سليم ومش موجود
                txtNationalNo.BorderBrush = System.Windows.Media.Brushes.Green;
                txtNationalNo.ToolTip = null;
            }
        }

        private void txtAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtAddress == null) return;
            _AdjustTextDirection(txtAddress);
        }

        private void txtPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 🛡️ حماية: لمنع الـ Exception لو الـ Text فارغ
            if (string.IsNullOrEmpty(e.Text)) return;

            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void txtPhone_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (string.IsNullOrEmpty(text) || !text.All(char.IsDigit))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private bool _AreRequiredFieldsValid()
        {
            // 🛡️ حماية كبرى: نتأكد إن الـ TextBoxes مجهزة بالكامل وموجودة في الذاكرة قبل فحصها
            if (txtFirstName == null || txtSecondName == null || txtLastName == null ||
                txtNationalNo == null || txtPhone1 == null || txtAddress == null)
            {
                return false;
            }

            var requiredFields = new List<(TextBox TextBoxControl, string FieldName)>
    {
        (txtFirstName, "First Name"),
        (txtSecondName, "Second Name"),
        (txtLastName, "Last Name"),
        (txtNationalNo, "National Number"),
        (txtPhone1, "Primary Phone"),
        (txtAddress, "Address")
    };

            bool isValid = true;
            TextBox firstEmptyTextBox = null;

            foreach (var field in requiredFields)
            {
                // 🛡️ حماية إضافية لكل حقل فردي
                if (field.TextBoxControl == null) continue;

                if (string.IsNullOrWhiteSpace(field.TextBoxControl.Text))
                {
                    field.TextBoxControl.BorderBrush = System.Windows.Media.Brushes.Red;
                    field.TextBoxControl.ToolTip = $"{field.FieldName} is required!";

                    if (firstEmptyTextBox == null)
                    {
                        firstEmptyTextBox = field.TextBoxControl;
                    }
                    isValid = false;
                }
                else
                {
                    field.TextBoxControl.BorderBrush = System.Windows.Media.Brushes.DarkGray;
                    field.TextBoxControl.ToolTip = null;
                }
            }

            if (!isValid)
            {
                MessageBox.Show("Please fill in all the required fields before saving.", "Missing Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                if (firstEmptyTextBox != null)
                {
                    Dispatcher.BeginInvoke(new Action(() => firstEmptyTextBox.Focus()));
                }
            }

            return isValid;
        }
        private bool _SaveData()
        {
            // 🛡️ حماية: التأكد من أن كائن الشخص ليس null في الذاكرة
            if (_Person == null)
            {
                MessageBox.Show("System Error: Person object is not initialized.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // نقل البيانات من عناصر الـ UI إلى كائن البزنس (Business Object)
            _Person.FirstName = txtFirstName.Text.Trim();
            _Person.SecondName = txtSecondName.Text.Trim();
            _Person.ThirdName = txtThirdName.Text.Trim();
            _Person.LastName = txtLastName.Text.Trim();
            _Person.NationalNo = txtNationalNo.Text.Trim();
            _Person.DateOfBirth = dtpDateOfBirth?.SelectedDate ?? DateTime.Now; // حماية ضد الـ null
            _Person.Phone1 = txtPhone1.Text.Trim();
            _Person.Phone2 = txtPhone2.Text.Trim();
            _Person.Email = txtEmail.Text.Trim();
            _Person.Address = txtAddress.Text.Trim();
            _Person.Gender = (rbFemale?.IsChecked == true); // حماية ضد الـ null

            // ربط الـ Country ID بشكل آمن
            string selectedCountryName = cbCountries?.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedCountryName))
            {
                ClsCountry country = ClsCountry.FindByCountryName_En(selectedCountryName);
                if (country != null)
                {
                    _Person.NationalityCountryID = country.CountryID;
                }
            }

            // معالجة وحفظ الصورة في مجلد المشروع
            if (!_HandlePersonImage())
                return false;

            // محاولة الحفظ الفعلي في قاعدة البيانات
            if (_Person.Save())
            {
                _PersonID = _Person.PersonID;
                _Mode = enMode.Update; // تغيير الوضع إلى تعديل بعد نجاح الحفظ الأول
                return true;
            }

            return false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // 1. التحقق من الحقول الإجبارية أولاً بالدالة الذكية
            if (!_AreRequiredFieldsValid())
            {
                return; // إيقاف العملية لو فيه حقل ناقص
            }

            // 2. التحقق الإضافي من صحة الإيميل (لو مكتوب إيميل بس بصيغة غلط)
            if (txtEmail != null && !string.IsNullOrEmpty(txtEmail.Text.Trim()))
            {
                if (!IsValidGmail(txtEmail.Text.Trim()))
                {
                    MessageBox.Show("Please enter a valid email address format (@gmail.com).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtEmail.Focus();
                    return;
                }
            }

            // 3. استدعاء دالة الحفظ والتحقق من النتيجة لإخطار المستخدم
            if (_SaveData())
            {
                MessageBox.Show("Data Saved Successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                lblPersonID.Text = _PersonID.ToString();
                txtNationalNo.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("Failed to save data. Please check connection and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            // الطريقة الاحترافية والآمنة لإغلاق النافذة الأب (Parent Window) من داخل الـ User Control
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                parentWindow.Close();
            }
        }

        // 💡 نصيحة: الأفضل في الـ XAML ربط الـ Click بالدوال الأساسية مباشرة
        private void btnSetImage_Click_1(object sender, RoutedEventArgs e)
        {
            btnSetImage_Click(sender, e);
        }

        private void btnRemoveImage_Click_1(object sender, RoutedEventArgs e)
        {
            btnRemoveImage_Click(sender, e);
        }

        private void txtFirstName_TextChanged(object sender, TextChangedEventArgs e)
        {
            // 🛡️ حماية: لو الكنترول لسه بيتبني في الذاكرة اخرج فوراً
            if (txtFirstName == null) return;

            string firstName = txtFirstName.Text.Trim();

            // إذا كان الحقل فارغاً، نفتح القفل (نسمح بتغيير اللغة مرة أخرى)
            if (string.IsNullOrEmpty(firstName))
            {
                _NameLanguageMode = enNameLanguage.Undecided;
            }
            else if (_NameLanguageMode == enNameLanguage.Undecided)
            {
                // تحديد اللغة بناءً على أول حرف تم كتابته فعلياً بشكل آمن
                char firstChar = firstName[0];
                if (firstChar >= 0x0600 && firstChar <= 0x06FF)
                    _NameLanguageMode = enNameLanguage.Arabic;
                else if ((firstChar >= 'A' && firstChar <= 'Z') || (firstChar >= 'a' && firstChar <= 'z'))
                    _NameLanguageMode = enNameLanguage.English;
            }

            _HandleLanguageAutoSwitch(txtFirstName); // دالة تغيير اتجاه الكيبورد التلقائي
        }

        private void txtSecondName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSecondName == null) return;
            _AdjustTextDirection(txtSecondName);
        }

        private void txtThirdName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtThirdName == null) return;
            _AdjustTextDirection(txtThirdName);
        }

        private void txtLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtLastName == null) return;
            _AdjustTextDirection(txtLastName);
        }
    }
}