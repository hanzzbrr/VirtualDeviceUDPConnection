using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.IO;

public class Program
{
    private const int listenPort = 62006;
    private const int sendPort = 62005;

    private static async Task StartListener()
    {
        IPEndPoint ep = new IPEndPoint(IPAddress.Any, listenPort);
        using (var listener = new UdpClient(ep))
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for broadcast");
                    byte[] bytes = (await listener.ReceiveAsync()).Buffer;
                    object resultPackage = null;

                    switch (bytes.Length)
                    {
                        case 8:
                            resultPackage = WardenPackage.FromArray(bytes);
                            break;
                        case 12:
                            resultPackage = Response.FromArray(bytes);
                            break;
                        default:
                            break;
                    }
                    Console.WriteLine($"len: {bytes.Length}, {resultPackage}");
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                listener.Close();
            }
        }
        
    }

    public static void Main()
    {
        var task = StartListener();
        ConsoleKey consoleKey;

        do
        {
            consoleKey = Console.ReadKey().Key;
            if(consoleKey == ConsoleKey.R)
            {
                Console.WriteLine("creating request");
                using(var client = new UdpClient())
                {
                    var endPoint = new IPEndPoint(IPAddress.Loopback, sendPort);
                    try
                    {
                        var readRequest = new ReadRequest(1);
                        byte[] sendBytes = readRequest.ToArray();

                        client.Connect(endPoint);
                        client.Send(sendBytes, sendBytes.Length);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
            else if(consoleKey == ConsoleKey.W)
            {
                Console.WriteLine("creating request");
                using (var client = new UdpClient())
                {
                    var endPoint = new IPEndPoint(IPAddress.Loopback, sendPort);
                    try
                    {
                        var readRequest = new WriteRequest(1,255,0);
                        byte[] sendBytes = readRequest.ToArray();

                        client.Connect(endPoint);
                        client.Send(sendBytes, sendBytes.Length);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }

        } while (consoleKey != ConsoleKey.X);
    }
}

[System.Serializable]
public class WardenPackage
{
    public int Id { private set; get; }
    public ushort Num1 { private set; get; }
    public ushort Num2 { private set; get; }

    public WardenPackage()
    {

    }

    public static WardenPackage FromArray(byte[] bytes)
    {
        var reader = new BinaryReader(new MemoryStream(bytes));

        var wardenPackage = new WardenPackage();

        wardenPackage.Id = reader.ReadInt32();
        wardenPackage.Num1 = reader.ReadUInt16();
        wardenPackage.Num2 = reader.ReadUInt16();

        return wardenPackage;
    }

    public override string ToString()
    {
        return $"Id: {Id} N1: {Num1} N2: {Num2}";
    }
}

[System.Serializable]
public class Response
{
    public int Id { private set; get; }
    public string Command { private set; get; }
    public ushort ResponseStatus { private set; get; }
    public ushort UThreshold { private set; get; }
    public ushort BThreshold { private set; get; }

    public Response()
    {
        
    }

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

[System.Serializable]
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

public class WriteRequest
{
    public int Id { set; get; }
    public string Command { set; get; }
    public ushort UThreshold { private set; get; }
    public ushort BThreshold { private set; get; }

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