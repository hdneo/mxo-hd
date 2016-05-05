using System;
using hds.shared;

namespace hds
{
    public class PlayerCharacter : Object12
	{
		
		public PlayerCharacter (){
						
		}
		
		
		private int parseAttributesPack(ref Attribute[] attList,int maxGroups,ref byte[] data,int offset){
			
			for(int i = 0;i<maxGroups;i++){
				byte groupIndicator = BufferHandler.readByte(ref data,ref offset);
				int basePos = i*7;
				//Output.WriteLine("Parsing group:"+i);
				
				//Output.WriteLine("Group indicator:"+(int)groupIndicator);
				
				for (int j = 0;j<7;j++){
					if ((groupIndicator & (1<<j))==1){ // Attribute is being updated
						//Output.WriteLine("Updating: "+attList[basePos+j].getName());
					}else{
						//Output.WriteLine("Not Updating: "+attList[basePos+j].getName());
					}
				}
				
				if (groupIndicator<0x80){ // More groups flag is OFF
					break;
				}
				if (groupIndicator==0x00){
					break;
				}
			}
			
			return offset;
		}
		
		public int parseAttributes(ref byte[] buffer, int _offset){
			int offset = _offset;
			byte flag = 0x00;
			flag = BufferHandler.readByte(ref buffer,ref offset);

            byte[] stateData = new byte[buffer.Length - offset+2];
            ArrayUtils.copy(buffer, offset-2, stateData, 0, buffer.Length - offset+2);
			//Flag Bits{0,0,0,0,Vector3f Position Update,  Yaw Update,Animation Update, AttributesPacked Update?}

            
            switch (flag)
            {
                case 0x02:
                    Action.setValue(BufferHandler.readBytes(ref buffer, ref offset, Action.getSize()));
                break;


                case 0x04:
                    YawInterval.setValue(BufferHandler.readBytes(ref buffer, ref offset, YawInterval.getSize()));
                break;


                case 0x08:
                    double x = (double)NumericalUtils.byteArrayToFloat(BufferHandler.readBytes(ref buffer, ref offset, 4), 1);
                    double y = (double)NumericalUtils.byteArrayToFloat(BufferHandler.readBytes(ref buffer, ref offset, 4), 1);
                    double z = (double)NumericalUtils.byteArrayToFloat(BufferHandler.readBytes(ref buffer, ref offset, 4), 1);
                    Position.setValue(NumericalUtils.doublesToLtVector3d(x, y, z));
                break;

                case 0x0e:
                    // UInt16 unknown + LtVector3f
                    UInt16 unknown = NumericalUtils.ByteArrayToUint16(BufferHandler.readBytes(ref buffer, ref offset, 2), 1);
                    float xPos = NumericalUtils.byteArrayToFloat(BufferHandler.readBytes(ref buffer, ref offset, 4), 1);
                    float yPos = NumericalUtils.byteArrayToFloat(BufferHandler.readBytes(ref buffer, ref offset, 4), 1);
                    float zPos = NumericalUtils.byteArrayToFloat(BufferHandler.readBytes(ref buffer, ref offset, 4), 1);
                    Position.setValue(NumericalUtils.doublesToLtVector3d((double)xPos, (double)yPos, (double)zPos));
                break;
            }
			
			// TODO: update player attribute packets someday (announce it to my spawners)
            Store.world.sendViewPacketToAllPlayers(stateData, Store.currentClient.playerData.getCharID(), NumericalUtils.ByteArrayToUint16(Store.currentClient.playerInstance.getGoid(), 1), Store.currentClient.playerData.getEntityId());

			
			return offset;
		}
		
		public int parseAttributes2(ref byte[] buffer, int _offset){
			int offset = _offset;
			byte flag = 0x00;
			flag = BufferHandler.readByte(ref buffer,ref offset);			
						
			if ((flag & 0x60)!=0){
				byte lastnameLength = BufferHandler.readByte(ref buffer,ref offset);
				RealLastName.setValue(BufferHandler.readBytes(ref buffer,ref offset,lastnameLength));
				byte firstnameLength = BufferHandler.readByte(ref buffer,ref offset);
				RealFirstName.setValue(BufferHandler.readBytes(ref buffer,ref offset,firstnameLength));
                //Store.currentClient.messageQueue.addObjectMessage(StringUtils.hexStringToBytes("0200028080808080101100"));
			}
			
			return offset;
		}
		
		
		public int parseAutoView(ref byte[] buffer, int _offset){
			int offset = _offset;
			
			byte subview =  BufferHandler.readByte(ref buffer,ref offset);
						
			switch(subview){
				case 0x01:
					//Self view Attributes only
					offset = parseAttributes(ref buffer,offset);
				break;
				
				case 0x02:
					offset = parseAttributes2(ref buffer,offset);
				break;
				
				
				
				default:
					//TODO: put something here
					break;
			}
			
			return offset;
		}
		
	}
}
