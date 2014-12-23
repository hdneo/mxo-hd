using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using hds.shared;

namespace hds
{
    class CommandHandler{

        public PacketsUtils pu;
        public ArrayUtils au;
        public NumericalUtils nu;
        public StringUtils su;

        public CommandHandler(){
            this.au = new ArrayUtils();
            this.su = new StringUtils();
            this.nu = new NumericalUtils();
            this.pu = new PacketsUtils();
        }

        public void processWhereamiCommand(ref byte[] packet){
            Output.WriteLine("[COMMAND HELPER] WHERE AM I ");
            double x = 0; double y = 0; double z = 0;
            byte[] Ltvector3d = Store.currentClient.playerInstance.Position.getValue();
            NumericalUtils.LtVector3dToDoubles(Ltvector3d, ref x, ref y, ref z);

            byte[] xPos = NumericalUtils.floatToByteArray((float)x,1);
            byte[] yPos = NumericalUtils.floatToByteArray((float)y, 1);
            byte[] zPos = NumericalUtils.floatToByteArray((float)z, 1);            

            string posHex = StringUtils.bytesToString_NS(xPos) + StringUtils.bytesToString_NS(yPos) + StringUtils.bytesToString_NS(zPos);

            ServerPackets serverpacket = new ServerPackets();
            serverpacket.sendWhereami(Store.currentClient, xPos, yPos, zPos);

            

        }
    }
}
