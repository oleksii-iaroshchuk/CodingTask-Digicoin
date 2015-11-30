using System;

namespace Digicoin.Core.Entities
{
    public class Broker2 : BrokerBase, IBroker
    {
        public Guid Id
        {
            get { return new Guid("B717E680-68D7-4074-AE5C-120A7B971AD4"); }
        }

        protected override Deal EstimateInternal(decimal amount)
        {
            var rate = 1.52m;
            var commission = GetCommission(amount);
            return new Deal(this, amount, rate, commission);
        }

        protected decimal GetCommission(decimal amount)
        {
            if (amount >= 10 && amount <= 40)
                return 0.03m;
            if (amount >= 50 && amount <= 80)
                return 0.025m;
            if (amount >= 90 && amount <= 100)
                return 0.02m;

            throw new Exception("Invalid amount.");
        }

        public void Execute(Deal deal)
        {
            //NOTE: In real application, broker would buy or sell some digicoins here
        }
    }
}
