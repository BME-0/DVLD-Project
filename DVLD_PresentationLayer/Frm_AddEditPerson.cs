using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD_PresentationLayer
{
    public partial class Frm_AddEditPerson : Form
    {
        private enum enMode { AddNew = 0, Update = 1 }
        private enMode _Mode = enMode.AddNew;
        private int _PersonID = -1;

        // Property بترجع الكارت جاهز من الـ ElementHost
        public UC_PersonCardAddEdit PersonCard => AddEdit_Element.Child as UC_PersonCardAddEdit;

        // 🟢 1. Constructor الإضافة
        public Frm_AddEditPerson()
        {
            InitializeComponent();

            _Mode = enMode.AddNew;
            _SubscribeToEvents();

            if (PersonCard != null)
            {
                PersonCard.ResetDefaultValues();
            }

            _UpdateFormHeader();
        }

        // 🔵 2. Constructor التعديل بـ PersonID
        public Frm_AddEditPerson(int personID)
        {
            InitializeComponent();

            _Mode = enMode.Update;
            _PersonID = personID;
            _SubscribeToEvents();

            if (PersonCard != null)
            {
                PersonCard.LoadPersonInfo(personID);
            }

            _UpdateFormHeader();
        }

        // 🟣 3. Constructor التعديل بـ NationalNo
        public Frm_AddEditPerson(string nationalNo)
        {
            InitializeComponent();

            _Mode = enMode.Update;
            _SubscribeToEvents();

            if (PersonCard != null)
            {
                PersonCard.LoadPersonInfo(nationalNo);
                _PersonID = PersonCard.PersonID; // جلب الـ ID بعد تحميل البيانات
            }

            _UpdateFormHeader();
        }

        private void Frm_AddEditPerson_Load(object sender, EventArgs e)
        {
            AddEdit_Element.Width = this.Width;
            AddEdit_Element.Height = this.Height;

            this.MaximizeBox = false;
        }

        // 🔗 الربط الموحد للـ Events
        private void _SubscribeToEvents()
        {
            if (PersonCard == null) return;

            // 🎯 عند نجاح الحفظ من الـ UserControl
            PersonCard.OnSaveCompleted += (sender, personID) =>
            {
                _Mode = enMode.Update;
                _PersonID = personID;

                // تحديث العنوان والأيقونة فوراً
                _UpdateFormHeader();
            };

            // ❌ عند الضغط على Cancel
            PersonCard.OnCancelClicked += (sender) =>
            {
                this.Close();
            };
        }

        // 🎨 دالة تحديث العنوان والأيقونة بحسب الـ Mode
        private void _UpdateFormHeader()
        {
            if (_Mode == enMode.AddNew)
            {
                this.Text = "Add New Person";
                this.Icon = new Icon("H:\\DVLD_Project\\DVLD-Project\\Icons\\AddNewPerson.ico");
            }
            else
            {
                this.Text = $"Edit Person";
                this.Icon = new Icon("H:\\DVLD_Project\\DVLD-Project\\Icons\\EditPerson.ico");
            }
        }

        private void AddEdit_Element_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }
    }
}