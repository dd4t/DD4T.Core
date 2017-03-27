﻿using System;
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
using DD4T.Core.Contracts.ViewModels;
using DD4T.ViewModels.Attributes;
using DD4T.ViewModels;
using DD4T.ViewModels.Base;

namespace DD4T.Core.Test
{
    [TestClass]
    public class ViewModelFactoryTest : BaseFactoryTest
    {
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            Initialize();
            ViewModelFactory.LoadViewModels(new[] { typeof(TestViewModelA).Assembly }); 
        }

        [TestMethod]
        public void MapAbstractLink()
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("", "componentlink");
            Assert.IsNotNull(cp);

            IViewModel vm = ViewModelFactory.BuildViewModel(cp);
            Assert.IsNotNull(vm);

            TestViewModelB b = ((TestViewModelA)vm).Link[0] as TestViewModelB;
            Assert.IsNotNull(b);

            Assert.IsTrue(b.Heading == "some heading");
        }

        [TestMethod]
        public void MapComponentId()
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("", "componentId");
            Assert.IsNotNull(cp);

            IViewModel vm = ViewModelFactory.BuildViewModel(cp);
            Assert.IsNotNull(vm);

            TestViewModelA b = (TestViewModelA)vm;
            Assert.IsNotNull(b);

            Assert.IsTrue(b.Id.ItemId == 8975);
            Assert.IsTrue(b.Id.PublicationId == 5);
        }

        [TestMethod]
        public void MapConcreteLink()
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("", "componentlink");
            Assert.IsNotNull(cp);

            IViewModel vm = ViewModelFactory.BuildViewModel(cp);
            Assert.IsNotNull(vm);

            TestViewModelB b = ((TestViewModelA)vm).ConcreteLink[0];
            Assert.IsNotNull(b);

            Assert.IsTrue(b.Heading == "some heading");
        }

        [TestMethod]
        public void MapFieldsIgnoreCase()
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("", "componentIgnoreCase");
            Assert.IsNotNull(cp);

            //ViewModelFactory.LoadViewModels();
            IViewModel vm = ViewModelFactory.BuildViewModel(cp);
            Assert.IsNotNull(vm);


            TestViewModelB b = ((TestViewModelA)vm).ConcreteLink[0];
            Assert.IsNotNull(b);

           /// Assert.IsTrue(b.Heading == "some heading");
        }

        [TestMethod]
        public void MapConcreteEmbeddedField()
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("", "embedded");
             Assert.IsNotNull(cp);

            EmbeddingModel vm = ViewModelFactory.BuildViewModel(cp) as EmbeddingModel;
            Assert.IsNotNull(vm);

            EmbeddedModel em = vm.ConcreteEmbedded;
            Assert.IsNotNull(em);

            Assert.IsTrue(em.Heading == "some heading");
        }

        [TestMethod]
        public void MapAbstractEmbeddedField()
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("", "embedded");
            Assert.IsNotNull(cp);

            EmbeddingModel vm = ViewModelFactory.BuildViewModel(cp) as EmbeddingModel;
            Assert.IsNotNull(vm);

            EmbeddedModel em = vm.Embedded as EmbeddedModel;
            Assert.IsNotNull(em);

            Assert.IsTrue(em.Heading == "some heading");
        }

        [TestMethod]
        public void MapAbstractKeywordField()
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("", "keyword");
            Assert.IsNotNull(cp);

            KeywordContainingModel vm = ViewModelFactory.BuildViewModel(cp) as KeywordContainingModel;
            Assert.IsNotNull(vm);

            KeywordModel km = vm.Keyword as KeywordModel;
            Assert.IsNotNull(km);

            Assert.IsTrue(km.Heading == "some heading");
        }

        [TestMethod]
        public void MapConcreteKeywordField()
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("", "keyword");
            Assert.IsNotNull(cp);

            KeywordContainingModel vm = ViewModelFactory.BuildViewModel(cp) as KeywordContainingModel;
            Assert.IsNotNull(vm);

            KeywordModel km = vm.ConcreteKeyword;
            Assert.IsNotNull(km);

            Assert.IsTrue(km.Heading == "some heading");
        }


        [TestMethod]
        public void PageId()
        {
            IPage page = PageFactory.FindPage("/page-which-matches-a-model");
            TestPageModel model = ViewModelFactory.BuildViewModel(page) as TestPageModel;
            Assert.IsNotNull(model.PageId, "PageId is not set");
        }

        [TestMethod]
        public void ComponentId()
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("", "componentlink");
            TestViewModelA model = ViewModelFactory.BuildViewModel(cp) as TestViewModelA;
            Assert.IsNotNull(model.Id, "ComponentId is not set");
        }

        [TestMethod]
        public void KeywordId()
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("", "keyword");
            Assert.IsNotNull(cp);

            KeywordContainingModel vm = ViewModelFactory.BuildViewModel(cp) as KeywordContainingModel;
            Assert.IsNotNull(vm);

            KeywordModel km = vm.ConcreteKeyword;
            Assert.IsNotNull(km);

            Assert.IsNotNull(km.KeywordId, "KeywordId is not set");

        }

    }



    [PageViewModel(TemplateTitle = "Standard")]
    public class TestPageModel : TestViewModelBase
    {
        [PageId]
        public DD4T.ContentModel.TcmUri PageId { get; set; }

    }

    [DD4T.ViewModels.Attributes.ContentModel("rootA", true)]
    public class TestViewModelA : TestViewModelBase, ITestViewModel
    {
        [ComponentId]
        public DD4T.ContentModel.TcmUri Id { get; set; }

        [LinkedComponentField(LinkedComponentTypes = new[] {  typeof(TestViewModelB)})]
        public List<ITestViewModel> Link { get; set; }

        [LinkedComponentField(FieldName="link")]
        public List<TestViewModelB> ConcreteLink { get; set; }
    }

    [DD4T.ViewModels.Attributes.ContentModel("rootB", true)]
    public class TestViewModelB : TestViewModelBase, ITestViewModel
    {
        [TextField]
        public string Heading { get; set; }
    }

    [ContentModel("rootEmbedding", true)]
    public class EmbeddingModel : ViewModelBase
    {
        [EmbeddedSchemaField(FieldName="embedded")]
        public EmbeddedModel ConcreteEmbedded { get; set; }

        [EmbeddedSchemaField(EmbeddedModelType = typeof(EmbeddedModel))]
        public ViewModelBase Embedded { get; set; }

    }

    [ContentModel("embeddedRoot", false)]
    public class EmbeddedModel : ViewModelBase
    {
        [TextField]
        public string Heading { get; set; }        
    }

    [ContentModel("hasKeyword", true)]
    public class KeywordContainingModel : ViewModelBase
    {
        [KeywordField(FieldName = "keyword")]
        public KeywordModel ConcreteKeyword { get; set; }

        [KeywordField(KeywordType = typeof(KeywordModel))]
        public ViewModelBase Keyword { get; set; }
    }

    [ContentModel("keyword", false)]
    public class KeywordModel : ViewModelBase
    {
        [KeywordId]
        public DD4T.ContentModel.TcmUri KeywordId { get; set; }


        [TextField(IsMetadata=true)]
        public string Heading { get; set; }
    }


    public abstract class TestViewModelBase : ViewModelBase
    {

    }

    public interface ITestViewModel : IViewModel
    {

    }
}
