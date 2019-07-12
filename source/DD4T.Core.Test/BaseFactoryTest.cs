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
using DD4T.ContentModel.Contracts.Configuration;
using DD4T.ContentModel.Contracts.Resolvers;
using DD4T.ContentModel.Contracts.Logging;
using DD4T.Utils;
using DD4T.Utils.Logging;
using DD4T.Utils.Caching;
using DD4T.Utils.Resolver;
using DD4T.Core.Contracts.ViewModels;
using DD4T.ViewModels;
using DD4T.ViewModels.Reflection;
using DD4T.Core.Contracts.Resolvers;


namespace DD4T.Core.Test
{

    public abstract class BaseFactoryTest
    {

        protected static IPageFactory PageFactory { get; set; }
        protected static IBinaryFactory BinaryFactory { get; set; }
        protected static IComponentPresentationFactory ComponentPresentationFactory { get; set; }
        protected static IComponentFactory ComponentFactory { get; set; }
        protected static ILinkFactory LinkFactory { get; set; }
        protected static ICacheAgent CacheAgent { get; set; }
        protected static IViewModelFactory ViewModelFactory { get; set; }


        public static void Initialize()
        {
            var kernel = new StandardKernel(new RegistrationModule());
            kernel.Load("DD4T.ContentModel.Contracts");
            kernel.Load("DD4T.Factories");
            kernel.Load("DD4T.Providers.Test");
            kernel.Load("DD4T.ViewModels");
            PageFactory = kernel.Get<IPageFactory>();
            BinaryFactory = kernel.Get<IBinaryFactory>();
            ComponentPresentationFactory = kernel.Get<IComponentPresentationFactory>();
            ComponentFactory = kernel.Get<IComponentFactory>();
            LinkFactory = kernel.Get<ILinkFactory>();
            CacheAgent = kernel.Get<ICacheAgent>();
            ComponentPresentationFactory.CacheAgent = PageFactory.CacheAgent = kernel.Get<ICacheAgent>();
            PageFactory.PageProvider = kernel.Get<IPageProvider>();
            ComponentPresentationFactory.ComponentPresentationProvider = kernel.Get<IComponentPresentationProvider>();
            ((ComponentFactory)ComponentFactory).ComponentPresentationFactory = ComponentPresentationFactory;
            ((TridionPageProvider)PageFactory.PageProvider).SerializerService = kernel.Get<ISerializerService>();
            ((TridionComponentPresentationProvider)ComponentPresentationFactory.ComponentPresentationProvider).SerializerService = kernel.Get<ISerializerService>();
            ((TridionPageProvider)PageFactory.PageProvider).ComponentPresentationProvider = ComponentPresentationFactory.ComponentPresentationProvider;
            kernel.Bind<IViewModelKeyProvider>().To <WebConfigViewModelKeyProvider>();
            kernel.Bind<IViewModelResolver>().To<DefaultViewModelResolver>();
            kernel.Bind<IViewModelFactory>().To<ViewModelFactory>();
            kernel.Bind<IReflectionHelper>().To<ReflectionOptimizer>();
            //kernel.Bind<ICacheAgent>().To<DefaultCacheAgent>().WhenInjectedInto(typeof(ILinkFactory));
            LinkFactory.CacheAgent = kernel.Get<ICacheAgent>();
            ViewModelFactory = kernel.Get<IViewModelFactory>();
            //ViewModelFactory.LoadViewModels(new [] { typeof(TestViewModelA).Assembly });
        }


        public class RegistrationModule : NinjectModule
        {
            public override void Load()
            {
                Bind<IDD4TConfiguration>().To<TestConfiguration>().InSingletonScope();
                Bind<IPublicationResolver>().To<DefaultPublicationResolver>().InSingletonScope();

                Bind<ILogger>().To<NullLogger>().InSingletonScope();
                Bind<IFactoryCommonServices>().To<FactoryCommonServices>().InSingletonScope();
                Bind<IPageFactory>().To<PageFactory>().InSingletonScope();
                Bind<IBinaryFactory>().To<BinaryFactory>().InSingletonScope();
                Bind<ILinkFactory>().To<LinkFactory>().InSingletonScope();
                Bind<IComponentPresentationFactory>().To<ComponentPresentationFactory>().InSingletonScope();
                Bind<IComponentFactory>().To<ComponentFactory>().InSingletonScope();
                Bind<IProvidersCommonServices>().To<ProviderCommonServices>().InSingletonScope();
                Bind<IPageProvider>().To<TridionPageProvider>().InSingletonScope();
                Bind<ILinkProvider>().To<TridionLinkProvider>().InSingletonScope();
                Bind<IBinaryProvider>().To<TridionBinaryProvider>().InSingletonScope();

                Bind<IComponentPresentationProvider>().To<TridionComponentPresentationProvider>().InSingletonScope();
                Bind<ICacheAgent>().To<DefaultCacheAgent>().InSingletonScope();
               
                Bind<ISerializerService>().To<JSONSerializerService>().InSingletonScope();


                Bind<ILinkResolver>().To<DefaultLinkResolver>().InSingletonScope();
                Bind<IRichTextResolver>().To<DefaultRichTextResolver>().InSingletonScope();
                Bind<IContextResolver>().To<DefaultContextResolver>().InSingletonScope();



            
            }
        }
    }
}
