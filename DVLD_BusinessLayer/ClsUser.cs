using DVLD_DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BassemCore.Security;

namespace DVLD_BusinessLayer
{
    public class ClsUser
    {
        // 1. الـ Mode لتحديد حالة الكائن (إضافة أم تعديل)
        private enum enMode { AddNew = 0, Update = 1 };
        private enMode _Mode = enMode.AddNew;

        // متغير ساكن لتخزين العدد في الذاكرة (Caching)
        private static int _TotalUsersCount = -1;

        // متغيرات داخلية للـ Password وتتبع هل تم تغييرها أم لا
        private string _Password = "";
        private bool _PasswordChanged = false;

        // 2. الخصائص (Properties) الممثلة لجدول المستخدمين
        public int UserID { get; private set; }
        public int PersonID { get; set; }
        public string UserName { get; set; }

        // خاصية الـ Password الذكية لمنع التشفير المزدوج
        public string Password
        {
            get { return _Password; }
            set
            {
                if (_Password != value)
                {
                    _Password = value;
                    // لو الكائن في وضع التعديل وتم تغيير الباسورد، نفعّل العلامة
                    if (_Mode == enMode.Update)
                    {
                        _PasswordChanged = true;
                    }
                }
            }
        }

        public bool IsActive { get; set; }

        // 3. كائن (Object) من كلاس الأشخاص لربط بيانات الشخص بالمستخدم
        public ClsPerson PersonInfo { get; private set; }

        // خاصية لجلب الاسم الكامل مباشرة
        public string FullName
        {
            get
            {
                return (PersonInfo != null) ? PersonInfo.FullName : "";
            }
        }

        // خاصية لعرض حالة الحساب
        public string StatusText
        {
            get
            {
                return IsActive ? "Active" : "Inactive";
            }
        }

        // 3. الـ Constructor الفارغ (لإنشاء مستخدم جديد AddNew)
        public ClsUser()
        {
            this.UserID = -1;
            this.PersonID = -1;
            this.UserName = "";
            this._Password = "";
            this.IsActive = true;
            this.PersonInfo = null;

            _Mode = enMode.AddNew;
            _PasswordChanged = false;
        }

        // 4. الـ Constructor الداخلي (لشحن بيانات المستخدم عند استدعاء Find)
        private ClsUser(int UserID, int PersonID, string UserName, string Password, bool IsActive)
        {
            this.UserID = UserID;
            this.PersonID = PersonID;
            this.UserName = UserName;
            this._Password = Password; // الباسورد القادم من الداتابيز يكون مشفراً بالفعل
            this.IsActive = IsActive;

            // جلب بيانات الشخص المرتبط فوراً بناءً على الـ PersonID
            this.PersonInfo = ClsPerson.Find(PersonID);

            _Mode = enMode.Update;
            _PasswordChanged = false; // لم يتم تغييره بعد حتى الآن
        }

        // ==========================================
        // 5. عمليات البحث والقراءة وتسجيل الدخول
        // ==========================================

        public static ClsUser Find(int UserID)
        {
            int personID = -1;
            string userName = "";
            string password = "";
            bool isActive = false;

            if (ClsUserDataAccess.GetUserInfoByUserID(UserID, ref personID, ref userName, ref password, ref isActive))
            {
                return new ClsUser(UserID, personID, userName, password, isActive);
            }
            else
            {
                return null;
            }
        }

        public static ClsUser FindByUsername(string UserName)
        {
            int userID = -1;
            int personID = -1;
            string password = "";
            bool isActive = false;

            if (ClsUserDataAccess.GetUserInfoByUsername(UserName, ref userID, ref personID, ref password, ref isActive))
            {
                return new ClsUser(userID, personID, UserName, password, isActive);
            }
            else
            {
                return null;
            }
        }

        public static ClsUser FindByPersonID(int PersonID)
        {
            int userID = -1;
            string userName = "";
            string password = "";
            bool isActive = false;

            if (ClsUserDataAccess.GetUserInfoByPersonID(PersonID, ref userID, ref userName, ref password, ref isActive))
            {
                return new ClsUser(userID, PersonID, userName, password, isActive);
            }
            else
            {
                return null;
            }
        }

        // دالة تسجيل الدخول (نقوم بتشفير الباسورد المدخل أولاً لنقمل المقارنة مع الهاش المخزن)
        public static ClsUser FindByUsernameAndPassword(string UserName, string Password)
        {
            // تشفير كلمة المرور العادية التي كتبها المستخدم في شاشة اللوجن
            string hashedPassword = ClsEncryption.ComputeHash(Password);

            int userID = -1;
            int personID = -1;
            bool isActive = false;

            if (ClsUserDataAccess.GetUserInfoByUsernameAndPassword(UserName, hashedPassword, ref userID, ref personID, ref isActive))
            {
                return new ClsUser(userID, personID, UserName, hashedPassword, isActive);
            }
            else
            {
                return null;
            }
        }

        // ==========================================
        // 6. المطبخ الداخلي للـ Save (الإضافة والتعديل مع التشفير)
        // ==========================================

        private bool _AddNewUser()
        {
            // تشفير كلمة المرور قبل إرسالها لقاعدة البيانات
            string hashedPassword = ClsEncryption.ComputeHash(this._Password);

            this.UserID = ClsUserDataAccess.AddNewUser(this.PersonID, this.UserName, hashedPassword, this.IsActive);

            return (this.UserID != -1);
        }

        private bool _UpdateUser()
        {
            string passwordToSave = this._Password;

            // إذا تم تغيير كلمة المرور في الواجهة، نقوم بتشفير القيمة الجديدة فقط
            if (_PasswordChanged)
            {
                passwordToSave = ClsEncryption.ComputeHash(this._Password);
            }

            return ClsUserDataAccess.UpdateUser(this.UserID, this.PersonID, this.UserName, passwordToSave, this.IsActive);
        }

        public bool Save()
        {
            if (string.IsNullOrEmpty(this.UserName) || string.IsNullOrEmpty(this._Password) || this.PersonID == -1)
            {
                return false; // نرفض الحفظ لو البيانات الأساسية ناقصة
            }

            switch (_Mode)
            {
                case enMode.AddNew:
                    if (ClsUserDataAccess.IsPersonLinkedToUser(this.PersonID))
                        return false;

                    if (ClsUser.FindByUsername(this.UserName) != null)
                        return false;

                    if (_AddNewUser())
                    {
                        _Mode = enMode.Update;
                        _PasswordChanged = false;

                        // تحديث العداد محلياً بـ O(1)
                        if (_TotalUsersCount != -1)
                        {
                            _TotalUsersCount++;
                        }
                        else
                        {
                            _TotalUsersCount = ClsUserDataAccess.GetUsersCount();
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    ClsUser userWithSameUsername = ClsUser.FindByUsername(this.UserName);
                    if (userWithSameUsername != null && userWithSameUsername.UserID != this.UserID)
                    {
                        return false;
                    }

                    if (_UpdateUser())
                    {
                        _PasswordChanged = false; // إعادة تعيين العلامة بعد نجاح الحفظ
                        return true;
                    }
                    return false;
            }

            return false;
        }

        // ==========================================
        // 7. دوال الإدارة والبحث والعداد
        // ==========================================

        public static bool DeleteUser(int UserID)
        {
            if (ClsUserDataAccess.DeleteUser(UserID))
            {
                if (_TotalUsersCount != -1 && _TotalUsersCount > 0)
                {
                    _TotalUsersCount--;
                }
                else
                {
                    _TotalUsersCount = ClsUserDataAccess.GetUsersCount();
                }
                return true;
            }
            return false;
        }

        public static DataTable GetAllUsers()
        {
            return ClsUserDataAccess.GetAllUsers();
        }

        public static List<ClsUser> GetAllUsersList()
        {
            List<ClsUser> usersList = new List<ClsUser>();
            DataTable dt = ClsUserDataAccess.GetAllUsers();

            foreach (DataRow row in dt.Rows)
            {
                usersList.Add(new ClsUser(
                    Convert.ToInt32(row["UserID"]),
                    Convert.ToInt32(row["PersonID"]),
                    row["UserName"].ToString(),
                    row["Password"].ToString(),
                    Convert.ToBoolean(row["IsActive"])
                ));
            }

            return usersList;
        }

        public static DataTable GetUsersByStatus(bool IsActive)
        {
            return ClsUserDataAccess.GetUsersByStatus(IsActive);
        }

        // دالة تغيير كلمة المرور مباشرة (تقوم بتشفير الباسورد الجديد تلقائياً)
        public static bool ChangePassword(int UserID, string NewPassword)
        {
            string hashedPassword = ClsEncryption.ComputeHash(NewPassword);
            return ClsUserDataAccess.ChangePassword(UserID, hashedPassword);
        }

        public static bool ChangeUserStatus(int UserID, bool IsActive)
        {
            return ClsUserDataAccess.ChangeUserStatus(UserID, IsActive);
        }

        public static bool IsPersonLinkedToUser(int PersonID)
        {
            return ClsUserDataAccess.IsPersonLinkedToUser(PersonID);
        }

        public static DataTable GetAllUsersWithDetails()
        {
            return ClsUserDataAccess.GetAllUsersWithDetails();
        }

        public static bool IsUserExist(int UserID)
        {
            return ClsUserDataAccess.IsUserExist(UserID);
        }

        public static bool IsUserExist(string UserName)
        {
            return ClsUserDataAccess.IsUserExist(UserName);
        }

        public static bool IsUserExistForPersonID(int PersonID)
        {
            return ClsUserDataAccess.IsUserExistForPersonID(PersonID);
        }

        public static bool IsUserExist(string UserName, string Password)
        {
            string hashedPassword = ClsEncryption.ComputeHash(Password);
            return ClsUserDataAccess.IsUserExist(UserName, hashedPassword);
        }

        public static DataTable SearchStartsWith(string ColumnName, string Value)
        {
            return ClsUserDataAccess.SearchUsersStartsWith(ColumnName, Value);
        }

        public static DataTable SearchEndsWith(string ColumnName, string Value)
        {
            return ClsUserDataAccess.SearchUsersEndsWith(ColumnName, Value);
        }

        public static DataTable SearchContains(string ColumnName, string Value)
        {
            return ClsUserDataAccess.SearchUsersContains(ColumnName, Value);
        }

        // خاصية العداد الذكي بـ O(1)
        public static int TotalUsersCount
        {
            get
            {
                if (_TotalUsersCount == -1)
                {
                    _TotalUsersCount = ClsUserDataAccess.GetUsersCount();
                }
                return _TotalUsersCount;
            }
        }

        public static int GetUsersCount()
        {
            return ClsUserDataAccess.GetUsersCount();
        }

        public static bool IsPasswordCorrect(int userID, string password)
        {
            ClsUser user = ClsUser.Find(userID);

            if (user != null)
            {
                return (user.Password == ClsEncryption.ComputeHash(password)); // قم بتعديلها لو كنت تستخدم تشفير كلمات المرور (مثل Hashing)
            }

            return false;
        }

        public static int GetTodayUsersCount()
        {
            return ClsUserDataAccess.GetTodayUsersCount();
        }

        public override string ToString()
        {
            return this.UserName;
        }
    }
}