using System;

namespace Digicoin.Core.Entities
{
    /// <summary>
    /// Client in our system
    /// </summary>
    public class Client
    {
        public Client(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; private set; }
       
        public string Name { get; private set; }
    }
}
