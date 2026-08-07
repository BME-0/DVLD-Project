using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DVLD_PresentationLayer 
{
    public static class ClsImageHelper
    {
        /// <summary>
        /// دالة لترجع الـ BitmapImage جاهز ومفكوك القفل
        /// </summary>
        public static BitmapImage LoadImageFreely(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
                return null;

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }

        /// <summary>
        /// دالة لضبط الأيقونة أو الرمز الافتراضي (الذكر/أنثى) في حال لم تكن الصورة موجودة
        /// </summary>
        public static void SetDefaultGenderIcon(TextBlock txtGenderIcon, dynamic personObject)
        {
            if (txtGenderIcon == null) return;

            txtGenderIcon.Visibility = Visibility.Visible;

            if (personObject != null)
            {
                // افترض أن خاصية الجنس هي Gender (True = انثى، False = ذكر)
                if (personObject.Gender)
                {
                    txtGenderIcon.Text = "👩"; // رمز الأنثى
                }
                else
                {
                    txtGenderIcon.Text = "🧔"; // رمز الذكر
                }
            }
            else
            {
                txtGenderIcon.Text = "👤"; // الرمز الافتراضي العام
            }
        }
    }
}