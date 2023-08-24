using System;

namespace MqttClient
{
    internal static class Logger
    {
        private static readonly ClientEntity clientEntity =new ClientEntity();
        private static readonly int logLevel = clientEntity.LogLevel;

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