

using System;
using System.Net;
using System.Net.Sockets;

namespace GeoRAT.Client.Core
{
    class RemoteDesktopHandler
    {
        private Socket _udpSender;

        public RemoteDesktopHandler()
        {
            _udpSender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);

        }

        private void Connected(IAsyncResult result)
        {

        }

    }
}
