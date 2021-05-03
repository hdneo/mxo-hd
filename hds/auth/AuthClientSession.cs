﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using hds;
 using hds.auth;
 using hds.databases.Entities;
 using NetCoreServer;

namespace hds
{
    public class AuthClientSession : TcpSession
    {
        private AuthHandler _handler;

        public AuthClientSession(TcpServer server) : base(server)
        {
            _handler = new AuthHandler(this);
        }
        
        protected override void OnConnected()
        {
            _handler = new AuthHandler(this);
            Console.WriteLine($"AuthClient with Id {Id} connected!");
            
        }

        protected override void OnDisconnected()
        {
            
            Console.WriteLine($"AuthClient with Id {Id} disconnected!");
        }

        protected override void OnReceived(byte[] packet, long offset, long size)
        {
            byte[] packetContent = new byte[size];
            ArrayUtils.copy(packet,0, packetContent, 0, (int)size);
            PacketReader reader = new PacketReader(packetContent);
            // Multicast message to all connected sessions
            _handler.ProcessPacket(reader, size);

        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"AuthClient Session caught an error with code {error}");
        }
    }
}