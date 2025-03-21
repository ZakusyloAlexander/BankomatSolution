
using System;

namespace BancomatClassLibrary
{
    public class ATMContext
    {
        public Bank SelectedBank { get; set; }
        public AutomatedTellerMachine ActiveBankomat { get; set; }
        public Account CurrentAccount { get; set; }
    }

}
