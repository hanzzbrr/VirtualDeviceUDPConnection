using System.IO;
using System.Text;

namespace VirtualDeviceUDPWeb.Packages_
{
    public class ReadRequest
    {
        public int Id { set; get; }
        public string Command { set; get; }

        public ReadRequest(int id)
        {
            Id = id;
            Command = "LR";
        }

        public byte[] ToArray()
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);


            writer.Write(this.Id);
            writer.Write(Encoding.ASCII.GetBytes(Command));

            return stream.ToArray();
        }
    }
}
