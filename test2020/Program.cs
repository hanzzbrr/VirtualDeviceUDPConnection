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
        using (var listener = new UdpClient(listenPort))
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for broadcast");
                    byte[] bytes = (await listener.ReceiveAsync()).Buffer;
                    GCHandle gcHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                    object resultPackage = null;

                    switch (bytes.Length)
                    {
                        case 8:
                            //resultPackage = (WardenPackage)Marshal.PtrToStructure(gcHandle.AddrOfPinnedObject(), typeof(WardenPackage));
                            resultPackage = WardenPackage.FromArray(bytes);
                            break;
                        case 16:
                            resultPackage = WardenPackage.FromArray(bytes);
                            break;
                        default:
                            break;
                    }

                    Console.WriteLine(resultPackage);
                    gcHandle.Free();
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

    private static void CreateRequest()
    {
        using(var udpClient = new UdpClient(sendPort))
        {
            Byte[] sendBytes = Encoding.ASCII.GetBytes("Is anybody there");
            try
            {
                udpClient.Send(sendBytes, sendBytes.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
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
    public byte ResponseStatus { private set; get; }
    public byte UThreshold { private set; get; }
    public byte BThreshold { private set; get; }

    public Response()
    {
        
    }

    public static Response FromArray(byte[] bytes)
    {
        var reader = new BinaryReader(new MemoryStream(bytes));

        var response = new Response();

        response.Id = reader.ReadInt32();
        response.Command = reader.ReadString();
        response.ResponseStatus = reader.ReadByte();
        response.UThreshold = reader.ReadByte();
        response.BThreshold = reader.ReadByte();

        return response;
    }

    public override string ToString()
    {
        return $"Id: {Id}, Command: {Command}, ResponseStatus: {ResponseStatus}, UpperThreshold: {UThreshold}" +
            $"BottomThreshold: {BThreshold}";
    }
}


public class ReadRequest
{
    public int Id { set; get; }
    public string Command { set; get; }
    
    public ReadRequest(int id)
    {
        id = Id;
        Command = "LR";
    }
}

public class WriteRequest
{
    public int Id { set; get; }
    public string Command { set; get; }
    public byte UThreshold { private set; get; }
    public byte BThreshold { private set; get; }

    public WriteRequest(int id, byte upperThreshold, byte bottomThreshold)
    {
        Id = id;
        Command = "LW";
        UThreshold = upperThreshold;
        BThreshold = bottomThreshold;
    }
}