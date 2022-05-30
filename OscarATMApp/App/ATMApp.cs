using OscarATMApp.UI;
using System;

namespace OscarATMApp
{
    internal class ATMApp
    {
        static void Main(string[] args)
        {
            AppDisplay.Welcome();
            long cardNumber = Validator.Convert<long>("Your card number");
            Console.WriteLine($"Your card number is {cardNumber}");

            Utility.PressEnterToContinue();
        }
    }
}
