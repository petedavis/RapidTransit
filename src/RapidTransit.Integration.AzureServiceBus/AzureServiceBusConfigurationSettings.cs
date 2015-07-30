namespace RapidTransit.Integration.AzureServiceBus
{
    public class AzureServiceBusConfigurationSettings : IAzureServiceBusSettings
    {
        public string Namespace { get; set; }
        public string AccessKeyName { get; set; }
        public string AccessKey { get; set; }
    }
}