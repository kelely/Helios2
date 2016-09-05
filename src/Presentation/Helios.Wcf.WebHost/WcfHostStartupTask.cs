using System.ServiceModel;
using System.ServiceModel.Description;
using Autofac.Integration.Wcf;
using Common.Logging;
using Helios.Wcf.WebHost.Behaviors;

// ReSharper disable once CheckNamespace
namespace Helios.Infrastructure.Wcf.WebHost
{
    public class AutofacHostStartupTask : IStartupTask
    {
        private static readonly ILog Logger = LogManager.GetLogger<AutofacHostStartupTask>();

        public void Execute()
        {
            Logger.Debug("配置 Autofac.Wcf 对象容器");

            AutofacHostFactory.HostConfigurationAction = HostConfiguration;

            // 设置 wcf 服务的容器
            AutofacHostFactory.Container = EngineContext.Current.ContainerManager.Container;
        }

        private void HostConfiguration(ServiceHostBase host)
        {
            // 添加接口调用耗时统计行为
            host.Description.Behaviors.Add(new IncomingMessageLoggerServiceBehavior());

            //// 添加元数据输出行为，TODO:生产环境禁用
            //var serviceMetadataBehavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
            //if (serviceMetadataBehavior == null)
            //    host.Description.Behaviors.Add(serviceMetadataBehavior = new ServiceMetadataBehavior());
            //serviceMetadataBehavior.HttpGetEnabled = true;
            //serviceMetadataBehavior.HttpsGetEnabled = true;

            // 添加详细错误输出行为
            var serviceDebugBehavior = host.Description.Behaviors.Find<ServiceDebugBehavior>();
            if (serviceDebugBehavior == null)
                host.Description.Behaviors.Add(serviceDebugBehavior = new ServiceDebugBehavior());
            serviceDebugBehavior.IncludeExceptionDetailInFaults = true;


            host.Opening += (sender, args) =>
            {
                Logger.Debug("服务正式启动");
            };

            //host.Opened += (sender, args) =>
            //{
            //    Logger.Debug("服务启动成功");
            //};

            host.UnknownMessageReceived += (sender, args) => {
                Logger.Debug($"A client attempted to send the message {args.Message.Headers.Action}");
            };

            //host.Closing += (sender, args) =>
            //{
            //    Logger.Debug("服务正在关闭中");
            //};

            //host.Closed += (sender, args) =>
            //{
            //    Logger.Debug("服务已经关闭");
            //};

            host.Faulted += (sender, args) =>
            {
                Logger.Debug("出现未处理的错误");
            };
        }

        public int Order => -990;
    }
}