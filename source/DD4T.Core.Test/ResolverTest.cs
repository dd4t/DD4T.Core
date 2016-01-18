using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DD4T.ContentModel.Factories;
using Ninject.Modules;
using DD4T.Factories;
using Ninject;
using System.Reflection;
using DD4T.Providers.Test;
using DD4T.ContentModel.Contracts.Providers;
using DD4T.ContentModel.Contracts.Caching;
using DD4T.ContentModel;
using DD4T.ContentModel.Contracts.Serializing;
using DD4T.Serialization;
using System.Collections.Generic;

namespace DD4T.Core.Test
{
    [TestClass]
    public class ResolverTest : BaseFactoryTest
    {
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            Initialize();
            TridionLinkProvider.link1 = "/something";
        }

        [TestMethod]
        public void ResolveRichTextKeepWhitespace()
        {
            string richtextRaw = "<p xmlns=\"http://www.w3.org/1999/xhtml\" class=\"acme\">Hello <b>world</b> how are you?</p>\r\n<div xmlns=\"http://www.w3.org/1999/xhtml\">Next, we will try a number of <b>bold bold</b> <i>italic</i> and <u>underlined yes underlined</u> words. And a bold word between brackets: (<b>bold</b>).</div>";
            string resolvedRichText = ResolveRichText(richtextRaw);
            Assert.IsNotNull(resolvedRichText);
            Assert.AreEqual(richtextRaw.Replace(" xmlns=\"http://www.w3.org/1999/xhtml\"", ""), resolvedRichText);
        }

        [TestMethod]
        public void ResolveRichTextWithLinkToPDF()
        {
            string richtextRaw = "<p xmlns=\"http://www.w3.org/1999/xhtml\" class=\"acme\">Hello <b>world</b> how are you?</p>\r\n<div xmlns=\"http://www.w3.org/1999/xhtml\">Next, we will try a number of <b>bold bold</b> <i>italic</i> and <u>underlined yes underlined</u> words. And a bold word between brackets: (<b>bold</b>).</div>";
            richtextRaw += "<div xmlns=\"http://www.w3.org/1999/xhtml\"><a xlink:href=\"tcm:8-11147\" title=\"Test Attachment\" xlink:title=\"Test Attachment\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" href=\"/en/media/tes_tcm8-11147.pdf\">test atata</a></div>";
            string resolvedRichText = ResolveRichText(richtextRaw);
            Assert.IsNotNull(resolvedRichText);
            Assert.IsTrue(resolvedRichText.Contains("href=\"/en/media/tes_tcm8-11147.pdf\""));
            Assert.IsFalse(resolvedRichText.Contains("xlink:href=\""));

        }
        [TestMethod]
        public void ResolveRichTextWithLinkToComponent()
        {
            string richtextRaw = "<p xmlns=\"http://www.w3.org/1999/xhtml\" class=\"acme\">Hello <b>world</b> how are you?</p>\r\n<div xmlns=\"http://www.w3.org/1999/xhtml\">Next, we will try a number of <b>bold bold</b> <i>italic</i> and <u>underlined yes underlined</u> words. And a bold word between brackets: (<b>bold</b>).</div>";
            richtextRaw += "<div xmlns=\"http://www.w3.org/1999/xhtml\"><a xlink:href=\"tcm:8-11147\" xlink:title=\"Test Link\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">test atata</a></div>";
            string resolvedRichText = ResolveRichText(richtextRaw);
            Assert.IsNotNull(resolvedRichText);
            Assert.IsTrue(resolvedRichText.Contains(" href=\"/something\""));
            Assert.IsFalse(resolvedRichText.Contains("xlink:href=\""));
        }

        private string ResolveRichText(string input)
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("", "");
            
            Field richTextField = new Field()
            {
                Name = "richtext",
                Values = new List<string>()
                {
                    input
                }
            };
            cp.Component.Fields.Add(richTextField.Name, richTextField);
            return (string) ViewModelFactory.RichTextResolver.Resolve(cp.Component.Fields["richtext"].Value);
        }


        [TestMethod]
        public void ResolveLink()
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("", "");
            Assert.IsNotNull(cp);

            TridionLinkProvider.link1 = "/this/link/works.html";
            string url = ViewModelFactory.LinkResolver.ResolveUrl(cp.Component);

            Assert.IsNotNull(url);
            Assert.AreEqual(url, "/this/link/works.html");

        }
        [TestMethod]
        public void ResolveLinkWithPageUri()
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("", "");
            Assert.IsNotNull(cp);

            TridionLinkProvider.link2 = "/this/link/works/too.html"; 
            string url = ViewModelFactory.LinkResolver.ResolveUrl(cp.Component, "tcm:2-2-64");

            Assert.IsNotNull(url);
            Assert.AreEqual(url, "/this/link/works/too.html");

        }
    }
}
