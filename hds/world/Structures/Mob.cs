using System;
using System.Collections.Generic;
using System.Threading;
using hds.shared;

namespace hds
{
    public class Mob
    {
        private ulong entityID;
        private ushort mobID;       
        private ushort district;
        private string districtName;
        private byte[] goId = {0x57, 0x02};
        private ushort level = 1;
        private ushort healthC;
        private ushort healthM;
        private ushort hitCounter;
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
        private uint weapon;
        private int state; // 0= nothing, 1=rotating (just an example - must check this near from the npc class)
        private bool is_dead;
        private bool is_lootable;
        private bool is_spawned = true;
        public bool isUpdateable = false;

        public enum statusList
        {
            WALKING = 0x00,
            WALKING_BACK = 0x01,
            STANDING = 0x02,
            STANDING_BACK = 0x03,
            STANDING_COMBAT = 0x04,
            COMBAT = 0x05
        }

        public int currentState = 0x02;
        public List<PacketContent> updateData;
        public Timer mobUpdateTimer;


        public enum Animations
        {
            STAND = 0x00,
            WALK = 0x01,
            WALK_BACKWARDS = 0x02
        }


        public Mob()
        {
            updateData = new List<PacketContent>();
        }

        public void setGoId(byte[] objectID)
        {
            goId = objectID;
        }

        public void setEntityId(ulong _entityId)
        {
            entityID = _entityId;
        }

        public void setMobId(ushort mobID)
        {
            this.mobID = mobID;
        }

        public void setDistrict(ushort _district)
        {
            district = _district;
        }

        public void setDistrictName(string _districtName)
        {
            districtName = _districtName;
        }

        public void setLevel(ushort level)
        {
            this.level = level;
        }

        public void setHealthC(ushort currentHealth)
        {
            healthC = currentHealth;
        }

        public void setHealthM(ushort maxHealth)
        {
            healthM = maxHealth;
        }

        public void setRsiHex(string _rsihex)
        {
            rsi_hex = _rsihex;
        }

        public void setRotation(uint rotation)
        {
            this.rotation = rotation;
        }

        public void setXPos(double xPosition)
        {
            xPos = xPosition;
        }

        public void setYPos(double yPosition)
        {
            yPos = yPosition;
        }

        public void setZPos(double zPosition)
        {
            zPos = zPosition;
        }

        public void setName(string mobName)
        {
            name = mobName;
        }

        public void setWeapon(uint weapon)
        {
            this.weapon = weapon;
        }

        public void setState(int state)
        {
            this.state = state;
        }

        public void setIsDead(bool dead)
        {
            is_dead = dead;
        }

        public void setIsLootable(bool lootable)
        {
            is_lootable = lootable;
        }

        public void setIsSpawned(bool spawned)
        {
            is_spawned = spawned;
        }

        public ulong getEntityId()
        {
            return entityID;
        }

        public ushort getMobId()
        {
            return mobID;
        }

        public ushort getDistrict()
        {
            return district;
        }

        public string getDistrictName()
        {
            return districtName;
        }

        public byte[] getGoId()
        {
            return goId;
        }

        public ushort getLevel()
        {
            return level;
        }

        public ushort getHealthC()
        {
            return healthC;
        }

        public ushort getHealthM()
        {
            return healthM;
        }

        public string getRsiHex()
        {
            return rsi_hex;
        }

        public uint getRotation()
        {
            return rotation;
        }

        public double getXPos()
        {
            return xPos;
        }

        public double getYPos()
        {
            return yPos;
        }

        public double getZPos()
        {
            return zPos;
        }

        public string getName()
        {
            return name;
        }

        public uint getWeapon()
        {
            return weapon;
        }

        public int getState()
        {
            return state;
        }

        public bool getIsDead()
        {
            return is_dead;
        }

        public bool getIsLootable()
        {
            return is_lootable;
        }

        public bool getIsSpawned()
        {
            return is_spawned;
        }


        public Object599 getCreationData()
        {
            var themob = new Object599();
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

            themob.CharacterName.setValue(getName());
            themob.Health.setValue(getHealthC());
            themob.MaxHealth.setValue(getHealthM());
            themob.Level.setValue(getLevel());
            themob.NPCRank.setValue(1);
            themob.RSIDescription.setValue(StringUtils.hexStringToBytes(getRsiHex()));
            themob.MoreInfoID.setValue(StringUtils.hexStringToBytes(getRsiHex())); // ToDo: Make dynamic
            themob.StopFollowActiveTracker.setValue(0x01);
            themob.ScriptCounter.setValue(1);
            themob.Description.setValue(StringUtils.hexStringToBytes(getRsiHex())); // ToDo: Make dynamic
            themob.CombatantMode.setValue(0x22);
            themob.Position.setValue(NumericalUtils.doublesToLtVector3d(getXPos(), getYPos(), getZPos()));
            themob.YawInterval.setValue((byte) getRotation());
            themob.IsDead.setValue(false);
            themob.IsEnemy.setValue(true);
            themob.IsFriendly.setValue(false);
            themob.StealthAwareness.setValue(0x11);
            byte[] titleAbility = {0x00, 0x10, 0x00, 0x00};
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
            var math = new Maths();

            // First choose a new pos in the range
            var newPos = math.RandomPointOnCircle((float) xBase, (float) yBase, (float) zBase, 5.0f * 100);
            #if DEBUG
            Output.WriteDebugLog("Mob Goes from X: " + getXPos() + " , Z: " + getZPos() + " to X: " + newPos.x +
                                 ", Z: " + newPos.z);
            #endif


            var xNew = (double) newPos.x;
            var zNew = (double) newPos.z;

            destination = newPos;
            // Try to calculate rotation

            // Oh this seems to match ...needs more testing later when we fixed random pos
            var yaw = Math.Atan((xNew - getXPos()) / (zNew - getZPos())) * 128 / Math.PI;
            var calcRotation = Math.Atan2(Math.Cos(xNew), Math.Sin(zNew) * Math.Sin(zNew)) * 128 / Math.PI;
            var testRot = Math.Atan2(xNew, zNew) * 180 / Math.PI;
            var testRot2 = Math.Atan2(xNew, zNew) * 128 / Math.PI;

            #if DEBUG
            Output.WriteDebugLog("Test Rot with 360 : " + testRot + "| 255 : " + testRot2 + " AND THE YAW: " + yaw +
                                 " (Cast to uint16 : " + Convert.ToInt16(yaw) + " )");
            #endif


            var yawVal = (int) Convert.ToInt16(yaw);

            if (zNew < getZPos() || xNew < getXPos())
            {
                #if DEBUG
                Output.WriteDebugLog("Need to adjust YAW + 128 from :" + yawVal + " to: " + (yawVal + 128));
                #endif
                yawVal = yawVal + 128;
            }
            else
            {
                #if DEBUG
                Output.WriteDebugLog("Need to adjust YAW - 128 from :" + yawVal + " to: " + (yawVal - 128));
                #endif
                yawVal = yawVal - 128;
            }
            
            #if DEBUG
            Output.WriteDebugLog("YAW VAL :" + yawVal);
            #endif

            rotation = (ushort) yawVal;
            #if DEBUG
            Output.WriteDebugLog("Calc Rotation : " + calcRotation + " and to UINT : " + (uint) calcRotation);
            #endif

            // Calculate the distance for seconds to move
            var requiredSeconds = (int) (math.distance2Coords((float) xPos, (float) xNew, (float) zPos, (float) zNew) /
                                         0.176 / 500 * 0.9586);
            return requiredSeconds;
        }

        public void DoMobUpdate(object e)
        {
            if (this.isUpdateable)
            {
                var nextMoveTime = 5;
                var rand = new Random();
                switch (currentState)
                {
                    case (int) statusList.WALKING_BACK:
                    case (int) statusList.WALKING:
                        // stop npc
                        currentState = (int) statusList.STANDING;
                        setXPos(destination.x);
                        setZPos(destination.z);
                        updateAnimation((byte) Animations.STAND);
                        nextMoveTime = rand.Next(5, 15);
                        break;
                    case (int) statusList.STANDING_BACK:
                    case (int) statusList.STANDING:
                        // Starts walking
                        currentState = (int) statusList.WALKING;
                        // ToDo: start walking NPC and recalculate
                        nextMoveTime = generatePathTable();
                        updateAnimation((byte) Animations.WALK);
                        // Generate walking packet message (raw)
                        break;
                    case (int) statusList.COMBAT:
                        // ToDo: he should attack the player who is in combat with him
                        // But he shouldnt move
                        break;
                }

                mobUpdateTimer = new Timer(DoMobUpdate, this, nextMoveTime * 1000, 0);
            }
            
        }

        public void HitEnemyWithDamage(ushort hitValue, uint hitFxId)
        {
            // Change to combat so that he isnt moving anymore
            currentState = (int) statusList.COMBAT;
            if (healthC - hitValue <= 0)
                healthC = 0;
            else
                healthC -= hitValue;
            hitCounter++;

            // Format 04 80 80 80 80 c0 <uint16 health> c0 <uint32 fxid> 01(but unknown for what it is) <uint8 random>
            // Example Full: send 02 03 <viewID> 04 80 80 80 c0 02 00 c0 c1 01 00 28 01 01 00 00;
            var hitPacket = new PacketContent();
            hitPacket.addByte(0x04);
            hitPacket.addByte(0x80);
            hitPacket.addByte(0x80);
            hitPacket.addByte(0x80);
            hitPacket.addByte(0xc0);
            hitPacket.addUint16(healthC, 1);
            hitPacket.addByte(0xc0);
            hitPacket.addUint32(hitFxId, 1); // FX ID 
            hitPacket.addByte(0x01);
            hitPacket.addUintShort(hitCounter); // Animation Count or something - must be an another as before
            Store.world.SendViewUpdateToClientsWhoHasSpawnedView(hitPacket, this);
        }

        public void updateAnimation(byte animationByte)
        {
            // Through Animation Packet here if you want to start / stop walking etc.
            // aka 0x0e
            var pak = new PacketContent();
            pak.addByte(0x02);
            pak.addByte(0x0e);
            pak.addByte(animationByte);
            pak.addByte((byte) rotation);
            pak.addFloatLtVector3f((float) getXPos(), (float) getYPos(), (float) getZPos());

            Store.world.SendViewUpdateToClientsWhoHasSpawnedView(pak, this);
        }

        public void updateCombat()
        {
        }
    }
}