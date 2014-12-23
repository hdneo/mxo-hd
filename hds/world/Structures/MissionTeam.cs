using System;
using System.Collections;
using System.Text;

namespace hds
{
    public class MissionTeam
    {
        public string teamName;
        public string characterMasterName;
        public ArrayList members;

        public MissionTeam(string _teamName, string _characterMasterName)
        {
            teamName = _teamName;
            characterMasterName = _characterMasterName;
            members = new ArrayList();
            members.Add(_characterMasterName);
        }

        public void addMember(string memberName)
        {
            bool alreadyExists = false;
            foreach (string name in members)
            {
                if (name.Equals(memberName))
                {
                    alreadyExists = true;
                }
            }

            if (!alreadyExists)
            {
                members.Add(memberName);
            }
        }

        public void removeMember(string memberName)
        {
            lock (members)
            {
                members.Remove(memberName);
            }
            
        }

    }
}
