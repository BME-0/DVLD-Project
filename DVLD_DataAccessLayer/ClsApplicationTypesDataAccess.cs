using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccessLayer
{
    public static class ClsApplicationTypesDataAccess
    {
        // Private

        private static bool _ExecuteUpdateApplicationType(int ApplicationTypeID, string ApplicationTypeName, decimal Fees)
        {
            bool isUpdated = false;

            string query = @"UPDATE ApplicationTypes 
                     SET ApplicationTypeName = @ApplicationTypeName,
                         Fees = @Fees
                     WHERE ApplicationTypeID = @ApplicationTypeID";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                    command.Parameters.AddWithValue("@ApplicationTypeName", ApplicationTypeName);
                    command.Parameters.AddWithValue("@Fees", Fees);

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

        private static bool _ExecuteFindApplicationTypeByID(int ApplicationTypeID, ref string ApplicationTypeName, ref decimal Fees)
        {
            bool isFound = false;
            string query = "SELECT * FROM ApplicationTypes WHERE ApplicationTypeID = @ApplicationTypeID";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                ApplicationTypeName = (string)reader["ApplicationTypeName"];
                                Fees = (decimal)reader["Fees"];
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

        private static bool _ExecuteFindApplicationTypeByName(string ApplicationTypeName, ref int ApplicationTypeID, ref decimal Fees)
        {
            bool isFound = false;
            string query = "SELECT * FROM ApplicationTypes WHERE ApplicationTypeName = @ApplicationTypeName";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationTypeName", ApplicationTypeName);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                ApplicationTypeID = (int)reader["ApplicationTypeID"];
                                Fees = (decimal)reader["Fees"];
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

        private static DataTable _ExecuteGetAllApplicationTypes()
        {
            DataTable dt = new DataTable();
            string query = "SELECT * FROM ApplicationTypes ORDER BY ApplicationTypeID";

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

        private static bool _ExecuteIsApplicationTypeExist(int ApplicationTypeID)
        {
            bool isFound = false;
            string query = "SELECT 1 FROM ApplicationTypes WHERE ApplicationTypeID = @ApplicationTypeID";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);

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

        private static bool _ExecuteIsApplicationTypeExist(string ApplicationTypeName)
        {
            bool isFound = false;
            string query = "SELECT 1 FROM ApplicationTypes WHERE ApplicationTypeName = @ApplicationTypeName";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationTypeName", ApplicationTypeName);

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

        private static DataTable _ExecuteSearchApplicationTypesStartsWith(string ColumnName, string Value)
        {
            DataTable dt = new DataTable();

            // نتحقق من اسم العمود لمنع أي SQL Injection (حماية إضافية)
            if (ColumnName != "ApplicationTypeID" && ColumnName != "ApplicationTypeName")
            {
                ColumnName = "ApplicationTypeName";
            }

            string query = $@"SELECT ApplicationTypeID, ApplicationTypeName, Fees 
                     FROM ApplicationTypes 
                     WHERE {ColumnName} LIKE @Value;";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // إذا كان البحث برقم المعاملة، نتحقق، وإلا نبحث بالنص
                    if (ColumnName == "ApplicationTypeID")
                    {
                        command.Parameters.AddWithValue("@Value", Value + "%");
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@Value", Value + "%");
                    }

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

        private static DataTable _ExecuteSearchApplicationTypesEndsWith(string ColumnName, string Value)
        {
            DataTable dt = new DataTable();

            if (ColumnName != "ApplicationTypeID" && ColumnName != "ApplicationTypeName")
            {
                ColumnName = "ApplicationTypeName";
            }

            string query = $@"SELECT ApplicationTypeID, ApplicationTypeName, Fees 
                     FROM ApplicationTypes 
                     WHERE {ColumnName} LIKE @Value;";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Value", "%" + Value);

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

        private static DataTable _ExecuteSearchApplicationTypesContains(string ColumnName, string Value)
        {
            DataTable dt = new DataTable();

            if (ColumnName != "ApplicationTypeID" && ColumnName != "ApplicationTypeName")
            {
                ColumnName = "ApplicationTypeName";
            }

            string query = $@"SELECT ApplicationTypeID, ApplicationTypeName, Fees 
                     FROM ApplicationTypes 
                     WHERE {ColumnName} LIKE @Value;";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Value", "%" + Value + "%");

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

        private static int _ExecuteGetApplicationTypesCount()
        {
            int count = 0;
            string query = "SELECT COUNT(*) FROM ApplicationTypes";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedCount))
                        {
                            count = insertedCount;
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

        // Public

        public static bool UpdateApplicationType(int ApplicationTypeID, string ApplicationTypeName, decimal Fees)
        {
            return _ExecuteUpdateApplicationType(ApplicationTypeID, ApplicationTypeName, Fees);
        }

        public static bool GetApplicationTypeInfoByID(int ApplicationTypeID, ref string ApplicationTypeName, ref decimal Fees)
        {
            return _ExecuteFindApplicationTypeByID(ApplicationTypeID, ref ApplicationTypeName, ref Fees);
        }

        public static bool GetApplicationTypeInfoByName(string ApplicationTypeName, ref int ApplicationTypeID, ref decimal Fees)
        {
            return _ExecuteFindApplicationTypeByName(ApplicationTypeName, ref ApplicationTypeID, ref Fees);
        }

        public static DataTable GetAllApplicationTypes()
        {
            return _ExecuteGetAllApplicationTypes();
        }

        public static bool IsApplicationTypeExist(int ApplicationTypeID)
        {
            return _ExecuteIsApplicationTypeExist(ApplicationTypeID);
        }

        public static bool IsApplicationTypeExist(string ApplicationTypeName)
        {
            return _ExecuteIsApplicationTypeExist(ApplicationTypeName);
        }

        public static DataTable SearchApplicationTypesStartsWith(string ColumnName, string Value)
        {
            return _ExecuteSearchApplicationTypesStartsWith(ColumnName, Value);
        }

        public static DataTable SearchApplicationTypesEndsWith(string ColumnName, string Value)
        {
            return _ExecuteSearchApplicationTypesEndsWith(ColumnName, Value);
        }

        public static DataTable SearchApplicationTypesContains(string ColumnName, string Value)
        {
            return _ExecuteSearchApplicationTypesContains(ColumnName, Value);
        }

        public static int GetApplicationTypesCount()
        {
            return _ExecuteGetApplicationTypesCount();
        }
    }
}
