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
             DateTime DateOfBirth, string Address, string Phone1, string Phone2, string Email,
             int NationalityCountryID, string ImagePath, bool Gendor)
        {
            int NewPersonID = -1;

            string query = @"INSERT INTO People 
                     (NationalNo, FirstName, SecondName, ThirdName, LastName, DateOfBirth, Address, Phone1, Phone2, Email, NationalityCountryID, ImagePath, Gendor) 
                     VALUES 
                     (@NationalNo, @FirstName, @SecondName, @ThirdName, @LastName, @DateOfBirth, @Address, @Phone1, @Phone2, @Email, @NationalityCountryID, @ImagePath, @Gendor);
                     SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // الحقول الأساسية NOT NULL
                    command.Parameters.AddWithValue("@NationalNo", NationalNo);
                    command.Parameters.AddWithValue("@FirstName", FirstName);
                    command.Parameters.AddWithValue("@SecondName", SecondName);
                    command.Parameters.AddWithValue("@LastName", LastName);
                    command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                    command.Parameters.AddWithValue("@Address", Address);
                    command.Parameters.AddWithValue("@Phone1", Phone1);
                    command.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryID);
                    command.Parameters.AddWithValue("@Gendor", Gendor);

                    // الحقول التي تقبل NULL
                    command.Parameters.AddWithValue("@ThirdName", string.IsNullOrEmpty(ThirdName) ? DBNull.Value : (object)ThirdName);
                    command.Parameters.AddWithValue("@Phone2", string.IsNullOrEmpty(Phone2) ? DBNull.Value : (object)Phone2);
                    command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(Email) ? DBNull.Value : (object)Email);
                    command.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(ImagePath) ? DBNull.Value : (object)ImagePath);

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
                } // إغلاق وحذف command تلقائياً
            } // إغلاق وحذف connection تلقائياً

            return NewPersonID;
        }

        private static int _ExecuteAddNewPerson(
            string NationalNo, string FirstName, string SecondName, string ThirdName, string LastName,
            DateTime DateOfBirth, string Address, string Phone1, string Phone2, string Email,
            string CountryName, string ImagePath, bool Gendor)
        {
            int NewPersonID = -1;

            string query = @"INSERT INTO People 
                     (NationalNo, FirstName, SecondName, ThirdName, LastName, DateOfBirth, Address, Phone1, Phone2, Email, NationalityCountryID, ImagePath, Gendor) 
                     VALUES 
                     (@NationalNo, @FirstName, @SecondName, @ThirdName, @LastName, @DateOfBirth, @Address, @Phone1, @Phone2, @Email, 
                     (SELECT TOP 1 CountryID FROM Countries WHERE CountryName_En = @CountryName OR CountryName_Ar = @CountryName), 
                     @ImagePath, @Gendor);
                     SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NationalNo", NationalNo);
                    command.Parameters.AddWithValue("@FirstName", FirstName);
                    command.Parameters.AddWithValue("@SecondName", SecondName);
                    command.Parameters.AddWithValue("@LastName", LastName);
                    command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                    command.Parameters.AddWithValue("@Address", Address);
                    command.Parameters.AddWithValue("@Phone1", Phone1);
                    command.Parameters.AddWithValue("@CountryName", CountryName);
                    command.Parameters.AddWithValue("@Gendor", Gendor);

                    command.Parameters.AddWithValue("@ThirdName", string.IsNullOrEmpty(ThirdName) ? DBNull.Value : (object)ThirdName);
                    command.Parameters.AddWithValue("@Phone2", string.IsNullOrEmpty(Phone2) ? DBNull.Value : (object)Phone2);
                    command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(Email) ? DBNull.Value : (object)Email);
                    command.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(ImagePath) ? DBNull.Value : (object)ImagePath);

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

            // تم إضافة فاصلة (,) قبل Gendor تصحيحاً لخطأ الـ SQL
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
                         ImagePath = @ImagePath,
                         Gendor = @Gendor
                     WHERE PersonID = @PersonID";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    command.Parameters.AddWithValue("@NationalNo", NationalNo);
                    command.Parameters.AddWithValue("@FirstName", FirstName);
                    command.Parameters.AddWithValue("@SecondName", SecondName);
                    command.Parameters.AddWithValue("@LastName", LastName);
                    command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                    command.Parameters.AddWithValue("@Address", Address);
                    command.Parameters.AddWithValue("@Phone1", Phone1);
                    command.Parameters.AddWithValue("@CountryName", CountryName);
                    command.Parameters.AddWithValue("@Gendor", Gendor);

                    command.Parameters.AddWithValue("@ThirdName", string.IsNullOrEmpty(ThirdName) ? DBNull.Value : (object)ThirdName);
                    command.Parameters.AddWithValue("@Phone2", string.IsNullOrEmpty(Phone2) ? DBNull.Value : (object)Phone2);
                    command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(Email) ? DBNull.Value : (object)Email);
                    command.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(ImagePath) ? DBNull.Value : (object)ImagePath);

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
        private static bool _ExecuteUpdatePerson(
            int PersonID, string NationalNo, string FirstName, string SecondName, string ThirdName, string LastName,
            DateTime DateOfBirth, string Address, string Phone1, string Phone2, string Email,
            int NationalityCountryID, string ImagePath, bool Gendor)
        {
            bool isUpdated = false;

            // تم إضافة الفاصلة (,) قبل Gendor لضبط جملة SQL
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
                         ImagePath = @ImagePath,
                         Gendor = @Gendor
                     WHERE PersonID = @PersonID";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // الحقول الأساسية NOT NULL
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    command.Parameters.AddWithValue("@NationalNo", NationalNo);
                    command.Parameters.AddWithValue("@FirstName", FirstName);
                    command.Parameters.AddWithValue("@SecondName", SecondName);
                    command.Parameters.AddWithValue("@LastName", LastName);
                    command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                    command.Parameters.AddWithValue("@Address", Address);
                    command.Parameters.AddWithValue("@Phone1", Phone1);
                    command.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryID);
                    command.Parameters.AddWithValue("@Gendor", Gendor);

                    // معالجة الحقول التي تقبل NULL
                    command.Parameters.AddWithValue("@ThirdName", string.IsNullOrEmpty(ThirdName) ? DBNull.Value : (object)ThirdName);
                    command.Parameters.AddWithValue("@Phone2", string.IsNullOrEmpty(Phone2) ? DBNull.Value : (object)Phone2);
                    command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(Email) ? DBNull.Value : (object)Email);
                    command.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(ImagePath) ? DBNull.Value : (object)ImagePath);

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
        private static bool _ExecuteFind(
            int PersonID, ref string NationalNo, ref string FirstName, ref string SecondName,
            ref string ThirdName, ref string LastName, ref DateTime DateOfBirth, ref string Address,
            ref string Phone1, ref string Phone2, ref string Email, ref int NationalityCountryID, ref string ImagePath, ref bool Gendor)
        {
            bool isFound = false;
            string query = "SELECT * FROM People WHERE PersonID = @PersonID";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                // 1. الحقول الإجبارية (NOT NULL)
                                NationalNo = (string)reader["NationalNo"];
                                FirstName = (string)reader["FirstName"];
                                SecondName = (string)reader["SecondName"];
                                LastName = (string)reader["LastName"];
                                DateOfBirth = (DateTime)reader["DateOfBirth"];
                                Address = (string)reader["Address"];
                                Phone1 = (string)reader["Phone1"];
                                NationalityCountryID = (int)reader["NationalityCountryID"];
                                Gendor = (bool)reader["Gendor"];

                                // 2. الحقول الاختيارية (NULL)
                                ThirdName = (reader["ThirdName"] != DBNull.Value) ? (string)reader["ThirdName"] : "";
                                Phone2 = (reader["Phone2"] != DBNull.Value) ? (string)reader["Phone2"] : "";
                                Email = (reader["Email"] != DBNull.Value) ? (string)reader["Email"] : "";
                                ImagePath = (reader["ImagePath"] != DBNull.Value) ? (string)reader["ImagePath"] : "";
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
        private static bool _ExecuteFindByNationalNo(
            string NationalNo, ref int PersonID, ref string FirstName, ref string SecondName,
            ref string ThirdName, ref string LastName, ref DateTime DateOfBirth, ref string Address,
            ref string Phone1, ref string Phone2, ref string Email, ref int NationalityCountryID, ref string ImagePath, ref bool Gendor)
        {
            bool isFound = false;
            string query = "SELECT * FROM People WHERE NationalNo = @NationalNo";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NationalNo", NationalNo);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                // أ) الحقول الإجبارية (NOT NULL)
                                PersonID = (int)reader["PersonID"];
                                FirstName = (string)reader["FirstName"];
                                SecondName = (string)reader["SecondName"];
                                LastName = (string)reader["LastName"];
                                DateOfBirth = (DateTime)reader["DateOfBirth"];
                                Address = (string)reader["Address"];
                                Phone1 = (string)reader["Phone1"];
                                NationalityCountryID = (int)reader["NationalityCountryID"];
                                Gendor = (bool)reader["Gendor"];

                                // ب) الحقول الاختيارية (NULL)
                                ThirdName = (reader["ThirdName"] != DBNull.Value) ? (string)reader["ThirdName"] : "";
                                Phone2 = (reader["Phone2"] != DBNull.Value) ? (string)reader["Phone2"] : "";
                                Email = (reader["Email"] != DBNull.Value) ? (string)reader["Email"] : "";
                                ImagePath = (reader["ImagePath"] != DBNull.Value) ? (string)reader["ImagePath"] : "";
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
        private static bool _ExecuteDeletePerson(int PersonID)
        {
            bool isDeleted = false;
            string query = "DELETE FROM People WHERE PersonID = @PersonID";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        isDeleted = (rowsAffected > 0);
                    }
                    catch (Exception ex)
                    {
                        isDeleted = false;
                    }
                }
            }

            return isDeleted;
        }
        private static bool _ExecuteDeleteByNationalNo(string NationalNo)
        {
            bool isDeleted = false;
            string query = "DELETE FROM People WHERE NationalNo = @NationalNo";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NationalNo", NationalNo);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        isDeleted = (rowsAffected > 0);
                    }
                    catch (Exception ex)
                    {
                        isDeleted = false;
                    }
                }
            }

            return isDeleted;
        }
        private static DataTable _ExecuteGetAllPeople()
        {
            DataTable dt = new DataTable();

            string query = @"SELECT PersonID, NationalNo, FirstName, SecondName, 
                            ISNULL(ThirdName, '') AS ThirdName, LastName, 
                            DateOfBirth, Address, Phone1, 
                            ISNULL(Phone2, '') AS Phone2, 
                            ISNULL(Email, '') AS Email, 
                            NationalityCountryID, 
                            ISNULL(ImagePath, '') AS ImagePath, Gendor
                     FROM People";

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
        private static DataTable _ExecuteGetAllPeopleWithCountryName()
        {
            DataTable dt = new DataTable();

            string query = @"SELECT People.PersonID, People.NationalNo, People.FirstName, People.SecondName, 
                            ISNULL(People.ThirdName, '') AS ThirdName, People.LastName, 
                            People.DateOfBirth, People.Address, People.Phone1, 
                            ISNULL(People.Phone2, '') AS Phone2, 
                            ISNULL(People.Email, '') AS Email, 
                            Countries.CountryName_En AS CountryName, 
                            ISNULL(People.ImagePath, '') AS ImagePath, People.Gendor
                     FROM People 
                     INNER JOIN Countries ON People.NationalityCountryID = Countries.CountryID";

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
        private static bool _ExecuteIsPersonExistByID(int PersonID)
        {
            bool isFound = false;
            string query = "SELECT 1 FROM People WHERE PersonID = @PersonID";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
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
                }
            }

            return isFound;
        }
        private static bool _ExecuteIsPersonExistByNationalNo(string NationalNo)
        {
            bool isFound = false;
            string query = "SELECT 1 FROM People WHERE NationalNo = @NationalNo";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NationalNo", NationalNo);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            isFound = true; // الرقم الوطني مسجل بالفعل
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
        private static DataTable _ExecuteSearchStartsWith(string ColumnName, string Value)
        {
            DataTable dt = new DataTable();

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

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Value", Value + "%");

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
        private static DataTable _ExecuteSearchEndsWith(string ColumnName, string Value)
        {
            DataTable dt = new DataTable();

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
        private static DataTable _ExecuteSearchContains(string ColumnName, string Value)
        {
            DataTable dt = new DataTable();

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
        private static string _ExecuteGetFullName(int PersonID)
        {
            string FullName = "";
            string query = @"SELECT FirstName + ' ' + SecondName + ' ' + 
                            CASE WHEN ThirdName IS NOT NULL THEN ThirdName + ' ' ELSE '' END + 
                            LastName AS FullName 
                     FROM People WHERE PersonID = @PersonID";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
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
                }
            }

            return FullName;
        }
        private static string _ExecuteGetImagePath(int PersonID)
        {
            string ImagePath = "";
            string query = "SELECT ISNULL(ImagePath, '') FROM People WHERE PersonID = @PersonID";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
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
                }
            }

            return ImagePath;
        }
        private static bool _ExecuteIsPersonLinkedToUser(int PersonID)
        {
            bool isLinked = false;
            string query = "SELECT 1 FROM Users WHERE PersonID = @PersonID";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            isLinked = true;
                        }
                    }
                    catch
                    {
                        isLinked = false;
                    }
                }
            }

            return isLinked;
        }
        private static bool _ExecuteIsPersonLinkedToDriver(int PersonID)
        {
            bool isLinked = false;
            string query = "SELECT 1 FROM Drivers WHERE PersonID = @PersonID";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            isLinked = true;
                        }
                    }
                    catch
                    {
                        isLinked = false;
                    }
                }
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
