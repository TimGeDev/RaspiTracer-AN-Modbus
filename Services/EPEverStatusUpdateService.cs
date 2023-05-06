using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RaspTracer_AN_Modbus.Logic;
using RaspTracer_AN_Modbus.Models;

namespace RaspTracer_AN_Modbus.Services
{
    public class EPEverStatusUpdateService : IHostedService, IDisposable
    {
        private readonly ILogger<EPEverStatusUpdateService> logger;
        private readonly EPEverIOConnector iOConnector;
        private readonly MQTTService mqqtService;
        private Timer? timer = null;

        public EPEverStatusUpdateService(ILogger<EPEverStatusUpdateService> logger, EPEverIOConnector iOConnector, MQTTService mqqtService)
        {
            this.logger = logger;
            this.iOConnector = iOConnector;
            this.mqqtService = mqqtService;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("EPEverStatusUpdateService is starting");
            this.timer = new Timer(RetrieveStatusInformation, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));

            return Task.CompletedTask;
        }

        private void RetrieveStatusInformation(object? state)
        {

            Console.WriteLine("Starting Up....");

            var results = new Dictionary<string, string>();

            foreach (var param in AvailableParameters.parametersToRetrieve)
            {
                var value = RetrieveParameter(param);
                results.Add(param.Name, value);
                Thread.Sleep(100);
            }

            var stringResults = JsonConvert.SerializeObject(results);

             _ = mqqtService.SendToHassAsync(stringResults);


            logger.LogInformation("Reporting values: " + stringResults);


            this.logger.LogInformation("work has been done");
        }

        private string RetrieveParameter(EPEverQuery param)
        {
            var queryPackage = new EPEverQueryPackage(param.ByteArray);
            byte[] response = this.iOConnector.GetValueFromEPever(queryPackage);

            byte[] responseBytes = new byte[] { response[3], response[4] };

            if (param.HasHighRegister)
            {
                byte[] byteArrayHigh = param.ByteArray;
                byteArrayHigh[2] = (byte)(byteArrayHigh[2] + 1);
                var queryPackageHigh = new EPEverQueryPackage(byteArrayHigh);
                byte[] responseHigh = this.iOConnector.GetValueFromEPever(queryPackageHigh);
                responseBytes = new byte[] { responseHigh[3], responseHigh[4], response[3], response[4] };
            }

            if (BitConverter.IsLittleEndian)
                Array.Reverse(responseBytes);

            if (param.HasHighRegister)
            {
                return ((float)BitConverter.ToInt32(responseBytes, 0) / param.Factor).ToString().Replace(",", ".");
            }
            else
            {
                return ((float)BitConverter.ToUInt16(responseBytes, 0) / param.Factor).ToString().Replace(",", ".");
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Timed Hosted Service is stopping.");

            timer?.Change(Timeout.Infinite, 0);

            mqqtService.Disconnect();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}

