using System;
using System.Collections.Generic;
using System.Text;

namespace hds
{
    [Serializable()] 
    public class StaticWorldObject
    {
        public bool exterior;
        public double pos_x;
        public double pos_y;
        public double pos_z;
        public double rot;
        public string quat; // should be changed
        public UInt16 sectorID;
        public UInt16 metrId;
        public UInt32 mxoId;
        public UInt32 staticId;
        public byte[] type;

        public StaticWorldObject()
        {
        }
    }
}
