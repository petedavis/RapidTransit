using RapidTransit.Core.Configuration;

namespace RapidTransit.Integration.AzureServiceBus
{
    public interface IAzureServiceBusSettings
        : ISettings
    {
        string Namespace { get; set; }

        string AccessKeyName { get; set; }

        string AccessKey { get; set; }
    }
}