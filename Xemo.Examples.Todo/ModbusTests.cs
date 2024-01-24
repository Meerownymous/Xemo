using System;
using NModbus.Device;
using System.Net.Sockets;
using Xunit;
using NModbus;
using System.Diagnostics;

namespace Xemo.Examples.Todo
{
    public sealed class ModbusTests
    {
        [Fact]
        public void Reads()
        {
            using (TcpClient client = new TcpClient("192.168.36.184", 502))
            {
                var factory = new ModbusFactory();
                IModbusMaster master = factory.CreateMaster(client);

                // read five input values
                ushort startAddress = 3000;
                ushort numInputs = 7;
                ushort[] inputs = master.ReadInputRegisters(20, startAddress, numInputs);

                for (int i = 0; i < numInputs; i++)
                {
                    Debug.WriteLine($"Input {(startAddress + i)}={(inputs[i])}");
                }
            }
        }
    }
}

