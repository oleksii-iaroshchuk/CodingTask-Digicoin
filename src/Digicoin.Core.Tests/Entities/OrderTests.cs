using System;
using Digicoin.Core.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Digicoin.Core.Tests.Entities
{
    [TestClass]
    public class OrderTests
    {
        [TestMethod]
        public void Ctor_AmountIsMultipleOf10_NoException()
        {   
            const int amount = 20;
            
            Action act = () => new Order(Build_Client(), OrderType.Buy, amount);
            
            act.ShouldNotThrow();
        }

        [TestMethod]
        public void Ctor_AmountIsNotMultipleOf10_Exception()
        {
            const int amount = 15;
            
            Action act = () => new Order(Build_Client(), OrderType.Buy, amount);
            
            act.ShouldThrow<ArgumentException>().WithMessage("Order amount must be a multiple of 10.");
        }

        [TestMethod]
        public void Ctor_AmountIs0_Exception()
        {
            const int amount = 0;
           
            Action act = () => new Order(Build_Client(), OrderType.Buy, amount);
            
            act.ShouldThrow<ArgumentException>().WithMessage("Order amount must be greater than 0.");
        }

        [TestMethod]
        public void Ctor_AmountIsLessThan0_Exception()
        {
            const int amount = -10;
            
            Action act = () => new Order(Build_Client(), OrderType.Buy, amount);
            
            act.ShouldThrow<ArgumentException>().WithMessage("Order amount must be greater than 0.");
        }

        private Client Build_Client()
        {
            return new Client(Guid.NewGuid(), "ClientName");
        }
    }
}