using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;
using System.Security.Cryptography;

using ManyMonkeys.Cryptography;
using System.Configuration;


namespace servertest
{
    class worldserver
    {
        /*
        private IPEndPoint udplistener;
        private Thread listenThread;
        public int serverport;
        int awake = 0;
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        utils utilitys = new utils();
        cipherman crypto = new cipherman();
        // later this should be in the client class
        uint cseq = 0;
        uint sseq = 0;
        uint pss = 0;
        Int16 messagecounter = 0;
        public PacketBuilder packet;
        public Logging logger;
        

        public worldserver(){
            // Startup our Server Entities
            AppSettingsReader worldConfig = new AppSettingsReader();
            serverport = (int)worldConfig.GetValue("WorldServerPort", typeof(int));
            this.startWorldServer();
            packet = new PacketBuilder(messagecounter);
            logger = new Logging();
            
        }
        
        public void startWorldServer(){
           
            udplistener = new IPEndPoint(IPAddress.Any, serverport);
            this.listenThread = new Thread(new ThreadStart(ListenForAllClients));
            this.listenThread.Start();
            System.Console.WriteLine("[World Server] Started at Port "+serverport);
        }

        public void handleInput(object remoteobject)
        {
            while (true)
            {
                EndPoint remote = remoteobject as EndPoint;
                string message = Console.ReadLine();
                byte[] response = { };
                if (message.Contains("sendsub:") && message.EndsWith(";"))
                {
                    // strip the hexpacket, we start imediately after "send:" and cut len - 6 cause to strip the ";"
                    string hexpacket = message.Substring(8, message.Length - 9);

                    hexpacket = hexpacket.Trim();
                    hexpacket = hexpacket.Replace(" ", "");

                    Console.WriteLine("user want to send :" + hexpacket);
                    packet.addMessage(utils.ToByteArray(hexpacket));
                    response = packet.finishReliable();
                    sendCrypted(response, remote);
                    packet.resetPackets();
                    Console.WriteLine("Send Sub ok");
                }

                if (message.Contains("send:") && message.EndsWith(";"))
                {
                    // for other packets than subpackets
                    string hexpacket = message.Substring(5, message.Length - 6);
                    hexpacket.Replace(" ", "");
                    sendCrypted(response, remote);
                    Console.WriteLine("Send Packet ok");
                }
            }
        }

        private void ListenForAllClients()
        {

            socket.Bind(this.udplistener);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)(sender);
            

            Thread readlinethread = new Thread(new ParameterizedThreadStart(handleInput));
           
            readlinethread.Start(Remote);

            while (true)
            {
                // check if an input is done
                //string stringInput = Console.ReadLine();

                byte[] message = new byte[4096];
                // Debug, show received packet in hex



                int receivedDataLenght = socket.ReceiveFrom(message, ref Remote);

                byte[] finalMessage = new byte[receivedDataLenght];

                for (int i = 0; i < receivedDataLenght; i++)
                {
                    finalMessage[i] = message[i];
                }

                if (receivedDataLenght > 0)
                {
                    Console.WriteLine("Datasize is :" + receivedDataLenght);
                }


                processPacket(finalMessage, Remote);
                //socket.SendTo(message, receivedDataLenght, SocketFlags.None, Remote);


            }
        }

        private void OnReceive(IAsyncResult ar)
        {
            IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint epSender = (EndPoint)ipeSender;
            socket.EndReceiveFrom(ar, ref epSender);


        }

        public void initWorldResponse(EndPoint remote)
        {
            // Here we send the Response 
            byte[] response = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05 };

            int i = 1;
            // Send the initial udp session packets 5 times
            while (i <= 5)
            {

                socket.SendTo(response, remote);
                Console.WriteLine("[Debug World] Send initial packet "+i);
                i++;
            }
            System.Threading.Thread.Sleep(1000);
            // And now init the client with 2 encrypted packets
            packet.resetPackets();

            packet.addMessage(utils.ToByteArray("060E00010000007763104901440034007265736F757263652F776F726C64732F66696E616C5F776F726C642F736C756D735F62617272656E735F66756C6C2E6D657472002B0048616C6C6F7765656E5F4576656E742C57696E7465723348616C6C6F7765656E466C794579655453454300"));
            packet.addMessage(utils.ToByteArray("80E5E7CBC01200000000"));
            packet.addMessage(utils.ToByteArray("80E400286BEE00000000"));
            packet.addMessage(utils.ToByteArray("80B24E0008000802"));
            packet.addMessage(utils.ToByteArray("80B252000C000802"));
            packet.addMessage(utils.ToByteArray("80B2540006000802"));
            packet.addMessage(utils.ToByteArray("80B24F000A000802"));
            packet.addMessage(utils.ToByteArray("80B2510008000802"));
            packet.addMessage(utils.ToByteArray("80BC1500020000F70300000802000000000000000000"));
            packet.addMessage(utils.ToByteArray("80BC1500030000F70300000000000000000000000000"));
            packet.addMessage(utils.ToByteArray("80BC1500040000F70300000000000000000000000000"));
            packet.addMessage(utils.ToByteArray("80BC1500050000F70300000000000000000000000000"));
            packet.addMessage(utils.ToByteArray("80BC1500060000F70300000000000000000000000000"));
            packet.addMessage(utils.ToByteArray("80BC1500070000F70300000000000000000000000000"));

            packet.addMessage(utils.ToByteArray("47000004010000000000000000000000000000001A0006000000010000000001010000000000800000000000000000"));
            packet.addMessage(utils.ToByteArray("2E0700000000000000000000005900002E00000000000000000000000000000000000000"));

            response = packet.finishReliable();   
            sendCrypted(response, remote);

            
            // 2. encrypted packets(Flash Traffic URL)
            // first lets reset all packet values in the packetbuilder object
            packet.resetPackets();
            
            Array.Clear(response, 0, response.Length);
            packet.addMessage(utils.ToByteArray("808628A9020000000000000000000000000000000000000000210000000000230000000000"));
            packet.addMessage(utils.ToByteArray("81A900000700053F00687474703A2F2F7468656D61747269786F6E6C696E652E73746174696F6E2E736F6E792E636F6D2F70726F63657373466C617368547261666669632E766D00"));
            response = packet.finishReliable();
            sendCrypted(response, remote);
            packet.resetPackets();

            pss = 1;
            packet.addMessage(utils.ToByteArray("060E00010000007763104901440034007265736F757263652F776F726C64732F66696E616C5F776F726C642F736C756D735F62617272656E735F66756C6C2E6D657472002B0048616C6C6F7765656E5F4576656E742C57696E7465723348616C6C6F7765656E466C794579655453454300"));
            //packet.addMessage(utils.ToByteArray("80E5E7CBC01200000000"));
            //packet.addMessage(utils.ToByteArray("80E400286BEE00000000"));
            response = packet.finishReliable();
            sendCrypted(response, remote);
            packet.resetPackets();

        }

        public void initPlayerToGame(EndPoint remote)
        {
            pss = 1;
            // first ack the packet
            Ack(remote);
            // now lets response 
            byte[] response = { };

            packet.actioncounter = 0;
            packet.addMessage(utils.ToByteArray("060E00010000007763104901440034007265736F757263652F776F726C64732F66696E616C5F776F726C642F736C756D735F62617272656E735F66756C6C2E6D657472002B0048616C6C6F7765656E5F4576656E742C57696E7465723348616C6C6F7765656E466C794579655453454300"));
            packet.addMessage(utils.ToByteArray("80E50000000000000000"));
            packet.addMessage(utils.ToByteArray("80E40000000000000000"));
            response = packet.finishReliable();
            sendCrypted(response, remote);
            packet.resetPackets();

            // Second Packet
            packet.addMessage(utils.ToByteArray("80B24E0008000802"));
            packet.addMessage(utils.ToByteArray("80B252000C000802"));
            packet.addMessage(utils.ToByteArray("80B2540006000802"));
            packet.addMessage(utils.ToByteArray("80B24F000A000802"));
            packet.addMessage(utils.ToByteArray("80B2510008000802"));
            packet.addMessage(utils.ToByteArray("80BC1500020000F70300000802000000000000000000"));
            packet.addMessage(utils.ToByteArray("80BC1500030000F70300000702EC0000000000000000"));
            packet.addMessage(utils.ToByteArray("80BC1500040000F70300005004000000000000000000"));
            response = packet.finishReliable();
            sendCrypted(response, remote);
            packet.resetPackets();
        }

        public void initPlayerToWorld(EndPoint remote)
        {
            byte[] response = { };
            // Initial Packets
            pss = 1;
            packet.addMessage(utils.ToByteArray("80B24E0008000802"));
            packet.addMessage(utils.ToByteArray("80B252000C000802"));
            packet.addMessage(utils.ToByteArray("80B2540006000802"));
            packet.addMessage(utils.ToByteArray("80B24F000A000802"));
            packet.addMessage(utils.ToByteArray("80B2510008000802"));
            packet.addMessage(utils.ToByteArray("80BC1500020000F70300000802000000000000000000"));
            packet.addMessage(utils.ToByteArray("80BC1500030000F70300000000000000000000000000"));
            packet.addMessage(utils.ToByteArray("80BC1500040000F70300005004000000000000000000"));
            response = packet.finishReliable();
            sendCrypted(response, remote);
            packet.resetPackets();

            // Spawn Packet
            pss = 15;
            byte[] spawnPacket = utils.ToByteArray("020301000C0C00A5CDAB108846697368626F726E650000000000000000000000000000000000000000000000904C617572656E7A000000000000000000000000000000000000000000000000008088280A84F700C44D6F72706865757A7A7A00000000000000000000000000000000000000000000280AE90010000028A902002010200208C0000160002A40800000F7009900000000C0D8D1400000000000F07E40000000000097BD4032228088070C010010000002000000000401000B011680BC1500050000F7030000F403000000000000000000");
            sendCrypted(spawnPacket, remote);

            // Again some Action / GUI things 
            packet.addMessage(utils.ToByteArray("80BC1500060000F70300000000000000000000000000"));
            packet.addMessage(utils.ToByteArray("80BC1500070000F703000052040F0000000000000000"));
            response = packet.finishReliable();
            sendCrypted(response, remote);
            packet.resetPackets();

            // PVP max safe
            pss = 127;

            response = utils.ToByteArray("020301000C180A2ECDAB0101010000000300000000");
            sendCrypted(response, remote);

            packet.addMessage(utils.ToByteArray("80BC15001A000011000000F303000000000000000000"));
            packet.addMessage(utils.ToByteArray("80BD051100000000000001"));
            packet.addMessage(utils.ToByteArray("3A050011000A0050765053657276657200040006000000"));
            packet.addMessage(utils.ToByteArray("3A0500170010005076504D6178536166654C6576656C0004000F000000"));
            packet.addMessage(utils.ToByteArray("3A050014000D0057525F52657A4576656E7473002B0048616C6C6F7765656E5F4576656E742C57696E7465723348616C6C6F7765656E466C794579655453454300"));
            packet.addMessage(utils.ToByteArray("3A0500190012004576656E74536C6F74315F456666656374000000"));
            response = packet.finishReliable();
            sendCrypted(response, remote);
            packet.resetPackets();

            response = utils.ToByteArray("02030100022400030001CF073B0005002103000000640064009600960000E0124600809343006086C60000");
            sendCrypted(response, remote);

            // Event and sky
            packet.addMessage(utils.ToByteArray("3A0500190012004576656E74536C6F74325F456666656374000D00666C796D616E5F69646C653300"));
            packet.addMessage(utils.ToByteArray("3A0500190012004576656E74536C6F74335F456666656374000A00666C795F766972757300"));
            packet.addMessage(utils.ToByteArray("3A05001B001400466978656442696E6B49444F766572726964650002002000"));
            packet.addMessage(utils.ToByteArray("8167170024001C2200C6011100000000000000310000080B004D6F72706865757A7A7A000B004D6F72706865757A7A7A0002000100000000000000"));
            response = packet.finishReliable();
            sendCrypted(response, remote);
            packet.resetPackets();

        }

        public void testSubMessage(EndPoint remote)
        {
            // Test Sub packets
            StreamReader tr = new StreamReader("subpacket.txt");
            string submessage = tr.ReadLine();
            submessage = submessage.Trim();
            submessage = submessage.Replace(" ", "");
            byte[] response = { };
            packet.addMessage(utils.ToByteArray(submessage));
            tr.Close();
            //packet.addMessage(utils.ToByteArray("81170000000010001C4D0000000000000100000000088E220000")); // items could not be uploaded
            //packet.addMessage(utils.ToByteArray("81190000000010001C4D0000000000000100000000088E220000")); // loot packet
            //packet.addMessage(utils.ToByteArray("80ACD9000000000000000000000000000040")); // Evade Combat PRogress Bar
            //packet.addMessage(utils.ToByteArray("0880FD000000000100"));
            response = packet.finishReliable();
            sendCrypted(response, remote);
            packet.resetPackets();
        }

        public void testPacket(EndPoint remote)
        {
            // Test Sub packets
            StreamReader tr = new StreamReader("packet_03.txt");
            string submessage = tr.ReadLine();
            submessage = submessage.Trim();
            submessage = submessage.Replace(" ", "");
            byte[] response = { };
            response = utils.ToByteArray(submessage);
            tr.Close();
            //packet.addMessage(utils.ToByteArray("81170000000010001C4D0000000000000100000000088E220000")); // items could not be uploaded
            //packet.addMessage(utils.ToByteArray("81190000000010001C4D0000000000000100000000088E220000")); // loot packet
            //packet.addMessage(utils.ToByteArray("80ACD9000000000000000000000000000040")); // Evade Combat PRogress Bar
            //packet.addMessage(utils.ToByteArray("0880FD000000000100"));
            sendCrypted(response, remote);
            packet.resetPackets();
        }

        public void Ack(EndPoint remote)
        {
            byte[] response = { };
            pss = 127;
            response = utils.ToByteArray("02");
            sendCrypted(response, remote);
        }

        public void sendCrypted(byte[] plain,EndPoint remote)
        {
            logger.LogPacket(plain, "[Send Packet] Current Messagecount is " + packet.actioncounter + "");
            sseq++;
            utils tool = new utils();
            Console.WriteLine("PSS: " + pss + " | CSEQ:" + cseq + " | SSEQ:" + sseq + "");
            tool.showHex(plain, "before encryption");
            byte[] encryptedResponse = crypto.encryptWorld(plain,sseq,cseq,pss);
            string encryptedPacket = BitConverter.ToString(encryptedResponse);
            encryptedPacket = encryptedPacket.Replace("-", " ");
            socket.SendTo(encryptedResponse, remote);
        }


        public void processPacket(byte[] packet, EndPoint remote)
        {
            byte header = packet[0];
            

            switch (header)
            {
                case 0x00:

                    if (awake == 0)
                    {
                        initWorldResponse(remote);
                        awake = 1;
                    }
                    else
                    {
                        // Just reply with the same thing
                        socket.SendTo(packet, remote);
                        
                    }
                break;

                case 0x01:
                    cseq++;
                    byte[] decryptedPacket = crypto.decryptWorld(packet);
                    //Array.Copy(decryptedPacket, 17, decryptedData, 0, decryptedPacket.Length);
                    processDecryptedPacket(decryptedPacket, remote);
                   
                break;
            }
        }

        public void processDecryptedPacket(byte[] data,EndPoint remote)
        {
            byte acktype = data[0];
            byte packettype = data[1];

            switch (acktype)
            {
                case 0x02:

                    switch (packettype)
                    {

                        case 0x04:
                            processActionPacket(data,remote);
                            Ack(remote);
                        break;


                        case 0x03:
                            
                            logger.LogPacket(data, "[World Server 03 packet request]");
                            Ack(remote);
                        break;

                        case 0x05:
                            // Process initial packets part 2
                            // we call it "initPlayerToGame"

                            Ack(remote);
                            pss = 1;
                            
                            logger.LogPacket(data, "[World Server 05 packet request]");

                        break;
                        default:
                            Ack(remote);
                        break;
                    }

                break;

                case 0x42:
                    Ack(remote);

                break;
            }
        }

        public void processActionPacket(byte[] packet, EndPoint remote)
        {
            // ToDo: Parse the 04 packet, strip the messages and parse every message to handle it
            // Idea : Do a while loop , while we process the submessages, add submessages to next packet
            // And check how much submessages we have..to prevent to have a big size and send the stuff
            
            int messagecount = BitConverter.ToInt16(packet, 5) >>8;
            
            byte[] submessages = new byte[packet.Length-6];
            Array.Copy(packet, 6, submessages,0, packet.Length-6);
            Console.WriteLine("[World Processor] Request has " + messagecount + " Subpackets");
            for (int i = 0; i < messagecount; i++)
            {
                byte[] sizebyte = new byte[2];
                sizebyte[0] = submessages[0];
                sizebyte[1] = 0x00;
                int size = BitConverter.ToInt16(sizebyte, 0);
                Console.WriteLine("[WorldDebug] Size of submessages is " + size);       
                byte[] message = new byte[size];
                Array.Copy(submessages, 1, message, 0, size);
                crypto.showPacket(message, "Submessage Received:");
                // Log Submessage to file
                
                logger.LogPacket(message, "[World Server 04 Submessage Client Request]");
                processActionRequest(message, remote);

            }

        }



        public void processActionRequest(byte[] message, EndPoint remote)
        {
            byte opcode = message[0];
            switch (opcode)
            {
                case 0x05:
                    initPlayerToWorld(remote);
                break;

                case 0x28:
                    testPacket(remote);
                break;
                case 0x81:
                    testSubMessage(remote);
                break;


                default:
                    Ack(remote);
                break;

            }

        }

        public static byte[] StrToByteArray(string str)
        {
            System.Text.ASCIIEncoding encoder = new ASCIIEncoding();
            return encoder.GetBytes(str);
        }

        public static string ByteArrayToStr(byte[] barr)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetString(barr, 0, barr.Length);
        }
  */
    }
}
