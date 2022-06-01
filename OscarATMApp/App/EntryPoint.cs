using OscarATMApp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarATMApp.App
{
    internal class EntryPoint
    {
        static void Main(string[] args)
        {
            AppDisplay.Welcome();
            ATMApp atmApp = new ATMApp();
            atmApp.InitializeData();
            atmApp.ConfirmUserCardNumberAndPin();
            atmApp.Welcome();

            Utility.PressEnterToContinue();
        }
    }
}
