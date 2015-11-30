using System;

namespace Digicoin.Core.Entities
{
    /// <summary>
    /// Represents broker abstraction.
    /// </summary>
    public interface IBroker
    {
        /// <summary>
        /// Broker unique identifier.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// The maximum amount broker can execute.
        /// </summary>
        decimal MaxAmount { get; }

        /// <summary>
        /// Estimates the deal for specified order type and amount amount.
        /// NOTE: In original task there is no difference in broker buy and sale quotes, but, for example, in real life buy and sale rates can differ, 
        ///       so we must take this into consideration. In our case it is used to 
        /// </summary>
        Deal Estimate(decimal amount);

        /// <summary>
        /// Executes the specified deal.
        /// </summary>
        /// <param name="deal">The deal.</param>
        void Execute(Deal deal);
    }
}