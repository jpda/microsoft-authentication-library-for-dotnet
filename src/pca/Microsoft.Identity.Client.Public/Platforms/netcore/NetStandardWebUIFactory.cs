// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Identity.Client.Core;
using Microsoft.Identity.Client.Platforms.Shared.Desktop.OsBrowser;
using Microsoft.Identity.Client.UI;
using Microsoft.Identity.Client.WsTrust;

namespace Microsoft.Identity.Client.Platforms.Shared.NetStdCore
{
    internal class NetCoreWebUIFactory : IWebUIFactory
    {
        public IWebUI CreateAuthenticationDialog(CoreUIParent parent, RequestContext requestContext)
        {
            return new DefaultOsBrowserWebUi(
                requestContext.ServiceBundle.GetPcaPlatformProxy(),
                requestContext.Logger,
                parent.SystemWebViewOptions);
        }
    }
}
