using FinTracker.Assets;
using FinTracker.Assets.FVCalc;
using System;
using System.Collections.Generic;

namespace FinTracker
{
    public class Card : AbstractAsset
    {
        public double YearInterest; //Процент на остаток // узать как считаеться
        public double SumYearInterest;
        public DateTime EnrollDateYearInterest;
        public double MinAmount;

        public double FixCashback;
        public DateTime EnrollDateCash; // дата выплаты кэшбека

        public double ServiceFee;
        public DateTime DateSpendServiceFee;

        public double Cashback = 0; // сумма кэшбека 

        public double Percent; // процент кэшбека
        public List<string> CashbackCategories; //категории с повышенным кэшбэком
        public Dictionary<string, double> CashbackAndPercent = new Dictionary<string, double>();

        public Card(string name, double amount,
            double yearInterest, double fixCashback, double serviceFee,
            DateTime enrollDateCash, DateTime enrollDateYearInterest,
            DateTime dateSpendServiceFee)
        {
            calcer = new CardFVCalc();
            Name = name;
            Amount = amount;
            StartAmount = amount;
            MinAmount = amount;
            ServiceFee = serviceFee;
            DateSpendServiceFee = dateSpendServiceFee;
            YearInterest = yearInterest / 100;
            FixCashback = fixCashback / 100;
            EnrollDateCash = enrollDateCash;
            EnrollDateYearInterest = enrollDateYearInterest;
            CashbackAndPercent.Add("Перевод", 0);
            CashbackAndPercent.Add("Коммунальные платежи", 0);
        }

        public void AddCategoryCashback(string category, double percent)
        {
            CashbackAndPercent.Add(category, percent / 100);
        }

        public void GetMinAmount() //при запуске и в транзакциях
        {
            double tmpAmount = GetAmount();
            if (tmpAmount < MinAmount)
            {
                MinAmount = tmpAmount;
            }
            if (DateTime.Today >= EnrollDateYearInterest)
            {
                MinAmount = GetAmount();
            }
        }

        public double GetCashback()
        {
            foreach (Transaction transaction in Transactions)
            {
                if (transaction.Date >= new DateTime(EnrollDateCash.Year, EnrollDateCash.Month - 1, EnrollDateCash.Day)
                    && transaction.Date < EnrollDateCash)
                {
                    if (transaction.Sign == Storage.sign.spend)
                    {
                        foreach (KeyValuePair<string, double> cashback in CashbackAndPercent)
                        {
                            if (transaction.Category == cashback.Key)
                            {
                                Cashback += (transaction.Amount * cashback.Value);
                            }
                            else
                            {
                                Cashback += (transaction.Amount * FixCashback);
                            }
                        }
                    }
                }
            }
            return Cashback;
        }

        public double GetSumYearInterest()
        {
            SumYearInterest = MinAmount * (YearInterest / 12);
            return SumYearInterest;
        }

        public void EnrollmentServiceFee()
        {
            if (DateTime.Today >= DateSpendServiceFee)
            {
                Transaction nTransaction = new Transaction(Storage.sign.spend, ServiceFee, DateSpendServiceFee, "", "Обслуживание банка");
                this.AddTransactions(nTransaction);
                DateSpendServiceFee = DateSpendServiceFee.AddMonths(1);
            }
        }

        public void EnrollmentSumYearInterest()
        {
            if (DateTime.Today >= EnrollDateYearInterest)
            {
                SumYearInterest = GetSumYearInterest();
                Transaction nTransaction = new Transaction(Storage.sign.spend, SumYearInterest, EnrollDateYearInterest, "", "Начисление % на остаток по счету");
                this.AddTransactions(nTransaction);
                EnrollDateYearInterest = EnrollDateYearInterest.AddMonths(1);
                SumYearInterest = 0;
            }

        }

        public void EnrollmentCashbak()
        {

            if (DateTime.Today >= EnrollDateCash)
            {
                Cashback = GetCashback();
                Transaction nTransaction = new Transaction(Storage.sign.spend, Cashback, EnrollDateCash, "", "Начисление кэшбека");
                this.AddTransactions(nTransaction);
                EnrollDateCash = EnrollDateCash.AddMonths(1);
                Cashback = 0;
            }

        }

        public void EditCard(string name, double amount,
            double yearInterest, double fixCashback, double serviceFee,
            DateTime enrollDateCash, DateTime enrollDateYearInterest,
            DateTime dateSpendServiceFee)
        {
            Name = name;
            Amount = amount;
            ServiceFee = serviceFee;
            DateSpendServiceFee = dateSpendServiceFee;
            YearInterest = yearInterest / 100;
            FixCashback = fixCashback / 100;
            EnrollDateCash = enrollDateCash;
            EnrollDateYearInterest = enrollDateYearInterest;

            if (Transactions == null)
            {
                StartAmount = amount;
                Amount = StartAmount;
                MinAmount = amount;
            }
        }
    }
}

