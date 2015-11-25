using System;
using DD4T.ContentModel.Contracts.Providers;

namespace DD4T.Providers.Test
{

    public class TridionLinkProvider : BaseProvider, ILinkProvider, IDisposable
    {

        public string ResolveLink(string componentUri)
        {
            return "/this/link/works.html";
        }

        public string ResolveLink(string sourcePageUri, string componentUri, string excludeComponentTemplateUri)
        {
            return "/this/link/works/too.html";
        }
        public void Dispose()
        {
        }
    }
}

