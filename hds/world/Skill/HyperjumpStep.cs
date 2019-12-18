using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hds.world.Skill
{
    class JumpStep
    {
        public LtVector3f FromPos;
        public LtVector3f ToPos;
        public float JumpHeight;
        public UInt32 endTime;
        public UInt16 NeededAckSeq;
        public ushort jumpId;
        public UInt32 maybeTimeBasedValue;

        public JumpStep(LtVector3f fromPos, LtVector3f toPos, float jumpHeight, uint endTime, ushort jumpId,
            uint maybeTimeBasedValue)
        {
            FromPos = fromPos;
            ToPos = toPos;
            JumpHeight = jumpHeight;
            this.endTime = endTime;
            this.jumpId = jumpId;
            this.maybeTimeBasedValue = maybeTimeBasedValue;
        }
    }
}
