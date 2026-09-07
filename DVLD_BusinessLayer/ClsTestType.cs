using System;
using System.Data;
using DVLD_DataAccessLayer;

namespace DVLD_BusinessLayer
{
    public class ClsTestType
    {
        private enum enMode { AddNew = 0, Update = 1 };
        private enMode _Mode = enMode.Update; // افتراضياً Update لأننا لا نضيف سجلات جديدة من هنا

        public int TestTypeID { get; private set; }
        public string TestTypeName { get; set; }
        public string TestTypeDescription { get; set; }
        public decimal Fees { get; set; }

        // Constructor افتراضي
        public ClsTestType()
        {
            this.TestTypeID = -1;
            this.TestTypeName = "";
            this.TestTypeDescription = "";
            this.Fees = 0;

            _Mode = enMode.Update;
        }

        // Constructor خاص لجلب البيانات
        private ClsTestType(int TestTypeID, string TestTypeName, string TestTypeDescription, decimal fees)
        {
            this.TestTypeID = TestTypeID;
            this.TestTypeName = TestTypeName;
            this.TestTypeDescription = TestTypeDescription;
            this.Fees = fees;

            _Mode = enMode.Update;
        }

        // دالة التحديث الفعلية
        private bool _UpdateTestType()
        {
            return ClsTestTypesDataAccess.UpdateTestType(this.TestTypeID, this.TestTypeName,this.TestTypeDescription, this.Fees);
        }

        // دالة الـ Save (بما أننا نعدل فقط)
        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    // غير مسموح بالإضافة برمجياً حسب رغبتك
                    return false;

                case enMode.Update:
                    return _UpdateTestType();
            }

            return false;
        }

        // البحث بالـ ID
        public static ClsTestType Find(int TestTypeID)
        {
            string TestTypeName = "";
            string TestTypeDescription = "";
            decimal Fees = 0;

            if (ClsTestTypesDataAccess.FindTestTypeByID(TestTypeID,ref TestTypeName, ref TestTypeDescription, ref Fees))
            {
                return new ClsTestType(TestTypeID, TestTypeName, TestTypeDescription, Fees);
            }
            else
            {
                return null;
            }
        }

        // البحث بالـ Name
        public static ClsTestType Find(string TestTypeName)
        {
            int TestTypeID = -1;
            string TestTypeDescription = "";
            decimal Fees = 0;

            if (ClsTestTypesDataAccess.FindTestTypeByName(TestTypeName, ref TestTypeID,ref TestTypeDescription, ref Fees))
            {
                return new ClsTestType(TestTypeID, TestTypeName, TestTypeDescription, Fees);
            }
            else
            {
                return null;
            }
        }

        // جلب كل الأنواع
        public static DataTable GetAllTestTypes()
        {
            return ClsTestTypesDataAccess.GetAllTestTypes();
        }

        // التحقق من الوجود بالـ ID
        public static bool IsTestTypeExist(int TestTypeID)
        {
            return ClsTestTypesDataAccess.IsTestTypeExist(TestTypeID);
        }

        // التحقق من الوجود بالاسم
        public static bool IsTestTypeExist(string TestTypeName)
        {
            return ClsTestTypesDataAccess.IsTestTypeExist(TestTypeName);
        }

        // البحث المتقدم (Starts With)
        public static DataTable SearchTestTypesStartsWith(string ColumnName, string Value)
        {
            return ClsTestTypesDataAccess.SearchTestTypeStartsWith(ColumnName, Value);
        }

        // البحث المتقدم (Ends With)
        public static DataTable SearchTestTypesEndsWith(string ColumnName, string Value)
        {
            return ClsTestTypesDataAccess.SearchTestTypeEndsWith(ColumnName, Value);
        }

        // البحث المتقدم (Contains)
        public static DataTable SearchTestTypesContains(string ColumnName, string Value)
        {
            return ClsTestTypesDataAccess.SearchTestTypeContains(ColumnName, Value);
        }

        // عدد السجلات
        public static int GetTestTypesCount()
        {
            return ClsTestTypesDataAccess.GetTestTypeCount();
        }

    }
}
