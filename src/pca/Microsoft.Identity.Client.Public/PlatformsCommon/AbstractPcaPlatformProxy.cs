// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.Identity.Client.AuthScheme.PoP;
using Microsoft.Identity.Client.Cache;
using Microsoft.Identity.Client.Core;
using Microsoft.Identity.Client.Internal.Broker;
using Microsoft.Identity.Client.PlatformsCommon.Interfaces;
using Microsoft.Identity.Client.Public.PlatformsCommon;
using Microsoft.Identity.Client.UI;

namespace Microsoft.Identity.Client.PlatformsCommon
{
    internal abstract class AbstractPcaPlatformProxy : IPcaPlatformProxy
    {
        protected ICoreLogger Logger { get; }

        protected AbstractPcaPlatformProxy(ICoreLogger logger)
        {
            Logger = logger;
        }

        public virtual bool IsSystemWebViewAvailable
        {
            get
            {
                return true;
            }
        }

        public virtual bool UseEmbeddedWebViewDefault
        {
            get
            {
                return true;
            }
        }

        protected IWebUIFactory OverloadWebUiFactory { get; set; }

        /// <inheritdoc />
        public IWebUIFactory GetWebUiFactory()
        {
            return OverloadWebUiFactory ?? CreateWebUiFactory();
        }

        /// <inheritdoc />
        public void SetWebUiFactory(IWebUIFactory webUiFactory)
        {
            OverloadWebUiFactory = webUiFactory;
        }

        /// <inheritdoc />
        public abstract Task<string> GetUserPrincipalNameAsync();

        /// <inheritdoc />
        public abstract bool IsDomainJoined();

        /// <inheritdoc />
        public abstract Task<bool> IsUserLocalAsync(RequestContext requestContext);

        protected IBroker OverloadBrokerForTest { get; private set; }
        protected abstract IWebUIFactory CreateWebUiFactory();

        public void SetBrokerForTest(IBroker broker)
        {
            OverloadBrokerForTest = broker;
        }

        public virtual Task StartDefaultOsBrowserAsync(string url)
        {
            throw new NotImplementedException();
        }

        public virtual IBroker CreateBroker(CoreUIParent uIParent)
        {
            return OverloadBrokerForTest ?? new NullBroker();
        }

        public virtual bool CanBrokerSupportSilentAuth()
        {
            return false;
        }
    }
}
