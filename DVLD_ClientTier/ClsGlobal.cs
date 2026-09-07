using DVLD_BusinessLayer; // تأكد من مطابقة مساحة النمط لطبقة الـ BLL لديك
using System;
using System.IO;
using System.Linq;

namespace DVLD_ClientTier
{
    public static class ClsGlobal
    {
        // مرجع يحمل بيانات المستخدم الحالي الذي قام بتسجيل الدخول بنجاح ويستخدم في كافة أنحاء النظام
        public static ClsUser CurrentUser { get; set; }

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
    }
}