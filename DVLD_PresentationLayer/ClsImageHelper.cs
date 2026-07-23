using System;
using System.Windows.Media.Imaging;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_PresentationLayer
{
    public static class ClsImageHelper
    {
        // دالة بتبعتلها مسار الصورة، وترجعلك BitmapImage جاهز ومفكوك القفل
        public static BitmapImage LoadImageFreely(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
                return null; // لو المسار فاضي أو الملف مش موجود

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad; // فك قفل الملف
            bitmap.EndInit();
            bitmap.Freeze(); // ميزة إضافية للسرعة وللـ Multi-threading

            return bitmap;
        }
    }
}
