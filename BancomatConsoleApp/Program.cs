using BancomatClassLibrary;
using System;
using System.Collections.Generic;

namespace BancomatConsoleApp
{
    internal class Program
    {
        static Bank[] banks = {
            new Bank("Моно банк"),
            new Bank("Приват банк")
        };

        static Bank selectedBank;
        static AutomatedTellerMachine activeBankomat;
        static Account currentAccount;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            foreach (var bank in banks)
            {
                bank.Message += BankMessageHandler;
            }

            InitializeAtms();
            ShowMainMenu();
        }

        static void InitializeAtms()
        {
            banks[0].AddAtm(1, "Велика Бердичiвська 52", 20000);
            banks[0].AddAtm(1, "Довженка 43", 50000);
            banks[1].AddAtm(2, "Київська 57", 1000);
            banks[1].AddAtm(2, "Михайлівська 57", 20000);
        }

        static void ShowMainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Головне меню:");
                int choice = DisplayMenu(new List<string> { "Обрати банк", "Вихід" });

                if (choice == 1) return;

                SelectBank();
            }
        }

        static void SelectBank()
        {
            Console.Clear();
            Console.WriteLine("Оберіть банк:");
            List<string> bankNames = new List<string>();

            foreach (var bank in banks)
            {
                bankNames.Add(bank.BankName);
            }
            bankNames.Add("Назад");

            int choice = DisplayMenu(bankNames);
            if (choice == bankNames.Count - 1) return;

            selectedBank = banks[choice];
            SelectAtm();
        }

        static void SelectAtm()
        {
            Console.Clear();
            Console.WriteLine($"Банк: {selectedBank.BankName}");
            Console.WriteLine("Оберіть банкомат:");

            List<string> atmAddresses = new List<string>();
            foreach (var atm in selectedBank.AtmList)
            {
                atmAddresses.Add(atm.GetBankomatAddress());
            }
            atmAddresses.Add("Назад");

            int choice = DisplayMenu(atmAddresses);
            if (choice == atmAddresses.Count - 1) return;

            activeBankomat = selectedBank.AtmList[choice];
            ShowAuthenticationMenu();
        }

        static void ShowAuthenticationMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Авторизація:");
                int choice = DisplayMenu(new List<string> { "Авторизуватися", "Зареєструватися", "Назад" });

                if (choice == 2) return;

                if (choice == 0)
                {
                    if (AuthenticateUser())
                    {
                        ShowAccountMenu();
                    }
                }
                else
                {
                    CreateNewAccount();
                }
            }
        }

        static void ShowAccountMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Вітаємо, {currentAccount.Name}!");

                int choice = DisplayMenu(new List<string>
                {
                    "Переглянути баланс",
                    "Зняти кошти",
                    "Поповнити рахунок",
                    "Перерахувати кошти",
                    "Назад"
                });

                switch (choice)
                {
                    case 0:
                        Console.Clear();
                        Console.WriteLine($"Ваш баланс: {currentAccount.GetBalance()} грн");
                        break;
                    case 1:
                        PerformTransaction(activeBankomat.WithDrawMoney, "зняття");
                        break;
                    case 2:
                        PerformTransaction(activeBankomat.PutMoney, "поповнення");
                        break;
                    case 3:
                        TransferFunds();
                        break;
                    case 4:
                        return;
                }
                Console.ReadKey();
            }
        }

        static bool AuthenticateUser()
        {
            Console.Clear();
            Console.WriteLine("Введіть номер картки:");
            string accountNumber = Console.ReadLine();
            Console.WriteLine("Введіть пін-код:");
            string pinCode = Console.ReadLine();

            if (selectedBank.Authenticate(accountNumber, pinCode))
            {
                currentAccount = selectedBank.FindAccount(accountNumber);
                return true;
            }
            else
            {
                Console.WriteLine("Аутентифікація не вдалася.");
                Console.ReadLine();
                return false;
            }
        }

        static int DisplayMenu(List<string> options)
        {
            while (true)
            {
                for (int i = 0; i < options.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {options[i]}");
                }

                Console.Write("Ваш вибір: ");
                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= options.Count)
                {
                    return choice - 1;
                }

                Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
            }
        }

        static void BankMessageHandler(object sender, MessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        static void PerformTransaction(Func<Account, double, bool> transactionAction, string operation)
        {
            double amount = PromptForAmount($"Введіть суму для {operation}: ");
            if (amount <= 0) return;

            if (!transactionAction(currentAccount, amount))
            {
                Console.WriteLine($"Не вдалося завершити операцію {operation}.");
            }
        }

        static void TransferFunds()
        {
            Console.Clear();
            Console.WriteLine("Введіть номер рахунку отримувача:");
            string receiverAccountNumber = Console.ReadLine();
            double amount = PromptForAmount("Введіть суму для перерахування: ");

            if (amount <= 0) return;

            selectedBank.TransferFunds(currentAccount.CardNumber, receiverAccountNumber, amount);
        }

        static double PromptForAmount(string message)
        {
            Console.Clear();
            Console.WriteLine(message);
            if (double.TryParse(Console.ReadLine(), out double amount) && amount > 0)
            {
                return amount;
            }
            else
            {
                Console.WriteLine("Невірний формат суми.");
                return -1;
            }
        }
        static void CreateNewAccount()
        {
            Console.Clear();
            Console.WriteLine("Введіть своє ім'я:");
            string name = Console.ReadLine();
            Console.WriteLine("Введіть пін-код (4 цифри):");
            string pinCode = Console.ReadLine();

            if (selectedBank.CreateAccount(name, pinCode))
            {
                Console.WriteLine("Акаунт створено успішно.");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Помилка при створенні акаунту.");
            }
        }
    }
}