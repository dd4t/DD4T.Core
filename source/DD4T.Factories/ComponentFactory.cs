using System;
using System.Collections.Generic;
using System.Linq;
using DD4T.ContentModel;
using DD4T.ContentModel.Contracts.Caching;
using DD4T.ContentModel.Contracts.Providers;
using DD4T.ContentModel.Exceptions;
using DD4T.ContentModel.Contracts.Logging;
using DD4T.ContentModel.Querying;
using DD4T.Utils;
using DD4T.ContentModel.Factories;

namespace DD4T.Factories
{
    /// <summary>
    /// Factory for the creation of IComponents
    /// </summary>

    public class ComponentFactory : FactoryBase, IComponentFactory
    {
        public IComponentPresentationFactory ComponentPresentationFactory
        {
            get;
            set;
        }

        public ComponentFactory( IComponentPresentationFactory componentPresentationFactory,
                            IFactoriesFacade facade)
            : base(facade)
        {

            if (componentPresentationFactory == null)
                throw new ArgumentNullException("componentPresentationFactory");

            ComponentPresentationFactory = componentPresentationFactory;
        }

        #region IComponentFactory members
        /// <summary>
        /// Create IComponent from a string representing that IComponent (XML)
        /// </summary>
        /// <param name="componentStringContent">XML content to deserialize into an IComponent</param>
        /// <returns></returns>
        /// 
        [Obsolete("Use the ComponentPresentationFactory to deserialize into a ComponentPresentation, then take the Component from there")]
        public IComponent GetIComponentObject(string componentStringContent)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the Component contents which could be found. Components that couldn't be found don't appear in the list. 
        /// </summary>
        /// <param name="componentUris"></param>
        /// <returns></returns>
        public IList<IComponent> GetComponents(string[] componentUris)
        {
            return ComponentPresentationFactory.GetComponentPresentations(componentUris).Select(a => a.Component).ToList();
        }

        public IList<IComponent> FindComponents(IQuery queryParameters, int pageIndex, int pageSize, out int totalCount)
        {
            return ComponentPresentationFactory.FindComponentPresentations(queryParameters, pageIndex, pageSize, out totalCount).Select(a => a.Component).ToList();
        }

        public IList<IComponent> FindComponents(IQuery queryParameters)
        {
            return ComponentPresentationFactory.FindComponentPresentations(queryParameters).Select(a => a.Component).ToList();
        }

        public DateTime GetLastPublishedDate(string uri)
        {
            return ComponentPresentationFactory.GetLastPublishedDate(uri);
        }

        public override DateTime GetLastPublishedDateCallBack(string key, object cachedItem)
        {
            if (cachedItem == null)
                return DateTime.Now; // this will force the item to be removed from the cache
            if (cachedItem is IComponent)
            {
                return GetLastPublishedDate(((IComponent)cachedItem).Id);
            }
            throw new Exception(string.Format("GetLastPublishedDateCallBack called for unexpected object type '{0}' or with unexpected key '{1}'", cachedItem.GetType(), key));
        }

        public bool TryGetComponent(string componentUri, out IComponent component, string templateUri = "")
        {
            IComponentPresentation cp = new ComponentPresentation();
            if (ComponentPresentationFactory.TryGetComponentPresentation(out cp, componentUri))
            {
                component = cp.Component;
                return true;
            }
            component = null;
            return false;
        }

        public IComponent GetComponent(string componentUri, string templateUri = "")
        {
            return ComponentPresentationFactory.GetComponentPresentation(componentUri, templateUri).Component;
        }

        [Obsolete]
        public IComponentProvider ComponentProvider
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        #endregion

    }
}
