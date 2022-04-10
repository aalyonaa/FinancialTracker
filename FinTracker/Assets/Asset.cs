using FinTracker.Assets;
using FinTracker.Assets.FVCalc;

namespace FinTracker
{
    public class Asset : AbstractAsset
    {
        public Asset(string name, double amount)
        {
            calcer = new CashFVCalc();
            Name = name;
            Amount = amount;
            StartAmount = amount;
        }
    }
}
