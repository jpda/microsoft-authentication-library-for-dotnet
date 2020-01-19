// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Identity.Client.OAuth2;
using Microsoft.Identity.Client.Utils;

namespace Microsoft.Identity.Client.Internal.Broker
{
    internal class BrokerResponseConst
    {
        public const string ErrorMetadata = "error_metadata";
        public const string BrokerErrorDomain = "broker_error_domain";
        public const string BrokerErrorCode = "broker_error_code";
        public const string BrokerErrorDescription = "error_description";

        public const string Authority = "authority";
        public const string AccessToken = "access_token";
        public const string ClientId = "client_id";
        public const string RefreshToken = "refresh_token";
        public const string IdToken = "id_token";
        public const string Bearer = "Bearer";
        public const string CorrelationId = "correlation_id";
        public const string Scope = "scope";
        public const string AndroidScopes = "scopes";
        public const string ExpiresOn = "expires_on";
        public const string ClientInfo = "client_info";
        public const string Account = "mAccount";
        public const string HomeAccountId = "home_account_id";
        public const string LocalAccountId = "local_account_id";
        public const string UserName = "username";
        public const string iOSBrokerNonce = "broker_nonce"; // included in request and response with iOS Broker v3

        internal static MsalTokenResponse CreateFromBrokerResponse(Dictionary<string, string> responseDictionary)
        {
            if (responseDictionary.ContainsKey(BrokerErrorCode) ||
                responseDictionary.ContainsKey(BrokerErrorDescription))
            {
                return new MsalTokenResponse
                {
                    Error = responseDictionary[BrokerErrorCode],
                    ErrorDescription = CoreHelpers.UrlDecode(responseDictionary[BrokerErrorDescription])
                };
            }

            var response = new MsalTokenResponse
            {
                Authority = responseDictionary.ContainsKey(Authority)
                    ? AuthorityInfo.CanonicalizeAuthorityUri(CoreHelpers.UrlDecode(responseDictionary[Authority]))
                    : null,
                AccessToken = responseDictionary[AccessToken],
                RefreshToken = responseDictionary.ContainsKey(RefreshToken)
                    ? responseDictionary[RefreshToken]
                    : null,
                IdToken = responseDictionary[IdToken],
                TokenType = Bearer,
                CorrelationId = responseDictionary[CorrelationId],
                Scope = responseDictionary[Scope],
                ExpiresIn = responseDictionary.ContainsKey(ExpiresOn)
                    ? long.Parse(responseDictionary[ExpiresOn].Split('.')[0], CultureInfo.InvariantCulture)
                    : Convert.ToInt64(DateTime.UtcNow, CultureInfo.InvariantCulture),
                ClientInfo = responseDictionary.ContainsKey(ClientInfo)
                    ? responseDictionary[ClientInfo]
                    : null,
            };

            if (responseDictionary.ContainsKey(TokenResponseClaim.RefreshIn))
            {
                response.RefreshIn = long.Parse(
                    responseDictionary[TokenResponseClaim.RefreshIn],
                    CultureInfo.InvariantCulture);
            }

            return response;
        }

    }
}
