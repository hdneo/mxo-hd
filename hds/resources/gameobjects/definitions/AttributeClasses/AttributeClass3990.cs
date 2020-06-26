using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass3990 :GameObject
	{
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
		public Attribute AFK = new Attribute(1, "AFK"); 
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
		public Attribute HasLuggable = new Attribute(1, "HasLuggable"); 
		public Attribute Jumping = new Attribute(1, "Jumping"); 
		public Attribute MissionKey = new Attribute(4, "MissionKey"); 
		public Attribute DissemblingType = new Attribute(2, "DissemblingType"); 
		public Attribute JumpFlags = new Attribute(2, "JumpFlags"); 
		public Attribute CurExclusiveAbility = new Attribute(4, "CurExclusiveAbility"); 
		public Attribute ZoneInTime = new Attribute(4, "ZoneInTime"); 
		public Attribute NoiseLevel = new Attribute(1, "NoiseLevel"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute YawInterval = new Attribute(1, "YawInterval"); 
		public Attribute RelevancyFlags = new Attribute(1, "RelevancyFlags"); 
		public Attribute Stance = new Attribute(1, "Stance"); 
		public Attribute UseRSIDescription = new Attribute(1, "UseRSIDescription"); 
		public Attribute EquippedItemID = new Attribute(4, "EquippedItemID"); 
		public Attribute Action = new Attribute(1, "Action"); 
		public Attribute OrganizationID = new Attribute(1, "OrganizationID"); 
		public Attribute CurCombatExclusiveAbility = new Attribute(4, "CurCombatExclusiveAbility"); 
		public Attribute ReputationZionMilitary = new Attribute(2, "ReputationZionMilitary"); 


		 public AttributeClass3990(string name,UInt16 _goid)
		: base(75, 36, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref ReputationMerovingian, 0, -1); 
			AddAttribute(ref ConquestPoints, 1, -1); 
			AddAttribute(ref CancelAbilityCounter, 2, 32); 
			AddAttribute(ref RealLastName, 3, -1); 
			AddAttribute(ref ScriptCounter, 4, -1); 
			AddAttribute(ref MissionTypeFlags, 5, -1); 
			AddAttribute(ref IsHeadless, 6, -1); 
			AddAttribute(ref IsDead, 7, 15); 
			AddAttribute(ref AbandonedState, 8, 34); 
			AddAttribute(ref CancelAbility, 9, 33); 
			AddAttribute(ref JumpEndTime, 10, 12); 
			AddAttribute(ref RealFirstName, 11, -1); 
			AddAttribute(ref TeleportCounter, 12, -1); 
			AddAttribute(ref MovementScale, 13, 5); 
			AddAttribute(ref EvadeShieldDamageScale, 14, -1); 
			AddAttribute(ref EffectCommand, 15, 25); 
			AddAttribute(ref EvadeShieldHealth, 16, 21); 
			AddAttribute(ref SecureTradeFlag, 17, -1); 
			AddAttribute(ref InteractionFlags, 18, 16); 
			AddAttribute(ref StealthLevel, 19, -1); 
			AddAttribute(ref RippleMagnitude, 20, -1); 
			AddAttribute(ref HeavyLuggableID, 21, 17); 
			AddAttribute(ref AFK, 22, 35); 
			AddAttribute(ref MissionTeamID, 23, 6); 
			AddAttribute(ref MaxHealth, 24, 8); 
			AddAttribute(ref StealthAwareness, 25, -1); 
			AddAttribute(ref JumpPeakHeight, 26, 10); 
			AddAttribute(ref ConditionStateFlags, 27, 30); 
			AddAttribute(ref InnerStrengthCommitted, 28, -1); 
			AddAttribute(ref ReputationMachines, 29, -1); 
			AddAttribute(ref InnerStrengthAvailable, 30, -1); 
			AddAttribute(ref FactionID, 31, 20); 
			AddAttribute(ref DeathPenalty, 32, -1); 
			AddAttribute(ref FollowingPath, 33, 19); 
			AddAttribute(ref EffectCounter, 34, 24); 
			AddAttribute(ref Description, 35, 28); 
			AddAttribute(ref ReputationNiobe, 36, -1); 
			AddAttribute(ref CharacterName, 37, -1); 
			AddAttribute(ref JumpDestination, 38, 13); 
			AddAttribute(ref SelectionRangeDebuff, 39, -1); 
			AddAttribute(ref RepelDistance, 40, 2); 
			AddAttribute(ref Health, 41, 18); 
			AddAttribute(ref TitleAbility, 42, 1); 
			AddAttribute(ref Demeanor, 43, -1); 
			AddAttribute(ref ReputationPluribusNeo, 44, -1); 
			AddAttribute(ref CharacterID, 45, -1); 
			AddAttribute(ref CrewID, 46, 29); 
			AddAttribute(ref RSIDescription, 47, 3); 
			AddAttribute(ref InnerStrengthMax, 48, -1); 
			AddAttribute(ref Position, 49, -1); 
			AddAttribute(ref IsDuelDeath, 50, 14); 
			AddAttribute(ref ReputationCypherites, 51, -1); 
			AddAttribute(ref Level, 52, 9); 
			AddAttribute(ref CombatantMode, 53, 31); 
			AddAttribute(ref ReputationGMOrganization, 54, -1); 
			AddAttribute(ref EffectID, 55, 23); 
			AddAttribute(ref DuelID, 56, 26); 
			AddAttribute(ref HasLuggable, 57, -1); 
			AddAttribute(ref Jumping, 58, -1); 
			AddAttribute(ref MissionKey, 59, 7); 
			AddAttribute(ref DissemblingType, 60, 27); 
			AddAttribute(ref JumpFlags, 61, 11); 
			AddAttribute(ref CurExclusiveAbility, 62, -1); 
			AddAttribute(ref ZoneInTime, 63, -1); 
			AddAttribute(ref NoiseLevel, 64, -1); 
			AddAttribute(ref HalfExtents, 65, -1); 
			AddAttribute(ref YawInterval, 66, -1); 
			AddAttribute(ref RelevancyFlags, 67, -1); 
			AddAttribute(ref Stance, 68, -1); 
			AddAttribute(ref UseRSIDescription, 69, 0); 
			AddAttribute(ref EquippedItemID, 70, 22); 
			AddAttribute(ref Action, 71, -1); 
			AddAttribute(ref OrganizationID, 72, 4); 
			AddAttribute(ref CurCombatExclusiveAbility, 73, -1); 
			AddAttribute(ref ReputationZionMilitary, 74, -1); 

		}

	}

}