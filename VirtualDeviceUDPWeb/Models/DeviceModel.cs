using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualDeviceUDPWeb.Models
{
    public class DeviceModel
    {
        public ushort Value1 { set; get; }
        public ushort Value2 { set; get; }
        public ushort UpLimit { set; get; }
        public ushort LowLimit { set; get; }

        public bool IsWithinLimits => (Value2 <= UpLimit && Value2 >= LowLimit);

        public DeviceModel() { }

        public override string ToString()
        {
            return $"Value1: {Value1}, Value2: {Value2}, UpLimit: {UpLimit}, LowLimit {LowLimit}";
        }
    }
}
