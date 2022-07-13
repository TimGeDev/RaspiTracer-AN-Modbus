using System.IO.Ports;

namespace RaspTracer_AN_Modbus;
internal class Program
{
    public const bool debug = true;


    private static async Task Main(string[] args)
    {
        Console.WriteLine("Starting Up....");

        var queryPackage = new Models.EPEveryBatteryVoltageQuery();

        byte[] response = Logic.EPEverCommunicationHandler.GetValueFromEPever(queryPackage);

        Console.WriteLine("Result: " + queryPackage.GetValue(response));


        await Logic.MQTTHandler.SendToHassAsync(queryPackage.GetValue(response).ToString().Replace(",", "."));


    }

}