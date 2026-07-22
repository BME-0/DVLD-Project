using DVLD_BusinessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    internal static class ClsTestCountry
    {
       public static void TestGetAllCountries()
        {
            Console.WriteLine("\n--- Testing GetAllCountries ---");
            DataTable dt = ClsCountry.GetAllCountries();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Console.WriteLine($"ID: {row["CountryID"]}, EN: {row["CountryName_En"]}, AR: {row["CountryName_Ar"]}");
                }
            }
            else
            {
                Console.WriteLine("No countries found or connection failed!");
            }
        }

        public static void TestAddNewCountry()
        {
            Console.WriteLine("\n--- Testing Add New Country ---");
            ClsCountry country = new ClsCountry();
            country.CountryName_En = "Test Country";
            country.CountryName_Ar = "دولة تجريبية";

            if (country.Save())
            {
                Console.WriteLine($"Success! Country Saved Successfully with ID: {country.CountryID}");
            }
            else
            {
                Console.WriteLine("Failed to add new country!");
            }
        }

        public static void TestFindCountryByID(int id)
        {
            Console.WriteLine($"\n--- Testing Find Country By ID: {id} ---");
            ClsCountry country = ClsCountry.Find(id);

            if (country != null)
            {
                Console.WriteLine($"Found! ID: {country.CountryID}, Name EN: {country.CountryName_En}, Name AR: {country.CountryName_Ar}");
            }
            else
            {
                Console.WriteLine($"Country with ID [{id}] NOT Found!");
            }
        }

        public static void TestUpdateCountry(int id)
        {
            Console.WriteLine($"\n--- Testing Update Country ID: {id} ---");
            ClsCountry country = ClsCountry.Find(id);

            if (country != null)
            {
                country.CountryName_En = "Updated Country Name";
                country.CountryName_Ar = "اسم معدل تجريبي";

                if (country.Save())
                {
                    Console.WriteLine("Country Updated Successfully!");
                    // إعادة البحث للتأكد من سماع التعديل
                    ClsCountry updatedCountry = ClsCountry.Find(id);
                    Console.WriteLine($"New Names -> EN: {updatedCountry.CountryName_En}, AR: {updatedCountry.CountryName_Ar}");
                }
                else
                {
                    Console.WriteLine("Failed to update country!");
                }
            }
            else
            {
                Console.WriteLine("Country not found to update!");
            }
        }

        public static void TestIsCountryExist()
        {
            Console.WriteLine("\n--- Testing IsCountryExist ---");

            // فحص بالـ ID
            bool existByID = ClsCountry.IsCountryExistByID(1);
            Console.WriteLine($"Is Country ID [1] Exist? {existByID}");

            // فحص بالاسم
            bool existByName = ClsCountry.IsCountryExistByName("دولة تجريبية");
            Console.WriteLine($"Is 'Test Country' Exist? {existByName}");
        }

        public static void TestDeleteCountry(int id)
        {
            Console.WriteLine($"\n--- Testing Delete Country ID: {id} ---");

            if (ClsCountry.DeleteCountry(id))
            {
                Console.WriteLine($"Country with ID [{id}] Deleted Successfully!");
            }
            else
            {
                Console.WriteLine($"Failed to delete! (Country might not exist or it is linked to a Person as Foreign Key)");
            }
        }

        public static void TestSearchWithStart()
        {
            Console.WriteLine("\n--- Testing SearchWithStart ---");

            // هنفتش مثلاً على الدول اللي بتبدأ بـ "Eg" (زي Egypt) أو "الأ" (زي الأردن)
            string searchText = "E";
            DataTable dt = ClsCountry.SearchWithStart(searchText);

            Console.WriteLine($"Search results for countries starting with '{searchText}':");
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Console.WriteLine($"ID: {row["CountryID"]}, EN: {row["CountryName_En"]}, AR: {row["CountryName_Ar"]}");
                }
            }
            else
            {
                Console.WriteLine("No countries found starting with this text.");
            }
        }

        public static void TestSearchContains()
        {
            Console.WriteLine("\n--- Testing SearchContains ---");

            // هنفتش مثلاً على أي دولة اسمهم جواه حروف "ia" (زي Syria, Tunisia, Algeria)
            string searchText = "ia";
            DataTable dt = ClsCountry.SearchContains(searchText);

            Console.WriteLine($"Search results for countries containing '{searchText}':");
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Console.WriteLine($"ID: {row["CountryID"]}, EN: {row["CountryName_En"]}, AR: {row["CountryName_Ar"]}");
                }
            }
            else
            {
                Console.WriteLine("No countries found containing this text.");
            }
        }

    }
}
