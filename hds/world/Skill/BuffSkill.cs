using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hds
{
    public class BuffSkill
    {
        public int castDuration = 0;
        public int[] animationList;
        public long buffEndTime;


        public BuffSkill(int _cast, int[] _animationList)
        {
            castDuration = _cast;
            this.animationList = _animationList;
        }


    }
}
