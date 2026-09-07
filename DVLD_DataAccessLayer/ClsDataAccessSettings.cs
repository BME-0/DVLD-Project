using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccessLayer
{
    public static class ClsDataAccessSettings
    {
        public static string ConnectionString = "Server=.; Database=DVLD_Database; User Id=sa; Password=ASX@12345; TrustServerCertificate=True;";

        public static object ExecuteScalarQuery(string query)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        return command.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        // يمكنك تسجيل الخطأ هنا إذا كان لديك نظام Log
                        return null;
                    }
                }
            }
        }

    }
}
