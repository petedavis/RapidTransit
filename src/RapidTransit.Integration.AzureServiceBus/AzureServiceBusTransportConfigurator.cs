using System;
using MassTransit;
using MassTransit.BusConfigurators;
using MassTransit.Transports.AzureServiceBus;
using RapidTransit.Core;

namespace RapidTransit.Integration.AzureServiceBus
{
    public class AzureServiceBusTransportConfigurator :
         ITransportConfigurator
    {
        readonly IAzureServiceBusSettings _settings;

        public AzureServiceBusTransportConfigurator(IAzureServiceBusSettings settings)
        {
            _settings = settings;
        }

        void ITransportConfigurator.Configure(ServiceBusConfigurator configurator, string queueName, int? consumerLimit)
        {
            // Configure the azure service bus.
            configurator.UseAzureServiceBus(a => a.ConfigureNamespace(_settings.Namespace, h =>
            {
                h.SetKeyName(_settings.AccessKeyName);
                h.SetKey(_settings.AccessKey);
            }));

            configurator.UseAzureServiceBusRouting();

            var receiveFrom = _settings.GetQueueAddress(queueName);
            configurator.ReceiveFrom(receiveFrom);

            if (consumerLimit.HasValue)
                configurator.SetConcurrentConsumerLimit(consumerLimit.Value);
        }

        public Uri GetQueueAddress(string queueName, int? consumerLimit = default(int?))
        {
            return _settings.GetQueueAddress(queueName);
        }
    }
}
