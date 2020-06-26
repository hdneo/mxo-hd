using System;
using System.Runtime.InteropServices;

namespace hds
{
	public class AttributeClass3827 :GameObject
	{
		public Attribute EvadeShieldHealth = new Attribute(1, "EvadeShieldHealth"); 
		public Attribute CharacterName = new Attribute(32, "CharacterName"); 
		public Attribute TitleAbility = new Attribute(4, "TitleAbility"); 
		public Attribute CombatantMode = new Attribute(1, "CombatantMode"); 
		public Attribute JumpFlags = new Attribute(2, "JumpFlags"); 
		public Attribute ConditionStateFlags = new Attribute(4, "ConditionStateFlags"); 
		public Attribute Jumping = new Attribute(1, "Jumping"); 
		public Attribute EffectCounter = new Attribute(1, "EffectCounter"); 
		public Attribute UseRSIDescription = new Attribute(1, "UseRSIDescription"); 
		public Attribute YawInterval = new Attribute(1, "YawInterval"); 
		public Attribute MasterCharacterID = new Attribute(4, "MasterCharacterID"); 
		public Attribute MoreInfoID = new Attribute(4, "MoreInfoID"); 
		public Attribute StealthLevel = new Attribute(1, "StealthLevel"); 
		public Attribute StopFollowActiveTracker = new Attribute(1, "StopFollowActiveTracker"); 
		public Attribute IsDead = new Attribute(1, "IsDead"); 
		public Attribute DissemblingType = new Attribute(2, "DissemblingType"); 
		public Attribute JumpEndTime = new Attribute(4, "JumpEndTime"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute WanderRange = new Attribute(4, "WanderRange"); 
		public Attribute ScriptCounter = new Attribute(1, "ScriptCounter"); 
		public Attribute EvadeShieldDamageScale = new Attribute(4, "EvadeShieldDamageScale"); 
		public Attribute Description = new Attribute(4, "Description"); 
		public Attribute EffectID = new Attribute(4, "EffectID"); 
		public Attribute FactionID = new Attribute(4, "FactionID"); 
		public Attribute DontAttack = new Attribute(1, "DontAttack"); 
		public Attribute NoiseLevel = new Attribute(1, "NoiseLevel"); 
		public Attribute ExclusiveLocator = new Attribute(18, "ExclusiveLocator"); 
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute ProneState = new Attribute(4, "ProneState"); 
		public Attribute CancelAbility = new Attribute(4, "CancelAbility"); 
		public Attribute UseCount = new Attribute(1, "UseCount"); 
		public Attribute MissionKey = new Attribute(4, "MissionKey"); 
		public Attribute Stance = new Attribute(1, "Stance"); 
		public Attribute OrganizationID = new Attribute(1, "OrganizationID"); 
		public Attribute IsEnemy = new Attribute(1, "IsEnemy"); 
		public Attribute Action = new Attribute(1, "Action"); 
		public Attribute NPCRank = new Attribute(1, "NPCRank"); 
		public Attribute IsFriendly = new Attribute(1, "IsFriendly"); 
		public Attribute Demeanor = new Attribute(1, "Demeanor"); 
		public Attribute MovementScale = new Attribute(4, "MovementScale"); 
		public Attribute CancelAbilityCounter = new Attribute(1, "CancelAbilityCounter"); 
		public Attribute JumpPeakHeight = new Attribute(4, "JumpPeakHeight"); 
		public Attribute FollowActiveTracker = new Attribute(1, "FollowActiveTracker"); 
		public Attribute GuardType = new Attribute(1, "GuardType"); 
		public Attribute ExclusiveType = new Attribute(1, "ExclusiveType"); 
		public Attribute Level = new Attribute(1, "Level"); 
		public Attribute GiverActiveTracker = new Attribute(1, "GiverActiveTracker"); 
		public Attribute Health = new Attribute(2, "Health"); 
		public Attribute EffectCommand = new Attribute(4, "EffectCommand"); 
		public Attribute JumpDestination = new Attribute(24, "JumpDestination"); 
		public Attribute DebuffState = new Attribute(1, "DebuffState"); 
		public Attribute TakerActiveTracker = new Attribute(1, "TakerActiveTracker"); 
		public Attribute RSIDescription = new Attribute(15, "RSIDescription"); 
		public Attribute MaxHealth = new Attribute(2, "MaxHealth"); 
		public Attribute CurrentState = new Attribute(4, "CurrentState"); 
		public Attribute IsEscortable = new Attribute(1, "IsEscortable"); 
		public Attribute EquippedItemID = new Attribute(4, "EquippedItemID"); 
		public Attribute SpawnFX = new Attribute(4, "SpawnFX"); 
		public Attribute DuelID = new Attribute(4, "DuelID"); 
		public Attribute TalkDefaultable = new Attribute(1, "TalkDefaultable"); 
		public Attribute OwnerCharacterID = new Attribute(4, "OwnerCharacterID"); 
		public Attribute PutActiveTracker = new Attribute(1, "PutActiveTracker"); 
		public Attribute RelevancyFlags = new Attribute(1, "RelevancyFlags"); 
		public Attribute TalkActiveTracker = new Attribute(1, "TalkActiveTracker"); 
		public Attribute StealthAwareness = new Attribute(1, "StealthAwareness"); 
		public Attribute CurrentStateContainer = new Attribute(4, "CurrentStateContainer"); 


		 public AttributeClass3827(string name,UInt16 _goid)
		: base(66, 47, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref EvadeShieldHealth, 0, 32); 
			AddAttribute(ref CharacterName, 1, -1); 
			AddAttribute(ref TitleAbility, 2, 2); 
			AddAttribute(ref CombatantMode, 3, 44); 
			AddAttribute(ref JumpFlags, 4, 20); 
			AddAttribute(ref ConditionStateFlags, 5, 43); 
			AddAttribute(ref Jumping, 6, -1); 
			AddAttribute(ref EffectCounter, 7, 35); 
			AddAttribute(ref UseRSIDescription, 8, 0); 
			AddAttribute(ref YawInterval, 9, -1); 
			AddAttribute(ref MasterCharacterID, 10, 17); 
			AddAttribute(ref MoreInfoID, 11, -1); 
			AddAttribute(ref StealthLevel, 12, -1); 
			AddAttribute(ref StopFollowActiveTracker, 13, 6); 
			AddAttribute(ref IsDead, 14, 26); 
			AddAttribute(ref DissemblingType, 15, 39); 
			AddAttribute(ref JumpEndTime, 16, 21); 
			AddAttribute(ref HalfExtents, 17, -1); 
			AddAttribute(ref WanderRange, 18, -1); 
			AddAttribute(ref ScriptCounter, 19, -1); 
			AddAttribute(ref EvadeShieldDamageScale, 20, -1); 
			AddAttribute(ref Description, 21, 40); 
			AddAttribute(ref EffectID, 22, 34); 
			AddAttribute(ref FactionID, 23, 31); 
			AddAttribute(ref DontAttack, 24, 38); 
			AddAttribute(ref NoiseLevel, 25, -1); 
			AddAttribute(ref ExclusiveLocator, 26, -1); 
			AddAttribute(ref Position, 27, -1); 
			AddAttribute(ref ProneState, 28, 11); 
			AddAttribute(ref CancelAbility, 29, 46); 
			AddAttribute(ref UseCount, 30, 1); 
			AddAttribute(ref MissionKey, 31, 15); 
			AddAttribute(ref Stance, 32, -1); 
			AddAttribute(ref OrganizationID, 33, 12); 
			AddAttribute(ref IsEnemy, 34, 25); 
			AddAttribute(ref Action, 35, -1); 
			AddAttribute(ref NPCRank, 36, 13); 
			AddAttribute(ref IsFriendly, 37, 23); 
			AddAttribute(ref Demeanor, 38, -1); 
			AddAttribute(ref MovementScale, 39, 14); 
			AddAttribute(ref CancelAbilityCounter, 40, 45); 
			AddAttribute(ref JumpPeakHeight, 41, 19); 
			AddAttribute(ref FollowActiveTracker, 42, 30); 
			AddAttribute(ref GuardType, 43, 28); 
			AddAttribute(ref ExclusiveType, 44, -1); 
			AddAttribute(ref Level, 45, 18); 
			AddAttribute(ref GiverActiveTracker, 46, 29); 
			AddAttribute(ref Health, 47, 27); 
			AddAttribute(ref EffectCommand, 48, 36); 
			AddAttribute(ref JumpDestination, 49, 22); 
			AddAttribute(ref DebuffState, 50, -1); 
			AddAttribute(ref TakerActiveTracker, 51, 5); 
			AddAttribute(ref RSIDescription, 52, 9); 
			AddAttribute(ref MaxHealth, 53, 16); 
			AddAttribute(ref CurrentState, 54, 42); 
			AddAttribute(ref IsEscortable, 55, 24); 
			AddAttribute(ref EquippedItemID, 56, 33); 
			AddAttribute(ref SpawnFX, 57, 7); 
			AddAttribute(ref DuelID, 58, 37); 
			AddAttribute(ref TalkDefaultable, 59, 3); 
			AddAttribute(ref OwnerCharacterID, 60, -1); 
			AddAttribute(ref PutActiveTracker, 61, 10); 
			AddAttribute(ref RelevancyFlags, 62, 8); 
			AddAttribute(ref TalkActiveTracker, 63, 4); 
			AddAttribute(ref StealthAwareness, 64, -1); 
			AddAttribute(ref CurrentStateContainer, 65, 41); 

		}

	}

}