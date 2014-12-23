using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using hds.shared;

namespace hds
{
    class AnimationHelper
    {        
        public AnimationHelper()
        {
            
        }

        public void processPlayFX(UInt32 fxID)
        {
            Output.writeToLogForConsole("PLAY FX FOR " + fxID.ToString());
            DynamicArray din = new DynamicArray();

            byte[] animationId = NumericalUtils.uint32ToByteArray(fxID, 1);
            byte[] viewID = { 0x02, 0x00 };

            Random rand = new Random();
            ushort updateViewCounter = (ushort)rand.Next(3, 200);
            byte[] updateCount = NumericalUtils.uint16ToByteArrayShort(updateViewCounter);

            din.append(viewID);
            din.append(0x02);
            din.append(0x80); 
            din.append(0x80); 
            din.append(0x80); 
            din.append(0x90); 
            din.append(0xed); 
            din.append(0x00);
            din.append(0x30);
            din.append(animationId);
            din.append(updateCount);

            Store.currentClient.messageQueue.addObjectMessage(din.getBytes(), false);
        
        }

        public void processAnimation()
        {

        }

        public void processJackout()
        {
            string jackoutMessage = "02 03 02 00 03 28 03 c0 00 74 00 10 3a 8f 7f c7 00 00 be 42 04 81 93 46 25 6f 82 23 80 80 80 80 80 80 10 <state>01</state> 00 00";
        }

    }
}
