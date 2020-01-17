// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Identity.Client.AuthScheme.PoP;
using Microsoft.Identity.Client.Cache;

namespace Microsoft.Identity.Client.PlatformsCommon.Interfaces
{
    /// <summary>
    /// Common operations for extracting platform / operating system specifics
    /// </summary>
    /// <remarks>PCA / CCA assemblies have their own proxies in addition to this one. </remarks>
    internal interface IPlatformProxy
    {
        /// <summary>
        /// Gets the device model. On some TFMs this is not returned for security reasonons.
        /// </summary>
        /// <returns>device model or null</returns>
        string GetDeviceModel(); 

        string GetEnvironmentVariable(string variable);

        string GetOperatingSystem();

        string GetProcessorArchitecture();

        /// <summary>
        /// Returns the name of the calling assembly
        /// </summary>
        /// <returns></returns>
        string GetCallingApplicationName();

        /// <summary>
        /// Returns the version of the calling assembly
        /// </summary>
        /// <returns></returns>
        string GetCallingApplicationVersion();

        /// <summary>
        /// Returns a device identifier. Varies by platform.
        /// </summary>
        /// <returns></returns>
        string GetDeviceId();

        /// <summary>
        /// Get the redirect Uri as string, or the a broker specified value
        /// </summary>
        string GetBrokerOrRedirectUri(Uri redirectUri);

        /// <summary>
        /// Gets the default redirect uri for the platform, which sometimes includes the clientId
        /// </summary>
        string GetDefaultRedirectUri(string clientId, bool useRecommendedRedirectUri = false);

        string GetProductName();

        ILegacyCachePersistence CreateLegacyCachePersistence();

        ITokenCacheAccessor CreateTokenCacheAccessor();

        ITokenCacheBlobStorage CreateTokenCacheBlobStorage();

        ICryptographyManager CryptographyManager { get; }

        IPlatformLogger PlatformLogger { get; }

        IPoPCryptoProvider GetDefaultPoPCryptoProvider();

        // MATS related data
        string GetDevicePlatformTelemetryId();
        string GetDeviceNetworkState();
        int GetMatsOsPlatformCode();
        string GetMatsOsPlatform();

        IFeatureFlags GetFeatureFlags();

        void /* for test */ SetFeatureFlags(IFeatureFlags featureFlags);

       
    }
}
