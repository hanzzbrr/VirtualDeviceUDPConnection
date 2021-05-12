using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using VirtualDeviceUDPAPI;

namespace VirtualDeviceUDPWeb
{
    public class Program
    {
        private static UdpNetworkManager _udpManager;
        public static UdpNetworkManager UdpNetworkManager => _udpManager;

        public static void Main(string[] args)
        {
            InitUdpmanager();
            CreateWebHostBuilder(args).Build().Run();
        }

        private static void InitUdpmanager()
        {
            _udpManager = new UdpNetworkManager();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
