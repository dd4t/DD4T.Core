using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DD4T.Utils.ExtensionMethods
{
    public static class DD4TConfigurationExtensions
    {
        public static string GetLocalAnchorTag(this bool useUriAsAnchor, TcmUri componentUri, string nativeAnchor)
        {
            return useUriAsAnchor ? Convert.ToString(componentUri.ItemId) : nativeAnchor;
        }
    }
}
