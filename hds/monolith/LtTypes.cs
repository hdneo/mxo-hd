using System;
using System.Collections.Generic;
using System.Text;

namespace hds {

    public interface LtType {
        int _Size { get;} // Internal usage, not for developpers
    }

    public class LtQuaternion:LtType {
        public float a { get; set; }
        public float b { get; set; }
        public float c { get; set; }
        public float d { get; set; }

        public int _Size { get { return 16; } }

        public LtQuaternion() {
            a = b = c = 0;
            d = 1;
        }
    }

    public class LtVector3f : LtType {

        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public int _Size { get { return 12; } }

        public LtVector3f() {
            x = y = z = 0;
        }

        public LtVector3f(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

    }

    public class LtVector3d : LtType {

        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }

        public int _Size { get { return 24; } }

        public LtVector3d() {
            x = y = z = 0;
        }

        public LtVector3d(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    // HD Created objects types

    public class LtString : LtType {
        public string InnerStr { get; set; }
        public int _Size { get { return InnerStr.Length; } }
    }

    public class LtBlob : LtType {
    
        public byte[] BlobValue {get;set;}
        
        public int _Size { get { return BlobValue.Length; } }
    
    }

    public class LtOffset : LtType {
    
        public int Size {get;set;}
        public int ValuesToOffset { get; set; }
        public int SourceLength { get; set; }

        public int _Size { get { return Size; } }

    }

}
