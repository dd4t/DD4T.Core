using System;
using System.Xml.Serialization;
using DD4T.ContentModel.Contracts.Providers;
using System.IO;
using System.Collections.Generic;
using DD4T.ContentModel;
using System.Text;
using DD4T.ContentModel.Contracts.Serializing;

namespace DD4T.Providers.Test
{
    /// <summary>
    /// 
    /// </summary>
    public class TridionPageProvider : BaseProvider, IPageProvider
    {

        public bool ThrowPageNotFound { get; set; }
        public ISerializerService SerializerService
        {
            get;
            set;
        }
        public IComponentPresentationProvider ComponentPresentationProvider
        {
            get;
            set;
        }

        #region IPageProvider Members

        /// <summary>
        /// Get all urls of published pages
        /// </summary>
        /// <param name="includeExtensions"></param>
        /// <param name="pathStarts"></param>
        /// <param name="publicationID"></param>
        /// <returns></returns>
        public string[] GetAllPublishedPageUrls(string[] includeExtensions, string[] pathStarts)
        {
            List<string> testUrls = new List<string>() { "/test/one.html", "/test/two.html" };
            return testUrls.ToArray();
        }


        /// <summary>
        /// Gets the raw string (xml) from the broker db by URL
        /// </summary>
        /// <param name="url">URL of the page</param>
        /// <returns>String with page xml or empty string if no page was found</returns>
        public string GetContentByUrl(string url)
        {

            if (ThrowPageNotFound)
            {
                return string.Empty;
            }

            Page page = new Page();
            page.Title = Randomizer.AnyString(15);
            page.Id = Randomizer.AnyUri(64);
            page.Filename = Randomizer.AnySafeString(8) + ".html";

            PageTemplate pt = new PageTemplate();
            pt.Title = Randomizer.AnyString(20);
            Field ptfieldView = new Field();
            ptfieldView.Name = "view";
            ptfieldView.Values.Add("Standard");
            pt.MetadataFields = new FieldSet();
            pt.MetadataFields.Add(ptfieldView.Name, ptfieldView);

            page.PageTemplate = pt;

            page.ComponentPresentations = new List<ComponentPresentation>();

            string cpString = ComponentPresentationProvider.GetContent("");
            page.ComponentPresentations.Add(SerializerService.Deserialize<ComponentPresentation>(cpString));

            FieldSet metadataFields = new FieldSet();
            page.MetadataFields = metadataFields;

            return SerializerService.Serialize<Page>(page);
        }


        /// <summary>
        /// Gets the raw string (xml) from the broker db by URI
        /// </summary>
        /// <param name="Url">TCM URI of the page</param>
        /// <returns>String with page xml or empty string if no page was found</returns>
        public string GetContentByUri(string TcmUri)
        {
            throw new NotImplementedException();
        }


        public DateTime GetLastPublishedDateByUrl(string url)
        {
            return DateTime.Now;
        }

        public DateTime GetLastPublishedDateByUri(string uri) {
            return DateTime.Now;
        }
        #endregion

        public void Dispose()
        {
        }
    }
}
