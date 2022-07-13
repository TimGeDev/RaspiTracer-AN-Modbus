using System;
namespace RaspTracer_AN_Modbus.Models
{
    public class EPEverQueryPackage
    {
        public EPEverQueryPackage(int deviceId = 1)
        {

            this.SetDeviceId(deviceId);
        }

        public byte[] byteRequest = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };


        public void SetDeviceId(int Id)
        {
            byte[] byteId = BitConverter.GetBytes(Id);
            SetBytePart(0, byteId.First());
        }

        public void SetFunctionCode(byte functionCode)
        {
            byte[] byteId = BitConverter.GetBytes(functionCode);
            SetBytePart(1, byteId.First());
        }

        public void SetRegisterAddress(byte[] bytes)
        {
            SetBytePart(2, bytes[0]);
            SetBytePart(3, bytes[1]);
        }

        public void SetRegisterCount(byte[] bytes)
        {
            SetBytePart(4, bytes[0]);
            SetBytePart(5, bytes[1]);
        }

        public void SetAdditionalByte(byte additionalByte)
        {
            SetBytePart(6, additionalByte);
        }

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
        public int GetValue()
        {
            return 0;
        }

    }
}

