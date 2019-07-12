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
using DD4T.ContentModel.Exceptions;

namespace DD4T.Core.Test
{
    [TestClass]
    public class PageFactoryTest : BaseFactoryTest
    {
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            Initialize();
        }

        [TestMethod]
        public void FindPage()
        {
            IPage page = PageFactory.FindPage("/index.html");
            Assert.IsNotNull(page);
            Assert.IsFalse(string.IsNullOrEmpty(page.Title));
        }

        [TestMethod]
        public void PageNotFound()
        {
            ((TridionPageProvider)PageFactory.PageProvider).ThrowPageNotFound = true;
            try
            {
                IPage page = PageFactory.FindPage("/index-page-not-found.html");
                Assert.Fail("PageFactory did not throw a PageNotFoundException");
            }
            catch (PageNotFoundException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void CacheNullResult()
        {
           
            int configuredPageExpiration = TestConfiguration.OverridePageExpiration;
            TestConfiguration.OverridePageExpiration = 60;           
            ((TridionPageProvider)PageFactory.PageProvider).ThrowPageNotFound = true;
            try
            {
                IPage page = PageFactory.FindPage("/test-null-result.html");
                Assert.Fail("PageFactory did not throw a PageNotFoundException");
            }
            catch (PageNotFoundException)
            {
            }
            ((TridionPageProvider)PageFactory.PageProvider).ThrowPageNotFound = false;
            try
            {
                IPage page = PageFactory.FindPage("/test-null-result.html");
                Assert.Fail("PageFactory did not throw a PageNotFoundException");
            }
            catch (PageNotFoundException)
            {
                Assert.IsTrue(true);
            }
            TestConfiguration.OverridePageExpiration = configuredPageExpiration;           

        }

    }
}
