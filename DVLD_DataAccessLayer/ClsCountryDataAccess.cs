using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DVLD_DataAccessLayer
{
    public static class ClsCountryDataAccess
    {
        // Private

        private static int _ExecuteAddNewCountry(string CountryName_En, string CountryName_Ar)
        {
            int NewCountryID = -1;

            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            string query = @"INSERT INTO Countries (CountryName_En, CountryName_Ar) 
                             VALUES (@CountryName_En, @CountryName_Ar);
                             SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(query, connection);

            // حماية الكود بالـ Parameters ضد الـ SQL Injection
            command.Parameters.AddWithValue("@CountryName_En", CountryName_En);
            command.Parameters.AddWithValue("@CountryName_Ar", CountryName_Ar);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int insertedId))
                {
                    NewCountryID = insertedId;
                }
            }
            catch (Exception ex)
            {
                // later you will write class error
                NewCountryID = -1;
            }
            finally
            {
                // ضمان قفل الاتصال مهما حصل
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return NewCountryID;
        }

        private static bool _ExecuteUpdateCountry(int CountryID, string CountryName_En, string CountryName_Ar)
        {
            bool isUpdated = false;

            // استخدام كلاس الـ Settings المركزي لفتح الاتصال
            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            string query = @"UPDATE Countries 
                             SET CountryName_En = @CountryName_En, CountryName_Ar = @CountryName_Ar 
                             WHERE CountryID = @CountryID";

            SqlCommand command = new SqlCommand(query, connection);

            // إضافة البارامترز لحماية الكود من الـ SQL Injection
            command.Parameters.AddWithValue("@CountryID", CountryID);
            command.Parameters.AddWithValue("@CountryName_En", CountryName_En);
            command.Parameters.AddWithValue("@CountryName_Ar", CountryName_Ar);

            try
            {
                connection.Open();

                // ExecuteNonQuery بترجع عدد الصفوف اللي اتأثرت بالتعديل
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    isUpdated = true; // التعديل سمع في الداتا بيز بنجاح
                }
            }
            catch (Exception ex)
            {
                isUpdated = false;
            }
            finally
            {
                // الـ Finally بتضمن قفل الاتصال مهما حصل جوه الـ try
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return isUpdated;
        }

        private static bool _ExecuteFindCountry(int CountryID, ref string CountryName_En, ref string CountryName_Ar)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);
            string query = "SELECT CountryID, CountryName_En, CountryName_Ar FROM Countries WHERE CountryID = @CountryID";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@CountryID", CountryID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                // لو لقى السجل بيقرأ البيانات ويحطها في متغيرات الـ ref ويقلب الحالة لـ true
                if (reader.Read())
                {
                    isFound = true;
                    CountryName_En = (string)reader["CountryName_En"];
                    CountryName_Ar = (string)reader["CountryName_Ar"];
                }

                reader.Close(); // قفل الـ Reader فوراً بعد القراءة
            }
            catch (Exception ex)
            {
                isFound = false;
            }
            finally
            {
                // الحارس الأمين اللي بيقفل الاتصال دايماً
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return isFound;
        }

        private static bool _ExecuteFindCountryByName_En(string CountryName_En, ref int CountryID, ref string CountryName_Ar)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);
            string query = "SELECT CountryID, CountryName_En, CountryName_Ar FROM Countries WHERE CountryName_En = @CountryName_En";
            SqlCommand command = new SqlCommand(query, connection);

            // حماية من الـ SQL Injection لأن المدخل نصي (هنا الثغرة بتبقى أخطر لو مفيش Parameter)
            command.Parameters.AddWithValue("@CountryName_En", CountryName_En);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    isFound = true;
                    CountryID = (int)reader["CountryID"];
                    CountryName_Ar = (string)reader["CountryName_Ar"];
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                isFound = false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return isFound;
        }

        private static bool _ExecuteFindCountryByName_Ar(string CountryName_Ar, ref int CountryID, ref string CountryName_En)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);
            string query = "SELECT CountryID, CountryName_En, CountryName_Ar FROM Countries WHERE CountryName_Ar = @CountryName_Ar";
            SqlCommand command = new SqlCommand(query, connection);

            // حماية تامة من الـ SQL Injection باستخدام الـ Parameter
            command.Parameters.AddWithValue("@CountryName_Ar", CountryName_Ar);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    isFound = true;
                    CountryID = (int)reader["CountryID"];
                    CountryName_En = (string)reader["CountryName_En"];
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                isFound = false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return isFound;
        }

        private static DataTable _ExecuteGetAllCountries()
        {
            DataTable dt = new DataTable();

            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);
            string query = "SELECT CountryID, CountryName_En, CountryName_Ar FROM Countries";
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    dt.Load(reader); // شحن الـ DataTable بالبيانات فوراً
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                // في حالة الخطأ، الـ DataTable هترجع فاضية بس السيستم مش هيضرب
            }
            finally
            {
                // الحارس الأمين اللي بيقفل الاتصال دايماً
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return dt;
        }

        private static DataTable _ExecuteSearchWithStart(string SearchText)
        {
            DataTable dt = new DataTable();

            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            // بنبحث في الاسمين (العربي والإنجليزي) عن أي دولة تبدأ بالنص المدخل
            string query = @"SELECT CountryID, CountryName_En, CountryName_Ar 
                             FROM Countries 
                             WHERE CountryName_En LIKE @SearchText + '%' 
                                OR CountryName_Ar LIKE @SearchText + '%'";

            SqlCommand command = new SqlCommand(query, connection);

            // حماية تامة من الـ SQL Injection: بنباصي النص كبارامتر عادي 
            // والـ SQL هو اللي هيدمج معاه الـ % بأمان تحت الأرض
            command.Parameters.AddWithValue("@SearchText", SearchText);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    dt.Load(reader);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                // في حالة الخطأ يرجع جدول فاضي
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return dt;
        }

        private static DataTable _ExecuteSearchContains(string SearchText)
        {
            DataTable dt = new DataTable();

            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            // بنحط % قبل وبعد الـ Parameter للبحث في أي مكان داخل النص
            string query = @"SELECT CountryID, CountryName_En, CountryName_Ar 
                             FROM Countries 
                             WHERE CountryName_En LIKE '%' + @SearchText + '%' 
                                OR CountryName_Ar LIKE '%' + @SearchText + '%'";

            SqlCommand command = new SqlCommand(query, connection);

            // حماية تامة من الـ SQL Injection
            command.Parameters.AddWithValue("@SearchText", SearchText);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    dt.Load(reader);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                // في حالة الخطأ يرجع جدول فاضي
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return dt;
        }

        private static bool _ExecuteIsCountryExistByID(int CountryID)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);
            string query = "SELECT 1 FROM Countries WHERE CountryID = @CountryID";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@CountryID", CountryID);

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
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return isFound;
        }

        private static bool _ExecuteDeleteCountry(int CountryID)
        {
            bool isDeleted = false;

            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            string query = "DELETE FROM Countries WHERE CountryID = @CountryID";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@CountryID", CountryID);

            try
            {
                connection.Open();

                // ExecuteNonQuery بترجع عدد الصفوف اللي اتمسحت فعلياً
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    isDeleted = true; // المسح تم بنجاح
                }
            }
            catch (Exception ex)
            {
                // لو حصل قيد علاقات (Foreign Key) أو أي خطأ، هيرجع false بأمان والبرنامج مش هيقفل
                isDeleted = false;
            }
            finally
            {
                // الحارس الأمين اللي بيقفل الاتصال دايماً لمنع تسريب الاتصالات
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return isDeleted;
        }

        private static bool _ExecuteIsCountryExistByName(string CountryName)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            // الـ SQL Query هنا ذكي: بيعمل تشيك لو الاسم بيساوي العمود الإنجليزي أَوْ العمود العربي
            // واستخدام EXISTS هو الأسرع للأداء لأن السيرفر بيوقف بحث فوراً أول ما يلاقي أول تطابق
            string query = @"SELECT 1 FROM Countries 
                             WHERE CountryName_En = @CountryName OR CountryName_Ar = @CountryName";

            SqlCommand command = new SqlCommand(query, connection);

            // بنباصي المتغير مرة واحدة بس وبنحميه من الـ SQL Injection
            command.Parameters.AddWithValue("@CountryName", CountryName);

            try
            {
                connection.Open();

                // ExecuteScalar بترجع قيمة أول عمود في أول صف (وهو رقم 1 لو لقى تطابق)
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    isFound = true; // الاسم موجود فعلاً سواء عربي أو إنجليزي
                }
            }
            catch (Exception ex)
            {
                isFound = false;
            }
            finally
            {
                // ضمان قفل الاتصال دايماً مهما حصل جوه الـ try
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return isFound;
        }

        // Public

        public static int AddNewCountry(string CountryName_En, string CountryName_Ar)
        {
            return _ExecuteAddNewCountry(CountryName_En, CountryName_Ar);
        }

        public static bool UpdateCountry(int CountryID, string CountryName_En, string CountryName_Ar)
        {
            // بتباصي المتغيرات علطول للدالة الـ Private عشان تنفذها
            return _ExecuteUpdateCountry(CountryID, CountryName_En, CountryName_Ar);
        }

        public static bool FindByID(int CountryID, ref string CountryName_En, ref string CountryName_Ar)
        {
            // بتباصي المتغيرات بالـ ref للدالة الـ Private عشان تملى البيانات وترجع بيها
            return _ExecuteFindCountry(CountryID, ref CountryName_En, ref CountryName_Ar);
        }

        public static bool FindByCountryName_En(string CountryName_En, ref int CountryID, ref string CountryName_Ar)
        {
            return _ExecuteFindCountryByName_En(CountryName_En, ref CountryID, ref CountryName_Ar);
        }

        public static bool FindByCountryName_Ar(string CountryName_Ar, ref int CountryID, ref string CountryName_En)
        {
            return _ExecuteFindCountryByName_Ar(CountryName_Ar, ref CountryID, ref CountryName_En);
        }

        public static DataTable GetAllCountries()
        {
            return _ExecuteGetAllCountries();
        }

        public static DataTable SearchWithStart(string SearchText)
        {
            return _ExecuteSearchWithStart(SearchText);
        }

        public static DataTable SearchContains(string SearchText)
        {
            return _ExecuteSearchContains(SearchText);
        }

        public static bool IsCountryExistByID(int CountryID)
        {
            return _ExecuteIsCountryExistByID(CountryID);
        }

        public static bool DeleteCountry(int CountryID)
        {
            return _ExecuteDeleteCountry(CountryID);
        }

        public static bool IsCountryExistByName(string CountryName)
        {
            return _ExecuteIsCountryExistByName(CountryName);
        }
    }
}
