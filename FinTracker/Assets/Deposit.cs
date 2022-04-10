using FinTracker.Assets;
using FinTracker.Assets.FVCalc;
using System;

namespace FinTracker
{

    public class Deposit : AbstractAsset
    {
        private readonly Storage _storage = Storage.GetStorage();

        public string Name { get; set; }
        public double Amount { get; set; }
        public string BankName { get; set; } // название банка
        public bool Withdrawable { get; set; } // возможность снятия
        public bool Putable { get; set; }  // возможность внесения
        public bool Сapitalization { get; set; } // процент к сумме вклада или нет
        public DateTime ClosingDate { get; set; } // дата закрытия
        public DateTime OpeningDate { get; set; } // дата открытия
        public double Percent { get; set; } // годовой процент
        public double SumIncome { get; set; } // сумма начисления
        public Storage.period Period { get; set; } // период
        public DateTime SpendDate { get; set; } // дата зачисления
        public int TermDeposit { get; set; }   // срок вклада
        public AbstractAsset AssetForEnroll { get; set; }

        public Deposit(string name, string bankName, double amount, bool withdrawable, bool putable,
            bool capitalization, int termDeposit, DateTime openingDate, double percent, Storage.period period, AbstractAsset asset)
        {
            if (capitalization)
            {
                calcer = new CompoundDepositFVCalc();
            }
            else
            {
                calcer = new SimpleDepositFVCalc();
            }
            Name = name;
            BankName = bankName;
            Amount = amount;
            Withdrawable = withdrawable;
            Putable = putable;
            Сapitalization = capitalization;
            ClosingDate = openingDate.AddYears(termDeposit);
            OpeningDate = openingDate;
            Percent = percent;
            Period = period;
            SpendDate = openingDate.AddDays((int)Period);
            TermDeposit = termDeposit;
            AssetForEnroll ??= asset;
        }
        public double GetSumIncome()
        {
            SumIncome = Amount * (Percent * (double)Period / 3600);

            return SumIncome;
        }
        public void EnrollIncomeFromDeposit()
        {
            // если % добавляется к вкладу
            if (Withdrawable == false && Putable == true)
            {
                if (DateTime.Today >= SpendDate && DateTime.Today <= ClosingDate)
                {
                    SumIncome = Amount * (Percent * (double)Period / 3600);
                    Transaction nTransaction = new Transaction(Storage.sign.income, SumIncome, SpendDate, "", "Начисление % по вкладам");
                    this.AddTransactions(nTransaction);
                    SpendDate = SpendDate.AddDays((double)Period);
                }
            }
            // если % не добавляется к вкладу
            if (Withdrawable == true && Putable == true)
            {
                if (DateTime.Today >= SpendDate && DateTime.Today <= ClosingDate)
                {
                    SumIncome += Amount * (Percent * (double)Period / 3600);
                    Transaction nTransaction = new Transaction(Storage.sign.income, SumIncome, SpendDate, "", "Начисление % по вкладам");
                    AssetForEnroll.AddTransactions(nTransaction);
                    SpendDate = SpendDate.AddDays((double)Period);
                }
            }
        }

        public void EditDeposit(string name, string bankName, double amount, bool withdrawable, bool putable,
            bool capitalization, int termDeposit, DateTime openingDate, double percent, Storage.period period)
        {
            Name = name;
            BankName = bankName;
            Amount = amount;
            Withdrawable = withdrawable;
            Putable = putable;
            Сapitalization = capitalization;
            ClosingDate = openingDate.AddYears(termDeposit);
            OpeningDate = openingDate;
            Percent = percent;
            Period = period;
            SpendDate = openingDate.AddDays((int)Period * 360);
        }
    }
}
