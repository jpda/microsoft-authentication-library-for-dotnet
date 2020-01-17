// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Microsoft.Identity.Client.Core;
using Microsoft.Identity.Client.UI;
using Microsoft.Identity.Client.Internal.Broker;
using Microsoft.Identity.Client.PlatformsCommon;

namespace Microsoft.Identity.Client.Platforms.Android
{
    /// <summary>
    /// Platform / OS specific logic.  No library (ADAL / MSAL) specific code should go in here.
    /// </summary>
    [global::Android.Runtime.Preserve(AllMembers = true)]
    internal class AndroidPcaPlatformProxy : AbstractPcaPlatformProxy
    {
        internal const string AndroidDefaultRedirectUriTemplate = "msal{0}://auth";
        private const string ChromePackage = "com.android.chrome";
        private const string CustomTabService = "android.support.customtabs.action.CustomTabsService";

        public AndroidPcaPlatformProxy(ICoreLogger logger) : base(logger)
        {
        }

        /// <summary>
        /// Get the user logged in
        /// </summary>
        /// <returns>The username or throws</returns>
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

        protected override IWebUIFactory CreateWebUiFactory()
        {
            return new AndroidWebUIFactory();
        }

        public override bool IsSystemWebViewAvailable
        {
            get
            {
                bool isBrowserWithCustomTabSupportAvailable = IsBrowserWithCustomTabSupportAvailable();
                return (isBrowserWithCustomTabSupportAvailable || IsChromeEnabled()) &&
                       isBrowserWithCustomTabSupportAvailable;
            }
        }

        private static bool IsBrowserWithCustomTabSupportAvailable()
        {
            Intent customTabServiceIntent = new Intent(CustomTabService);

            IEnumerable<ResolveInfo> resolveInfoListWithCustomTabs =
                Application.Context.PackageManager.QueryIntentServices(
                    customTabServiceIntent, PackageInfoFlags.MatchAll);

            // queryIntentServices could return null or an empty list if no matching service existed.
            if (resolveInfoListWithCustomTabs == null || !resolveInfoListWithCustomTabs.Any())
            {
                return false;
            }

            return true;
        }

        private static bool IsChromeEnabled()
        {
            try
            {
                ApplicationInfo applicationInfo = Application.Context.PackageManager.GetApplicationInfo(ChromePackage, 0);

                // Chrome is difficult to uninstall on an Android device. Most users will disable it, but the package will still
                // show up, therefore need to check application.Enabled is false
                return applicationInfo.Enabled;
            }
            catch (PackageManager.NameNotFoundException)
            {
                // In case Chrome is actually uninstalled, GetApplicationInfo will throw
                return false;
            }
        }

        public override bool UseEmbeddedWebViewDefault => false;

        public override IBroker CreateBroker(CoreUIParent uIParent)
        {
            if (OverloadBrokerForTest != null)
            {
                return OverloadBrokerForTest;
            }

            return new AndroidBroker(uIParent, Logger);
        }

        public override bool CanBrokerSupportSilentAuth()
        {
            return true;
        }
    }
}
