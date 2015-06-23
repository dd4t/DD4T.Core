using DD4T.ContentModel.Contracts.Configuration;
using DD4T.ContentModel.Contracts.Providers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace DD4T.Utils
{
    public class DD4TConfiguration : IDD4TConfiguration
    {
        int publicationId = 0;
        public int PublicationId
        {
            get
            {
                if (publicationId == 0)
                {
                    int r = SafeGetConfigSettingAsInt(ConfigurationKeys.PublicationId);
                    if (r == int.MinValue)
                    {
                        publicationId = 0;
                    }
                    else
                    {
                        publicationId = r;
                    }
                }
                return publicationId;
            }
        }

        string defaulPage = "default.html";
        public string DefaultPage
        {
            get
            {
                var configurationValue = SafeGetConfigSettingAsString(ConfigurationKeys.WelcomeFile, ConfigurationKeys.WelcomeFileAlt1);
                if (!string.IsNullOrEmpty(configurationValue))
                    defaulPage = configurationValue;

                return defaulPage;
            }
        }


        public string ComponentPresentationController
        {
            get
            {
                return SafeGetConfigSettingAsString(ConfigurationKeys.ComponentPresentationController);
            }
        }

        public string ComponentPresentationAction
        {
            get
            {
                return SafeGetConfigSettingAsString(ConfigurationKeys.ComponentPresentationAction, ConfigurationKeys.ComponentPresentationActionAlt1);
            }
        }

        public string ActiveWebsite
        {
            get
            {
                return SafeGetConfigSettingAsString(ConfigurationKeys.ActiveWebsite, ConfigurationKeys.ActiveWebsiteAlt1);

            }
        }

        public string SelectComponentByComponentTemplateId
        {
            get
            {
                return SafeGetConfigSettingAsString(ConfigurationKeys.SelectComponentByComponentTemplateId, ConfigurationKeys.SelectComponentByComponentTemplateIdAlt1);
            }
        }

        public string SelectComponentByOutputFormat
        {
            get
            {
              return SafeGetConfigSettingAsString(ConfigurationKeys.SelectComponentByOutputFormat, ConfigurationKeys.SelectComponentByOutputFormatAlt1);

            }
        }

        private string _dataFormat = "json";
        public string DataFormat
        {
            get
            {
                var configurationValue = SafeGetConfigSettingAsString(ConfigurationKeys.DataFormat);
                if (!string.IsNullOrEmpty(configurationValue))
                    _dataFormat = configurationValue;

                return _dataFormat;
            }
        }

        public string ContentProviderEndPoint
        {
            get
            {
               var configurationvalue = SafeGetConfigSettingAsString(ConfigurationKeys.ContentProviderEndPoint);
                if(string.IsNullOrEmpty(configurationvalue))
                    throw new ConfigurationException("Content Provider endpoint not defined. Configure 'DD4T.ContentProviderEndPoint'.");

                return configurationvalue;
            }
        }

        public string SiteMapPath
        {
            get
            {
               var configurationvalue = SafeGetConfigSettingAsString(ConfigurationKeys.SitemapPath, ConfigurationKeys.SitemapPathAlt1);
                if(string.IsNullOrEmpty(configurationvalue))
                    throw new ConfigurationException("SiteMapPath not defined. Configure 'DD4T.SitemapPath'.");

                return configurationvalue;
            }
        }

        public int BinaryHandlerCacheExpiration
        {
            get
            {
                return SafeGetConfigSettingAsInt(ConfigurationKeys.BinaryHandlerCacheExpiration, ConfigurationKeys.BinaryHandlerCacheExpirationAlt1);

            }
        }

        public string BinaryFileExtensions
        {
            get
            {
               var configurationvalue = SafeGetConfigSettingAsString(ConfigurationKeys.BinaryFileExtensions ,ConfigurationKeys.BinaryFileExtensionsAlt1 );
                if(string.IsNullOrEmpty(configurationvalue))
                    throw new ConfigurationException("BinaryFileExtensions not defined. Configure 'DD4T.BinaryFileExtensions'.");

                return configurationvalue;
            }
        }

        public string BinaryUrlPattern
        {
            get
            {
               var configurationvalue = SafeGetConfigSettingAsString(ConfigurationKeys.BinaryUrlPattern );
                if(string.IsNullOrEmpty(configurationvalue))
                    throw new ConfigurationException("BinaryUrlPattern not defined. Configure 'DD4T.BinaryUrlPattern'.");

                return configurationvalue;
            }
        }


        public bool IncludeLastPublishedDate
        {
            get
            {
                return SafeGetConfigSettingAsBoolean(ConfigurationKeys.IncludeLastPublishedDate);
            }
        }

        public bool ShowAnchors
        {
            get
            {
                return SafeGetConfigSettingAsBoolean(ConfigurationKeys.ShowAnchors);
            }
        }


        public bool LinkToAnchor
        {
            get
            {
                return SafeGetConfigSettingAsBoolean(ConfigurationKeys.LinkToAnchor);
            }
        }

        public int DefaultCacheSettings
        {
            get
            {
                return SafeGetConfigSettingAsInt(ConfigurationKeys.DefaultCacheSettings);
            }
        }


        public int CacheCallBackInterval
        {
            get
            {
                return SafeGetConfigSettingAsInt(ConfigurationKeys.CacheSettingCallBackInterval);
            }
        }

        public string ResourcePath
        {
            get { throw new NotImplementedException(); }
        }
        public bool UseUriAsAnchor
        {
            get
            {
               return SafeGetConfigSettingAsBoolean(ConfigurationKeys.UseUriAsAnchor );
            }
        }

        public ProviderVersion ProviderVersion
        {
            get { throw new NotImplementedException(); }
        }

        #region private methods
        private static string SafeGetConfigSettingAsString(params string[] keys)
        {
            foreach (string key in keys)
            {
                string setting = ConfigurationManager.AppSettings[key];
                if (!string.IsNullOrEmpty(setting))
                    return setting;
            }
            return string.Empty;
        }

        private static int SafeGetConfigSettingAsInt(params string[] keys)
        {
            string setting = SafeGetConfigSettingAsString(keys);
            if (string.IsNullOrEmpty(setting))
                return int.MinValue;
            int i = int.MinValue;
            Int32.TryParse(setting, out i);
            return i;
        }
        private static bool SafeGetConfigSettingAsBoolean(params string[] keys)
        {
            string setting = SafeGetConfigSettingAsString(keys);
            if (string.IsNullOrEmpty(setting))
                return false;
            bool b = false;
            Boolean.TryParse(setting, out b);
            return b;
        }
        #endregion

    }
}
