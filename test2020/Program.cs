using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;

public class Program
{
    private const int listenPort = 62006;

    private static void StartListener()
    {
        UdpClient listener = new UdpClient(listenPort);
        IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

        try
        {
            while (true)
            {
                Console.WriteLine("Waiting for broadcast");
                byte[] bytes = listener.Receive(ref groupEP);

                Console.WriteLine($"Received broadcast from {groupEP} :");
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

    public static void Main()
    {
        StartListener();
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
        return $"Id: {Id} + N1: {Num1} + N2: {Num2}";
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class ResponsePackage
{
    public int Id { private set; get; }
    public string Command { private set; get; }
    public byte ResponseStatus { private set; get; }
    public byte UThreshold { private set; get; }
    public byte BThreshold { private set; get; }

    public ResponsePackage()
    {
        
    }

    public override string ToString()
    {
        return $"Id: {Id}, Command: {Command}, ResponseStatus: {ResponseStatus}, UpperThreshold: {UThreshold}" +
            $"BottomThreshold: {BThreshold}";
    }
}