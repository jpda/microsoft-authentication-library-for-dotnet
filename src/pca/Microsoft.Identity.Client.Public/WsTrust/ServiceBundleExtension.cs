// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Identity.Client.Core;

namespace Microsoft.Identity.Client.WsTrust
{
    internal static class ServiceBundleExtension
    {
        public static WsTrustWebRequestManager GetWsTrustWebRequestManager(this IServiceBundle serviceBundle)
        {
            return serviceBundle.Get<WsTrustWebRequestManager>();
        }
    }
}
