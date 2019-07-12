﻿using DD4T.ContentModel.Contracts.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD4T.Core.Test
{
    class TestConfiguration : IDD4TConfiguration
    {
        public static int OverridePageExpiration { get; set; }
        public static int OverrideComponentPresentationExpiration { get; set; }
        public static int OverrideBinaryExpiration { get; set; }
        public static bool OverrideUdpEnabled { get; set; } = true;
        public string ActiveWebsite
        {
            get
            {
                return string.Empty;
            }
        }

        public string BinaryFileExtensions
        {
            get
            {
                return string.Empty;
            }
        }

        public int BinaryHandlerCacheExpiration
        {
            get
            {
                return 0;
            }
        }

        public string BinaryUrlPattern
        {
            get
            {
                return string.Empty;
            }
        }

        public string ComponentPresentationAction
        {
            get
            {
                return "Component";
            }
        }

        public string ComponentPresentationController
        {
            get
            {
                return "Component";
            }
        }

        public string ContentProviderEndPoint
        {
            get
            {
                return "";
            }
        }

        public string DataFormat
        {
            get
            {
                return string.Empty;
            }
        }

        public int DefaultCacheSettings
        {
            get
            {
                return 60;
            }
        }

        public bool IncludeLastPublishedDate
        {
            get
            {
                return false;
            }
        }

        public bool IsPreview
        {
            get
            {
                return true;
            }
        }

        public string JMSHostname
        {
            get
            {
                return string.Empty;
            }
        }

        public string JMSUrl
        {
            get
            {
                return string.Empty;
            }
        }

        public int JMSNumberOfRetriesToConnect
        {
            get
            {
                return 0;
            }
        }

        public int JMSPort
        {
            get
            {
                return 0;
            }
        }

        public int JMSSecondsBetweenRetries
        {
            get
            {
                return 0;
            }
        }

        public string JMSTopic
        {
            get
            {
                return string.Empty;
            }
        }

        public bool LinkToAnchor
        {
            get
            {
                return false;
            }
        }

        public int PublicationId
        {
            get
            {
                return 0;
            }
        }

        public string ResourcePath
        {
            get
            {
                return string.Empty;
            }
        }

        public string SelectComponentByComponentTemplateId
        {
            get
            {
                return string.Empty;
            }
        }

        public string SelectComponentByOutputFormat
        {
            get
            {
                return string.Empty;
            }
        }

        public string SelectComponentPresentationByComponentTemplateId
        {
            get
            {
                return string.Empty;
            }
        }

        public string SelectComponentPresentationByOutputFormat
        {
            get
            {
                return string.Empty;
            }
        }

        public bool ShowAnchors
        {
            get
            {
                return false;
            }
        }

        public string SiteConfigurationUrl
        {
            get
            {
                return "siteconfiguration.json";
            }
        }

        public string SiteMapPath
        {
            get
            {
                return string.Empty;
            }
        }

        public bool UseUriAsAnchor
        {
            get
            {
                return true;
            }
        }

        public string ViewModelKeyField
        {
            get
            {
                return string.Empty;
            }
        }

        public string WelcomeFile
        {
            get
            {
                return "index.html";
            }
        }

        public string BinaryFileSystemCachePath
        {
            get
            {
                string p = Path.Combine(Path.GetTempPath(), "DD4TBinaries");
                if (!Directory.Exists(p))
                {
                    Directory.CreateDirectory(p);
                }
                return p;
                
            }
        }

        public bool UseDefaultViewModels
        {
            get
            {
                return true;
            }
        }

        public bool UDPEnabled
        {
            get
            {
                return OverrideUdpEnabled;
            }
        }

        public int GetExpirationForCacheRegion(string region)
        {
            if (region == "Page")
            {
                return OverridePageExpiration;
            }
            if (region == "ComponentPresentation")
            {
                return OverrideComponentPresentationExpiration;
            }
            if (region == "Binary")
            {
                return OverrideBinaryExpiration;
            }
            return DefaultCacheSettings;
        }
    }
}
