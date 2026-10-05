using System;
using BassemCore.Security;
using DVLD_BusinessLayer; 

public class ClsCredentialsManager
{
    private static string registryPath = @"HKEY_CURRENT_USER\SOFTWARE\DVLD_System\LoginSettings";

    /// <summary>
    /// دالة لحفظ الـ Username و Password في الريجستري مع تشفير كلمة المرور بـ AES
    /// </summary>
    public static bool SaveStoredCredential(string username, string password)
    {
        try
        {
            RegistryHelper reg = new RegistryHelper(registryPath);

            // حفظ اسم المستخدم عادياً (أو يمكنك تشفيره أيضاً لو أردت)
            bool isUserSaved = reg.Write("Username", username);

            // [تشفير كلمة المرور هنا قبل حفظها]:
            string encryptedPassword = ClsEncryption.EncryptAES(password);
            bool isPassSaved = reg.Write("Password", encryptedPassword);

            return isUserSaved && isPassSaved;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// دالة لقراءة البيانات من الريجستري وفك تشفير كلمة المرور تلقائياً
    /// </summary>
    public static bool GetStoredCredential(ref string username, ref string password)
    {
        try
        {
            RegistryHelper reg = new RegistryHelper(registryPath);

            object userObj = reg.Read("Username", string.Empty);
            object passObj = reg.Read("Password", string.Empty);

            string savedUsername = userObj?.ToString() ?? string.Empty;
            string encryptedPassword = passObj?.ToString() ?? string.Empty;

            if (!string.IsNullOrEmpty(savedUsername) && !string.IsNullOrEmpty(encryptedPassword))
            {
                username = savedUsername;

                // [فك تشفير كلمة المرور هنا عند استرجاعها]:
                password = ClsEncryption.DecryptAES(encryptedPassword);

                return true;
            }

            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }
}