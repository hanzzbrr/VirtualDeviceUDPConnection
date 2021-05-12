using System.IO;
using System.Text;

namespace VirtualDeviceUDPAPI.Packages_
{
    public class Response
    {
        public int Id { private set; get; }
        public string Command { private set; get; }
        public ushort ResponseStatus { private set; get; }
        public ushort UThreshold { private set; get; }
        public ushort BThreshold { private set; get; }

        public Response(){ }

        public static Response FromArray(byte[] bytes)
        {
            var reader = new BinaryReader(new MemoryStream(bytes));

            var response = new Response();

            response.Id = reader.ReadInt32();
            response.Command = Encoding.ASCII.GetString(reader.ReadBytes(2));
            response.ResponseStatus = reader.ReadUInt16();
            response.UThreshold = reader.ReadUInt16();
            response.BThreshold = reader.ReadUInt16();

            return response;
        }

        public override string ToString()
        {
            return $"Id: {Id}, Command: {Command}, ResponseStatus: {ResponseStatus}, UpperThreshold: {UThreshold}" +
                $"BottomThreshold: {BThreshold}";
        }
    }
}
