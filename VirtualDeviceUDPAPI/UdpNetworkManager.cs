using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using VirtualDeviceUDPAPI.Packages_;

namespace VirtualDeviceUDPAPI
{
    public class UdpNetworkManager
    {
        private const int _wardenPackageSize = 8;
        private const int _responsePackageSize = 12;

        private const int _deviceLifetime = 30; //TODO: remove dead devices from _devices dict

        private const int listenPort = 62006;
        private const int sendPort = 62005;

        private Dictionary<int, IDevice> _devices = new Dictionary<int, IDevice>();

        public Dictionary<int, IDevice> Devices => _devices;

        public UdpNetworkManager()
        {
            _ = ListenToIncomingPackagesAsync();
        }

        public void CreateWriteRequest(string[] writeRequestArgs)
        {
            _ = CreateWriteRequestAsync(writeRequestArgs);
        }

        private async Task ListenToIncomingPackagesAsync()
        {
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
                            case _wardenPackageSize:
                                WardenPackage wardenPackage = WardenPackage.FromArray(bytes);
                                if (!_devices.ContainsKey(wardenPackage.Id))
                                {
                                    _devices.Add(wardenPackage.Id, new Device()
                                    {
                                        Value1 = wardenPackage.Value1,
                                        Value2 = wardenPackage.Value2
                                    });
                                    _ = CreateReadRequestAsync(wardenPackage.Id);
                                }
                                else
                                {
                                    _devices[wardenPackage.Id].Value1 = wardenPackage.Value1;
                                    _devices[wardenPackage.Id].Value2 = wardenPackage.Value2;
                                }
                                break;
                            case _responsePackageSize:
                                Response response = Response.FromArray(bytes);
                                if (_devices.ContainsKey(response.Id))
                                {
                                    _devices[response.Id].LowLimit = response.LowLimit;
                                    _devices[response.Id].UpLimit = response.UpLimit;
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

        private async Task CreateWriteRequestAsync(string[] writeRequestArgs)
        {
            Console.WriteLine("creating request");
            using (var client = new UdpClient())
            {
                var endPoint = new IPEndPoint(IPAddress.Loopback, sendPort);
                try
                {
                    WriteRequest writeRequest = new WriteRequest(writeRequestArgs);
                    byte[] sendBytes = writeRequest.ToArray();

                    client.Connect(endPoint);
                    await client.SendAsync(sendBytes, sendBytes.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        private async Task CreateReadRequestAsync(int id)
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

    }
}
