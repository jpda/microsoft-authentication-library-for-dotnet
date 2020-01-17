// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Identity.Client.Core;
using Microsoft.Identity.Client.Public.PlatformsCommon;

namespace Microsoft.Identity.Client.WsTrust //TODO: move to correct ns 
{
    internal static class ServiceBundleExtension
    {
        public static WsTrustWebRequestManager GetWsTrustWebRequestManager(this IServiceBundle serviceBundle)
        {
            return serviceBundle.Get<WsTrustWebRequestManager>();
        }

        public static IPcaPlatformProxy GetPcaPlatformProxy(this IServiceBundle serviceBundle)
        {
            return serviceBundle.Get<IPcaPlatformProxy>();
        }
    }
}
