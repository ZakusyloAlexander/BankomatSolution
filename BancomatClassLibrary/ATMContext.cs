
using System;

namespace BancomatClassLibrary
{
    public class Account
    {
        public string Name { get; private set; }
        public string CardNumber { get; private set; }
        public string PinCode { get; private set; }
        public double CardBalance { get; private set; }
        public event EventHandler<MessageEventArgs> Message;

        public event EventHandler<DrawMoneyArgs> DrawMoneyHandler;

        public Account(string name, string cardNumber, string pinCode)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name), "Ім'я не може бути пустим");

            if (cardNumber.Length != 6 || !ulong.TryParse(cardNumber, out _))
                throw new ArgumentException("Номер картки повинен містити 6 цифр");

            if (pinCode.Length != 4 || !int.TryParse(pinCode, out _))
                throw new ArgumentException("Пін-код повинен містити 4 цифри");

            Name = name;
            CardNumber = cardNumber;
            PinCode = pinCode;
            CardBalance = 0.0;
        }

        public bool AddToBalance(double amount)
        {
            if (amount <= 0)
            {
                Notify($"Введена сума некоректна. Баланс не можна поповнити.");
                return false;
            }

            CardBalance += amount;
            Notify($"Баланс успішно поповнено на {amount} грн. Поточний баланс: {CardBalance} грн.");
            return true;
        }

        public double GetBalance()
        {
            Notify($"Ваш баланс: {CardBalance} грн.");
            return CardBalance;
        }

        public bool Withdraw(double amount)
        {
            if (amount <= 0)
            {
                Notify($"Некоректна сума для зняття коштів.");
                return false;
            }

            if (CardBalance < amount)
            {
                Notify($"Недостатньо коштів на рахунку для зняття {amount} грн.");
                return false;
            }

            CardBalance -= amount;
            DrawMoneyHandler?.Invoke(this, new DrawMoneyArgs($"Знято кошти на суму {amount} грн.", amount));
            Notify($"Успішно знято {amount} грн. Поточний баланс: {CardBalance} грн.");
            return true;
        }

        private void Notify(string message)
        {
            Message?.Invoke(this, new MessageEventArgs(message));
        }
    }
}
