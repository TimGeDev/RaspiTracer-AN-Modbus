using System;
namespace RaspTracer_AN_Modbus.Models
{
	public class EPEveryBatteryVoltageQuery: EPEverQueryPackage
	{
		public EPEveryBatteryVoltageQuery()
        {
            this.SetFunctionCode(0x04);
            this.SetRegisterAddress(new byte[] { 0x31, 0x04 });
            this.SetRegisterCount(new byte[] { 0x00, 0x01 });
        }

        public static decimal GetValue(byte[] responseBytes)
        {
            byte[] holdingRegister = new byte[] { responseBytes[3], responseBytes[4] };
            Console.WriteLine(Logic.EPEverCommunicationHandler.ByteArrayToString(holdingRegister));
            return (decimal) (holdingRegister[0] << 8 | holdingRegister[1]) / 100;
        }

    }
}

