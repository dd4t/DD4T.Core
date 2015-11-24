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
        }

        [TestMethod]
        public void ResolveRichText()
        {
            IComponentPresentation cp = ComponentPresentationFactory.GetComponentPresentation("", "");
            Assert.IsNotNull(cp);

            string richtextRaw = "<p class=\"acme\">Hello <b>world</b> how are you?</p>\r\n<div>Next, we will try a number of <b>bold bold</b> <i>italic</i> and <u>underlined yes underlined</u> words. And a bold word between brackets: (<b>bold</b>).</div>";
            Field richTextField = new Field()
            {
                Name = "richtext",
                Values = new List<string>()
                {
                    richtextRaw
                }
            };
            cp.Component.Fields.Add(richTextField.Name, richTextField);

            string resolvedRichText = (string) ViewModelFactory.RichTextResolver.Resolve(cp.Component.Fields["richtext"].Value);
            Assert.IsNotNull(resolvedRichText);
            Assert.AreEqual(richtextRaw, resolvedRichText);

        }

    }
}
