using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttClient
{
    // 设备的连接参数，在设备详情页面即可查看，配置到配置文件的[MqttDevice]小节
    internal class DeviceEntity
    {
        public string ProductId { get => INIHelp.GetString("MqttDevice", "productId"); }

        public string ClientId { get => INIHelp.GetString("MqttDevice", "clientId"); }

        public string DeviceId { get => INIHelp.GetString("MqttDevice", "deviceId");  }

        public string UserName { get => INIHelp.GetString("MqttDevice", "userName"); }

        public string Password { get => INIHelp.GetString("MqttDevice", "password"); }
    }
}
