using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MqttClient
{
    // 客户端的配置参数，配置到配置文件的[MqttClient]小节
    internal class ClientEntity
    {
        private readonly double minTime = 5;
        private readonly double defaultTime = 15;


        // 连接检查时间间隔，最小值/默认值为5秒
        public double ReconnectInterval
        {
            get
            {
                double d = INIHelp.GetDouble("MqttClient", "reconnectInterval");
                if (d >= minTime)
                {
                    return d;
                }
                Logger.Warn($"reconnectInterval={d} from ini file is too small, use default value: {defaultTime}");
                return defaultTime;
            }
        }

        //连接、重连、发布、订阅等操作的超时时间，最小值/默认值为5秒
        public double ConnectTimeout
        {
            get
            {
                double d = INIHelp.GetDouble("MqttClient", "connectTimeout");
                if (d >= minTime)
                {
                    return d;
                }
                Logger.Warn($"connectTimeout={d} from ini file is too small, use default value: {defaultTime}");
                return defaultTime;
            }
        }

        // 日志输出级别，默认值为 debug
        public int LogLevel
        {
            get
            {
                string str = INIHelp.GetString("MqttClient", "logLevel").ToLower();
                int logLevel = 4;
                if (str == "none")
                {
                    logLevel = 0;
                }
                else if (str == "error")
                {
                    logLevel = 1;
                }
                else if (str == "warn")
                {
                    logLevel = 2;
                }
                else if (str == "info")
                {
                    logLevel = 3;
                }
                else if (str == "debug")
                {
                    logLevel = 4;
                }
                return logLevel;
            }
        }
    }
}
