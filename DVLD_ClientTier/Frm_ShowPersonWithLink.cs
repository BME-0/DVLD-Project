using DVLD_ClientTier;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Linq;

namespace DVLD_ClientTier
{
    public partial class Frm_ShowPersonWithLink : Form
    {
        // Property بترجع الكارت جاهز
        public UC_PersonCardWithLink PersonCardWithLink => Element.Child as UC_PersonCardWithLink;

        public Frm_ShowPersonWithLink(int PersonID)
        {
            InitializeComponent();

            PersonCardWithLink?.LoadPersonInfo(PersonID);
        }

        public Frm_ShowPersonWithLink(string National_No)
        {
            InitializeComponent();

            PersonCardWithLink?.LoadPersonInfo(National_No);
        }

        private void Frm_ShowPersonWithLink_Load(object sender, EventArgs e)
        {
            Element.Width = this.Width;
            Element.Height = this.Height;

            this.MaximizeBox = false;
        }

        private void Element_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }

        private void Element_ChildChanged_1(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }
    }
}
