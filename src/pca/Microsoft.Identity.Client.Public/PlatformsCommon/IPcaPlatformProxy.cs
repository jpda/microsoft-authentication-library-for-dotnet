// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.Identity.Client.Core;
using Microsoft.Identity.Client.Internal.Broker;
using Microsoft.Identity.Client.UI;

namespace Microsoft.Identity.Client.Public.PlatformsCommon
{
    internal interface IPcaPlatformProxy
    {
        bool IsSystemWebViewAvailable { get; }

        bool UseEmbeddedWebViewDefault { get; }

        /// <summary>
        /// Returns true if the current OS logged in user is AD or AAD joined.
        /// </summary>
        bool IsDomainJoined();

        Task<bool> IsUserLocalAsync(RequestContext requestContext);

        /// <summary>
        /// Gets the upn of the currently logged in user (e.g. the current Windows logged in user).
        /// </summary>
        Task<string> GetUserPrincipalNameAsync();

        IWebUIFactory GetWebUiFactory();

        /// <summary>
        /// Go to a Url using the OS default browser. 
        /// </summary>
        Task StartDefaultOsBrowserAsync(string url);

        #region Broker

        IBroker CreateBroker(CoreUIParent uiParent);

        void /* for test */ SetBrokerForTest(IBroker broker);

        bool CanBrokerSupportSilentAuth();

        #endregion
    }
}
