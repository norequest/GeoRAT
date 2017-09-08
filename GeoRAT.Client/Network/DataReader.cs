using System;
using System.Net.Sockets;
using System.Threading;


namespace GeoRAT.Client.Network
{
    class DataReader
    {
        #region Declarations
        //delegates and events

        public delegate void Received(byte[] buffer, int messageSize);
        public event Received OnReceived;
        public delegate void Disconnected(Socket s);
        public event Disconnected OnDisconnected;
        //fields and properties 
        private readonly Socket _clientReader;
        private readonly byte[] _lenbuffer = new byte[4];

        #endregion

        #region Constructor

        public DataReader(Socket s)
        {
            _clientReader = s;
            _clientReader.BeginReceive(new byte[] { 0 }, 0, 0, SocketFlags.None, ReceivedCallback, null);
        }


        #endregion


        #region ReceiveCallback
        //Begin reading data from server here 

        private void ReceivedCallback(IAsyncResult result)
        {
            try
            {

                var totalReceived = _clientReader.EndReceive(result);
                var left = sizeof(int) - totalReceived; // How much bytes left to receive. 
                do
                {
                    var recv = _clientReader.Receive(_lenbuffer, totalReceived, left, SocketFlags.None);
                    left -= recv;
                } while (left != 0);
                var len = BitConverter.ToInt32(_lenbuffer, 0); //Deserialize received bytes back to INT and get length of packet
                ReceiveMessage(len); //Begin reading incoming packets based on length now 
                _clientReader.BeginReceive(new byte[] { 0 }, 0, 0, SocketFlags.None, ReceivedCallback, null);
            }
            catch
            {

                OnDisconnected?.Invoke(_clientReader);
            }
        }

        #endregion


        #region ReceiveMessage
        //Reads incoming data from client, based on length of data we got earlier 
        //Keeps reading until it gets exact amount of bytes we need 
        private void ReceiveMessage(int size)
        {
            try
            {
                int total = 0;

                byte[] buffer = new byte[size];
                do
                {
                    var result = _clientReader.BeginReceive(buffer, total, size, SocketFlags.None, null, null);
                    result?.AsyncWaitHandle.WaitOne(_clientReader.ReceiveTimeout);
                    if (result == null) continue;
                    var received = _clientReader.EndReceive(result);
                    total += received;
                    if (received == 0)
                        break;
                } while (total < size);
                OnReceived?.Invoke(buffer, size);

            }
            catch
            {

                OnDisconnected?.Invoke(_clientReader);
            }
        }

        #endregion

        #region Send

        //Send data to server via this function 
        public void Send(byte[] buf)
        {
            _clientReader.BeginSend(buf, 0, buf.Length, SocketFlags.None, SendCb, null);
        }

        private void SendCb(IAsyncResult result)
        {
            _clientReader.EndSend(result);
        }

    }
    #endregion

}
