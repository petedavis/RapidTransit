using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using RapidTransit.Core.Configuration;

namespace RapidTransit.Integration.AzureServiceBus
{
    public class AzureCloudConfigurationProvider : IConfigurationProvider
    {
        public bool TryGetSetting(string name, out string value)
        {
            var setting = CloudConfigurationManager.GetSetting(name);

            value = setting;

            return setting != null;
        }

        public bool TryGetConnectionString(string name, out string connectionString, out string providerName)
        {
            connectionString = providerName = null;
            return false;
        }

        public bool TryGetNameValueCollectionSection(string section, out NameValueCollection collection)
        {
            collection = null;
            return false;
        }
    }
}
