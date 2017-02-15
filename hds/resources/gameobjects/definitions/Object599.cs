using System;
namespace hds
{
    public class Object599 : GameObject
    {

        
        public int SelfViewGroups;

        public  Attribute EvadeShieldHealth = new Attribute(1, "EvadeShieldHealth");
        public  Attribute CharacterName = new Attribute(32, "CharacterName");
        public  Attribute TitleAbility = new Attribute(4, "TitleAbility");
        public  Attribute CombatantMode = new Attribute(1, "CombatantMode");
        public  Attribute JumpFlags = new Attribute(2, "JumpFlags");
        public  Attribute ConditionStateFlags = new Attribute(4, "ConditionStateFlags");
        public  Attribute Jumping = new Attribute(1, "Jumping");
        public  Attribute EffectCounter = new Attribute(1, "EffectCounter");
        public  Attribute UseRSIDescription = new Attribute(1, "UseRSIDescription");
        public  Attribute YawInterval = new Attribute(1, "Yaw Interval");
        public  Attribute MasterCharacterID = new Attribute(4, "MasterCharacterID");
        public  Attribute MoreInfoID = new Attribute(4, "MoreInfoID");
        public  Attribute StealthLevel = new Attribute(1, "StealthLevel");
        public  Attribute StopFollowActiveTracker = new Attribute(1, "StopFollowActiveTracker");
        public  Attribute IsDead = new Attribute(1, "IsDead");
        public  Attribute DissemblingType = new Attribute(2, "DissemblingType");
        public  Attribute JumpEndTime = new Attribute(4, "JumpEndTime");
        public  Attribute HalfExtents = new Attribute(12, "HalfExtents");
        public  Attribute WanderRange = new Attribute(4, "WanderRange");
        public  Attribute ScriptCounter = new Attribute(1, "ScriptCounter");
        public  Attribute EvadeShieldDamageScale = new Attribute(4, "EvadeShieldDamageScale");
        public  Attribute Description = new Attribute(4, "Description");
        public  Attribute EffectID = new Attribute(4, "EffectID");
        public  Attribute FactionID = new Attribute(4, "FactionID");
        public  Attribute DontAttack = new Attribute(1, "DontAttack");
        public  Attribute NoiseLevel = new Attribute(1, "NoiseLevel");
        public  Attribute ExclusiveLocator = new Attribute(18, "ExclusiveLocator");
        public  Attribute Position = new Attribute(24, "Position");
        public  Attribute ProneState = new Attribute(4, "ProneState");
        public  Attribute CancelAbility = new Attribute(4, "CancelAbility");
        public  Attribute UseCount = new Attribute(1, "UseCount");
        public  Attribute MissionKey = new Attribute(4, "MissionKey");
        public  Attribute Stance = new Attribute(1, "Stance");
        public  Attribute OrganizationID = new Attribute(1, "OrganizationID");
        public  Attribute IsEnemy = new Attribute(1, "IsEnemy");
        public  Attribute Action = new Attribute(1, "Action");
        public  Attribute NPCRank = new Attribute(1, "NPCRank");
        public  Attribute IsFriendly = new Attribute(1, "IsFriendly");
        public  Attribute Demeanor = new Attribute(1, "Demeanor");
        public  Attribute MovementScale = new Attribute(4, "MovementScale");
        public  Attribute CancelAbilityCounter = new Attribute(1, "CancelAbilityCounter");
        public  Attribute JumpPeakHeight = new Attribute(4, "JumpPeakHeight");
        public  Attribute FollowActiveTracker = new Attribute(1, "FollowActiveTracker");
        public  Attribute GuardType = new Attribute(1, "GuardType");
        public  Attribute ExclusiveType = new Attribute(1, "ExclusiveType");
        public  Attribute Level = new Attribute(1, "Level");
        public  Attribute GiverActiveTracker = new Attribute(1, "GiverActiveTracker");
        public  Attribute Health = new Attribute(2, "Health");
        public  Attribute EffectCommand = new Attribute(4, "EffectCommand");
        public  Attribute JumpDestination = new Attribute(24, "JumpDestination");
        public  Attribute DebuffState = new Attribute(1, "DebuffState");
        public  Attribute TakerActiveTracker = new Attribute(1, "TakerActiveTracker");
        public  Attribute RSIDescription = new Attribute(15, "RSIDescription");
        public  Attribute MaxHealth = new Attribute(2, "MaxHealth");
        public  Attribute CurrentState = new Attribute(4, "CurrentState");
        public  Attribute IsEscortable = new Attribute(1, "IsEscortable");
        public  Attribute EquippedItemID = new Attribute(4, "EquippedItemID");
        public  Attribute SpawnFX = new Attribute(4, "SpawnFX");
        public  Attribute DuelID = new Attribute(4, "DuelID");
        public  Attribute TalkDefaultable = new Attribute(1, "TalkDefaultable");
        public  Attribute OwnerCharacterID = new Attribute(4, "OwnerCharacterID");
        public  Attribute PutActiveTracker = new Attribute(1, "PutActiveTracker");
        public  Attribute RelevancyFlags = new Attribute(1, "RelevancyFlags");
        public  Attribute TalkActiveTracker = new Attribute(1, "TalkActiveTracker");
        public  Attribute StealthAwareness = new Attribute(1, "StealthAwareness");
        public  Attribute CurrentStateContainer = new Attribute(4, "CurrentStateContainer");



        public Object599()
            : base(66, 47, 10, 7, "NPC_BASE", 0x257, 0xFFFFFFFF)
        {
            this.AddAttribute(ref EvadeShieldHealth, 0, 32);
            this.AddAttribute(ref CharacterName, 1, -1);
            this.AddAttribute(ref TitleAbility, 2, 2);
            this.AddAttribute(ref CombatantMode, 3, 44);
            this.AddAttribute(ref JumpFlags, 4, 20);
            this.AddAttribute(ref ConditionStateFlags, 5, 43);
            this.AddAttribute(ref Jumping, 6, -1);
            this.AddAttribute(ref EffectCommand, 7, 35);
            this.AddAttribute(ref UseRSIDescription, 8, 1);
            this.AddAttribute(ref YawInterval, 9, -1);
            this.AddAttribute(ref MasterCharacterID, 10, 17);
            this.AddAttribute(ref MoreInfoID, 11, -1);
            this.AddAttribute(ref StealthLevel, 12, -1);
            this.AddAttribute(ref StopFollowActiveTracker, 13, 6);
            this.AddAttribute(ref IsDead, 14, 26);
            this.AddAttribute(ref DissemblingType, 15, 39);
            this.AddAttribute(ref JumpEndTime, 16, 21);
            this.AddAttribute(ref HalfExtents, 17, -1);
            this.AddAttribute(ref WanderRange, 18, -1);
            this.AddAttribute(ref ScriptCounter, 19, -1);
            this.AddAttribute(ref EvadeShieldDamageScale, 20, -1);
            this.AddAttribute(ref Description, 21, 40);
            this.AddAttribute(ref EffectID, 22, 34);
            this.AddAttribute(ref FactionID, 23, 31);
            this.AddAttribute(ref DontAttack, 24, 38);
            this.AddAttribute(ref NoiseLevel, 25, -1);
            this.AddAttribute(ref ExclusiveLocator, 26, -1);
            this.AddAttribute(ref Position, 27, -1);
            this.AddAttribute(ref ProneState, 28, 11);
            this.AddAttribute(ref CancelAbility, 29, 46);
            this.AddAttribute(ref UseCount, 30, 1);
            this.AddAttribute(ref MissionKey, 31, 15);
            this.AddAttribute(ref Stance, 32, -1);
            this.AddAttribute(ref OrganizationID, 33, 12);
            this.AddAttribute(ref IsEnemy, 34, 25);
            this.AddAttribute(ref Action, 35, -1);
            this.AddAttribute(ref NPCRank, 36, 13);
            this.AddAttribute(ref IsFriendly, 37, 23);
            this.AddAttribute(ref Demeanor, 38, -1);
            this.AddAttribute(ref MovementScale, 39, 14);
            this.AddAttribute(ref CancelAbility, 40, 45);
            this.AddAttribute(ref JumpPeakHeight, 41, 19);
            this.AddAttribute(ref FollowActiveTracker, 42, 30);
            this.AddAttribute(ref GuardType, 43, 28);
            this.AddAttribute(ref ExclusiveType, 44, -1);
            this.AddAttribute(ref Level, 45, 18);
            this.AddAttribute(ref GiverActiveTracker, 46, 29);
            this.AddAttribute(ref Health, 47, 27);
            this.AddAttribute(ref EffectCommand, 48, 36);
            this.AddAttribute(ref JumpDestination, 49, 22);
            this.AddAttribute(ref DebuffState, 50, -1);
            this.AddAttribute(ref TakerActiveTracker, 51, 5);
            this.AddAttribute(ref RSIDescription, 52, 9);
            this.AddAttribute(ref MaxHealth, 53, 16);
            this.AddAttribute(ref CurrentState, 54, 42);
            this.AddAttribute(ref IsEscortable, 55, 24);
            this.AddAttribute(ref EquippedItemID, 56, 33);
            this.AddAttribute(ref SpawnFX, 57, 7);
            this.AddAttribute(ref DuelID, 58, 37);
            this.AddAttribute(ref TalkDefaultable, 59, 3);
            this.AddAttribute(ref OwnerCharacterID, 60, -1);
            this.AddAttribute(ref PutActiveTracker, 61, 10);
            this.AddAttribute(ref RelevancyFlags, 62, 8);
            this.AddAttribute(ref TalkActiveTracker, 63, 4);
            this.AddAttribute(ref StealthAwareness, 64, -1);
            this.AddAttribute(ref CurrentStateContainer, 65, 41);

        }

    }
}
