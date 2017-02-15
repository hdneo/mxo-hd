using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Threading;

using hds.shared;

namespace hds
{
    public class AbilityHandler{

        public AbilityItem currentAbility;
        public UInt16 currentTargetViewId;
        public Timer damageTimer;
        public void processAbility(ref byte[] packet)
        {
            byte[] ability = {packet[0], packet[1]};
            byte[] targetView = { packet[2], packet[3] };
            UInt16 AbilityID = NumericalUtils.ByteArrayToUint16(ability, 1);

            UInt16 viewId = 0;
            currentTargetViewId = NumericalUtils.ByteArrayToUint16(targetView, 1);


            // load the ability name from a list to see if we match the right ability
            DataLoader AbilityLoader = DataLoader.getInstance();
            this.currentAbility = AbilityLoader.getAbilityByID(AbilityID);

            // lets create a message for the client - we will later execute the right AbilityScript for it 
            ServerPackets pak = new ServerPackets();
            pak.sendSystemChatMessage(Store.currentClient, "Ability ID is " + AbilityID.ToString() + " and the name is " + currentAbility.getAbilityName() + " and Target ViewId Is " + currentTargetViewId, "BROADCAST");

            // ToDo: do something with the entity (or queue a fx hit animation or something lol)    
            this.processAbilityScript(this.currentAbility);

            
        }

        public void processAbilityScript(AbilityItem abilityItem)
        {

            // Lets just test
            ServerPackets pak = new ServerPackets();
            if (this.currentAbility.getCastingTime()>0)
            {
                pak.sendCastAbilityBar(this.currentAbility.getAbilityID(), this.currentAbility.getCastingTime());
                this.processSelfAnimation(abilityItem.getAbilityID());
            }

            this.processCharacterAnimationSelf(abilityItem.getAbilityID());

            if (currentAbility.getAbilityID() == 12 || currentAbility.getAbilityID() == 184)
            {
                pak.sendHyperSpeed();
            }
            // Test Is
            pak.sendISCurrent(Store.currentClient, 50);
        }

        public void abilityAnimateTheTarget(object e)
        {
            ServerPackets pak = new ServerPackets();
            UInt32 targetAnim = this.currentAbility.getActivationFX();
            if (targetAnim == 0)
            {
                targetAnim = this.currentAbility.getAbilityExecutionFX();
            }
            pak.sendCastAbilityOnEntityId(currentTargetViewId, targetAnim);
        }

        public void processCharacterAnimationSelf(UInt16 abilityID)
        {
            ServerPackets pak = new ServerPackets();
            // 2904 0429 = Hacker_VirusLaunch_A
            // 2a04 042a = Hacker_VirusLaunch_D
            // see movementAnims.tx - its for codes something (0x31)
            if (currentAbility.getAbilityExecutionFX() > 0)
            {
                pak.sendCastAbilityOnEntityId(2, currentAbility.getAbilityExecutionFX());
            }
            if (currentAbility.getCastingTime() > 0)
            {
                // Cast 
                pak.sendPlayerAnimation(Store.currentClient, "2904");

                // And Time a "Damage" or "Buff" Animation
                int castingTime = (int)this.currentAbility.getCastingTime() * 1000;
                this.damageTimer = new Timer(new TimerCallback(abilityAnimateTheTarget), this, castingTime, 0);
            }
            
        }


        public void processSelfAnimation(UInt16 abilityID)
        {
            DynamicArray din = new DynamicArray();
            //byte[] animationId = {0x22, 0x0a, 0x00, 0x28}; // Repair RSI
            //byte[] animationId = { 0x39, 0x0a, 0x00, 0x28 }; // Jackout
            
            
            /*
            Array fxIDs = Enum.GetValues(typeof(FXList));
            Random randAnim = new Random();
            UInt32 randAnimID = (UInt32)fxIDs.GetValue(randAnim.Next(fxIDs.Length));
            */
            UInt32 randAnimID = (UInt32)FXList.FX_VIRUSCAST_SPLIT_CODE_NUKE_CODENUKE_START;
            
            byte[] animationId = NumericalUtils.uint32ToByteArray(randAnimID, 0);
            byte[] viewID = { 0x02, 0x00 };

            byte[] updateCount = NumericalUtils.uint16ToByteArrayShort(Store.currentClient.playerData.assignSpawnIdCounter());

            din.append(viewID);
            din.append(0x02);
            din.append(0x80);
            din.append(0x80);
            din.append(0x80);
            din.append(0x90);
            din.append(0xed);
            din.append(0x00);
            din.append(0x30);
            din.append(animationId);
            din.append(updateCount);

            Store.currentClient.messageQueue.addObjectMessage(din.getBytes(),false);

            
        }

        
    }
}
