using System;
using MQTTnet;
using System.Text;
using MQTTnet.Diagnostics;

namespace MqttClient
{
    internal static class Logger
    {
        private static readonly ClientEntity clientEntity =new ClientEntity();
        private static readonly int logLevel = clientEntity.LogLevel;

        public static MqttNetEventLogger getEventLogger() {
            if (logLevel >= 4)
            {
                // The logger ID is optional but can be set do distinguish different logger instances.
                MqttNetEventLogger mqttEventLogger = new MqttNetEventLogger("DebugLogger");

                mqttEventLogger.LogMessagePublished += (sender, args) =>
                {
                    var output = new StringBuilder();
                    output.AppendLine($">> [{args.LogMessage.Timestamp:O}] [{args.LogMessage.ThreadId}] [{args.LogMessage.Source}] [{args.LogMessage.Level}]: {args.LogMessage.Message}");
                    if (args.LogMessage.Exception != null)
                    {
                        output.AppendLine(args.LogMessage.Exception.ToString());
                    }

                    Console.Write(output);
                };
                return mqttEventLogger;
            }
            return null;
        }

        public static void Debug(String message)
        {
            if (logLevel >= 4)
            {
                DateTime now = DateTime.Now;
                Console.WriteLine($"[{now}] [DEBUG] {message}");
            }

        }

        public static void Error(String message)
        {
            if (logLevel >= 1)
            {
                DateTime now = DateTime.Now;
                Console.WriteLine($"[{now}] [ERROR] {message}");
            }
        }

        public static void Info(String message)
        {
            if (logLevel >= 3)
            {
                DateTime now = DateTime.Now;
                Console.WriteLine($"[{now}] [INFO] {message}");
            }
        }

        public static void Warn(String message)
        {
            if (logLevel >= 2)
            {
                DateTime now = DateTime.Now;
                Console.WriteLine($"[{now}] [WARN] {message}");
            }
        }
    }
}