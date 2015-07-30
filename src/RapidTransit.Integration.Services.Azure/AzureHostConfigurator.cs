using System;
using System.Diagnostics;
using System.Reflection;
using MassTransit.Logging;
using MassTransit.Monitoring;
using RapidTransit.Core.Services;
using Topshelf;
using Topshelf.HostConfigurators;
using Topshelf.Runtime;

namespace RapidTransit.Integration.Services.Azure
{
    public class AzureHostConfigurator<T> : TopshelfRoleEntryPoint
        where T : TopshelfServiceBootstrapper<T>
    {
        private static readonly ILog Log = MassTransit.Logging.Logger.Get<AzureHostConfigurator<T>>();
        private T _bootstrapper;

        public AzureHostConfigurator()
        {
            ServiceName = typeof (T).GetDisplayName();
            DisplayName = typeof (T).GetDisplayName();
            Description = typeof (T).GetServiceDescription();

            BootstrapperFactory = settings => (T) Activator.CreateInstance(typeof (T), settings);
        }

        public string ServiceName { private get; set; }
        public string DisplayName { private get; set; }
        public string Description { private get; set; }
        public Func<HostSettings, T> BootstrapperFactory { private get; set; }


        public void Start(HostConfigurator configurator)
        {
            Configure(configurator);
        }
        
        protected override void Configure(HostConfigurator configurator)
        {
            configurator.SetServiceName(ServiceName);
            configurator.SetDisplayName(DisplayName);
            configurator.SetDescription(Description);

            configurator.AfterInstall(() =>
            {
                VerifyEventLogSourceExists(ServiceName);

                // this will force the performance counters to register during service installation
                // making them created - of course using the InstallUtil stuff completely skips
                // this part of the install :(
                ServiceBusPerformanceCounters counters = ServiceBusPerformanceCounters.Instance;
            });

            configurator.Service(settings =>
            {
                OnStarting(settings);

                _bootstrapper = BootstrapperFactory(settings);

                return _bootstrapper.GetService();
            },
                s => s.AfterStoppingService(() =>
                {
                    if (_bootstrapper != default(T))
                        _bootstrapper.Dispose();
                }));
        }

        private static void VerifyEventLogSourceExists(string serviceName)
        {
            var entryAssembly = Assembly.GetEntryAssembly();

            var logName = ((AssemblyCompanyAttribute) Attribute.GetCustomAttribute(
                entryAssembly, typeof (AssemblyCompanyAttribute), false))
                .Company;

            if (string.IsNullOrEmpty(logName))
            {
                logName = "RapidTransit";

                Log.InfoFormat("Using default EventLog name '{0}'. Specify a CompanyNameAttribute on {1} to use the company name as the log name.", logName, entryAssembly.FullName);
            }

            if (!EventLog.SourceExists(serviceName))
                EventLog.CreateEventSource(serviceName, logName);
        }



        /// <summary>
        /// Called when the service is being started, prior to the container initialization
        /// </summary>
        public event OnHostStarting Starting;

        protected virtual void OnStarting(HostSettings settings)
        {
            var handler = Starting;
            if (handler != null) handler(settings);
        }
    }
}