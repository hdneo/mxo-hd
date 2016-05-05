using System;
using System.Collections.Generic;
using System.Text;

namespace hds
{
    public class ClientView
    {
        public UInt32 GoID = 0; // the Type
        public UInt16 ViewID = 0;
        public UInt64 entityId = 0;
        public uint spawnId = 0; // Spawn Counter 
        public bool viewCreated = false;  // To define if the view Packet was successful created
        public bool viewNeedsToBeDeleted = false; // set it to true if views needs to be "cleaned" up

        public ClientView(UInt32 GoID, UInt16 ViewID, UInt64 entityId)
        {
            this.GoID = GoID;
            this.ViewID = ViewID;
            this.entityId = entityId;
        }

    }
}
