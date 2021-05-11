using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class Program
{
    private const int _deviceLifetime = 30;

    private const int listenPort = 62006;
    private const int sendPort = 62005;

    private static bool _lockOutput;

    private static Dictionary<int, Device> _devices = new Dictionary<int, Device>();

    private static async Task StartListener()
    {
        _lockOutput = false;
        IPEndPoint ep = new IPEndPoint(IPAddress.Any, listenPort);
        using (var listener = new UdpClient(ep))
        {
            try
            {
                while (true)
                {
                    byte[] bytes = (await listener.ReceiveAsync()).Buffer;
                    switch (bytes.Length)
                    {
                        case 8:
                            WardenPackage wardenPackage = WardenPackage.FromArray(bytes);
                            if (!_devices.ContainsKey(wardenPackage.Id))
                            {
                                _devices.Add(wardenPackage.Id, new Device() { Value1 = wardenPackage.Value1, Value2 = wardenPackage.Value2 });
                                var readRequestTask = SendReadRequest(wardenPackage.Id);
                            }
                            else
                            {
                                _devices[wardenPackage.Id].Value1 = wardenPackage.Value1;
                                _devices[wardenPackage.Id].Value2 = wardenPackage.Value2;
                            }
                            break;
                        case 12:
                            Response response = Response.FromArray(bytes);
                            if(_devices.ContainsKey(response.Id))
                            {
                                _devices[response.Id].BThreshold = response.BThreshold;
                                _devices[response.Id].UThreshold = response.UThreshold;
                            }
                            break;
                        default:
                            break;
                    }                  
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

    private static async Task SendReadRequest(int id)
    {
        using (var client = new UdpClient())
        {
            var endPoint = new IPEndPoint(IPAddress.Loopback, sendPort);
            try
            {
                var readRequest = new ReadRequest(id);
                byte[] sendBytes = readRequest.ToArray();

                client.Connect(endPoint);
                await client.SendAsync(sendBytes, sendBytes.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
    
    private static async Task OutputDevicesState()
    {
        while(true)
        {
            await Task.Delay(1000);
            if(_lockOutput)
            {
                continue;
            }
            foreach (var d in _devices)
            {
                Console.Write($"id: {d.Key}, Value1: {d.Value.Value1}, ");
                Console.ForegroundColor = d.Value.IsWithinLimits ? ConsoleColor.White : ConsoleColor.Red;
                Console.Write($"Value2: {d.Value.Value2}, ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"UpperLimit: {d.Value.UThreshold}, BottomLimit: {d.Value.BThreshold}");
            }
            Console.WriteLine();
        }
    }


    public static void Main()
    {
        var listenerTask = StartListener();
        var outStateTask = OutputDevicesState();
        ConsoleKey consoleKey;
        do
        {
            consoleKey = Console.ReadKey().Key;
            if(consoleKey == ConsoleKey.W)
            {
                _lockOutput = true;
                Console.WriteLine("creating request");

                using (var client = new UdpClient())
                {
                    var endPoint = new IPEndPoint(IPAddress.Loopback, sendPort);
                    try
                    {
                        WriteRequest writeRequest;
                        WriteRequestInputLoop(out writeRequest);
                        byte[] sendBytes = writeRequest.ToArray();

                        client.Connect(endPoint);
                        client.Send(sendBytes, sendBytes.Length);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
                _lockOutput = false;
            }

        } while (consoleKey != ConsoleKey.X);
    }

    private static void WriteRequestInputLoop(out WriteRequest wr)
    {        
        var rg = new Regex(@"()([1-9]|[1-5]?[0-9]{2,4}|6[1-4][0-9]{3}|65[1-4][0-9]{2}|655[1-2][0-9]|6553[1-5]) \b(1?[0-9]{1,2}|2[0-4][0-9]|25[0-5])\b \b(1?[0-9]{1,2}|2[0-4][0-9]|25[0-5])\b");
        string input = "";
        BufferedStream stream;
        do
        {
            Console.WriteLine("Enter request parameteres");
            input = Console.ReadLine();
        } while (!rg.IsMatch(input));
        
        wr = new WriteRequest(input.Split(' '));
    }
}


public class Device
{
    public ushort Value1 { set; get; }
    public ushort Value2 { set; get; }
    public ushort UThreshold { set; get; }
    public ushort BThreshold { set; get; }

    public bool IsWithinLimits => (Value2 <= UThreshold && Value2 >= BThreshold);

    public Device() { }

    public override string ToString()
    {
        return $"Value1: {Value1}, Value2: {Value2}, UThreshold: {UThreshold}, BThreshold {BThreshold}";
    }
}

[System.Serializable]
public class WardenPackage
{
    public int Id { private set; get; }
    public ushort Value1 { private set; get; }
    public ushort Value2 { private set; get; }

    public WardenPackage()
    {

    }

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

    public WriteRequest(string[] args)
    {
        Id = Int32.Parse(args[0]);
        Command = "LW";
        UThreshold =  UInt16.Parse(args[1]);
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