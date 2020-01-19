// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Identity.Json;

namespace Microsoft.Identity.Client.OAuth2
{
    internal class TokenResponseClaim : OAuth2ResponseBaseClaim
    {
        public const string Code = "code";
        public const string TokenType = "token_type";
        public const string AccessToken = "access_token";
        public const string RefreshToken = "refresh_token";
        public const string IdToken = "id_token";
        public const string Scope = "scope";
        public const string ClientInfo = "client_info";
        public const string ExpiresIn = "expires_in";
        public const string CloudInstanceHost = "cloud_instance_host_name";
        public const string CreatedOn = "created_on";
        public const string ExtendedExpiresIn = "ext_expires_in";
        public const string Authority = "authority";
        public const string FamilyId = "foci";
        public const string RefreshIn = "refresh_in";
    }

    [JsonObject]
    internal class MsalTokenResponse : OAuth2ResponseBase
    {
        private long _expiresIn;
        private long _extendedExpiresIn;
        private long _refreshIn;

        [JsonProperty(PropertyName = TokenResponseClaim.TokenType)]
        public string TokenType { get; set; }

        [JsonProperty(PropertyName = TokenResponseClaim.AccessToken)]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = TokenResponseClaim.RefreshToken)]
        public string RefreshToken { get; set; }

        [JsonProperty(PropertyName = TokenResponseClaim.Scope)]
        public string Scope { get; set; }

        [JsonProperty(PropertyName = TokenResponseClaim.ClientInfo)]
        public string ClientInfo { get; set; }

        [JsonProperty(PropertyName = TokenResponseClaim.IdToken)]
        public string IdToken { get; set; }

        [JsonProperty(PropertyName = TokenResponseClaim.ExpiresIn)]
        public long ExpiresIn
        {
            get => _expiresIn;
            set
            {
                _expiresIn = value;
                AccessTokenExpiresOn = DateTime.UtcNow + TimeSpan.FromSeconds(_expiresIn);
            }
        }

        [JsonProperty(PropertyName = TokenResponseClaim.ExtendedExpiresIn)]
        public long ExtendedExpiresIn
        {
            get => _extendedExpiresIn;
            set
            {
                _extendedExpiresIn = value;
                AccessTokenExtendedExpiresOn = DateTime.UtcNow + TimeSpan.FromSeconds(_extendedExpiresIn);
            }
        }

        [JsonProperty(PropertyName = TokenResponseClaim.RefreshIn)]
        public long RefreshIn
        {
            get => _refreshIn;
            set
            {
                _refreshIn = value;
                AccessTokenRefreshOn = DateTime.UtcNow + TimeSpan.FromSeconds(_refreshIn);
            }
        }

        /// <summary>
        /// Optional field, FOCI support.
        /// </summary>
        [JsonProperty(PropertyName = TokenResponseClaim.FamilyId)]
        public string FamilyId { get; set; }

        public DateTimeOffset AccessTokenExpiresOn { get; private set; }
        public DateTimeOffset AccessTokenExtendedExpiresOn { get; private set; }

        public DateTimeOffset? AccessTokenRefreshOn { get; private set; }

        public string Authority { get; set; }

    }
}
