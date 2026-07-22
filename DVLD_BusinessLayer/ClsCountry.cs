using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLD_DataAccessLayer;

namespace DVLD_BusinessLayer
{
    public class ClsCountry
    {
        // 1. الـ Mode والخصائص
        private enum enMode { AddNew = 0, Update = 1 };
        private enMode _Mode = enMode.AddNew;

        public int CountryID { get; private set; }
        public string CountryName_En { get; set; }
        public string CountryName_Ar { get; set; }

        // 2. الـ Constructor الفارغ (لإضافة دولة جديدة)
        public ClsCountry()
        {
            this.CountryID = -1;
            this.CountryName_En = "";
            this.CountryName_Ar = "";
            _Mode = enMode.AddNew;
        }

        // 3. الـ Constructor الداخلي (لشحن البيانات عند البحث)
        private ClsCountry(int CountryID, string CountryName_En, string CountryName_Ar)
        {
            this.CountryID = CountryID;
            this.CountryName_En = CountryName_En;
            this.CountryName_Ar = CountryName_Ar;
            _Mode = enMode.Update; // طالما ليه ID يبقى جاي من الداتا بيز وجاهز للتعديل
        }

        // ==========================================
        // 4. عمليات البحث والقراءة (تُرجع Object)
        // ==========================================

        public static ClsCountry Find(int CountryID)
        {
            string CountryName_En = "";
            string CountryName_Ar = "";

            if (ClsCountryDataAccess.FindByID(CountryID, ref CountryName_En, ref CountryName_Ar))
            {
                return new ClsCountry(CountryID, CountryName_En, CountryName_Ar);
            }
            else
            {
                return null; // لو ملهاش وجود يرجع null
            }
        }

        public static ClsCountry FindByCountryName_En(string CountryName_En)
        {
            int CountryID = -1;
            string CountryName_Ar = "";

            if (ClsCountryDataAccess.FindByCountryName_En(CountryName_En, ref CountryID, ref CountryName_Ar))
            {
                return new ClsCountry(CountryID, CountryName_En, CountryName_Ar);
            }
            else
            {
                return null;
            }
        }

        public static ClsCountry FindByCountryName_Ar(string CountryName_Ar)
        {
            int CountryID = -1;
            string CountryName_En = "";

            if (ClsCountryDataAccess.FindByCountryName_Ar(CountryName_Ar, ref CountryID, ref CountryName_En))
            {
                return new ClsCountry(CountryID, CountryName_En, CountryName_Ar);
            }
            else
            {
                return null;
            }
        }

        // ==========================================
        // 5. المطبخ الداخلي للـ Save (الحفظ والتعديل)
        // ==========================================

        private bool _AddNewCountry()
        {
            this.CountryID = ClsCountryDataAccess.AddNewCountry(this.CountryName_En, this.CountryName_Ar);
            return (this.CountryID != -1);
        }

        private bool _UpdateCountry()
        {
            return ClsCountryDataAccess.UpdateCountry(this.CountryID, this.CountryName_En, this.CountryName_Ar);
        }

        // الدالة السحرية اللي بيستخدمها الـ UI فوراً
        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    if (_AddNewCountry())
                    {
                        _Mode = enMode.Update; // اقلب الحالة لـ Update بعد نجاح الإضافة
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdateCountry();
            }

            return false;
        }

        // ==========================================
        // 6. عمليات الحذف والـ الفلترة (تُرجع DataTable أو bool)
        // ==========================================

        public static bool DeleteCountry(int CountryID)
        {
            return ClsCountryDataAccess.DeleteCountry(CountryID);
        }

        public static DataTable GetAllCountries()
        {
            return ClsCountryDataAccess.GetAllCountries();
        }

        public static DataTable SearchWithStart(string SearchText)
        {
            return ClsCountryDataAccess.SearchWithStart(SearchText);
        }

        public static DataTable SearchContains(string SearchText)
        {
            return ClsCountryDataAccess.SearchContains(SearchText);
        }

        public static bool IsCountryExistByID(int CountryID)
        {
            return ClsCountryDataAccess.IsCountryExistByID(CountryID);
        }

        public static bool IsCountryExistByName(string CountryName)
        {
            return ClsCountryDataAccess.IsCountryExistByName(CountryName);
        }
    }
}
