using System;
using Digicoin.Core.Utility;

namespace Digicoin.Core.Entities
{
    public abstract class BrokerBase
    {
        public decimal MaxAmount { get { return 100m; } }

        public Deal Estimate(decimal amount)
        {
            ValidateAmount(amount);

            return EstimateInternal(amount);
        }

        protected void ValidateAmount(decimal amount)
        {
            AmountValidationUtility.ValidateGreaterThan0(amount, "Broker");

            if (amount > MaxAmount)
                throw new ArgumentException("Broker amount must be less or equal to 100.");

            AmountValidationUtility.ValidateIsMultipleOf10(amount, "Broker");
        }

        protected abstract Deal EstimateInternal(decimal amount);
    }
}
