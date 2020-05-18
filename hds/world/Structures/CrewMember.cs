using System;

namespace hds.world.Structures
{
    public class CrewMember
    {
        public UInt32 charId;
        public string handle;
        public long timestampInvite;
        public bool inviteAccepted = false;
        public bool isCaptain = false;
        public bool isFirstMate = false;
        public ushort isOnline = 0;
    }
}