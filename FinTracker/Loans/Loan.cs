using FinTracker.Assets;
using FinTracker.Loans;
using System;
using System.Collections.Generic;

namespace FinTracker
{
    public class Loan
    {
        public List<LoanTransaction> LoanTransactions = new List<LoanTransaction>();
        public DateTime PreviousPaymentDateTime { get; set; }
        public DateTime ActualPaymentDateTime { get; set; }
        public DateTime NextPaymentDateTime { get; set; }
        public DateTime LastPaymentDateTime { get; set; }
        public AbstractAsset Asset { get; set; }
        public int Id { get; set; }
        public string CreditorsName { get; set; }
        public double Percent { get; set; }
        public double Period { get; set; }
        public string Status { get; set; }
        public double Amount { get; set; }
        public double TotalAmountOfPercents { get; set; }
        public double TotalAmountOfLoan { get; set; }
        public double RemainingAmountOfPercents { get; set; }
        public double AmountOfReturned { get; set; }
        public double RemainingTerm { get; set; }
        public double RemainingAmount { get; set; }
        public double MonthlyPayment { get; set; }
        public double TotalAmountOfExtraPaymentsDoneBetweenDates { get; set; }
        public double TotalAmountOfExtraPaymentsDoneInDateOfPayment { get; set; }
        public double TotalAmountOfExtraPaymentsDoneToDecreaseLoanTerm { get; set; }
        public double MonthlyPaymentRounded { get; set; }
        public double AmountOfReturnedRounded { get; set; }
        public double RemainingAmountRounded { get; set; }



        public Loan(AbstractAsset asset, int id, DateTime actualPaymentDateTime, string creditorsName,
                    double percent, double period, string status,
                    double remainingTerm,
                    double amount)
        {
            Asset = asset;
            Id = id;
            PreviousPaymentDateTime = actualPaymentDateTime.AddMonths(-1);
            ActualPaymentDateTime = actualPaymentDateTime;
            NextPaymentDateTime = ActualPaymentDateTime.AddMonths(1);
            RemainingTerm = remainingTerm;
            LastPaymentDateTime = actualPaymentDateTime.AddMonths(Convert.ToInt32(RemainingTerm));
            CreditorsName = creditorsName;
            Percent = percent;
            Period = period;
            Status = status;
            Amount = amount;
            TotalAmountOfPercents = Amount * ((Percent / 1200) * Period);
            TotalAmountOfLoan = Amount + TotalAmountOfPercents;
            MonthlyPayment = Amount * ((Percent / 1200) / (1 - Math.Pow((1 + (Percent / 1200)), -Period)));
            AmountOfReturned = (period - remainingTerm) * MonthlyPayment;
            RemainingAmount = Amount - AmountOfReturned;
            RemainingAmountOfPercents = RemainingAmount * (Percent / 100);
            TotalAmountOfExtraPaymentsDoneBetweenDates = 0;
            TotalAmountOfExtraPaymentsDoneInDateOfPayment = 0;
            TotalAmountOfExtraPaymentsDoneToDecreaseLoanTerm = 0;
            MonthlyPaymentRounded = Math.Round(MonthlyPayment, 2);
            AmountOfReturnedRounded = Math.Round(AmountOfReturned, 2);
            RemainingAmountRounded = Math.Round(RemainingAmount, 2);
        }



        //привязать актуальную дату к программе
        public void DoRegularPayment()
        {
            if (DateTime.Today == ActualPaymentDateTime)
            {
                Asset.Amount -= MonthlyPayment;
                TotalAmountOfLoan -= MonthlyPayment;
                Amount -= (MonthlyPayment - (Amount * (Percent / 1200)));
                ActualPaymentDateTime = NextPaymentDateTime;
                LoanTransaction nLoanTransaction = new LoanTransaction(Storage.sign.spend, MonthlyPayment, DateTime.Today, "", "Регулярный платёж по кредиту", "Регулярный платёж по кредиту");
                Transaction transaction = nLoanTransaction;
            }

        }


        //метод досрочного погашения, уменьшающий ежемесячный платёж
        public void DoExtraPaymentToDecreasePayment(DateTime extraPaymentDate, double extraPaymentAmount)
        {
            double daylyPercent = (Percent / 1200) / (ActualPaymentDateTime - PreviousPaymentDateTime).TotalDays; //вычисляем дневную процентную ставку в текущем месячном промежутке

            if (extraPaymentDate != ActualPaymentDateTime)
            {
                if (extraPaymentAmount <= RemainingAmount && RemainingAmount > 0)
                {
                    double balanceForRepaymentOfLoanPercents = RemainingAmount * (((extraPaymentDate - ActualPaymentDateTime).TotalDays) * daylyPercent); //сколько из досрочного погашения уйдет на проценты
                    double balanceForRepaymentOfLoanBody = extraPaymentAmount - balanceForRepaymentOfLoanPercents; //сколько из досрочного погашения уйдет на тело кредита
                    AmountOfReturned += balanceForRepaymentOfLoanBody;
                    TotalAmountOfPercents -= balanceForRepaymentOfLoanPercents;
                    Asset.Amount -= extraPaymentAmount;
                    MonthlyPayment = (Amount - balanceForRepaymentOfLoanBody) * ((Percent / 1200) / (1 - Math.Pow((1 + (Percent / 1200)), -Period)));
                    MonthlyPaymentRounded = Math.Round(MonthlyPayment, 2);
                }
            }
            else if (extraPaymentDate == ActualPaymentDateTime)
            {
                if (extraPaymentAmount <= RemainingAmount && RemainingAmount > 0)
                {
                    AmountOfReturned += extraPaymentAmount;
                    Asset.Amount -= extraPaymentAmount;
                    MonthlyPayment = (Amount - extraPaymentAmount) * ((Percent / 1200) / (1 - Math.Pow((1 + (Percent / 1200)), -Period)));
                    MonthlyPaymentRounded = Math.Round(MonthlyPayment, 2);
                    TotalAmountOfPercents = RemainingAmount * ((Percent / 1200) * Period);
                }
            }
        }

        public void DoExtraPaymentToDecreaseLoanTerm(DateTime extraPaymentDate, double extraPaymentAmount)
        {
            TotalAmountOfExtraPaymentsDoneToDecreaseLoanTerm += extraPaymentAmount;
            Asset.Amount -= MonthlyPayment;
            AmountOfReturned += extraPaymentAmount;

            if (TotalAmountOfExtraPaymentsDoneToDecreaseLoanTerm >= MonthlyPayment)
            {
                while (MonthlyPayment <= TotalAmountOfExtraPaymentsDoneToDecreaseLoanTerm)
                {
                    RemainingTerm -= 1;
                    TotalAmountOfExtraPaymentsDoneToDecreaseLoanTerm -= MonthlyPayment;
                }

            }

            TotalAmountOfPercents = Amount * ((Percent / 1200) * RemainingTerm);


        }


    }
}
