using DVLD_BusinessLayer;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace DVLD_ClientTier
{
    public partial class TestTypePage : Page
    {
        private DataTable _dtTestTypes;

        public TestTypePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _RefreshTestTypesList();
            txtSearchValue.Visibility = Visibility.Collapsed; // إخفاء حقل البحث إذا كان الخيار "All"
        }

        private void _RefreshTestTypesList()
        {
            _dtTestTypes = ClsTestType.GetAllTestTypes();
            dgvTestTypes.ItemsSource = _dtTestTypes.DefaultView;
            lblCount.Text = _dtTestTypes.Rows.Count.ToString();
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
                if (_dtTestTypes != null)
                    _dtTestTypes.DefaultView.RowFilter = "";
                lblCount.Text = dgvTestTypes.Items.Count.ToString();
            }
            else
            {
                txtSearchValue.Visibility = Visibility.Visible;
                txtSearchValue.Focus();
            }
        }

        private void txtSearchValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_dtTestTypes == null) return;

            string filterColumn = "";
            ComboBoxItem selectedItem = (ComboBoxItem)cbFilterColumn.SelectedItem;
            if (selectedItem == null) return;

            string selectedFilter = selectedItem.Content.ToString();

            if (selectedFilter == "Test ID")
                filterColumn = "TestTypeID";
            else if (selectedFilter == "Test Name")
                filterColumn = "TestTypeName";

            if (string.IsNullOrWhiteSpace(txtSearchValue.Text))
            {
                _dtTestTypes.DefaultView.RowFilter = "";
            }
            else
            {
                if (filterColumn == "TestTypeID")
                {
                    // لو بيبحث برقم، نتأكد إنه رقم عشان ما يحصلش Crash
                    if (int.TryParse(txtSearchValue.Text.Trim(), out int id))
                        _dtTestTypes.DefaultView.RowFilter = $"{filterColumn} = {id}";
                    else
                        _dtTestTypes.DefaultView.RowFilter = $"{filterColumn} = 0";
                }
                else
                {
                    // لو بيبحث بالنص (Contains)
                    _dtTestTypes.DefaultView.RowFilter = $"{filterColumn} LIKE '%{txtSearchValue.Text.Trim().Replace("'", "''")}%'";
                }
            }

            lblCount.Text = dgvTestTypes.Items.Count.ToString();
        }

        private void EditTestType_Click(object sender, RoutedEventArgs e)
        {
            if (dgvTestTypes.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)dgvTestTypes.SelectedItem;
                int TestTypeID = Convert.ToInt32(selectedRow["TestTypeID"]);

                EditTestTypeWindow frm = new EditTestTypeWindow(TestTypeID);
                frm.ShowDialog();

                _RefreshTestTypesList();
            }
            else
            {
                MessageBox.Show("Please select an Test type to edit.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}