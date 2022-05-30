using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarATMApp.UI
{
    public static class AppDisplay
    {
        internal static void Welcome()
        {
            Console.Clear();
            Console.Title = "Oscar Bank ATM";
            Console.ForegroundColor = ConsoleColor.White;


            Console.WriteLine("\n\n==========Welcome to Oscar Bank==========\n\n");
            Console.WriteLine("Provide your card number and pin");
            Console.WriteLine("Please note that you do not need physical ATM card here. Thanks");

            Utility.PressEnterToContinue();
        }

    }
}
