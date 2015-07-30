using System.Configuration;
using Autofac;
using RapidTransit.Core;
using RapidTransit.Core.Configuration;

namespace RapidTransit.Integration.AzureServiceBus
{
    public class AzureServiceBusConfigurationModule :
        Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(GetAzureServiceBusSettings)
                .As<IAzureServiceBusSettings>()
                .SingleInstance();

            builder.RegisterType<AzureServiceBusTransportConfigurator>()
                .As<ITransportConfigurator>();
        }

        static IAzureServiceBusSettings GetAzureServiceBusSettings(IComponentContext context)
        {
            IAzureServiceBusSettings settings;
            if (context.Resolve<ISettingsProvider>().TryGetSettings("AzureServiceBus", out settings))
                return settings;

            throw new ConfigurationErrorsException("Unable to resolve AzureServiceBus from configuration");
        }
    }
}