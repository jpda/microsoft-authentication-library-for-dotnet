// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Client.Core;
using Microsoft.Identity.Client.Internal.Requests;
using Microsoft.Identity.Client.OAuth2;
using Microsoft.Identity.Client.Shared.ApiConfig.Parameters;

namespace Microsoft.Identity.Client.Shared.Requests
{
    internal class ByRefreshTokenRequest : RequestBase
    {
        private readonly AcquireTokenByRefreshTokenParameters _refreshTokenParameters;

        public ByRefreshTokenRequest(
            IServiceBundle serviceBundle,
            AuthenticationRequestParameters authenticationRequestParameters,
            AcquireTokenByRefreshTokenParameters refreshTokenParameters)
            : base(serviceBundle, authenticationRequestParameters, refreshTokenParameters)
        {
            _refreshTokenParameters = refreshTokenParameters;
        }

        internal override async Task<AuthenticationResult> ExecuteAsync(CancellationToken cancellationToken)
        {
            AuthenticationRequestParameters.RequestContext.Logger.Verbose(LogMessages.BeginningAcquireByRefreshToken);
            await ResolveAuthorityEndpointsAsync().ConfigureAwait(false);
            var msalTokenResponse = await SendTokenRequestAsync(
                                        GetBodyParameters(_refreshTokenParameters.RefreshToken),
                                        cancellationToken).ConfigureAwait(false);

            if (msalTokenResponse.RefreshToken == null)
            {
                AuthenticationRequestParameters.RequestContext.Logger.Info(MsalErrorMessage.NoRefreshTokenInResponse);
                throw new MsalServiceException(msalTokenResponse.Error, msalTokenResponse.ErrorDescription, null);
            }

            return await CacheTokenResponseAndCreateAuthenticationResultAsync(msalTokenResponse).ConfigureAwait(false);
        }

        private Dictionary<string, string> GetBodyParameters(string refreshTokenSecret)
        {
            var dict = new Dictionary<string, string>
            {
                [OAuth2Parameter.GrantType] = OAuth2GrantType.RefreshToken,
                [OAuth2Parameter.RefreshToken] = refreshTokenSecret
            };

            return dict;
        }
    }
}
