using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Client.PlatformsCommon.Factories;
using Microsoft.Identity.Client.PlatformsCommon.Interfaces;
using Microsoft.Identity.Client.TelemetryCore;

namespace Microsoft.Identity.Client
{
    /// <summary>
    /// Component to be used with confidential client applications like Web Apps/API.
    /// </summary>
    public partial interface IConfidentialClientApplication
    {
        #region MSAL3X deprecations

        /// <summary>
        /// [V3 API] Acquires token using On-Behalf-Of flow. (See https://aka.ms/msal-net-on-behalf-of)
        /// </summary>
        /// <param name="scopes">Array of scopes requested for resource</param>
        /// <param name="userAssertion">Instance of UserAssertion containing user's token.</param>
        /// <returns>Authentication result containing token of the user for the requested scopes</returns>
        [Obsolete("Use AcquireTokenOnBehalfOf instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        Task<AuthenticationResult> AcquireTokenOnBehalfOfAsync(
            IEnumerable<string> scopes,
            UserAssertion userAssertion);

        /// <summary>
        /// [V3 API] Acquires token using On-Behalf-Of flow. (See https://aka.ms/msal-net-on-behalf-of)
        /// </summary>
        /// <param name="scopes">Array of scopes requested for resource</param>
        /// <param name="userAssertion">Instance of UserAssertion containing user's token.</param>
        /// <param name="authority">Specific authority for which the token is requested. Passing a different value than configured does not change the configured value</param>
        /// <returns>Authentication result containing token of the user for the requested scopes</returns>
        [Obsolete("Use AcquireTokenOnBehalfOf instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        Task<AuthenticationResult> AcquireTokenOnBehalfOfAsync(
            IEnumerable<string> scopes,
            UserAssertion userAssertion,
            string authority);

        /// <summary>
        /// [V2 API] Acquires security token from the authority using authorization code previously received.
        /// This method does not lookup token cache, but stores the result in it, so it can be looked up using other methods such as <see cref="IClientApplicationBase.AcquireTokenSilentAsync(System.Collections.Generic.IEnumerable{string}, IAccount)"/>.
        /// </summary>
        /// <param name="authorizationCode">The authorization code received from service authorization endpoint.</param>
        /// <param name="scopes">Array of scopes requested for resource</param>
        /// <returns>Authentication result containing token of the user for the requested scopes</returns>
        [Obsolete("Use AcquireTokenByAuthorizationCode instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        Task<AuthenticationResult> AcquireTokenByAuthorizationCodeAsync(
            string authorizationCode,
            IEnumerable<string> scopes);

        /// <summary>
        /// [V2 API] Acquires token from the service for the confidential client. This method attempts to look up valid access token in the cache.
        /// </summary>
        /// <param name="scopes">Array of scopes requested for resource</param>
        /// <returns>Authentication result containing application token for the requested scopes</returns>
        [Obsolete("Use AcquireTokenForClient instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        Task<AuthenticationResult> AcquireTokenForClientAsync(
            IEnumerable<string> scopes);

        /// <summary>
        /// [V2 API] Acquires token from the service for the confidential client. This method attempts to look up valid access token in the cache.
        /// </summary>
        /// <param name="scopes">Array of scopes requested for resource</param>
        /// <param name="forceRefresh">If TRUE, API will ignore the access token in the cache and attempt to acquire new access token using client credentials</param>
        /// <returns>Authentication result containing application token for the requested scopes</returns>
        [Obsolete("Use AcquireTokenForClient instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        Task<AuthenticationResult> AcquireTokenForClientAsync(
            IEnumerable<string> scopes,
            bool forceRefresh);

        /// <summary>
        /// [V2 API] URL of the authorize endpoint including the query parameters.
        /// </summary>
        /// <param name="scopes">Array of scopes requested for resource</param>
        /// <param name="loginHint">Identifier of the user. Generally a UPN.</param>
        /// <param name="extraQueryParameters">This parameter will be appended as is to the query string in the HTTP authentication request to the authority. The parameter can be null.</param>
        /// <returns>URL of the authorize endpoint including the query parameters.</returns>
        [Obsolete("Use GetAuthorizationRequestUrl instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        Task<Uri> GetAuthorizationRequestUrlAsync(
            IEnumerable<string> scopes,
            string loginHint,
            string extraQueryParameters);

        /// <summary>
        /// [V2 API] Gets URL of the authorize endpoint including the query parameters.
        /// </summary>
        /// <param name="scopes">Array of scopes requested for resource</param>
        /// <param name="redirectUri">Address to return to upon receiving a response from the authority.</param>
        /// <param name="loginHint">Identifier of the user. Generally a UPN.</param>
        /// <param name="extraQueryParameters">This parameter will be appended as is to the query string in the HTTP authentication request to the authority. The parameter can be null.</param>
        /// <param name="extraScopesToConsent">Array of scopes for which a developer can request consent upfront.</param>
        /// <param name="authority">Specific authority for which the token is requested. Passing a different value than configured does not change the configured value</param>
        /// <returns>URL of the authorize endpoint including the query parameters.</returns>
        [Obsolete("Use GetAuthorizationRequestUrl instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        Task<Uri> GetAuthorizationRequestUrlAsync(
            IEnumerable<string> scopes,
            string redirectUri,
            string loginHint,
            string extraQueryParameters, IEnumerable<string> extraScopesToConsent, string authority);

        #endregion MSAL3X deprecations
    }

    /// <summary>
    /// Component to be used with confidential client applications like Web Apps/API.
    /// This component supports Subject Name + Issuer authentication in order to help, in the future,
    /// Azure AD certificates rollover
    /// </summary>
    public interface IConfidentialClientApplicationWithCertificate
    {
        /// <summary>
        /// [V2 API] Acquires token from the service for the confidential client using the client credentials flow. (See https://aka.ms/msal-net-client-credentials)
        /// This method enables application developers to achieve easy certificates roll-over
        /// in Azure AD: this method will send the public certificate to Azure AD
        /// along with the token request, so that Azure AD can use it to validate the subject name based on a trusted issuer policy.
        /// This saves the application admin from the need to explicitly manage the certificate rollover
        /// (either via portal or powershell/CLI operation)
        /// </summary>
        /// <param name="scopes">Array of scopes requested for resource</param>
        /// <returns>Authentication result containing application token for the requested scopes</returns>
        [Obsolete("Use AcquireTokenForClient instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        Task<AuthenticationResult> AcquireTokenForClientWithCertificateAsync(IEnumerable<string> scopes);

        /// <summary>
        /// [V2 API] Acquires token from the service for the confidential client using the client credentials flow. (See https://aka.ms/msal-net-client-credentials)
        /// This method attempts to look up valid access token in the cache unless<paramref name="forceRefresh"/> is true
        /// This method enables application developers to achieve easy certificates roll-over
        /// in Azure AD: this method will send the public certificate to Azure AD
        /// along with the token request, so that Azure AD can use it to validate the subject name based on a trusted issuer policy.
        /// This saves the application admin from the need to explicitly manage the certificate rollover
        /// (either via portal or powershell/CLI operation)
        /// </summary>
        /// <param name="scopes">Array of scopes requested for resource</param>
        /// <param name="forceRefresh">If TRUE, API will ignore the access token in the cache and attempt to acquire new access token using client credentials</param>
        /// <returns>Authentication result containing application token for the requested scopes</returns>
        [Obsolete("Use AcquireTokenForClient instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        Task<AuthenticationResult> AcquireTokenForClientWithCertificateAsync(IEnumerable<string> scopes, bool forceRefresh);

        /// <summary>
        ///[V2 API] Acquires token using On-Behalf-Of flow. (See https://aka.ms/msal-net-on-behalf-of)
        /// </summary>
        /// <param name="scopes">Array of scopes requested for resource</param>
        /// <param name="userAssertion">Instance of UserAssertion containing user's token.</param>
        /// <returns>Authentication result containing token of the user for the requested scopes</returns>
        [Obsolete("Use AcquireTokenForClient instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        Task<AuthenticationResult> AcquireTokenOnBehalfOfWithCertificateAsync(IEnumerable<string> scopes, UserAssertion userAssertion);

        /// <summary>
        /// [V2 API] Acquires token using On-Behalf-Of flow. (See https://aka.ms/msal-net-on-behalf-of)
        /// </summary>
        /// <param name="scopes">Array of scopes requested for resource</param>
        /// <param name="userAssertion">Instance of UserAssertion containing user's token.</param>
        /// <param name="authority">Specific authority for which the token is requested. Passing a different value than configured does not change the configured value</param>
        /// <returns>Authentication result containing token of the user for the requested scopes</returns>
        [Obsolete("Use AcquireTokenForClient instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        Task<AuthenticationResult> AcquireTokenOnBehalfOfWithCertificateAsync(IEnumerable<string> scopes, UserAssertion userAssertion, string authority);
    }

    public sealed partial class ConfidentialClientApplication
    {
        /// <summary>
        /// [V2 API] Constructor for a confidential client application requesting tokens with the default authority (<see cref="ClientApplicationBase.DefaultAuthority"/>)
        /// </summary>
        /// <param name="clientId">Client ID (also known as App ID) of the application as registered in the
        /// application registration portal (https://aka.ms/msal-net-register-app)/. REQUIRED</param>
        /// <param name="redirectUri">URL where the STS will call back the application with the security token. REQUIRED</param>
        /// <param name="clientCredential">Credential, previously shared with Azure AD during the application registration and proving the identity
        /// of the application. An instance of <see cref="ClientCredential"/> can be created either from an application secret, or a certificate. REQUIRED.</param>
        /// <param name="userTokenCache">Token cache for saving user tokens. Can be set to null if the confidential client
        /// application only uses the Client Credentials grants (that is requests token in its own name and not in the name of users).
        /// Otherwise should be provided. REQUIRED</param>
        /// <param name="appTokenCache">Token cache for saving application (that is client token). Can be set to <c>null</c> except if the application
        /// uses the client credentials grants</param>
        /// <remarks>
        /// See https://aka.ms/msal-net-client-applications for a description of confidential client applications (and public client applications)
        /// Client credential grants are overrides of <see cref="ConfidentialClientApplication.AcquireTokenForClientAsync(IEnumerable{string})"/>
        ///
        /// See also <see cref="T:ConfidentialClientApplicationBuilder"/> for the V3 API way of building a confidential client application
        /// with a builder pattern. It offers building the application from configuration options, and a more fluid way of providing parameters.
        /// </remarks>
        /// <seealso cref="ConfidentialClientApplication"/> which
        /// enables app developers to specify the authority
        [Obsolete("Use ConfidentialClientApplicationBuilder instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        public ConfidentialClientApplication(string clientId, string redirectUri,
            ClientCredential clientCredential, TokenCache userTokenCache, TokenCache appTokenCache)
            : this(ConfidentialClientApplicationBuilder
                .Create(clientId)
                .BuildConfiguration())
        {
            throw MigrationHelper.CreateMsalNet3BreakingChangesException();
        }

        /// <summary>
        /// [V2 API] Constructor for a confidential client application requesting tokens with a specified authority
        /// </summary>
        /// <param name="clientId">Client ID (also named Application ID) of the application as registered in the
        /// application registration portal (https://aka.ms/msal-net-register-app)/. REQUIRED</param>
        /// <param name="authority">Authority of the security token service (STS) from which MSAL.NET will acquire the tokens.
        /// Usual authorities are:
        /// <list type="bullet">
        /// <item><description><c>https://login.microsoftonline.com/tenant/</c>, where <c>tenant</c> is the tenant ID of the Azure AD tenant
        /// or a domain associated with this Azure AD tenant, in order to sign-in users of a specific organization only</description></item>
        /// <item><description><c>https://login.microsoftonline.com/common/</c> to sign-in users with any work and school accounts or Microsoft personal accounts</description></item>
        /// <item><description><c>https://login.microsoftonline.com/organizations/</c> to sign-in users with any work and school accounts</description></item>
        /// <item><description><c>https://login.microsoftonline.com/consumers/</c> to sign-in users with only personal Microsoft accounts(live)</description></item>
        /// </list>
        /// Note that this setting needs to be consistent with what is declared in the application registration portal
        /// </param>
        /// <param name="redirectUri">URL where the STS will call back the application with the security token. REQUIRED</param>
        /// <param name="clientCredential">Credential, previously shared with Azure AD during the application registration and proving the identity
        /// of the application. An instance of <see cref="ClientCredential"/> can be created either from an application secret, or a certificate. REQUIRED.</param>
        /// <param name="userTokenCache">Token cache for saving user tokens. Can be set to null if the confidential client
        /// application only uses the Client Credentials grants (that is requests token in its own name and not in the name of users).
        /// Otherwise should be provided. REQUIRED</param>
        /// <param name="appTokenCache">Token cache for saving application (that is client token). Can be set to <c>null</c> except if the application
        /// uses the client credentials grants</param>
        /// <remarks>
        /// See https://aka.ms/msal-net-client-applications for a description of confidential client applications (and public client applications)
        /// Client credential grants are overrides of <see cref="ConfidentialClientApplication.AcquireTokenForClientAsync(IEnumerable{string})"/>
        ///
        /// See also <see cref="T:ConfidentialClientApplicationBuilder"/> for the V3 API way of building a confidential client application
        /// with a builder pattern. It offers building the application from configuration options, and a more fluid way of providing parameters.
        /// </remarks>
        /// <seealso cref="ConfidentialClientApplication"/> which
        /// enables app developers to create a confidential client application requesting tokens with the default authority.
        [Obsolete("Use ConfidentialClientApplicationBuilder instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        public ConfidentialClientApplication(string clientId, string authority, string redirectUri,
            ClientCredential clientCredential, TokenCache userTokenCache, TokenCache appTokenCache)
            : this(ConfidentialClientApplicationBuilder
                .Create(clientId)
                .BuildConfiguration())
        {
            throw MigrationHelper.CreateMsalNet3BreakingChangesException();
        }

        /// <summary>
        /// [V2 API] Acquires an access token for this application (usually a Web API) from the authority configured in the application, in order to access
        /// another downstream protected Web API on behalf of a user using the OAuth 2.0 On-Behalf-Of flow. (See https://aka.ms/msal-net-on-behalf-of).
        /// This confidential client application was itself called with a token which will be provided in the
        /// <paramref name="userAssertion">userAssertion</paramref> parameter.
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="userAssertion">Instance of <see cref="UserAssertion"/> containing credential information about
        /// the user on behalf of whom to get a token.</param>
        /// <returns>Authentication result containing a token for the requested scopes and account</returns>
        /// <seealso cref="AcquireTokenOnBehalfOfAsync(IEnumerable{string}, UserAssertion, string)"/> for the on-behalf-of flow when specifying the authority
        /// <seealso cref="AcquireTokenOnBehalfOf(IEnumerable{string}, UserAssertion)"/> which is the corresponding V3 API.
        [Obsolete("Use AcquireTokenOnBehalfOf instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        public Task<AuthenticationResult> AcquireTokenOnBehalfOfAsync(IEnumerable<string> scopes, UserAssertion userAssertion)
        {
            throw MigrationHelper.CreateMsalNet3BreakingChangesException();
        }

        /// <summary>
        /// [V2 API] Acquires an access token for this application (usually a Web API) from a specific authority, in order to access
        /// another downstream protected Web API on behalf of a user (See https://aka.ms/msal-net-on-behalf-of).
        /// This confidential client application was itself called with a token which will be provided in the
        /// <paramref name="userAssertion">userAssertion</paramref> parameter.
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="userAssertion">Instance of <see cref="UserAssertion"/> containing credential information about
        /// the user on behalf of whom to get a token.</param>
        /// <param name="authority">Specific authority for which the token is requested. Passing a different value than configured does not change the configured value</param>
        /// <returns>Authentication result containing a token for the requested scopes and account</returns>
        /// <seealso cref="AcquireTokenOnBehalfOfAsync(IEnumerable{string}, UserAssertion)"/> for the on-behalf-of flow without specifying the authority
        /// <seealso cref="AcquireTokenOnBehalfOf(IEnumerable{string}, UserAssertion)"/> which is the corresponding V3 API.
        [Obsolete("Use AcquireTokenOnBehalfOf instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        public Task<AuthenticationResult> AcquireTokenOnBehalfOfAsync(
            IEnumerable<string> scopes,
            UserAssertion userAssertion,
            string authority)
        {
            throw MigrationHelper.CreateMsalNet3BreakingChangesException();
        }

        /// <summary>
        /// [V2 API] Acquires an access token for this application (usually a Web API) from the authority configured in the application, in order to access
        /// another downstream protected Web API on behalf of a user using the OAuth 2.0 On-Behalf-Of flow. (See https://aka.ms/msal-net-on-behalf-of).
        /// This confidential client application was itself called with a token which will be provided in the
        /// <paramref name="userAssertion">userAssertion</paramref> parameter.
        /// This override sends the certificate, which helps certificate rotation in Azure AD
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="userAssertion">Instance of <see cref="UserAssertion"/> containing credential information about
        /// the user on behalf of whom to get a token.</param>
        /// <returns>Authentication result containing a token for the requested scopes and account</returns>
        /// <seealso cref="AcquireTokenOnBehalfOf(IEnumerable{string}, UserAssertion)"/> which is the corresponding V3 API
        [Obsolete("Use AcquireTokenOnBehalfOf instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        Task<AuthenticationResult> IConfidentialClientApplicationWithCertificate.AcquireTokenOnBehalfOfWithCertificateAsync(IEnumerable<string> scopes, UserAssertion userAssertion)
        {
            throw MigrationHelper.CreateMsalNet3BreakingChangesException();
        }

        /// <summary>
        /// [V2 API] Acquires an access token for this application (usually a Web API) from a specific authority, in order to access
        /// another downstream protected Web API on behalf of a user (See https://aka.ms/msal-net-on-behalf-of).
        /// This confidential client application was itself called with a token which will be provided in the
        /// This override sends the certificate, which helps certificate rotation in Azure AD
        /// <paramref name="userAssertion">userAssertion</paramref> parameter.
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="userAssertion">Instance of <see cref="UserAssertion"/> containing credential information about
        /// the user on behalf of whom to get a token.</param>
        /// <param name="authority">Specific authority for which the token is requested. Passing a different value than configured does not change the configured value</param>
        /// <returns>Authentication result containing a token for the requested scopes and account</returns>
        /// <seealso cref="AcquireTokenOnBehalfOf(IEnumerable{string}, UserAssertion)"/> which is the corresponding V3 API
        [Obsolete("Use AcquireTokenOnBehalfOf instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        Task<AuthenticationResult> IConfidentialClientApplicationWithCertificate.AcquireTokenOnBehalfOfWithCertificateAsync(IEnumerable<string> scopes, UserAssertion userAssertion,
            string authority)
        {
            throw MigrationHelper.CreateMsalNet3BreakingChangesException();
        }

        /// <summary>
        /// [V2 API] Acquires a security token from the authority configured in the app using the authorization code previously received from the STS. It uses
        /// the OAuth 2.0 authorization code flow (See https://aka.ms/msal-net-authorization-code).
        /// It's usually used in Web Apps (for instance ASP.NET / ASP.NET Core Web apps) which sign-in users, and therefore receive an authorization code.
        /// This method does not lookup the token cache, but stores the result in it, so it can be looked up using other methods
        /// such as <see cref="IClientApplicationBase.AcquireTokenSilentAsync(IEnumerable{string}, IAccount)"/>.
        /// </summary>
        /// <param name="authorizationCode">The authorization code received from service authorization endpoint.</param>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <returns>Authentication result containing token of the user for the requested scopes</returns>
        /// <seealso cref="AcquireTokenByAuthorizationCode(IEnumerable{string}, string)"/> which is the corresponding V2 API
        [Obsolete("Use AcquireTokenByAuthorizationCode instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        public Task<AuthenticationResult> AcquireTokenByAuthorizationCodeAsync(string authorizationCode, IEnumerable<string> scopes)
        {
            throw MigrationHelper.CreateMsalNet3BreakingChangesException();
        }

        /// <summary>
        /// [V3 API] Acquires a token from the authority configured in the app, for the confidential client itself (in the name of no user)
        /// using the client credentials flow. (See https://aka.ms/msal-net-client-credentials)
        /// </summary>
        /// <param name="scopes">scopes requested to access a protected API. For this flow (client credentials), the scopes
        /// should be of the form "{ResourceIdUri/.default}" for instance <c>https://management.azure.net/.default</c> or, for Microsoft
        /// Graph, <c>https://graph.microsoft.com/.default</c> as the requested scopes are really defined statically at application registration
        /// in the portal, and cannot be overriden in the application. See also </param>
        /// <returns>Authentication result containing the token of the user for the requested scopes</returns>
        [Obsolete("Use AcquireTokenForClient instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        public Task<AuthenticationResult> AcquireTokenForClientAsync(IEnumerable<string> scopes)
        {
            throw MigrationHelper.CreateMsalNet3BreakingChangesException();
        }

        /// <summary>
        /// [V2 API] Acquires a token from the authority configured in the app, for the confidential client itself (in the name of no user)
        /// using the client credentials flow. (See https://aka.ms/msal-net-client-credentials)
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API. For this flow (client credentials), the scopes
        /// should be of the form "{ResourceIdUri/.default}" for instance <c>https://management.azure.net/.default</c> or, for Microsoft
        /// Graph, <c>https://graph.microsoft.com/.default</c> as the requested scopes are really defined statically at application registration
        /// in the portal, and cannot be overriden in the application</param>
        /// <param name="forceRefresh">If <c>true</c>, API will ignore the access token in the cache and attempt to acquire new access token using client credentials.
        /// This override can be used in case the application knows that conditional access policies changed</param>
        /// <returns>Authentication result containing token of the user for the requested scopes</returns>
        /// <seealso cref="AcquireTokenForClient(IEnumerable{string})"/> which is the corresponding V3 API
        [Obsolete("Use AcquireTokenForClient instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        public Task<AuthenticationResult> AcquireTokenForClientAsync(IEnumerable<string> scopes, bool forceRefresh)
        {
            throw MigrationHelper.CreateMsalNet3BreakingChangesException();
        }

        /// <summary>
        /// [V2 API] Acquires token from the service for the confidential client using the client credentials flow. (See https://aka.ms/msal-net-client-credentials)
        /// This method enables application developers to achieve easy certificate roll-over
        /// in Azure AD: this method will send the public certificate to Azure AD
        /// along with the token request, so that Azure AD can use it to validate the subject name based on a trusted issuer policy.
        /// This saves the application admin from the need to explicitly manage the certificate rollover
        /// (either via portal or powershell/CLI operation)
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <returns>Authentication result containing application token for the requested scopes</returns>
        /// <seealso cref="AcquireTokenForClient(IEnumerable{string})"/> which is the corresponding V3 API
        [Obsolete("Use AcquireTokenForClient instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        Task<AuthenticationResult> IConfidentialClientApplicationWithCertificate.AcquireTokenForClientWithCertificateAsync(IEnumerable<string> scopes)
        {
            throw MigrationHelper.CreateMsalNet3BreakingChangesException();
        }

        /// <summary>
        /// [V2 API] Acquires token from the service for the confidential client using the client credentials flow. (See https://aka.ms/msal-net-client-credentials)
        /// This method attempts to look up valid access token in the cache unless<paramref name="forceRefresh"/> is true
        /// This method enables application developers to achieve easy certificate roll-over
        /// in Azure AD: this method will send the public certificate to Azure AD
        /// along with the token request, so that Azure AD can use it to validate the subject name based on a trusted issuer policy.
        /// This saves the application admin from the need to explicitly manage the certificate rollover
        /// (either via portal or powershell/CLI operation)
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="forceRefresh">If TRUE, API will ignore the access token in the cache and attempt to acquire new access token using client credentials</param>
        /// <returns>Authentication result containing application token for the requested scopes</returns>
        /// <seealso cref="AcquireTokenForClient(IEnumerable{string})"/> which is the corresponding V3 API
        [Obsolete("Use AcquireTokenForClient instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        Task<AuthenticationResult> IConfidentialClientApplicationWithCertificate.AcquireTokenForClientWithCertificateAsync(IEnumerable<string> scopes, bool forceRefresh)
        {
            throw MigrationHelper.CreateMsalNet3BreakingChangesException();
        }

        /// <summary>
        /// Acquires an access token from an existing refresh token and stores it and the refresh token into
        /// the application user token cache, where it will be available for further AcquireTokenSilentAsync calls.
        /// This method can be used in migration to MSAL from ADAL v2 and in various integration
        /// scenarios where you have a RefreshToken available.
        /// (see https://aka.ms/msal-net-migration-adal2-msal2)
        /// </summary>
        /// <param name="scopes">Scope to request from the token endpoint.
        /// Setting this to null or empty will request an access token, refresh token and ID token with default scopes</param>
        /// <param name="refreshToken">The refresh token (for example previously obtained from ADAL 2.x)</param>
        [Obsolete("Use AcquireTokenByRefreshToken instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        Task<AuthenticationResult> IByRefreshToken.AcquireTokenByRefreshTokenAsync(IEnumerable<string> scopes, string refreshToken)
        {
            throw MigrationHelper.CreateMsalNet3BreakingChangesException();
        }

        /// <summary>
        /// [V2 API] Computes the URL of the authorization request letting the user sign-in and consent to the application accessing specific scopes in
        /// the user's name. The URL targets the /authorize endpoint of the authority configured in the application.
        /// This override enables you to specify a login hint and extra query parameter.
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="loginHint">Identifier of the user. Generally a UPN. This can be empty</param>
        /// <param name="extraQueryParameters">This parameter will be appended as is to the query string in the HTTP authentication request to the authority.
        /// This is expected to be a string of segments of the form <c>key=value</c> separated by an ampersand character.
        /// The parameter can be null.</param>
        /// <returns>URL of the authorize endpoint including the query parameters.</returns>
        /// <seealso cref="GetAuthorizationRequestUrl(IEnumerable{string})"/> which is the corresponding V3 API
        [Obsolete("Use GetAuthorizationRequestUrl instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        public Task<Uri> GetAuthorizationRequestUrlAsync(
            IEnumerable<string> scopes,
            string loginHint,
            string extraQueryParameters)
        {
            throw MigrationHelper.CreateMsalNet3BreakingChangesException();
        }

        /// <summary>
        /// [V2 API] Computes the URL of the authorization request letting the user sign-in and consent to the application accessing specific scopes in
        /// the user's name. The URL targets the /authorize endpoint of the authority specified as the <paramref name="authority"/> parameter.
        /// This override enables you to specify a redirectUri, login hint extra query parameters, extra scope to consent (which are not for the
        /// same resource as the <paramref name="scopes"/>), and an authority.
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API (a resource)</param>
        /// <param name="redirectUri">Address to return to upon receiving a response from the authority.</param>
        /// <param name="loginHint">Identifier of the user. Generally a UPN.</param>
        /// <param name="extraQueryParameters">This parameter will be appended as is to the query string in the HTTP authentication request to the authority.
        /// This is expected to be a string of segments of the form <c>key=value</c> separated by an ampersand character.
        /// The parameter can be null.</param>
        /// <param name="extraScopesToConsent">Scopes for additional resources (other than the resource for which <paramref name="scopes"/> are requested),
        /// which a developer can request the user to consent to upfront.</param>
        /// <param name="authority">Specific authority for which the token is requested. Passing a different value than configured does not change the configured value</param>
        /// <returns>URL of the authorize endpoint including the query parameters.</returns>
        /// <seealso cref="GetAuthorizationRequestUrl(IEnumerable{string})"/> which is the corresponding V3 API
        [Obsolete("Use GetAuthorizationRequestUrl instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        public Task<Uri> GetAuthorizationRequestUrlAsync(
            IEnumerable<string> scopes,
            string redirectUri,
            string loginHint,
            string extraQueryParameters,
            IEnumerable<string> extraScopesToConsent,
            string authority)
        {
            throw MigrationHelper.CreateMsalNet3BreakingChangesException();
        }
    }


    /// <summary>
    /// Certificate for a client assertion. This class is used in one of the constructors of <see cref="ClientCredential"/>. ClientCredential
    /// is itself used in the constructor of <see cref="ConfidentialClientApplication"/> to pass to Azure AD a shared secret (registered in the
    /// Azure AD application)
    /// </summary>
    /// <seealso cref="ClientCredential"/> for the constructor of <seealso cref="ClientCredential"/>
    /// with a certificate, and <seealso cref="ConfidentialClientApplication"/>
    /// <remarks>To understand the difference between public client applications and confidential client applications, see https://aka.ms/msal-net-client-applications</remarks>
    [Obsolete("Use ConfidentialClientApplicationBuilder.WithCertificate instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
    public sealed class ClientAssertionCertificate
    {
        /// <summary>
        /// Constructor to create certificate information used in <see cref="ClientCredential"/>
        /// to instantiate a <see cref="ClientCredential"/> used in the constructors of <see cref="ConfidentialClientApplication"/>
        /// </summary>
        /// <param name="certificate">The X509 certificate used as credentials to prove the identity of the application to Azure AD.</param>
        public ClientAssertionCertificate(X509Certificate2 certificate)
        {
            throw MigrationHelper.CreateMsalNet3BreakingChangesException();
        }

        /// <summary>
        /// Gets minimum X509 certificate key size in bits
        /// </summary>
        public static int MinKeySizeInBits => 2048;

        /// <summary>
        /// Gets the X509 certificate used as credentials to prove the identity of the application to Azure AD.
        /// </summary>
        public X509Certificate2 Certificate => throw MigrationHelper.CreateMsalNet3BreakingChangesException();

        internal byte[] Sign(ICryptographyManager cryptographyManager, string message)
        {
            throw MigrationHelper.CreateMsalNet3BreakingChangesException();
        }

        // Thumbprint should be url encoded
        internal string Thumbprint => throw MigrationHelper.CreateMsalNet3BreakingChangesException();
    }

    /// <summary>
    /// Meant to be used in confidential client applications, an instance of <c>ClientCredential</c> is passed
    /// to the constructors of (<see cref="ConfidentialClientApplication"/>)
    /// as credentials proving that the application (the client) is what it claims it is. These credentials can be
    /// either a client secret (an application password) or a certificate.
    /// This class has one constructor for each case.
    /// These credentials are added in the application registration portal (in the secret section).
    /// </summary>
    [Obsolete("Use ConfidentialClientApplicationBuilder.WithCertificate or WithClientSecret instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
    public sealed class ClientCredential
    {
        /// <summary>
        /// Constructor of client (application) credentials from a <see cref="ClientAssertionCertificate"/>
        /// </summary>
        /// <param name="certificate">contains information about the certificate previously shared with AAD at application
        /// registration to prove the identity of the application (the client) requesting the tokens.</param>
        public ClientCredential(ClientAssertionCertificate certificate)
        {
            throw MigrationHelper.CreateMsalNet3BreakingChangesException();
        }

        internal ClientAssertionCertificate Certificate => throw MigrationHelper.CreateMsalNet3BreakingChangesException();
        internal string Assertion
        {
            get { throw MigrationHelper.CreateMsalNet3BreakingChangesException(); }
            set { throw MigrationHelper.CreateMsalNet3BreakingChangesException(); }
        }

        internal long ValidTo
        {
            get { throw MigrationHelper.CreateMsalNet3BreakingChangesException(); }
            set { throw MigrationHelper.CreateMsalNet3BreakingChangesException(); }
        }

        internal bool ContainsX5C
        {
            get { throw MigrationHelper.CreateMsalNet3BreakingChangesException(); }
            set { throw MigrationHelper.CreateMsalNet3BreakingChangesException(); }
        }

        internal string Audience
        {
            get { throw MigrationHelper.CreateMsalNet3BreakingChangesException(); }
            set { throw MigrationHelper.CreateMsalNet3BreakingChangesException(); }
        }

        /// <summary>
        /// Constructor of client (application) credentials from a client secret, also known as the application password.
        /// </summary>
        /// <param name="secret">Secret string previously shared with AAD at application registration to prove the identity
        /// of the application (the client) requesting the tokens.</param>
        public ClientCredential(string secret)
        {
            throw MigrationHelper.CreateMsalNet3BreakingChangesException();
        }

        internal string Secret => throw MigrationHelper.CreateMsalNet3BreakingChangesException();
    }
}
