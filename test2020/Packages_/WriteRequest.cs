using System;
using System.IO;
using System.Text;

namespace VirtualDeviceUDP.Packages_
{
    public class WriteRequest
    {
        public int Id { set; get; }
        public string Command { set; get; }
        public ushort UThreshold { private set; get; }
        public ushort BThreshold { private set; get; }

        public WriteRequest(string[] args)
        {
            Id = Int32.Parse(args[0]);
            Command = "LW";
            UThreshold = UInt16.Parse(args[1]);
            BThreshold = UInt16.Parse(args[2]);
        }

        public WriteRequest(int id, ushort upperThreshold, ushort bottomThreshold)
        {
            Id = id;
            Command = "LW";
            UThreshold = upperThreshold;
            BThreshold = bottomThreshold;
        }

        public byte[] ToArray()
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);


            writer.Write(Id);
            writer.Write(Encoding.ASCII.GetBytes(Command));
            writer.Write(UThreshold);
            writer.Write(BThreshold);

            return stream.ToArray();
        }
    }
}
