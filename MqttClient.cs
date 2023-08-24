using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Authentication;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace MqttClient
{
    internal class MqttClient
    {
        // 设备配置参数：客户端id、用户名、密码等
        private readonly DeviceEntity device=new DeviceEntity();
        // 服务器配置参数：地址、端口、证书等
        private readonly ServerEntity server=new ServerEntity();
        // 客户端配置参数：连接超时、重连时间、日志级别等
        private readonly ClientEntity client=new ClientEntity();
        // mqtt客户端
        private readonly MqttFactory mqttFactory = new MqttFactory();
        private readonly IMqttClient mqttClient;
        private readonly MqttClientOptionsBuilder mqttClientOptionsbuilder;

        // 自实现的服务器证书验证类
        //This is a helper class to allow verifying a root CA separately from the Windows root store
        private readonly RootCertificateTrust rootCertificateTrust;

        public MqttClient() {
            mqttClient = mqttFactory.CreateMqttClient();
            mqttClient.ApplicationMessageReceivedAsync += ProcessMessage;
            mqttClientOptionsbuilder = new MqttClientOptionsBuilder()
                .WithCleanSession(true)
                .WithTcpServer(server.Adrress,server.Port)
                .WithClientId(device.ClientId)
                .WithCredentials(device.UserName,device.Password)
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(60));

            //ssl相关配置：只有配置有服务端效证书时使用ssl连接
            rootCertificateTrust = server.CertificateTrust;
            if (rootCertificateTrust != null) {
                //mqttClientOptionsbuilder.WithTlsOptions(
                //    o =>
                //    {
                //        o.UseTls(true);
                //        o.WithAllowUntrustedCertificates(false);
                //        o.WithIgnoreCertificateChainErrors(false);
                //        o.WithIgnoreCertificateRevocationErrors(true);
                //        o.WithCertificateValidationHandler(rootCertificateTrust.VerifyServerCertificate);
                //        // The default value is determined by the OS. Set manually to force version.
                //        o.WithSslProtocols(SslProtocols.Tls12);
                //        o.WithRevocationMode(X509RevocationMode.NoCheck);


                //    });
                mqttClientOptionsbuilder.WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    UseTls = true,
                    AllowUntrustedCertificates = false,
                    IgnoreCertificateChainErrors = false,
                    IgnoreCertificateRevocationErrors = true,
                    CertificateValidationHandler = rootCertificateTrust.VerifyServerCertificate,
                });
            }
        }

        // 使用单独线程周期性检查连接是否正常，如果不正常则重连，检查周期为client.ReconnectInterval
        public void Start()
        {
            /*
             * This sample shows how to reconnect when the connection was dropped.
             * This approach uses a custom Task/Thread which will monitor the connection status.
             * This is the recommended way but requires more custom code!
             */
            _ = Task.Run(
                async () =>
                {
                    CancellationTokenSource cts;
                    // User proper cancellation and no while(true).
                    while (true)
                    {
                        try
                        {
                            cts = new CancellationTokenSource(TimeSpan.FromSeconds(client.ConnectTimeout));
                            // This code will also do the very first connect! So no call to _ConnectAsync_ is required in the first place.
                            if (!await mqttClient.TryPingAsync(cts.Token))
                            {
                                Logger.Info("The MQTT client is connecting ...");
                                cts = new CancellationTokenSource(TimeSpan.FromSeconds(client.ConnectTimeout));
                                await mqttClient.ConnectAsync(mqttClientOptionsbuilder.Build(), cts.Token);
                                Logger.Info("The MQTT client is connected.");
                                // Subscribe to topics when session is clean etc.
                                await SubscribedAsyncs($"{device.ProductId}/in/{device.DeviceId}");
                            }
                            else
                            {
                                Logger.Debug("connection is ok");
                            }
                        }
                        catch (OperationCanceledException) {
                            Logger.Error($"connect timeout to server: {server.Adrress}:{server.Port}");
                        }
                        catch (Exception ex)
                        {
                            // Handle the exception properly (logging etc.).
                            Logger.Error("connect error: " + ex);
                        }
                        finally
                        {
                            // Check the connection state every 5 seconds and perform a reconnect if required.
                            Logger.Debug($"waiting for {client.ReconnectInterval} seconds ...");
                            await Task.Delay(TimeSpan.FromSeconds(client.ReconnectInterval));
                        }
                    }
                });
        }

        // 订阅topic
        private async Task SubscribedAsyncs(string topic) {
            try {
                await mqttClient.SubscribeAsync(topic);
                Logger.Info($"subscribe success: topic={topic}");
            }catch(Exception ex) {
                Logger.Warn("error while subscribing:"+ex);
            }
        }

        // 处理接收到的消息：业务逻辑可放在些处
        private Task ProcessMessage(MqttApplicationMessageReceivedEventArgs arg) {
            try 
            {
                string topic = arg.ApplicationMessage.Topic;
                string msg = arg.ApplicationMessage.ConvertPayloadToString();
                Logger.Info($"message received: topic={topic},message={msg}");

                string replyTopic = $"{device.ProductId}/out/{device.DeviceId}";
                string replyMsg = "hello";
                Logger.Info($"replying message: topic={replyTopic},message={replyMsg}");

                // Publishing messages inside that received messages handler requires to use Task.Run when using a QoS > 0.
                // The reason is that the message handler has to finish first before the next message is received.
                // The reason is to preserve ordering of the application messages.
                // 在消息处理方法中发送消息，建议在线程池中运行
                Task.Run(() => PublishMessageAsyncs(replyTopic, replyMsg));
            }
            catch(Exception ex)
            {
                Logger.Error("error while processing income message:"+ex);
            }
            return Task.CompletedTask;
        }

        // 发送消息：超时时间为client.ConnectTimeout
        private async Task PublishMessageAsyncs(string topic, string message) {
            try
            {
                var msg=new MqttApplicationMessageBuilder()
                    .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                    .WithTopic(topic)
                    .WithPayload(message)
                    .Build();
                CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(client.ConnectTimeout));
                await mqttClient.PublishAsync(msg,cts.Token);
                Logger.Info($"message published: topic={topic},message={message}");
            }
            catch (Exception ex)
            {
                Logger.Error("error while publishing message:" + ex);
            }
        }
    }
}
