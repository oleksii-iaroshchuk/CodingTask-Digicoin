using System;
using System.Linq;
using Digicoin.Core.Entities;
using Digicoin.Core.Repositories;

namespace Digicoin.Core.Services
{
    /// <summary>
    /// Provides the functionality to execute client orders. 
    /// </summary>
    public interface IOrderExecutionService
    {
        /// <summary>
        /// Executes the specified order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns>Total cost.</returns>
        decimal Execute(Order order);
    }

    public class OrderExecutionService : IOrderExecutionService
    {
        private readonly IOrderSplitService _orderSplitService;
        private readonly IOrderRepository _orderRepository;

        public OrderExecutionService(IOrderSplitService orderSplitService, IOrderRepository orderRepository)
        {
            if (orderSplitService == null) throw new ArgumentNullException("orderSplitService");
            if (orderRepository == null) throw new ArgumentNullException("orderRepository");


            _orderSplitService = orderSplitService;
            _orderRepository = orderRepository;
        }

        /// <summary>
        /// Executes the specified order.
        /// This is a main enty point into application.
        /// In real application we can expose it via REST API. But please note:
        /// - This is a Domain Service, and it expects already prepared Order domain object.
        /// - In case of REST API, we would need to create an Application Service that gets Order Data Transfer Object (DTO), wich then would be transformed into our domain Order object.
        /// - This, for example, would include validating whether Client specified in order exists in our system.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        public decimal Execute(Order order)
        {
            if (order == null) throw new ArgumentNullException("order");

            // Split order on deals and then execute them
            order.Deals = _orderSplitService.Split(order);
           
            // NOTE: In real application, deals execution and inserting order must be executed in scope of one transaction 
            foreach (var deal in order.Deals)
            {
                deal.Execute();
            }
            // Keep audit trail of order
            _orderRepository.Insert(order);

            var totalCost = order.Deals.Sum(q => q.TotalCost);

            return totalCost;
        }
    }
}
