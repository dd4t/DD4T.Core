using DD4T.ContentModel;
using DD4T.Providers.Test;
using System.Collections.Generic;
using Xunit;

namespace DD4T.Core.Test
{
    public class ResolverTest : BaseFactoryTest
    {
        static ResolverTest()
        {
            Initialize();
            TridionLinkProvider.link1 = "/something";
        }

        [Fact]
        public void ResolveRichTextKeepWhitespace()
        {
            string richtextRaw = "<p xmlns=\"http://www.w3.org/1999/xhtml\" class=\"acme\">Hello <b>world</b> how are you?</p>\r\n<div xmlns=\"http://www.w3.org/1999/xhtml\">Next, we will try a number of <b>bold bold</b> <i>italic</i> and <u>underlined yes underlined</u> words. And a bold word between brackets: (<b>bold</b>).</div>";
            string resolvedRichText = ResolveRichText(richtextRaw);
            Assert.NotNull(resolvedRichText);
            Assert.Equal(richtextRaw.Replace(" xmlns=\"http://www.w3.org/1999/xhtml\"", ""), resolvedRichText);
        }

        [Fact]
        public void ResolveRichTextWithLinkToPDF()
        {
            string richtextRaw = "<p xmlns=\"http://www.w3.org/1999/xhtml\" class=\"acme\">Hello <b>world</b> how are you?</p>\r\n<div xmlns=\"http://www.w3.org/1999/xhtml\">Next, we will try a number of <b>bold bold</b> <i>italic</i> and <u>underlined yes underlined</u> words. And a bold word between brackets: (<b>bold</b>).</div>";
            richtextRaw += "<div xmlns=\"http://www.w3.org/1999/xhtml\"><a xlink:href=\"tcm:8-11147\" title=\"Test Attachment\" xlink:title=\"Test Attachment\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" href=\"/en/media/tes_tcm8-11147.pdf\">test atata</a></div>";
            string resolvedRichText = ResolveRichText(richtextRaw);
            Assert.NotNull(resolvedRichText);
            Assert.True(resolvedRichText.Contains("href=\"/en/media/tes_tcm8-11147.pdf\""));
            Assert.False(resolvedRichText.Contains("xlink:href=\""));
        }

        [Fact]
        public void ResolveRichTextWithLinkToComponent()
        {
            string richtextRaw = "<p xmlns=\"http://www.w3.org/1999/xhtml\" class=\"acme\">Hello <b>world</b> how are you?</p>\r\n<div xmlns=\"http://www.w3.org/1999/xhtml\">Next, we will try a number of <b>bold bold</b> <i>italic</i> and <u>underlined yes underlined</u> words. And a bold word between brackets: (<b>bold</b>).</div>";
            richtextRaw += "<div xmlns=\"http://www.w3.org/1999/xhtml\"><a xlink:href=\"tcm:8-11147\" xlink:title=\"Test Link\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">test atata</a></div>";
            string resolvedRichText = ResolveRichText(richtextRaw);
            Assert.NotNull(resolvedRichText);
            Assert.True(resolvedRichText.Contains(" href=\"/something\""));
            Assert.False(resolvedRichText.Contains("xlink:href=\""));
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
            return (string)RichTextResolver.Resolve(cp.Component.Fields["richtext"].Value);
        }

        [Fact]
        public void ResolveLink()
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("", "");
            Assert.NotNull(cp);

            TridionLinkProvider.link1 = "/this/link/works.html";
            string url = LinkResolver.ResolveUrl(cp.Component);

            Assert.NotNull(url);
            Assert.Equal(url, "/this/link/works.html");
        }

        [Fact]
        public void ResolveLinkWithPageUri()
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("", "");
            Assert.NotNull(cp);

            TridionLinkProvider.link2 = "/this/link/works/too.html";
            string url = LinkResolver.ResolveUrl(cp.Component, "tcm:2-2-64");

            Assert.NotNull(url);
            Assert.Equal(url, "/this/link/works/too.html");
        }
    }
}