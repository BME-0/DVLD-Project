using System;
using System.Windows;
using DVLD_BusinessLayer;

namespace DVLD_ClientTier
{
    public partial class EditTestTypeWindow : Window
    {
        private int _TestTypeID;
        private ClsTestType _TestType;

        public EditTestTypeWindow(int TestTypeID)
        {
            InitializeComponent();
            _TestTypeID = TestTypeID;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _TestType = ClsTestType.Find(_TestTypeID);

            if (_TestType == null)
            {
                MessageBox.Show("No Test Type with ID = " + _TestTypeID, "Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            lblTestTypeID.Text = _TestType.TestTypeID.ToString();
            txtTitle.Text = _TestType.TestTypeName;
            txtDescription.Text = _TestType.TestTypeDescription;
            txtFees.Text = _TestType.Fees.ToString();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // التحقق من المدخلات
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtDescription.Text) || !decimal.TryParse(txtFees.Text, out decimal fees))
            {
                MessageBox.Show("Please enter a valid title and Description and numeric fees.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _TestType.TestTypeName = txtTitle.Text.Trim();
            _TestType.TestTypeDescription = txtDescription.Text.Trim();
            _TestType.Fees = fees;

            if (_TestType.Save())
            {
                MessageBox.Show("Data Successfully Saved.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Error: Data Is not Saved.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}