using System;
using MQTTnet;
using System.Text;
using MQTTnet.Diagnostics;
using System.Reflection.Emit;
using System.Threading;
using System.Diagnostics;

namespace MqttClient
{
    internal static class Logger
    {
        private static readonly ClientEntity clientEntity = new ClientEntity();
        private static readonly int logLevel = clientEntity.LogLevel;

        private static string LogLevel2String(int level)
        {
            string str = "";
            switch (level)
            {
                case 0:
                    str = "None";
                    break;
                case 1:
                    str = "Error";
                    break;
                case 2:
                    str = "Warn";
                    break;
                case 3:
                    str = "Info";
                    break;
                case 4:
                    str = "Debug";
                    break;
            }
            return str;
        }

        private static void WriteLog(String message, int level)
        {
            // level越小，优先级越高，LogLevel是过滤掉低优化级日志的基准线
            if (level <= logLevel)
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;
                StackTrace stack = new StackTrace();
                // 0是本身，1是调用方，2是调用方的调用方，以此类推
                System.Reflection.MethodBase method = stack.GetFrame(0).GetMethod();
                string className = method.DeclaringType.Name;
                for (int i = 0; i < stack.FrameCount; i++)
                {
                    // 查找非本类的上一级调用方
                    method = stack.GetFrame(i).GetMethod();
                    if (className != method.DeclaringType.Name)
                    {
                        className = method.DeclaringType.Name;
                        break;
                    }
                }
                Console.WriteLine($"[{DateTime.UtcNow.ToLocalTime():yyyy-MM-dd HH:mm:ss.fff}] [{threadId}] [{className}] [{LogLevel2String(level)}]: {message}");
            }
        }

        public static MqttNetEventLogger getEventLogger()
        {
            if (logLevel >= 4)
            {
                // The logger ID is optional but can be set do distinguish different logger instances.
                MqttNetEventLogger mqttEventLogger = new MqttNetEventLogger("DebugLogger");

                mqttEventLogger.LogMessagePublished += (sender, args) =>
                {
                    var output = new StringBuilder();
                    output.AppendLine($"[{args.LogMessage.Timestamp.ToLocalTime():yyyy-MM-dd HH:mm:ss.fff}] [{args.LogMessage.ThreadId}] [{args.LogMessage.Source}] [{args.LogMessage.Level}]: {args.LogMessage.Message}");
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

        public static void Error(String message)
        {
            WriteLog(message, 1);
        }

        public static void Warn(String message)
        {
            WriteLog(message, 2);
        }

        public static void Info(String message)
        {
            WriteLog(message, 3);
        }

        public static void Debug(String message)
        {
            WriteLog(message, 4);
        }
    }
}