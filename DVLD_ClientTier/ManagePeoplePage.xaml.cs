using BassemCore.Validation;
using DVLD_BusinessLayer;
using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace DVLD_ClientTier
{
    public partial class ManagePeoplePage : Page
    {
        #region Private Fields

        // ===== متغيرات نظام الـ Pagination والبيانات =====
        private DataTable _dtAllPeople = new DataTable(); // الاحتفاظ بكافة البيانات الأصلية/المفلترة
        private int _currentPage = 1;                      // رقم الصفحة الحالية
        private int _pageSize = 10;                         // عدد العناصر في الصفحة الواحدة
        private int _totalRecords = 0;                     // إجمالي عدد السجلات
        private int _totalPages = 1;                        // إجمالي عدد الصفحات

        #endregion

        #region Constructor

        public ManagePeoplePage()
        {
            InitializeComponent();
            this.Loaded += ManagePeoplePage_Loaded;
        }

        #endregion

        #region Page Events & Initialization

        private void ManagePeoplePage_Loaded(object sender, RoutedEventArgs e)
        {
            // ربط ميثود حماية اللصق بمربع النص
            DataObject.AddPastingHandler(txtFilterValue, txtFilterValue_Pasting);

            _RefreshPeopleList();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            // العودة للصفحة السابقة في حال استخدام نظام الـ Navigation
            if (NavigationService != null && NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        #endregion

        #region Data Loading & Pagination Logic

        private void _LoadPeopleData()
        {
            _RefreshPeopleList();
        }

        private void _RefreshPeopleList()
        {
            // 1. جلب البيانات من طبقة الأعمال
            _dtAllPeople = ClsPerson.GetAllPeopleWithDetails();

            // 2. تصفير أي شرط فلترة سابق
            if (_dtAllPeople != null)
            {
                _dtAllPeople.DefaultView.RowFilter = "";
            }

            // 3. ضبط الصفحة الحالية على الأولى وتطبيق التقسيم
            _currentPage = 1;
            _ApplyPagination();

            // 4. تحديث كروت الإحصائيات
            _UpdateDashboardCards();
        }

        private void _ApplyPagination()
        {
            if (_dtAllPeople == null)
            {
                dgvPeople.ItemsSource = null;
                _totalRecords = 0;
                _totalPages = 1;
                _currentPage = 1;
                _UpdatePaginationUI();
                return;
            }

            // 1. جلب الجدول المفلتر حالياً
            DataTable dtActive = _dtAllPeople.DefaultView.ToTable();
            _totalRecords = dtActive.Rows.Count;

            if (_totalRecords == 0)
            {
                dgvPeople.ItemsSource = null;
                _totalPages = 1;
                _currentPage = 1;
                if (lblRecordsCount != null) lblRecordsCount.Text = "0";
                _UpdatePaginationUI();
                return;
            }

            // 2. حساب إجمالي الصفحات وضبط النطاق
            _totalPages = (int)Math.Ceiling((double)_totalRecords / _pageSize);
            if (_currentPage > _totalPages) _currentPage = _totalPages;
            if (_currentPage < 1) _currentPage = 1;

            // 3. تطبيق Skip و Take باستخدام LINQ على السجلات المفلترة
            var pagedRows = dtActive.AsEnumerable()
                                    .Skip((_currentPage - 1) * _pageSize)
                                    .Take(_pageSize);

            // 4. تحديث الـ DataGrid بالصفحة الحالية
            if (pagedRows.Any())
            {
                dgvPeople.ItemsSource = pagedRows.CopyToDataTable().DefaultView;
            }
            else
            {
                dgvPeople.ItemsSource = null;
            }

            // 5. تحديث العداد السفلي
            if (lblRecordsCount != null)
                lblRecordsCount.Text = _totalRecords.ToString("N0");

            _UpdatePaginationUI();
        }

        private void _UpdatePaginationUI()
        {
            if (lblPageInfo != null)
            {
                lblPageInfo.Text = $"صفحة {_currentPage} من {_totalPages} (إجمالي: {_totalRecords})";
            }

            // تفعيل/تعطيل الأزرار بناءً على موقع الصفحة الحالية
            if (btnFirst != null) btnFirst.IsEnabled = _currentPage > 1;
            if (btnPrevious != null) btnPrevious.IsEnabled = _currentPage > 1;
            if (btnNext != null) btnNext.IsEnabled = _currentPage < _totalPages;
            if (btnLast != null) btnLast.IsEnabled = _currentPage < _totalPages;
        }

        private void _UpdateDashboardCards()
        {
            if (_dtAllPeople == null) return;

            // تحويل العرض المفلتر حالياً إلى جدول مصغر في الذاكرة
            DataTable dtFiltered = _dtAllPeople.DefaultView.ToTable();

            if (dtFiltered.Rows.Count == 0)
            {
                lblTotalPeopleCard.Text = "0";
                lblMaleCard.Text = "0";
                lblFemaleCard.Text = "0";
                lblCountriesCard.Text = "0";
                return;
            }

            // 1. إجمالي الأشخاص المفلترين
            lblTotalPeopleCard.Text = dtFiltered.Rows.Count.ToString("N0");

            // 2. عدد الذكور في نتائج البحث
            int maleCount = dtFiltered.Select("GendorCaption = 'Male'").Length;
            lblMaleCard.Text = maleCount.ToString("N0");

            // 3. عدد الإناث في نتائج البحث
            int femaleCount = dtFiltered.Select("GendorCaption = 'Female'").Length;
            lblFemaleCard.Text = femaleCount.ToString("N0");

            // 4. عدد الدول الفريدة في نتائج البحث
            DataTable dtUniqueCountries = dtFiltered.DefaultView.ToTable(true, "CountryName");
            lblCountriesCard.Text = dtUniqueCountries.Rows.Count.ToString("N0");
        }

        #endregion

        #region Filter Logic & Events

        private void _ApplyFilter()
        {
            if (_dtAllPeople == null || txtFilterValue == null || cbFilterBy == null) return;

            ComboBoxItem selectedItem = cbFilterBy.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            string selectedText = selectedItem.Content.ToString();
            string filterValue = txtFilterValue.Text.Trim();

            // 1. مطابقة اسم العمود
            string filterColumn = "";
            switch (selectedText)
            {
                case "Person ID": filterColumn = "PersonID"; break;
                case "National No": filterColumn = "NationalNo"; break;
                case "First Name": filterColumn = "FirstName"; break;
                case "Second Name": filterColumn = "SecondName"; break;
                case "Third Name": filterColumn = "ThirdName"; break;
                case "Last Name": filterColumn = "LastName"; break;
                case "Gender": filterColumn = "GendorCaption"; break;
                case "Nationality": filterColumn = "CountryName"; break;
                case "Phone 1": filterColumn = "Phone1"; break;
                case "Email": filterColumn = "Email"; break;
                default: filterColumn = "None"; break;
            }

            // 2. تطبيق شرط الفلترة
            if (string.IsNullOrEmpty(filterValue) || filterColumn == "None")
            {
                _dtAllPeople.DefaultView.RowFilter = "";
            }
            else if (filterColumn == "PersonID")
            {
                if (int.TryParse(filterValue, out int personID))
                {
                    _dtAllPeople.DefaultView.RowFilter = $"PersonID = {personID}";
                }
                else
                {
                    _dtAllPeople.DefaultView.RowFilter = "1 = 0";
                }
            }
            else
            {
                string safeValue = filterValue.Replace("'", "''");
                _dtAllPeople.DefaultView.RowFilter = $"{filterColumn} LIKE '%{safeValue}%'";
            }

            // 3. العودة للصفحة الأولى وتطبيق Pagination والكروت
            _currentPage = 1;
            _ApplyPagination();
            _UpdateDashboardCards();
        }

        private void cbFilterBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (txtFilterValue == null) return;

            if (cbFilterBy.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedText = selectedItem.Content.ToString();

                if (selectedText == "None")
                {
                    txtFilterValue.Visibility = Visibility.Collapsed;
                    txtFilterValue.Text = "";
                }
                else
                {
                    txtFilterValue.Visibility = Visibility.Visible;
                    txtFilterValue.Text = "";
                    txtFilterValue.Focus();
                }
            }

            _ApplyFilter();
        }

        private void cbxFilterBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbFilterBy_SelectionChanged(sender, e);
        }

        private void txtFilterValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            _ApplyFilter();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            cbFilterBy.SelectedIndex = 0;
            txtFilterValue.Text = "";
            txtFilterValue.Visibility = Visibility.Collapsed;

            _RefreshPeopleList();
        }

        #endregion

        #region ContextMenu & Actions

        private int _GetPersonIDFromSender(object sender)
        {
            if (sender is Button btn && btn.DataContext is DataRowView rowFromButton)
            {
                return Convert.ToInt32(rowFromButton["PersonID"]);
            }

            if (dgvPeople.SelectedItem is DataRowView selectedRow)
            {
                return Convert.ToInt32(selectedRow["PersonID"]);
            }

            return -1;
        }

        private void cmShowDetails_Click(object sender, RoutedEventArgs e)
        {
            int selectedPersonID = _GetPersonIDFromSender(sender);

            if (selectedPersonID != -1)
            {
                Frm_ShowPerson showPerson = new Frm_ShowPerson(selectedPersonID);
                showPerson.ShowDialog();
            }
        }

        private void btnAddNewPerson_Click(object sender, RoutedEventArgs e)
        {
            cmAddNewPerson_Click(sender, e);
        }

        private void cmAddNewPerson_Click(object sender, RoutedEventArgs e)
        {
            Frm_AddEditPerson AddNewperson = new Frm_AddEditPerson();
            AddNewperson.ShowDialog();

            _RefreshPeopleList();
        }

        private void cmEditPerson_Click(object sender, RoutedEventArgs e)
        {
            int selectedPersonID = _GetPersonIDFromSender(sender);

            if (selectedPersonID != -1)
            {
                Frm_AddEditPerson EditPerson = new Frm_AddEditPerson(selectedPersonID);
                EditPerson.ShowDialog();

                _RefreshPeopleList();
            }
        }

        private void cmDeletePerson_Click(object sender, RoutedEventArgs e)
        {
            int selectedPersonID = _GetPersonIDFromSender(sender);

            if (selectedPersonID == -1) return;

            MessageBoxResult result = MessageBox.Show(
                $"هل أنت تأكد من حذف الشخص رقم [{selectedPersonID}]؟",
                "تأكيد الحذف",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (ClsPerson.DeletePerson(selectedPersonID))
                {
                    MessageBox.Show("تم حذف الشخص بنجاح!", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    _RefreshPeopleList();
                }
                else
                {
                    MessageBox.Show("فشل الحذف! هذا الشخص مرتبط ببيانات أخرى داخل النظام.", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void cmSendEmail_Click(object sender, RoutedEventArgs e)
        {
            if (dgvPeople.SelectedItem is DataRowView row)
            {
                string email = row["Email"]?.ToString();

                if (!string.IsNullOrWhiteSpace(email))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo($"mailto:{email}") { UseShellExecute = true });
                }
                else
                {
                    MessageBox.Show("هذا الشخص لا يملك بريد إلكتروني مسجل!", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void cmPhoneCall_Click(object sender, RoutedEventArgs e)
        {
            if (dgvPeople.SelectedItem is DataRowView row)
            {
                string phone = row["Phone1"]?.ToString();
                MessageBox.Show($"الاتصال بالرقم: {phone}", "Phone Call", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        #endregion

        #region Input Validation Rules

        private void txtFilterValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ComboBoxItem selectedItem = cbFilterBy.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            string selectedText = selectedItem.Content.ToString();

            if (selectedText == "Person ID")
            {
                ClsInputValidation.ValidateTextInput(e, ClsInputValidation.eValidationType.NumbersOnly);
            }
            else if (selectedText.Contains("Name") || selectedText == "Nationality" || selectedText == "Gender")
            {
                ClsInputValidation.ValidateTextInput(e, ClsInputValidation.eValidationType.LettersAndSpaces);
            }
            else if (selectedText == "National No")
            {
                ClsInputValidation.ValidateTextInput(e, ClsInputValidation.eValidationType.AlphaNumeric);
            }
            else if (selectedText == "Phone 1")
            {
                ClsInputValidation.ValidateTextInput(e, ClsInputValidation.eValidationType.Phone);
            }
        }

        private void txtFilterValue_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ComboBoxItem selectedItem = cbFilterBy.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            string selectedText = selectedItem.Content.ToString();

            if (selectedText == "Person ID" || selectedText == "National No" || selectedText == "Phone 1")
            {
                ClsInputValidation.PreventSpaceKey(e);
            }
        }

        private void txtFilterValue_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            ComboBoxItem selectedItem = cbFilterBy.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            string selectedText = selectedItem.Content.ToString();

            if (selectedText == "Person ID")
            {
                ClsInputValidation.ValidatePasting(e, ClsInputValidation.eValidationType.NumbersOnly);
            }
            else if (selectedText.Contains("Name") || selectedText == "Nationality" || selectedText == "Gender")
            {
                ClsInputValidation.ValidatePasting(e, ClsInputValidation.eValidationType.LettersAndSpaces);
            }
            else if (selectedText == "National No")
            {
                ClsInputValidation.ValidatePasting(e, ClsInputValidation.eValidationType.AlphaNumeric);
            }
        }

        #endregion

        #region Pagination Navigation Events

        private void btnFirst_Click(object sender, RoutedEventArgs e)
        {
            _currentPage = 1;
            _ApplyPagination();
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                _ApplyPagination();
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                _ApplyPagination();
            }
        }

        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            _currentPage = _totalPages;
            _ApplyPagination();
        }

        private void cbPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPageSize.SelectedItem is ComboBoxItem selectedItem && int.TryParse(selectedItem.Content.ToString(), out int newSize))
            {
                _pageSize = newSize;
                _currentPage = 1;
                _ApplyPagination();
            }
        }

        #endregion
    }
}