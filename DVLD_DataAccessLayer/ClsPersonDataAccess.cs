using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccessLayer
{
    public static class ClsPersonDataAccess
    {
        // Private

        private static int _ExecuteAddNewPerson(
            string NationalNo, string FirstName, string SecondName, string ThirdName, string LastName,
            DateTime DateOfBirth,string Address, string Phone1, string Phone2, string Email,
            int NationalityCountryID, string ImagePath,bool Gendor)
        {
            int NewPersonID = -1;

            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            string query = @"INSERT INTO People 
                             (NationalNo, FirstName, SecondName, ThirdName, LastName, DateOfBirth, Address, Phone1, Phone2, Email, NationalityCountryID, ImagePath, Gendor) 
                             VALUES 
                             (@NationalNo, @FirstName, @SecondName, @ThirdName, @LastName, @DateOfBirth, @Address, @Phone1, @Phone2, @Email, @NationalityCountryID, @ImagePath, @Gendor);
                             SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(query, connection);

            // إضافة الحقول الأساسية الـ NOT NULL (إجباري يكون فيها داتا)
            command.Parameters.AddWithValue("@NationalNo", NationalNo);
            command.Parameters.AddWithValue("@FirstName", FirstName);
            command.Parameters.AddWithValue("@SecondName", SecondName);
            command.Parameters.AddWithValue("@LastName", LastName);
            command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
            command.Parameters.AddWithValue("@Address", Address);
            command.Parameters.AddWithValue("@Phone1", Phone1);
            command.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryID);
            command.Parameters.AddWithValue("@Gendor", Gendor);

            // --- معالجة الحقول التي تقبل NULL الذكية جداً ---

            // 1. الاسم الثالث
            if (!string.IsNullOrEmpty(ThirdName))
                command.Parameters.AddWithValue("@ThirdName", ThirdName);
            else
                command.Parameters.AddWithValue("@ThirdName", DBNull.Value);

            // 2. التليفون الثاني
            if (!string.IsNullOrEmpty(Phone2))
                command.Parameters.AddWithValue("@Phone2", Phone2);
            else
                command.Parameters.AddWithValue("@Phone2", DBNull.Value);

            // 3. البريد الإلكتروني
            if (!string.IsNullOrEmpty(Email))
                command.Parameters.AddWithValue("@Email", Email);
            else
                command.Parameters.AddWithValue("@Email", DBNull.Value);

            // 4. مسار الصورة
            if (!string.IsNullOrEmpty(ImagePath))
                command.Parameters.AddWithValue("@ImagePath", ImagePath);
            else
                command.Parameters.AddWithValue("@ImagePath", DBNull.Value);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar(); // تنفيذ الاستعلام وجلب الـ ID الجديد

                if (result != null && int.TryParse(result.ToString(), out int insertedId))
                {
                    NewPersonID = insertedId;
                }
            }
            catch (Exception ex)
            {
                // لو حصل أي خطأ، الدالة بترجع -1 والسيستم بيفهم إن الإضافة فشلت بدون كراش
                NewPersonID = -1;
            }
            finally
            {
                // قفل الاتصال دايماً لمنع تسريب الـ Connections في السيرفر
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return NewPersonID;
        }

        private static int _ExecuteAddNewPerson(
            string NationalNo, string FirstName, string SecondName, string ThirdName, string LastName,
            DateTime DateOfBirth, string Address, string Phone1, string Phone2, string Email,
            string CountryName, string ImagePath, bool Gendor)
        {
            int NewPersonID = -1;

            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            // الحركة الصايعة هنا: مكان الـ NationalityCountryID عملنا (SELECT CountryID FROM Countries...) 
            // عشان الـ SQL يجيب الرقم بناءً على الاسم (عربي أو إنجليزي) ويخزنه فوراً
            string query = @"INSERT INTO People 
                             (NationalNo, FirstName, SecondName, ThirdName, LastName, DateOfBirth, Address, Phone1, Phone2, Email, NationalityCountryID, ImagePath, Gendor) 
                             VALUES 
                             (@NationalNo, @FirstName, @SecondName, @ThirdName, @LastName, @DateOfBirth, @Address, @Phone1, @Phone2, @Email, 
                             (SELECT TOP 1 CountryID FROM Countries WHERE CountryName_En = @CountryName OR CountryName_Ar = @CountryName), 
                             @ImagePath, @Gendor);
                             SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(query, connection);

            // إضافة الحقول الأساسية
            command.Parameters.AddWithValue("@NationalNo", NationalNo);
            command.Parameters.AddWithValue("@FirstName", FirstName);
            command.Parameters.AddWithValue("@SecondName", SecondName);
            command.Parameters.AddWithValue("@LastName", LastName);
            command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
            command.Parameters.AddWithValue("@Address", Address);
            command.Parameters.AddWithValue("@Phone1", Phone1);
            command.Parameters.AddWithValue("@CountryName", CountryName); // باصينا اسم الدولة هنا
            command.Parameters.AddWithValue("@Gendor", Gendor); 

            // --- معالجة الحقول التي تقبل NULL ---
            if (!string.IsNullOrEmpty(ThirdName))
                command.Parameters.AddWithValue("@ThirdName", ThirdName);
            else
                command.Parameters.AddWithValue("@ThirdName", DBNull.Value);

            if (!string.IsNullOrEmpty(Phone2))
                command.Parameters.AddWithValue("@Phone2", Phone2);
            else
                command.Parameters.AddWithValue("@Phone2", DBNull.Value);

            if (!string.IsNullOrEmpty(Email))
                command.Parameters.AddWithValue("@Email", Email);
            else
                command.Parameters.AddWithValue("@Email", DBNull.Value);

            if (!string.IsNullOrEmpty(ImagePath))
                command.Parameters.AddWithValue("@ImagePath", ImagePath);
            else
                command.Parameters.AddWithValue("@ImagePath", DBNull.Value);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int insertedId))
                {
                    NewPersonID = insertedId;
                }
            }
            catch (Exception ex)
            {
                NewPersonID = -1;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return NewPersonID;
        }

        private static bool _ExecuteUpdatePerson(
            int PersonID, string NationalNo, string FirstName, string SecondName, string ThirdName, string LastName,
            DateTime DateOfBirth, string Address, string Phone1, string Phone2, string Email,
            string CountryName, string ImagePath, bool Gendor)
        {
            bool isUpdated = false;

            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            // الحركة الذكية: بنجيب الـ CountryID من جدول الدول بناءً على الاسم المبعوث جوه الـ UPDATE نفسه
            string query = @"UPDATE People 
                             SET NationalNo = @NationalNo, 
                                 FirstName = @FirstName, 
                                 SecondName = @SecondName, 
                                 ThirdName = @ThirdName, 
                                 LastName = @LastName, 
                                 DateOfBirth = @DateOfBirth, 
                                 Address = @Address, 
                                 Phone1 = @Phone1, 
                                 Phone2 = @Phone2, 
                                 Email = @Email, 
                                 NationalityCountryID = (SELECT TOP 1 CountryID FROM Countries WHERE CountryName_En = @CountryName OR CountryName_Ar = @CountryName), 
                                 ImagePath = @ImagePath
                                 Gendor = @Gendor
                             WHERE PersonID = @PersonID";

            SqlCommand command = new SqlCommand(query, connection);

            // إضافة الحقول الأساسية الـ NOT NULL
            command.Parameters.AddWithValue("@PersonID", PersonID);
            command.Parameters.AddWithValue("@NationalNo", NationalNo);
            command.Parameters.AddWithValue("@FirstName", FirstName);
            command.Parameters.AddWithValue("@SecondName", SecondName);
            command.Parameters.AddWithValue("@LastName", LastName);
            command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
            command.Parameters.AddWithValue("@Address", Address);
            command.Parameters.AddWithValue("@Phone1", Phone1);
            command.Parameters.AddWithValue("@CountryName", CountryName); // اسم الدولة
            command.Parameters.AddWithValue("@Gendor", Gendor); 

            // --- معالجة الحقول التي تقبل NULL عند التعديل ---
            if (!string.IsNullOrEmpty(ThirdName))
                command.Parameters.AddWithValue("@ThirdName", ThirdName);
            else
                command.Parameters.AddWithValue("@ThirdName", DBNull.Value);

            if (!string.IsNullOrEmpty(Phone2))
                command.Parameters.AddWithValue("@Phone2", Phone2);
            else
                command.Parameters.AddWithValue("@Phone2", DBNull.Value);

            if (!string.IsNullOrEmpty(Email))
                command.Parameters.AddWithValue("@Email", Email);
            else
                command.Parameters.AddWithValue("@Email", DBNull.Value);

            if (!string.IsNullOrEmpty(ImagePath))
                command.Parameters.AddWithValue("@ImagePath", ImagePath);
            else
                command.Parameters.AddWithValue("@ImagePath", DBNull.Value);

            try
            {
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery(); // تنفيذ التعديل

                if (rowsAffected > 0)
                {
                    isUpdated = true; // التعديل نجح وسَمّع في الداتا بيز
                }
            }
            catch (Exception ex)
            {
                isUpdated = false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return isUpdated;
        }

        private static bool _ExecuteUpdatePerson(
            int PersonID, string NationalNo, string FirstName, string SecondName, string ThirdName, string LastName,
            DateTime DateOfBirth, string Address, string Phone1, string Phone2, string Email,
            int NationalityCountryID, string ImagePath, bool Gendor)
        {
            bool isUpdated = false;

            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            // رجعنا الـ Query للأصل: بيعدل الـ NationalityCountryID مباشرة بالـ Parameter الرقمي
            string query = @"UPDATE People 
                             SET NationalNo = @NationalNo, 
                                 FirstName = @FirstName, 
                                 SecondName = @SecondName, 
                                 ThirdName = @ThirdName, 
                                 LastName = @LastName, 
                                 DateOfBirth = @DateOfBirth, 
                                 Address = @Address, 
                                 Phone1 = @Phone1, 
                                 Phone2 = @Phone2, 
                                 Email = @Email, 
                                 NationalityCountryID = @NationalityCountryID, 
                                 ImagePath = @ImagePath
                                 Gendor = @Gendor
                             WHERE PersonID = @PersonID";

            SqlCommand command = new SqlCommand(query, connection);

            // إضافة الحقول الأساسية الـ NOT NULL
            command.Parameters.AddWithValue("@PersonID", PersonID);
            command.Parameters.AddWithValue("@NationalNo", NationalNo);
            command.Parameters.AddWithValue("@FirstName", FirstName);
            command.Parameters.AddWithValue("@SecondName", SecondName);
            command.Parameters.AddWithValue("@LastName", LastName);
            command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
            command.Parameters.AddWithValue("@Address", Address);
            command.Parameters.AddWithValue("@Phone1", Phone1);
            command.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryID); // تمرير الرقم مباشرة
            command.Parameters.AddWithValue("@Gendor", Gendor); 

            // --- معالجة الحقول التي تقبل NULL عند التعديل ---
            if (!string.IsNullOrEmpty(ThirdName))
                command.Parameters.AddWithValue("@ThirdName", ThirdName);
            else
                command.Parameters.AddWithValue("@ThirdName", DBNull.Value);

            if (!string.IsNullOrEmpty(Phone2))
                command.Parameters.AddWithValue("@Phone2", Phone2);
            else
                command.Parameters.AddWithValue("@Phone2", DBNull.Value);

            if (!string.IsNullOrEmpty(Email))
                command.Parameters.AddWithValue("@Email", Email);
            else
                command.Parameters.AddWithValue("@Email", DBNull.Value);

            if (!string.IsNullOrEmpty(ImagePath))
                command.Parameters.AddWithValue("@ImagePath", ImagePath);
            else
                command.Parameters.AddWithValue("@ImagePath", DBNull.Value);

            try
            {
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    isUpdated = true;
                }
            }
            catch (Exception ex)
            {
                isUpdated = false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return isUpdated;
        }

        private static bool _ExecuteFind(
            int PersonID, ref string NationalNo, ref string FirstName, ref string SecondName,
            ref string ThirdName, ref string LastName, ref DateTime DateOfBirth, ref string Address,
            ref string Phone1, ref string Phone2, ref string Email, ref int NationalityCountryID, ref string ImagePath, ref bool Gendor)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM People WHERE PersonID = @PersonID";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    isFound = true;

                    // 1. الحقول الإجبارية (NOT NULL) -> كاست مباشر بدون خوف
                    NationalNo = (string)reader["NationalNo"];
                    FirstName = (string)reader["FirstName"];
                    SecondName = (string)reader["SecondName"];
                    LastName = (string)reader["LastName"];
                    DateOfBirth = (DateTime)reader["DateOfBirth"];
                    Address = (string)reader["Address"];
                    Phone1 = (string)reader["Phone1"];
                    NationalityCountryID = (int)reader["NationalityCountryID"];
                    Gendor = (bool)reader["Gendor"];

                    // 2. الحقول الاختيارية (تريكة الـ NULL الحارسة) 🛡️
                    // لو القيمة في الداتا بيز بتمرر DBNull، بنحولها لـ "" عشان الـ C# يفهمها

                    ThirdName = (reader["ThirdName"] != DBNull.Value) ? (string)reader["ThirdName"] : "";

                    Phone2 = (reader["Phone2"] != DBNull.Value) ? (string)reader["Phone2"] : "";

                    Email = (reader["Email"] != DBNull.Value) ? (string)reader["Email"] : "";

                    ImagePath = (reader["ImagePath"] != DBNull.Value) ? (string)reader["ImagePath"] : "";
                }
                else
                {
                    isFound = false;
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

        private static bool _ExecuteFindByNationalNo(
            string NationalNo, ref int PersonID, ref string FirstName, ref string SecondName,
            ref string ThirdName, ref string LastName, ref DateTime DateOfBirth, ref string Address,
            ref string Phone1, ref string Phone2, ref string Email, ref int NationalityCountryID, ref string ImagePath, ref bool Gendor)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM People WHERE NationalNo = @NationalNo";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@NationalNo", NationalNo);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    isFound = true; // السجل موجود

                    // أ) قراءة الحقول الإجبارية (NOT NULL) بأمان
                    PersonID = (int)reader["PersonID"];
                    FirstName = (string)reader["FirstName"];
                    SecondName = (string)reader["SecondName"];
                    LastName = (string)reader["LastName"];
                    DateOfBirth = (DateTime)reader["DateOfBirth"];
                    Address = (string)reader["Address"];
                    Phone1 = (string)reader["Phone1"];
                    NationalityCountryID = (int)reader["NationalityCountryID"];
                    Gendor = (bool)reader["Gendor"];

                    // ب) الحرس الهندسي: معالجة الـ DBNull للحقول التي تقبل NULL 🛡️
                    ThirdName = (reader["ThirdName"] != DBNull.Value) ? (string)reader["ThirdName"] : "";
                    Phone2 = (reader["Phone2"] != DBNull.Value) ? (string)reader["Phone2"] : "";
                    Email = (reader["Email"] != DBNull.Value) ? (string)reader["Email"] : "";
                    ImagePath = (reader["ImagePath"] != DBNull.Value) ? (string)reader["ImagePath"] : "";
                }
                else
                {
                    isFound = false; // لم يتم العثور على الرقم الوطني
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

        private static bool _ExecuteDeletePerson(int PersonID)
        {
            bool isDeleted = false;

            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            string query = "DELETE FROM People WHERE PersonID = @PersonID";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                connection.Open();

                // ExecuteNonQuery بترجع عدد الصفوف اللي اتأثرت بالحذف
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    isDeleted = true; // الحذف تم بنجاح
                }
            }
            catch (Exception ex)
            {
                // لو الشخص مربوط بجداول تانية، الـ catch هتمسك الخطأ هنا وترجع false بأمان
                isDeleted = false;
            }
            finally
            {
                // ضمان قفل الاتصال بالسيرفر دايماً
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return isDeleted;
        }

        private static bool _ExecuteDeleteByNationalNo(string NationalNo)
        {
            bool isDeleted = false;

            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            string query = "DELETE FROM People WHERE NationalNo = @NationalNo";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@NationalNo", NationalNo);

            try
            {
                connection.Open();

                // تنفيذ الحذف وجلب عدد الصفوف المتأثرة
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    isDeleted = true; // تم الحذف بنجاح
                }
            }
            catch (Exception ex)
            {
                // حماية السيستم لو الرقم مربوط بجداول تانية أو حصل أي استثناء
                isDeleted = false;
            }
            finally
            {
                // قفل الاتصال بالسيرفر دايماً
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return isDeleted;
        }

        private static DataTable _ExecuteGetAllPeople()
        {
            DataTable dt = new DataTable();

            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            // استخدام ISNULL(Column, '') بيضمن إن لو الحقل NULL يرجع كنص فاضي ونظيف
            string query = @"SELECT PersonID, NationalNo, FirstName, SecondName, 
                             ISNULL(ThirdName, '') AS ThirdName, LastName, 
                             DateOfBirth, Address, Phone1, 
                             ISNULL(Phone2, '') AS Phone2, 
                             ISNULL(Email, '') AS Email, 
                             NationalityCountryID, 
                             ISNULL(ImagePath, '') AS ImagePath , Gendor
                             FROM People";

            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    dt.Load(reader); // ميزه Load إنها بتتعامل مع الـ DBNull اللي جاي وتملى الـ DataTable بأمان
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                // لو حصل أي خطأ بنرجع جدول فاضي عشان السيستم ميعملش كراش
                dt = new DataTable();
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

        private static DataTable _ExecuteGetAllPeopleWithCountryName()
        {
            DataTable dt = new DataTable();

            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            // الـ Query بيعمل INNER JOIN مع جدول Countries عشان يسحب CountryName_En أو CountryName_Ar حسب رغبتك
            // هنا سحبنا الاسم الإنجليزي وممكن تغيره للعربي حسب واجهة سيستمك
            string query = @"SELECT People.PersonID, People.NationalNo, People.FirstName, People.SecondName, 
                             ISNULL(People.ThirdName, '') AS ThirdName, People.LastName, 
                             People.DateOfBirth, People.Address, People.Phone1, 
                             ISNULL(People.Phone2, '') AS Phone2, 
                             ISNULL(People.Email, '') AS Email, 
                             Countries.CountryName_En AS CountryName, 
                             ISNULL(People.ImagePath, '') AS ImagePath, People.Gendor
                             FROM People 
                             INNER JOIN Countries ON People.NationalityCountryID = Countries.CountryID";

            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    dt.Load(reader); // تحميل البيانات وتنظيف الـ DBNull تلقائياً للـ DataTable
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                dt = new DataTable(); // جدول فاضي في حالة الخطأ لحماية السيستم
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

        private static bool _ExecuteIsPersonExistByID(int PersonID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            string query = "SELECT 1 FROM People WHERE PersonID = @PersonID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    isFound = true; // الـ ID موجود فعلاً
                }
            }
            catch (Exception ex)
            {
                isFound = false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }

            return isFound;
        }

        private static bool _ExecuteIsPersonExistByNationalNo(string NationalNo)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            string query = "SELECT 1 FROM People WHERE NationalNo = @NationalNo";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@NationalNo", NationalNo);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    isFound = true; // الرقم الوطني مسجل لشخص آخر بالفعل
                }
            }
            catch (Exception ex)
            {
                isFound = false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }

            return isFound;
        }

        private static DataTable _ExecuteSearchStartsWith(string ColumnName, string Value)
        {
            DataTable dt = new DataTable();

            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            // تريكة الـ SQL Injection للحماية: بما إن اسم العمود متغير ومينفعش نحطه كـ Parameter مباشر،
            // بنحشره في الـ Query بعد التأكد من نظافته، وبنحط الـ LIKE مع الـ Parameter للقيمة.
            // علامة الـ % في الآخر تعني: يبدأ بـ (Starts With)
            string query = $@"SELECT People.PersonID, People.NationalNo, People.FirstName, People.SecondName, 
                              ISNULL(People.ThirdName, '') AS ThirdName, People.LastName, 
                              People.DateOfBirth, People.Address, People.Phone1, 
                              ISNULL(People.Phone2, '') AS Phone2, 
                              ISNULL(People.Email, '') AS Email, 
                              Countries.CountryName_En AS CountryName, 
                              ISNULL(People.ImagePath, '') AS ImagePath, People.Gendor
                              FROM People 
                              INNER JOIN Countries ON People.NationalityCountryID = Countries.CountryID
                              WHERE People.{ColumnName} LIKE @Value";

            SqlCommand command = new SqlCommand(query, connection);

            // بنضيف علامة الـ % في نهاية النص المبعوث
            command.Parameters.AddWithValue("@Value", Value + "%");

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
                dt = new DataTable(); // جدول فاضي في حالة الخطأ لحماية السيستم
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

        private static DataTable _ExecuteSearchEndsWith(string ColumnName, string Value)
        {
            DataTable dt = new DataTable();

            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            // الاستعلام مع وضع اسم العمود المتغير بحذر، والـ LIKE لفلترة النهايات
            string query = $@"SELECT People.PersonID, People.NationalNo, People.FirstName, People.SecondName, 
                              ISNULL(People.ThirdName, '') AS ThirdName, People.LastName, 
                              People.DateOfBirth, People.Address, People.Phone1, 
                              ISNULL(People.Phone2, '') AS Phone2, 
                              ISNULL(People.Email, '') AS Email, 
                              Countries.CountryName_En AS CountryName, 
                              ISNULL(People.ImagePath, '') AS ImagePath, People.Gendor
                              FROM People 
                              INNER JOIN Countries ON People.NationalityCountryID = Countries.CountryID
                              WHERE People.{ColumnName} LIKE @Value";

            SqlCommand command = new SqlCommand(query, connection);

            // التريكة هنا: علامة الـ % في الأول تعني (Ends With)
            command.Parameters.AddWithValue("@Value", "%" + Value);

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
                dt = new DataTable(); // حماية السيستم بجدول فاضي في حالة الخطأ
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

        private static DataTable _ExecuteSearchContains(string ColumnName, string Value)
        {
            DataTable dt = new DataTable();

            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            // الاستعلام مع الـ INNER JOIN وتنظيف الـ NULL بـ ISNULL
            string query = $@"SELECT People.PersonID, People.NationalNo, People.FirstName, People.SecondName, 
                              ISNULL(People.ThirdName, '') AS ThirdName, People.LastName, 
                              People.DateOfBirth, People.Address, People.Phone1, 
                              ISNULL(People.Phone2, '') AS Phone2, 
                              ISNULL(People.Email, '') AS Email, 
                              Countries.CountryName_En AS CountryName, 
                              ISNULL(People.ImagePath, '') AS ImagePath, People.Gendor
                              FROM People 
                              INNER JOIN Countries ON People.NationalityCountryID = Countries.CountryID
                              WHERE People.{ColumnName} LIKE @Value";

            SqlCommand command = new SqlCommand(query, connection);

            // التريكة هنا: علامة الـ % من الطرفين تعني احتواء النص في أي مكان (Contains)
            command.Parameters.AddWithValue("@Value", "%" + Value + "%");

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
                dt = new DataTable(); // حماية السيستم بجدول فاضي في حالة الخطأ
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

        private static string _ExecuteGetFullName(int PersonID)
        {
            string FullName = "";
            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);

            // دمج ذكي عشان لو الـ ThirdName بـ NULL مسافات الاسم تطلع مظبوطة ومفيش مسافة زيادة
            string query = @"SELECT FirstName + ' ' + SecondName + ' ' + 
                     CASE WHEN ThirdName IS NOT NULL THEN ThirdName + ' ' ELSE '' END + 
                     LastName AS FullName 
                     FROM People WHERE PersonID = @PersonID";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    FullName = result.ToString();
                }
            }
            catch
            {
                FullName = "";
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }

            return FullName;
        }

        private static string _ExecuteGetImagePath(int PersonID)
        {
            string ImagePath = "";
            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);
            string query = "SELECT ISNULL(ImagePath, '') FROM People WHERE PersonID = @PersonID";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    ImagePath = result.ToString();
                }
            }
            catch
            {
                ImagePath = "";
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }

            return ImagePath;
        }

        private static bool _ExecuteIsPersonLinkedToUser(int PersonID)
        {
            bool isLinked = false;
            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);
            string query = "SELECT 1 FROM Users WHERE PersonID = @PersonID";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    isLinked = true; // الشخص ده مربوط بـ User ومينفعش يتحذف
                }
            }
            catch
            {
                isLinked = false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }

            return isLinked;
        }

        private static bool _ExecuteIsPersonLinkedToDriver(int PersonID)
        {
            bool isLinked = false;
            SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString);
            string query = "SELECT 1 FROM Drivers WHERE PersonID = @PersonID";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    isLinked = true; // الشخص ده مربوط بـ User ومينفعش يتحذف
                }
            }
            catch
            {
                isLinked = false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open) connection.Close();
            }

            return isLinked;
        }

        // Public

        public static int AddNewPerson(
            string NationalNo, string FirstName, string SecondName, string ThirdName, string LastName,
            DateTime DateOfBirth, string Address, string Phone1, string Phone2, string Email,
            int NationalityCountryID, string ImagePath, bool Gendor)
        {
            // بتسلم البارامترز الفردية فوراً للدالة الـ Private
            return _ExecuteAddNewPerson(
                NationalNo, FirstName, SecondName, ThirdName, LastName,
                DateOfBirth, Address, Phone1, Phone2, Email,
                NationalityCountryID, ImagePath, Gendor);
        }

        public static int AddNewPerson(
            string NationalNo, string FirstName, string SecondName, string ThirdName, string LastName,
            DateTime DateOfBirth, string Address, string Phone1, string Phone2, string Email,
            string CountryName, string ImagePath, bool Gendor)
        {
            return _ExecuteAddNewPerson(
                NationalNo, FirstName, SecondName, ThirdName, LastName,
                DateOfBirth, Address, Phone1, Phone2, Email,
                CountryName, ImagePath, Gendor);
        }

        public static bool UpdatePerson(
            int PersonID, string NationalNo, string FirstName, string SecondName, string ThirdName, string LastName,
            DateTime DateOfBirth, string Address, string Phone1, string Phone2, string Email,
            string CountryName, string ImagePath, bool Gendor)
        {
            return _ExecuteUpdatePerson(
                PersonID, NationalNo, FirstName, SecondName, ThirdName, LastName,
                DateOfBirth, Address, Phone1, Phone2, Email,
                CountryName, ImagePath, Gendor);
        }

        public static bool UpdatePerson(
            int PersonID, string NationalNo, string FirstName, string SecondName, string ThirdName, string LastName,
            DateTime DateOfBirth, string Address, string Phone1, string Phone2, string Email,
            int NationalityCountryID, string ImagePath, bool Gendor)
        {
            return _ExecuteUpdatePerson(
                PersonID, NationalNo, FirstName, SecondName, ThirdName, LastName,
                DateOfBirth, Address, Phone1, Phone2, Email,
                NationalityCountryID, ImagePath, Gendor);
        }

        public static bool FindByPersonID(
            int PersonID, ref string NationalNo, ref string FirstName, ref string SecondName,
            ref string ThirdName, ref string LastName, ref DateTime DateOfBirth, ref string Address,
            ref string Phone1, ref string Phone2, ref string Email, ref int NationalityCountryID, ref string ImagePath,ref bool Gendor)
        {
            return _ExecuteFind(
                PersonID, ref NationalNo, ref FirstName, ref SecondName,
                ref ThirdName, ref LastName, ref DateOfBirth, ref Address,
                ref Phone1, ref Phone2, ref Email, ref NationalityCountryID, ref ImagePath,ref Gendor);
        }

        public static bool FindByNationalNo(
            string NationalNo, ref int PersonID, ref string FirstName, ref string SecondName,
            ref string ThirdName, ref string LastName, ref DateTime DateOfBirth, ref string Address,
            ref string Phone1, ref string Phone2, ref string Email, ref int NationalityCountryID, ref string ImagePath, ref bool Gendor)
        {
            return _ExecuteFindByNationalNo(
                NationalNo, ref PersonID, ref FirstName, ref SecondName,
                ref ThirdName, ref LastName, ref DateOfBirth, ref Address,
                ref Phone1, ref Phone2, ref Email, ref NationalityCountryID, ref ImagePath,ref Gendor);
        }

        public static bool DeletePerson(int PersonID)
        {
            return _ExecuteDeletePerson(PersonID);
        }

        public static bool DeleteByNationalNo(string NationalNo)
        {
            return _ExecuteDeleteByNationalNo(NationalNo);
        }

        public static DataTable GetAllPeople()
        {
            return _ExecuteGetAllPeople();
        }

        public static DataTable GetAllPeopleWithCountryName()
        {
            return _ExecuteGetAllPeopleWithCountryName();
        }

        public static bool IsPersonExistByID(int PersonID)
        {
            return _ExecuteIsPersonExistByID(PersonID);
        }

        public static bool IsPersonExistByNationalNo(string NationalNo)
        {
            return _ExecuteIsPersonExistByNationalNo(NationalNo);
        }

        public static DataTable SearchStartsWith(string ColumnName, string Value)
        {
            // حماية إضافية: نضمن إن اسم العمود ميكونش فاضي عشان الكويري ميتكسرش
            if (string.IsNullOrEmpty(ColumnName) || string.IsNullOrEmpty(Value))
            {
                return GetAllPeopleWithCountryName(); // لو مفيش قيمة للبحث، رجع الجدول كله
            }

            return _ExecuteSearchStartsWith(ColumnName, Value);
        }

        public static DataTable SearchEndsWith(string ColumnName, string Value)
        {
            // حماية لمنع كسر الاستعلام لو مفيش بيانات مبعوتة
            if (string.IsNullOrEmpty(ColumnName) || string.IsNullOrEmpty(Value))
            {
                return GetAllPeopleWithCountryName(); // رجع الجدول كله لو الخانات فاضية
            }

            return _ExecuteSearchEndsWith(ColumnName, Value);
        }

        public static DataTable SearchContains(string ColumnName, string Value)
        {
            // حماية لمنع كسر الاستعلام لو الخانات فاضية
            if (string.IsNullOrEmpty(ColumnName) || string.IsNullOrEmpty(Value))
            {
                return GetAllPeopleWithCountryName(); // رجع الجدول كله لو مفيش نص بحث
            }

            return _ExecuteSearchContains(ColumnName, Value);
        }

        public static string GetFullName(int PersonID)
        {
            return _ExecuteGetFullName(PersonID);
        }

        public static string GetImagePath(int PersonID)
        {
            return _ExecuteGetImagePath(PersonID);
        }

        public static bool IsPersonLinkedToUser(int PersonID)
        {
            return _ExecuteIsPersonLinkedToUser(PersonID);
        }

        public static bool IsPersonLinkedToDriver(int PersonID)
        {
            return _ExecuteIsPersonLinkedToDriver(PersonID);
        }
    }
}
