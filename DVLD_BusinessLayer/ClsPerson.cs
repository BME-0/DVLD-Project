using DVLD_DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_BusinessLayer
{
    public class ClsPerson
    {
        // 1. الـ Mode لتحديد حالة الكائن (إضافة أم تعديل)
        private enum enMode { AddNew = 0, Update = 1 };
        private enMode _Mode = enMode.AddNew;

        // 2. الخصائص (Properties) الممثلة لجدول الأشخاص
        public int PersonID { get; private set; }
        public string NationalNo { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string ThirdName { get; set; } // يقبل NULL في الداتا بيز (محمي بـ "")
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }   // يقبل NULL في الداتا بيز (محمي بـ "")
        public string Email { get; set; }    // يقبل NULL في الداتا بيز (محمي بـ "")
        public int NationalityCountryID { get; set; }
        public string ImagePath { get; set; } // يقبل NULL في الداتا بيز (محمي بـ "")

        public bool Gender {  get; set; } // Male = 0 & Female = 1

        // تريكة الـ Read-Only Property: بتريحك جداً في الشاشات لما تحب تعرض الاسم الكامل مرة واحدة
        public string FullName
        {
            get { return FirstName + " " + SecondName + " " + (string.IsNullOrEmpty(ThirdName) ? "" : ThirdName + " ") + LastName; }
        }

        public string CountryName
        {
            get
            {
                // بنشيك الأول لو الـ ID صالح ومش بـ -1
                if (this.NationalityCountryID != -1)
                {
                    ClsCountry country = ClsCountry.Find(this.NationalityCountryID);

                    // لو لقى الدولة يرجع اسمها (تقدر تختار CountryName_Ar لو واجهتك عربي)
                    return (country != null) ? country.CountryName_En : "";
                }
                return "";
            }
        }

        public string GenderName
        {
            get
            {
                return (Gender == false) ? "Male" : "Female";
            }
        }

        public int Age
        {
            get
            {
                int age = DateTime.Now.Year - this.DateOfBirth.Year;
                // لو لسه يوم ميلاده مجاش السنة دي، بنقص سنة
                if (DateTime.Now < this.DateOfBirth.AddYears(age)) age--;
                return age;
            }
        }

        // 3. الـ Constructor الفارغ (لإنشاء شخص جديد AddNew)
        public ClsPerson()
        {
            this.PersonID = -1;
            this.NationalNo = "";
            this.FirstName = "";
            this.SecondName = "";
            this.ThirdName = "";
            this.LastName = "";
            this.DateOfBirth = DateTime.Now;
            this.Address = "";
            this.Phone1 = "";
            this.Phone2 = "";
            this.Email = "";
            this.NationalityCountryID = -1;
            this.ImagePath = "";
            this.Gender = false;

            _Mode = enMode.AddNew;
        }

        // 4. الـ Constructor الداخلي (لشحن بيانات الشخص عند استدعاء Find)
        private ClsPerson(int PersonID, string NationalNo, string FirstName, string SecondName,
        string ThirdName, string LastName, DateTime DateOfBirth, string Address,
        string Phone1, string Phone2, string Email, int NationalityCountryID, string ImagePath,bool Gendor)
        {
            this.PersonID = PersonID;
            this.NationalNo = NationalNo;
            this.FirstName = FirstName;
            this.SecondName = SecondName;
            this.Gender = Gendor;

            // تأمين الحقول التي تقبل NULL في قاعدة البيانات
            this.ThirdName = ThirdName ?? "";
            this.LastName = LastName;
            this.DateOfBirth = DateOfBirth;
            this.Address = Address;
            this.Phone1 = Phone1;
            this.Phone2 = Phone2 ?? "";
            this.Email = Email ?? "";
            this.NationalityCountryID = NationalityCountryID;
            this.ImagePath = ImagePath ?? "";

            _Mode = enMode.Update;
        }

        // ==========================================
        // 5. عمليات البحث والقراءة (ترجع Object من الكلاس)
        // ==========================================

        public static ClsPerson Find(int PersonID)
        {
            string NationalNo = "", FirstName = "", SecondName = "", ThirdName = "", LastName = "", Address = "", Phone1 = "", Phone2 = "", Email = "", ImagePath = "";
            DateTime DateOfBirth = DateTime.Now;
            int NationalityCountryID = -1;
            bool Gednor = false;

            // استدعاء دالة الـ DAL المأمنة بالـ ref والـ DBNull
            if (ClsPersonDataAccess.FindByPersonID(PersonID, ref NationalNo, ref FirstName, ref SecondName,
                ref ThirdName, ref LastName, ref DateOfBirth, ref Address, ref Phone1, ref Phone2,
                ref Email, ref NationalityCountryID, ref ImagePath,ref Gednor))
            {
                return new ClsPerson(PersonID, NationalNo, FirstName, SecondName, ThirdName, LastName,
                    DateOfBirth, Address, Phone1, Phone2, Email, NationalityCountryID, ImagePath, Gednor);
            }
            else
            {
                return null; // الشخص غير موجود
            }
        }

        public static ClsPerson FindByNationalNo(string NationalNo)
        {
            string FirstName = "", SecondName = "", ThirdName = "", LastName = "", Address = "", Phone1 = "", Phone2 = "", Email = "", ImagePath = "";
            int PersonID = -1;
            DateTime DateOfBirth = DateTime.Now;
            int NationalityCountryID = -1;
            bool Gednor = false;

            if (ClsPersonDataAccess.FindByNationalNo(NationalNo, ref PersonID, ref FirstName, ref SecondName,
                ref ThirdName, ref LastName, ref DateOfBirth, ref Address, ref Phone1, ref Phone2,
                ref Email, ref NationalityCountryID, ref ImagePath,ref Gednor))
            {
                return new ClsPerson(PersonID, NationalNo, FirstName, SecondName, ThirdName, LastName,
                    DateOfBirth, Address, Phone1, Phone2, Email, NationalityCountryID, ImagePath, Gednor);
            }
            else
            {
                return null;
            }
        }

        // ==========================================
        // 6. المطبخ الداخلي للـ Save (الإضافة والتعديل)
        // ==========================================

        private bool _AddNewPerson()
        {
            // بنباصي البيانات للـ DAL وبنستقبل الـ ID الجديد المتولد تلقائياً من الـ Database
            this.PersonID = ClsPersonDataAccess.AddNewPerson(this.NationalNo, this.FirstName, this.SecondName,
                this.ThirdName, this.LastName, this.DateOfBirth, this.Address, this.Phone1, this.Phone2,
                this.Email, this.NationalityCountryID, this.ImagePath, this.Gender);

            return (this.PersonID != -1);
        }

        private bool _UpdatePerson()
        {
            return ClsPersonDataAccess.UpdatePerson(this.PersonID, this.NationalNo, this.FirstName, this.SecondName,
                this.ThirdName, this.LastName, this.DateOfBirth, this.Address, this.Phone1, this.Phone2,
                this.Email, this.NationalityCountryID, this.ImagePath,this.Gender);
        }

        // الدالة الذهبية اللي الشاشات بتناديها فوراً بدون ما توجع دماغها باللوجيك الداخلي
        public bool Save()
        {

            switch (_Mode)
            {
                case enMode.AddNew:

                    if (ClsPersonDataAccess.IsPersonExistByNationalNo(this.NationalNo))
                    {
                        return false; // أو تقدر ترمي Exception مخصصة تشرح السبب
                    }

                    if (_AddNewPerson())
                    {
                        _Mode = enMode.Update; // اقلب الحالة لتعديل بعد نجاح الحفظ الأول
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:

                    // 2. في التعديل (الحارس الذكي): 
                    // هنروح نبحث في الداتا بيز عن الشخص اللي شايل الرقم الوطني المكتوب ده حالياً
                    ClsPerson personWithSameNationalNo = ClsPerson.FindByNationalNo(this.NationalNo);

                    // لو لقينا شخص عنده نفس الرقم الوطني، وبصينا على الـ ID بتاعه لقيناه مختلف عن ID الشخص الحالي!
                    // ده معناه إن الموظف بيحاول يسرق رقم وطني بتاع شخص تاني! نرفض فوراً.
                    if (personWithSameNationalNo != null && personWithSameNationalNo.PersonID != this.PersonID)
                    {
                        return false; // نرفض التعديل ونحمى السيستم
                    }

                    return _UpdatePerson();
            }

            return false;
        }

        // ==========================================
        // 7. ترسانة الحذف، العرض، والبحث المرن (تتعامل مع الـ UI مباشرة)
        // ==========================================

        public static bool DeletePerson(int PersonID)
        {
            if (IsPersonLinkedToUser(PersonID) || IsPersonLinkedToDriver(PersonID))
            {
                return false;
            }

            return ClsPersonDataAccess.DeletePerson(PersonID);
        }

        public static bool DeleteByNationalNo(string NationalNo)
        {
            // 1. البحث عن الشخص بالرقم الوطني لجلب الـ ID بتاعه
            ClsPerson Person = ClsPerson.FindByNationalNo(NationalNo);

            // لو الشخص مش موجود أصلاً في السيستم، نرجع false
            if (Person == null)
            {
                return false;
            }

            // 2. الحارس الذكي: نشيك لو الـ PersonID ده مربوط بـ User
            if (IsPersonLinkedToUser(Person.PersonID) || IsPersonLinkedToDriver(Person.PersonID)) 
            {
                return false; // نرفض الحذف لحماية النظام من الـ Constraint Error
            }

            // 3. لو كله تمام ونظيف، نفذ الحذف فوراً من الداتا بيز
            return ClsPersonDataAccess.DeleteByNationalNo(NationalNo);
        }

        public static DataTable GetAllPeople()
        {
            return ClsPersonDataAccess.GetAllPeople();
        }

        public static DataTable GetAllPeopleWithCountryName()
        {
            return ClsPersonDataAccess.GetAllPeopleWithCountryName(); // دي المفضلة للـ DataGridView 
        }

        public static bool IsPersonExistByID(int PersonID)
        {
            return ClsPersonDataAccess.IsPersonExistByID(PersonID);
        }

        public static bool IsPersonExistByNationalNo(string NationalNo)
        {
            return ClsPersonDataAccess.IsPersonExistByNationalNo(NationalNo);
        }

        // --- دوال المقترحات الاحترافية التي قمنا بصياغتها بالـ DAL ---

        public static string GetFullName(int PersonID)
        {
            return ClsPersonDataAccess.GetFullName(PersonID);
        }

        public static string GetImagePath(int PersonID)
        {
            return ClsPersonDataAccess.GetImagePath(PersonID);
        }

        public static bool IsPersonLinkedToUser(int PersonID)
        {
            return ClsPersonDataAccess.IsPersonLinkedToUser(PersonID);
        }

        public static bool IsPersonLinkedToDriver(int PersonID)
        {
            return ClsPersonDataAccess.IsPersonLinkedToDriver(PersonID);
        }

        // --- ترسانة البحث المرن الثلاثية للـ UI الفوري ---

        public static DataTable SearchStartsWith(string ColumnName, string Value)
        {
            return ClsPersonDataAccess.SearchStartsWith(ColumnName, Value);
        }

        public static DataTable SearchEndsWith(string ColumnName, string Value)
        {
            return ClsPersonDataAccess.SearchEndsWith(ColumnName, Value);
        }

        public static DataTable SearchContains(string ColumnName, string Value)
        {
            return ClsPersonDataAccess.SearchContains(ColumnName, Value);
        }
    }
}
