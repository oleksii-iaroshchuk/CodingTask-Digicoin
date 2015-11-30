using System;
using System.Collections.Generic;
using System.Linq;
using Digicoin.Core.Entities;

namespace Digicoin.Core.Repositories
{
    /// <summary>
    /// Orders Data Access Layer.
    /// </summary>
    public interface IOrderRepository
    {
        void Insert(Order order);

        List<ClientNetPosition> GetClientNetPositions();

        List<BrokerAmount> GetBrokerAmounts();
    }

    /// <summary>
    /// In-memory implementation.
    /// NOTE: In real application repository would use, for example, SQL DB for persistence.
    ///       Also note that in real application DAL implementations would be moved into separate dll.
    /// </summary>
    public class InMemoryOrderRepository : IOrderRepository
    { 
        private readonly List<Order> _orders = new List<Order>();

        public void Insert(Order order)
        {
            if (order == null) throw new ArgumentNullException("order");
            //NOTE: In real application Orders and Deals are saved into different tables (with 1:N relationship).
            //      Here, for simplicity, we just store Order object in memory.
            _orders.Add(order);
        }

        public List<ClientNetPosition> GetClientNetPositions()
        {
            var clientNetPositions = new List<ClientNetPosition>();

            // To report a client net position, we must find the average price of all transactions and multiply that by the sum of all orders.
            // Buy orders are positive and sell orders are negative. For example if client has a buy order of 50 Digicoins and a sell order of 40 the sum will be 10.
            // NOTE: In real application, to avoid loading all historic data into memory, this report might be generated on DB level using SQL function

            var clientIds = _orders.GroupBy(o => o.Client.Id).Select(g => g.Key).ToList();

            foreach (var clientId in clientIds)
            {   
                var clientOrders = _orders.Where(o => o.Client.Id == clientId).ToList();

                decimal sumOfAveragePricesInAllOrders = 0;

                foreach (var order in clientOrders)
                {
                    var amountInOrder = order.Deals.Sum(d => d.Amount);
                    var totalCostInOrder = order.Deals.Sum(d => d.TotalCost);
                    var averagePriceInOrder = totalCostInOrder / amountInOrder;
                    sumOfAveragePricesInAllOrders += averagePriceInOrder;
                }
                var ordersCount = clientOrders.Count;
                var averagePriceInAllOrders = sumOfAveragePricesInAllOrders / ordersCount;
                var totalAmountInAllOrders =  clientOrders.SelectMany(o => o.Deals).Sum(d => d.Order.Type == OrderType.Buy ? d.Amount : -d.Amount);

                var netPosition = averagePriceInAllOrders * totalAmountInAllOrders;

                clientNetPositions.Add(new ClientNetPosition(clientId, netPosition));
            }

            return clientNetPositions;
        }

        public List<BrokerAmount> GetBrokerAmounts()
        {
            return _orders.SelectMany(o => o.Deals).GroupBy(d => d.Broker.Id)
                .Select(g => new BrokerAmount(g.Key, g.Sum(e => e.Amount)))
                .ToList();
        }
    }
}
