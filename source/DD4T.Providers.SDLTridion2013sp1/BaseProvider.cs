using DD4T.ContentModel.Contracts.Providers;
using DD4T.ContentModel.Contracts.Resolvers;
using DD4T.ContentModel.Contracts.Logging;
using DD4T.ContentModel.Contracts.Configuration;
using System;

namespace DD4T.Providers.SDLTridion2013sp1
{
    public class BaseProvider : IProvider
    {
      
        private readonly IPublicationResolver PublicationResolver;
        protected readonly ILogger LoggerService;
        protected readonly IDD4TConfiguration Configuration;
         

        public BaseProvider(IProvidersFacade providersFacade)
        {
            if (providersFacade == null)
                throw new ArgumentNullException("providersFacade");

            LoggerService = providersFacade.Logger;
            PublicationResolver = providersFacade.PublicationResolver;
            Configuration = providersFacade.Configuration;

        }

        private int publicationId = 0;
        public int PublicationId
        {
            get
            {
                if (publicationId == 0)
                    return PublicationResolver.ResolvePublicationId();

                return publicationId;
            }
            set
            {
                publicationId = value;
            }
        }
    }
}
