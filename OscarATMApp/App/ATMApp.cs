using ConsoleTables;
using OscarATMApp.Model.Entities;
using OscarATMApp.Model.Enums;
using OscarATMApp.Model.Interfaces;
using OscarATMApp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OscarATMApp
{
    public class ATMApp : IUserLogin, IUserAccountActions, ITransaction
    {
        private List<UserAccount> userAccountList;
        private UserAccount selectedAccount;
        private List<Transaction> _listOfTransactions;
        private const decimal minimumKeptAmount = 500;
        private readonly AppDisplay display;

        public ATMApp()
        {
            display = new AppDisplay();
        }

        public void Run()
        {
            AppDisplay.Welcome();
            ConfirmUserCardNumberAndPin();
            AppDisplay.WelcomeCustomer(selectedAccount.FullName);
            while (true)
            {
                AppDisplay.DisplayATMMenu();
                ProcessMenuOption();
            }
            
        }
        public void InitializeData()
        {
            userAccountList = new List<UserAccount>
            {
                new UserAccount{Id = 1, FullName = "Yinka Adeyemi", AccountNumber = 1133557799, CardNumber = 654321, CardPin = 123456, AccountBalance = 10000.00M, IsLocked = false},
                new UserAccount{Id = 2, FullName = "Faith Andgo", AccountNumber = 2233557799, CardNumber = 754321, CardPin = 123457, AccountBalance = 5000.00M, IsLocked = false},
                new UserAccount{Id = 3, FullName = "Samuel Oloba", AccountNumber = 3333557799, CardNumber = 854321, CardPin = 123458, AccountBalance = 20000.00M, IsLocked = false}
            };
            _listOfTransactions = new List<Transaction>();
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
                    MakeDeposit();
                    break;

                case (int)ATMMenu.MakeWithdrawal:
                    MakeWithdrawal();
                    break;

                case (int)ATMMenu.MakeTransfer:
                    var internalTransfer = display.InternalTransferForm();
                    ProcessInternalTransfer(internalTransfer);
                    break;

                case (int)ATMMenu.ViewTransactions:
                    ViewTransaction();
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
            Console.WriteLine("\nOnly multiples of 500 and 100 naira are allowed.\n");
            var transaction_amt = Validator.Convert<int>($"amount {AppDisplay.cur}");

            //To simulate counting
            Console.WriteLine("\nChecking and counting bank notes.");
            Utility.PrintDotAnimation();
            Console.WriteLine("");

            //Some gaurd clause
            if (transaction_amt <= 0)
            {
                Utility.PrintMessage("Amount needs to be greater than zero. Try again later.", false);
                return;
            }
            if (transaction_amt % 500 != 0)
            {
                Utility.PrintMessage($"Enter deposit amount in multiple of 500 or 100. Try again.", false);
                return;
            }

            if (PreviewBankNotesCount(transaction_amt) == false)
            {
                Utility.PrintMessage($"You have cancelled your action.", false);
                return;
            }

            //bind transaction details to transaction object
            InsertTransaction(selectedAccount.Id, TransactionType.Deposit, transaction_amt, "");

            //update account balance
            selectedAccount.AccountBalance += transaction_amt;

            //print success messgae
            Utility.PrintMessage($"Your deposit of {Utility.FormatAmount(transaction_amt)} was" +
                $"successful.", true);
        }

        public void MakeWithdrawal()
        {
            var transaction_amt = 0;
            int selectedAmount = AppDisplay.SelectAmount();
            if (selectedAmount == -1)
            {
                //recursive calling of same method
                MakeWithdrawal();
                return;
            } else if (selectedAmount != 0)
            {
                transaction_amt = selectedAmount;
            }
            else
            {
                transaction_amt = Validator.Convert<int>($"amount {AppDisplay.cur}");
            }

            //input validation on transaction amount
            if(transaction_amt <= 0)
            {
                Utility.PrintMessage("Amount needs to be greater than zero. Try again", false);
                return;
            }
            if(transaction_amt % 500 !=0 )
            {
                Utility.PrintMessage("You can only withdraw amount in multiple of 500 or 1000 naira. Try again.", false);
                return;
            }
            //Business logic validations

            if (transaction_amt > selectedAccount.AccountBalance)
            {
                Utility.PrintMessage($"Insuficient balance. Fund this account to make withdrawal" +
                    $"{Utility.FormatAmount(transaction_amt)}", false);
                return;
            }
            if((selectedAccount.AccountBalance - transaction_amt) < minimumKeptAmount)
            {
                Utility.PrintMessage($"Insuficient balance. You need to have minimum of " +
                    $"minimum {Utility.FormatAmount(minimumKeptAmount)}", false);
                return;
            }
            //Bind withdrawal details to transaction object
            InsertTransaction(selectedAccount.Id, TransactionType.Withdrawal, -transaction_amt, "");
            //Update account balance
            selectedAccount.AccountBalance -= transaction_amt;
            //success message
            Utility.PrintMessage($"You have successfully wmade withrawal of" +
                $"{Utility.FormatAmount(transaction_amt)}.", true);
        }

        private bool PreviewBankNotesCount(int amount)
        {
            int thousandNotesCount = amount / 1000;
            int fiveHundredNotesCount = (amount % 1000) / 500;

            Console.WriteLine("\nSummary");
            Console.WriteLine("--------");
            Console.WriteLine($"{AppDisplay.cur}1000 x {thousandNotesCount} = {1000 * thousandNotesCount}");
            Console.WriteLine($"{AppDisplay.cur}500 x {fiveHundredNotesCount} = {500 * fiveHundredNotesCount}");
            Console.WriteLine($"Total amount: {Utility.FormatAmount(amount)}\n\n");

            int opt = Validator.Convert<int>("1 to confirm");
            return opt.Equals(1);
        }

        public void InsertTransaction(long _UserBankAccountId, TransactionType _tranType, decimal _tranAmount, string _desc)
        {
            //create a new transaction object
            var transaction = new Transaction()
            {
                TransactionId = Utility.GetTransactionId(),
                UserBankAccountId = _UserBankAccountId,
                TransactionDate = DateTime.Now,
                TransactionType = _tranType,
                TransactionAmount = _tranAmount,
                Description = _desc
            };

            //add transaction object to the list
            _listOfTransactions.Add(transaction);
        }

        public void ViewTransaction()
        {
            var filteredTransactionList = _listOfTransactions.Where(t => t.UserBankAccountId == selectedAccount.Id).ToList();
            //check if there is transaction yet
            if(filteredTransactionList.Count <= 0)
            {
                Utility.PrintMessage("You have no transaction yet.", true);
            }
            else
            {
                var table = new ConsoleTable("Id", "Transaction Date", "Type", "Description", "Amount " + AppDisplay.cur);
                foreach(var tran in filteredTransactionList)
                {
                    table.AddRow(tran.TransactionId,tran.TransactionDate,tran.TransactionType, tran.Description, tran.TransactionAmount);
                }
                table.Options.EnableCount = false;
                table.Write();
                Utility.PrintMessage($"You have {filteredTransactionList.Count} transaction(s)", true);
            }
        }
        private void ProcessInternalTransfer(InternalTransfer internalTransfer)
        {
            if(internalTransfer.TransferAmount <= 0)
            {
                Utility.PrintMessage("Amount needs to be more than zero. Try again.", false);
                return;
            }
            //check sender's account balance
            if(internalTransfer.TransferAmount > selectedAccount.AccountBalance)
            {
                Utility.PrintMessage($"Transfer failed due to insuficient balance" +
                    $" to transfer {Utility.FormatAmount(internalTransfer.TransferAmount)}", false);
                return;
            }
            //check the minimum kept amount
            if((selectedAccount.AccountBalance - internalTransfer.TransferAmount) < minimumKeptAmount)
            {
                Utility.PrintMessage($"Transfer fail. Your account needs to have minimum" +
                    $"{ Utility.FormatAmount(minimumKeptAmount)}", false);
            }

            //check receiver's account number is valid
            var selectedBankAccountReceiver = (from userAcc in userAccountList
                                               where userAcc.AccountNumber == internalTransfer.RecipientBankAccountNumber
                                               select userAcc).FirstOrDefault();
             if (selectedBankAccountReceiver == null)
            {
                Utility.PrintMessage("Transfer failed. Receiver bank account number is invalid.", false);
                return;
            }
             //check receivers's name
             if(selectedBankAccountReceiver.FullName != internalTransfer.RecipientBankAccountName)
            {
                Utility.PrintMessage("Transfer failed. Recipient's bank account name does not match.", false);
                return;
            }

            //Add transaction to transactions record - sender
            InsertTransaction(selectedAccount.Id, TransactionType.Transfer, -internalTransfer.TransferAmount, "Transfered " +
                $"to {selectedBankAccountReceiver.AccountNumber} ({selectedBankAccountReceiver.FullName})");
            //update sender's account balance
            selectedAccount.AccountBalance -= internalTransfer.TransferAmount;

            //Add transaction record - receiver
            InsertTransaction(selectedBankAccountReceiver.Id, TransactionType.Transfer, internalTransfer.TransferAmount, "Transfered from" +
                $"{selectedAccount.AccountNumber}({selectedAccount.FullName})");
            //update receiver's account balance
            selectedBankAccountReceiver.AccountBalance += internalTransfer.TransferAmount;
            //print success message
            Utility.PrintMessage($"You have successfully transfered" +
                $" {Utility.FormatAmount(internalTransfer.TransferAmount)} to " +
                $"{internalTransfer.RecipientBankAccountName}", true);
        }
    }
}
