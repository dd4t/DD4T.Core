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
using DD4T.Utils.Caching;

namespace DD4T.Core.Test
{
    [TestClass]
    public class LinkFactoryTest : BaseFactoryTest
    {
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            Initialize();
        }

        [TestMethod]
        public void ResolveLink()
        {
            var link = LinkFactory.ResolveLink("tcm:2-123");
           
            Assert.IsNotNull(link);
            Assert.IsFalse(string.IsNullOrEmpty(link));
            Assert.AreEqual(link, TridionLinkProvider.link1);
        }

        [TestMethod]
        public void ResolveLinkWithTemplateUri()
        {
            var link = LinkFactory.ResolveLink("tcm:2-456-64", "tcm:2-123", "tcm:2-789-32");

            Assert.IsNotNull(link);
            Assert.IsFalse(string.IsNullOrEmpty(link));
            Assert.AreEqual(link, TridionLinkProvider.link2);
        }

        [TestMethod]
        public void GetLinkFromCache()
        {
            var link = LinkFactory.ResolveLink("tcm:2-123");

            Assert.IsNotNull(link);
            Assert.IsFalse(string.IsNullOrEmpty(link));

            TridionLinkProvider.link1 = "something else";
            var newLink = LinkFactory.ResolveLink("tcm:2-123");

            // Note: changing the underlying value of the link shouldn't matter because the LinkFactory should cache the link anyway
            Assert.AreEqual(link, newLink);

        }
    }
}
