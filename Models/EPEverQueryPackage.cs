using System;
namespace RaspTracer_AN_Modbus.Models
{
    public class EPEverQueryPackage
    {
        public EPEverQueryPackage(byte[] bytes, byte deviceId = 1)
        {

            SetBytePart(0, deviceId);
            SetBytePart(1, bytes[0]);
            SetBytePart(2, bytes[1]);
            SetBytePart(3, bytes[2]);
            SetBytePart(4, bytes[3]);
            SetBytePart(5, bytes[4]);
        }

        public byte[] byteRequest = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };


        private void SetBytePart(int index, byte byteValue = 0x00)
        {
            byteRequest[index] = byteValue;
        }


        public byte[] GetQueryMessage()
        {
            this.CalculateCRCByte();
            return byteRequest;
        }

        public void CalculateCRCByte()
        {
            var param = new CRC.Parameters("CRC-16/MODBUS", 16, 0x8005, 0xFFFF, true, true, 0x0, 0x4B37);
            var crcClass = new CRC.Crc(param);
            ulong crc = crcClass.ComputeCrc(0xFFFF, byteRequest, 0, 6);
            var result = BitConverter.GetBytes(crc);
            this.SetBytePart(6, result[0]);
            this.SetBytePart(7, result[1]);
        }
        public decimal GetValue(byte[] responseBytes, int factor = 100)
        {
            byte[] holdingRegister = new byte[] { responseBytes[3], responseBytes[4] };
            Console.WriteLine(Logic.EPEverCommunicationHandler.ByteArrayToString(holdingRegister));
            return (decimal)(holdingRegister[0] << 8 | holdingRegister[1]) / factor;
        }

    }
}

