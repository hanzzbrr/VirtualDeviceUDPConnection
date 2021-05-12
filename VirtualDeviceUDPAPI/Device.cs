namespace VirtualDeviceUDPAPI
{
    public class Device : IDevice
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

    public interface IDevice
    {
        ushort Value1 { set; get; }
        ushort Value2 { set; get; }
        ushort UpLimit { set; get; }
        ushort LowLimit { set; get; }
    }
}
