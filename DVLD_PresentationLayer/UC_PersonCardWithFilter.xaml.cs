using DVLD.Global_Classes;
using DVLD_BusinessLayer;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DVLD_PresentationLayer
{
    public partial class UC_PersonCardWithFilter : UserControl
    {
        public UC_PersonCardWithFilter()
        {
            InitializeComponent();

            // 1. كود منع اللصق (Paste) نهائياً في مربع البحث
            CommandManager.AddPreviewCanExecuteHandler(txtFilterValue, OnPasteCommand);
        }

        // دالة منع اللصق
        private void OnPasteCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
            {
                e.CanExecute = false;
                e.Handled = true;
            }
        }

        private void cbFilterBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (txtFilterValue == null) return;

            // مسح حقل النص عند تغيير نوع الفلتر
            txtFilterValue.Clear();
            txtFilterValue.Focus();
        }

        private void txtFilterValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            // منطق البحث عند تغيير النص
        }

        private void txtFilterValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // التحقق من الفلتر المختار حالياً
            if (cbFilterBy.SelectedItem is ComboBoxItem selectedItem)
            {
                string filterName = selectedItem.Content.ToString();

                // لو المستخدم مختار Person ID -> نسمح بالأرقام فقط
                if (filterName == "Person ID")
                {
                    ClsInputValidation.ValidateTextInput(e, ClsInputValidation.eValidationType.NumbersOnly);
                }
                // لو المستخدم مختار National No -> مسموح بكل شيء (حروف وأرقام) أو اتركها كما تريد
                else if (filterName == "National No")
                {
                    // لا تقم بتقييد الإدخال هنا لكي يسمح بالرقم القومي (حروف/أرقام)
                }
            }
        }

        private void txtFilterValue_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // تنفيذ البحث عند الضغط على مفتاح Enter
            if (e.Key == Key.Enter)
            {
                // ⬇️ حط الكود بتاعك هنا ⬇️

                string filterValue = txtFilterValue.Text.Trim();

                // التأكد من أن حقل النص ليس فارغاً
                if (string.IsNullOrEmpty(filterValue))
                {
                    MessageBox.Show("الرجاء إدخال قيمة للبحث!", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                // معرفة الفلتر المختار حالياً لتنفيذ البحث الصحيح
                if (cbFilterBy.SelectedItem is ComboBoxItem selectedItem)
                {
                    string filterType = selectedItem.Content.ToString();

                    if (filterType == "Person ID")
                    {
                        // مثال: تحويل النص لـ رقم واستدعاء دالة الكارت
                         int personID = Convert.ToInt32(filterValue);
                         PersonCard.LoadPersonInfo(personID);
                    }
                    else if (filterType == "National No")
                    {
                        // مثال: استدعاء دالة الكارت باستخدام الرقم القومي مباشرة
                        PersonCard.LoadPersonInfo(filterValue);
                    }
                }

                // ⬆️ نهاية الكود بتاعك ⬆️

                e.Handled = true; // منع انتقال الحدث لأزرار أخرى
            }
        }
    }
}