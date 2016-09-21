using DD4T.ContentModel;
using DD4T.ContentModel.Contracts.Caching;
using DD4T.ContentModel.Contracts.Providers;
using DD4T.ContentModel.Contracts.Serializing;
using DD4T.ContentModel.Factories;
using DD4T.Core.Contracts.ViewModels;
using DD4T.Factories;
using DD4T.Providers.Test;
using DD4T.Serialization;
using DD4T.ViewModels;
using DD4T.ViewModels.Attributes;
using DD4T.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace DD4T.Core.Test
{
    public class ViewModelFactoryTest : BaseFactoryTest
    {
        //public static void Setup(TestContext context)
        //{
        //    Initialize();
        //    ViewModelFactory.LoadViewModels(new[] { typeof(TestViewModelA).Assembly });
        //}
        static ViewModelFactoryTest()
        {
            Initialize();
            var assembly = typeof(TestViewModelA).GetTypeInfo().Assembly;
            ViewModelFactory.LoadViewModels(new[] { assembly });
        }

        [Fact]
        public void MapAbstractLink()
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("", "componentlink");
            Assert.NotNull(cp);

            IViewModel vm = ViewModelFactory.BuildViewModel(cp);
            Assert.NotNull(vm);

            TestViewModelB b = ((TestViewModelA)vm).Link[0] as TestViewModelB;
            Assert.NotNull(b);

            Assert.True(b.Heading == "some heading");
        }

        [Fact]
        public void MapComponentId()
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("", "componentId");
            Assert.NotNull(cp);

            IViewModel vm = ViewModelFactory.BuildViewModel(cp);
            Assert.NotNull(vm);

            TestViewModelA b = (TestViewModelA)vm;
            Assert.NotNull(b);

            Assert.True(b.Id.ItemId == 8975);
            Assert.True(b.Id.PublicationId == 5);
        }

        [Fact]
        public void MapConcreteLink()
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("", "componentlink");
            Assert.NotNull(cp);

            IViewModel vm = ViewModelFactory.BuildViewModel(cp);
            Assert.NotNull(vm);

            TestViewModelB b = ((TestViewModelA)vm).ConcreteLink[0];
            Assert.NotNull(b);

            Assert.True(b.Heading == "some heading");
        }

        [Fact]
        public void MapFieldsIgnoreCase()
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("", "componentIgnoreCase");
            Assert.NotNull(cp);

            //ViewModelFactory.LoadViewModels();
            IViewModel vm = ViewModelFactory.BuildViewModel(cp);
            Assert.NotNull(vm);

            TestViewModelB b = ((TestViewModelA)vm).ConcreteLink[0];
            Assert.NotNull(b);

            /// Assert.True(b.Heading == "some heading");
        }

        [Fact]
        public void MapConcreteEmbeddedField()
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("", "embedded");
            Assert.NotNull(cp);

            EmbeddingModel vm = ViewModelFactory.BuildViewModel(cp) as EmbeddingModel;
            Assert.NotNull(vm);

            EmbeddedModel em = vm.ConcreteEmbedded;
            Assert.NotNull(em);

            Assert.True(em.Heading == "some heading");
        }

        [Fact]
        public void MapAbstractEmbeddedField()
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("", "embedded");
            Assert.NotNull(cp);

            EmbeddingModel vm = ViewModelFactory.BuildViewModel(cp) as EmbeddingModel;
            Assert.NotNull(vm);

            EmbeddedModel em = vm.Embedded as EmbeddedModel;
            Assert.NotNull(em);

            Assert.True(em.Heading == "some heading");
        }

        [Fact]
        public void MapAbstractKeywordField()
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("", "keyword");
            Assert.NotNull(cp);

            KeywordContainingModel vm = ViewModelFactory.BuildViewModel(cp) as KeywordContainingModel;
            Assert.NotNull(vm);

            KeywordModel km = vm.Keyword as KeywordModel;
            Assert.NotNull(km);

            Assert.True(km.Heading == "some heading");
        }

        [Fact]
        public void MapConcreteKeywordField()
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("", "keyword");
            Assert.NotNull(cp);

            KeywordContainingModel vm = ViewModelFactory.BuildViewModel(cp) as KeywordContainingModel;
            Assert.NotNull(vm);

            KeywordModel km = vm.ConcreteKeyword;
            Assert.NotNull(km);

            Assert.True(km.Heading == "some heading");
        }
    }

    [DD4T.ViewModels.Attributes.ContentModel("rootA", true)]
    public class TestViewModelA : TestViewModelBase, ITestViewModel
    {
        [LinkedComponentField(FieldName = "link")]
        public List<TestViewModelB> ConcreteLink { get; set; }

        [ComponentId]
        public TcmUri Id { get; set; }

        [LinkedComponentField(LinkedComponentTypes = new[] { typeof(TestViewModelB) })]
        public List<ITestViewModel> Link { get; set; }
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
        [EmbeddedSchemaField(FieldName = "embedded")]
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
        [TextField(IsMetadata = true)]
        public string Heading { get; set; }
    }

    public abstract class TestViewModelBase : ViewModelBase
    {
    }

    public interface ITestViewModel : IViewModel
    {
    }
}