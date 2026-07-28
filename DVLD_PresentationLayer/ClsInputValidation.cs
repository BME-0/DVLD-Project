using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.IO;

namespace DVLD.Global_Classes
{
    public class ClsInputValidation
    {
        // أنواع التحقق المتاحة في النظام
        public enum eValidationType
        {
            NumbersOnly,
            LettersAndSpaces,
            AlphaNumeric,
            Phone
        }

        /// <summary>
        /// التحقق أثناء كتابة الحروف من الكيبورد
        /// </summary>
        public static void ValidateTextInput(TextCompositionEventArgs e, eValidationType validationType)
        {
            switch (validationType)
            {
                case eValidationType.NumbersOnly:
                    e.Handled = !Regex.IsMatch(e.Text, @"^[0-9]+$");
                    break;

                case eValidationType.LettersAndSpaces:
                    e.Handled = !Regex.IsMatch(e.Text, @"^[\p{L}\s]+$");
                    break;

                case eValidationType.AlphaNumeric:
                    e.Handled = !Regex.IsMatch(e.Text, @"^[a-zA-Z0-9]+$");
                    break;

                case eValidationType.Phone:
                    e.Handled = !Regex.IsMatch(e.Text, @"^[0-9+]+$");
                    break;
            }
        }

        /// <summary>
        /// منع مسافة الكيبورد (Space Key)
        /// </summary>
        public static void PreventSpaceKey(KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// التحقق عند عمل Paste (نسخ ولصق)
        /// </summary>
        public static void ValidatePasting(DataObjectPastingEventArgs e, eValidationType validationType)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string pasteText = (string)e.DataObject.GetData(typeof(string));
                bool isValid = false;

                switch (validationType)
                {
                    case eValidationType.NumbersOnly:
                        isValid = Regex.IsMatch(pasteText, @"^[0-9]+$");
                        break;

                    case eValidationType.LettersAndSpaces:
                        isValid = Regex.IsMatch(pasteText, @"^[\p{L}\s]+$");
                        break;

                    case eValidationType.AlphaNumeric:
                        isValid = Regex.IsMatch(pasteText, @"^[a-zA-Z0-9]+$");
                        break;

                    case eValidationType.Phone:
                        isValid = Regex.IsMatch(pasteText, @"^[0-9+]+$");
                        break;
                }

                if (!isValid)
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        /// <summary>
        /// التحقق من صحة صيغة البريد الإلكتروني
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            // Regex قياسي لفحص الإيميل
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// السماح بالأرقام ونقطة عشرية واحدة فقط (مثلاً: 150.50)
        /// </summary>
        public static void ValidateDecimalInput(TextCompositionEventArgs e, string currentText)
        {
            if (e.Text == ".")
            {
                // منع إضافة أكثر من نقطة عشرية واحدة في نفس الحقل
                e.Handled = currentText.Contains(".");
            }
            else
            {
                // منع أي عنصر غير الأرقام
                e.Handled = !Regex.IsMatch(e.Text, @"^[0-9]+$");
            }
        }

        /// <summary>
        /// التحقق من أن الرقم القومي يتكون من عدد محدد من الأرقام (مثلاً 14 رقم)
        /// </summary>
        public static bool IsValidNationalNo(string nationalNo, int expectedLength = 14)
        {
            if (string.IsNullOrWhiteSpace(nationalNo)) return false;

            return nationalNo.Length == expectedLength && Regex.IsMatch(nationalNo, @"^[0-9]+$");
        }

        /// <summary>
        /// التحقق من أن تاريخ الميلاد يطابق السن الأدنى المسموح به
        /// </summary>
        public static bool IsValidAge(DateTime birthDate, int minimumAge = 18)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - birthDate.Year;

            // خصم سنة لو لم يأت يوم ميلاده بعد في هذه السنة
            if (birthDate.Date > today.AddYears(-age)) age--;

            return age >= minimumAge;
        }

        /// <summary>
        /// فحص ما إذا كان النص فارغاً أو يحتوي على مسافات فقط
        /// </summary>
        public static bool IsEmpty(string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        /// <summary>
        /// فحص وقوع الرقم بين قيمة دنيا وقيمة قصوى
        /// </summary>
        public static bool IsNumberBetween(int number, int min, int max)
        {
            return number >= min && number <= max;
        }

        /// <summary>
        /// التحقق من قوة كلمة المرور (حروف كبيرة، صغيرة، أرقام، ورموز)
        /// </summary>
        public static bool IsStrongPassword(string password, int minLength = 8)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < minLength) return false;

            bool hasUpper = Regex.IsMatch(password, @"[A-Z]");
            bool hasLower = Regex.IsMatch(password, @"[a-z]");
            bool hasDigit = Regex.IsMatch(password, @"[0-9]");
            bool hasSpecial = Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]");

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }

        /// <summary>
        /// التحقق من أن صيغة امتداد الملف صورة صحيحة
        /// </summary>
        public static bool IsValidImageFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) return false;

            string extension = Path.GetExtension(filePath).ToLower();
            return extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".bmp";
        }
    }
}