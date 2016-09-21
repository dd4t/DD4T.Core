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

//namespace DD4T.Core.Test
//{
//    public class ComponentFactoryTest : BaseFactoryTest
//    {
//        public static void Setup(TestContext context)
//        {
//            Initialize();
//        }

//        [Fact]
//        public void GetComponent()
//        {
//            IComponent component = ComponentFactory.GetComponent("");
//            Assert.NotNull(component);
//            Assert.False(string.IsNullOrEmpty(component.Title));
//        }

//    }
//}