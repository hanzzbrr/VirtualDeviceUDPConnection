using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtualDeviceUDPAPI;

namespace VirtualDeviceUDPWeb.Models
{
    public class DeviceModel : IDevice
    {
        private Device _device;

        public ushort Value1 { get; set; }
        public ushort Value2 { get; set; }
        public ushort UpLimit { get; set; }
        public ushort LowLimit { get; set; }

        public bool IsWithinLimits => (Value2 <= UpLimit && Value2 >= LowLimit);

        public DeviceModel() { }
        public override string ToString()
        {
            return $"Value1: {Value1}, Value2: {Value2}, UpLimit: {UpLimit}, LowLimit {LowLimit}";
        }
    }
}
