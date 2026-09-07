using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace DVLD_ClientTier
{
    public partial class ManageApplicationTypesPage : Page
    {
        private DataTable _dtApplicationTypes;

        public ManageApplicationTypesPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _RefreshApplicationTypesList();
            txtSearchValue.Visibility = Visibility.Collapsed; // إخفاء حقل البحث إذا كان الخيار "All"
        }

        private void _RefreshApplicationTypesList()
        {
            _dtApplicationTypes = ClsApplicationType.GetAllApplicationTypes();
            dgvApplicationTypes.ItemsSource = _dtApplicationTypes.DefaultView;
            lblCount.Text = _dtApplicationTypes.Rows.Count.ToString();
        }

        private void cbFilterColumn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (txtSearchValue == null) return;

            ComboBoxItem selectedItem = (ComboBoxItem)cbFilterColumn.SelectedItem;
            string filterText = selectedItem.Content.ToString();

            if (filterText == "All")
            {
                txtSearchValue.Visibility = Visibility.Collapsed;
                txtSearchValue.Clear();
                if (_dtApplicationTypes != null)
                    _dtApplicationTypes.DefaultView.RowFilter = "";
                lblCount.Text = dgvApplicationTypes.Items.Count.ToString();
            }
            else
            {
                txtSearchValue.Visibility = Visibility.Visible;
                txtSearchValue.Focus();
            }
        }

        private void txtSearchValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_dtApplicationTypes == null) return;

            string filterColumn = "";
            ComboBoxItem selectedItem = (ComboBoxItem)cbFilterColumn.SelectedItem;
            if (selectedItem == null) return;

            string selectedFilter = selectedItem.Content.ToString();

            if (selectedFilter == "ID")
                filterColumn = "ApplicationTypeID";
            else if (selectedFilter == "Application Type")
                filterColumn = "ApplicationTypeName";

            if (string.IsNullOrWhiteSpace(txtSearchValue.Text))
            {
                _dtApplicationTypes.DefaultView.RowFilter = "";
            }
            else
            {
                if (filterColumn == "ApplicationTypeID")
                {
                    // لو بيبحث برقم، نتأكد إنه رقم عشان ما يحصلش Crash
                    if (int.TryParse(txtSearchValue.Text.Trim(), out int id))
                        _dtApplicationTypes.DefaultView.RowFilter = $"{filterColumn} = {id}";
                    else
                        _dtApplicationTypes.DefaultView.RowFilter = $"{filterColumn} = 0";
                }
                else
                {
                    // لو بيبحث بالنص (Contains)
                    _dtApplicationTypes.DefaultView.RowFilter = $"{filterColumn} LIKE '%{txtSearchValue.Text.Trim().Replace("'", "''")}%'";
                }
            }

            lblCount.Text = dgvApplicationTypes.Items.Count.ToString();
        }

        private void EditApplicationType_Click(object sender, RoutedEventArgs e)
        {
            if (dgvApplicationTypes.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)dgvApplicationTypes.SelectedItem;
                int applicationTypeID = Convert.ToInt32(selectedRow["ApplicationTypeID"]);

                EditApplicationTypeWindow frm = new EditApplicationTypeWindow(applicationTypeID);
                frm.ShowDialog();

                _RefreshApplicationTypesList();
            }
            else
            {
                MessageBox.Show("Please select an application type to edit.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}