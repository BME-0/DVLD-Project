using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccessLayer
{
    public static class ClsLocalDrivingLicenseApplicationsDataAccess
    {
        // Private Execution Methods

        private static bool _ExecuteGetLocalDrivingLicenseApplicationInfoByID(int LocalDrivingLicenseApplicationID, ref int ApplicationID, ref int LicenseClassID)
        {
            bool isFound = false;
            string query = "SELECT * FROM LocalDrivingLicenseApplications WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                ApplicationID = (int)reader["ApplicationID"];
                                LicenseClassID = (int)reader["LicenseClassID"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        isFound = false;
                    }
                }
            }

            return isFound;
        }

        private static DataTable _ExecuteGetAllLocalDrivingLicenseApplications()
        {
            DataTable dt = new DataTable();

            // استعلام يعرض البيانات مدمجة أو من خلال View جاهز
            string query = "SELECT * FROM LocalDrivingLicenseApplications ORDER BY LocalDrivingLicenseApplicationID ASC";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                dt.Load(reader);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        dt = new DataTable();
                    }
                }
            }

            return dt;
        }

        private static bool _ExecuteIsLicenseClassActiveForPerson(int ApplicationPersonID, int LicenseClassID)
        {
            bool isFound = false;

            // الاستعلام يبحث عن طلب لنفس الشخص ونفس الفئة بشرط أن يكون الطلب في حالة نشطة (مثلاً ApplicationStatus = 1 أي New)
            string query = @"SELECT 1 
                     FROM LocalDrivingLicenseApplications 
                     INNER JOIN Applications ON LocalDrivingLicenseApplications.ApplicationID = Applications.ApplicationID 
                     WHERE Applications.ApplicationPersonID = @ApplicationPersonID 
                       AND LocalDrivingLicenseApplications.LicenseClassID = @LicenseClassID 
                       AND Applications.ApplicationStatus = 1"; // 1 عادة تعني New / Active

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationPersonID", ApplicationPersonID);
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            isFound = true; // وجدنا أن الشخص لديه طلب بنفس الفئة وقيد التنفيذ بالفعل
                        }
                    }
                    catch (Exception ex)
                    {
                        isFound = false;
                    }
                }
            }

            return isFound;
        }

        private static bool _ExecuteGetLocalDrivingLicenseApplicationInfoByApplicationID(int ApplicationID, ref int LocalDrivingLicenseApplicationID, ref int LicenseClassID)
        {
            bool isFound = false;
            string query = "SELECT * FROM LocalDrivingLicenseApplications WHERE ApplicationID = @ApplicationID";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                LocalDrivingLicenseApplicationID = (int)reader["LocalDrivingLicenseApplicationID"];
                                LicenseClassID = (int)reader["LicenseClassID"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        isFound = false;
                    }
                }
            }

            return isFound;
        }

        private static int _ExecuteAddNewLocalDrivingLicenseApplication(int ApplicationPersonID, DateTime ApplicationDate, int ApplicationTypeID, int ApplicationStatus, decimal PaidFees, int CreatedByUserID, int LicenseClassID)
        {
            int localDrivingLicenseApplicationID = -1;

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. إدخال الطلب العام في جدول Applications أولاً والحصول على الـ ID
                        string queryApp = @"INSERT INTO Applications 
                                            (ApplicationPersonID, ApplicationDate, ApplicationTypeID, ApplicationStatus, PaidFees, CreatedByUserID)
                                            VALUES 
                                            (@ApplicationPersonID, @ApplicationDate, @ApplicationTypeID, @ApplicationStatus, @PaidFees, @CreatedByUserID);
                                            SELECT SCOPE_IDENTITY();";

                        int applicationID = -1;
                        using (SqlCommand cmdApp = new SqlCommand(queryApp, connection, transaction))
                        {
                            cmdApp.Parameters.AddWithValue("@ApplicationPersonID", ApplicationPersonID);
                            cmdApp.Parameters.AddWithValue("@ApplicationDate", ApplicationDate);
                            cmdApp.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                            cmdApp.Parameters.AddWithValue("@ApplicationStatus", ApplicationStatus);
                            cmdApp.Parameters.AddWithValue("@PaidFees", PaidFees);
                            cmdApp.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                            object result = cmdApp.ExecuteScalar();
                            if (result != null && int.TryParse(result.ToString(), out int insertedAppID))
                            {
                                applicationID = insertedAppID;
                            }
                        }

                        if (applicationID == -1)
                        {
                            transaction.Rollback();
                            return -1;
                        }

                        // 2. إدخال الطلب المحلي في جدول LocalDrivingLicenseApplications باستخدام الـ ApplicationID الناتج
                        string queryLocalApp = @"INSERT INTO LocalDrivingLicenseApplications 
                                                 (ApplicationID, LicenseClassID)
                                                 VALUES 
                                                 (@ApplicationID, @LicenseClassID);
                                                 SELECT SCOPE_IDENTITY();";

                        using (SqlCommand cmdLocal = new SqlCommand(queryLocalApp, connection, transaction))
                        {
                            cmdLocal.Parameters.AddWithValue("@ApplicationID", applicationID);
                            cmdLocal.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                            object resultLocal = cmdLocal.ExecuteScalar();
                            if (resultLocal != null && int.TryParse(resultLocal.ToString(), out int insertedLocalID))
                            {
                                localDrivingLicenseApplicationID = insertedLocalID;
                            }
                        }

                        if (localDrivingLicenseApplicationID == -1)
                        {
                            transaction.Rollback();
                            return -1;
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch { }
                        localDrivingLicenseApplicationID = -1;
                    }
                }
            }

            return localDrivingLicenseApplicationID;
        }

        private static bool _ExecuteUpdateLocalDrivingLicenseApplication(int LocalDrivingLicenseApplicationID, int LicenseClassID)
        {
            bool isUpdated = false;
            string query = @"UPDATE LocalDrivingLicenseApplications 
                             SET LicenseClassID = @LicenseClassID
                             WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                    command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        isUpdated = (rowsAffected > 0);
                    }
                    catch (Exception ex)
                    {
                        isUpdated = false;
                    }
                }
            }

            return isUpdated;
        }

        private static bool _ExecuteDeleteLocalDrivingLicenseApplication(int LocalDrivingLicenseApplicationID)
        {
            bool isDeleted = false;
            int applicationID = -1;
            int dummyLicenseClassID = -1; // متغير مؤقت بدلاً من الـ var و _

            // استدعاء الدالة مع تمرير المتغير المؤقت
            if (!_ExecuteGetLocalDrivingLicenseApplicationInfoByID(LocalDrivingLicenseApplicationID, ref applicationID, ref dummyLicenseClassID))
                return false;

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. الحذف من الجدول الفرعي أولاً
                        string queryLocal = "DELETE FROM LocalDrivingLicenseApplications WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";
                        using (SqlCommand cmdLocal = new SqlCommand(queryLocal, connection, transaction))
                        {
                            cmdLocal.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
                            cmdLocal.ExecuteNonQuery();
                        }

                        // 2. الحذف من الجدول العام Applications
                        string queryApp = "DELETE FROM Applications WHERE ApplicationID = @ApplicationID";
                        using (SqlCommand cmdApp = new SqlCommand(queryApp, connection, transaction))
                        {
                            cmdApp.Parameters.AddWithValue("@ApplicationID", applicationID);
                            cmdApp.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        isDeleted = true;
                    }
                    catch (Exception ex)
                    {
                        try { transaction.Rollback(); } catch { }
                        isDeleted = false;
                    }
                }
            }

            return isDeleted;
        }
       
        private static bool _ExecuteIsLocalDrivingLicenseApplicationExist(int LocalDrivingLicenseApplicationID)
        {
            bool isFound = false;
            string query = "SELECT 1 FROM LocalDrivingLicenseApplications WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            isFound = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        isFound = false;
                    }
                }
            }

            return isFound;
        }

        private static int _ExecuteGetLocalDrivingLicenseApplicationsCount()
        {
            int count = 0;
            string query = "SELECT COUNT(*) FROM LocalDrivingLicenseApplications";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int parsedCount))
                        {
                            count = parsedCount;
                        }
                    }
                    catch (Exception ex)
                    {
                        count = 0;
                    }
                }
            }

            return count;
        }

        private static int _ExecuteGetPassedTestCount(int LocalDrivingLicenseApplicationID)
        {
            int passedTestsCount = 0;
            string query = @"SELECT COUNT(TestAppointments.TestTypeID) AS PassedTestsCount
                     FROM TestAppointments 
                     INNER JOIN Tests ON TestAppointments.TestAppointmentID = Tests.TestAppointmentID
                     WHERE TestAppointments.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID 
                       AND Tests.TestResult = 1"; // 1 تعني Pass (اجتاز الاختبار)

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int count))
                        {
                            passedTestsCount = count;
                        }
                    }
                    catch (Exception ex)
                    {
                        passedTestsCount = 0;
                    }
                }
            }

            return passedTestsCount;
        }

        // Public Methods

        public static bool Find(int LocalDrivingLicenseApplicationID, ref int ApplicationID, ref int LicenseClassID)
        {
            return _ExecuteGetLocalDrivingLicenseApplicationInfoByID(LocalDrivingLicenseApplicationID, ref ApplicationID, ref LicenseClassID);
        }

        public static DataTable GetAllLocalDrivingLicenseApplications()
        {
            return _ExecuteGetAllLocalDrivingLicenseApplications();
        }

        public static int AddNewLocalDrivingLicenseApplication(int ApplicationPersonID, DateTime ApplicationDate, int ApplicationTypeID, int ApplicationStatus, decimal PaidFees, int CreatedByUserID, int LicenseClassID)
        {
            return _ExecuteAddNewLocalDrivingLicenseApplication(ApplicationPersonID, ApplicationDate, ApplicationTypeID, ApplicationStatus, PaidFees, CreatedByUserID, LicenseClassID);
        }

        public static bool UpdateLocalDrivingLicenseApplication(int LocalDrivingLicenseApplicationID, int LicenseClassID)
        {
            return _ExecuteUpdateLocalDrivingLicenseApplication(LocalDrivingLicenseApplicationID, LicenseClassID);
        }

        public static bool DeleteLocalDrivingLicenseApplication(int LocalDrivingLicenseApplicationID)
        {
            return _ExecuteDeleteLocalDrivingLicenseApplication(LocalDrivingLicenseApplicationID);
        }

        public static bool IsLocalDrivingLicenseApplicationExist(int LocalDrivingLicenseApplicationID)
        {
            return _ExecuteIsLocalDrivingLicenseApplicationExist(LocalDrivingLicenseApplicationID);
        }

        public static int GetLocalDrivingLicenseApplicationsCount()
        {
            return _ExecuteGetLocalDrivingLicenseApplicationsCount();
        }

        public static bool FindByApplicationID(int ApplicationID, ref int LocalDrivingLicenseApplicationID, ref int LicenseClassID)
        {
            return _ExecuteGetLocalDrivingLicenseApplicationInfoByApplicationID(ApplicationID, ref LocalDrivingLicenseApplicationID, ref LicenseClassID);
        }

        public static bool IsLicenseClassActiveForPerson(int ApplicationPersonID, int LicenseClassID)
        {
            return _ExecuteIsLicenseClassActiveForPerson(ApplicationPersonID, LicenseClassID);
        }

        public static int GetPassedTestCount(int LocalDrivingLicenseApplicationID)
        {
            return _ExecuteGetPassedTestCount(LocalDrivingLicenseApplicationID);
        }
    }
}