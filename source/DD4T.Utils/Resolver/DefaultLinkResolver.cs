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
        private readonly ILogger _logger;
        private readonly IDD4TConfiguration _configuration;

        public DefaultLinkResolver(ILinkFactory linkFactory, ILogger logger, IDD4TConfiguration configuration)
        {
            if (linkFactory == null) throw new ArgumentNullException("linkFactory");
            if (logger == null) throw new ArgumentNullException("logger");
            if (configuration == null) throw new ArgumentNullException("configuration");

            _linkFactory = linkFactory;
            _logger = logger;
            _configuration = configuration;
        }

        public string ResolveLink(IComponent component)
        {
            _logger.Debug("ResolveUrl - Start resolving url for componentId:{0}", component.Id);
            string resolvedUrl;
            resolvedUrl = _linkFactory.ResolveLink(component.Id);
            _logger.Debug("ResolveUrl - Resolved Url for componentId: {0} = {1}", component.Id, resolvedUrl);
            return resolvedUrl;
        }

        public string ResolveUrl(IComponent component, string pageId = null)
        {
            _logger.Debug("ResolveUrl - Start resolving url for componentId:{0} and pageId:{0}", 
                                component.Id, 
                                string.IsNullOrEmpty(pageId) ? TcmUri.NullUri.ToString() : pageId);

            string resolvedUrl;
            if (string.IsNullOrEmpty(pageId))
            {
                resolvedUrl = _linkFactory.ResolveLink(component.Id);
            }
            else
            {
                resolvedUrl = _linkFactory.ResolveLink(pageId, component.Id, TcmUri.NullUri.ToString());
            }
            _logger.Debug("ResolveUrl - Resolved Url for componentId: {0} = {1}", component.Id, resolvedUrl);
            return resolvedUrl;
        }
    }
}
