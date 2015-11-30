using System;
using System.Collections.Generic;
using Digicoin.Core.Utility;

namespace Digicoin.Core.Entities
{
    /// <summary>
    /// Represents order in our system
    /// </summary>
    public class Order
    {
        public Order(Client client, OrderType type, decimal amount)
        {
            if (client == null) throw new ArgumentNullException("client");
            ValidateAmount(amount);

            Id = Guid.NewGuid();
            
            Client = client;
            Type = type;
            Amount = amount;
        }

        public Guid Id { get; private set; }

        public OrderType Type { get; private set; }

        public decimal Amount { get; private set; }

        public Client Client { get; private set; }

        public List<Deal> Deals { get; set; }

        private void ValidateAmount(decimal amount)
        {
            AmountValidationUtility.ValidateGreaterThan0(amount, "Order");

            AmountValidationUtility.ValidateIsMultipleOf10(amount, "Order");
        }
    }
}
