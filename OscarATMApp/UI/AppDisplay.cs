using OscarATMApp.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        internal static UserAccount UserLoginForm()
        {
            UserAccount tempUserAccount = new UserAccount();

            tempUserAccount.CardNumber = Validator.Convert<long>("Your card number.");
            tempUserAccount.CardPin = Convert.ToInt32(Utility.GetSecretInput("Enter your Card PIN"));
            return tempUserAccount;
        }
        internal static void LoginProgress()
        {  
            Console.WriteLine("\nChecking card number and PIN...");
            Utility.PrintDotAnimation();            
        }

        internal static void PrintLockScreen()
        {
            Console.Clear();
            Utility.PrintMessage("Your account is locked. Please visit the nearest branch to unlock your account. Thank you.", true);
            Utility.PressEnterToContinue();
            Environment.Exit(1);
        }
        internal static void WelcomeCustomer(string fullName)
        {
            Console.WriteLine($"Welcome back, {fullName}");
            Utility.PressEnterToContinue();
        } 

        internal static void DisplayATMMenu()
        {
            Console.Clear();
            Console.WriteLine("---------- Menu ----------");
            Console.WriteLine(":                        :");
            Console.WriteLine("1. Account Balance       :");
            Console.WriteLine("2. Cash Deposit          :");
            Console.WriteLine("3. Withdrawal            :");
            Console.WriteLine("4. Transafer             :");
            Console.WriteLine("5. Transactions          :");
            Console.WriteLine("6. Logout                :");

        }

        internal static void LogoutProgress()
        {
            Console.WriteLine("Thank you for banking with us.");
            Utility.PrintDotAnimation();
            Console.Clear();
        }

    }
}
