using DD4T.ContentModel.Contracts.Caching;
using DD4T.ContentModel.Contracts.Configuration;
using DD4T.ContentModel.Contracts.Logging;
using DD4T.ContentModel.Contracts.Providers;
using DD4T.ContentModel.Factories;
using DD4T.Core.Contracts.Resolvers;
using DD4T.Core.Contracts.ViewModels;
using DD4T.Factories;
using DD4T.Providers.Test;
using DD4T.Serialization;
using DD4T.Utils.Logging;
using DD4T.Utils.Resolver;
using DD4T.ViewModels;
using DD4T.ViewModels.Reflection;

namespace DD4T.Core.Test
{
    public abstract class BaseFactoryTest
    {
        protected static IPageFactory PageFactory { get; set; }
        protected static IBinaryFactory BinaryFactory { get; set; }
        protected static IComponentPresentationFactory ComponentPresentationFactory { get; set; }
        protected static IComponentFactory ComponentFactory { get; set; }
        protected static ILinkFactory LinkFactory { get; set; }
        protected static ICacheAgent DefaultCacheAgent { get; set; }
        protected static IViewModelFactory ViewModelFactory { get; set; }

        protected static ILinkResolver LinkResolver { get; set; }
        protected static IRichTextResolver RichTextResolver { get; set; }

        public static void Initialize()
        {
            ViewModelFactory = MockViewModelFactory();
            LinkFactory = MockLinkFactory();
            LinkResolver = MockLinkResolver();
            RichTextResolver = MockRichTextResolver();
            //var cpFactory = new TridionComponentPresentationProvider();
            ComponentPresentationFactory = MockCP();
            PageFactory = MockPageFactory();
            //var kernel = new StandardKernel(new RegistrationModule());
            //kernel.Load("DD4T.ContentModel.Contracts");
            //kernel.Load("DD4T.Factories");
            //kernel.Load("DD4T.Providers.Test");
            //kernel.Load("DD4T.ViewModels");
            //PageFactory = kernel.Get<IPageFactory>();
            //BinaryFactory = kernel.Get<IBinaryFactory>();
            //ComponentPresentationFactory = kernel.Get<IComponentPresentationFactory>();
            //ComponentFactory = kernel.Get<IComponentFactory>();
            //LinkFactory = kernel.Get<ILinkFactory>();
            //ComponentPresentationFactory.CacheAgent = PageFactory.CacheAgent = kernel.Get<ICacheAgent>();
            //PageFactory.PageProvider = kernel.Get<IPageProvider>();
            //ComponentPresentationFactory.ComponentPresentationProvider = kernel.Get<IComponentPresentationProvider>();
            //((ComponentFactory)ComponentFactory).ComponentPresentationFactory = ComponentPresentationFactory;
            //((TridionPageProvider)PageFactory.PageProvider).SerializerService = kernel.Get<ISerializerService>();
            //((TridionComponentPresentationProvider)ComponentPresentationFactory.ComponentPresentationProvider).SerializerService = kernel.Get<ISerializerService>();
            //((TridionPageProvider)PageFactory.PageProvider).ComponentPresentationProvider = ComponentPresentationFactory.ComponentPresentationProvider;
            //kernel.Bind<IViewModelKeyProvider>().To <WebConfigViewModelKeyProvider>();
            //kernel.Bind<IViewModelResolver>().To<DefaultViewModelResolver>();
            //kernel.Bind<IViewModelFactory>().To<ViewModelFactory>();
            //kernel.Bind<IReflectionHelper>().To<ReflectionOptimizer>();
            ////kernel.Bind<ICacheAgent>().To<DefaultCacheAgent>().WhenInjectedInto(typeof(ILinkFactory));
            //LinkFactory.CacheAgent = kernel.Get<ICacheAgent>();
            //ViewModelFactory = kernel.Get<IViewModelFactory>();
            ////ViewModelFactory.LoadViewModels(new [] { typeof(TestViewModelA).Assembly });
        }

        private static ILinkResolver MockLinkResolver()
        {
            var linkResolver = new DefaultLinkResolver(MockLinkFactory(), MockLogger(), MockBinaryFactory(), MockConfig());
            return linkResolver;
        }

        private static IPageFactory MockPageFactory()
        {
            var pageFactory = new PageFactory(MockTridionPageProvider(), MockCP(), new MockFactoryCommenServices());
            return pageFactory;
        }

        private static IRichTextResolver MockRichTextResolver()
        {
            var rich = new DefaultRichTextResolver(MockLinkResolver(), MockLogger(), MockConfig());
            return rich;
        }

        private static IBinaryFactory MockBinaryFactory()
        {
            var binaryFactory = new BinaryFactory(new TridionBinaryProvider(), new MockFactoryCommenServices());
            return binaryFactory;
        }

        private static ILinkFactory MockLinkFactory()
        {
            var linkFactory = new LinkFactory(new TridionLinkProvider(), new MockFactoryCommenServices());
            return linkFactory;
        }

        private static IComponentPresentationFactory MockCP()
        {
            var cpFactory = new ComponentPresentationFactory(MockTridionComponentPresentationProvider(), new MockFactoryCommenServices());
            return cpFactory;
        }

        private static IComponentPresentationProvider MockTridionComponentPresentationProvider()
        {
            var serialazation = new JSONSerializerService();
            var cpProvider = new TridionComponentPresentationProvider() { SerializerService = serialazation };
            return cpProvider;
        }

        private static IPageProvider MockTridionPageProvider()
        {
            var provider = new TridionPageProvider()
            {
                ComponentPresentationProvider = MockTridionComponentPresentationProvider(),
                SerializerService = new JSONSerializerService()
            };
            return provider;
        }

        private static IViewModelFactory MockViewModelFactory()
        {
            var config = MockConfig();
            var reflectionHelper = new ReflectionOptimizer();
            var viewmodelResolver = new DefaultViewModelResolver(reflectionHelper);
            var viewModelKeyProdiver = new WebConfigViewModelKeyProvider(config);
            var linkResolver = new TestLinkResolver();
            var richTextResovler = new TestRichtextResovler();
            var contextResolver = new DefaultContextResolver();
            var logger = MockLogger();
            var ViewModelFactory = new ViewModelFactory(viewModelKeyProdiver,
                                                        viewmodelResolver,
                                                        linkResolver,
                                                        richTextResovler,
                                                        contextResolver,
                                                        config,
                                                        logger);
            return ViewModelFactory;
        }

        private static ILogger MockLogger()
        {
            return new NullLogger();
        }

        private static IDD4TConfiguration MockConfig()
        {
            return new TestConfiguration();
        }

        //public class RegistrationModule : NinjectModule
        //{
        //    public override void Load()
        //    {
        //        Bind<IDD4TConfiguration>().To<TestConfiguration>().InSingletonScope();
        //        Bind<IPublicationResolver>().To<DefaultPublicationResolver>().InSingletonScope();

        //        Bind<ILogger>().To<NullLogger>().InSingletonScope();
        //        Bind<IFactoryCommonServices>().To<FactoryCommonServices>().InSingletonScope();
        //        Bind<IPageFactory>().To<PageFactory>().InSingletonScope();
        //        Bind<IBinaryFactory>().To<BinaryFactory>().InSingletonScope();
        //        Bind<ILinkFactory>().To<LinkFactory>().InSingletonScope();
        //        Bind<IComponentPresentationFactory>().To<ComponentPresentationFactory>().InSingletonScope();
        //        Bind<IComponentFactory>().To<ComponentFactory>().InSingletonScope();
        //        Bind<IProvidersCommonServices>().To<ProviderCommonServices>().InSingletonScope();
        //        Bind<IPageProvider>().To<TridionPageProvider>().InSingletonScope();
        //        Bind<ILinkProvider>().To<TridionLinkProvider>().InSingletonScope();
        //        Bind<IBinaryProvider>().To<TridionBinaryProvider>().InSingletonScope();

        //        Bind<IComponentPresentationProvider>().To<TridionComponentPresentationProvider>().InSingletonScope();
        //        Bind<ICacheAgent>().To<DefaultCacheAgent>().InSingletonScope();

        //        Bind<ISerializerService>().To<JSONSerializerService>().InSingletonScope();

        //        Bind<ILinkResolver>().To<DefaultLinkResolver>().InSingletonScope();
        //        Bind<IRichTextResolver>().To<DefaultRichTextResolver>().InSingletonScope();
        //        Bind<IContextResolver>().To<DefaultContextResolver>().InSingletonScope();

        //    }
    }
}