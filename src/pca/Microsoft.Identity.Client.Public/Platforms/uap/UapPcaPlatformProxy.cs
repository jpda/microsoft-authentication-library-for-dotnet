// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.System;
using Microsoft.Identity.Client.Core;
using Microsoft.Identity.Client.UI;
using Microsoft.Identity.Client.PlatformsCommon;

namespace Microsoft.Identity.Client.Platforms.uap
{
    /// <summary>
    /// Platform / OS specific logic. No library (ADAL / MSAL) specific code should go in here.
    /// </summary>
    internal class UapPcaPlatformProxy : AbstractPcaPlatformProxy
    {
        public UapPcaPlatformProxy(ICoreLogger logger)
            : base(logger)
        {
        }

        /// <summary>
        /// Get the user logged in to Windows or throws
        /// </summary>
        /// <remarks>
        /// Win10 allows several identities to be logged in at once;
        /// select the first principal name that can be used
        /// </remarks>
        /// <returns>The username or throws</returns>
        public override async Task<string> GetUserPrincipalNameAsync()
        {
            IReadOnlyList<User> users = await User.FindAllAsync();
            if (users == null || !users.Any())
            {
                throw new MsalClientException(
                    MsalError.CannotAccessUserInformationOrUserNotDomainJoined,
                    MsalErrorMessage.UapCannotFindDomainUser);
            }

            var getUserDetailTasks = users.Select(async u =>
            {
                object domainObj = await u.GetPropertyAsync(KnownUserProperties.DomainName);
                string domainString = domainObj?.ToString();

                object principalObject = await u.GetPropertyAsync(KnownUserProperties.PrincipalName);
                string principalNameString = principalObject?.ToString();

                return new { Domain = domainString, PrincipalName = principalNameString };
            }).ToList();

            var userDetails = await Task.WhenAll(getUserDetailTasks).ConfigureAwait(false);

            // try to get a user that has both domain name and upn
            var userDetailWithDomainAndPn = userDetails.FirstOrDefault(
                d => !string.IsNullOrWhiteSpace(d.Domain) &&
                !string.IsNullOrWhiteSpace(d.PrincipalName));

            if (userDetailWithDomainAndPn != null)
            {
                return userDetailWithDomainAndPn.PrincipalName;
            }

            // try to get a user that at least has upn
            var userDetailWithPn = userDetails.FirstOrDefault(
              d => !string.IsNullOrWhiteSpace(d.PrincipalName));

            if (userDetailWithPn != null)
            {
                return userDetailWithPn.PrincipalName;
            }

            // user has domain name, but no upn -> missing Enterprise Auth capability
            if (userDetails.Any(d => !string.IsNullOrWhiteSpace(d.Domain)))
            {
                throw new MsalClientException(
                   MsalError.CannotAccessUserInformationOrUserNotDomainJoined,
                   MsalErrorMessage.UapCannotFindUpn);
            }

            // no domain, no upn -> missing User Info capability
            throw new MsalClientException(
                MsalError.CannotAccessUserInformationOrUserNotDomainJoined,
                MsalErrorMessage.UapCannotFindDomainUser);

        }

        public override async Task<bool> IsUserLocalAsync(RequestContext requestContext)
        {
            IReadOnlyList<User> users = await User.FindAllAsync();
            return users.Any(u => u.Type == UserType.LocalUser || u.Type == UserType.LocalGuest);
        }

        public override bool IsDomainJoined()
        {
            return NetworkInformation.GetHostNames().Any(entry => entry.Type == HostNameType.DomainName);
        }

        protected override IWebUIFactory CreateWebUiFactory() => new WebUIFactory();

        public override bool IsSystemWebViewAvailable => false;
    }
}
