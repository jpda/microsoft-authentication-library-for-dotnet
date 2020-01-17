// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Identity.Client.Http;
using Microsoft.Identity.Client.Instance;
using Microsoft.Identity.Client.Instance.Discovery;
using Microsoft.Identity.Client.PlatformsCommon.Interfaces;
using Microsoft.Identity.Client.TelemetryCore;

namespace Microsoft.Identity.Client.Core
{
    internal interface IServiceBundle
    {
        IApplicationConfiguration Config { get; }
        ICoreLogger DefaultLogger { get; }
        IHttpManager HttpManager { get; }
        ITelemetryManager TelemetryManager { get; }
        IInstanceDiscoveryManager InstanceDiscoveryManager { get; }
        IPlatformProxy PlatformProxy { get; }

        IAuthorityEndpointResolutionManager AuthorityEndpointResolutionManager { get; }
        ITelemetryClient Mats { get; }

        T Get<T>() where T : class;
        void Register<T>(T implementation) where T : class;
    }
}
