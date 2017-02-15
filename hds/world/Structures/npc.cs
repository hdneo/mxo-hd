using System;
using System.Collections;
using System.Text;

namespace hds
{
    public class npc
    {
        private UInt64 entityID;
        private UInt16 mobID;
        private UInt16 district;
        private string districtName;
        private byte[] goId = { 0x57, 0x02 };
        private ushort level = 1;
        private UInt16 healthC = 0;
        private UInt16 healthM = 0;
        private string rsi_hex;
        private uint rotation;
        private double xPos;
        private double yPos;
        private double zPos;
        public double xBase;
        public double yBase;
        public double zBase;
        private LtVector3f destination;
        private string name;
        private UInt32 weapon;
        public bool updateClient = false; // Should indicate that in the loop the client should be updated
        private int state; // 0= nothing, 1=rotating (just an example - must check this near from the npc class)
        private bool is_dead = false;
        private bool is_lootable = false;
        private bool is_spawned = true;
        public long next_move_time;
        public double prev_x_pos = 0;  // Important for some calc tests
        public double prev_z_pos = 0;  // Important for some calc tests
        public enum statusList : int { WALKING = 0x00, WALKING_BACK = 0x01, STANDING = 0x02, STANDING_BACK = 0x03, STANDING_COMBAT = 0x04 };
        public int currentState = 0x02;
        public byte[] updateData;

        public int ticksNeeded = 10;
        public int ticksReceived = 0;

        public enum Animations : int
        {
            STAND = 0x00,
            WALK = 0x01,
            WALK_BACKWARDS = 0x02
        }
        

        public npc()
        {
            this.next_move_time = TimeUtils.getUnixTimeUint32() + (30); // 30 seconds for a move to test
        }

        public void setGoId(byte[] objectID) { this.goId = objectID; }
        public void setEntityId(UInt64 _entityId) { this.entityID = _entityId; }
        public void setMobId(UInt16 mobID){ this.mobID = mobID; }
        public void setDistrict(UInt16 _district) { this.district = _district; }
        public void setDistrictName(string _districtName) { this.districtName = _districtName; }
        public void setLevel(ushort level) { this.level = level; }
        public void setHealthC(UInt16 currentHealth) { this.healthC = currentHealth; }
        public void setHealthM(UInt16 maxHealth) { this.healthM = maxHealth; }
        public void setRsiHex(string _rsihex) { this.rsi_hex = _rsihex; }
        public void setRotation(uint rotation) { this.rotation = rotation; }
        public void setXPos(double xPosition) { this.xPos = xPosition; }
        public void setYPos(double yPosition) { this.yPos = yPosition; }
        public void setZPos(double zPosition) { this.zPos = zPosition; }
        public void setName(string mobName) { this.name = mobName; }
        public void setWeapon(UInt32 weapon) { this.weapon = weapon; }
        public void setState(int state) { this.state = state; }
        public void setIsDead(bool dead) { this.is_dead = dead; }
        public void setIsLootable(bool lootable) { this.is_lootable = lootable; }
        public void setIsSpawned(bool spawned) { this.is_spawned = spawned; }

        public UInt64 getEntityId() { return this.entityID; }
        public UInt16 getMobId() { return this.mobID; }
        public UInt16 getDistrict() { return this.district; }
        public string getDistrictName() { return this.districtName; }
        public byte[] getGoId(){ return this.goId; }
        public ushort getLevel(){ return this.level;}
        public UInt16 getHealthC(){return this.healthC;}
        public UInt16 getHealthM(){ return this.healthM;}
        public string getRsiHex() { return this.rsi_hex;  }
        public uint getRotation() { return this.rotation; }
        public double getXPos() { return this.xPos; }
        public double getYPos() { return this.yPos; }
        public double getZPos() { return this.zPos; }
        public string getName() { return this.name; }
        public UInt32 getWeapon() { return this.weapon; }
        public int getState() { return this.state; }
        public bool getIsDead() { return this.is_dead; }
        public bool getIsLootable() { return this.is_lootable; }
        public bool getIsSpawned() { return this.is_spawned; }


        public Object599 getCreationData()
        {
            Object599 themob = new Object599();
            themob.DisableAllAttributes();

            themob.CharacterName.enable();
            themob.Health.enable();
            themob.MaxHealth.enable();
            themob.Level.enable();
            themob.NPCRank.enable();
            themob.RSIDescription.enable();
            themob.MoreInfoID.enable();
            themob.StopFollowActiveTracker.enable();
            themob.ScriptCounter.enable();
            themob.Description.enable();
            themob.CombatantMode.enable();
            themob.Position.enable();
            themob.YawInterval.enable();
            themob.IsDead.enable();
            themob.IsEnemy.enable();
            themob.IsFriendly.enable();
            themob.StealthAwareness.enable();
            themob.TitleAbility.enable();
            themob.ConditionStateFlags.enable();
            themob.CurrentState.enable();

            themob.CharacterName.setValue((string)this.getName());
            themob.Health.setValue(this.getHealthC());
            themob.MaxHealth.setValue(this.getHealthM());
            themob.Level.setValue(this.getLevel());
            themob.NPCRank.setValue(1);
            themob.RSIDescription.setValue(StringUtils.hexStringToBytes(this.getRsiHex()));
            themob.MoreInfoID.setValue(StringUtils.hexStringToBytes(this.getRsiHex())); // ToDo: Make dynamic
            themob.StopFollowActiveTracker.setValue((byte)0x01);
            themob.ScriptCounter.setValue(1);
            themob.Description.setValue(StringUtils.hexStringToBytes(this.getRsiHex())); // ToDo: Make dynamic
            themob.CombatantMode.setValue((byte)0x22);
            themob.Position.setValue(NumericalUtils.doublesToLtVector3d(this.getXPos(), this.getYPos(), this.getZPos()));
            themob.YawInterval.setValue((byte)this.getRotation());
            themob.IsDead.setValue(false);
            themob.IsEnemy.setValue(true);
            themob.IsFriendly.setValue(false);
            themob.StealthAwareness.setValue((byte)0x11);
            byte[] titleAbility = { 0x00, 0x10, 0x00, 0x00 };
            themob.TitleAbility.setValue(titleAbility);
            themob.ConditionStateFlags.setValue(StringUtils.hexStringToBytes("00001000"));
            themob.CurrentState.setValue(StringUtils.hexStringToBytes("06080000"));

            return themob;
        }

        public int generatePathTable()
        {
            // We firs test this with just one moove from point x to y
            // To calc this we need to know the time the object needs to reach this point,
            // the new point and the rotation to calc pretty to sync client and server position is right
            // to get a smooth walking :)
            Maths math = new Maths();

            // First choose a new pos in the range
            LtVector3f newPos = math.RandomPointOnCircle((float)xBase,(float)yBase, (float)zBase, 5.0f * 100);
            Output.WriteDebugLog("Mob Goes from X: " + this.getXPos() + " , Z: " + this.getZPos() + " to X: " + newPos.a + ", Z: " + newPos.c);

            
            double xNew = (double)newPos.a;
            double zNew = (double)newPos.c;

            this.destination = newPos;
            // Try to calculate rotation
            
            
            // Oh this seems to match ...needs more testing later when we fixed random pos


            double yaw = Math.Atan((double)(xNew - getXPos()) / (zNew - getZPos()))*128/Math.PI;

            double calcRotation = Math.Atan2(Math.Cos(xNew), Math.Sin(zNew) * Math.Sin(zNew)) * 128/Math.PI;

            double testRot = Math.Atan2(xNew, zNew) * 180 / Math.PI;
            double testRot2 = Math.Atan2(xNew, zNew) * 128 / Math.PI;

            
            Output.WriteDebugLog("Test Rot with 360 : " + testRot + "| 255 : " + testRot2 + " AND THE YAW: " + yaw + " (Cast to uint16 : " + Convert.ToInt16(yaw) + " )");


            int yawVal = (int)Convert.ToInt16(yaw);

            if (zNew < this.getZPos() || xNew < this.getXPos())
            {
                Output.WriteDebugLog("Need to adjust YAW + 128 from :" + yawVal.ToString() + " to: " + (yawVal + 128).ToString());
                yawVal = yawVal + 128;
            }
            else
            {
                Output.WriteDebugLog("Need to adjust YAW - 128 from :" + yawVal.ToString() + " to: " + (yawVal - 128).ToString());
                yawVal = yawVal - 128;
            }

            Output.WriteDebugLog("YAW VAL :" + yawVal.ToString() );

            this.rotation = (ushort)yawVal;
            Output.WriteDebugLog("Calc Rotation : " + calcRotation.ToString() + " and to UINT : " + (uint)calcRotation);

            // Calculate the distance for seconds to move
            int requiredSeconds = (int)(((math.distance2Coords((float)xPos, (float)xNew, (float)zPos, (float)zNew) / 0.176) / 500) * 0.9586);
            return requiredSeconds;
        }

        public bool doTick()
        {
            ticksReceived++;
            // Reach the next action
            if (this.ticksReceived >= this.ticksNeeded)
            {
                int nextMoveTime = 5;
                switch (this.currentState)
                {
                    case ((int)statusList.WALKING_BACK):
                    case ((int)statusList.WALKING):
                        // stop npc
                        this.currentState = (int)statusList.STANDING;
                        this.setXPos(destination.a);
                        this.setZPos(destination.c);

                        Random rand = new Random();
                        nextMoveTime = rand.Next(10, 15);
                        
                        this.updateAnimation((byte)npc.Animations.STAND);
                        break;

                    case ((int)statusList.STANDING_BACK):
                    case ((int)statusList.STANDING):
                        // Starts walking
                        this.currentState = (int)statusList.WALKING;
                        // ToDo: start walking NPC and recalculate
                        nextMoveTime = this.generatePathTable();
                        this.updateAnimation((byte)npc.Animations.WALK);
                        // Generate walking packet message (raw)
                        break;

                }

                this.ticksReceived = 0;
                this.ticksNeeded = nextMoveTime;
                this.updateClient = true;
            }
            else
            {
                this.updateClient = false;
            }
            return this.updateClient;

        }

        public void updateAnimation(byte animationByte)
        {
            // Through Animation Packet here if you want to start / stop walking etc.
            // aka 0x0e
            PacketContent pak = new PacketContent();
            pak.addByte(0x02);
            pak.addByte(0x0e);
            pak.addByte(animationByte);
            pak.addByte((byte)this.rotation);
            pak.addFloatLtVector3f((float)this.getXPos(), (float)this.getYPos(), (float)this.getZPos());
            this.updateData = pak.returnFinalPacket();
        }

        public void updatePosition()
        {
            // Through update position Packet here - if you want to correct position only and dont care about the animation
            // aka 0x0c 
        }

        public byte[] getAndResetUpdateData()
        {
            byte[] thedata = this.updateData;          
            return thedata;
        }

        public void flushClientUpdateData()
        {
            Array.Clear(updateData, 0, this.updateData.Length);
        }

        public void updateCombat()
        {

        }
    }
}
