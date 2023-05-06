using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json.Linq;
using RaspTracer_AN_Modbus.Logic;

namespace RaspTracer_AN_Modbus.Services
{
    public class MQTTService
    {
        private static string HOST = "";
        private static string USER = "";
        private static string PASS = "";
        private static string TOPIC = "";
        private static string CLIENTID = "";
        private readonly ILogger<MQTTService> logger;
        private readonly IMqttClient mqttClient;
        private MqttClientOptions mqttClientOptions;

        public MQTTService(ILogger<MQTTService> logger)
        {
            this.logger = logger;
            var mqttFactory = new MqttFactory();
            mqttClient = mqttFactory.CreateMqttClient();

            loadCredentials();

            mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(HOST)
                .WithClientId(CLIENTID)
                .WithCredentials(USER, PASS)
                .Build();
        }

        private void Connect()
        { 
            mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
        }
        internal void Disconnect()
        {
            mqttClient.DisconnectAsync();
        }

        public async Task SendToHassAsync(string payload)
        {
            if (!mqttClient.IsConnected)
            {
                Connect();
            }
            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(TOPIC)
                .WithPayload(payload)
                .Build();

            await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

            logger.LogInformation("Published: " + payload);

        }

        private static void loadCredentials()
        {
            using StreamReader r = new StreamReader("mqtt.json");
            string json = r.ReadToEnd();
            dynamic credentials = JObject.Parse(json);
            HOST = credentials.mqttHost;
            USER = credentials.mqttUser;
            PASS = credentials.mqttPass;
            TOPIC = credentials.mqttTopic;
            CLIENTID = credentials.mqttClientid;
            var t = "";
        }

        public void Dispose()
        {
            Disconnect();
        }

    }
}

