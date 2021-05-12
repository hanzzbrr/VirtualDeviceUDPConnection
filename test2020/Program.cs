using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VirtualDeviceUDPAPI;

namespace VirtualDeviceUDP
{

    public class Program
    {
        private static UdpNetworkManager _udpManager;

        private const int _refresDelayMs = 3000;

        private static bool _lockOutput;        

        public static void Main()
        {
            _lockOutput = false;

            _udpManager = new UdpNetworkManager();

            _ = OutDeviteStateAsync();

            ConsoleKey consoleKey;
            do
            {
                consoleKey = Console.ReadKey().Key;
                if (consoleKey == ConsoleKey.W)
                {
                    _lockOutput = true;
                    CreateWriteRequest();
                    _lockOutput = false;
                }

            } while (consoleKey != ConsoleKey.X);
        }

        private static async Task OutDeviteStateAsync()
        {
            while (true)
            {
                await Task.Delay(_refresDelayMs);
                if (_lockOutput)
                {
                    continue;
                }
                foreach (var d in _udpManager.Devices)
                {
                    Console.Write($"id: {d.Key}, Value1: {d.Value.Value1}, ");
                    Console.ForegroundColor = d.Value.IsWithinLimits ? ConsoleColor.White : ConsoleColor.Red;
                    Console.Write($"Value2: {d.Value.Value2}, ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"UpperLimit: {d.Value.UThreshold}, BottomLimit: {d.Value.BThreshold}");
                }
                Console.WriteLine();
            }
        }

        private static void CreateWriteRequest()
        {
            var rg = new Regex(@"()([1-9]|[1-5]?[0-9]{2,4}|6[1-4][0-9]{3}|65[1-4][0-9]{2}|655[1-2][0-9]|6553[1-5]) \b(1?[0-9]{1,2}|2[0-4][0-9]|25[0-5])\b \b(1?[0-9]{1,2}|2[0-4][0-9]|25[0-5])\b");
            string input = "";
            do
            {
                Console.WriteLine("Enter request parameteres in format: <Id> <UpperLimit> <BottomLimit>, Example: 1 225 25");
                input = Console.ReadLine();
            } while (!rg.IsMatch(input));

            _udpManager.CreateWriteRequest(input.Split(' '));
        }
    }
}