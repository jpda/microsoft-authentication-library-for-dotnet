// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client.Cache;
using Microsoft.Identity.Client.Core;
using Microsoft.Identity.Client.TelemetryCore.Internal;
using Microsoft.Identity.Client.PlatformsCommon.Interfaces;
using Microsoft.Identity.Client.PlatformsCommon;
using Microsoft.Identity.Client.UI;
using Microsoft.Win32;
using Microsoft.Identity.Client.AuthScheme.PoP;

namespace Microsoft.Identity.Client.Platforms.net45
{
    /// <summary>
    ///     Platform / OS specific logic.
    /// </summary>
    internal class NetDesktopPcaPlatformProxy : AbstractPcaPlatformProxy
    {
        /// <inheritdoc />
        public NetDesktopPcaPlatformProxy(ICoreLogger logger)
            : base(logger)
        {
        }
        private bool IsWindows
        {
            get
            {
                switch (Environment.OSVersion.Platform)
                {
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.Win32NT:
                case PlatformID.WinCE:
                    return true;
                default:
                    return false;
                }
            }
        }

        /// <summary>
        ///     Get the user logged in to Windows or throws
        /// </summary>
        /// <returns>Upn or throws</returns>
        public override Task<string> GetUserPrincipalNameAsync()
        {
            const int NameUserPrincipal = 8;
            return Task.FromResult(GetUserPrincipalName(NameUserPrincipal));
        }

        private string GetUserPrincipalName(int nameFormat)
        {
            // TODO: there is discrepancy between the implementation of this method on net45 - throws if upn not found - and uap and
            // the rest of the platforms - returns ""

            uint userNameSize = 0;
            WindowsNativeMethods.GetUserNameEx(nameFormat, null, ref userNameSize);
            if (userNameSize == 0)
            {
                throw new MsalClientException(
                    MsalError.GetUserNameFailed,
                    MsalErrorMessage.GetUserNameFailed,
                    new Win32Exception(Marshal.GetLastWin32Error()));
            }

            var sb = new StringBuilder((int)userNameSize);
            if (!WindowsNativeMethods.GetUserNameEx(nameFormat, sb, ref userNameSize))
            {
                throw new MsalClientException(
                    MsalError.GetUserNameFailed,
                    MsalErrorMessage.GetUserNameFailed,
                    new Win32Exception(Marshal.GetLastWin32Error()));
            }

            return sb.ToString();
        }

        public override Task<bool> IsUserLocalAsync(RequestContext requestContext)
        {
            var current = WindowsIdentity.GetCurrent();
            if (current != null)
            {
                string prefix = WindowsIdentity.GetCurrent().Name.Split('\\')[0].ToUpperInvariant();
                return Task.FromResult(
                    prefix.Equals(Environment.MachineName.ToUpperInvariant(), StringComparison.OrdinalIgnoreCase));
            }

            return Task.FromResult(false);
        }

        public override bool IsDomainJoined()
        {
            if (!IsWindows)
            {
                return false;
            }

            bool returnValue = false;
            try
            {
                int result = WindowsNativeMethods.NetGetJoinInformation(null, out var pDomain, out var status);
                if (pDomain != IntPtr.Zero)
                {
                    WindowsNativeMethods.NetApiBufferFree(pDomain);
                }

                returnValue = result == WindowsNativeMethods.ErrorSuccess &&
                              status == WindowsNativeMethods.NetJoinStatus.NetSetupDomainName;
            }
            catch (Exception ex)
            {
                Logger.WarningPii(ex);
                // ignore the exception as the result is already set to false;
            }

            return returnValue;
        }

        /// <inheritdoc />
        protected override IWebUIFactory CreateWebUiFactory()
        {
            return new NetDesktopWebUIFactory();
        }

        public override Task StartDefaultOsBrowserAsync(string url)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }

            return Task.FromResult(0);
        }
    }
}
