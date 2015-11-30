using System;
using System.Collections.Generic;
using Digicoin.Core.Entities;
using Digicoin.Core.Repositories;
using Digicoin.Core.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digicoin.Core.Tests
{
    [TestClass]
    public class TestSuites
    {
        private IOrderRepository _orderRepository;
        private IOrderExecutionService _orderExecutionService;

        private Client _clientA;
        private Client _clientB;
        private Client _clientC;

        private IBroker _broker1;
        private IBroker _broker2;

        [TestInitialize]
        public void TestInitialize()
        {
            _broker1 = new Broker1();
            _broker2 = new Broker2();
            var orderSplitService = new OrderSplitService(_broker1, _broker2);
            _orderRepository = new InMemoryOrderRepository();
            _orderExecutionService = new OrderExecutionService(orderSplitService, _orderRepository);

            _clientA = new Client(Guid.NewGuid(), "ClientA");
            _clientB = new Client(Guid.NewGuid(), "ClientB");
            _clientC = new Client(Guid.NewGuid(), "ClientC");
        }

        [TestMethod]
        public void TestSuite1()
        {
            // 1) Client A buys 10 at 15.645
            ExecuteOrderTestCase(_clientA, OrderType.Buy, 10, 15.645m);
            
            // 2) Client B buys 40 at 62.58
            ExecuteOrderTestCase(_clientB, OrderType.Buy, 40, 62.58m);

            // 3) Client A buys 50 at 77.9
            ExecuteOrderTestCase(_clientA, OrderType.Buy, 50, 77.9m);

            // 4) Client B buys 100 at 155.04
            ExecuteOrderTestCase(_clientB, OrderType.Buy, 100, 155.04m);

            // 5) Client B sells 80 at 124.64
            ExecuteOrderTestCase(_clientB, OrderType.Sell, 80, 124.64m);

            // 6) Client C sells 70 at 109.06
            ExecuteOrderTestCase(_clientC, OrderType.Sell, 70, 109.06m);

            // 7) Client A buys 130 at 201.975
            ExecuteOrderTestCase(_clientA, OrderType.Buy, 130, 201.975m);

            // 8) Client B sells 60 at 93.48
            ExecuteOrderTestCase(_clientB, OrderType.Sell, 60, 93.48m);

            // 9) Report client net positions: Client A 296.156 Client B 0 Client C -109.06
            ExecuteClientNetPositionsTestCase(new List<ClientNetPosition>
            {
                new ClientNetPosition(_clientA.Id, 296.156m),
                new ClientNetPosition(_clientB.Id, 0m),
                new ClientNetPosition(_clientC.Id, -109.06m),
            });

            // 10) Report number of Digicoins transacted by Broker: Broker 1 80 Broker 2 460
            ExecuteBrokerAmountsTestCase(new List<BrokerAmount>
            {
                new BrokerAmount(_broker1.Id, 80),
                new BrokerAmount(_broker2.Id, 460),
            });
        }

        private void ExecuteOrderTestCase(Client client, OrderType orderType, decimal amount, decimal expectedTotalCost)
        {
            var order = new Order(client, orderType, amount);
            var actualTotalCost = _orderExecutionService.Execute(order);
            Assert.AreEqual(expectedTotalCost, actualTotalCost);
        }

        private void ExecuteClientNetPositionsTestCase(List<ClientNetPosition> expectedClientNetPositions)
        {
            var actualClientNetPositions = _orderRepository.GetClientNetPositions();
            actualClientNetPositions.ShouldAllBeEquivalentTo(expectedClientNetPositions);
        }

        private void ExecuteBrokerAmountsTestCase(List<BrokerAmount> expectedBrokerAmounts)
        {
            var actualBrokerAmounts = _orderRepository.GetBrokerAmounts();
            actualBrokerAmounts.ShouldAllBeEquivalentTo(expectedBrokerAmounts);
        }
    }
}
