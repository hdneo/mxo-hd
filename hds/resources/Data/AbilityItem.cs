using System;
using System.Text;

namespace hds
{
    public class AbilityItem
    {
        private UInt16 AbilityID;
        private Int32 GOID;
        private string AbilityName;
        private bool isCastable;
        private float CastingTime;
        private byte[] castAnimStart;
        private byte[] castAnimMid;
        private byte[] castAnimEnd;
        private UInt32 ActivationFX { get; set; }
        private UInt32 AbilityExecutionFX { get; set;  }
        private Int16 valueFrom { get; set; }
        private UInt16 valueTo { get; set; }
        private bool isBuff { get; set; }
        private float buffTime { get; set; }

        public AbilityItem()
        {

        }

        public void setAbilityID(UInt16 ID)
        {
            this.AbilityID = ID;
        }

        public UInt16 getAbilityID()
        {
            return AbilityID;
        }

        public void setGOID(Int32 GOID)
        {
            this.GOID = GOID;
        }

        public Int32 getGOID()
        {
            return this.GOID;
        }

        public void setAbilityName(string name)
        {
            this.AbilityName = name;
        }

        public string getAbilityName()
        {
            return this.AbilityName;
        }

        public void setIsCastable(bool _isCastable)
        {
            this.isCastable = _isCastable;
        }

        public bool getIsCastable()
        {
            return this.isCastable;
        }

        public void setCastingTime(float _castingTime)
        {
            this.CastingTime = _castingTime;
        }

        public float getCastingTime()
        {
            return this.CastingTime;
        }

        public void setCastAnimStart(byte[] _castAnimStart)
        {
            this.castAnimStart = _castAnimStart;
        }

        public byte[] getCastAnimStart()
        {
            return this.castAnimStart;
        }

        public void setCastAnimMid(byte[] _castAnimMid)
        {
            this.castAnimMid = _castAnimMid;
        }

        public byte[] getCastAnimMid()
        {
            return this.castAnimMid;
        }

        public void setCastAnimEnd(byte[] _castAnimEnd)
        {
            this.castAnimEnd = _castAnimEnd;
        }

        public byte[] getCastAnimEnd()
        {
            return this.castAnimEnd;
        }

        public void setActivationFX(UInt32 _targetAnim)
        {
            this.ActivationFX = _targetAnim;
        }

        public UInt32 getActivationFX()
        {
            return this.ActivationFX;
        }

        public void setAbilityExecutionFX(UInt32 _AbilityExecutionFX)
        {
            this.AbilityExecutionFX = _AbilityExecutionFX;
        }

        public UInt32 getAbilityExecutionFX()
        {
            return this.AbilityExecutionFX;
        }



        public void setValueFrom(Int16 _valueFrom)
        {
            this.valueFrom = _valueFrom;
        }

        public Int16 getValueFrom()
        {
            return this.valueFrom;
        }

        public void setValueTo(UInt16 _valueTo)
        {
            this.valueTo = _valueTo;
        }

        public UInt16 getValueTo()
        {
            return this.valueTo;
        }

        public void setIsBuff(bool _isBuff)
        {
            this.isBuff = _isBuff;
        }

        public bool getIsBuff()
        {
            return this.isBuff;
        }

        public void setBuffTime(float _buffTime)
        {
            this.buffTime = _buffTime;
        }

        public float getBuffTime()
        {
            return this.buffTime;
        }


    }

}
