// ------------------------------------------------------------------------------
// 
// Copyright (c) Microsoft Corporation.
// All rights reserved.
// 
// This code is licensed under the MIT License.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Cache;
using Microsoft.Identity.Client.Core;
using Microsoft.Identity.Client.Instance;
using Microsoft.Identity.Client.Internal.Requests;
using Microsoft.Identity.Client.TelemetryCore;
using Microsoft.Identity.Client.Utils;
using Microsoft.Identity.Test.Common;
using Microsoft.Identity.Test.Common.Core.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Identity.Test.Unit.RequestsTests
{
    [TestClass]
    public class SilentRequestTests
    {
        private TokenCacheHelper _tokenCacheHelper;

        [TestInitialize]
        public void TestInitialize()
        {
            TestCommon.ResetStateAndInitMsal();
            _tokenCacheHelper = new TokenCacheHelper();
        }

        [TestMethod]
        [TestCategory("SilentRequestTests")]
        public void ConstructorTests()
        {
            using (var harness = new MockHttpTestHarness(MsalTestConstants.AuthorityHomeTenant))
            {
                var parameters = harness.CreateRequestParams(harness.Cache);
                var request = new SilentRequest(harness.ServiceBundle, parameters, ApiEvent.ApiIds.None, false);
                Assert.IsNotNull(request);

                parameters.Account = new Account(MsalTestConstants.UserIdentifier, MsalTestConstants.DisplayableId, null);
                request = new SilentRequest(harness.ServiceBundle, parameters, ApiEvent.ApiIds.None, false);
                Assert.IsNotNull(request);

                request = new SilentRequest(harness.ServiceBundle, parameters, ApiEvent.ApiIds.None, false);
                Assert.IsNotNull(request);
            }
        }

        [TestMethod]
        [TestCategory("SilentRequestTests")]
        public void ExpiredTokenRefreshFlowTest()
        {
            using (var harness = new MockHttpTestHarness(MsalTestConstants.AuthorityHomeTenant))
            {
                _tokenCacheHelper.PopulateCache(harness.Cache.Accessor);
                var parameters = harness.CreateRequestParams(harness.Cache);

                // set access tokens as expired
                foreach (string atCacheItemStr in harness.Cache.GetAllAccessTokenCacheItems(RequestContext.CreateForTest(harness.ServiceBundle)))
                {
                    var accessItem = JsonHelper.DeserializeFromJson<MsalAccessTokenCacheItem>(atCacheItemStr);
                    accessItem.ExpiresOnUnixTimestamp =
                        ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds)
                        .ToString(CultureInfo.InvariantCulture);

                    harness.Cache.AddAccessTokenCacheItem(accessItem);
                }

                TestCommon.MockInstanceDiscoveryAndOpenIdRequest(harness.HttpManager);

                harness.HttpManager.AddMockHandler(
                    new MockHttpMessageHandler()
                    {
                        Method = HttpMethod.Post,
                        ResponseMessage = MockHelpers.CreateSuccessTokenResponseMessage()
                    });

                var request = new SilentRequest(harness.ServiceBundle, parameters, ApiEvent.ApiIds.None, false);

                Task<AuthenticationResult> task = request.RunAsync(CancellationToken.None);
                var result = task.Result;
                Assert.IsNotNull(result);
                Assert.AreEqual("some-access-token", result.AccessToken);
                Assert.AreEqual(MsalTestConstants.Scope.AsSingleString(), result.Scopes.AsSingleString());
            }
        }

        [TestMethod]
        [TestCategory("SilentRequestTests")]
        public void SilentRefreshFailedNullCacheTest()
        {
            using (var harness = new MockHttpTestHarness(MsalTestConstants.AuthorityHomeTenant))
            {
                var parameters = new AuthenticationRequestParameters()
                {
                    Authority = harness.Authority,
                    ClientId = MsalTestConstants.ClientId,
                    Scope = ScopeHelper.CreateSortedSetFromEnumerable(
                        new[]
                        {
                            "some-scope1",
                            "some-scope2"
                        }),
                    TokenCache = null,
                    Account = new Account(MsalTestConstants.UserIdentifier, MsalTestConstants.DisplayableId, null),
                    RequestContext = RequestContext.CreateForTest(harness.ServiceBundle)
                };

                try
                {
                    var request = new SilentRequest(harness.ServiceBundle, parameters, ApiEvent.ApiIds.None, false);
                    Task<AuthenticationResult> task = request.RunAsync(CancellationToken.None);
                    var authenticationResult = task.Result;
                    Assert.Fail("MsalUiRequiredException should be thrown here");
                }
                catch (AggregateException ae)
                {
                    var exc = ae.InnerException as MsalUiRequiredException;
                    Assert.IsNotNull(exc);
                    Assert.AreEqual(MsalUiRequiredException.TokenCacheNullError, exc.ErrorCode);
                }
            }
        }

        [TestMethod]
        [TestCategory("SilentRequestTests")]
        public void SilentRefreshFailedNoCacheItemFoundTest()
        {
            using (var harness = new MockHttpTestHarness(MsalTestConstants.AuthorityHomeTenant))
            {
                harness.HttpManager.AddInstanceDiscoveryMockHandler();

                var parameters = new AuthenticationRequestParameters()
                {
                    Authority = harness.Authority,
                    ClientId = MsalTestConstants.ClientId,
                    Scope = ScopeHelper.CreateSortedSetFromEnumerable(
                        new[]
                        {
                            "some-scope1",
                            "some-scope2"
                        }),
                    TokenCache = harness.Cache,
                    Account = new Account(MsalTestConstants.UserIdentifier, MsalTestConstants.DisplayableId, null),
                    RequestContext = RequestContext.CreateForTest(harness.ServiceBundle)
                };

                try
                {
                    var request = new SilentRequest(harness.ServiceBundle, parameters, ApiEvent.ApiIds.None, false);
                    Task<AuthenticationResult> task = request.RunAsync(CancellationToken.None);
                    var authenticationResult = task.Result;
                    Assert.Fail("MsalUiRequiredException should be thrown here");
                }
                catch (AggregateException ae)
                {
                    var exc = ae.InnerException as MsalUiRequiredException;
                    Assert.IsNotNull(exc, "Actual exception type is " + ae.InnerException.GetType());
                    Assert.AreEqual(MsalUiRequiredException.NoTokensFoundError, exc.ErrorCode);
                }
            }
        }

        private class MockHttpTestHarness : IDisposable
        {
            private readonly MockHttpAndServiceBundle _mockHttpAndServiceBundle;

            public MockHttpTestHarness(string authorityUri)
            {
                _mockHttpAndServiceBundle = new MockHttpAndServiceBundle();
                Authority = Authority.CreateAuthority(ServiceBundle, authorityUri);
                Cache = new TokenCache(ServiceBundle);
            }

            public IServiceBundle ServiceBundle => _mockHttpAndServiceBundle.ServiceBundle;
            public MockHttpManager HttpManager => _mockHttpAndServiceBundle.HttpManager;
            public Authority Authority { get; }
            public ITokenCacheInternal Cache { get; }

            /// <inheritdoc />
            public void Dispose()
            {
                _mockHttpAndServiceBundle.Dispose();
            }

            public AuthenticationRequestParameters CreateRequestParams(ITokenCacheInternal cache)
            {
                var parameters = new AuthenticationRequestParameters()
                {
                    Authority = Authority,
                    ClientId = MsalTestConstants.ClientId,
                    Scope = MsalTestConstants.Scope,
                    TokenCache = cache,
                    Account = new Account(MsalTestConstants.UserIdentifier, MsalTestConstants.DisplayableId, null),
                    RequestContext = RequestContext.CreateForTest(ServiceBundle)
                };
                return parameters;
            }
        }
    }
}