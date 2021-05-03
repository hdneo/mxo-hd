using System.Net;
using System.Net.Sockets;
using System.Threading;
using NetCoreServer;
using TcpClient = System.Net.Sockets.TcpClient;

namespace hds.auth
{
    public class AuthServer : TcpServer
    {
        private TcpListener tcpListener;
        private Thread listenThread;
        private bool mainThreadWorking;

        
        protected override TcpSession CreateSession()
        {
            return new AuthClientSession(this);
        }

        public AuthServer() : base(IPAddress.Any, 11000)
        {
            mainThreadWorking = true;
            Output.WriteLine("Auth server set and ready at port 11000");
        }

        public void startServer()
        {
            mainThreadWorking = true;
            Start();
        }

        public void stopServer()
        {
            mainThreadWorking = false;
            Stop();
        }
    }
}