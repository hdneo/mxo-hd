using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using hds.shared;

namespace hds
{
    class CommandHandler{

        public void processWhereamiCommand(ref byte[] packet)
        {

            double x = 0; double y = 0; double z = 0;
            byte[] Ltvector3d = Store.currentClient.playerInstance.Position.getValue();
            NumericalUtils.LtVector3dToDoubles(Ltvector3d, ref x, ref y, ref z);

            byte[] xPos = NumericalUtils.floatToByteArray((float)x,1);
            byte[] yPos = NumericalUtils.floatToByteArray((float)y, 1);
            byte[] zPos = NumericalUtils.floatToByteArray((float)z, 1);

            ServerPackets serverpacket = new ServerPackets();
            serverpacket.sendWhereami(Store.currentClient, xPos, yPos, zPos);
        }

        public void processWhoCommand(ref byte[] packet)
        {
            // ToDo: implement
            ServerPackets serverpacket = new ServerPackets();
            
            serverpacket.SendWhoCMD(Store.currentClient);

        }
    }
}
