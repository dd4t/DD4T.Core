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

namespace DD4T.Core.Test
{
    [TestClass]
    public class ComponentPresentationFactoryTest : BaseFactoryTest
    {
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            Initialize();
        }

        [TestMethod]
        public void GetComponentPresentation()
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("");
            Assert.IsNotNull(cp);
            Assert.IsFalse(string.IsNullOrEmpty(cp.Component.Title) || string.IsNullOrEmpty(cp.ComponentTemplate.Title));
            Assert.IsNotNull(cp.Conditions[0]);
        }

        [TestMethod]
        public void GetComponentPresentationFromComponent()
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("component");
            Assert.IsNotNull(cp);
            Assert.IsFalse(string.IsNullOrEmpty(cp.Component.Title) || cp.ComponentTemplate == null);
        }

        [TestMethod]
        public void GetComponentPresentations()
        {
            IList<IComponentPresentation> cps = ComponentPresentationFactory.GetComponentPresentations(new string[] { "tcm:0-1", "tcm:0-2" });
            Assert.IsTrue(cps.Count == 2);
            Assert.IsNotNull(cps[0].Component.Title);
        }

        [TestMethod]
        public void GetLastPublishedDate()
        {
            DateTime lastPublishDate = ComponentPresentationFactory.GetLastPublishedDate("tcm:0-1");
            Assert.IsFalse(lastPublishDate == default(DateTime));
        }
    }
}
