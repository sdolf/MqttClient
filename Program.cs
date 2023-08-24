using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Logger.Info("starting ...");
            MqttClient mqttClient=new MqttClient();
            mqttClient.Start();
            Logger.Info("started");
            Logger.Info("enter 'exit' or 'quit' to end");
            string cmd;
            do { 
                cmd = Console.ReadLine().ToLower(); 
            }while(cmd!="exit" && cmd!="quit");
        }
    }
}
