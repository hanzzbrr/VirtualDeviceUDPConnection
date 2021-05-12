using System.Collections.Generic;

namespace VirtualDeviceUDPWeb.Models
{
    public class DeviceRepository
    {
        private static DeviceRepository _sharedRepository = new DeviceRepository();
        public static DeviceRepository SharedRepository => _sharedRepository;

        public Dictionary<int, DeviceModel> Devices => Program.UdpNetworkManager.Devices;

        public DeviceRepository()
        {

        }

        public void UpdateRepository()
        {

        }
    }
}
