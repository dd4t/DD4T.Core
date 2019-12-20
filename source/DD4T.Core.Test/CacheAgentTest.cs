using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DD4T.ContentModel.Factories;
using Ninject.Modules;
using DD4T.Factories;
using Ninject;
using System.Reflection;
using DD4T.Providers.Test;
using DD4T.ContentModel.Contracts.Providers;
using DD4T.ContentModel.Contracts.Caching;
using DD4T.ContentModel;
using DD4T.ContentModel.Contracts.Serializing;
using DD4T.Serialization;
using System.Collections.Generic;
using DD4T.Utils.Caching;

namespace DD4T.Core.Test
{
    [TestClass]
    public class CacheAgentTest : BaseFactoryTest
    {
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            Initialize();

        }

        [TestMethod]
        public void CacheInvalidationUdp()
        {
            TestConfiguration.OverrideUdpEnabled = true;
            TestConfiguration.OverridePageExpiration = 300;
            MockMessageProvider messageProvider = new MockMessageProvider();
            var page = PageFactory.FindPage("/cachetest.html");
            TcmUri pageUri = new TcmUri(page.Id);
            var page2 = PageFactory.FindPage("/cachetest.html");
            // the test page provider will set a random tcm uri, so if the uri has not changed, the page was returned from the cache
            Assert.AreEqual(page2.Id, page.Id, "page should have come from the cache but it didn't");

            var cacheEvent = GenerateCacheEvent(pageUri, true);
            // TODO: subscribe and use message provider
            //messageProvider.BroadcastCacheEvent(cacheEvent);
            ((DefaultCacheAgent)CacheAgent).OnNext(cacheEvent);
            var page3 = PageFactory.FindPage("/cachetest.html");
            Assert.AreNotEqual(page3.Id, page.Id, "page shouldn't have come from the cache but it did");


            // try to invalidate with incorrect (non-UDP) key
            ((DefaultCacheAgent)CacheAgent).OnNext(GenerateCacheEvent(pageUri, false));
            var page4 = PageFactory.FindPage("/cachetest.html");
            Assert.AreEqual(page4.Id, page3.Id, "page should still have come from the cache because a non-UDP cache key was provided, but it didn't");

        }

        [TestMethod]
        public void CacheInvalidationNonUdp()
        {
            TestConfiguration.OverrideUdpEnabled = false;
            TestConfiguration.OverridePageExpiration = 300;
            var page = PageFactory.FindPage("/cachetest-non-udp.html");
            TcmUri pageUri = new TcmUri(page.Id);
            var page2 = PageFactory.FindPage("/cachetest-non-udp.html");
            // the test page provider will set a random tcm uri, so if the uri has not changed, the page was returned from the cache
            Assert.AreEqual(page2.Id, page.Id, "page should have come from the cache but it didn't");

            ((DefaultCacheAgent)CacheAgent).OnNext(GenerateCacheEvent(pageUri, false));
            var page3 = PageFactory.FindPage("/cachetest-non-udp.html");


            Assert.AreNotEqual(page3.Id, page.Id, "page shouldn't have come from the cache but it did");

            // try to invalidate with incorrect (UDP) key
            ((DefaultCacheAgent)CacheAgent).OnNext(GenerateCacheEvent(pageUri, true));
            var page4 = PageFactory.FindPage("/cachetest-non-udp.html");
            Assert.AreEqual(page4.Id, page3.Id, "page should still have come from the cache because a UDP cache key was provided, but it didn't");
        }

        public static CacheEvent GenerateCacheEvent(TcmUri pageUri, bool udpEnabled = true)
        {
            return new CacheEvent()
            {
                Key = (udpEnabled ? "1:" : "") + $"{pageUri.PublicationId}:{pageUri.ItemId}",
                RegionPath = "/com.tridion.storage.ItemMeta",
                Type = 1
            };
        }
    }
}
