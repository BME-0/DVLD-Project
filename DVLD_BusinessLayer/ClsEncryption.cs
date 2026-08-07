using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DVLD_BusinessLayer
{
    public static class ClsEncryption
    {
        // ==========================================
        // 1. التشفير باتجاه واحد العادي (SHA256 Hash)
        // ==========================================
        /// <summary>
        /// تقوم بتحويل أي نص (مثل كلمة المرور العادية) إلى بصفرة أو هاش (Hash) غير قابل للفك.
        /// الاستخدام الأساسي: مقارنة كلمات المرور (تأخذ النص، تشفره، وتقارنه بالهاش المخزن في الداتابيز).
        /// ملاحظة: العملية باتجاه واحد، يعني مستحيل ترجع النص الأصلي من الهاش.
        /// </summary>
        public static string ComputeHash(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return "";

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainText));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // ==========================================
        // 2. التشفير الاحترافي لكلمات المرور مع الملح (PBKDF2 With Salt)
        // ==========================================
        /// <summary>
        /// المعيار الذهبي والأعلى أماناً عالمياً لتخزين كلمات المرور.
        /// تقوم بإضافة عشوائية تسمى "Salt" مع التكرار لضمان استحالة اختراق الباسورد حتى لو تمت سرقة الداتابيز.
        /// ترجع لك الهاش النهائي، وتعطيك معك متغير الـ Salt لحفظه بجانبه في الداتابيز.
        /// </summary>
        public static string HashPasswordWithSalt(string password, out string salt)
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            salt = Convert.ToBase64String(saltBytes);

            // استخدام PBKDF2 مع 100,000 تكرار لزيادة الأمان وصعوبة التخمين
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100000, HashAlgorithmName.SHA256))
            {
                byte[] hashBytes = pbkdf2.GetBytes(32);
                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// تقارن كلمة المرور التي كتبها المستخدم في شاشة اللوجن بالهاش والـ Salt المخزنين في الداتابيز.
        /// ترجع True إذا تطابقا، و False إذا كان خطأ.
        /// </summary>
        public static bool VerifyPasswordWithSalt(string password, string storedHash, string storedSalt)
        {
            try
            {
                byte[] saltBytes = Convert.FromBase64String(storedSalt);
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100000, HashAlgorithmName.SHA256))
                {
                    byte[] hashBytes = pbkdf2.GetBytes(32);
                    string computedHash = Convert.ToBase64String(hashBytes);
                    return computedHash == storedHash;
                }
            }
            catch
            {
                return false;
            }
        }

        // ==========================================
        // 3. التشفير باتجاهين (Symmetric Encryption - AES)
        // ==========================================
        private static readonly string _SecretKey = "DVLD_System_Secure_Key_2026!";

        /// <summary>
        /// تشفير نص بحيث يمكن استرجاع أصله لاحقاً (Directional / Two-Way Encryption).
        /// الاستخدام: لو عندك بيانات حساسة جداً وعاوز تشفرها في الداتابيز وترجع تقرأها وتفك تشفيرها.
        /// </summary>
        public static string EncryptAES(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return "";

            byte[] keyBytes = Encoding.UTF8.GetBytes(_SecretKey.PadRight(32).Substring(0, 32));
            byte[] ivBytes = new byte[16];

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = ivBytes;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        /// <summary>
        /// عكس الدالة السابقة؛ تأخذ النص المشفر بـ AES وتعيده لك نَصّاً عادياً ومقروءاً.
        /// </summary>
        public static string DecryptAES(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return "";

            try
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(_SecretKey.PadRight(32).Substring(0, 32));
                byte[] ivBytes = new byte[16];
                byte[] buffer = Convert.FromBase64String(cipherText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.IV = ivBytes;

                    using (MemoryStream ms = new MemoryStream(buffer))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        // ==========================================
        // 4. توقيع البيانات والتأكد من سلامتها (HMACSHA256)
        // ==========================================
        /// <summary>
        /// تقوم بعمل توقيع رقمي (Signature) للبيانات باستخدام مفتاح سري.
        /// الاستخدام: التأكد من أن البيانات أو الملفات لم يتم التلاعب بها أو تعديلها من قبل طرف خارجي أثناء نقلها.
        /// </summary>
        public static string ComputeHMAC(string data, string secretKey)
        {
            if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(secretKey))
                return "";

            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(dataBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        // ==========================================
        // 5. توليد رموز عشوائية آمنة (Secure Random Token)
        // ==========================================
        /// <summary>
        /// تولد نص عشوائي معقد وآمن تماماً (Token).
        /// الاستخدام: لو احتجت كود تفعيل، مفتاح ترخيص مؤقت، أو كلمة مرور مؤقتة لمرة واحدة.
        /// </summary>
        public static string GenerateSecureToken(int length = 32)
        {
            byte[] bytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes);
        }

        // ==========================================
        // 6. الترميز السريع (Base64 Encode / Decode)
        // ==========================================
        /// <summary>
        /// ترميز النص إلى صيغة Base64 (هذا ليس تشفيراً حقيقياً، مجرد إخفاء شكل النص ظاهرياً).
        /// الاستخدام: تمرير بيانات عبر الـ URL أو حفظ نصوص بشكل لا يبدو مقروءاً للعين المجردة.
        /// </summary>
        public static string Base64Encode(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return "";

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// فك ترميز الـ Base64 لإرجاع النص الأصلي.
        /// </summary>
        public static string Base64Decode(string base64EncodedData)
        {
            if (string.IsNullOrEmpty(base64EncodedData))
                return "";

            try
            {
                byte[] base64EncodedBytes = FromBase64StringSafe(base64EncodedData);
                if (base64EncodedBytes == null)
                    return "";

                return Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static byte[] FromBase64StringSafe(string s)
        {
            try
            {
                return Convert.FromBase64String(s);
            }
            catch
            {
                return null;
            }
        }
    }
}