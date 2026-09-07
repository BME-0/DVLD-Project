using System;
using System.Data;
using DVLD_DataAccessLayer;

namespace DVLD_BusinessLayer
{
    public class ClsLocalDrivingLicenseApplication
    {
        // Enums لتحديد وضع الكائن (جديد أو تحديث)
        private enum enMode { AddNew = 0, Update = 1 };
        private enMode _Mode = enMode.AddNew;

        public int LocalDrivingLicenseApplicationID { get; private set; }
        public int ApplicationID { get; set; }
        // يمكن إضافة خصائص جدول Applications الأساسية هنا لتسهيل الوصول إليها عند الحاجة
        public int ApplicantPersonID { get; set; }
        public DateTime ApplicationDate { get; set; }
        public int ApplicationTypeID { get; set; }
        public byte ApplicationStatus { get; set; }
        public decimal PaidFees { get; set; }
        public int CreatedByUserID { get; set; }

        public int LicenseClassID { get; set; }
        // خصائص مساعدة لربط الفئات أو الجداول الأخرى إذا أردت
        // public clsLicenseClass LicenseClassInfo { get; set; }

        // Constructor للإنشاء الجديد
        public ClsLocalDrivingLicenseApplication()
        {
            this.LocalDrivingLicenseApplicationID = -1;
            this.ApplicationID = -1;
            this.ApplicantPersonID = -1;
            this.ApplicationDate = DateTime.Now;
            this.ApplicationTypeID = -1;
            this.ApplicationStatus = 1; // New
            this.PaidFees = 0;
            this.CreatedByUserID = -1;
            this.LicenseClassID = -1;

            _Mode = enMode.AddNew;
        }

        // Constructorخاص بالتحديث أو جلب البيانات
        private ClsLocalDrivingLicenseApplication(int localDrivingLicenseApplicationID, int applicationID, int licenseClassID)
        {
            this.LocalDrivingLicenseApplicationID = localDrivingLicenseApplicationID;
            this.ApplicationID = applicationID;
            this.LicenseClassID = licenseClassID;

            // يمكنك هنا استدعاء كلاس الـ Applications العام لجلب باقي بيانات الـ ApplicationID إذا احتجت لتعبئتها

            _Mode = enMode.Update;
        }

        // دالة الإضافة الخاصة
        private bool _AddNewLocalDrivingLicenseApplication()
        {
            // استدعاء DAL للإضافة (ملاحظة: تأكد من تطابق ترتيب البارامترات مع دالة DAL)
            this.LocalDrivingLicenseApplicationID = ClsLocalDrivingLicenseApplicationsDataAccess.AddNewLocalDrivingLicenseApplication(
                this.ApplicantPersonID,
                this.ApplicationDate,
                this.ApplicationTypeID,
                this.ApplicationStatus,
                this.PaidFees,
                this.CreatedByUserID,
                this.LicenseClassID
            );

            return (this.LocalDrivingLicenseApplicationID != -1);
        }

        // دالة التحديث الخاصة
        private bool _UpdateLocalDrivingLicenseApplication()
        {
            return ClsLocalDrivingLicenseApplicationsDataAccess.UpdateLocalDrivingLicenseApplication(
                this.LocalDrivingLicenseApplicationID,
                this.LicenseClassID
            );
        }

        // دالة البحث بواسطة الـ ID المحلي
        public static ClsLocalDrivingLicenseApplication Find(int localDrivingLicenseApplicationID)
        {
            int applicationID = -1;
            int licenseClassID = -1;

            bool isFound = ClsLocalDrivingLicenseApplicationsDataAccess.Find(
                localDrivingLicenseApplicationID,
                ref applicationID,
                ref licenseClassID
            );

            if (isFound)
            {
                return new ClsLocalDrivingLicenseApplication(localDrivingLicenseApplicationID, applicationID, licenseClassID);
            }
            else
            {
                return null;
            }
        }

        // دالة البحث بواسطة الـ ApplicationID العام
        public static ClsLocalDrivingLicenseApplication FindByApplicationID(int applicationID)
        {
            int localDrivingLicenseApplicationID = -1;
            int licenseClassID = -1;

            bool isFound = ClsLocalDrivingLicenseApplicationsDataAccess.FindByApplicationID(
                applicationID,
                ref localDrivingLicenseApplicationID,
                ref licenseClassID
            );

            if (isFound)
            {
                return new ClsLocalDrivingLicenseApplication(localDrivingLicenseApplicationID, applicationID, licenseClassID);
            }
            else
            {
                return null;
            }
        }

        // دالة الحفظ (تحديد ما إذا كانت عملية إضافة أو تحديث)
        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    if (_AddNewLocalDrivingLicenseApplication())
                    {
                        _Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdateLocalDrivingLicenseApplication();
            }

            return false;
        }

        // جلب كل الطلبات كـ DataTable
        public static DataTable GetAllLocalDrivingLicenseApplications()
        {
            return ClsLocalDrivingLicenseApplicationsDataAccess.GetAllLocalDrivingLicenseApplications();
        }

        // حذف طلب
        public static bool Delete(int localDrivingLicenseApplicationID)
        {
            return ClsLocalDrivingLicenseApplicationsDataAccess.DeleteLocalDrivingLicenseApplication(localDrivingLicenseApplicationID);
        }

        // التحقق من وجود الطلب
        public static bool IsExist(int localDrivingLicenseApplicationID)
        {
            return ClsLocalDrivingLicenseApplicationsDataAccess.IsLocalDrivingLicenseApplicationExist(localDrivingLicenseApplicationID);
        }

        // حساب عدد الطلبات الكلي
        public static int GetCount()
        {
            return ClsLocalDrivingLicenseApplicationsDataAccess.GetLocalDrivingLicenseApplicationsCount();
        }

        // التحقق مما إذا كان الشخص لديه نفس فئة الرخصة بشكل نشط
        public static bool IsLicenseClassActiveForPerson(int applicantPersonID, int licenseClassID)
        {
            return ClsLocalDrivingLicenseApplicationsDataAccess.IsLicenseClassActiveForPerson(applicantPersonID, licenseClassID);
        }

        // معرفة عدد الاختبارات التي اجتازها هذا الطلب المحلي
        public int GetPassedTestCount()
        {
            return ClsLocalDrivingLicenseApplicationsDataAccess.GetPassedTestCount(this.LocalDrivingLicenseApplicationID);
        }

        // دالة عامة ساكنة أيضاً للحصول على عدد الاختبارات مباشرة برقم الطلب
        public static int GetPassedTestCount(int localDrivingLicenseApplicationID)
        {
            return ClsLocalDrivingLicenseApplicationsDataAccess.GetPassedTestCount(localDrivingLicenseApplicationID);
        }
    }
}