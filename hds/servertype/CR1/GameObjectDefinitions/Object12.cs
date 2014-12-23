using System;
namespace hds.servertype.cr1
//namespace hds
{
    public class Object12 : GameObject
    {

        public Attribute[] attributesSelfView;
        public int selfViewGroups;

        public Attribute ReputationMerovingian = new Attribute(2, "ReputationMerovingian");
        public Attribute ConquestPoints = new Attribute(4, "ConquestPoints");
        public Attribute CancelAbilityCounter = new Attribute(1, "CancelAbilityCounter");
        public Attribute RealLastName = new Attribute(32, "RealLastName");
        public Attribute ScriptCounter = new Attribute(1, "ScriptCounter");
        public Attribute MissionTypeFlags = new Attribute(1, "MissionTypeFlags");
        public Attribute IsHeadless = new Attribute(1, "IsHeadless");
        public Attribute IsDead = new Attribute(1, "IsDead");
        public Attribute AbandonedState = new Attribute(1, "AbandonedState");
        public Attribute CancelAbility = new Attribute(4, "CancelAbility");
        public Attribute JumpEndTime = new Attribute(4, "JumpEndTime");
        public Attribute RealFirstName = new Attribute(32, "RealFirstName");
        public Attribute TeleportCounter = new Attribute(1, "TeleportCounter");
        public Attribute MovementScale = new Attribute(4, "MovementScale");
        public Attribute EvadeShieldDamageScale = new Attribute(4, "EvadeShieldDamageScale");
        public Attribute EffectCommand = new Attribute(4, "EffectCommand");
        public Attribute EvadeShieldHealth = new Attribute(1, "EvadeShieldHealth");
        public Attribute SecureTradeFlag = new Attribute(4, "SecureTradeFlag");
        public Attribute InteractionFlags = new Attribute(4, "InteractionFlags");
        public Attribute StealthLevel = new Attribute(1, "StealthLevel");
        public Attribute RippleMagnitude = new Attribute(1, "RippleMagnitude");
        public Attribute HeavyLuggableID = new Attribute(4, "HeavyLuggableID");
        //public Attribute AFK = new Attribute(1, "AFK");
        public Attribute MissionTeamID = new Attribute(4, "MissionTeamID");
        public Attribute MaxHealth = new Attribute(2, "MaxHealth");
        public Attribute StealthAwareness = new Attribute(1, "StealthAwareness");
        public Attribute JumpPeakHeight = new Attribute(4, "JumpPeakHeight");
        public Attribute ConditionStateFlags = new Attribute(4, "ConditionStateFlags");
        public Attribute InnerStrengthCommitted = new Attribute(2, "InnerStrengthCommitted");
        public Attribute ReputationMachines = new Attribute(2, "ReputationMachines");
        public Attribute InnerStrengthAvailable = new Attribute(2, "InnerStrengthAvailable");
        public Attribute FactionID = new Attribute(4, "FactionID");
        public Attribute DeathPenalty = new Attribute(4, "DeathPenalty");
        public Attribute FollowingPath = new Attribute(1, "FollowingPath");
        public Attribute EffectCounter = new Attribute(1, "EffectCounter");
        public Attribute Description = new Attribute(4, "Description");
        public Attribute ReputationNiobe = new Attribute(2, "ReputationNiobe");
        public Attribute CharacterName = new Attribute(32, "CharacterName");
        public Attribute JumpDestination = new Attribute(24, "JumpDestination");
        public Attribute SelectionRangeDebuff = new Attribute(4, "SelectionRangeDebuff");
        public Attribute RepelDistance = new Attribute(4, "RepelDistance");
        public Attribute Health = new Attribute(2, "Health");
        public Attribute TitleAbility = new Attribute(4, "TitleAbility");
        public Attribute Demeanor = new Attribute(1, "Demeanor");
        public Attribute ReputationPluribusNeo = new Attribute(2, "ReputationPluribusNeo");
        public Attribute CharacterID = new Attribute(4, "CharacterID");
        public Attribute CrewID = new Attribute(4, "CrewID");
        public Attribute RSIDescription = new Attribute(15, "RSIDescription");
        public Attribute InnerStrengthMax = new Attribute(2, "InnerStrengthMax");
        public Attribute Position = new Attribute(24, "Position");
        public Attribute IsDuelDeath = new Attribute(1, "IsDuelDeath");
        public Attribute ReputationCypherites = new Attribute(2, "ReputationCypherites");
        public Attribute Level = new Attribute(1, "Level");
        public Attribute CombatantMode = new Attribute(1, "CombatantMode");
        public Attribute ReputationGMOrganization = new Attribute(2, "ReputationGMOrganization");
        public Attribute EffectID = new Attribute(4, "EffectID");
        public Attribute DuelID = new Attribute(4, "DuelID");
        //public Attribute HasLuggable = new Attribute(1, "HasLuggable");
        public Attribute Jumping = new Attribute(1, "Jumping");
        public Attribute MissionKey = new Attribute(4, "MissionKey");
        public Attribute DissemblingType = new Attribute(2, "DissemblingType");
        public Attribute JumpFlags = new Attribute(2, "JumpFlags");
        public Attribute CurExclusiveAbility = new Attribute(4, "CurExclusiveAbility");
        public Attribute ZoneInTime = new Attribute(4, "ZoneInTime");
        public Attribute NoiseLevel = new Attribute(1, "NoiseLevel");
        public Attribute HalfExtents = new Attribute(12, "HalfExtents");
        public Attribute YawInterval = new Attribute(1, "Yaw Interval");
        public Attribute RelevancyFlags = new Attribute(1, "RelevancyFlags");
        public Attribute Stance = new Attribute(1, "Stance");
        public Attribute UseRSIDescription = new Attribute(1, "UseRSIDescription");
        public Attribute EquippedItemID = new Attribute(4, "EquippedItemID");
        public Attribute Action = new Attribute(1, "Action");
        public Attribute OrganizationID = new Attribute(1, "OrganizationID");
        public Attribute CurCombatExclusiveAbility = new Attribute(4, "CurCombatExclusiveAbility");
        public Attribute ReputationZionMilitary = new Attribute(2, "ReputationZionMilitary");


        public Object12()
            : base(73, 36, 11, 6, "PlayerCharacter", 0x0c, 0xFFFFFFFF)
        {
            this.AddAttribute(ref ReputationMerovingian, 0, -1);
            this.AddAttribute(ref ConquestPoints, 1, -1);
            this.AddAttribute(ref CancelAbilityCounter, 2, 32);
            this.AddAttribute(ref RealLastName, 3, -1);
            this.AddAttribute(ref ScriptCounter, 4, -1);
            this.AddAttribute(ref MissionTypeFlags, 5, -1);
            this.AddAttribute(ref IsHeadless, 6, -1);
            this.AddAttribute(ref IsDead, 7, 15);
            this.AddAttribute(ref AbandonedState, 8, 34);
            this.AddAttribute(ref CancelAbility, 9, 33);
            this.AddAttribute(ref JumpEndTime, 10, 12);
            this.AddAttribute(ref RealFirstName, 11, -1);
            this.AddAttribute(ref TeleportCounter, 12, -1);
            this.AddAttribute(ref MovementScale, 13, 5);
            this.AddAttribute(ref EvadeShieldDamageScale, 14, -1);
            this.AddAttribute(ref EffectCommand, 15, 25);
            this.AddAttribute(ref EvadeShieldHealth, 16, 21);
            this.AddAttribute(ref SecureTradeFlag, 17, -1);
            this.AddAttribute(ref InteractionFlags, 18, 16);
            this.AddAttribute(ref StealthLevel, 19, -1);
            this.AddAttribute(ref RippleMagnitude, 20, -1);
            this.AddAttribute(ref HeavyLuggableID, 21, 17);
            //this.AddAttribute(ref AFK, 22, 35);
            this.AddAttribute(ref MissionTeamID, 22, 6);
            this.AddAttribute(ref MaxHealth, 23, 8);
            this.AddAttribute(ref StealthAwareness, 24, -1);
            this.AddAttribute(ref JumpPeakHeight, 25, 10);
            this.AddAttribute(ref ConditionStateFlags, 26, 30);
            this.AddAttribute(ref InnerStrengthCommitted, 27, -1);
            this.AddAttribute(ref ReputationMachines, 28, -1);
            this.AddAttribute(ref InnerStrengthAvailable, 29, -1);
            this.AddAttribute(ref FactionID, 30, 20);
            this.AddAttribute(ref DeathPenalty, 31, -1);
            this.AddAttribute(ref FollowingPath, 32, 19);
            this.AddAttribute(ref EffectCounter, 33, 24);
            this.AddAttribute(ref Description, 34, 28);
            this.AddAttribute(ref ReputationNiobe, 35, -1);
            this.AddAttribute(ref CharacterName, 36, -1);
            this.AddAttribute(ref JumpDestination, 37, 13);
            this.AddAttribute(ref SelectionRangeDebuff, 38, -1);
            this.AddAttribute(ref RepelDistance, 39, 2);
            this.AddAttribute(ref Health, 40, 18);
            this.AddAttribute(ref TitleAbility, 41, 1);
            this.AddAttribute(ref Demeanor, 42, -1);
            this.AddAttribute(ref ReputationPluribusNeo, 43, -1);
            this.AddAttribute(ref CharacterID, 44, -1);
            this.AddAttribute(ref CrewID, 45, 29);
            this.AddAttribute(ref RSIDescription, 46, 3);
            this.AddAttribute(ref InnerStrengthMax, 47, -1);
            this.AddAttribute(ref Position, 48, -1);
            this.AddAttribute(ref IsDuelDeath, 49, 14);
            this.AddAttribute(ref ReputationCypherites, 50, -1);
            this.AddAttribute(ref Level, 51, 9);
            this.AddAttribute(ref CombatantMode, 52, 31);
            this.AddAttribute(ref ReputationGMOrganization, 53, -1);
            this.AddAttribute(ref EffectID, 54, 23);
            this.AddAttribute(ref DuelID, 55, 26);
            //this.AddAttribute(ref HasLuggable, 57, -1);
            this.AddAttribute(ref Jumping, 56, -1);
            this.AddAttribute(ref MissionKey, 57, 7);
            this.AddAttribute(ref DissemblingType, 58, 27);
            this.AddAttribute(ref JumpFlags, 59, 11);
            this.AddAttribute(ref CurExclusiveAbility, 60, -1);
            this.AddAttribute(ref ZoneInTime, 61, -1);
            this.AddAttribute(ref NoiseLevel, 62, -1);
            this.AddAttribute(ref HalfExtents, 63, -1);
            this.AddAttribute(ref YawInterval, 64, -1);
            this.AddAttribute(ref RelevancyFlags, 65, -1);
            this.AddAttribute(ref Stance, 66, -1);
            this.AddAttribute(ref UseRSIDescription, 67, 0);
            this.AddAttribute(ref EquippedItemID, 68, 22);
            this.AddAttribute(ref Action, 69, -1);
            this.AddAttribute(ref OrganizationID, 70, 4);
            this.AddAttribute(ref CurCombatExclusiveAbility, 71, -1);
            this.AddAttribute(ref ReputationZionMilitary, 72, -1);

            setSelfViewAttributes();
        }


        private void setSelfViewAttributes()
        {

            attributesSelfView = new Attribute[48];
            selfViewGroups = 7;

            attributesSelfView[0] = UseRSIDescription;
            attributesSelfView[1] = TitleAbility;
            attributesSelfView[2] = SelectionRangeDebuff;
            attributesSelfView[3] = RippleMagnitude;
            attributesSelfView[4] = RepelDistance;
            attributesSelfView[5] = RealLastName;
            attributesSelfView[6] = RealFirstName;
            attributesSelfView[7] = RSIDescription;
            attributesSelfView[8] = OrganizationID;
            attributesSelfView[9] = NoiseLevel;
            attributesSelfView[10] = MovementScale;
            attributesSelfView[11] = MissionTypeFlags;
            attributesSelfView[12] = MissionTeamID;
            attributesSelfView[13] = MissionKey;
            attributesSelfView[14] = MaxHealth;
            attributesSelfView[15] = Level;
            attributesSelfView[16] = JumpPeakHeight;
            attributesSelfView[17] = JumpFlags;
            attributesSelfView[18] = JumpEndTime;
            attributesSelfView[19] = JumpDestination;
            attributesSelfView[20] = IsDuelDeath;
            attributesSelfView[21] = IsDead;
            attributesSelfView[22] = InteractionFlags;
            attributesSelfView[23] = InnerStrengthMax;
            attributesSelfView[24] = InnerStrengthCommitted;
            attributesSelfView[25] = InnerStrengthAvailable;
            attributesSelfView[26] = HeavyLuggableID;
            attributesSelfView[27] = Health;
            attributesSelfView[28] = FollowingPath;
            attributesSelfView[29] = FactionID;
            attributesSelfView[30] = EvadeShieldHealth;
            attributesSelfView[31] = EquippedItemID;
            attributesSelfView[32] = EffectID;
            attributesSelfView[33] = EffectCounter;
            attributesSelfView[34] = EffectCommand;
            attributesSelfView[35] = DuelID;
            attributesSelfView[36] = DissemblingType;
            attributesSelfView[37] = Description;
            attributesSelfView[38] = DeathPenalty;
            attributesSelfView[39] = CurCombatExclusiveAbility;
            attributesSelfView[40] = CrewID;
            attributesSelfView[41] = ConquestPoints;
            attributesSelfView[42] = ConditionStateFlags;
            attributesSelfView[43] = CombatantMode;
            attributesSelfView[44] = CancelAbilityCounter;
            attributesSelfView[45] = CancelAbility;
            attributesSelfView[46] = AbandonedState;
            //attributesSelfView[47] = AFK;
        }

    }
}
