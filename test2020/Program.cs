using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

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
                            resultPackage = (WardenPackage)Marshal.PtrToStructure(gcHandle.AddrOfPinnedObject(), typeof(WardenPackage));
                            break;
                        case 16:
                            resultPackage = (WardenPackage)Marshal.PtrToStructure(gcHandle.AddrOfPinnedObject(), typeof(WardenPackage));
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

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class WardenPackage
{
    public int Id { private set; get; }
    public ushort Num1 { private set; get; }
    public ushort Num2 { private set; get; }

    public WardenPackage()
    {

    }

    public override string ToString()
    {
        return $"Id: {Id} N1: {Num1} N2: {Num2}";
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
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