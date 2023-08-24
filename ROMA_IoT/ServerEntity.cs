using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MqttClient
{
    // 服务器的连接参数，在设备详情页面即可查看，配置到配置文件的[MqttServer]小节，证书文件名可以配置多个，用分号或逗号分开
    // 证书信息在wiki中可以找到，注：c#语言只验证根证书，中间证书和服务器证书没有用，如需要使用非ssl连接，保持证书配置项为空即可
    internal class ServerEntity
    {
        public string Adrress 
        { 
            get => String.IsNullOrEmpty(INIHelp.GetString("MqttServer", "address")) ? "127.0.0.1" : INIHelp.GetString("MqttServer", "address"); 
        }

        public int Port 
        { 
            get => INIHelp.GetInt("MqttServer", "port") < 1000 ? 1883 : INIHelp.GetInt("MqttServer", "port");
        }

        // 配置的所有信任的CA根证书文件
        public string[] CaCertificates 
        { 
            get 
            {
                string caCertificatePaths = INIHelp.GetString("MqttServer", "caCertificates");
                string[] caCertificatePathsArr = caCertificatePaths.Split(",;".ToCharArray());
                return caCertificatePathsArr;
            } 
        }

        // 把所有CA根证书文件读进来用做证书链验证
        public RootCertificateTrust CertificateTrust
        {
            get
            {
                RootCertificateTrust rootCertificateTrust = new RootCertificateTrust();
                bool isTls = false;
                foreach (string caCertificatePath in CaCertificates)
                {
                    if (File.Exists(caCertificatePath))
                    {
                        // Load the Certificate
                        X509Certificate2 caCrt = new X509Certificate2(File.ReadAllBytes(caCertificatePath));
                        rootCertificateTrust.AddCert(caCrt);
                        isTls = true;
                        Logger.Debug($"trust CA certificate loaded from file: {caCertificatePath}");
                    }
                    else 
                    {
                        Logger.Warn($"trust CA certificate file not exist: {caCertificatePath}");
                    }
                }
                if (isTls) 
                {
                    return rootCertificateTrust;
                }
                else
                {
                    return null;
                }
            }
        }

    }
}
