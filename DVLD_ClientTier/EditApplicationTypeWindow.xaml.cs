using System;
using System.Windows;

namespace DVLD_ClientTier
{
    public partial class EditApplicationTypeWindow : Window
    {
        private int _ApplicationTypeID;
        private ClsApplicationType _ApplicationType;

        public EditApplicationTypeWindow(int applicationTypeID)
        {
            InitializeComponent();
            _ApplicationTypeID = applicationTypeID;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _ApplicationType = ClsApplicationType.Find(_ApplicationTypeID);

            if (_ApplicationType == null)
            {
                MessageBox.Show("No Application Type with ID = " + _ApplicationTypeID, "Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            lblApplicationTypeID.Text = _ApplicationType.ApplicationTypeID.ToString();
            txtTitle.Text = _ApplicationType.ApplicationTypeName;
            txtFees.Text = _ApplicationType.Fees.ToString();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // التحقق من المدخلات
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || !decimal.TryParse(txtFees.Text, out decimal fees))
            {
                MessageBox.Show("Please enter a valid title and numeric fees.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _ApplicationType.ApplicationTypeName = txtTitle.Text.Trim();
            _ApplicationType.Fees = fees;

            if (_ApplicationType.Save())
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