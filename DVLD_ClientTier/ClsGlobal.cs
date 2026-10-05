using DVLD_BusinessLayer; // تأكد من مطابقة مساحة النمط لطبقة الـ BLL لديك
using System;
using System.IO;
using System.Linq;
using BassemCore.Security; // مساحة الأسماء الخاصة بكلاس الـ RegistryHelper إن أردت استخدامها هنا أو تركها

namespace DVLD_ClientTier
{
    public static class ClsGlobal
    {
        // مرجع يحمل بيانات المستخدم الحالي الذي قام بتسجيل الدخول بنجاح ويستخدم في كافة أنحاء النظام
        public static ClsUser CurrentUser { get; set; }

        #region الطريقة القديمة (الملفات النصية - كما هي تماماً بدون حذف)

        /// <summary>
        /// حفظ اسم المستخدم وكلمة المرور في ملف نصي محلي عند اختيار تذكرني
        /// </summary>
        public static bool RememberUsernameAndPassword(string username, string password)
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "loginInfo.txt");

                if (string.IsNullOrEmpty(username))
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    return true;
                }

                string line = username + "#//#" + password;
                File.WriteAllText(filePath, line);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// قراءة بيانات الدخول المحفوظة مسبقاً في حال كان الملف موجوداً
        /// </summary>
        public static bool GetStoredCredential(ref string username, ref string password)
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "loginInfo.txt");

                if (File.Exists(filePath))
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        string line = reader.ReadLine();
                        if (!string.IsNullOrEmpty(line))
                        {
                            string[] result = line.Split(new string[] { "#//#" }, StringSplitOptions.None);
                            if (result.Length == 2)
                            {
                                username = result[0];
                                password = result[1];
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion


        #region الطريقة الجديدة (توجيه العمليات مباشرة إلى ClsCredentialsManager)

        /// <summary>
        /// حفظ اسم المستخدم وكلمة المرور في Windows Registry (عبر استدعاء ClsCredentialsManager)
        /// </summary>
        public static bool RememberUsernameAndPasswordInRegistry(string username, string password)
        {
            try
            {
                // إذا كان اسم المستخدم فارغاً (عند إلغاء تذكرني)، نقوم بحفظ قيم فارغة لتقوم الكلاس بحذفها أو تفريغها
                if (string.IsNullOrEmpty(username))
                {
                    return ClsCredentialsManager.SaveStoredCredential(string.Empty, string.Empty);
                }

                // تمرير العمليات بالكامل إلى الكلاس المنفصل
                return ClsCredentialsManager.SaveStoredCredential(username, password);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// قراءة بيانات الدخول المحفوظة مسبقاً من Windows Registry (عبر استدعاء ClsCredentialsManager)
        /// </summary>
        public static bool GetStoredCredentialFromRegistry(ref string username, ref string password)
        {
            try
            {
                // تفويض عملية القراءة وفك التشفير بالكامل إلى ClsCredentialsManager
                return ClsCredentialsManager.GetStoredCredential(ref username, ref password);
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
    }
}