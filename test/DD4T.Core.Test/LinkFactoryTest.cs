//using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using DD4T.ContentModel.Factories;
//using Ninject.Modules;
//using DD4T.Factories;
//using Ninject;
//using System.Reflection;
//using DD4T.Providers.Test;
//using DD4T.ContentModel.Contracts.Providers;
//using DD4T.ContentModel.Contracts.Caching;
//using DD4T.ContentModel;
//using DD4T.ContentModel.Contracts.Serializing;
//using DD4T.Serialization;
//using DD4T.Utils.Caching;

//namespace DD4T.Core.Test
//{
//    public class LinkFactoryTest : BaseFactoryTest
//    {
//        public static void Setup(TestContext context)
//        {
//            Initialize();
//        }

//        [Fact]
//        public void ResolveLink()
//        {
//            TridionLinkProvider.link1 = "/something";
//            var link = LinkFactory.ResolveLink("tcm:2-123");

//            Assert.NotNull(link);
//            Assert.False(string.IsNullOrEmpty(link));
//            Assert.AreEqual(link, "/something");
//        }

//        [Fact]
//        public void ResolveLinkWithTemplateUri()
//        {
//            TridionLinkProvider.link2 = "/something";
//            var link = LinkFactory.ResolveLink("tcm:2-456-64", "tcm:2-123", "tcm:2-789-32");

//            Assert.NotNull(link);
//            Assert.False(string.IsNullOrEmpty(link));
//            Assert.AreEqual(link, "/something");
//        }

//        [Fact]
//        public void GetLinkFromCache()
//        {
//            TridionLinkProvider.link1 = "/something";
//            var link = LinkFactory.ResolveLink("tcm:2-123");

//            Assert.NotNull(link);
//            Assert.False(string.IsNullOrEmpty(link));

//            TridionLinkProvider.link1 = "/something/else";
//            var newLink = LinkFactory.ResolveLink("tcm:2-123");

//            // Note: changing the underlying value of the link shouldn't matter because the LinkFactory should cache the link anyway
//            Assert.AreEqual(link, newLink);

//        }
//    }
//}