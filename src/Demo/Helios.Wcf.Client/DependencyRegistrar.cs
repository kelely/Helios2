using System.ServiceModel;
using Autofac;
using Autofac.Integration.Wcf;
using Helios.Common.Services;
using Helios.Configuration;
using Helios.Infrastructure;
using Helios.Infrastructure.DependencyManagement;

namespace Helios.Wcf.Client
{
    public class DependencyRegistrar : IDependencyRegistrar
    {

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, HeliosConfig config)
        {
            // IGenericAttributeService
            builder.Register(c => new ChannelFactory<IGenericAttributeService>(
                new BasicHttpBinding(),
                new EndpointAddress("http://wcf.helios.com/helios.common/generic-attribute-service")))
                .SingleInstance();
            builder.Register(c => c.Resolve<ChannelFactory<IGenericAttributeService>>().CreateChannel()).UseWcfSafeRelease();

            // ISettingService
            builder.Register(c => new ChannelFactory<ISettingService>(
                new BasicHttpBinding(),
                new EndpointAddress("http://wcf.helios.com/helios.common/setting-service")))
                .SingleInstance();
            builder.Register(c => c.Resolve<ChannelFactory<ISettingService>>().CreateChannel()).UseWcfSafeRelease();
        }

        public int Order => 10;
    }
}
