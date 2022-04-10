namespace FinTracker.Assets.FVCalc
{
    public interface IFVCalcer
    {
        public double GetFutureValue(double amount, int term, double interest);
    }
}
