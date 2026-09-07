using DVLD_ClientTier;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Xml.Linq;

namespace DVLD_ClientTier
{
    public partial class Frm_ShowPerson : Form
    {
        // Property بترجع الكارت جاهز
        public UC_PersonCard PersonCard => ShowPersonElement.Child as UC_PersonCard;

        public Frm_ShowPerson(int PersonID)
        {
            InitializeComponent();

            PersonCard?.LoadPersonInfo(PersonID);
        }

        public Frm_ShowPerson(string National_No)
        {
            InitializeComponent();

            PersonCard?.LoadPersonInfo(National_No);
        }

        private void Frm_ShowPerson_Load(object sender, EventArgs e)
        {
            ShowPersonElement.Width = this.Width;
            ShowPersonElement.Height = this.Height;

            this.MaximizeBox = false;
        }

        private void Element1_ChildChanged(object sender, ChildChangedEventArgs e)
        {

        }
    }
}
