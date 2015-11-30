using System;
using System.Collections.Generic;
using System.Linq;
using Digicoin.Core.Entities;

namespace Digicoin.Core.Services
{
    /// <summary>
    /// Provides the functionality to split client orders between brokers.
    /// </summary>
    public interface IOrderSplitService
    {
        List<Deal> Split(Order order);
    }

    public class OrderSplitService : IOrderSplitService
    {
        private readonly IBroker _broker1;
        private readonly IBroker _broker2;
        private const decimal AmountDelta = 10;

        public OrderSplitService(IBroker broker1, IBroker broker2)
        {
            if (broker1 == null) throw new ArgumentNullException("broker1");
            if (broker2 == null) throw new ArgumentNullException("broker2");

            _broker1 = broker1;
            _broker2 = broker2;
        }

        public List<Deal> Split(Order order)
        {
            if (order == null) throw new ArgumentNullException("order");

            CheckOrderAmount(order);
            
            // This is the simplest-stupid split logic for 2 brokers
            // Traverse all possible deal combinations and select the one with minimum total cost
            decimal broker1Amount;
            decimal broker2Amount;
            var deals = new List<Deal>();

            // Start from putting the maximum possible amount to broker1
            if (order.Amount > _broker1.MaxAmount)
            {
                broker1Amount = _broker1.MaxAmount;
                broker2Amount = order.Amount - _broker1.MaxAmount;

                deals.Add(_broker1.Estimate(broker1Amount));
                deals.Add(_broker2.Estimate(broker2Amount));
                
            }
            else
            {
                broker1Amount = order.Amount;
                broker2Amount = 0;

                deals.Add(_broker1.Estimate(broker1Amount));
            }

            var minCost = deals.Sum(q => q.TotalCost);

            // Now, while broker1 has what to substract, and broker2 has capacity for addition, check all possible combinations
            while (broker1Amount > 0 && broker2Amount < _broker2.MaxAmount)
            {
                broker1Amount -= AmountDelta;
                broker2Amount += AmountDelta;
                var currentDeals = new List<Deal>();

                if (broker1Amount > 0)
                    currentDeals.Add(_broker1.Estimate(broker1Amount));
                if (broker2Amount > 0)
                    currentDeals.Add(_broker2.Estimate(broker2Amount));

                var totalCost = currentDeals.Sum(d => d.TotalCost);
                if (totalCost < minCost)
                {
                    minCost = totalCost;
                    deals = currentDeals;
                }
            }

            // Initialize Order property in all deals
            foreach (var deal in deals)
            {
                deal.Order = order;
            }

            return deals;
        }

        private void CheckOrderAmount(Order order)
        {
            var maxAmount = _broker1.MaxAmount + _broker2.MaxAmount;
            if (order.Amount > maxAmount)
                throw new InvalidOperationException(string.Format("Order amount ({0}) is greater than maximum amount possible for execution by all brokers ({1}).",
                    order.Amount, maxAmount));
        }
    }
}