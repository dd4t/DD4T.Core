using DD4T.ContentModel;
using DD4T.Core.Contracts.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD4T.Core.Contracts.Resolvers
{
    public interface ILinkResolver
    {
        /// <summary>
        /// Return hyperlink for this component using dynamic linking. The returned value is a complete HTML snippet.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        string ResolveLink(IComponent component);
        /// <summary>
        /// Return hyperlink for this component using dynamic linking. The returned value is a URL only.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        string ResolveUrl(IComponent component, string pageId = null);


        void model(IModel pageId);

    }
}
