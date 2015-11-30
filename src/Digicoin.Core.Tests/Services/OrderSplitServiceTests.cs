using System;
using System.Collections.Generic;
using Digicoin.Core.Entities;
using Digicoin.Core.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Digicoin.Core.Tests.Services
{
    [TestClass]
    public class OrderSplitServiceTests
    {
        private OrderSplitService _orderSplitService;
        private Mock<IBroker> _broker1Mock;
        private Mock<IBroker> _broker2Mock;

        [TestInitialize]
        public void TestInitialize()
        {
            _broker1Mock = new Mock<IBroker>();
            _broker2Mock = new Mock<IBroker>();

            _orderSplitService = new OrderSplitService(_broker1Mock.Object, _broker2Mock.Object);
            
            _broker1Mock.Setup(m => m.MaxAmount).Returns(100);
            _broker2Mock.Setup(m => m.MaxAmount).Returns(100);
            
        }

        [TestMethod]
        public void Split_OrderAmountIsToLarge_Exception()
        {
            var order = new Order(Build_Client(), OrderType.Buy, 210);

            Action act = () => _orderSplitService.Split(order);

            act.ShouldThrow<InvalidOperationException>().WithMessage("Order amount (210) is greater than maximum amount possible for execution by all brokers (200).");
        }

        [TestMethod]
        public void Split_OptimalIsAllAmountToBroker1_ReturnsBroker1Deal()
        {
            var order = new Order(Build_Client(), OrderType.Buy, 100);
            // Broker1 always returns optimal deal
            var broker1OptimalDeal = new Deal(_broker1Mock.Object, 100, 1, 0.01m);
            Setup_Broker1Mock_Estimate(broker1OptimalDeal);
            Setup_Broker2Mock_Estimate(new Deal(_broker2Mock.Object, 500, 1, 0.01m));
            var expectedDeals = new List<Deal>
            {
                broker1OptimalDeal
            };

            var actualDeals = _orderSplitService.Split(order);

            actualDeals.ShouldAllBeEquivalentTo(expectedDeals);
        }

        [TestMethod]
        public void Split_OptimalIsAllAmountToBroker2_ReturnsBroker2Deal()
        {
            var order = new Order(Build_Client(), OrderType.Buy, 100);
            // Broker2 always returns optimal deal
            var broker2OptimalDeal = new Deal(_broker2Mock.Object, 500, 1, 0.01m);
            Setup_Broker1Mock_Estimate(new Deal(_broker1Mock.Object, 1000, 1, 0.01m));
            Setup_Broker2Mock_Estimate(broker2OptimalDeal);
            var expectedDeals = new List<Deal>
            {
                broker2OptimalDeal
            };

            var actualDeals = _orderSplitService.Split(order);

            actualDeals.ShouldAllBeEquivalentTo(expectedDeals);
        }

        [TestMethod]
        public void Split_OptimalIsToSplitOrder_ReturnsBroker2Deal()
        {
            var order = new Order(Build_Client(), OrderType.Buy, 100);
            // Only one combination of Broker1 and Broker2 deals is optimal
            var broker1OptimalDeal = new Deal(_broker1Mock.Object, 60, 1, 0.01m);
            var broker2OptimalDeal = new Deal(_broker2Mock.Object, 40, 1, 0.01m);
            Setup_Broker1Mock_Estimate(new Deal(_broker1Mock.Object, 1000, 1, 0.01m));
            Setup_Broker1Mock_Estimate(60, broker1OptimalDeal);
            Setup_Broker2Mock_Estimate(new Deal(_broker2Mock.Object, 1000, 1, 0.01m));
            Setup_Broker2Mock_Estimate(40, broker2OptimalDeal);
            var expectedDeals = new List<Deal>
            {
                broker1OptimalDeal,
                broker2OptimalDeal
            };

            var actualDeals = _orderSplitService.Split(order);

            actualDeals.ShouldAllBeEquivalentTo(expectedDeals);
        }

        private Client Build_Client()
        {
            return new Client(Guid.NewGuid(), "ClientName");
        }

        private void Setup_Broker1Mock_Estimate(Deal result)
        {
            _broker1Mock.Setup(m => m.Estimate(It.IsAny<decimal>()))
                .Returns(result);
        }

        private void Setup_Broker1Mock_Estimate(decimal forInput, Deal result)
        {
            _broker1Mock.Setup(m => m.Estimate(forInput))
                .Returns(result);
        }

        private void Setup_Broker2Mock_Estimate(Deal result)
        {
            _broker2Mock.Setup(m => m.Estimate(It.IsAny<decimal>()))
                .Returns(result);
        }

        private void Setup_Broker2Mock_Estimate(decimal forInput, Deal result)
        {
            _broker2Mock.Setup(m => m.Estimate(forInput))
                .Returns(result);
        }
    }
}