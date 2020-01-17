// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.Identity.Client.Http;
using Microsoft.Identity.Client.Instance;
using Microsoft.Identity.Client.Instance.Discovery;
using Microsoft.Identity.Client.Internal;
using Microsoft.Identity.Client.PlatformsCommon.Factories;
using Microsoft.Identity.Client.PlatformsCommon.Interfaces;
using Microsoft.Identity.Client.TelemetryCore;

namespace Microsoft.Identity.Client.Core
{
    internal class ServiceBundle : IServiceBundle
    {
        internal ServiceBundle(
            ApplicationConfiguration config,
            bool shouldClearCaches = false)
        {
            Config = config;

            DefaultLogger = new MsalLogger(
                Guid.Empty,
                config.ClientName,
                config.ClientVersion,
                config.LogLevel,
                config.EnablePiiLogging,
                config.IsDefaultPlatformLoggingEnabled,
                config.LoggingCallback);

            PlatformProxy = config.PlatformProxy ?? PlatformProxyFactory.CreatePlatformProxy(DefaultLogger);
            HttpManager = config.HttpManager ?? new HttpManager(config.HttpClientFactory);

            if (config.TelemetryConfig != null)
            {
                // This can return null if the device isn't sampled in.  There's no need for processing MATS events if we're not going to send them.
                Mats = TelemetryClient.CreateMats(config, PlatformProxy, config.TelemetryConfig);
                TelemetryManager = Mats?.TelemetryManager ?? new TelemetryManager(config, PlatformProxy, config.TelemetryCallback);
            }
            else
            {
                TelemetryManager = new TelemetryManager(config, PlatformProxy, config.TelemetryCallback);
            }

            InstanceDiscoveryManager = new InstanceDiscoveryManager(HttpManager, TelemetryManager, shouldClearCaches, config.CustomInstanceDiscoveryMetadata);
           // WsTrustWebRequestManager = new WsTrustWebRequestManager(HttpManager);
            AuthorityEndpointResolutionManager = new AuthorityEndpointResolutionManager(this, shouldClearCaches);
        }

        public ICoreLogger DefaultLogger { get; }

        /// <inheritdoc />
        public IHttpManager HttpManager { get; }

        /// <inheritdoc />
        public ITelemetryManager TelemetryManager { get; }

        public IInstanceDiscoveryManager InstanceDiscoveryManager { get; }

        public IAuthorityEndpointResolutionManager AuthorityEndpointResolutionManager { get; }

        /// <inheritdoc />
        public IPlatformProxy PlatformProxy { get; }

        /// <inheritdoc />
        public IApplicationConfiguration Config { get; }

        /// <inheritdoc />
        public ITelemetryClient Mats { get; }

        public static ServiceBundle Create(ApplicationConfiguration config)
        {
            return new ServiceBundle(config);
        }

        #region Service Locator
        private readonly IDictionary<Type, object> _dependencyImplementations = new Dictionary<Type, object>();

        /// <summary>
        /// Acts like a local Service Locator, i.e. registers an object to a type. Allows PCA and CCA to register
        /// their own types while still keeping the code in the Shared project.
        /// </summary>
        public void Register<T>(T implementation) where T : class 
        {

             Type targetType = typeof(T);


            if (_dependencyImplementations.ContainsKey(targetType))
                throw new InvalidOperationException("Do not register multiple  implementations in the service bundle");


            _dependencyImplementations[targetType] = implementation;
        }

        /// <summary>
        /// Acts like a Service Locator - returns an object based on its type. The object must have been 
        /// registered with <see cref="Register{T}(T)"/>
        /// </summary>
        public T Get<T>() where T : class
        {
            Type targetType = typeof(T);

            if (!_dependencyImplementations.ContainsKey(targetType))
                throw new InvalidOperationException($"Type {targetType} not registered");

            return _dependencyImplementations[targetType] as T;
        }

        #endregion
    }
}
