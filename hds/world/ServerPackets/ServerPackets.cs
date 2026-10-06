using hds.shared;

namespace hds
{
    /// <summary>
    /// Builds and queues server messages. Instances are bound to the client they are created for.
    /// </summary>
    public partial class ServerPackets
    {
        private readonly WorldClient boundClient;

        public ServerPackets()
        {
        }

        public ServerPackets(WorldClient client)
        {
            boundClient = client;
        }

        /// <summary>
        /// Target client of the methods that do not take an explicit client parameter.
        /// Falls back to the client whose packet is being processed on the world thread.
        /// </summary>
        protected WorldClient Client => boundClient ?? Store.currentClient;
    }
}
