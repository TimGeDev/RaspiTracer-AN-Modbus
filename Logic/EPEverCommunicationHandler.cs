using System;
using System.IO.Ports;

namespace RaspTracer_AN_Modbus.Logic
{
    public class EPEverCommunicationHandler
    {
        public const int TimeoutMs = 2000;
        public const string TTYPath = "/dev/ttyUSB0";
        public const int BaudRate = 115200;
        private static SerialPort SerialPort;


        public static void OpenConnection()
        {
            SerialPort = new SerialPort(TTYPath, BaudRate);
            SerialPort.Open();
        }
        public static void CloseConnection()
        {
            SerialPort.Close();
        }

        public static Byte[] GetValueFromEPever(Models.EPEverQueryPackage queryPackage)
        {

            if (Program.debug)
            {
                Console.WriteLine("Write: " + ByteArrayToString(queryPackage.GetQueryMessage()));
            }
            
            SerialPort.Write(queryPackage.GetQueryMessage(), 0, 8);

            long startMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            while (SerialPort.BytesToRead == 0)
            {
                long currentMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                if ((startMillis + TimeoutMs) < currentMillis)
                {
                    Console.WriteLine("Timeout reached");
                    return Array.Empty<byte>();
                }
                Thread.Sleep(50);
            }

            byte[] buffer = new byte[SerialPort.BytesToRead];
            SerialPort.Read(buffer, 0, buffer.Length);

            if (Program.debug)
            {
                Console.WriteLine("Result:" + ByteArrayToString(buffer));
            }

            return buffer;
        }
        public static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

    }
}

