using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccessLayer
{
    public static class ClsTestTypesDataAccess
    {
        // Private

        private static bool _ExecuteUpdateTestType(int TestTypeID, string TestTypeName, string TestTypeDescription, decimal Fees)
        {
            bool isUpdated = false;

            string query = @"UPDATE TestTypes 
                     SET TestTypeName = @TestTypeName,
                         TestTypeDescription = @TestTypeDescription,
                         TestTypeFees = @Fees
                     WHERE TestTypeID = @TestTypeID";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
                    command.Parameters.AddWithValue("@TestTypeName", TestTypeName);
                    command.Parameters.AddWithValue("@TestTypeDescription", TestTypeDescription);
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

        private static bool _ExecuteFindTestTypeByID(int TestTypeID, ref string TestTypeName, ref string TestTypeDescription, ref decimal Fees)
        {
            bool isFound = false;
            string query = "SELECT * FROM TestTypes WHERE TestTypeID = @TestTypeID";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                TestTypeName = (string)reader["TestTypeName"];
                                TestTypeDescription = (string)reader["TestTypeDescription"];
                                Fees = (decimal)reader["TestTypeFees"];
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

        private static bool _ExecuteFindTestTypeByName(string TestTypeName, ref int TestTypeID, ref string TestTypeDescription, ref decimal Fees)
        {
            bool isFound = false;
            string query = "SELECT * FROM TestTypes WHERE TestTypeName = @TestTypeName";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestTypeName", TestTypeName);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                TestTypeID = (int)reader["TestTypeID"];
                                TestTypeDescription = (string)reader["TestTypeDescription"];
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

        private static DataTable _ExecuteGetAllTestTypes()
        {
            DataTable dt = new DataTable();
            string query = "SELECT * FROM TestTypes ORDER BY TestTypeID";

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

        private static bool _ExecuteIsTestTypeExist(int TestTypeID)
        {
            bool isFound = false;
            string query = "SELECT 1 FROM TestTypes WHERE TestTypeID = @TestTypeID";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

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

        private static bool _ExecuteIsTestTypeExist(string TestTypeName)
        {
            bool isFound = false;
            string query = "SELECT 1 FROM TestTypes WHERE TestTypeName = @TestTypeName";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TestTypeName", TestTypeName);

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

        private static DataTable _ExecuteSearchTestTypeStartsWith(string ColumnName, string Value)
        {
            DataTable dt = new DataTable();

            // نتحقق من اسم العمود لمنع أي SQL Injection (حماية إضافية)
            if (ColumnName != "TestTypeID" && ColumnName != "TestTypeName")
            {
                ColumnName = "TestTypeName";
            }

            string query = $@"SELECT TestTypeID, TestTypeName,TestTypeDescription, Fees 
                     FROM TestTypes 
                     WHERE {ColumnName} LIKE @Value;";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // إذا كان البحث برقم المعاملة، نتحقق، وإلا نبحث بالنص
                    if (ColumnName == "TestTypeID")
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

        private static DataTable _ExecuteSearchTestTypeEndsWith(string ColumnName, string Value)
        {
            DataTable dt = new DataTable();

            if (ColumnName != "TestTypeID" && ColumnName != "TestTypeName")
            {
                ColumnName = "TestTypeName";
            }

            string query = $@"SELECT TestTypeID, TestTypeName,TestTypeDescription, Fees 
                     FROM TestTypes 
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

        private static DataTable _ExecuteSearchTestTypeContains(string ColumnName, string Value)
        {
            DataTable dt = new DataTable();

            if (ColumnName != "TestTypeID" && ColumnName != "TestTypeName")
            {
                ColumnName = "TestTypeName";
            }

            string query = $@"SELECT TestTypeID, TestTypeName,TestTypeDescription, Fees 
                     FROM TestTypes 
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

        private static int _ExecuteGetTestTypeCount()
        {
            int count = 0;
            string query = "SELECT COUNT(*) FROM TestTypes";

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


        // public

        public static bool UpdateTestType(int TestTypeID, string TestTypeName, string TestTypeDescription, decimal Fees)
        {
            return _ExecuteUpdateTestType(TestTypeID, TestTypeName, TestTypeDescription, Fees);
        }

        public static bool FindTestTypeByID(int TestTypeID, ref string TestTypeName, ref string TestTypeDescription, ref decimal Fees)
        {
            return _ExecuteFindTestTypeByID(TestTypeID,ref TestTypeName, ref TestTypeDescription,ref Fees);
        }

        public static bool FindTestTypeByName(string TestTypeName, ref int TestTypeID, ref string TestTypeDescription, ref decimal Fees)
        {
            return _ExecuteFindTestTypeByName(TestTypeName, ref TestTypeID, ref TestTypeDescription, ref Fees);
        }

        public static DataTable GetAllTestTypes()
        {
            return _ExecuteGetAllTestTypes();
        }

        public static bool IsTestTypeExist(int TestTypeID)
        {
            return _ExecuteIsTestTypeExist(TestTypeID);
        }

        public static bool IsTestTypeExist(string TestTypeName)
        {
            return _ExecuteIsTestTypeExist(TestTypeName);
        }

        public static DataTable SearchTestTypeStartsWith(string ColumnName, string Value)
        {
            return _ExecuteSearchTestTypeStartsWith(ColumnName, Value);
        }

        public static DataTable SearchTestTypeEndsWith(string ColumnName, string Value)
        {
            return _ExecuteSearchTestTypeEndsWith(ColumnName, Value);
        }

        public static DataTable SearchTestTypeContains(string ColumnName, string Value)
        {
            return _ExecuteSearchTestTypeContains(ColumnName, Value);
        }

        public static int GetTestTypeCount()
        {
            return _ExecuteGetTestTypeCount();
        }

    }
}
