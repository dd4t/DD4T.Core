using DD4T.ContentModel.Contracts.Caching;
using DD4T.ContentModel.Contracts.Configuration;
using DD4T.ContentModel.Contracts.Logging;
using DD4T.ContentModel.Contracts.Resolvers;
using DD4T.Core.Contracts.DependencyInjection;
using DD4T.Core.Contracts.Resolvers;
using DD4T.Core.DD4T.Utils.Models;
using DD4T.Utils.Caching;
using DD4T.Utils.Logging;
using DD4T.Utils.Resolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD4T.Utils
{
    public class DependencyMappings : IDependencyMapper
    {
        public TypeDescriptionList TypeDescriptions()
        {
            var list = new TypeDescriptionList();

            list.Add(typeof(ILinkResolver), typeof(DefaultLinkResolver));
            list.Add(typeof(IRichTextResolver), typeof(DefaultRichTextResolver));
            list.Add(typeof(IPublicationResolver), typeof(DefaultPublicationResolver));
            list.Add(typeof(IDD4TConfiguration), typeof(DD4TConfiguration));
            list.Add(typeof(ICacheAgent), typeof(DefaultCacheAgent));
            list.Add(typeof(ILogger), typeof(NullLogger));

            return list;
        }
    }
}