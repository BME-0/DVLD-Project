using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccessLayer
{
    public static class ClsUserDataAccess
    {
        // Private

        private static int _ExecuteAddNewUser(int PersonID, string UserName, string Password, bool IsActive)
        {
            int NewUserID = -1;

            string query = @"INSERT INTO Users (PersonID, UserName, Password, IsActive)
                     VALUES (@PersonID, @UserName, @Password, @IsActive);
                     SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    command.Parameters.AddWithValue("@UserName", UserName);
                    command.Parameters.AddWithValue("@Password", Password);
                    command.Parameters.AddWithValue("@IsActive", IsActive);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedId))
                        {
                            NewUserID = insertedId;
                        }
                    }
                    catch (Exception ex)
                    {
                        NewUserID = -1;
                    }
                }
            }

            return NewUserID;
        }

        private static bool _ExecuteUpdateUser(int UserID, int PersonID, string UserName, string Password, bool IsActive)
        {
            bool isUpdated = false;

            string query = @"UPDATE Users 
                     SET PersonID = @PersonID,
                         UserName = @UserName,
                         Password = @Password,
                         IsActive = @IsActive
                     WHERE UserID = @UserID";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", UserID);
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    command.Parameters.AddWithValue("@UserName", UserName);
                    command.Parameters.AddWithValue("@Password", Password);
                    command.Parameters.AddWithValue("@IsActive", IsActive);

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

        private static bool _ExecuteFindUserByUserID(int UserID, ref int PersonID, ref string UserName, ref string Password, ref bool IsActive)
        {
            bool isFound = false;
            string query = "SELECT * FROM Users WHERE UserID = @UserID";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", UserID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                PersonID = (int)reader["PersonID"];
                                UserName = (string)reader["UserName"];
                                Password = (string)reader["Password"];
                                IsActive = (bool)reader["IsActive"];
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

        private static bool _ExecuteFindUserByPersonID(int PersonID, ref int UserID, ref string UserName, ref string Password, ref bool IsActive)
        {
            bool isFound = false;
            string query = "SELECT * FROM Users WHERE PersonID = @PersonID";

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

                                UserID = (int)reader["UserID"];
                                UserName = (string)reader["UserName"];
                                Password = (string)reader["Password"];
                                IsActive = (bool)reader["IsActive"];
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

        private static bool _ExecuteFindUserByUsernameAndPassword(string UserName, string Password, ref int UserID, ref int PersonID, ref bool IsActive)
        {
            bool isFound = false;
            string query = "SELECT * FROM Users WHERE UserName = @UserName AND Password = @Password";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", UserName);
                    command.Parameters.AddWithValue("@Password", Password);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                UserID = (int)reader["UserID"];
                                PersonID = (int)reader["PersonID"];
                                IsActive = (bool)reader["IsActive"];
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

        private static bool _ExecuteFindUserByUsername(string UserName, ref int UserID, ref int PersonID, ref string Password, ref bool IsActive)
        {
            bool isFound = false;
            string query = "SELECT * FROM Users WHERE UserName = @UserName";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", UserName);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;

                                UserID = (int)reader["UserID"];
                                PersonID = (int)reader["PersonID"];
                                Password = (string)reader["Password"];
                                IsActive = (bool)reader["IsActive"];
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

        private static bool _ExecuteDeleteUser(int UserID)
        {
            bool isDeleted = false;
            string query = "DELETE FROM Users WHERE UserID = @UserID";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", UserID);

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

        private static DataTable _ExecuteGetAllUsers()
        {
            DataTable dt = new DataTable();

            string query = @"SELECT Users.UserID, Users.PersonID, 
                           (People.FirstName + ' ' + People.SecondName + ' ' + ISNULL(People.ThirdName, '') + ' ' + People.LastName) AS FullName, 
                           Users.UserName, Users.IsActive
                    FROM Users INNER JOIN
                         People ON Users.PersonID = People.PersonID";

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

        private static DataTable _ExecuteGetAllUsersWithDetails()
        {
            DataTable dt = new DataTable();

            string query = @"SELECT 
                        Users.UserID, 
                        Users.PersonID, 
                        People.NationalNo,
                        (People.FirstName + ' ' + People.SecondName + ' ' + ISNULL(People.ThirdName, '') + ' ' + People.LastName) AS FullName, 
                        Users.UserName, 
                        Users.IsActive
                     FROM Users 
                     INNER JOIN People ON Users.PersonID = People.PersonID";

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

        private static bool _ExecuteIsUserExist(int UserID)
        {
            bool isFound = false;
            string query = "SELECT 1 FROM Users WHERE UserID = @UserID";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", UserID);

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

        private static bool _ExecuteIsUserExist(string UserName)
        {
            bool isFound = false;
            string query = "SELECT 1 FROM Users WHERE UserName = @UserName";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", UserName);

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

        private static bool _ExecuteIsUserExistForPersonID(int PersonID)
        {
            bool isFound = false;
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

        private static bool _ExecuteIsUserExistByUsernameAndPassword(string UserName, string Password)
        {
            bool isFound = false;
            string query = "SELECT 1 FROM Users WHERE UserName = @UserName AND Password = @Password";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", UserName);
                    command.Parameters.AddWithValue("@Password", Password);

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

        private static DataTable _ExecuteSearchUsersStartsWith(string ColumnName, string Value)
        {
            DataTable dt = new DataTable();

            string query = $@"SELECT 
                        Users.UserID, 
                        Users.PersonID, 
                        People.NationalNo,
                        (People.FirstName + ' ' + People.SecondName + ' ' + ISNULL(People.ThirdName, '') + ' ' + People.LastName) AS FullName, 
                        Users.UserName, 
                        Users.IsActive
                     FROM Users 
                     INNER JOIN People ON Users.PersonID = People.PersonID
                     WHERE Users.{ColumnName} LIKE @Value;";

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

        private static DataTable _ExecuteSearchUsersEndsWith(string ColumnName, string Value)
        {
            DataTable dt = new DataTable();

            string query = $@"SELECT 
                        Users.UserID, 
                        Users.PersonID, 
                        People.NationalNo,
                        (People.FirstName + ' ' + People.SecondName + ' ' + ISNULL(People.ThirdName, '') + ' ' + People.LastName) AS FullName, 
                        Users.UserName, 
                        Users.IsActive
                     FROM Users 
                     INNER JOIN People ON Users.PersonID = People.PersonID
                     WHERE Users.{ColumnName} LIKE @Value;";

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

        private static DataTable _ExecuteSearchUsersContains(string ColumnName, string Value)
        {
            DataTable dt = new DataTable();

            string query = $@"SELECT 
                        Users.UserID, 
                        Users.PersonID, 
                        People.NationalNo,
                        (People.FirstName + ' ' + People.SecondName + ' ' + ISNULL(People.ThirdName, '') + ' ' + People.LastName) AS FullName, 
                        Users.UserName, 
                        Users.IsActive
                     FROM Users 
                     INNER JOIN People ON Users.PersonID = People.PersonID
                     WHERE Users.{ColumnName} LIKE @Value;";

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

        private static bool _ExecuteIsPersonLinkedToUser(int PersonID)
        {
            return ClsPersonDataAccess.IsPersonLinkedToUser(PersonID);
        }

        private static bool _ExecuteChangePassword(int UserID, string NewPassword)
        {
            bool isUpdated = false;
            string query = "UPDATE Users SET Password = @Password WHERE UserID = @UserID";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", UserID);
                    command.Parameters.AddWithValue("@Password", NewPassword);

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

        private static bool _ExecuteChangeUserStatus(int UserID, bool IsActive)
        {
            bool isUpdated = false;
            string query = "UPDATE Users SET IsActive = @IsActive WHERE UserID = @UserID";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", UserID);
                    command.Parameters.AddWithValue("@IsActive", IsActive);

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

        private static int _ExecuteGetUsersCount()
        {
            int count = 0;
            string query = "SELECT COUNT(*) FROM Users";

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

        private static DataTable _ExecuteGetUsersByStatus(bool IsActive)
        {
            DataTable dt = new DataTable();

            string query = @"SELECT Users.UserID, Users.PersonID, 
                           (People.FirstName + ' ' + People.SecondName + ' ' + ISNULL(People.ThirdName, '') + ' ' + People.LastName) AS FullName, 
                           Users.UserName, Users.IsActive
                    FROM Users 
                    INNER JOIN People ON Users.PersonID = People.PersonID
                    WHERE Users.IsActive = @IsActive";

            using (SqlConnection connection = new SqlConnection(ClsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IsActive", IsActive);

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

        private static int _ExcuteGetTodayUsersCount()
        {
            int count = 0;
            string query = "SELECT COUNT(*) FROM Users WHERE CAST(CreatedDate AS DATE) = CAST(GETDATE() AS DATE)";

            object result = ClsDataAccessSettings.ExecuteScalarQuery(query);

            if (result != null && int.TryParse(result.ToString(), out int insertedCount))
            {
                count = insertedCount;
            }

            return count;
        }

        // Public

        public static int AddNewUser(int PersonID, string UserName, string Password, bool IsActive)
        {
            // يمكنك وضع أي تحقق إضافي هنا قبل التنفيذ إذا أردت
            return _ExecuteAddNewUser(PersonID, UserName, Password, IsActive);
        }

        public static bool UpdateUser(int UserID, int PersonID, string UserName, string Password, bool IsActive)
        {
            return _ExecuteUpdateUser(UserID, PersonID, UserName, Password, IsActive);
        }

        public static bool GetUserInfoByUserID(int UserID, ref int PersonID, ref string UserName, ref string Password, ref bool IsActive)
        {
            return _ExecuteFindUserByUserID(UserID, ref PersonID, ref UserName, ref Password, ref IsActive);
        }

        public static bool GetUserInfoByPersonID(int PersonID, ref int UserID, ref string UserName, ref string Password, ref bool IsActive)
        {
            return _ExecuteFindUserByPersonID(PersonID, ref UserID, ref UserName, ref Password, ref IsActive);
        }

        public static bool GetUserInfoByUsernameAndPassword(string UserName, string Password, ref int UserID, ref int PersonID, ref bool IsActive)
        {
            return _ExecuteFindUserByUsernameAndPassword(UserName, Password, ref UserID, ref PersonID, ref IsActive);
        }

        public static bool GetUserInfoByUsername(string UserName, ref int UserID, ref int PersonID, ref string Password, ref bool IsActive)
        {
            return _ExecuteFindUserByUsername(UserName, ref UserID, ref PersonID, ref Password, ref IsActive);
        }

        public static bool DeleteUser(int UserID)
        {
            return _ExecuteDeleteUser(UserID);
        }

        public static DataTable GetAllUsers()
        {
            return _ExecuteGetAllUsers();
        }

        public static DataTable GetAllUsersWithDetails()
        {
            return _ExecuteGetAllUsersWithDetails();
        }

        public static bool IsUserExist(int UserID)
        {
            return _ExecuteIsUserExist(UserID);
        }

        public static bool IsUserExist(string UserName)
        {
            return _ExecuteIsUserExist(UserName);
        }

        public static bool IsUserExistForPersonID(int PersonID)
        {
            return _ExecuteIsUserExistForPersonID(PersonID);
        }

        public static bool IsUserExist(string UserName, string Password)
        {
            return _ExecuteIsUserExistByUsernameAndPassword(UserName, Password);
        }

        public static DataTable SearchUsersStartsWith(string ColumnName, string Value)
        {
            return _ExecuteSearchUsersStartsWith(ColumnName, Value);
        }

        public static DataTable SearchUsersEndsWith(string ColumnName, string Value)
        {
            return _ExecuteSearchUsersEndsWith(ColumnName, Value);
        }

        public static DataTable SearchUsersContains(string ColumnName, string Value)
        {
            return _ExecuteSearchUsersContains(ColumnName, Value);
        }

        public static bool IsPersonLinkedToUser(int PersonID)
        {
            return _ExecuteIsPersonLinkedToUser(PersonID);
        }

        public static bool ChangePassword(int UserID, string NewPassword)
        {
            return _ExecuteChangePassword(UserID, NewPassword);
        }

        public static bool ChangeUserStatus(int UserID, bool IsActive)
        {
            return _ExecuteChangeUserStatus(UserID, IsActive);
        }

        public static int GetUsersCount()
        {
            return _ExecuteGetUsersCount();
        }

        public static int GetTodayUsersCount()
        {
            return _ExcuteGetTodayUsersCount();
        }

        public static DataTable GetUsersByStatus(bool IsActive)
        {
            return _ExecuteGetUsersByStatus(IsActive);
        }
    }
}
