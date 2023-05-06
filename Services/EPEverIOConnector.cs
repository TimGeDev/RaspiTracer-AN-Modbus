using System;
using System.IO.Ports;

namespace RaspTracer_AN_Modbus.Logic
{
    public class EPEverIOConnector
    {
        private const int TimeoutMs = 2000;
        private const string TTYPath = "/dev/ttyUSB0";
        private const int BaudRate = 115200;
        private readonly SerialPort SerialPort;
        private object CLock = new Object();

        public EPEverIOConnector()
        {
            SerialPort = new SerialPort(TTYPath, BaudRate);
            SerialPort.Open();
        }

        public void CloseConnection()
        {
            SerialPort.Close();
        }

        public Byte[] GetValueFromEPever(Models.EPEverQueryPackage queryPackage)
        {
            byte[] buffer;


            if (Program.debug)
            {
                Console.WriteLine("Write: " + ByteArrayToString(queryPackage.GetQueryMessage()));
            }

            lock (CLock)
            {

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

                buffer = new byte[SerialPort.BytesToRead];
                SerialPort.Read(buffer, 0, buffer.Length);

            }

            if (Program.debug)
            {
                Console.WriteLine("Result:" + ByteArrayToString(buffer));
            }
            return buffer;
        }
        public string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }
    }
}

