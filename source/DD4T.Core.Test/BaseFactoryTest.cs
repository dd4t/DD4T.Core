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


namespace DD4T.Core.Test
{

    public abstract class BaseFactoryTest
    {

        protected static IPageFactory PageFactory { get; set; }
        protected static IComponentPresentationFactory ComponentPresentationFactory { get; set; }
        protected static IComponentFactory ComponentFactory { get; set; }


        public static void Initialize()
        {
            var kernel = new StandardKernel(new RegistrationModule());
            kernel.Load("DD4T.ContentModel.Contracts");
            kernel.Load("DD4T.Factories");
            kernel.Load("DD4T.Providers.Test");
            PageFactory = kernel.Get<IPageFactory>();
            ComponentPresentationFactory = kernel.Get<IComponentPresentationFactory>();
            ComponentFactory = kernel.Get<IComponentFactory>();
            PageFactory.CacheAgent = kernel.Get<ICacheAgent>();
            PageFactory.PageProvider = kernel.Get<IPageProvider>();
            ComponentPresentationFactory.CacheAgent = kernel.Get<ICacheAgent>();
            ComponentPresentationFactory.ComponentPresentationProvider = kernel.Get<IComponentPresentationProvider>();
            ((ComponentFactory)ComponentFactory).ComponentPresentationFactory = ComponentPresentationFactory;
            ((TridionPageProvider)PageFactory.PageProvider).SerializerService = kernel.Get<ISerializerService>();
            ((TridionComponentPresentationProvider)ComponentPresentationFactory.ComponentPresentationProvider).SerializerService = kernel.Get<ISerializerService>();
            ((TridionPageProvider)PageFactory.PageProvider).ComponentPresentationProvider = ComponentPresentationFactory.ComponentPresentationProvider;
        }


        public class RegistrationModule : NinjectModule
        {
            public override void Load()
            {
                Bind<IDD4TConfiguration>().To<DD4TConfiguration>().InSingletonScope();
                Bind<IPublicationResolver>().To<DefaultPublicationResolver>().InSingletonScope();

                Bind<ILogger>().To<NullLogger>().InSingletonScope();
                Bind<IFactoriesFacade>().To<FactoriesFacade>().InSingletonScope();
                Bind<IPageFactory>().To<PageFactory>().InSingletonScope();
                Bind<IComponentPresentationFactory>().To<ComponentPresentationFactory>().InSingletonScope();
                Bind<IComponentFactory>().To<ComponentFactory>().InSingletonScope();
                Bind<IProvidersFacade>().To<ProvidersFacade>().InSingletonScope();
                Bind<IPageProvider>().To<TridionPageProvider>().InSingletonScope();
                Bind<IComponentPresentationProvider>().To<TridionComponentPresentationProvider>().InSingletonScope();
                Bind<ICacheAgent>().To<NullCacheAgent>().InSingletonScope();
                Bind<ISerializerService>().To<JSONSerializerService>().InSingletonScope();
            }
        }
    }
}
