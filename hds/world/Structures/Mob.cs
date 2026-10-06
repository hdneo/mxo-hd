using System;
using System.Collections.Generic;
using hds.Core;
using hds.Core.Wire;
using hds.Protocol.View;
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
        private string name;
        private uint weapon;
        private int state;
        private bool is_dead;
        private bool is_lootable;
        private bool is_spawned = true;

        /// <summary>
        /// Movement::Action values of the char movement component as the original server used them for NPCs
        /// (packet logs: stand; walk while the position moves ~185 units/s; run ~450 units/s; turning in place
        /// while the yaw increases (4) or decreases (5)).
        /// </summary>
        public enum Animations
        {
            STAND = 0x00,
            WALK = 0x01,
            RUN = 0x03,
            TURN_LEFT = 0x04,
            TURN_RIGHT = 0x05
        }

        private enum MoveState
        {
            Standing,
            Walking,
            Combat
        }

        /// <summary>World units per second of a walking NPC (measured in the packet logs: 175-195).</summary>
        public const double WalkSpeed = 185.0;

        /// <summary>The original server sent a walking NPC's position about 4 times a second.</summary>
        public static readonly TimeSpan WalkUpdateInterval = TimeSpan.FromMilliseconds(250);

        /// <summary>Mobs wander around their spawn point.</summary>
        public const double WanderRadius = 500.0;

        private MoveState moveState = MoveState.Standing;
        private ScheduledTask moveTimer;
        private bool wandering;
        private double walkStartX, walkStartZ, walkEndX, walkEndZ;
        private long walkStartMs;
        private double walkDurationMs;
        private static readonly Random random = new Random();

        public Mob()
        {
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
            if (dead) HaltMovement();
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


        public GameObject getCreationData()
        {
            var themob = new GameObjectDefinitions().Object599;
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

        #region movement

        /// <summary>Yaw byte (0..255 = 0..360 degrees) of a direction: atan2(dx, dz), verified with the logs.</summary>
        public static byte YawTowards(double dx, double dz)
        {
            var yaw = Math.Round(Math.Atan2(dx, dz) * 128.0 / Math.PI);
            return (byte) ((int) yaw & 0xFF);
        }

        /// <summary>Starts the random walks around the spawn point (idempotent, called when a client sees the mob).</summary>
        public void StartWandering()
        {
            if (wandering || is_dead) return;
            wandering = true;
            if (moveState == MoveState.Standing) ScheduleNextWalk(random.Next(2, 8));
        }

        /// <summary>Stops in place and faces the attacker - the fight must not move the mob.</summary>
        public void EnterCombat(double faceX, double faceZ)
        {
            HaltMovement();
            moveState = MoveState.Combat;
            var dx = faceX - xPos;
            var dz = faceZ - zPos;
            if (Math.Abs(dx) > 0.01 || Math.Abs(dz) > 0.01) rotation = YawTowards(dx, dz);
            BroadcastMovement(CharMvt.FlagAction | CharMvt.FlagYaw | CharMvt.FlagPosition, Animations.STAND);
        }

        /// <summary>The fight ended: dead mobs stay, living ones start wandering again after a pause.</summary>
        public void LeaveCombat()
        {
            if (moveState != MoveState.Combat) return;
            moveState = MoveState.Standing;
            if (is_dead) return;
            if (wandering) ScheduleNextWalk(random.Next(5, 15));
        }

        /// <summary>Stops a walk at the current (interpolated) position.</summary>
        public void HaltMovement()
        {
            moveTimer?.Cancel();
            moveTimer = null;
            if (moveState != MoveState.Walking) return;
            UpdateWalkPosition();
            moveState = MoveState.Standing;
            BroadcastMovement(CharMvt.FlagAction | CharMvt.FlagYaw | CharMvt.FlagPosition, Animations.STAND);
        }

        /// <summary>Movement for a client that just spawned the mob (the creation data has no action), or null.</summary>
        public byte[] GetCurrentMovementUpdate()
        {
            return moveState == MoveState.Walking
                ? BuildMovementUpdate(CharMvt.FlagAction | CharMvt.FlagYaw | CharMvt.FlagPosition, Animations.WALK)
                : null;
        }

        private void ScheduleNextWalk(int seconds)
        {
            moveTimer?.Cancel();
            moveTimer = Store.Loop.Schedule(TimeSpan.FromSeconds(seconds), StartWalk);
        }

        private void StartWalk()
        {
            moveTimer = null;
            if (is_dead || moveState != MoveState.Standing) return;

            // random point inside the wander circle, not too close to the current position
            double x, z;
            var tries = 0;
            do
            {
                var angle = random.NextDouble() * Math.PI * 2;
                var radius = WanderRadius * Math.Sqrt(random.NextDouble());
                x = xBase + radius * Math.Cos(angle);
                z = zBase + radius * Math.Sin(angle);
            } while (Distance(x, z) < 100 && ++tries < 10);

            walkStartX = xPos;
            walkStartZ = zPos;
            walkEndX = x;
            walkEndZ = z;
            walkStartMs = Environment.TickCount64;
            walkDurationMs = Distance(x, z) / WalkSpeed * 1000.0;
            rotation = YawTowards(x - xPos, z - zPos);
            moveState = MoveState.Walking;

            #if DEBUG
            Output.WriteDebugLog("[Mob] " + name + " walks from (" + xPos.ToString("0") + "," + zPos.ToString("0") + ") to (" +
                                 x.ToString("0") + "," + z.ToString("0") + ") yaw " + rotation + " in " + walkDurationMs.ToString("0") + " ms");
            #endif

            BroadcastMovement(CharMvt.FlagAction | CharMvt.FlagYaw | CharMvt.FlagPosition, Animations.WALK);
            moveTimer = Store.Loop.Schedule(WalkUpdateInterval, WalkTick);
        }

        private void WalkTick()
        {
            moveTimer = null;
            if (moveState != MoveState.Walking) return;

            if (UpdateWalkPosition())
            {
                moveState = MoveState.Standing;
                BroadcastMovement(CharMvt.FlagAction | CharMvt.FlagYaw | CharMvt.FlagPosition, Animations.STAND);
                if (wandering) ScheduleNextWalk(random.Next(5, 15));
                return;
            }

            BroadcastMovement(CharMvt.FlagPosition, Animations.WALK);
            moveTimer = Store.Loop.Schedule(WalkUpdateInterval, WalkTick);
        }

        /// <summary>Moves the stored position along the walk; true when the destination is reached.</summary>
        private bool UpdateWalkPosition()
        {
            var t = walkDurationMs <= 0 ? 1.0 : (Environment.TickCount64 - walkStartMs) / walkDurationMs;
            if (t >= 1.0)
            {
                xPos = walkEndX;
                zPos = walkEndZ;
                return true;
            }
            xPos = walkStartX + (walkEndX - walkStartX) * t;
            zPos = walkStartZ + (walkEndZ - walkStartZ) * t;
            return false;
        }

        private double Distance(double x, double z) => Math.Sqrt((x - xPos) * (x - xPos) + (z - zPos) * (z - zPos));

        /// <summary>NPC view update: component mask 0x02 (the char movement is the second component of NPCs).</summary>
        public byte[] BuildMovementUpdate(byte flags, Animations action)
        {
            var mvt = new CharMvt {Flags = flags, Action = (byte) action, Yaw = (byte) rotation};
            mvt.X = (float) xPos;
            mvt.Y = (float) yPos;
            mvt.Z = (float) zPos;
            var writer = new WireWriter(20);
            writer.WriteByte(0x02);
            mvt.WriteToClient(writer, false);
            return writer.ToArray();
        }

        private void BroadcastMovement(byte flags, Animations action)
        {
            Store.world?.SendViewUpdateToClientsWhoHasMobSpawned(BuildMovementUpdate(flags, action), this);
        }

        #endregion

        public void HitEnemyWithDamage(ushort hitValue, uint hitFxId)
        {
            // a hit mob stops walking
            HaltMovement();
            if (healthC - hitValue <= 0)
                healthC = 0;
            else
                healthC -= hitValue;
            hitCounter++;

            // Format 04 80 80 80 80 c0 <uint16 health> c0 <uint32 fxid> 01(but unknown for what it is) <uint8 random>
            // Example Full: send 02 03 <viewID> 04 80 80 80 c0 02 00 c0 c1 01 00 28 01 01 00 00;
            var hitPacket = new PacketContent();
            hitPacket.AddByte(0x04);
            hitPacket.AddByte(0x80);
            hitPacket.AddByte(0x80);
            hitPacket.AddByte(0x80);
            hitPacket.AddByte(0xc0);
            hitPacket.AddUint16(healthC, 1);
            hitPacket.AddByte(0xc0);
            hitPacket.AddUint32(hitFxId, 1); // FX ID 
            hitPacket.AddByte(0x01);
            hitPacket.AddUShort(hitCounter); // Animation Count or something - must be an another as before
            Store.world.SendViewUpdateToClientsWhoHasMobSpawned(hitPacket, this);
        }

    }
}