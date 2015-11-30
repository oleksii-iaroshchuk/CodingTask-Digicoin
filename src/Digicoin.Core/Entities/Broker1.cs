using System;

namespace Digicoin.Core.Entities
{
    public class Broker1 : BrokerBase, IBroker
    {
        public Guid Id
        {
            get { return new Guid("D66940BB-0DB5-48E5-9226-C695A7C01ECC"); }
        }

        protected override Deal EstimateInternal(decimal amount)
        {
            var rate = 1.49m;
            var commission = 0.05m;
            return new Deal(this, amount, rate, commission);
        }

        public void Execute(Deal deal)
        {
            //NOTE: In real application, broker would buy or sell some digicoins here
        }
    }
}