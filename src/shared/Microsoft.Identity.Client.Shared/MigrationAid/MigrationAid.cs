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

#if iOS
using Microsoft.Identity.Client.Platforms.iOS;
#endif

#if ANDROID
using Android.App;
#endif

namespace Microsoft.Identity.Client
{
    internal static class MigrationHelper
    {
        public static NotImplementedException CreateMsalNet3BreakingChangesException()
        {
            return new NotImplementedException("See https://aka.ms/msal-net-3-breaking-changes");
        }
    }

    /// <summary>
    /// In MSAL.NET 1.x, was representing a User. From MSAL 2.x use <see cref="IAccount"/> which represents an account
    /// (a user has several accounts). See https://aka.ms/msal-net-2-released for more details.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use IAccount instead (See https://aka.ms/msal-net-2-released)", true)]
    public interface IUser
    {
        /// <summary>
        /// In MSAL.NET 1.x was the displayable ID of a user. From MSAL 2.x use the <see cref="IAccount.Username"/> of an account.
        /// See https://aka.ms/msal-net-2-released for more details
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Obsolete("Use IAccount.Username instead (See https://aka.ms/msal-net-2-released)", true)]
        string DisplayableId { get; }

        /// <summary>
        /// In MSAL.NET 1.x was the name of the user (which was not very useful as the concatenation of
        /// some claims). From MSAL 2.x rather use <see cref="IAccount.Username"/>. See https://aka.ms/msal-net-2-released for more details.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Obsolete("Use IAccount.Username instead (See https://aka.ms/msal-net-2-released)", true)]
        string Name { get; }

        /// <summary>
        /// In MSAL.NET 1.x was the URL of the identity provider (e.g. https://login.microsoftonline.com/tenantId).
        /// From MSAL.NET 2.x use <see cref="IAccount.Environment"/> which retrieves the host only (e.g. login.microsoftonline.com).
        /// See https://aka.ms/msal-net-2-released for more details.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Obsolete("Use IAccount.Environment instead to get the Identity Provider host (See https://aka.ms/msal-net-2-released)", true)]
        string IdentityProvider { get; }

        /// <summary>
        /// In MSAL.NET 1.x was an identifier for the user in the guest tenant.
        /// From MSAL.NET 2.x, use <see cref="IAccount.HomeAccountId"/><see cref="AccountId.Identifier"/> to get
        /// the user identifier (globally unique accross tenants). See https://aka.ms/msal-net-2-released for more details.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Obsolete("Use IAccount.HomeAccountId.Identifier instead to get the user identifier (See https://aka.ms/msal-net-2-released)", true)]
        string Identifier { get; }
    }

    /// <Summary>
    /// Interface defining common API methods and properties. Both <see cref="T:PublicClientApplication"/> and <see cref="T:ConfidentialClientApplication"/>
    /// extend this class. For details see https://aka.ms/msal-net-client-applications
    /// </Summary>
    public partial interface IClientApplicationBase
    {
        /// <summary>
        /// In MSAL 1.x returned an enumeration of <see cref="IUser"/>. From MSAL 2.x, use <see cref="GetAccountsAsync"/> instead.
        /// See https://aka.ms/msal-net-2-released for more details.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Obsolete("Use GetAccountsAsync instead (See https://aka.ms/msal-net-2-released)", true)]
        IEnumerable<IUser> Users { get; }

        /// <summary>
        /// In MSAL 1.x, return a user from its identifier. From MSAL 2.x, use <see cref="GetAccountsAsync"/> instead.
        /// See https://aka.ms/msal-net-2-released for more details.
        /// </summary>
        /// <param name="identifier">Identifier of the user to retrieve</param>
        /// <returns>the user in the cache with the identifier passed as an argument</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use GetAccountAsync instead and pass IAccount.HomeAccountId.Identifier (See https://aka.ms/msal-net-2-released)", true)]
        IUser GetUser(string identifier);

        /// <summary>
        /// In MSAL 1.x removed a user from the cache. From MSAL 2.x, use <see cref="RemoveAsync(IAccount)"/> instead.
        /// See https://aka.ms/msal-net-2-released for more details.
        /// </summary>
        /// <param name="user">User to remove from the cache</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use RemoveAccountAsync instead (See https://aka.ms/msal-net-2-released)", true)]
        void Remove(IUser user);

        /// <summary>
        /// Identifier of the component (libraries/SDK) consuming MSAL.NET.
        /// This will allow for disambiguation between MSAL usage by the app vs MSAL usage by component libraries.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use WithComponent on AbstractApplicationBuilder<T> to configure this instead.  See https://aka.ms/msal-net-3-breaking-changes or https://aka.ms/msal-net-application-configuration", true)]
        string Component { get; set; }

        /// <summary>
        /// Sets or Gets a custom query parameters that may be sent to the STS for dogfood testing or debugging. This is a string of segments
        /// of the form <c>key=value</c> separated by an ampersand character.
        /// Unless requested otherwise, this parameter should not be set by application developers as it may have adverse effect on the application.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use ExtraQueryParameters on each call instead.  See https://aka.ms/msal-net-3-breaking-changes or https://aka.ms/msal-net-application-configuration", true)]
        string SliceParameters { get; set; }

        /// <summary>
        /// Gets a boolean value telling the application if the authority needs to be verified against a list of known authorities. The default
        /// value is <c>true</c>. It should currently be set to <c>false</c> for Azure AD B2C authorities as those are customer specific
        /// (a list of known B2C authorities cannot be maintained by MSAL.NET)
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Can be set on AbstractApplicationBuilder<T>.WithAuthority as needed.  See https://aka.ms/msal-net-3-breaking-changes or https://aka.ms/msal-net-application-configuration", true)]
        bool ValidateAuthority { get; }

#if !DESKTOP && !NET_CORE
#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
#endif
        /// <summary>
        /// The redirect URI (also known as Reply URI or Reply URL), is the URI at which Azure AD will contact back the application with the tokens.
        /// This redirect URI needs to be registered in the app registration (https://aka.ms/msal-net-register-app)
        /// In MSAL.NET, <see cref="T:PublicClientApplication"/> define the following default RedirectUri values:
        /// <list type="bullet">
        /// <item><description><c>urn:ietf:wg:oauth:2.0:oob</c> for desktop (.NET Framework and .NET Core) applications</description></item>
        /// <item><description><c>msal{ClientId}</c> for Xamarin iOS and Xamarin Android (as this will be used by the system web browser by default on these
        /// platforms to call back the application)
        /// </description></item>
        /// </list>
        /// These default URIs could change in the future.
        /// In a ConfidentialClientApplication, this can be the URL of the Web application / Web API.
        /// </summary>
        /// <remarks>This is especially important when you deploy an application that you have initially tested locally;
        /// you then need to add the reply URL of the deployed application in the application registration portal.
        /// </remarks>
        [Obsolete("Should be set using AbstractApplicationBuilder<T>.WithRedirectUri and can be viewed with ClientApplicationBase.AppConfig.RedirectUri. See https://aka.ms/msal-net-3-breaking-changes or https://aka.ms/msal-net-application-configuration", true)]
        string RedirectUri { get; set; }
#pragma warning restore CS1574 // XML comment has cref attribute that could not be resolved

        #region MSAL3X deprecations
        /// <summary>
        /// Attempts to acquire an access token for the <paramref name="account"/> from the user token cache.
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="account">Account for which the token is requested. <see cref="IAccount"/></param>
        /// <returns>An <see cref="AuthenticationResult"/> containing the requested token</returns>
        /// <exception cref="MsalUiRequiredException">can be thrown in the case where an interaction is required with the end user of the application,
        /// for instance so that the user consents, or re-signs-in (for instance if the password expirred), or performs two factor authentication</exception>
        /// <remarks>
        /// The access token is considered a match if it contains <b>at least</b> all the requested scopes.
        /// This means that an access token with more scopes than requested could be returned as well. If the access token is expired or
        /// close to expiration (within 5 minute window), then the cached refresh token (if available) is used to acquire a new access token by making a silent network call.
        /// See https://aka.ms/msal-net-acuiretokensilent for more details
        /// </remarks>
        [Obsolete("Use AcquireTokenSilent instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        Task<AuthenticationResult> AcquireTokenSilentAsync(
            IEnumerable<string> scopes,
            IAccount account);

        /// <summary>
        /// Attempts to acquire and access token for the <paramref name="account"/> from the user token cache, with advanced parameters making a network call.
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="account">Account for which the token is requested. <see cref="IAccount"/></param>
        /// <param name="authority">Specific authority for which the token is requested. Passing a different value than configured in the application constructor
        /// narrows down the selection of tenants for which to get a tenant, but does not change the configured value</param>
        /// <param name="forceRefresh">If <c>true</c>, the will ignore the access token in the cache and attempt to acquire new access token
        /// using the refresh token for the account if this one is available. This can be useful in the case when the application developer wants to make
        /// sure that conditional access policies are applies immediately, rather than after the expiration of the access token</param>
        /// <returns>An <see cref="AuthenticationResult"/> containing the requested token</returns>
        /// <exception cref="MsalUiRequiredException">can be thrown in the case where an interaction is required with the end user of the application,
        /// for instance, if no refresh token was in the cache, or the user needs to consents, or re-sign-in (for instance if the password expirred),
        /// or performs two factor authentication</exception>
        /// <remarks>
        /// The access token is considered a match if it contains <b>at least</b> all the requested scopes. This means that an access token with more scopes than
        /// requested could be returned as well. If the access token is expired or close to expiration (within 5 minute window),
        /// then the cached refresh token (if available) is used to acquire a new access token by making a silent network call.
        /// See https://aka.ms/msal-net-acquiretokensilent for more details
        /// </remarks>
        [Obsolete("Use AcquireTokenSilent instead." + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        Task<AuthenticationResult> AcquireTokenSilentAsync(
            IEnumerable<string> scopes,
            IAccount account,
            string authority,
            bool forceRefresh);

        /// <summary>
        /// Gets the Client ID (also known as Application ID) of the application as registered in the application registration portal (https://aka.ms/msal-net-register-app)
        /// and as passed in the constructor of the application.
        /// </summary>
        [Obsolete("Use AppConfig.ClientId instead." + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        string ClientId { get; }

        #endregion MSAL3X deprecations
    }

    /// <Summary>
    /// Abstract class containing common API methods and properties. Both <see cref="T:PublicClientApplication"/> and <see cref="T:ConfidentialClientApplication"/>
    /// extend this class. For details see https://aka.ms/msal-net-client-applications
    /// </Summary>
    public partial class ClientApplicationBase
    {
        /// <summary>
        /// In MSAL 1.x returned an enumeration of <see cref="IUser"/>. From MSAL 2.x, use <see cref="GetAccountsAsync"/> instead.
        /// See https://aka.ms/msal-net-2-released for more details.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Obsolete("Use GetAccountsAsync instead (See https://aka.ms/msal-net-2-released)", true)]
        public IEnumerable<IUser> Users { get { throw new NotImplementedException(); } }

        /// <summary>
        /// In MSAL 1.x, return a user from its identifier. From MSAL 2.x, use <see cref="GetAccountsAsync"/> instead.
        /// See https://aka.ms/msal-net-2-released for more details.
        /// </summary>
        /// <param name="identifier">Identifier of the user to retrieve</param>
        /// <returns>the user in the cache with the identifier passed as an argument</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use GetAccountAsync instead and pass IAccount.HomeAccountId.Identifier (See https://aka.ms/msal-net-2-released)", true)]
        public IUser GetUser(string identifier) { throw new NotImplementedException(); }

        /// <summary>
        /// In MSAL 1.x removed a user from the cache. From MSAL 2.x, use <see cref="RemoveAsync(IAccount)"/> instead.
        /// See https://aka.ms/msal-net-2-released for more details.
        /// </summary>
        /// <param name="user">User to remove from the cache</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use RemoveAccountAsync instead (See https://aka.ms/msal-net-2-released)", true)]
        public void Remove(IUser user) { throw new NotImplementedException(); }

        /// <summary>
        /// Identifier of the component (libraries/SDK) consuming MSAL.NET.
        /// This will allow for disambiguation between MSAL usage by the app vs MSAL usage by component libraries.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use WithComponent on AbstractApplicationBuilder<T> to configure this instead." + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        public string Component { get; set; }

        /// <summary>
        /// Sets or Gets a custom query parameters that may be sent to the STS for dogfood testing or debugging. This is a string of segments
        /// of the form <c>key=value</c> separated by an ampersand character.
        /// Unless requested otherwise, this parameter should not be set by application developers as it may have adverse effect on the application.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use ExtraQueryParameters on each call instead." + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        public string SliceParameters { get; set; }

        /// <summary>
        /// Gets/sets a boolean value telling the application if the authority needs to be verified against a list of known authorities. The default
        /// value is <c>true</c>. It should currently be set to <c>false</c> for Azure AD B2C authorities as those are customer specific
        /// (a list of known B2C authorities cannot be maintained by MSAL.NET). This property can be set just after the construction of the application
        /// and before an operation acquiring a token or interacting with the STS.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Can be set on AbstractApplicationBuilder<T>.WithAuthority as needed." + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        public bool ValidateAuthority { get; set; }

#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
        /// <summary>
        /// The redirect URI (also known as Reply URI or Reply URL), is the URI at which Azure AD will contact back the application with the tokens.
        /// This redirect URI needs to be registered in the app registration (https://aka.ms/msal-net-register-app).
        /// In MSAL.NET, <see cref="T:PublicClientApplication"/> define the following default RedirectUri values:
        /// <list type="bullet">
        /// <item><description><c>https://login.microsoftonline.com/common/oauth2/nativeclient</c> for desktop (.NET Framework and .NET Core) applications</description></item>
        /// <item><description><c>msal{ClientId}</c> for Xamarin iOS and Xamarin Android (as this will be used by the system web browser by default on these
        /// platforms to call back the application)
        /// </description></item>
        /// </list>
        /// These default URIs could change in the future.
        /// In <see cref="Microsoft.Identity.Client.ConfidentialClientApplication"/>, this can be the URL of the Web application / Web API.
        /// </summary>
        /// <remarks>This is especially important when you deploy an application that you have initially tested locally;
        /// you then need to add the reply URL of the deployed application in the application registration portal</remarks>
        [Obsolete("Should be set using AbstractApplicationBuilder<T>.WithRedirectUri and can be viewed with ClientApplicationBase.AppConfig.RedirectUri." + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        public string RedirectUri { get; set; }
#pragma warning restore CS1574 // XML comment has cref attribute that could not be resolved

        #region MSAL3X deprecations

        /// <summary>
        /// Gets the Client ID (also known as <i>Application ID</i>) of the application as registered in the application registration portal (https://aka.ms/msal-net-register-app)
        /// and as passed in the constructor of the application
        /// </summary>
        [Obsolete("Use AppConfig.ClientId instead." + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        public string ClientId => AppConfig.ClientId;

        /// <summary>
        /// [V2 API] Attempts to acquire an access token for the <paramref name="account"/> from the user token cache, with advanced parameters controlling network call.
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="account">Account for which the token is requested. <see cref="IAccount"/></param>
        /// <param name="authority">Specific authority for which the token is requested. Passing a different value than configured in the application constructor
        /// narrows down the selection to a specific tenant. This does not change the configured value in the application. This is specific
        /// to applications managing several accounts (like a mail client with several mailboxes)</param>
        /// <param name="forceRefresh">If <c>true</c>, ignore any access token in the cache and attempt to acquire new access token
        /// using the refresh token for the account if this one is available. This can be useful in the case when the application developer wants to make
        /// sure that conditional access policies are applied immediately, rather than after the expiration of the access token</param>
        /// <returns>An <see cref="AuthenticationResult"/> containing the requested access token</returns>
        /// <exception cref="MsalUiRequiredException">can be thrown in the case where an interaction is required with the end user of the application,
        /// for instance, if no refresh token was in the cache,a or the user needs to consent, or re-sign-in (for instance if the password expired),
        /// or performs two factor authentication</exception>
        /// <remarks>
        /// The access token is considered a match if it contains <b>at least</b> all the requested scopes. This means that an access token with more scopes than
        /// requested could be returned as well. If the access token is expired or close to expiration (within a 5 minute window),
        /// then the cached refresh token (if available) is used to acquire a new access token by making a silent network call.
        ///
        /// See https://aka.ms/msal-net-acquiretokensilent for more details
        /// </remarks>
        [Obsolete("Use AcquireTokenSilent instead." + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        public Task<AuthenticationResult> AcquireTokenSilentAsync(
            IEnumerable<string> scopes,
            IAccount account,
            string authority, bool forceRefresh)
        {
            throw MigrationHelper.CreateMsalNet3BreakingChangesException();
        }

        /// <summary>
        /// [V2 API] Attempts to acquire an access token for the <paramref name="account"/> from the user token cache.
        /// </summary>
        /// <param name="scopes">Scopes requested to access a protected API</param>
        /// <param name="account">Account for which the token is requested. <see cref="IAccount"/></param>
        /// <returns>An <see cref="AuthenticationResult"/> containing the requested token</returns>
        /// <exception cref="MsalUiRequiredException">can be thrown in the case where an interaction is required with the end user of the application,
        /// for instance so that the user consents, or re-signs-in (for instance if the password expired), or performs two factor authentication</exception>
        /// <remarks>
        /// The access token is considered a match if it contains <b>at least</b> all the requested scopes.
        /// This means that an access token with more scopes than requested could be returned as well. If the access token is expired or
        /// close to expiration (within a 5 minute window), then the cached refresh token (if available) is used to acquire a new access token by making a silent network call.
        ///
        /// See https://aka.ms/msal-net-acquiretokensilent for more details
        /// </remarks>
        [Obsolete("Use AcquireTokenSilent instead." + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        public Task<AuthenticationResult> AcquireTokenSilentAsync(IEnumerable<string> scopes, IAccount account)
        {
            throw MigrationHelper.CreateMsalNet3BreakingChangesException();
        }
        #endregion MSAL3X deprecations
    }

    public partial class AuthenticationResult
    {
        /// <summary>
        /// In MSAL.NET 1.x, returned the user who signed in to get the authentication result. From MSAL 2.x
        /// rather use <see cref="Account"/> instead. See https://aka.ms/msal-net-2-released for more details.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Obsolete("Use Account instead (See https://aka.ms/msal-net-2-released)", true)]
        public IUser User { get { throw new NotImplementedException(); } }
    }

    public partial class TokenCacheNotificationArgs
    {
        /// <summary>
        /// In MSAL.NET 1.x, returned the user who signed in to get the authentication result. From MSAL 2.x
        /// rather use <see cref="Account"/> instead. See https://aka.ms/msal-net-2-released for more details.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Obsolete("Use Account instead (See https://aka.ms/msal-net-2-released)", true)]
        public IUser User { get { throw new NotImplementedException(); } }
    }

    public partial interface IByRefreshToken
    {
        /// <summary>
        /// Acquires an access token from an existing refresh token and stores it and the refresh token into
        /// the user token cache, where it will be available for further AcquireTokenSilentAsync calls.
        /// This method can be used in migration to MSAL from ADAL v2 and in various integration
        /// scenarios where you have a RefreshToken available.
        /// (see https://aka.ms/msal-net-migration-adal2-msal2)
        /// </summary>
        /// <param name="scopes">Scope to request from the token endpoint.
        /// Setting this to null or empty will request an access token, refresh token and ID token with default scopes</param>
        /// <param name="refreshToken">The refresh token from ADAL 2.x</param>
        [Obsolete("Use AcquireTokenByRefreshToken instead. " + MsalErrorMessage.AkaMsmsalnet3BreakingChanges, true)]
        Task<AuthenticationResult> AcquireTokenByRefreshTokenAsync(IEnumerable<string> scopes, string refreshToken);
    }

    /// <summary>
    /// </summary>
    [Obsolete(MsalErrorMessage.LoggingClassIsObsolete, true)]
    public sealed class Logger
    {
        /// <summary>
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(MsalErrorMessage.LoggingClassIsObsolete, true)]
        public static LogCallback LogCallback
        {
            set => throw new NotImplementedException(MsalErrorMessage.LoggingClassIsObsolete);
        }

        /// <summary>
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(MsalErrorMessage.LoggingClassIsObsolete, true)]
        public static LogLevel Level
        {
            get => throw new NotImplementedException(MsalErrorMessage.LoggingClassIsObsolete);
            set => throw new NotImplementedException(MsalErrorMessage.LoggingClassIsObsolete);
        }

        /// <summary>
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(MsalErrorMessage.LoggingClassIsObsolete, true)]
        public static bool PiiLoggingEnabled { get; set; } = false;

        /// <summary>
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(MsalErrorMessage.LoggingClassIsObsolete, true)]
        public static bool DefaultLoggingEnabled { get; set; } = false;
    }

    /// <summary>
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete(MsalErrorMessage.TelemetryClassIsObsolete, true)]
    public class Telemetry : ITelemetryReceiver
    {
        /// <summary>
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(MsalErrorMessage.TelemetryClassIsObsolete, true)]
        public delegate void Receiver(List<Dictionary<string, string>> events);

        /// <summary>
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(MsalErrorMessage.TelemetryClassIsObsolete, true)]
        public static Telemetry GetInstance()
        {
            throw new NotImplementedException(MsalErrorMessage.TelemetryClassIsObsolete);
        }

        /// <summary>
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(MsalErrorMessage.TelemetryClassIsObsolete, true)]
        public bool TelemetryOnFailureOnly
        {
            get => throw new NotImplementedException(MsalErrorMessage.TelemetryClassIsObsolete);
            set => throw new NotImplementedException(MsalErrorMessage.TelemetryClassIsObsolete);
        }

        /// <summary>
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(MsalErrorMessage.TelemetryClassIsObsolete, true)]
        public void RegisterReceiver(Receiver r)
        {
            throw new NotImplementedException(MsalErrorMessage.TelemetryClassIsObsolete);
        }

        /// <summary>
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(MsalErrorMessage.TelemetryClassIsObsolete, true)]
        public bool HasRegisteredReceiver()
        {
            throw new NotImplementedException(MsalErrorMessage.TelemetryClassIsObsolete);
        }

        /// <summary>
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(MsalErrorMessage.TelemetryClassIsObsolete, true)]
        void ITelemetryReceiver.HandleTelemetryEvents(List<Dictionary<string, string>> events)
        {
            throw new NotImplementedException(MsalErrorMessage.TelemetryClassIsObsolete);
        }
    }
}
