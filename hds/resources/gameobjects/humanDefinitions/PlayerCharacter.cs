using System;
using hds.shared;

namespace hds
{
    public class PlayerCharacter : Object12
	{
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
		
		public int parseAttributes(ref byte[] buffer, int _offset)
		{
			_offset++;
			byte flag = 0x00;
			flag = BufferHandler.readByte(ref buffer,ref _offset);

            byte[] stateData = new byte[buffer.Length - _offset+2];
            ArrayUtils.copy(buffer, _offset-2, stateData, 0, buffer.Length - _offset+2);
			//Flag Bits{0,0,0,0,Vector3f Position Update,  Yaw Update,Animation Update, AttributesPacked Update?}

			double x = 0;
			double y = 0;
			double z = 0;
            
            switch (flag)
            {
                case 0x02:
                    Action.setValue(BufferHandler.readBytes(ref buffer, ref _offset, Action.getSize()));
                break;


                case 0x04:
                    YawInterval.setValue(BufferHandler.readBytes(ref buffer, ref _offset, YawInterval.getSize()));
                break;


                case 0x08:
                    x = (double)NumericalUtils.byteArrayToFloat(BufferHandler.readBytes(ref buffer, ref _offset, 4), 1);
                    y = (double)NumericalUtils.byteArrayToFloat(BufferHandler.readBytes(ref buffer, ref _offset, 4), 1);
                    z = (double)NumericalUtils.byteArrayToFloat(BufferHandler.readBytes(ref buffer, ref _offset, 4), 1);
                    Position.setValue(NumericalUtils.doublesToLtVector3d(x, y, z));
                break;
	                
	            case 0x0a:
		            YawInterval.setValue(BufferHandler.readByte(ref buffer, ref _offset));
		            x = (double)NumericalUtils.byteArrayToFloat(BufferHandler.readBytes(ref buffer, ref _offset, 4), 1);
		            y = (double)NumericalUtils.byteArrayToFloat(BufferHandler.readBytes(ref buffer, ref _offset, 4), 1);
		            z = (double)NumericalUtils.byteArrayToFloat(BufferHandler.readBytes(ref buffer, ref _offset, 4), 1);
		            Position.setValue(NumericalUtils.doublesToLtVector3d(x, y, z));
		         break;

                case 0x0e:
                    // UInt16 unknown + LtVector3f
                    UInt16 unknown = NumericalUtils.ByteArrayToUint16(BufferHandler.readBytes(ref buffer, ref _offset, 2), 1);
                    x = NumericalUtils.byteArrayToFloat(BufferHandler.readBytes(ref buffer, ref _offset, 4), 1);
                    y = NumericalUtils.byteArrayToFloat(BufferHandler.readBytes(ref buffer, ref _offset, 4), 1);
                    z = NumericalUtils.byteArrayToFloat(BufferHandler.readBytes(ref buffer, ref _offset, 4), 1);
                    Position.setValue(NumericalUtils.doublesToLtVector3d(x, y, z));
                break;
	            
	                default:
		                // ToDo: we need a proper way to proove if there is a 00 00 04 somewhere (and if set the offset to it)
		                
		                // If this doesnt match we need to write this somewhere...
		                string message = "RPCMAIN : Unknown Client 03 Request Packet \r\n" +
		                                 "Flag: " + flag + "\r\n " +
		                                 "Content: \n" +
		                                 StringUtils.bytesToString(stateData) + "\r\n";
		                Output.WriteClientViewRequestLog(message);
		                
		                // ...and we dont want to crash so we just say "offset is full packet"
		                _offset = buffer.Length - 1;
		                break;
            }
			
			// TODO: update player attribute packets someday (announce it to my spawners)
            Store.world.sendViewPacketToAllPlayers(stateData, Store.currentClient.playerData.getCharID(), NumericalUtils.ByteArrayToUint16(Store.currentClient.playerInstance.GetGoid(), 1), Store.currentClient.playerData.getEntityId());
			return _offset;
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
		
		
		public int parseAutoView(ref byte[] buffer, int _offset)
		{

			_offset = _offset +2;
			byte subview = buffer[_offset];
						
			switch(subview){
				case 0x01:
					#if DEBUG
					Output.WriteLine("[MPM] Parsing selfview Attributes");
					#endif
					//Self view Attributes only
					_offset = parseAttributes(ref buffer,_offset);
				break;
				
				case 0x02:
					Output.WriteLine("[MPM] Parsing 02 View");
					_offset = parseAttributes2(ref buffer,_offset);
				break;
				
				default:
					//TODO: put something here
					#if DEBUG
					Output.WriteLine("[MPM] Nothing to Parse autoview so no offset");
					#endif
					break;
			}
			
			return _offset;
		}
		
	}
}
