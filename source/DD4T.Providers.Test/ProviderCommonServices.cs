using DD4T.ContentModel.Contracts.Configuration;
using DD4T.ContentModel.Contracts.Logging;
using DD4T.ContentModel.Contracts.Providers;
using DD4T.ContentModel.Contracts.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD4T.Providers.Test
{
    public class ProviderCommonServices : IProvidersCommonServices
    {
        public IPublicationResolver PublicationResolver
        {
            get { throw new NotImplementedException(); }
        }

        public ILogger Logger
        {
            get { throw new NotImplementedException(); }
        }

        public IDD4TConfiguration Configuration
        {
            get { throw new NotImplementedException(); }
        }
    }
}
