using System.Collections;

namespace hds.world.Structures
{
    public class Crew
    {
        public string crewName;
        public string characterMasterName;
        public ArrayList members;

        public Crew(string _crewName, string _characterMasterName)
        {
            crewName = _crewName;
            characterMasterName = _characterMasterName;
            members = new ArrayList();
            members.Add(_characterMasterName);
        }

        public void inviteMember(string memberName)
        {
            bool alreadyExists = false;
            foreach (FCMember member in members)
            {
                if (member.handle.Equals(memberName))
                {
                    alreadyExists = true;
                }
            }

            if (!alreadyExists)
            {
                FCMember member = new FCMember();
                member.handle = memberName;
                member.timestampInvite = TimeUtils.getUnixTimeUint32();
                members.Add(member);
            }
        }

        public void removeMember(string memberName)
        {
            lock (members)
            {
                FCMember removeObject = null;
                foreach (FCMember member in members)
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