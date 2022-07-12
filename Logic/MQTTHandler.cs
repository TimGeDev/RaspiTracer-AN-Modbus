using System;
using System.Net;
using System.Text;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json.Linq;

namespace RaspTracer_AN_Modbus.Logic
{
	public class MQTTHandler
    {
        private static string HOST = "";
        private static string USER = "";
        private static string PASS = "";
        private static string TOPIC = "";
        private static string CLIENTID = "";

        public static async Task SendToHassAsync(string payload)
        {
            loadCredentials();

            var mqttFactory = new MqttFactory();
            using var mqttClient = mqttFactory.CreateMqttClient();
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(HOST)
                .WithClientId(CLIENTID)
                .WithCredentials(USER, PASS)
                .Build();

            await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(TOPIC)
                .WithPayload(payload)
                .Build();

            await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

            Console.WriteLine("MQTT application message is published.");

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
    }
}

