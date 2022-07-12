using System;
using System.IO.Ports;

namespace RaspTracer_AN_Modbus.Logic
{
	public class EPEverCommunicationHandler
    { 
        public const int TimeoutMs = 1000;
        public const string TTYPath = "/dev/ttyUSB0";
        public const int BaudRate = 115200;


        public static Byte[] GetValueFromEPever(Models.EPEverQueryPackage queryPackage)
        {
            var port = new SerialPort(TTYPath, BaudRate);
            port.Open();

            if (Program.debug)
            {
                Console.WriteLine("Write: " + ByteArrayToString(queryPackage.GetQueryMessage()));
            }

            port.Write(queryPackage.GetQueryMessage(), 0, 8);

            long startMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            while (port.BytesToRead == 0)
            {
                long currentMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                if ((startMillis + TimeoutMs) > currentMillis)
                {
                    Console.WriteLine("Timeout reached");
                    return Array.Empty<byte>();
                }
                Thread.Sleep(10);
            }

            byte[] buffer = new byte[port.BytesToRead];
            port.Read(buffer, 0, buffer.Length);
            port.Close();

            if (Program.debug)
            {
                Console.WriteLine("Result:"  + ByteArrayToString(buffer));
            }

            return buffer;
        }
        public static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }
    }
}

