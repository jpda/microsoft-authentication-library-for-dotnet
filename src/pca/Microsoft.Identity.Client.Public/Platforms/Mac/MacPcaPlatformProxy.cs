// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using System.Reflection;
using System.Net.NetworkInformation;
using System.Linq;
using Microsoft.Identity.Client.Core;
using Microsoft.Identity.Client.PlatformsCommon.Interfaces;
using Microsoft.Identity.Client.PlatformsCommon.Shared;
using Microsoft.Identity.Client.Cache;
using Microsoft.Identity.Client.UI;
using Microsoft.Identity.Client.TelemetryCore.Internal;
using System.Diagnostics;
using Microsoft.Identity.Client.PlatformsCommon;

namespace Microsoft.Identity.Client.Platforms.Mac
{
    /// <summary>
    /// Platform / OS specific logic.
    /// </summary>
    internal class MacPcaPlatformProxy : AbstractPcaPlatformProxy
    {
        internal const string IosDefaultRedirectUriTemplate = "msal{0}://auth";

        public MacPcaPlatformProxy(ICoreLogger logger)
            : base(logger)
        {
        }

        /// <summary>
        ///     Get the user logged
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


        private static readonly Lazy<string> DeviceIdLazy = new Lazy<string>(
           () => NetworkInterface.GetAllNetworkInterfaces().Where(nic => nic.OperationalStatus == OperationalStatus.Up)
                                 .Select(nic => nic.GetPhysicalAddress()?.ToString()).FirstOrDefault());

        protected override IWebUIFactory CreateWebUiFactory() => new MacUIFactory();

        public override Task StartDefaultOsBrowserAsync(string url)
        {
            Process.Start("open", url);
            return Task.FromResult(0);
        }
    }
}
