using System.IO;

namespace VirtualDeviceUDPWeb.Packages_
{
    public class WardenPackage
    {
        public int Id { private set; get; }
        public ushort Value1 { private set; get; }
        public ushort Value2 { private set; get; }       

        public WardenPackage() { }

        public static WardenPackage FromArray(byte[] bytes)
        {
            var reader = new BinaryReader(new MemoryStream(bytes));

            var wardenPackage = new WardenPackage();

            wardenPackage.Id = reader.ReadInt32();
            wardenPackage.Value1 = reader.ReadUInt16();
            wardenPackage.Value2 = reader.ReadUInt16();

            return wardenPackage;
        }

        public override string ToString()
        {
            return $"Id: {Id} Value1: {Value1} N2: {Value2}";
        }
    }
}
