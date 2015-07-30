using System;

namespace RapidTransit.Integration.AzureServiceBus
{
    public static class AzureServiceBusSettingsExtensions
    {
        public static Uri GetQueueAddress(this IAzureServiceBusSettings settings, string queueName)
        {
            var azureNamespace = settings.Namespace;

            var queueUri = "azure-sb://" + azureNamespace + "/" + queueName;

            return new Uri(queueUri);
        }
    }
}