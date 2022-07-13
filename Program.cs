using System.IO.Ports;

namespace RaspTracer_AN_Modbus;
internal class Program
{
    public const bool debug = true;

    public static byte[] BATTERY_VOLTAGE = new byte[] { 0x04, 0x33, 0x1A, 0x00, 0x01 };

    private static async Task Main(string[] args)
    {

        Console.WriteLine("Starting Up....");

        var queryPackage = new Models.EPEverQueryPackage(BATTERY_VOLTAGE);

        byte[] response = Logic.EPEverCommunicationHandler.GetValueFromEPever(queryPackage);

        Console.WriteLine("Result: " + queryPackage.GetValue(response));

        await Logic.MQTTHandler.SendToHassAsync(queryPackage.GetValue(response).ToString().Replace(",", "."));


    }

}