namespace VirtualDeviceUDP
{
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
}
