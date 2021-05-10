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
                var data = (IncomingPackage)Marshal.PtrToStructure(gcHandle.AddrOfPinnedObject(), typeof(IncomingPackage));
                gcHandle.Free();

                Console.WriteLine($"Id: {data.Id} + N1: {data.Num1} + N2: {data.Num2}\n");

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
public class IncomingPackage
{
    public int Id { private set; get; }
    public int Num1 { private set; get; }
    public int Num2 { private set; get; }

    public IncomingPackage()
    {

    }
}