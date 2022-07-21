using RaspTracer_AN_Modbus.Models;
using Newtonsoft.Json;
using RaspTracer_AN_Modbus.Logic;
using System.Numerics;
using System.Text;
using System.Collections;

namespace RaspTracer_AN_Modbus;
internal class Program
{
    public const bool debug = false;
    public static readonly List<EPEverQuery> parametersToRetrieve = new(){
        new EPEverQuery() { Name = "LOAD_VOLTAGE", ByteArray = new byte[] { 0x04, 0x31, 0x0C, 0x00, 0x01 }, Factor = 100, HasHighRegister = false },
        new EPEverQuery() { Name = "LOAD_CURRENT", ByteArray = new byte[] { 0x04, 0x31, 0x0D, 0x00, 0x01 }, Factor = 100, HasHighRegister = false },
        new EPEverQuery() { Name = "BATTERY_VOLTAGE", ByteArray = new byte[] { 0x04, 0x33, 0x1A, 0x00, 0x01 }, Factor = 100, HasHighRegister = false },
        new EPEverQuery() { Name = "BATTERY_CURRENT", ByteArray = new byte[] { 0x04, 0x33, 0x1B, 0x00, 0x01 }, Factor = 100, HasHighRegister = false }, // It should have one, but it only returns 0xFFFF
        new EPEverQuery() { Name = "SOLAR_VOLTAGE", ByteArray = new byte[] { 0x04, 0x31, 0x00, 0x00, 0x01 }, Factor = 100, HasHighRegister = false },
        new EPEverQuery() { Name = "SOLAR_CURRENT", ByteArray = new byte[] { 0x04, 0x31, 0x01, 0x00, 0x01 }, Factor = 100, HasHighRegister = false },
        new EPEverQuery() { Name = "BATTERY_TEMPERATURE", ByteArray = new byte[] { 0x04, 0x31, 0x10, 0x00, 0x01 }, Factor = 100, HasHighRegister = false },
        new EPEverQuery() { Name = "DEVICE_TEMPERATURE", ByteArray = new byte[] { 0x04, 0x31, 0x11, 0x00, 0x01 }, Factor = 100, HasHighRegister = false },
        new EPEverQuery() { Name = "BATTERY_SOC", ByteArray = new byte[] { 0x04, 0x31, 0x1A, 0x00, 0x01 }, Factor = 1, HasHighRegister = false },
        new EPEverQuery() { Name = "TOTAL_GENERATED_ENERGY", ByteArray = new byte[] { 0x04, 0x33, 0x12, 0x00, 0x01 }, Factor = 100, HasHighRegister = true },
        new EPEverQuery() { Name = "TOTAL_GENERATED_ENERGY_YEAR", ByteArray = new byte[] { 0x04, 0x33, 0x10, 0x00, 0x01 }, Factor = 100, HasHighRegister = true },
        new EPEverQuery() { Name = "TOTAL_GENERATED_ENERGY_MONTH", ByteArray = new byte[] { 0x04, 0x33, 0x0E, 0x00, 0x01 }, Factor = 100, HasHighRegister = true },
        new EPEverQuery() { Name = "TOTAL_GENERATED_ENERGY_TODAY", ByteArray = new byte[] { 0x04, 0x33, 0x0C, 0x00, 0x01 }, Factor = 100, HasHighRegister = true },
};



    private static async Task Main(string[] args)
    {

        if (args.Length > 0 && args[0] == "--brute-force")
        {
            BruteForceWorkingReadRegisters();
            return;
        }

        Console.WriteLine("Starting Up....");

        EPEverCommunicationHandler.OpenConnection();

        string values = RetrieveParameters();
        Console.WriteLine("Reporting values: " + values);

        await MQTTHandler.SendToHassAsync(values);

        EPEverCommunicationHandler.CloseConnection();
    }

    private static void BruteForceWorkingReadRegisters()
    {
        EPEverCommunicationHandler.OpenConnection();
        for (int a0 = 48; a0 < 52; a0++)
        {
            for (int a1 = 0; a1 < 256; a1++)
            {
                byte[] a0Bytes = BitConverter.GetBytes(a0);
                byte[] a1Bytes = BitConverter.GetBytes(a1);
                var address = new byte[] { 0x04, a0Bytes.First(), a1Bytes.First(), 0x00, 0x01 };
                try
                {
                    var queryPackage = new EPEverQueryPackage(address);
                    byte[] response = EPEverCommunicationHandler.GetValueFromEPever(queryPackage);

                    if (EPEverCommunicationHandler.ByteArrayToString(response) != "018402C2C100" && EPEverCommunicationHandler.ByteArrayToString(response).Substring(7, 4) != "2C10")
                    {
                        Console.WriteLine("Address: " + EPEverCommunicationHandler.ByteArrayToString(new byte[] { a0Bytes.First(), a1Bytes.First() })
                            + "; Response: " + EPEverCommunicationHandler.ByteArrayToString(response).Substring(7, 4));
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
        EPEverCommunicationHandler.CloseConnection();

    }

    private static string RetrieveParameters()
    {
        var results = new Dictionary<string, string>();

        foreach (var param in parametersToRetrieve)
        {
            var value = RetrieveParameter(param);
            results.Add(param.Name, value);
            Thread.Sleep(100);
        }
        return JsonConvert.SerializeObject(results, Formatting.None);

    }

    private static string RetrieveParameter(EPEverQuery param)
    {
        var queryPackage = new EPEverQueryPackage(param.ByteArray);
        byte[] response = EPEverCommunicationHandler.GetValueFromEPever(queryPackage);

        byte[] responseBytes = new byte[] { response[3], response[4] };

        if (param.HasHighRegister)
        {
            byte[] byteArrayHigh = param.ByteArray;
            byteArrayHigh[2] = (byte)(byteArrayHigh[2] + 1);
            var queryPackageHigh = new EPEverQueryPackage(byteArrayHigh);
            byte[] responseHigh = EPEverCommunicationHandler.GetValueFromEPever(queryPackageHigh);
            responseBytes = new byte[] { responseHigh[3], responseHigh[4], response[3], response[4] };
        }

        if (BitConverter.IsLittleEndian)
            Array.Reverse(responseBytes);

        if (param.HasHighRegister)
        {
            return ((float)BitConverter.ToUInt32(responseBytes, 0) / param.Factor).ToString().Replace(",", ".");
        }
        else
        {
            return ((float)BitConverter.ToUInt16(responseBytes, 0) / param.Factor).ToString().Replace(",", ".");
        }

    }

}