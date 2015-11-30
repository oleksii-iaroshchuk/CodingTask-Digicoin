using System;

namespace Digicoin.Core.Entities
{
    /// <summary>
    /// Represents the deal that can be executed by broker.
    /// Order can be split on multiple such deals.
    /// </summary>
    public class Deal
    {
        public Deal(IBroker broker, decimal amount, decimal rate, decimal commission)
        {
            if (broker == null) throw new ArgumentNullException("broker");

            Broker = broker;

            Amount = amount;
            Rate = rate;
            Commission = commission;

            CostWithoutCommission = amount * rate;
            CommissionCost = CostWithoutCommission * commission;
            TotalCost = CostWithoutCommission + CommissionCost;
        }

        public IBroker Broker { get; private set; }

        public decimal Amount { get; private set; }

        public decimal Rate { get; private set; }

        public decimal Commission { get; private set; }

        public decimal CostWithoutCommission { get; private set; }

        public decimal CommissionCost { get; private set; }

        public decimal TotalCost { get; private set; }

        public Order Order { get; set; }

        public void Execute()
        {
            Broker.Execute(this);
        }
    }
}
