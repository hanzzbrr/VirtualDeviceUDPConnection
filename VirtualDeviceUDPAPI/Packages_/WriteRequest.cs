using System;
using System.IO;
using System.Text;

namespace VirtualDeviceUDPAPI.Packages_
{
    public class WriteRequest
    {
        public int Id { set; get; }
        public string Command { set; get; }
        public ushort UpLimit { private set; get; }
        public ushort LowLimit { private set; get; }

        public WriteRequest(string[] args)
        {
            Id = Int32.Parse(args[0]);
            Command = "LW";
            UpLimit = UInt16.Parse(args[1]);
            LowLimit = UInt16.Parse(args[2]);
        }

        public WriteRequest(int id, ushort upperThreshold, ushort bottomThreshold)
        {
            Id = id;
            Command = "LW";
            UpLimit = upperThreshold;
            LowLimit = bottomThreshold;
        }

        public byte[] ToArray()
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);


            writer.Write(Id);
            writer.Write(Encoding.ASCII.GetBytes(Command));
            writer.Write(UpLimit);
            writer.Write(LowLimit);

            return stream.ToArray();
        }
    }
}
