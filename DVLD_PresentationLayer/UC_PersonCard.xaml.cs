using DVLD_BusinessLayer; // استدعي الـ Namespace الخاص بطبقة الـ Business عندك
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
//using System.ComponentModel;

namespace DVLD_PresentationLayer
{
    public partial class UC_PersonCard : UserControl
    {
        // تعريف الـ Modes الخاصة بالكنترول

        private ClsPerson _Person;
        private int _PersonID = -1;

        public int PersonID => _PersonID;
        public ClsPerson SelectedPersonInfo => _Person;

        // خصائص (Properties) لقراءة حالة الكنترول من الخارج

        public UC_PersonCard()
        {
            InitializeComponent();

            // السطر ده هو السحر اللي بيمنع الـ Designer من الانهيار
            // لو الفيجوال ستوديو بيرسم الكنترول في وضع التصميم، اخرج فوراً ومتنفذش أي كود خلفي
            //if (DesignerProperties.GetIsInDesignMode(this))
            //{
            //    return;
            //}

            //// الأكواد دي هتتنفذ فقط أثناء تشغيل البرنامج الفعلي (Runtime)
            ResetPersonInfo();
        }

        // دالة جلب البيانات باستخدام الـ ID (تلقائياً تحول الـ Mode إلى Update)
        public bool LoadPersonInfo(int PersonID)
        {
            _Person = ClsPerson.Find(PersonID);

            if (_Person == null)
            {
                ResetPersonInfo();
                MessageBox.Show("No Person with ID = " + PersonID, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            
            _PersonID = _Person.PersonID;

            _FillPersonData();
            return true;
        }

        // دالة جلب البيانات باستخدام الرقم الوطني (تلقائياً تحول الـ Mode إلى Update)
        public bool LoadPersonInfo(string NationalNo)
        {
            _Person = ClsPerson.FindByNationalNo(NationalNo);

            if (_Person == null)
            {
                ResetPersonInfo();
                MessageBox.Show("No Person with National No = " + NationalNo, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            
            _PersonID = _Person.PersonID;

            _FillPersonData();
            return true;
        }

        // دالة تعبئة الحقول بالبيانات بشكل منفصل
        private void _FillPersonData()
        {
            // 1. الرقم التعريفي
            lblPersonID.Text = _Person.PersonID.ToString();

            // 2. توزيع الأسماء الأربعة بالتفصيل
            lblFirstName.Text = _Person.FirstName;
            lblSecondName.Text = _Person.SecondName;
            lblThirdName.Text = string.IsNullOrEmpty(_Person.ThirdName) ? "—" : _Person.ThirdName;
            lblLastName.Text = _Person.LastName;

            // 3. الرقم الوطني
            lblNationalNo.Text = _Person.NationalNo;

            // 4. تاريخ الميلاد وحساب السن بدقة
            int Age = DateTime.Now.Year - _Person.DateOfBirth.Year;
            if (_Person.DateOfBirth.Date > DateTime.Now.AddYears(-Age))
                Age--;

            lblDateOfBirthAndAge.Text = $"{_Person.DateOfBirth.ToString("dd/MM/yyyy")} ({Age} Years)";

            // 5. الهواتف والبريد الإلكتروني والعنوان
            lblPhone1.Text = _Person.Phone1;
            lblPhone2.Text = string.IsNullOrEmpty(_Person.Phone2) ? "—" : _Person.Phone2;
            lblEmail.Text = string.IsNullOrEmpty(_Person.Email) ? "—" : _Person.Email;
            lblAddress.Text = _Person.Address;

            // 6. جلب اسم الدولة من الـ ID (NationalityCountryID -> CountryName)
            ClsCountry Country = ClsCountry.Find(_Person.NationalityCountryID);
            lblCountryName.Text = (Country != null) ? Country.CountryName_En : "Egypt";

            // فحص الـ Gender لعرض الأيقونة والاسم المناسب في lblGender
            //if (_Person.Gender) // True تعني أنثى (Female)
            //{
            //    lblGender.Text = "👩 Female";
            //}
            //else // False تعني ذكر (Male)
            //{
            //    lblGender.Text = "🧔 Male";
            //}

            lblGender.Text = _Person.GenderName;

            // 7. تحميل الصورة الشخصية
            _LoadPersonImage();
        }

        // تحميل صورة الشخص بأمان
        private void _LoadPersonImage()
        {
            try
            {
                if (!string.IsNullOrEmpty(_Person.ImagePath) && File.Exists(_Person.ImagePath))
                {
                    imgPerson.ImageSource = new BitmapImage(new Uri(_Person.ImagePath));
                }
                else
                {
                    _LoadDefaultImage();
                }
            }
            catch
            {
                _LoadDefaultImage();
            }
        }

        // تحميل الصورة الافتراضية
        private void _LoadDefaultImage()
        {
            imgPerson.ImageSource = null;
            txtGenderIcon.Visibility = Visibility.Visible;

            if (_Person != null)
            {
                if (_Person.Gender) // True = Female
                {
                    txtGenderIcon.Text = "👩"; // رمز البنت بالخلفية
                }
                else // False = Male
                {
                    txtGenderIcon.Text = "🧔"; // رمز الولد بالخلفية
                }
            }
            else
            {
                txtGenderIcon.Text = "👤"; // الرمز الافتراضي العام
            }
        }

        // إعادة الكارت لوضع الـ AddNew وتصفير الحقول الفردية
        public void ResetPersonInfo()
        {
            
            _PersonID = -1;
            _Person = null;

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
            lblAddress.Text = "N/A";
            lblCountryName.Text = "N/A";
            lblGender.Text = "N/A";
            _LoadDefaultImage();
        }
    }
}