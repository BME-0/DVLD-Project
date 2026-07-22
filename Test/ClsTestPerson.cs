using DVLD_BusinessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    internal static class ClsTestPerson
    {
        public static void TestSearchStartsWith()
        {
            Console.WriteLine("\n--- [TEST] Testing SearchStartsWith ---");

            // السيناريو الأول: احتمال صحيح (البحث عن الأشخاص اللي اسمهم الأول يبدأ بـ "Jo" مثل John)
            string columnName = "FirstName";
            string searchValue = "ر";

            DataTable dtValid = ClsPerson.SearchStartsWith(columnName, searchValue);
            Console.WriteLine($"A) Searching in [{columnName}] for values starting with '{searchValue}':");
            if (dtValid != null && dtValid.Rows.Count > 0)
            {
                Console.WriteLine($"   ✅ [SUCCESS] Found {dtValid.Rows.Count} matching people.");
                foreach (DataRow row in dtValid.Rows)
                {
                    Console.WriteLine($"      - ID: {row["PersonID"]}, Name: {row["FirstName"]} {row["LastName"]}");
                }
            }
            else
            {
                Console.WriteLine("   ⚠️ [INFO] No rows found or connection failed.");
            }

            // السيناريو الثاني: احتمال عدم وجود بيانات (البحث عن قيمة وهمية)
            string fakeValue = "XYZ_FAKE";
            DataTable dtInvalid = ClsPerson.SearchStartsWith(columnName, fakeValue);
            Console.WriteLine($"\nB) Searching in [{columnName}] for fake value '{fakeValue}':");
            if (dtInvalid != null && dtInvalid.Rows.Count == 0)
            {
                Console.WriteLine("   ✅ [CORRECT BEHAVIOUR] Successfully returned an EMPTY DataTable (0 rows) for non-existing data.");
            }
            else
            {
                Console.WriteLine("   ❌ [BUG] Table should be empty for fake data!");
            }
        }

        public static void TestSearchEndsWith()
        {
            Console.WriteLine("\n--- [TEST] Testing SearchEndsWith ---");

            // السيناريو الأول: احتمال صحيح (البحث في الاسم الأخير عن العائلات المنتهية بـ "oe" مثل Doe)
            string columnName = "LastName";
            string searchValue = "ب";

            DataTable dtValid = ClsPerson.SearchEndsWith(columnName, searchValue);
            Console.WriteLine($"A) Searching in [{columnName}] for values ending with '{searchValue}':");
            if (dtValid != null && dtValid.Rows.Count > 0)
            {
                Console.WriteLine($"   ✅ [SUCCESS] Found {dtValid.Rows.Count} matching people.");
                foreach (DataRow row in dtValid.Rows)
                {
                    Console.WriteLine($"      - ID: {row["PersonID"]}, FullName: {row["FirstName"]} {row["LastName"]}");
                }
            }
            else
            {
                Console.WriteLine("   ⚠️ [INFO] No rows found.");
            }

            // السيناريو الثاني: احتمال عدم وجود بيانات
            string fakeValue = "9999_FAKE";
            DataTable dtInvalid = ClsPerson.SearchEndsWith(columnName, fakeValue);
            Console.WriteLine($"\nB) Searching in [{columnName}] for fake value '{fakeValue}':");
            if (dtInvalid != null && dtInvalid.Rows.Count == 0)
            {
                Console.WriteLine("   ✅ [CORRECT BEHAVIOUR] Safely returned 0 rows for non-matching ends.");
            }
        }

        public static void TestSearchContains()
        {
            Console.WriteLine("\n--- [TEST] Testing SearchContains ---");

            // السيناريو الأول: احتمال صحيح (البحث في عمود العنوان عن أي شخص جواه كلمة "Cairo")
            string columnName = "Address";
            string searchValue = "حي";

            DataTable dtValid = ClsPerson.SearchContains(columnName, searchValue);
            Console.WriteLine($"A) Searching in [{columnName}] for values containing '{searchValue}':");
            if (dtValid != null && dtValid.Rows.Count > 0)
            {
                Console.WriteLine($"   ✅ [SUCCESS] Found {dtValid.Rows.Count} people living in or containing '{searchValue}'.");
                foreach (DataRow row in dtValid.Rows)
                {
                    Console.WriteLine($"      - ID: {row["PersonID"]}, Address from DB: {row["Address"]}");
                }
            }
            else
            {
                Console.WriteLine("   ⚠️ [INFO] No rows found matching this address snippet.");
            }

            // السيناريو الثاني: احتمال عدم وجود بيانات
            string fakeValue = "Fake_Street_123";
            DataTable dtInvalid = ClsPerson.SearchContains(columnName, fakeValue);
            Console.WriteLine($"\nB) Searching in [{columnName}] for fake value '{fakeValue}':");
            if (dtInvalid != null && dtInvalid.Rows.Count == 0)
            {
                Console.WriteLine("   ✅ [CORRECT BEHAVIOUR] Handled non-matching text properly with 0 rows.");
            }
        }

        public static void TestGetFullName_Isolated()
        {
            Console.WriteLine("\n--- [Isolated Test] GetFullName ---");

            // الاحتمال الأول: شخص موجود فعلاً (تأكد من وجود ID رقم 1 أو غيره في جهازك)
            int validID = 1;
            string validResult = ClsPerson.GetFullName(validID);
            Console.WriteLine($"[Valid Scenario] ID [{validID}] Full Name: \"{validResult}\"");

            // الاحتمال الثاني (خطأ): شخص وهمي غير موجود في السيستم
            int fakeID = 999999;
            string fakeResult = ClsPerson.GetFullName(fakeID);
            Console.WriteLine($"[Error Scenario] ID [{fakeID}] Full Name: \"{fakeResult}\"");

            if (string.IsNullOrEmpty(fakeResult))
            {
                Console.WriteLine("   ✅ [SUCCESS] Returned empty string safely for non-existing person.");
            }
            else
            {
                Console.WriteLine("   ❌ [BUG] Should return empty/null for fake IDs!");
            }
        }

        public static void TestGetImagePath_Isolated()
        {
            Console.WriteLine("\n--- [Isolated Test] GetImagePath ---");

            // الاحتمال الأول: شخص موجود فعلاً
            int validID = 1;
            string validPath = ClsPerson.GetImagePath(validID);
            Console.WriteLine($"[Valid Scenario] ID [{validID}] Image Path: \"{validPath}\"");

            // الاحتمال الثاني (خطأ): شخص وهمي غير موجود
            int fakeID = 999999;
            string fakePath = ClsPerson.GetImagePath(fakeID);
            Console.WriteLine($"[Error Scenario] ID [{fakeID}] Image Path: \"{fakePath}\"");

            if (string.IsNullOrEmpty(fakePath))
            {
                Console.WriteLine("   ✅ [SUCCESS] Handled missing/fake person image path safely.");
            }
        }

        public static void TestIsPersonLinkedToUser_Isolated()
        {
            Console.WriteLine("\n--- [Isolated Test] IsPersonLinkedToUser ---");

            // الاحتمال الأول: شخص مربوط بحساب مستخدم (مثلاً ID = 1 الموظف المسؤول)
            int validID = 1;
            bool isLinked = ClsPerson.IsPersonLinkedToUser(validID);
            Console.WriteLine($"[Valid Scenario] Is Person ID [{validID}] linked to a User? {isLinked}");
            Console.WriteLine(isLinked ? "   🔒 [GUARD ACTIVE] System will block deleting this person." : "   🔓 [INFO] Not linked, safe to delete.");

            // الاحتمال الثاني (خطأ): شخص وهمي تماماً
            int fakeID = 999999;
            bool fakeLinked = ClsPerson.IsPersonLinkedToUser(fakeID);
            Console.WriteLine($"[Error Scenario] Is Fake ID [{fakeID}] linked to a User? {fakeLinked}");

            if (fakeLinked == false)
            {
                Console.WriteLine("   ✅ [SUCCESS] Correct! Fake IDs cannot be linked to any user.");
            }
            else
            {
                Console.WriteLine("   ❌ [BUG] A non-existing ID cannot return TRUE!");
            }
        }

        public static void TestIsPersonLinkedToDriver_Isolated()
        {
            Console.WriteLine("\n--- [Isolated Test] IsPersonLinkedToDriver ---");

            // الاحتمال الأول: شخص مسجل كسائق في النظام
            int validID = 1;
            bool isLinked = ClsPerson.IsPersonLinkedToDriver(validID);
            Console.WriteLine($"[Valid Scenario] Is Person ID [{validID}] linked to a Driver? {isLinked}");
            Console.WriteLine(isLinked ? "   🔒 [GUARD ACTIVE] System will protect this driver's history." : "   🔓 [INFO] Not a driver, safe to delete.");

            // الاحتمال الثاني (خطأ): شخص وهمي تماماً
            int fakeID = 999999;
            bool fakeLinked = ClsPerson.IsPersonLinkedToDriver(fakeID);
            Console.WriteLine($"[Error Scenario] Is Fake ID [{fakeID}] linked to a Driver? {fakeLinked}");

            if (fakeLinked == false)
            {
                Console.WriteLine("   ✅ [SUCCESS] Correct! System returned False for fake ID.");
            }
            else
            {
                Console.WriteLine("   ❌ [BUG] Detected a bug in the Driver lookup logic!");
            }
        }

        public static void TestDeletePerson_Isolated()
        {
            Console.WriteLine("\n--- [Isolated Test] DeletePerson ---");

            // السيناريو الأول: احتمال المنع (حذف شخص مربوط بـ User أو Driver مثل ID = 1)
            int linkedID = 46;
            Console.WriteLine($"A) Trying to delete a LINKED person ID [{linkedID}]:");
            if (!ClsPerson.DeletePerson(linkedID))
            {
                Console.WriteLine("   ✅ [CORRECT BEHAVIOUR] Guard blocked the deletion safely! Database constraint protected.");
            }
            else
            {
                Console.WriteLine("   ❌ [BUG] System deleted a linked person! This will break database integrity.");
            }

            // السيناريو الثاني: احتمال النجاح أو عدم الوجود (ID وهمي أو شخص غير مربوط تماماً)
            int fakeID = 999999;
            Console.WriteLine($"\nB) Trying to delete a FAKE person ID [{fakeID}]:");
            if (!ClsPerson.DeletePerson(fakeID))
            {
                Console.WriteLine("   ✅ [CORRECT BEHAVIOUR] Returned false because the person doesn't exist.");
            }
        }

        public static void TestDeleteByNationalNo_Isolated()
        {
            Console.WriteLine("\n--- [Isolated Test] DeleteByNationalNo ---");

            // السيناريو الأول: رقم وطني لشخص مربوط (مثلاً N123)
            string linkedNationalNo = "N10043";
            Console.WriteLine($"A) Trying to delete a LINKED NationalNo [{linkedNationalNo}]:");
            if (!ClsPerson.DeleteByNationalNo(linkedNationalNo))
            {
                Console.WriteLine("   ✅ [CORRECT BEHAVIOUR] Guard protected the system and refused to delete.");
            }
            else
            {
                Console.WriteLine("   ❌ [BUG] System allowed deleting a linked NationalNo!");
            }

            // السيناريو الثاني: رقم وطني وهمي تماماً
            string fakeNationalNo = "FAKE-999";
            Console.WriteLine($"\nB) Trying to delete a FAKE NationalNo [{fakeNationalNo}]:");
            if (!ClsPerson.DeleteByNationalNo(fakeNationalNo))
            {
                Console.WriteLine("   ✅ [CORRECT BEHAVIOUR] Returned false safely because NationalNo does not exist.");
            }
        }

        public static void TestGetAllPeople_Isolated()
        {
            Console.WriteLine("\n--- [Isolated Test] GetAllPeople ---");

            DataTable dt = ClsPerson.GetAllPeople();
            if (dt != null)
            {
                Console.WriteLine($"   ✅ [SUCCESS] Retrieved {dt.Rows.Count} rows from Persons Table.");
            }
            else
            {
                Console.WriteLine("   ❌ [ERROR] DataTable returned null. Check connection or SQL query!");
            }
        }

        public static void TestGetAllPeopleWithCountryName_Isolated()
        {
            Console.WriteLine("\n--- [Isolated Test] GetAllPeopleWithCountryName ---");

            DataTable dt = ClsPerson.GetAllPeopleWithCountryName();
            if (dt != null && dt.Rows.Count > 0)
            {
                Console.WriteLine($"   ✅ [SUCCESS] Retrieved {dt.Rows.Count} rows with country names.");

                // التحقق من وجود عمود اسم الدولة فعلياً لتجنب مشاكل الـ Binding في الـ Grid
                if (dt.Columns.Contains("CountryName"))
                {
                    Console.WriteLine($"   ✅ [CHECK PASS] Column 'CountryName' exists. First row value: {dt.Rows[0]["CountryName"]}");
                }
                else
                {
                    Console.WriteLine("   ⚠️ [WARNING] Column 'CountryName' is missing from the query results!");
                }
            }
            else
            {
                Console.WriteLine("   ❌ [ERROR] Query returned no data or failed.");
            }
        }

        public static void TestIsPersonExistByID_Isolated()
        {
            Console.WriteLine("\n--- [Isolated Test] IsPersonExistByID ---");

            // الاحتمال الأول: ID حقيقي موجود
            int validID = 1;
            bool existValid = ClsPerson.IsPersonExistByID(validID);
            Console.WriteLine($"   - Existing ID [{validID}] Check: {(existValid ? "✅ FOUND (Correct)" : "❌ NOT FOUND (Bug)")}");

            // الاحتمال الثاني: ID وهمي
            int fakeID = 999999;
            bool existFake = ClsPerson.IsPersonExistByID(fakeID);
            Console.WriteLine($"   - Fake ID [{fakeID}] Check: {(existFake ? "❌ FOUND (Bug)" : "✅ NOT FOUND (Correct)")}");
        }

        public static void TestIsPersonExistByNationalNo_Isolated()
        {
            Console.WriteLine("\n--- [Isolated Test] IsPersonExistByNationalNo ---");

            // الاحتمال الأول: رقم وطني موجود
            string validNo = "N10030";
            bool existValid = ClsPerson.IsPersonExistByNationalNo(validNo);
            Console.WriteLine($"   - Existing NationalNo [{validNo}] Check: {(existValid ? "✅ FOUND (Correct)" : "❌ NOT FOUND (Bug)")}");

            // الاحتمال الثاني: رقم وطني وهمي
            string fakeNo = "FAKE-XYZ";
            bool existFake = ClsPerson.IsPersonExistByNationalNo(fakeNo);
            Console.WriteLine($"   - Fake NationalNo [{fakeNo}] Check: {(existFake ? "❌ FOUND (Bug)" : "✅ NOT FOUND (Correct)")}");
        }

        public static void TestAddNewPersonViaSave_Isolated()
        {
            Console.WriteLine("\n--- [Isolated Test] Save (AddNew Mode) ---");

            // إنشاء كائن جديد تماماً (الـ Mode الافتراضي له هو AddNew)
            ClsPerson person = new ClsPerson();

            // توليد رقم وطني عشوائي للتجربة حتى لا يصطدم بحاجز التكرار عند إعادة التشغيل
            
            //string uniqueNationalNo = "N-REG-" + new Random().Next(10000, 99999);
            string uniqueNationalNo = "N-REG-38268";

            person.NationalNo = uniqueNationalNo;
            person.FirstName = "Basem";
            person.SecondName = "Mosad";
            person.ThirdName = "Eid"; // حقل يقبل Null مأمن بـ ""
            person.LastName = "Mansour";
            person.DateOfBirth = new DateTime(1990, 8, 20); // عمره حوالي 36 سنة في 2026
            person.Address = "Giza, Egypt";
            person.Phone1 = "01122334455";
            person.NationalityCountryID = 8; // تأكد من وجود ID دولة صالح في قاعدة بياناتك

            Console.WriteLine($"[Action] Attempting to save new person with NationalNo: {uniqueNationalNo}...");

            if (person.Save())
            {
                Console.WriteLine("   ✅ [SUCCESS] Person saved successfully to the database!");
                Console.WriteLine($"   👉 New Generated PersonID: {person.PersonID}");
                Console.WriteLine($"   👉 Calculated Full Name: {person.FullName}");
                Console.WriteLine($"   👉 Calculated Age: {person.Age} years.");
            }
            else
            {
                Console.WriteLine("   ❌ [FAILED] Save operation failed. Check if NationalNo is duplicated or CountryID doesn't exist.");
            }
        }

        public static void TestUpdatePersonViaSave_Isolated(int existingPersonID)
        {
            Console.WriteLine("\n--- [Isolated Test] Save (Update Mode & Guard Check) ---");

            // 1. شحن بيانات الشخص من قاعدة البيانات (يتحول الـ Mode تلقائياً إلى Update)
            ClsPerson person = ClsPerson.Find(existingPersonID);

            if (person == null)
            {
                Console.WriteLine($"   ❌ [ABORTED] Cannot run test. Person ID [{existingPersonID}] not found in DB.");
                return;
            }

            Console.WriteLine($"[Loaded] Person Found: {person.FullName} | Current NationalNo: {person.NationalNo}");

            // -------------------------------------------------------------
            // السيناريو الأول: احتمال تعديل عادي ناجح (تغيير العنوان والاسم الثاني)
            // -------------------------------------------------------------
            person.SecondName = "Kamal";
            person.Address = "New Cairo, Block 5";

            Console.WriteLine("\nScenario A: Testing valid data update...");
            if (person.Save())
            {
                // إعادة القراءة للتأكد من حفظ التعديلات
                ClsPerson updatedPerson = ClsPerson.Find(existingPersonID);
                Console.WriteLine($"   ✅ [SUCCESS] Update allowed! New Full Name: {updatedPerson.FullName} | New Address: {updatedPerson.Address}");
            }
            else
            {
                Console.WriteLine("   ❌ [BUG] Valid update was rejected!");
            }

            // -------------------------------------------------------------
            // السيناريو الثاني: احتمال خطأ (محاولة إدخال رقم وطني يخص شخصاً آخر بالسيستم)
            // -------------------------------------------------------------
            Console.WriteLine("\nScenario B: Testing 'Smart Guard' against NationalNo theft...");

            // سنقوم بتعيين رقم وطني مسجل لشخص آخر مسبقاً (تأكد من وجود هذا الرقم لشخص آخر في جهازك)
            person.NationalNo = "N10044";

            if (!person.Save())
            {
                Console.WriteLine("   ✅ [CORRECT BEHAVIOUR] The Save function blocked the update! Guard protected the system from duplicate NationalNo constraint crash.");
            }
            else
            {
                Console.WriteLine("   ❌ [CRITICAL BUG DETECTED] Guard failed! The system allowed updating this person with another person's unique NationalNo!");
            }
        }

        public static void TestFindMethods_Isolated()
        {
            Console.WriteLine("\n--- [Isolated Test] Find & FindByNationalNo ---");

            // الاحتمال الأول: البحث عن شخص موجود فعلاً (مثلاً ID = 1)
            int targetID = 1;
            ClsPerson personByID = ClsPerson.Find(targetID);

            if (personByID != null)
            {
                Console.WriteLine($"   ✅ [SUCCESS] Find(ID) matched! Full Name: {personByID.FullName}");

                // اختبار البحث بالرقم الوطني لنفس الشخص المسترجع
                ClsPerson personByNo = ClsPerson.FindByNationalNo(personByID.NationalNo);
                if (personByNo != null && personByNo.PersonID == personByID.PersonID)
                {
                    Console.WriteLine($"   ✅ [SUCCESS] FindByNationalNo matched the exact same person ID [{personByNo.PersonID}].");
                }
            }
            else
            {
                Console.WriteLine($"   ⚠️ [INFO] Person ID [{targetID}] not found. Put a real ID to fully test successful lookup.");
            }

            // الاحتمال الثاني (خطأ): البحث عن قيم وهمية تماماً
            Console.WriteLine("\nTesting fake lookups:");
            ClsPerson fakePerson = ClsPerson.Find(999999);
            if (fakePerson == null)
            {
                Console.WriteLine("   ✅ [CORRECT BEHAVIOUR] Find(999999) safely returned NULL.");
            }
            else
            {
                Console.WriteLine("   ❌ [BUG] Find returned an instantiated object for a fake ID!");
            }
        }

        public static void TestInvalidInputsAndExceptions_Isolated()
        {
            Console.WriteLine("\n==================================================");
            Console.WriteLine("    TESTING INVALID INPUTS & SYSTEM RESILIENCE    ");
            Console.WriteLine("==================================================\n");

            // -------------------------------------------------------------
            // احتمال الغلط 1: الموظف ساب الحقول الإلزامية فاضية أو بعتها Null
            // -------------------------------------------------------------
            Console.WriteLine("Scenario 1: Sending NULL/Empty values to Required Fields:");
            ClsPerson brokenPerson = new ClsPerson();

            brokenPerson.NationalNo = ""; // غلط: الرقم الوطني إجباري وفريد
            brokenPerson.FirstName = null; // غلط كارثي: الاسم الأول NOT NULL في الداتا بيز
            brokenPerson.LastName = "Gharib";
            brokenPerson.NationalityCountryID = 1;

            try
            {
                Console.WriteLine("[Action] Attempting to save a person with NULL FirstName...");
                // المفروض الدالة ترجع false أو ترمي Exception تمنع الكراش
                if (!brokenPerson.Save())
                {
                    Console.WriteLine("   ✅ [CORRECT BEHAVIOUR] System safely rejected saving empty/null required fields.");
                }
                else
                {
                    Console.WriteLine("   ❌ [BUG] System allowed saving a person with a NULL FirstName! This will crash the DB.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ✅ [SAFE HANDLED] The system threw an exception instead of crashing the app: {ex.Message}");
            }


            Console.WriteLine("\n--------------------------------------------------\n");


            // -------------------------------------------------------------
            // احتمال الغلط 2: الموظف دخل تاريخ ميلاد غير منطقي (في المستقبل أو طفل رضيع)
            // -------------------------------------------------------------
            Console.WriteLine("Scenario 2: Testing Invalid/Future DateOfBirth:");
            ClsPerson babyPerson = new ClsPerson();

            // الموظف دخل تاريخ ميلاد في المستقبل (مثلاً سنة 2028 وإحنا في 2026!)
            babyPerson.DateOfBirth = DateTime.Now.AddYears(2);
            Console.WriteLine($"   - Future Date entered: {babyPerson.DateOfBirth.ToShortDateString()}");
            Console.WriteLine($"   - Calculated Age property returned: {babyPerson.Age}");

            if (babyPerson.Age < 0)
            {
                Console.WriteLine("   ⚠️ [LOGIC WARNING] Property returned negative age. You should add validation in UI to block future dates!");
            }

            // الموظف دخل تاريخ ميلاد لطفل عنده سنة واحدة (والسيستم رخص قيادة يعني لازم فوق 18 سنة)
            babyPerson.DateOfBirth = DateTime.Now.AddYears(-1);
            Console.WriteLine($"   - Toddler Date entered: {babyPerson.DateOfBirth.ToShortDateString()}");
            Console.WriteLine($"   - Calculated Age property returned: {babyPerson.Age} years old.");


            Console.WriteLine("\n--------------------------------------------------\n");


            // -------------------------------------------------------------
            // احتمال الغلط 3: كود دولة وهمي أو بـ -1 (-1NationalityCountryID =)
            // -------------------------------------------------------------
            Console.WriteLine("Scenario 3: Testing Invalid/Fake Country ID lookup:");
            ClsPerson countryTestPerson = new ClsPerson();

            // القيمة الافتراضية للـ ID هي -1 (يعني مفيش دولة اختيرت)
            countryTestPerson.NationalityCountryID = -1;
            Console.WriteLine($"   A) CountryID is [-1] -> Property CountryName returned: \"{countryTestPerson.CountryName}\"");
            if (countryTestPerson.CountryName == "")
            {
                Console.WriteLine("      ✅ [SUCCESS] CountryName protected itself from -1 and returned empty string safely.");
            }

            // الموظف باصى ID دولة مش موجود في جدول الدول أصلاً (مثلاً 99999)
            countryTestPerson.NationalityCountryID = 99999;
            Console.WriteLine($"   B) CountryID is [99999] (Fake) -> Property CountryName returned: \"{countryTestPerson.CountryName}\"");
            if (countryTestPerson.CountryName == "")
            {
                Console.WriteLine("      ✅ [SUCCESS] CountryName safely handled non-existing Country ID without crashing the app.");
            }
            else
            {
                Console.WriteLine("   ❌ [BUG] CountryName property crashed or behaved unexpectedly!");
            }

            Console.WriteLine("\n==================================================");
            Console.WriteLine("             END OF INVALID INPUT TEST            ");
            Console.WriteLine("==================================================");
        }
    }
}
