using DD4T.ContentModel.Contracts.Configuration;
using DD4T.ContentModel.Contracts.Logging;
using DD4T.Core.Contracts.Resolvers;
using System.Linq;
using System.Xml.Linq;

namespace DD4T.Utils.Resolver
{
    public class DefaultRichTextResolver : IRichTextResolver
    {
        /// <summary>
        /// xhtml namespace uri
        /// </summary>
        private const string XhtmlNamespaceUri = "http://www.w3.org/1999/xhtml";

        /// <summary>
        /// xlink namespace uri
        /// </summary>
        private const string XlinkNamespaceUri = "http://www.w3.org/1999/xlink";

        private readonly ILogger _logger;

        private readonly IDD4TConfiguration _configuration;
        private readonly ILinkResolver _linkResolver;

        public DefaultRichTextResolver(ILinkResolver linkResolver, ILogger logger, IDD4TConfiguration configuration)
        {
            Contract.ThrowIfNull(linkResolver, nameof(linkResolver));
            Contract.ThrowIfNull(logger, nameof(logger));
            Contract.ThrowIfNull(configuration, nameof(configuration));

            _linkResolver = linkResolver;
            _logger = logger;
            _configuration = configuration;
        }

        public object Resolve(string input, string pageUri = null)
        {
            var document = XElement.Parse(string.Format("<xhtmlroot>{0}</xhtmlroot>", input));
            //XmlNamespaceManager nsmgr = new XmlNamespaceManager(new NameTable());
            XNamespace xhtml = XNamespace.Get(XhtmlNamespaceUri);
            XNamespace xlink = XNamespace.Get(XlinkNamespaceUri);
            foreach (var item in document.Descendants().Where(e => e.Name.NamespaceName.Equals(XhtmlNamespaceUri) && e.Name.LocalName == "a"))
            {
                // this link contains BOTH an xlink:href (TCM uri) AND an xhtml:href (resolved link, probably to a binary)
                // in that case we will let the pre-resolved hyperlink win
                var xhtmlAttribute = item.Attributes("href").FirstOrDefault();
                if (xhtmlAttribute == null)
                    xhtmlAttribute = new XAttribute("href", string.Empty);

                if (!string.IsNullOrEmpty(xhtmlAttribute.Value))
                    continue;

                //Find the xlink:href attrubute and if it starts with tcm: resolve the link
                var xlinkAttribute = item.Attributes(xlink + "href").FirstOrDefault();
                if (!xlinkAttribute.Value.StartsWith("tcm:"))
                    continue;

                string tcmuri = xlinkAttribute.Value;

                string linkUrl = _linkResolver.ResolveUrl(tcmuri, pageUri);
                //multimedia component added as component link into a RTF field.
                if (string.IsNullOrEmpty(linkUrl))
                    linkUrl = _linkResolver.ResolveUrl(tcmuri);

                if (!string.IsNullOrEmpty(linkUrl))
                {
                    xhtmlAttribute.Value = linkUrl;
                    item.Add(xhtmlAttribute);
                    // remove all xlink attributes
                    foreach (var xlinkAttr in item.Attributes().Where(a => a.Name.NamespaceName.Equals(XlinkNamespaceUri)))
                        xlinkAttr.Remove();
                }
            }

            // remove any additional xlink attribute
            foreach (var item in document.Descendants())
            {
                foreach (var xlinkAttr in item.Attributes().Where(a => a.Name.NamespaceName.Equals(XlinkNamespaceUri)))
                    xlinkAttr.Remove();
            }

#warning Is this still needed and use
            // fix empty anchors by placing the id value as a text node and adding a style attribute with position:absolute and visibility:hidden so the value won't show up
            //foreach (XmlElement anchor in doc.SelectNodes("//xhtml:a[not(node())]", nsmgr))
            //{
            //    XmlAttribute style = doc.CreateAttribute("style");
            //    style.Value = "position:absolute;visibility:hidden;";
            //    anchor.Attributes.Append(style);
            //    anchor.InnerText = anchor.Attributes["id"] != null ? anchor.Attributes["id"].Value : "empty";
            //}

            //var result = doc2.Descendants().Select(x => x.ToString()).Aggregate(string.Concat);
            var result = document.Elements().Select(x => x.ToString()).Aggregate(string.Concat);
            return RemoveNamespaceReferences(result);
        }

        /// <summary>
        /// removes unwanted namespace references (like xhtml and xlink) from the html
        /// </summary>
        /// <param name="html">html as a string</param>
        /// <returns>html as a string without namespace references</returns>
        private static string RemoveNamespaceReferences(string html)
        {
            if (!string.IsNullOrEmpty(html))
            {
                html = html.Replace(" xmlns=\"\"", string.Empty);
                html = html.Replace(string.Format(" xmlns=\"{0}\"", XhtmlNamespaceUri), string.Empty);
                html = html.Replace(string.Format(" xmlns:xhtml=\"{0}\"", XhtmlNamespaceUri), string.Empty);
                html = html.Replace(string.Format(" xmlns:xlink=\"{0}\"", XlinkNamespaceUri), string.Empty);
            }

            return html;
        }
    }
}