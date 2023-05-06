using RaspTracer_AN_Modbus.Models;
using Newtonsoft.Json;
using RaspTracer_AN_Modbus.Logic;
using RaspTracer_AN_Modbus.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace RaspTracer_AN_Modbus;
internal class Program
{
    public const bool debug = false;

    private static async Task Main(string[] args)
    {
        var host = new HostBuilder()
            .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddEnvironmentVariables(prefix: "env_");
                    configHost.AddCommandLine(args);
                })

            .ConfigureAppConfiguration(configApp =>
                {
                    configApp.SetBasePath(Directory.GetCurrentDirectory());
                    configApp.AddJsonFile("appsettings.json", optional: true);
                    configApp.AddEnvironmentVariables(prefix: "env_");
                    configApp.AddCommandLine(args);
                })

            .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    services.AddSingleton<EPEverIOConnector>();
                    services.AddSingleton<MQTTService>();
                    services.AddHostedService<EPEverStatusUpdateService>();

                })

            .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                })

          .UseConsoleLifetime()
          .Build();

        await host.RunAsync();
    }


    //private static void BruteForceWorkingReadRegisters()
    //{
    //    EPEverIOConnector.OpenConnection();
    //    for (int a0 = 48; a0 < 52; a0++)
    //    {
    //        for (int a1 = 0; a1 < 256; a1++)
    //        {
    //            byte[] a0Bytes = BitConverter.GetBytes(a0);
    //            byte[] a1Bytes = BitConverter.GetBytes(a1);
    //            var address = new byte[] { 0x04, a0Bytes.First(), a1Bytes.First(), 0x00, 0x01 };
    //            try
    //            {
    //                var queryPackage = new EPEverQueryPackage(address);
    //                byte[] response = EPEverIOConnector.GetValueFromEPever(queryPackage);

    //                if (EPEverIOConnector.ByteArrayToString(response) != "018402C2C100" && EPEverIOConnector.ByteArrayToString(response).Substring(7, 4) != "2C10")
    //                {
    //                    Console.WriteLine("Address: " + EPEverIOConnector.ByteArrayToString(new byte[] { a0Bytes.First(), a1Bytes.First() })
    //                        + "; Response: " + EPEverIOConnector.ByteArrayToString(response).Substring(7, 4));
    //                }
    //            }
    //            catch (Exception ex)
    //            {

    //            }
    //        }
    //    }
    //    EPEverIOConnector.CloseConnection();

    //}



}