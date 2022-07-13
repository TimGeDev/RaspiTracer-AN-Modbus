using System.IO.Ports;
using Newtonsoft.Json;

namespace RaspTracer_AN_Modbus;
internal class Program
{
    public const bool debug = true;
    public static readonly Dictionary<string, byte[]> parametersToRetrieve = new Dictionary<string, byte[]>
        {
            { "LOAD_VOLTAGE", new byte[] { 0x04, 0x31, 0x0C, 0x00, 0x01 } },
            { "LOAD_CURRENT", new byte[] { 0x04, 0x31, 0x0D, 0x00, 0x01 } },
            { "SOLAR_VOLTAGE", new byte[] { 0x04, 0x31, 0x00, 0x00, 0x01 } },
            { "SOLAR_CURRENT", new byte[] { 0x04, 0x31, 0x01, 0x00, 0x01 } },
            { "BATTERY_VOLTAGE", new byte[] { 0x04, 0x33, 0x1A, 0x00, 0x01 } },
           // { "BATTERY_AMPERAGE", new byte[] { 0x04, 0x33, 0x1A, 0x00, 0x01 } },
            { "BATTERY_TEMPERATURE", new byte[] { 0x04, 0x33, 0x10, 0x00, 0x01 } },
            { "DEVICE_TEMPERATURE", new byte[] { 0x04, 0x31, 0x11, 0x00, 0x01 } },
        };

    private static async Task Main(string[] args)
    {


        Console.WriteLine("Starting Up....");

        Logic.EPEverCommunicationHandler.OpenConnection();

        string values = RetrieveParameters();
        Console.WriteLine("Reporting values: " + values);

        await Logic.MQTTHandler.SendToHassAsync(values);

        Logic.EPEverCommunicationHandler.CloseConnection();
    }

    private static string RetrieveParameters()
    {
        var results = new Dictionary<string, string>();

        foreach (var param in parametersToRetrieve)
        {
            var queryPackage = new Models.EPEverQueryPackage(param.Value);
            byte[] response = Logic.EPEverCommunicationHandler.GetValueFromEPever(queryPackage);
            string value = queryPackage.GetValue(response).ToString().Replace(",", ".");
            results.Add(param.Key, value);
        }
        return JsonConvert.SerializeObject(results, Formatting.None);

    }

}