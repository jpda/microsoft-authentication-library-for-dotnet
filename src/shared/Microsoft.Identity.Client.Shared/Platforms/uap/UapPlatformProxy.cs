// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Windows.ApplicationModel;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage;
using Microsoft.Identity.Client.Cache;
using Microsoft.Identity.Client.Core;
using Microsoft.Identity.Client.PlatformsCommon.Interfaces;
using Microsoft.Identity.Client.PlatformsCommon.Shared;
using Microsoft.Identity.Client.TelemetryCore.Internal;
using Windows.Security.Authentication.Web;

namespace Microsoft.Identity.Client.Platforms.uap
{
    /// <summary>
    /// Platform / OS specific logic. No library (ADAL / MSAL) specific code should go in here.
    /// </summary>
    internal class UapPlatformProxy : AbstractPlatformProxy
    {
        public UapPlatformProxy(ICoreLogger logger)
            : base(logger)
        {
        }

        public override string GetEnvironmentVariable(string variable)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            return localSettings.Values.ContainsKey(variable) ? localSettings.Values[variable].ToString() : null;
        }

        protected override string InternalGetProcessorArchitecture()
        {
            return WindowsNativeMethods.GetProcessorArchitecture();
        }

        protected override string InternalGetOperatingSystem()
        {
            // In WinRT, there is no way to reliably get OS version. All can be done reliably is to check
            // for existence of specific features which does not help in this case, so we do not emit OS in WinRT.
            return null;
        }

        protected override string InternalGetDeviceModel()
        {
            var deviceInformation = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();
            return deviceInformation.SystemProductName;
        }

        /// <inheritdoc />
        public override string GetBrokerOrRedirectUri(Uri redirectUri)
        {
            return redirectUri.OriginalString;
        }

        /// <inheritdoc />
        public override string GetDefaultRedirectUri(string clientId, bool useRecommendedRedirectUri = false)
        {
            if (useRecommendedRedirectUri)
            {
                return WebAuthenticationBroker.GetCurrentApplicationCallbackUri().ToString();
            }
            return Constants.DefaultRedirectUri;
        }

        protected override string InternalGetProductName()
        {
            return "MSAL.UAP";
        }

        /// <summary>
        /// Considered PII, ensure that it is hashed.
        /// </summary>
        /// <returns>Name of the calling application</returns>
        protected override string InternalGetCallingApplicationName()
        {
            return Package.Current?.DisplayName?.ToString();
        }

        /// <summary>
        /// Considered PII, ensure that it is hashed.
        /// </summary>
        /// <returns>Version of the calling application</returns>
        protected override string InternalGetCallingApplicationVersion()
        {
            return Package.Current?.Id?.Version.ToString();
        }

        /// <summary>
        /// Considered PII. Please ensure that it is hashed.
        /// </summary>
        /// <returns>Device identifier</returns>
        protected override string InternalGetDeviceId()
        {
            return new EasClientDeviceInformation()?.Id.ToString();
        }

        public override ILegacyCachePersistence CreateLegacyCachePersistence() => new UapLegacyCachePersistence(Logger, CryptographyManager);

        public override ITokenCacheAccessor CreateTokenCacheAccessor() => new InMemoryTokenCacheAccessor();

        public override ITokenCacheBlobStorage CreateTokenCacheBlobStorage() => new UapTokenCacheBlobStorage(CryptographyManager, Logger);

        protected override ICryptographyManager InternalGetCryptographyManager() => new UapCryptographyManager();
        protected override IPlatformLogger InternalGetPlatformLogger() => new EventSourcePlatformLogger();

        public override string GetDeviceNetworkState()
        {
            // TODO(mats):
            return string.Empty;
        }

        public override string GetDevicePlatformTelemetryId()
        {
            // TODO(mats):
            return string.Empty;
        }

        public override string GetMatsOsPlatform()
        {
            return MatsConverter.AsString(OsPlatform.Win32);
        }

        public override int GetMatsOsPlatformCode()
        {
            return MatsConverter.AsInt(OsPlatform.Win32);
        }
        protected override IFeatureFlags CreateFeatureFlags() => new UapFeatureFlags();

        public override bool IsSystemWebViewAvailable => false;
    }
}
