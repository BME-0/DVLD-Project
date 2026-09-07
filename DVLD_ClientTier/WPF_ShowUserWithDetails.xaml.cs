using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace DVLD_ClientTier
{
    /// <summary>
    /// Interaction logic for WPF_ShowUserWithDetails.xaml
    /// </summary>
    public partial class WPF_ShowUserWithDetails : Window
    {
        public WPF_ShowUserWithDetails(int UserID)
        {
            InitializeComponent();

            UserCard.LoadUserInfo(UserID);
        }
    }
}
