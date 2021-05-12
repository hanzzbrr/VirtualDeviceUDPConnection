using System.Collections.Generic;
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
            _devices = new Dictionary<int, DeviceModel>();
        }

        public void UpdateRepository()
        {
            _devices = new Dictionary<int, DeviceModel>()
        }
    }
}
