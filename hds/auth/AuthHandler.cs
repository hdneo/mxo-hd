using System;
using System.Linq;
using HDS.Network;
using hds.shared;

namespace hds.auth
{
    public class AuthHandler
    {
        private MxoRSA rsa;
        private MxoTwofish tf;
        private Md5 md5;

        int status;

        byte[] response;

        byte[] challenge;
        byte[] blankIV;
        byte[] md5edChallenge;
        
        WorldList wl;

        public AuthClientSession authClientSession;
        public AuthHandler(AuthClientSession authClientSession)
        {
            this.authClientSession = authClientSession;
            status = 0;
            rsa = new MxoRSA();
            tf = new MxoTwofish();
            md5 = new Md5();

            wl = new WorldList();

            blankIV = new byte[]
                {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
        }

        public int getStatus()
        {
            return status;
        }

        private int getOpCode(byte[] data)
        {
            int opCode = -1;
            if ((int) data[0] < 0x80)
                opCode = (int) data[1];
            else
                opCode = (int) data[2];

            return opCode;
        }

        public void ProcessPacket(PacketReader reader, long receivedBytes)
        {
            byte[] opcodeData = reader.ReadBytes(3);
            status = getOpCode(opcodeData);
            switch (status)
            {
                case (byte) AuthOpcode.AS_GetPublicKeyRequest:
                    ProcessGetPublicKey_Request(reader);
                    break;

                case (byte) AuthOpcode.AS_AuthRequest:
                    ProcessHandleAuth_Request(reader);
                    break;

                case (byte) AuthOpcode.AS_AuthChallengeResponse:
                    ProcessHandleAuthChallenge_Response(reader);
                    break;

                default:
#if DEBUG
                    Output.WriteLine("Received: " + StringUtils.bytesToString(reader.ReturnFullData()));
#endif
                    throw new AuthException("OPcode not developped");
            }
        }

        public void ProcessGetPublicKey_Request(PacketReader reader)
        {
            PacketContent responsePacket = new PacketContent();
            responsePacket.AddByte(0x12);
            responsePacket.AddByte((byte) AuthOpcode.AS_GetPublicKeyReply);
            responsePacket.AddUint32(0, 1);
            responsePacket.AddByteArray(TimeUtils.getUnixTime()); // Maybe reversed
            // implicit RSA version = 0x00000004
            responsePacket.AddUint32(4, 1);
            responsePacket.AddUint32(0, 0);
            responsePacket.AddByte(0x00);
            
            authClientSession.SendAsync(responsePacket.ReturnFinalPacket());
        }


        public void ProcessHandleAuth_Request(PacketReader reader)
        {
            // RSA version check
            byte[] neededRSAV = {0x04, 0x00, 0x00, 0x00};
            byte[] packetRSAV = new byte[4];

            // In this packet, RSA starts at offset 3
            reader.SetOffsetOverrideValue(3);
            packetRSAV = reader.ReadBytes(4);
            //ArrayUtils.copy(data, 3, packetRSAV, 0, 4);

            if (!ArrayUtils.equal(neededRSAV, packetRSAV))
            {
                throw new AuthException("Invalid RSA version");
            }

            // Get RSA encrypted blob from the data
            byte[] blob = new byte[128];

            reader.SetOffsetOverrideValue(44);
            blob = reader.ReadBytes(128);
            // ArrayUtils.copy(data, 44, blob, 0, 128);
            Output.OptWriteLine("-> Encrypted blob received.");

            // Get RSA decrypted blob
            byte[] decryptedBlob = rsa.decryptWithPrivkey(blob);
            Output.OptWriteLine("-> Blob decoded.");


            // Copy the Auth TF key from decoded blob.
            byte[] tfKey = new byte[16];
            ArrayUtils.copy(decryptedBlob, 7, tfKey, 0, 16);

            tf.SetIV(blankIV);
            tf.SetKey(tfKey);

            // Create the challenge
            challenge = new byte[]
                {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};

            byte[] criptedChallenge = new byte[16];
            tf.Encrypt(challenge, criptedChallenge);

            md5edChallenge = md5.digest(challenge); // Rajko said this

            Output.WriteLine(" --> Client TF key: " + StringUtils.bytesToString(tfKey));
            Output.WriteLine(" --> MD5(challenge): " + StringUtils.bytesToString(md5edChallenge));
            Output.WriteLine(" --> Twofished(challenge): " + StringUtils.bytesToString(criptedChallenge));

            // Copy the encrypted thing to the global variable "Challenge"
            ArrayUtils.copy(criptedChallenge, 0, challenge, 0, 16);

            int nameLength = decryptedBlob.Length - 30;
            byte[] nameB = new byte[nameLength];
            ArrayUtils.copy(decryptedBlob, 29, nameB, 0, nameLength);

            // Set WorldList username
            wl.setUsername(StringUtils.charBytesToString(nameB));

            Output.OptWriteLine("-> User login: " + wl.getUsername());

            PacketContent packetContent = new PacketContent();
            packetContent.AddByte(0x11);
            packetContent.AddByte((byte) AuthOpcode.AS_AuthChallenge);
            packetContent.AddByteArray(criptedChallenge);

            Output.WriteLine("Auth Request Response: " + StringUtils.bytesToString(packetContent.ReturnFinalPacket()));
            authClientSession.SendAsync(packetContent.ReturnFinalPacket());
        }

        public void ProcessHandleAuthChallenge_Response(PacketReader reader)
        {
            reader.SetOffsetOverrideValue(4);
            int blobSize = (int) reader.ReadUint8();

            byte[] encryptedBlob = new byte[blobSize];
            byte[] decryptedBlob = new byte[blobSize];

            reader.SetOffsetOverrideValue(6);
            encryptedBlob = reader.ReadBytes(blobSize);
            // ArrayUtils.copy(data, 6, encryptedBlob, 0, blobSize);

            // Reset IV to 0
            tf.SetIV(blankIV);
            tf.Decrypt(encryptedBlob, decryptedBlob);

            byte[] receivedMD5 = new byte[16];
            ArrayUtils.copy(decryptedBlob, 1, receivedMD5, 0, 16);
            
            // Security says that must be the same
            Output.WriteLine("Decrypted (TF) blob:" + StringUtils.bytesToString(decryptedBlob));
            if (!ArrayUtils.equal(receivedMD5, md5edChallenge))
            {
                Output.WriteLine("The Md5 from client and Our Md5 are not same, aborting");
                Output.WriteLine("Decrypted (TF) blob:" + StringUtils.bytesToString(decryptedBlob));
                Output.WriteLine("Stored MD5ed Challenge:" + StringUtils.bytesToString(md5edChallenge));
                throw new AuthException("Md5 challenge differs");
            }

            // We take the pass from the decrypted Blob and subtract 1 byte, the ending "0"
            int passSize = ((int) decryptedBlob[23]) - 1;

            byte[] passwordB = new byte[passSize];
            ArrayUtils.copy(decryptedBlob, 25, passwordB, 0, passSize);

            Output.OptWriteLine("-> Password decrypted, size:" + passSize);

            // Set WorldList password
            wl.setPassword(StringUtils.bytesToString_NS(md5.digest(passwordB)));
            
            try
            {
                var user = Store.matrixDbContext.Users
                    .Where(u => u.Username ==
                                wl.getUsername())
                    .Single(u => u.Passwordmd5 ==
                                 StringUtils.bytesToString_NS(md5.digest(passwordB)));

                if (user.AccountStatus.Equals(1))
                {
                    SendAuthError((UInt32) AuthErrorCode.LTAS_ACCOUNTISBANNED);
                    return;
                }
            }
            catch (Exception ex)
            {
                SendAuthError((UInt32) AuthErrorCode.LTAS_ACCOUNTDOESNOTEXIST);
                return;
            }

            if (Store.dbManager.AuthDbHandler.FetchWorldList(ref wl))
            {
                // Do calculations magic
                byte[] hexUserID = NumericalUtils.uint32ToByteArray((UInt32) wl.getUserID(), 1);

                byte[] nameH = new byte[33];

                string name = wl.getUsername();
                for (int i = 0; i < name.Length; i++)
                {
                    nameH[i] = (byte) name[i];
                }

                byte[] padding = new byte[4]; // 4 empty byte ==> [0x00,0x00,0x00,0x00]

                // +10 mins from now, so 60secs per min
                byte[] expiredTime = TimeUtils.getUnixTime(60 * 10);

                byte[] padding2 = new byte[32]; // 32 empty byte ==> [0x00,0x00,0x00,0x00,...]
                byte[] pubModulusH = wl.getPublicModulus();
                byte[] createdTime = NumericalUtils.uint32ToByteArray((UInt32) wl.getTimeCreated(), 1);


                // Create signed data
                DynamicArray signedData = new DynamicArray();
                signedData.append(new byte[] {0x01});
                signedData.append(hexUserID);
                signedData.append(nameH);
                signedData.append(new byte[] {0x00, 0x01});
                signedData.append(padding);
                signedData.append(expiredTime);
                signedData.append(padding2);
                signedData.append(new byte[] {0x00, 0x11});
                signedData.append(pubModulusH);
                signedData.append(createdTime);

                byte[] md5FromStructure = md5.digest(signedData.getBytes());

                // Do MD5 to the signed data and RSA encrypt the result
                byte[] signature = rsa.signWithMD5(md5FromStructure);


                Output.OptWriteLine("-> Signing with RSA");

                // Do: privExp = Twofish(privExp)
                byte[] privExp = wl.getPrivateExponent();
                byte[] buffer = new byte[privExp.Length];

                // We keep the Key from before this part
                // Set the challenge we saved before as IV
                tf.SetIV(challenge);
                tf.Encrypt(privExp, buffer);

                privExp = buffer;

                byte[] privExpSize = NumericalUtils.uint16ToByteArray((UInt16) privExp.Length, 1);

                // Calculate offsets
                int offsetAuthData = 33 + wl.getCharPack().getPackLength() + wl.getWorldPack().getTotalSize();
                int offsetEncryptedData = offsetAuthData + signature.Length + signedData.getSize() + 2;
                int offsetCharData = 33;
                int offsetServerData = 33 + wl.getCharPack().getPackLength();
                int offsetUsernameLast = offsetEncryptedData + 2 + privExp.Length;

                // Create the auth header

                DynamicArray authHeader = new DynamicArray();
                authHeader.append(new byte[] {0x0b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00});
                authHeader.append(NumericalUtils.uint16ToByteArray((UInt16) offsetAuthData, 1));
                authHeader.append(NumericalUtils.uint16ToByteArray((UInt16) offsetEncryptedData, 1));
                authHeader.append(new byte[] {0x1f, 0x00, 0x00, 0x00});
                authHeader.append(NumericalUtils.uint16ToByteArray((UInt16) offsetCharData, 1));
                authHeader.append(new byte[] {0x6E, 0xD1, 0x00, 0x00});
                authHeader.append(NumericalUtils.uint32ToByteArray((UInt32) offsetServerData, 1));
                authHeader.append(NumericalUtils.uint32ToByteArray((UInt32) offsetUsernameLast, 1));

                byte[] usernameSize = NumericalUtils.uint16ToByteArray((UInt16) (name.Length + 1), 1);

                // Create the semiResponse (full data, except the total size)
                DynamicArray semiResponse = new DynamicArray();
                semiResponse.append(authHeader.getBytes());
                semiResponse.append(wl.getCharPack().getByteContents());
                semiResponse.append(wl.getWorldPack().getByteContents());
                semiResponse.append(new byte[] {0x36, 0x01});
                semiResponse.append(signature);
                semiResponse.append(signedData.getBytes());
                semiResponse.append(privExpSize);
                semiResponse.append(privExp);
                semiResponse.append(usernameSize);

                byte[] tempName = new byte[name.Length];

                for (int i = 0; i < name.Length; i++)
                {
                    tempName[i] = (byte) name[i];
                }

                semiResponse.append(tempName);
                semiResponse.append(new byte[] {0x00});

                int bigSize = semiResponse.getSize();
                bigSize += 0x8000; // Add TCP Len Var

                byte[] finalSize = NumericalUtils.uint16ToByteArray((UInt16) bigSize, 0);

                // Create the finalResponse (full data, plus total size)
                DynamicArray finalResponse = new DynamicArray();
                finalResponse.append(finalSize);
                finalResponse.append(semiResponse.getBytes());
                

                Output.OptWriteLine("Sending world list.");
                status = -1;
                
                authClientSession.SendAsync(finalResponse.getBytes());
            }

            byte[] worldListPacket = {0x00, 0x00};

            status = -1; // trick
            authClientSession.SendAsync(worldListPacket);
        }
        
        public void SendAuthError(UInt32 errorCode)
        {
            PacketContent packetContent = new PacketContent();
            packetContent.AddByte((byte) AuthOpcode.AS_AuthReply);
            packetContent.AddUint32(errorCode, 1);
            packetContent.AddInt32(0, 1);
            packetContent.AddUint16(0, 1);
            authClientSession.SendAsync(packetContent.ReturnFinalPacket());
        }
    }
}