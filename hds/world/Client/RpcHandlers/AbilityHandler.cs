using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using hds.shared;

namespace hds
{
    public class AbilityHandler{


        public void processAbility(ref byte[] packet)
        {
            byte[] ability = {packet[0], packet[1]};
            UInt16 AbilityID = NumericalUtils.ByteArrayToUint16(ability, 1);
            string AbilityName;

            // load the ability name from a list to see if we match the right ability
            DataLoader AbilityLoader = DataLoader.getInstance();
            AbilityName = AbilityLoader.getAbilityNameByID(AbilityID);
            //Output.WriteLine("Ability ID is " + AbilityID.ToString() + " and the name is " + AbilityName);

            // lets create a message for the client - we will later execute the right AbilityScript for it 

            Store.currentClient.messageQueue.addRpcMessage(PacketsUtils.createMessage("Ability ID is " + AbilityID.ToString() + " and the name is " + AbilityName, "BROADCAST", Store.currentClient));
            this.processBarCreation(AbilityID);
            this.processAbilityScript(AbilityID);

            
        }

        public void processAbilityScript(UInt16 abilityID)
        {
            // Lets just test
            //this.processBarCreation(abilityID);
            this.processSelfAnimation(abilityID);
            this.processCharacterAnimationSelf(abilityID);
        }

        public void processCharacterAnimationSelf(UInt16 abilityID)
        {
            ServerPackets pak = new ServerPackets();
            pak.sendPlayerAnimation(Store.currentClient, "500b");
            // send 02 03 02 00 01 02 23 00 00;
            /*
            byte[] viewID = { 0x02, 0x00 };
            byte[] animId = { 0x31 };  // see movementAnims.tx - its for codes something (0x31)

            // packet gen
            DynamicArray din2 = new DynamicArray();
            din2.append(viewID);
            din2.append(0x01);
            din2.append(0x02);
            din2.append(animId);
            din2.append(0x00);
            Store.currentClient.messageQueue.addObjectMessage(din2.getBytes(),false);
             * */
        }

        public void processBarCreation(UInt16 abilityID)
        {
            byte[] barResponseHeader = {0x80, 0xa7};
            byte[] barResponseTimer  = {0x00, 0x40};

            DynamicArray din = new DynamicArray();
            din.append(barResponseHeader);
            din.append(NumericalUtils.uint16ToByteArray(abilityID, 1));
            din.append(StringUtils.hexStringToBytes("000000000000000000000000")); // Execution Time Bar 
            din.append(barResponseTimer); // Ticks ? Or Time how long bar should execute
            
            Store.currentClient.messageQueue.addRpcMessage(din.getBytes());
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
            UInt32 randAnimID = (UInt32)FXList.FX_VIRUSCAST_SPLIT_GROUP_REPAIRS_GROUPREPAIRS5_START;
            
            byte[] animationId = NumericalUtils.uint32ToByteArray(randAnimID, 0);
            byte[] viewID = { 0x02, 0x00 };
            Random rand = new Random();
            ushort updateViewCounter = (ushort)rand.Next(3, 200);
            byte[] updateCount = NumericalUtils.uint16ToByteArrayShort(updateViewCounter);

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
