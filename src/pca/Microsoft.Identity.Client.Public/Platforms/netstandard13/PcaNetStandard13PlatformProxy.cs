// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.Identity.Client.Core;
using Microsoft.Identity.Client.Platforms.Shared.NetStdCore;
using Microsoft.Identity.Client.PlatformsCommon;
using Microsoft.Identity.Client.UI;

namespace Microsoft.Identity.Client.Platforms.netstandard13
{
    /// <summary>
    /// Platform / OS specific logic.  No library (ADAL / MSAL) specific code should go in here.
    /// </summary>
    internal class Netstandard13PcaPlatformProxy : AbstractPcaPlatformProxy
    {
        public Netstandard13PcaPlatformProxy(ICoreLogger logger)
            : base(logger)
        {
        }

        /// <summary>
        /// Get the user logged in
        /// </summary>
        public override Task<string> GetUserPrincipalNameAsync()
        {
            return Task.FromResult(string.Empty);
        }

        public override Task<bool> IsUserLocalAsync(RequestContext requestContext)
        {
            return Task.FromResult(false);
        }

        public override bool IsDomainJoined()
        {
            return false;
        }

        protected override IWebUIFactory CreateWebUiFactory() => new WebUIFactory();

        public override Task StartDefaultOsBrowserAsync(string url)
        {
            PlatformProxyShared.StartDefaultOsBrowser(url);
            return Task.FromResult(0);
        }

        public override bool UseEmbeddedWebViewDefault => false;
    }
}
