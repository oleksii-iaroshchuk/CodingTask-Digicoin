using System;
using Digicoin.Core.Utility;

namespace Digicoin.Core.Entities
{
    /// <summary>
    /// Client net position (used for report)
    /// </summary>
    public class ClientNetPosition
    {
        public ClientNetPosition(Guid clientId, decimal netPosition)
        {
            ClientId = clientId;
            NetPosition = netPosition.RoundForReport();
        }

        public Guid ClientId { get; private set; }

        public decimal NetPosition { get; private set; }
    }
}
