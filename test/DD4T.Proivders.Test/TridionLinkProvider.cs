using System;
using DD4T.ContentModel.Contracts.Providers;

namespace DD4T.Providers.Test
{

    public class TridionLinkProvider : BaseProvider, ILinkProvider, IDisposable
    {

        public static string link1 = "/this/link/works.html";
        public static string link2 = "/this/link/works/too.html";
 
        public string ResolveLink(string componentUri)
        {
            return link1;
        }

        public string ResolveLink(string sourcePageUri, string componentUri, string excludeComponentTemplateUri)
        {
            return link2;
        }

        public void Dispose()
        {
        }
    }
}

