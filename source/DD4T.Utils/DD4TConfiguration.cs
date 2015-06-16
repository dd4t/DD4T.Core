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

        string componentPresentationController = string.Empty;
        public string ComponentPresentationController
        {
            get
            {
                if (!string.IsNullOrEmpty(componentPresentationController))
                    return componentPresentationController;

                var configurationValue = SafeGetConfigSettingAsString(ConfigurationKeys.ComponentPresentationController);
                if (!string.IsNullOrEmpty(configurationValue))
                    componentPresentationController = configurationValue;

                return componentPresentationController;
            }
        }

        public string ComponentPresentationAction
        {
            get { throw new NotImplementedException(); }
        }

        public string ActiveWebsite
        {
            get { throw new NotImplementedException(); }
        }

        string selectComponentByComponentTemplateId = string.Empty;
        public string SelectComponentByComponentTemplateId
        {
            get
            {
                if (!string.IsNullOrEmpty(selectComponentByComponentTemplateId))
                    return selectComponentByComponentTemplateId;

                var configurationValue =  SafeGetConfigSettingAsString(ConfigurationKeys.SelectComponentByComponentTemplateId, ConfigurationKeys.SelectComponentByComponentTemplateIdAlt1);
                if (!string.IsNullOrEmpty(configurationValue))
                    selectComponentByComponentTemplateId = configurationValue;

                return selectComponentByComponentTemplateId;
            }
        }

        string selectComponentByOutputFormat;
        public string SelectComponentByOutputFormat
        {
            get
            {
                if (!string.IsNullOrEmpty(selectComponentByOutputFormat))
                    return selectComponentByOutputFormat;

                var configurationValue = SafeGetConfigSettingAsString(ConfigurationKeys.SelectComponentByOutputFormat, ConfigurationKeys.SelectComponentByOutputFormatAlt1);
                if (!string.IsNullOrEmpty(configurationValue))
                    selectComponentByOutputFormat = configurationValue;

                return selectComponentByOutputFormat;
            }
        }

        private string _dataFormat = "json";
        public string DataFormat
        {
            get {

                var configurationValue = SafeGetConfigSettingAsString(ConfigurationKeys.DataFormat );
                if (!string.IsNullOrEmpty(configurationValue))
                    _dataFormat = configurationValue;

                return _dataFormat;
            }
        }

        public string ContentProviderEndPoint
        {
            get
            {
              return SafeGetConfigSettingAsString(ConfigurationKeys.ContentProviderEndPoint);
                
            }
        }

        public string SiteMapPath
        {
            get { throw new NotImplementedException(); }
        }

        public int BinaryHandlerCacheExpiration
        {
            get { throw new NotImplementedException(); }
        }

        public string BinaryFileExtensions
        {
            get { throw new NotImplementedException(); }
        }

        public string BinaryUrlPattern
        {
            get { throw new NotImplementedException(); }
        }

        public string LoggerClass
        {
            get { throw new NotImplementedException(); }
        }

        public bool IncludeLastPublishedDate
        {
            get
            {
                var configurationValue = SafeGetConfigSettingAsBoolean(ConfigurationKeys.IncludeLastPublishedDate);
                return configurationValue;
            }
        }

        public bool ShowAnchors
        {
            get { throw new NotImplementedException(); }
        }


        public bool LinkToAnchor
        {
            get
            {
                var configurationValue = SafeGetConfigSettingAsBoolean(ConfigurationKeys.LinkToAnchor);
                return configurationValue;
            }
        }

        public int DefaultCacheSettings
        {
            get
            {
                var configurationValue = SafeGetConfigSettingAsInt(ConfigurationKeys.DefaultCacheSettings);
                return configurationValue;
            }
        }


        public int CacheCallBackInterval
        {
            get
            {
                var configurationValue = SafeGetConfigSettingAsInt(ConfigurationKeys.CacheSettingCallBackInterval);
                return configurationValue;
            }
        }

        public string ResourcePath
        {
            get { throw new NotImplementedException(); }
        }
        public bool UseUriAsAnchor
        {
            get { throw new NotImplementedException(); }
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
