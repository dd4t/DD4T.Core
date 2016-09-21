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
//using System.Collections.Generic;

//namespace DD4T.Core.Test
//{
//    public class ComponentPresentationFactoryTest : BaseFactoryTest
//    {
//        public static void Setup(TestContext context)
//        {
//            Initialize();
//        }

//        [Fact]
//        public void GetComponentPresentation()
//        {
//            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("");
//            Assert.NotNull(cp);
//            Assert.False(string.IsNullOrEmpty(cp.Component.Title) || string.IsNullOrEmpty(cp.ComponentTemplate.Title));
//            Assert.NotNull(cp.Conditions[0]);
//        }

//        [Fact]
//        public void GetComponentPresentationFromComponent()
//        {
//            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("component");
//            Assert.NotNull(cp);
//            Assert.False(string.IsNullOrEmpty(cp.Component.Title) || cp.ComponentTemplate == null);
//        }

//        [Fact]
//        public void GetComponentPresentations()
//        {
//            IList<IComponentPresentation> cps = ComponentPresentationFactory.GetComponentPresentations(new string[] { "tcm:0-1", "tcm:0-2" });
//            Assert.True(cps.Count == 2);
//            Assert.NotNull(cps[0].Component.Title);
//        }

//        [Fact]
//        public void GetLastPublishedDate()
//        {
//            DateTime lastPublishDate = ComponentPresentationFactory.GetLastPublishedDate("tcm:0-1");
//            Assert.False(lastPublishDate == default(DateTime));
//        }
//    }
//}