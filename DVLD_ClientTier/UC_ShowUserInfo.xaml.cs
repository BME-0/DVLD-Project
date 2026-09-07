using DVLD_BusinessLayer;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DVLD_ClientTier
{
    public partial class UC_ShowUserInfo : UserControl
    {
        private int _UserID = -1;
        private ClsUser _User;

        public int UserID => _UserID;
        public ClsUser SelectedUserInfo => _User;

        public UC_ShowUserInfo()
        {
            InitializeComponent();
        }

        public void LoadUserInfo(int userID)
        {
            _UserID = userID;
            _User = ClsUser.Find(_UserID);

            if (_User == null)
            {
                ResetUserInfo();
                MessageBox.Show($"No User with UserID = {_UserID} was found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _FillUserData();
        }

        private void _FillUserData()
        {
            if (lblUserID != null) lblUserID.Text = _User.UserID.ToString();
            if (lblUserName != null) lblUserName.Text = _User.UserName;

            if (lblIsActive != null)
            {
                if (_User.IsActive)
                {
                    lblIsActive.Text = "Active";
                    lblIsActive.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#38A169")); // أخضر عصري
                }
                else
                {
                    lblIsActive.Text = "Inactive";
                    lblIsActive.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E53E3E")); // أحمر عصري
                }
            }
        }

        public void ResetUserInfo()
        {
            _UserID = -1;
            _User = null;

            if (lblUserID != null) lblUserID.Text = "[????]";
            if (lblUserName != null) lblUserName.Text = "[????]";
            if (lblIsActive != null)
            {
                lblIsActive.Text = "[????]";
                lblIsActive.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D3748"));
            }
        }
    }
}