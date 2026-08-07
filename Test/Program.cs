using DVLD_BusinessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    internal class Program
    {
       
        static void Main(string[] args)
        {
            Console.WriteLine("HASH:");
            Console.WriteLine(ClsEncryption.ComputeHash("123456789"));

            //Console.WriteLine(ClsEncryption.HashPasswordWithSalt("123456789",out ));
        }
    }
}
