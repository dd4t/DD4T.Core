using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using DD4T.ContentModel.Contracts.Configuration;
using DD4T.ContentModel.Contracts.Logging;
using DD4T.ContentModel.Contracts.Caching;
using System.IO;
using DD4T.ContentModel;

namespace DD4T.Utils.Caching
{
    /// <summary>
    /// Default implementation of ICacheAgent, as used by the factories in DD4T.Factories. It uses the System.Runtime.Caching API introduced in .NET 4. This will run in a web environment as well as a windows service, console application or any other type of environment.
    /// </summary>
    public class DefaultCacheAgent : ICacheAgent, IObserver<ICacheEvent>, IDisposable
    {
        private readonly IDD4TConfiguration _configuration;
        private readonly ILogger _logger;
        private IDisposable unsubscriber;

        public DefaultCacheAgent(IDD4TConfiguration configuration, ILogger logger)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            if (logger == null)
                throw new ArgumentNullException("logger");

            _logger = logger;
            _configuration = configuration;
        }

        #region properties
        private static ObjectCache Cache
        {
            get
            {
                return MemoryCache.Default;
            }
        }

        #endregion properties

        #region ICacheAgent

        /// <summary>
        /// Load object from the cache
        /// </summary>
        /// <param name="key">Identification of the object</param>
        /// <returns></returns>
        public object Load(string key)
        {
            return Cache[key];
        }


        /// <summary>
        /// Store any object in the cache 
        /// </summary>
        /// <param name="key">Identification of the item</param>
        /// <param name="item">The object to store (can be a page, component, schema, etc) </param>
        public void Store(string key, object item)
        {
            Store(key, null, item, null);
        }

        /// <summary>
        /// Store any object in the cache with a dependency on other items in the cache
        /// </summary>
        /// <param name="key">Identification of the item</param>
        /// <param name="item">The object to store (can be a page, component, schema, etc) </param>
        /// <param name="dependOnTcmUris">List of items on which the current item depends</param>
        public void Store(string key, object item, List<string> dependOnTcmUris)
        {
            Store(key, null, item, dependOnTcmUris);
        }

        /// <summary>
        /// Store an object belonging to a specific region in the cache 
        /// </summary>
        /// <param name="key">Identification of the item</param>
        /// <param name="region">Identification of the region</param>
        /// <param name="item">The object to store (can be a page, component, schema, etc) </param>
        /// <remarks>The expiration time can be configured by adding an appSetting to the config with key 'CacheSettings_REGION' 
        /// (replace 'REGION' with the name of the region). If this key does not exist, the key 'CacheSettings_Default' will be used.
        /// </remarks>
        public void Store(string key, string region, object item)
        {
            Store(key, region, item, null);
        }

        /// <summary>
        /// Store an object belonging to a specific region in the cache with a dependency on other items in the cache.
        /// </summary>
        /// <param name="key">Identification of the item</param>
        /// <param name="region">Identification of the region</param>
        /// <param name="item">The object to store (can be a page, component, schema, etc) </param>
        /// <param name="dependOnTcmUris">List of items on which the current item depends</param>
        /// <remarks>The expiration time can be configured by adding an appSetting to the config with key 'CacheSettings_REGION' 
        /// (replace 'REGION' with the name of the region). If this key does not exist, the key 'CacheSettings_Default' will be used.
        /// </remarks>
        public void Store(string key, string region, object item, List<string> dependOnTcmUris)
        {
            Cache.Add(key, item, FindCacheItemPolicy(key, item, region));
            if (dependOnTcmUris != null)
            {
                foreach (string tcmUri in dependOnTcmUris)
                {
                    TcmUri u = new TcmUri(tcmUri);
                    string lookupkey = string.Format("{0}:{1}", u.PublicationId, u.ItemId);  // Tridion communicates about cache expiry using a key like 6:1120 (pubid:itemid)
                    IList<string> dependentItems = (IList<string>)Cache[GetDependencyCacheKey(lookupkey)];
                    if (dependentItems == null)
                    {
                        dependentItems = new List<string>();
                        dependentItems.Add(key);
                        Cache.Add(GetDependencyCacheKey(lookupkey), dependentItems, DateTimeOffset.MaxValue);
                        continue;
                    }
                    if (!dependentItems.Contains(key))
                    {
                        dependentItems.Add(key);
                    }
                }
            }
        }

        #endregion

        #region private

        private string GetDependencyCacheKey(string tcmUri)
        {
            return "Dependencies:" + tcmUri;
        }
        private CacheItemPolicy FindCacheItemPolicy(string key, object item, string region)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.Priority = CacheItemPriority.Default;


            int expirationSetting = 0;
            if (!string.IsNullOrEmpty(region))
            {
                //Todo: introduce regions in the IDD4TConfiguration interface
                //expirationSetting = ConfigurationHelper.GetSetting("DD4T.CacheSettings." + region, "CacheSettings_" + region);
                expirationSetting = _configuration.GetExpirationForCacheRegion(region);
            }
            policy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(expirationSetting);
            return policy;
        }

        public void Remove(string key)
        {
            Cache.Remove(key);
        }
        #endregion

        #region IObserver
        public virtual void Subscribe(IObservable<ICacheEvent> provider)
        {
            _logger.Information($"subscribing to provider {provider.GetType().Name}", LoggingCategory.Background);
            unsubscriber = provider.Subscribe(this);
        }

        public void OnCompleted()
        {

        }

        public void OnError(Exception e)
        {
            _logger.Error("Exception is thrown in DefaultCacheAgent", e);
            throw e;
        }


        private static object lockOnDependencyList = new object();
        public void OnNext(ICacheEvent cacheEvent)
        {
            _logger.Debug("Received event with region {0}, uri {1} and type {2}", LoggingCategory.Background, cacheEvent.RegionPath, cacheEvent.Key, cacheEvent.Type);
            // only proceed for events in the region ItemMeta
            if (!cacheEvent.RegionPath.Contains("ItemMeta"))
            {
                return;
            }
            // get the list of dependent items from the cache
            // NOTE: locking is not a problem here since this code is always running on a background thread (QS)
            lock (lockOnDependencyList)
            {
                RemoveWithDependencies(cacheEvent.Key, 0);               
            }
        }

        private void RemoveWithDependencies(string key, int depth)
        {
            var prefix = "";
            for (var i = 0; i <= depth; i++) prefix += ">";
            _logger.Debug($"{prefix} Called RemoveWithDependencies for key {key} and depth {depth}");
            if (depth > 4) // TODO: make this configurable
            {
                _logger.Debug($"{prefix} Max depth reached");
                return;
            }
            IList<string> dependencies = (IList<string>)Cache[GetDependencyCacheKey(key)];
            if (dependencies != null)
            {
                _logger.Debug($"{prefix} Found one or more dependencies for an (un)published item with cache key {key}");
                foreach (var cacheKey in dependencies)
                {
                    // if the item in the cache represents a Tridion item, let's look for indirect dependencies and remove them also
                    if (!Cache.Contains(cacheKey))
                    {
                        _logger.Debug($"{prefix} Found dependency {cacheKey} that doesn't exist in the cache any longer; skipping");
                        continue;
                    }
                    _logger.Debug($"{prefix} Found dependency in the cache with key {cacheKey} of type {Cache[cacheKey].GetType()}");
                    if (Cache[cacheKey] is IComponentPresentation payloadAsCp)
                    {
                        var dependentCacheKey = ConvertTcmUriToCacheKey(payloadAsCp.Component.Id);
                        if (dependentCacheKey != key)
                        {
                            RemoveWithDependencies(dependentCacheKey, depth + 1);
                        }
                    }
                    if (Cache[cacheKey] is IPage payloadAsPage)
                    {
                        var dependentCacheKey = ConvertTcmUriToCacheKey(payloadAsPage.Id);
                        if (dependentCacheKey != key)
                        {
                            RemoveWithDependencies(dependentCacheKey, depth + 1);
                        }
                    }
                    Cache.Remove(cacheKey);
                    _logger.Debug($"{prefix} Removed item from cache (key = {cacheKey})", LoggingCategory.Background);

                }
                Cache.Remove(GetDependencyCacheKey(key));
            }
        }

        private string ConvertTcmUriToCacheKey(string id)
        {
            var uri = new TcmUri(id);
            return $"{uri.PublicationId}:{uri.ItemId}";
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (unsubscriber != null)
            {
                unsubscriber.Dispose();
            }
        }
        #endregion

    }
}
