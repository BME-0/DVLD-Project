using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DVLD_PresentationLayer
{
    /// <summary>
    /// Interaction logic for UC_PersonCardWithLink.xaml
    /// </summary>
    public partial class UC_PersonCardWithLink : UserControl
    {
        // 1. المُنشئ الافتراضي المطلوب للـ Visual Studio Designer والـ Toolbox
        public UC_PersonCardWithLink()
        {
            InitializeComponent();

            // يمنع الـ Designer من تنفيذ أي كود يعمل في الـ Runtime
            //if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            //{
            //    return;
            //}
        }

        public bool LoadPersonInfo(int PersonID)
        {
           return PersonCard.LoadPersonInfo(PersonID);
        }

        public bool LoadPersonInfo(string NationalNO)
        {
           return PersonCard.LoadPersonInfo(NationalNO);
        }

        // 📧 1. إرسال بريد إلكتروني (Send Email)
        private void btnSendEmail_Click(object sender, RoutedEventArgs e)
        {
            // جلب البريد الإلكتروني من الكارت الداخلي PersonCard
            string email = PersonCard.SelectedPersonInfo?.Email; // أو PersonCard.Email حسب تسمية الخاصية عندك

            if (!string.IsNullOrWhiteSpace(email) && email != "N/A" && email != "[N/A]")
            {
                try
                {
                    Process.Start(new ProcessStartInfo($"mailto:{email}") { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"تعذر فتح تطبيق البريد: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("هذا الشخص لا يملك بريد إلكتروني مسجل!", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 📞 2. إجراء مكالمة هاتفية (Phone Call)
        private void btnCallPhone_Click(object sender, RoutedEventArgs e)
        {
            // جلب رقم الهاتف من الكارت الداخلي PersonCard
            string phone = PersonCard.SelectedPersonInfo?.Phone1; // أو PersonCard.Phone حسب تسمية الخاصية عندك

            if (!string.IsNullOrWhiteSpace(phone) && phone != "N/A" && phone != "[N/A]")
            {
                MessageBox.Show($"جاري الاتصال بالرقم: {phone}", "Phone Call", MessageBoxButton.OK, MessageBoxImage.Information);

                // يمكنك إزالة التعليق أدناه إذا أردت فتح تطبيق الاتصال الافتراضي في الويندوز:
                // Process.Start(new ProcessStartInfo($"tel:{phone}") { UseShellExecute = true });
            }
            else
            {
                MessageBox.Show("لا يوجد رقم هاتف مسجل لهذا الشخص!", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // ✏ 3. تعديل بيانات الشخص (لضمان عدم حدوث Compilation Error مع الـ XAML)
        private void btnEditPerson_Click(object sender, RoutedEventArgs e)
        {
            int personID = PersonCard.PersonID;

            if (personID != -1)
            {
                Frm_AddEditPerson frm = new Frm_AddEditPerson(personID);
                frm.ShowDialog();

                PersonCard.LoadPersonInfo(personID); // إعادة تحميل البيانات بعد التعديل
            }
            else
            {
                MessageBox.Show("رجاءً قم بختيار شخص أولاً!", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void PersonCard_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}