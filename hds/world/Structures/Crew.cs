using System;
using System.Collections;
using System.Collections.Generic;

namespace hds.world.Structures
{
    public class Crew
    {
        public UInt32 crewId;
        public string crewName;
        public string characterMasterName;
        public List<CrewMember> members = new List<CrewMember>();
        public UInt32 factionId;
        public ushort factionRank;
        public UInt32 masterPlayerCharId;
        public UInt32 money;
        public ushort org;

        public void SetMembers(List<CrewMember> members)
        {
            this.members = members;
        }

        public void inviteMember(string memberName)
        {
            bool alreadyExists = false;
            foreach (CrewMember member in members)
            {
                if (member.handle.Equals(memberName))
                {
                    alreadyExists = true;
                }
            }

            if (!alreadyExists)
            {
                CrewMember member = new CrewMember();
                member.handle = memberName;
                member.timestampInvite = TimeUtils.getUnixTimeUint32();
                members.Add(member);
            }
        }

        public void removeMember(string memberName)
        {
            lock (members)
            {
                CrewMember removeObject = null;
                foreach (CrewMember member in members)
                {
                    if (member.handle.Equals(memberName))
                    {
                        removeObject = member;
                    }
                }

                if (removeObject != null)
                {
                    members.Remove(removeObject);
                }
            }
        }
    }
}