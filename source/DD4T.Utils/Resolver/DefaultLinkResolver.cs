using DD4T.ContentModel;
using DD4T.ContentModel.Contracts.Configuration;
using DD4T.ContentModel.Contracts.Logging;
using DD4T.ContentModel.Factories;
using DD4T.Core.Contracts.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD4T.Utils.Resolver
{
    public class DefaultLinkResolver : ILinkResolver
    {
        private readonly ILinkFactory _linkFactory;
        private readonly IBinaryFactory _binaryFactory;
        private readonly ILogger _logger;
        private readonly IDD4TConfiguration _configuration;

        public DefaultLinkResolver(ILinkFactory linkFactory, ILogger logger, IBinaryFactory binaryFactory, IDD4TConfiguration configuration)
        {
            linkFactory.ThrowIfNull(nameof(linkFactory));
            binaryFactory.ThrowIfNull(nameof(binaryFactory));
            logger.ThrowIfNull(nameof(logger));
            configuration.ThrowIfNull(nameof(configuration));

            _binaryFactory = binaryFactory;
            _linkFactory = linkFactory;
            _logger = logger;
            _configuration = configuration;
        }

        public string ResolveUrl(IComponent component, string pageId = null)
        {
            return this.ResolveUrl(component.Id, pageId);
        }

        public string ResolveUrl(string tcmUri, string pageId = null)
        {
            _logger.Debug("ResolveUrl - Start resolving url for componentId:{0} and pageId:{0}",
                               tcmUri,
                               string.IsNullOrEmpty(pageId) ? TcmUri.NullUri.ToString() : pageId);

           var resolvedUrl = pageId.IsNullOrEmpty() ? _linkFactory.ResolveLink(tcmUri) : _linkFactory.ResolveLink(pageId, tcmUri, TcmUri.NullUri.ToString());

            //it could be a binary link. let's resolve it as a binaryLink ..
            if (resolvedUrl.IsNullOrEmpty())
                resolvedUrl = _binaryFactory.GetUrlForUri(tcmUri);

            _logger.Debug("ResolveUrl - Resolved Url for componentId: {0} = {1}", tcmUri, resolvedUrl);
            return resolvedUrl;
        }
    }
}
