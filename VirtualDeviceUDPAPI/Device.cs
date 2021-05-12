namespace VirtualDeviceUDPAPI
{
    public class Device
    {
        public ushort Value1 { set; get; }
        public ushort Value2 { set; get; }
        public ushort UpLimit { set; get; }
        public ushort LowLimit { set; get; }

        public bool IsWithinLimits => (Value2 <= UpLimit && Value2 >= LowLimit);

        public Device() { }

        public override string ToString()
        {
            return $"Value1: {Value1}, Value2: {Value2}, UpLimit: {UpLimit}, LowLimit: {LowLimit}";
        }
    }
}
