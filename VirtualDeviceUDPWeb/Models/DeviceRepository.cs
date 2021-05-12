using System.Collections.Generic;
using VirtualDeviceUDPAPI;
using VirtualDeviceUDPWeb;

namespace VirtualDeviceUDPWeb.Models
{
    public class DeviceRepository
    {
        private static DeviceRepository _sharedRepository = new DeviceRepository();
        private Dictionary<int, DeviceModel> _devices = new Dictionary<int, DeviceModel>();

        public static DeviceRepository SharedRepository => _sharedRepository;

        

        public Dictionary<int, DeviceModel> Devices => _devices;

        public DeviceRepository()
        {

        }

        public void UpdateRepository()
        {

        }
    }
}
