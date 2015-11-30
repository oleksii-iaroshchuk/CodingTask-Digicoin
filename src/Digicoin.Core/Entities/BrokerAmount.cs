using System;
using Digicoin.Core.Utility;

namespace Digicoin.Core.Entities
{
    /// <summary>
    /// Broker amount (used for report).
    /// </summary>
    public class BrokerAmount
    {
        public BrokerAmount(Guid brokerId, decimal amount)
        {
            BrokerId = brokerId;
            Amount = amount.RoundForReport();
        }

        public Guid BrokerId { get; private set; }

        public decimal Amount { get; private set; }
    }
}
