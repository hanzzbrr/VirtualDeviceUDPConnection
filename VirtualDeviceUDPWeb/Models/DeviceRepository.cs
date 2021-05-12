using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            _devices.Add(1, new DeviceModel()
            {
                Value1 = 25,
                Value2 = 15,
                UpLimit = 225,
                LowLimit = 25
            });

            _devices.Add(2, new DeviceModel()
            {
                Value1 = 55,
                Value2 = 4,
                UpLimit = 222,
                LowLimit = 12
            });
        }
    }
}
