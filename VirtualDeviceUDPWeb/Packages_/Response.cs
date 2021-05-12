using System.IO;
using System.Text;

namespace VirtualDeviceUDPWeb.Packages_
{
    public class Response
    {
        public int Id { private set; get; }
        public string Command { private set; get; }
        public ushort ResponseStatus { private set; get; }
        public ushort UpLimit { private set; get; }
        public ushort LowLimit { private set; get; }

        public Response(){ }

        public static Response FromArray(byte[] bytes)
        {
            var reader = new BinaryReader(new MemoryStream(bytes));

            var response = new Response();

            response.Id = reader.ReadInt32();
            response.Command = Encoding.ASCII.GetString(reader.ReadBytes(2));
            response.ResponseStatus = reader.ReadUInt16();
            response.UpLimit = reader.ReadUInt16();
            response.LowLimit = reader.ReadUInt16();

            return response;
        }

        public override string ToString()
        {
            return $"Id: {Id}, Command: {Command}, ResponseStatus: {ResponseStatus}, UpperThreshold: {UpLimit}" +
                $"BottomThreshold: {LowLimit}";
        }
    }
}
