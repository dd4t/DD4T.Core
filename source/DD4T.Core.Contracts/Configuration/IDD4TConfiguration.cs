using DD4T.ContentModel.Contracts.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DD4T.ContentModel.Contracts.Configuration
{
    public interface IDD4TConfiguration
    {
        int PublicationId { get; }
        string DefaultPage { get; }
        string ComponentPresentationController { get; }
        string ComponentPresentationAction { get; }
        string ActiveWebsite { get; }
        string SelectComponentByComponentTemplateId { get; }
        string SelectComponentByOutputFormat { get; }
        string SiteMapPath { get; }
        int BinaryHandlerCacheExpiration { get; }
        string BinaryFileExtensions { get; }
        string BinaryUrlPattern { get; }
        bool IncludeLastPublishedDate { get; }
        bool ShowAnchors { get; }
        bool LinkToAnchor { get; }
        bool UseUriAsAnchor { get; }
        ProviderVersion ProviderVersion { get; }
        int DefaultCacheSettings { get; }
        int CacheCallBackInterval { get; }
        string DataFormat { get; }
        string ContentProviderEndPoint { get; }
        string ResourcePath { get; }
    }
}
