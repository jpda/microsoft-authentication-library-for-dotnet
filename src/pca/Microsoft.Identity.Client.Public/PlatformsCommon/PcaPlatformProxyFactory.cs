// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Identity.Client.PlatformsCommon.Interfaces;
using System;
using Microsoft.Identity.Client.Core;
using Microsoft.Identity.Client.Internal;
using Microsoft.Identity.Client.Public.PlatformsCommon;

namespace Microsoft.Identity.Client.PlatformsCommon
{
    /// <summary>
    ///     Returns the platform / os specific implementation of a PlatformProxy.
    /// </summary>
    internal static class PcaPlatformProxyFactory
    {
        /// <summary>
        ///     Gets the platform proxy, which can be used to perform platform specific operations
        /// </summary>
        public static IPcaPlatformProxy CreatePcaPlatformProxy(ICoreLogger logger)
        {
            var finalLogger = logger ?? MsalLogger.NullLogger;

#if NET_CORE
            return new Microsoft.Identity.Client.Platforms.netcore.NetCorePcaPlatformProxy(finalLogger);
#elif ANDROID
            return new Microsoft.Identity.Client.Platforms.Android.AndroidPcaPlatformProxy(finalLogger);
#elif iOS
            return new Microsoft.Identity.Client.Platforms.iOS.iOSPcaPlatformProxy(finalLogger);
#elif MAC
            return new Platforms.Mac.MacPcaPlatformProxy(finalLogger);
#elif WINDOWS_APP
            return new Microsoft.Identity.Client.Platforms.uap.UapPcaPlatformProxy(finalLogger);
#elif NETSTANDARD1_3
            return new Microsoft.Identity.Client.Platforms.netstandard13.Netstandard13PcaPlatformProxy(finalLogger);
#elif DESKTOP
            return new Microsoft.Identity.Client.Platforms.net45.NetDesktopPcaPlatformProxy(finalLogger);
#else
            throw new PlatformNotSupportedException();
#endif
        }
    }
}
