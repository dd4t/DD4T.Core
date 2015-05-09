using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using DD4T.ContentModel;
using DD4T.ContentModel.Contracts.Caching;
using DD4T.ContentModel.Contracts.Providers;
using DD4T.ContentModel.Exceptions;
using DD4T.ContentModel.Logging;
using DD4T.ContentModel.Querying;
using DD4T.Factories.Caching;
using DD4T.Utils;
using DD4T.ContentModel.Factories;

namespace DD4T.Factories
{
    /// <summary>
    /// Factory for the creation of IComponents
    /// </summary>
    public class ComponentFactory : FactoryBase, IComponentFactory
    {
        public const string CacheKeyFormatByUri = "ComponentByUri_{0}_{1}";
        public const string CacheRegion = "Component";
        private ICacheAgent _cacheAgent = null;

        private IComponentProvider _componentProvider = null;
        public IComponentProvider ComponentProvider
        {
            get
            {
                if (_componentProvider == null)
                {
                    _componentProvider = (IComponentProvider)ProviderLoader.LoadProvider<IComponentProvider>(this.PublicationId);
                }
				
				// If using your own DI you can pass the provider PublicationID yourself
				// However by not doing so, the below will leverage the configuted PublicationResolver - which could still return 0 if you needed.					
                if (_componentProvider.PublicationId == 0)
                    _componentProvider.PublicationId = this.PublicationId;
					
                return _componentProvider;
            }
            set
            {
                _componentProvider = value;
            }
        }

        private static XmlSerializer _componentSerializer = null;
        private static XmlSerializer ComponentSerializer
        {
            get
            {
                if (_componentSerializer == null)
                    _componentSerializer = new XmlSerializer(typeof(Component));
                return _componentSerializer;
            }
        }

        private static XmlSerializer _componentPresentationSerializer = null;
        private static XmlSerializer ComponentPresentationSerializer
        {
            get
            {
                if (_componentPresentationSerializer == null)
                    _componentPresentationSerializer = new XmlSerializer(typeof(ComponentPresentation));
                return _componentPresentationSerializer;
            }
        }

        #region IComponentFactory members

        /// <summary>
        /// Create IComponent from a string representing that IComponent (XML)
        /// </summary>
        /// <param name="componentStringContent">XML content to deserialize into an IComponent</param>
        /// <returns></returns>
        public IComponent GetIComponentObject(string componentStringContent)
        {
            XmlDocument componentContent = new XmlDocument();
            componentContent.LoadXml(componentStringContent);
            
            IComponent component = null;
            using (var reader = new XmlNodeReader(componentContent.DocumentElement))
            {
                component = (IComponent)ComponentSerializer.Deserialize(reader);
            }
            return component;
        }

        /// <summary>
        /// Create IComponentPresentation from a string representing that IComponentPresentation (XML)
        /// </summary>
        /// <param name="componentPresentationStringContent">XML content to deserialize into an IComponent</param>
        /// <returns></returns>
        public IComponentPresentation GetIComponentPresentationObject(string componentPresentationStringContent)
        {
            XmlDocument xmlcontent = new XmlDocument();
            xmlcontent.LoadXml(componentPresentationStringContent);

            IComponentPresentation cp = null;
            using (var reader = new XmlNodeReader(xmlcontent.DocumentElement))
            {
                cp = (IComponentPresentation)ComponentPresentationSerializer.Deserialize(reader);
            }
            return cp;
        }

        /// <summary>
        /// Returns the Component contents which could be found. Components that couldn't be found don't appear in the list. 
        /// </summary>
        /// <param name="componentUris"></param>
        /// <returns></returns>
        public IList<IComponent> GetComponents(string[] componentUris)
        {
            List<IComponent> components = new List<IComponent>();
            foreach (string content in ComponentProvider.GetContentMultiple(componentUris))
            {
                components.Add(GetIComponentObject(content));
            }
            return components;
        }

        public IComponentPresentation GetComponentPresentation(string componentUri, string templateUri = "")
        {
            LoggerService.Debug(">>GetComponentPresentation ({0})", LoggingCategory.Performance, componentUri);
            IComponentPresentation cp;
            if (!TryGetComponentPresentation(componentUri, out cp, templateUri))
            {
                LoggerService.Debug("<<GetComponentPresentation ({0}) -- not found", LoggingCategory.Performance, componentUri);
                throw new ComponentNotFoundException();
            }

            LoggerService.Debug("<<GetComponentPresentation ({0})", LoggingCategory.Performance, componentUri);
            return cp;
        }

        public IList<IComponent> FindComponents(IQuery queryParameters, int pageIndex, int pageSize, out int totalCount)
        {
            LoggerService.Debug(">>FindComponents ({0},{1})", LoggingCategory.Performance, queryParameters.ToString(), Convert.ToString(pageIndex));
            totalCount = 0;
            IList<string> results = ComponentProvider.FindComponents(queryParameters);
            totalCount = results.Count;

            var pagedResults = results
                .Skip(pageIndex*pageSize)
                .Take(pageSize)
                .Select(c => { IComponent comp = null; TryGetComponent(c, out comp); return comp; })
                .Where(c => c!= null)
                .ToList();

            LoggerService.Debug("<<FindComponents ({0},{1})", LoggingCategory.Performance, queryParameters.ToString(), Convert.ToString(pageIndex));
            return pagedResults;

        }

        public IList<IComponent> FindComponents(IQuery queryParameters)
        {
            LoggerService.Debug(">>FindComponents ({0})", LoggingCategory.Performance, queryParameters.ToString());

            var results = ComponentProvider.FindComponents(queryParameters)
                .Select(c => { IComponent comp = null; TryGetComponent(c, out comp); return comp; })
                .Where(c => c!= null)
                .ToList();
            LoggerService.Debug("<<FindComponents ({0})", LoggingCategory.Performance, queryParameters.ToString());
            return results;
        }

        public DateTime GetLastPublishedDate(string uri)
        {
            return ComponentProvider.GetLastPublishedDate(uri);
        }

        public override DateTime GetLastPublishedDateCallBack(string key, object cachedItem)
        {
            if (cachedItem == null)
                return DateTime.Now; // this will force the item to be removed from the cache
            if (cachedItem is IComponent)
            {
                return GetLastPublishedDate(((IComponent)cachedItem).Id);
            }
            if (cachedItem is IComponentPresentation)
            {
                return GetLastPublishedDate(((IComponentPresentation)cachedItem).Component.Id);
            }
            throw new Exception(string.Format("GetLastPublishedDateCallBack called for unexpected object type '{0}' or with unexpected key '{1}'", cachedItem.GetType(), key));
        }
        /// <summary>
        /// Get or set the CacheAgent
        /// </summary>  
        public override ICacheAgent CacheAgent
        {
            get
            {
                if (_cacheAgent == null)
                {
                    _cacheAgent = new DefaultCacheAgent();
                    // the next line is the only reason we are overriding this property: to set a callback
                    _cacheAgent.GetLastPublishDateCallBack = GetLastPublishedDateCallBack;
                }
                return _cacheAgent;
            }
            set
            {
                _cacheAgent = value;
                _cacheAgent.GetLastPublishDateCallBack = GetLastPublishedDateCallBack;
            }
        }

        private bool TryGetComponentPresentationOrComponent(string componentUri, out object cpOrComp, string templateUri = "")
        {
            LoggerService.Debug(">>TryGetComponentPresentationOrComponent ({0})", LoggingCategory.Performance, componentUri);

            cpOrComp = null;

            string cacheKey = String.Format(CacheKeyFormatByUri, componentUri, templateUri);
            cpOrComp = CacheAgent.Load(cacheKey);

            if (cpOrComp != null)
            {
                LoggerService.Debug("<<TryGetComponentPresentationOrComponent ({0}) - from cache", LoggingCategory.Performance, componentUri);
                return true;
            }

            string content = !String.IsNullOrEmpty(templateUri) ? ComponentProvider.GetContent(componentUri, templateUri) : ComponentProvider.GetContent(componentUri);

            if (string.IsNullOrEmpty(content))
            {
                LoggerService.Debug("<<TryGetComponentPresentationOrComponent ({0}) - from provider", LoggingCategory.Performance, componentUri);
                return false;
            }

            // note: we need to introduce logic to detect the data format (json/xml, compressed/uncompressed) BEFORE we do this
            // for the time being we will assume the data is uncompressed xml
            if (content.StartsWith("<ComponentPresentation"))
            {
                cpOrComp = GetIComponentPresentationObject(content);
                if (IncludeLastPublishedDate)
                    ((ComponentPresentation)cpOrComp).Component.LastPublishedDate = ComponentProvider.GetLastPublishedDate(componentUri);
            }
            else
            {
                cpOrComp = GetIComponentObject(content);
                if (IncludeLastPublishedDate)
                    ((Component)cpOrComp).LastPublishedDate = ComponentProvider.GetLastPublishedDate(componentUri);
            }

            LoggerService.Debug("about to store object in cache ({0})", LoggingCategory.Performance, componentUri);
            CacheAgent.Store(cacheKey, CacheRegion, cpOrComp);
            LoggerService.Debug("finished storing IComponent in cache ({0})", LoggingCategory.Performance, componentUri);
            LoggerService.Debug("<<TryGetComponentPresentationOrComponent ({0})", LoggingCategory.Performance, componentUri);
            return true;
        }

        public bool TryGetComponentPresentation(string componentUri, out IComponentPresentation cp, string templateUri = "")
        {
            object cpobj = null;
            if (TryGetComponentPresentationOrComponent(componentUri, out cpobj, templateUri))
            {
                cp = (IComponentPresentation)cpobj;
                return true;
            }
            cp = null;
            return false;
        }

        public bool TryGetComponent(string componentUri, out IComponent component, string templateUri = "")
        {
            object compobj = null;
            if (TryGetComponentPresentationOrComponent(componentUri, out compobj, templateUri))
            {
                if (compobj is IComponentPresentation)
                {
                    component = ((IComponentPresentation)compobj).Component;
                }
                else
                {
                    component = (IComponent)compobj;
                }
                return true;
            }
            component = null;
            return false;
        }

        public IComponent GetComponent(string componentUri, string templateUri = "")
        {
            LoggerService.Debug(">>GetComponent ({0})", LoggingCategory.Performance, componentUri);
            IComponent component;
            if (!TryGetComponent(componentUri, out component, templateUri))
            {
                LoggerService.Debug("<<GetComponent ({0}) -- not found", LoggingCategory.Performance, componentUri);
                throw new ComponentNotFoundException();
            }

            LoggerService.Debug("<<GetComponent ({0})", LoggingCategory.Performance, componentUri);
            return component;
        }

        #endregion
    }
}
