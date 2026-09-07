using System;
using System.Data;
using DVLD_DataAccessLayer;

public class ClsApplicationType
{
    private enum enMode { AddNew = 0, Update = 1 };
    private enMode _Mode = enMode.Update; // افتراضياً Update لأننا لا نضيف سجلات جديدة من هنا

    public int ApplicationTypeID { get;private set; }
    public string ApplicationTypeName { get; set; }
    public decimal Fees { get; set; }

    // Constructor افتراضي
    public ClsApplicationType()
    {
        this.ApplicationTypeID = -1;
        this.ApplicationTypeName = "";
        this.Fees = 0;
        _Mode = enMode.Update;
    }

    // Constructor خاص لجلب البيانات
    private ClsApplicationType(int applicationTypeID, string applicationTypeName, decimal fees)
    {
        this.ApplicationTypeID = applicationTypeID;
        this.ApplicationTypeName = applicationTypeName;
        this.Fees = fees;
        _Mode = enMode.Update;
    }

    // دالة التحديث الفعلية
    private bool _UpdateApplicationType()
    {
        return ClsApplicationTypesDataAccess.UpdateApplicationType(this.ApplicationTypeID, this.ApplicationTypeName, this.Fees);
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
                return _UpdateApplicationType();
        }

        return false;
    }

    // البحث بالـ ID
    public static ClsApplicationType Find(int ApplicationTypeID)
    {
        string ApplicationTypeName = "";
        decimal Fees = 0;

        if (ClsApplicationTypesDataAccess.GetApplicationTypeInfoByID(ApplicationTypeID, ref ApplicationTypeName, ref Fees))
        {
            return new ClsApplicationType(ApplicationTypeID, ApplicationTypeName, Fees);
        }
        else
        {
            return null;
        }
    }

    // البحث بالـ Name
    public static ClsApplicationType Find(string ApplicationTypeName)
    {
        int ApplicationTypeID = -1;
        decimal Fees = 0;

        if (ClsApplicationTypesDataAccess.GetApplicationTypeInfoByName(ApplicationTypeName, ref ApplicationTypeID, ref Fees))
        {
            return new ClsApplicationType(ApplicationTypeID, ApplicationTypeName, Fees);
        }
        else
        {
            return null;
        }
    }

    // جلب كل الأنواع
    public static DataTable GetAllApplicationTypes()
    {
        return ClsApplicationTypesDataAccess.GetAllApplicationTypes();
    }

    // التحقق من الوجود بالـ ID
    public static bool IsApplicationTypeExist(int ApplicationTypeID)
    {
        return ClsApplicationTypesDataAccess.IsApplicationTypeExist(ApplicationTypeID);
    }

    // التحقق من الوجود بالاسم
    public static bool IsApplicationTypeExist(string ApplicationTypeName)
    {
        return ClsApplicationTypesDataAccess.IsApplicationTypeExist(ApplicationTypeName);
    }

    // البحث المتقدم (Starts With)
    public static DataTable SearchApplicationTypesStartsWith(string ColumnName, string Value)
    {
        return ClsApplicationTypesDataAccess.SearchApplicationTypesStartsWith(ColumnName, Value);
    }

    // البحث المتقدم (Ends With)
    public static DataTable SearchApplicationTypesEndsWith(string ColumnName, string Value)
    {
        return ClsApplicationTypesDataAccess.SearchApplicationTypesEndsWith(ColumnName, Value);
    }

    // البحث المتقدم (Contains)
    public static DataTable SearchApplicationTypesContains(string ColumnName, string Value)
    {
        return ClsApplicationTypesDataAccess.SearchApplicationTypesContains(ColumnName, Value);
    }

    // عدد السجلات
    public static int GetApplicationTypesCount()
    {
        return ClsApplicationTypesDataAccess.GetApplicationTypesCount();
    }
}