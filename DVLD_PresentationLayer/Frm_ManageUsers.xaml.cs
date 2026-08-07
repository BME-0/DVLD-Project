using DVLD.Global_Classes;
using DVLD_BusinessLayer;
using DVLD_PresentationLayer;
using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace DVLD_PresentationLayer
{
    public partial class Frm_ManageUsers : Window
    {
        #region Private Fields

        private DataTable _dtAllUsers = new DataTable();
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalRecords = 0;
        private int _totalPages = 1;

        #endregion

        #region Constructor

        public Frm_ManageUsers()
        {
            InitializeComponent();
        }

        #endregion

        #region Form Events & Initialization

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (txtFilterValue != null)
            {
                DataObject.AddPastingHandler(txtFilterValue, txtFilterValue_Pasting);
            }

            _RefreshUsersList();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            _RefreshUsersList();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Data Loading & Pagination Logic

        private void _LoadUsersData()
        {
            _RefreshUsersList();
        }

        private void _RefreshUsersList()
        {
            _dtAllUsers = ClsUser.GetAllUsers();

            if (_dtAllUsers != null)
            {
                _dtAllUsers.DefaultView.RowFilter = "";
            }

            _currentPage = 1;
            _ApplyPagination();
            _UpdateDashboardCards();
        }

        private void _ApplyPagination()
        {
            if (_dtAllUsers == null)
            {
                if (dgvUsers != null) dgvUsers.ItemsSource = null;
                _totalRecords = 0;
                _totalPages = 1;
                _currentPage = 1;
                _UpdatePaginationUI();
                return;
            }

            DataTable dtActive = _dtAllUsers.DefaultView.ToTable();
            _totalRecords = dtActive.Rows.Count;

            if (_totalRecords == 0)
            {
                if (dgvUsers != null) dgvUsers.ItemsSource = null;
                _totalPages = 1;
                _currentPage = 1;
                if (lblRecordsCount != null) lblRecordsCount.Text = "0";
                _UpdatePaginationUI();
                return;
            }

            _totalPages = (int)Math.Ceiling((double)_totalRecords / _pageSize);
            if (_currentPage > _totalPages) _currentPage = _totalPages;
            if (_currentPage < 1) _currentPage = 1;

            var pagedRows = dtActive.AsEnumerable()
                                    .Skip((_currentPage - 1) * _pageSize)
                                    .Take(_pageSize);

            if (dgvUsers != null)
            {
                if (pagedRows.Any())
                {
                    dgvUsers.ItemsSource = pagedRows.CopyToDataTable().DefaultView;
                }
                else
                {
                    dgvUsers.ItemsSource = null;
                }
            }

            if (lblRecordsCount != null)
                lblRecordsCount.Text = _totalRecords.ToString("N0");

            _UpdatePaginationUI();
        }

        private void _UpdatePaginationUI()
        {
            if (lblPageInfo != null)
            {
                lblPageInfo.Text = $"Page {_currentPage} of {_totalPages} (Total: {_totalRecords})";
            }

            if (btnFirst != null) btnFirst.IsEnabled = _currentPage > 1;
            if (btnPrevious != null) btnPrevious.IsEnabled = _currentPage > 1;
            if (btnNext != null) btnNext.IsEnabled = _currentPage < _totalPages;
            if (btnLast != null) btnLast.IsEnabled = _currentPage < _totalPages;
        }

        private void _UpdateDashboardCards()
        {
            if (_dtAllUsers == null) return;

            DataTable dtFiltered = _dtAllUsers.DefaultView.ToTable();

            if (dtFiltered.Rows.Count == 0)
            {
                if (lblTotalUsersCard != null) lblTotalUsersCard.Text = "0";
                if (lblActiveUsersCard != null) lblActiveUsersCard.Text = "0";
                if (lblInactiveUsersCard != null) lblInactiveUsersCard.Text = "0";
                if (lblTodayUsersCard != null) lblTodayUsersCard.Text = "0";
                return;
            }

            if (lblTotalUsersCard != null)
                lblTotalUsersCard.Text = dtFiltered.Rows.Count.ToString("N0");

            try
            {
                int activeCount = dtFiltered.Select("IsActive = 1").Length;
                if (lblActiveUsersCard != null) lblActiveUsersCard.Text = activeCount.ToString("N0");

                int inactiveCount = dtFiltered.Select("IsActive = 0").Length;
                if (lblInactiveUsersCard != null) lblInactiveUsersCard.Text = inactiveCount.ToString("N0");
            }
            catch
            {
                if (lblActiveUsersCard != null) lblActiveUsersCard.Text = "0";
                if (lblInactiveUsersCard != null) lblInactiveUsersCard.Text = "0";
            }

            if (lblTodayUsersCard != null)
                lblTodayUsersCard.Text = "0";
        }

        #endregion

        #region Filter Logic & Events

        private void _ApplyFilter()
        {
            if (_dtAllUsers == null || cbFilterBy == null) return;

            ComboBoxItem selectedItem = cbFilterBy.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            string selectedText = selectedItem.Content.ToString();
            string filterColumn = "";

            switch (selectedText)
            {
                case "User ID": filterColumn = "UserID"; break;
                case "Person ID": filterColumn = "PersonID"; break;
                case "Full Name": filterColumn = "FullName"; break;
                case "UserName": filterColumn = "UserName"; break;
                case "Is Active": filterColumn = "IsActive"; break;
                default: filterColumn = "None"; break;
            }

            if (filterColumn == "None")
            {
                _dtAllUsers.DefaultView.RowFilter = "";
            }
            else if (filterColumn == "UserID" || filterColumn == "PersonID")
            {
                string filterValue = txtFilterValue != null ? txtFilterValue.Text.Trim() : "";
                if (string.IsNullOrEmpty(filterValue))
                {
                    _dtAllUsers.DefaultView.RowFilter = "";
                }
                else if (int.TryParse(filterValue, out int id))
                {
                    _dtAllUsers.DefaultView.RowFilter = $"{filterColumn} = {id}";
                }
                else
                {
                    _dtAllUsers.DefaultView.RowFilter = "1 = 0";
                }
            }
            else if (filterColumn == "IsActive")
            {
                if (cbIsActiveValue != null && cbIsActiveValue.SelectedItem is ComboBoxItem activeItem)
                {
                    string activeText = activeItem.Content.ToString();
                    if (activeText.Equals("Yes", StringComparison.OrdinalIgnoreCase))
                        _dtAllUsers.DefaultView.RowFilter = "IsActive = 1";
                    else if (activeText.Equals("No", StringComparison.OrdinalIgnoreCase))
                        _dtAllUsers.DefaultView.RowFilter = "IsActive = 0";
                    else
                        _dtAllUsers.DefaultView.RowFilter = "";
                }
                else
                {
                    _dtAllUsers.DefaultView.RowFilter = "";
                }
            }
            else
            {
                string filterValue = txtFilterValue != null ? txtFilterValue.Text.Trim() : "";
                if (string.IsNullOrEmpty(filterValue))
                {
                    _dtAllUsers.DefaultView.RowFilter = "";
                }
                else
                {
                    string safeValue = filterValue.Replace("'", "''");
                    _dtAllUsers.DefaultView.RowFilter = $"{filterColumn} LIKE '%{safeValue}%'";
                }
            }

            _currentPage = 1;
            _ApplyPagination();
            _UpdateDashboardCards();
        }

        private void cbFilterBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbFilterBy == null || cbFilterBy.SelectedItem == null) return;

            if (cbFilterBy.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedText = selectedItem.Content.ToString();

                if (selectedText == "None")
                {
                    if (txtFilterValue != null)
                    {
                        txtFilterValue.Visibility = Visibility.Collapsed;
                        txtFilterValue.Text = "";
                    }
                    if (cbIsActiveValue != null)
                        cbIsActiveValue.Visibility = Visibility.Collapsed;
                }
                else if (selectedText == "Is Active")
                {
                    if (txtFilterValue != null)
                        txtFilterValue.Visibility = Visibility.Collapsed;
                    if (cbIsActiveValue != null)
                    {
                        cbIsActiveValue.Visibility = Visibility.Visible;
                        cbIsActiveValue.SelectedIndex = 0;
                    }
                }
                else
                {
                    if (cbIsActiveValue != null)
                        cbIsActiveValue.Visibility = Visibility.Collapsed;
                    if (txtFilterValue != null)
                    {
                        txtFilterValue.Visibility = Visibility.Visible;
                        txtFilterValue.Text = "";
                        txtFilterValue.Focus();
                    }
                }
            }

            _ApplyFilter();
        }

        private void cbIsActiveValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _ApplyFilter();
        }

        private void txtFilterValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            _ApplyFilter();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (cbFilterBy != null) cbFilterBy.SelectedIndex = 0;

            if (txtFilterValue != null)
            {
                txtFilterValue.Text = "";
                txtFilterValue.Visibility = Visibility.Collapsed;
            }
            if (cbIsActiveValue != null)
            {
                cbIsActiveValue.SelectedIndex = 0;
                cbIsActiveValue.Visibility = Visibility.Collapsed;
            }

            _RefreshUsersList();
        }

        #endregion

        #region ContextMenu & Actions

        private int _GetUserIDFromSender(object sender)
        {
            if (sender is Button btn && btn.DataContext is DataRowView rowFromButton)
            {
                return Convert.ToInt32(rowFromButton["UserID"]);
            }

            if (dgvUsers != null && dgvUsers.SelectedItem is DataRowView selectedRow)
            {
                return Convert.ToInt32(selectedRow["UserID"]);
            }

            return -1;
        }

        private void cmShowDetails_Click(object sender, RoutedEventArgs e)
        {
            int userID = _GetUserIDFromSender(sender);
            if (userID != -1)
            {
                // Frm_ShowUserInfo frm = new Frm_ShowUserInfo(userID);
                // frm.ShowDialog();
            }
        }

        private void btnAddNewUser_Click(object sender, RoutedEventArgs e)
        {
            cmAddNewUser_Click(sender, e);
        }

        private void cmAddNewUser_Click(object sender, RoutedEventArgs e)
        {
            // Frm_AddUpdateUser frm = new Frm_AddUpdateUser();
            // frm.ShowDialog();

            _RefreshUsersList();
        }

        private void cmEditUser_Click(object sender, RoutedEventArgs e)
        {
            int userID = _GetUserIDFromSender(sender);
            if (userID != -1)
            {
                // Frm_AddUpdateUser frm = new Frm_AddUpdateUser(userID);
                // frm.ShowDialog();

                _RefreshUsersList();
            }
        }

        private void cmDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            int userID = _GetUserIDFromSender(sender);
            if (userID == -1) return;

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete user ID [{userID}]?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (ClsUser.DeleteUser(userID))
                {
                    MessageBox.Show("User deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    _RefreshUsersList();
                }
                else
                {
                    MessageBox.Show("Failed to delete user! This user may be linked to other records.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void cmChangePassword_Click(object sender, RoutedEventArgs e)
        {
            int userID = _GetUserIDFromSender(sender);
            if (userID != -1)
            {
                // Frm_ChangePassword frm = new Frm_ChangePassword(userID);
                // frm.ShowDialog();
                _RefreshUsersList();
            }
        }

        #endregion

        #region Input Validation Rules

        private void txtFilterValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (cbFilterBy == null || cbFilterBy.SelectedItem == null) return;
            ComboBoxItem selectedItem = cbFilterBy.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            string selectedText = selectedItem.Content.ToString();

            if (selectedText == "User ID" || selectedText == "Person ID")
            {
                ClsInputValidation.ValidateTextInput(e, ClsInputValidation.eValidationType.NumbersOnly);
            }
            else if (selectedText == "Full Name")
            {
                ClsInputValidation.ValidateTextInput(e, ClsInputValidation.eValidationType.LettersAndSpaces);
            }
            else if (selectedText == "UserName")
            {
                ClsInputValidation.ValidateTextInput(e, ClsInputValidation.eValidationType.AlphaNumeric);
            }
        }

        private void txtFilterValue_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (cbFilterBy == null || cbFilterBy.SelectedItem == null) return;
            ComboBoxItem selectedItem = cbFilterBy.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            string selectedText = selectedItem.Content.ToString();

            if (selectedText == "User ID" || selectedText == "Person ID" || selectedText == "UserName")
            {
                ClsInputValidation.PreventSpaceKey(e);
            }
        }

        private void txtFilterValue_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (cbFilterBy == null || cbFilterBy.SelectedItem == null) return;
            ComboBoxItem selectedItem = cbFilterBy.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            string selectedText = selectedItem.Content.ToString();

            if (selectedText == "User ID" || selectedText == "Person ID")
            {
                ClsInputValidation.ValidatePasting(e, ClsInputValidation.eValidationType.NumbersOnly);
            }
            else if (selectedText == "Full Name")
            {
                ClsInputValidation.ValidatePasting(e, ClsInputValidation.eValidationType.LettersAndSpaces);
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
            if (cbPageSize != null && cbPageSize.SelectedItem is ComboBoxItem selectedItem && int.TryParse(selectedItem.Content.ToString(), out int newSize))
            {
                _pageSize = newSize;
                _currentPage = 1;
                _ApplyPagination();
            }
        }

        #endregion

        #region Window Maximized Hook

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                hwndSource.AddHook(HwndHook);
            }
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOVE = 0xF010;

            if (msg == WM_SYSCOMMAND)
            {
                int command = wParam.ToInt32() & 0xFFF0;
                if (command == SC_MOVE && this.WindowState == WindowState.Maximized)
                {
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }

        #endregion
    }
}