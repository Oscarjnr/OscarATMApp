using OscarATMApp.Model.Entities;
using OscarATMApp.Model.Enums;
using OscarATMApp.Model.Interfaces;
using OscarATMApp.UI;
using System;
using System.Collections.Generic;
using System.Threading;

namespace OscarATMApp
{
    public class ATMApp : IUserLogin, IUserAccountActions
    {
        private List<UserAccount> userAccountList;
        private UserAccount selectedAccount;

        public void Run()
        {
            AppDisplay.Welcome();
            ConfirmUserCardNumberAndPin();
            AppDisplay.WelcomeCustomer(selectedAccount.FullName);
            AppDisplay.DisplayATMMenu();
            ProcessMenuOption();
        }
        public void InitializeData()
        {
            userAccountList = new List<UserAccount>
            {
                new UserAccount{Id = 1, FullName = "Yinka Adeyemi", AccountNumber = 1133557799, CardNumber = 654321, CardPin = 123456, AccountBalance = 10000.00M, IsLocked = false},
                new UserAccount{Id = 2, FullName = "Faith Andgo", AccountNumber = 2233557799, CardNumber = 754321, CardPin = 123457, AccountBalance = 5000.00M, IsLocked = false},
                new UserAccount{Id = 3, FullName = "Samuel Oloba", AccountNumber = 3333557799, CardNumber = 854321, CardPin = 123458, AccountBalance = 20000.00M, IsLocked = false}
            };
        }

        public void ConfirmUserCardNumberAndPin()
        {
            bool isCorrectLogin = false;
            while(isCorrectLogin == false)
            {
                UserAccount inputAccount = AppDisplay.UserLoginForm();
                AppDisplay.LoginProgress();
                foreach(UserAccount account in userAccountList)
                {
                    selectedAccount = account;
                    if (inputAccount.CardNumber.Equals(selectedAccount.CardNumber))
                    {
                        selectedAccount.TotalLogin++;

                        if (inputAccount.CardPin.Equals(selectedAccount.CardPin))
                        {
                            selectedAccount = account;

                            if(selectedAccount.IsLocked || selectedAccount.TotalLogin > 3)
                            {
                                //Print a lock message
                                AppDisplay.PrintLockScreen();
                            }
                            else
                            {
                                selectedAccount.TotalLogin = 0;
                                isCorrectLogin = true;
                                break;
                            }
                        }
                    }
                    if (isCorrectLogin == false)
                    {
                        Utility.PrintMessage("\nInvalid card number or PIN.", false);
                        selectedAccount.IsLocked = selectedAccount.TotalLogin == 3;
                        if (selectedAccount.IsLocked)
                        {
                            AppDisplay.PrintLockScreen();
                        }
                    }
                    Console.Clear();
                }
            }

        }

        private void ProcessMenuOption()
        {
            switch (Validator.Convert<int>("an option:"))
            {
                case (int)ATMMenu.CheckBalance:
                    CheckBalance();
                    break;

                case (int)ATMMenu.MakeDeposit:
                    Console.WriteLine("Making deposit...");
                    break;

                case (int)ATMMenu.MakeWithdrawal:
                    Console.WriteLine("Making withdrawal...");
                    break;

                case (int)ATMMenu.MakeTransfer:
                    Console.WriteLine("Making transfer...");
                    break;

                case (int)ATMMenu.ViewTransactions:
                    Console.WriteLine("Viewing transactions...");
                    break;

                case (int)ATMMenu.Logout:
                    AppDisplay.LogoutProgress();
                    Utility.PrintMessage("You have successfully logged out.");
                    Run();
                    break;

                default:
                    Utility.PrintMessage("Invalid Option.", false);
                    break;
            }
        }

        public void CheckBalance()
        {
            Utility.PrintMessage($"Your account balance is: {Utility.FormatAmount(selectedAccount.AccountBalance)}");
        }

        public void MakeDeposit()
        {
            throw new NotImplementedException();
        }

        public void MakeWithdrawal()
        {
            throw new NotImplementedException();
        }
    }
}
