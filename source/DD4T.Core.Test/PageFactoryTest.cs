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
using DD4T.Factories.Caching;
using DD4T.ContentModel;
using DD4T.ContentModel.Contracts.Serializing;
using DD4T.Serialization;

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

    }
}
