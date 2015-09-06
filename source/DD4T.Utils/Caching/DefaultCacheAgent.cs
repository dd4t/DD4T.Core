using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using DD4T.ContentModel.Contracts.Configuration;
using DD4T.ContentModel.Contracts.Logging;
using DD4T.ContentModel.Contracts.Caching;

namespace DD4T.Utils.Caching
{
    /// <summary>
    /// Default implementation of ICacheAgent, as used by the factories in DD4T.Factories. It uses the System.Runtime.Caching API introduced in .NET 4. This will run in a web environment as well as a windows service, console application or any other type of environment.
    /// If you are unable to run .NET 4, you can use the WebCacheAgent which is part of DD4T.Mvc.
    /// </summary>
    public class DefaultCacheAgent : ICacheAgent, IObserver<ICacheEvent>, IDisposable
    {
        public const int DefaultExpirationInSeconds = 60;

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
                    IList<string> dependentItems = (IList<string>) Cache[GetDependencyCacheKey(tcmUri)];
                    if (dependentItems == null)
                    {
                        dependentItems = new List<string>();
                        dependentItems.Add(key);
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
            }
            if (expirationSetting == 0)
            {
                expirationSetting = _configuration.DefaultCacheSettings;
            }
            int expirationInSeconds = -1;

            try
            {
                expirationInSeconds = expirationSetting == 0 ? DefaultExpirationInSeconds : expirationSetting;
            }
            catch
            {
                // if the value is not a proper number, we will use the default set in the code automatically
                expirationInSeconds = DefaultExpirationInSeconds;
            }
            policy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(expirationInSeconds);
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
            unsubscriber = provider.Subscribe(this);
        }

        public void OnCompleted()
        {
            _logger.Debug("Called OnCompleted");
            // todo: see what this does....
        }

        public void OnError(Exception error)
        {
            throw error;
        }


        private static object lockOnDependencyList = new object();
        public void OnNext(ICacheEvent cacheEvent)
        {
            _logger.Debug("received event with region {0}, uri {1} and type {2}", cacheEvent.RegionPath, cacheEvent.TcmUri, cacheEvent.Type);
            // get the list of dependent items from the cache
            // NOTE: locking is not a problem here since this code is always running on a background thread (QS)
            lock (lockOnDependencyList)
            {
                IList<string> dependencies = (IList<string>)Cache[GetDependencyCacheKey(cacheEvent.TcmUri)];
                if (dependencies != null)
                {
                    foreach (var cacheKey in dependencies)
                    {
                        Cache.Remove(cacheKey);
                        _logger.Debug("Removed item from cache (key = {0})", cacheKey);

                    }
                    Cache.Remove(GetDependencyCacheKey(cacheEvent.TcmUri));
                }
            }        
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            unsubscriber.Dispose();
        }
        #endregion

    }
}
