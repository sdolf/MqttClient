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
            get => INIHelp.GetDouble("MqttClient", "reconnectInterval")> minTime ? INIHelp.GetDouble("MqttClient", "reconnectInterval"): defaultTime;
        }

        //连接、重连、发布、订阅等操作的超时时间，最小值/默认值为5秒
        public double ConnectTimeout
        {
            get => INIHelp.GetDouble("MqttClient", "connectTimeout") > minTime ? INIHelp.GetDouble("MqttClient", "connectTimeout") : defaultTime;
        }

        // 日志输出级别，默认值为info
        public int LogLevel 
        { 
            get 
            {
                string str = INIHelp.GetString("MqttClient", "logLevel").ToLower();
                int logLevel=3;
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
