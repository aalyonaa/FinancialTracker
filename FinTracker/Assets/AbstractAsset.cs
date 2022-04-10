using FinTracker.Assets.FVCalc;
using System.Collections.Generic;

namespace FinTracker.Assets
{
    public class AbstractAsset
    {
        protected IFVCalcer calcer { get; set; }

        public string Name;
        public double Amount;
        public List<Transaction> Transactions = new List<Transaction>();
        public double StartAmount;

        public AbstractAsset()
        {

        }


        public double GetAmount()
        {
            double result = StartAmount;
            foreach (Transaction transaction in Transactions)
            {
                if (transaction.Sign == Storage.sign.income)
                {
                    result += transaction.Amount;
                }
                else if (transaction.Sign == Storage.sign.spend)
                {
                    result -= transaction.Amount;
                }
            }
            return result;
        }

        public void AddTransactions(Transaction nTransaction)
        {
            Transactions.Add(nTransaction);
            Amount = GetAmount();
        }

        public void DeleteTransaction(Transaction curTransaction)
        {
            Transactions.Remove(curTransaction);
        }

        public void EditAsset(string name, double amount)
        {
            Name = name;
            Amount = amount;

            if (Transactions == null)
            {
                StartAmount = amount;
                Amount = StartAmount;
            }
        }

        public double GetFutureValue(double amount, int term, double interest)
        {
            return calcer.GetFutureValue(amount, term, interest);
        }
    }
}
