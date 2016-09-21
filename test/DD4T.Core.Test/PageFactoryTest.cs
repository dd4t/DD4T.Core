using DD4T.ContentModel;
using DD4T.ContentModel.Exceptions;
using DD4T.Providers.Test;
using Xunit;

namespace DD4T.Core.Test
{
    public class PageFactoryTest : BaseFactoryTest
    {
        static PageFactoryTest()
        {
            Initialize();
        }

        [Fact]
        public void FindPage()
        {
            IPage page = PageFactory.FindPage("/index.html");
            Assert.NotNull(page);
            Assert.False(string.IsNullOrEmpty(page.Title));
        }

        [Fact]
        public void PageNotFound()
        {
            ((TridionPageProvider)PageFactory.PageProvider).ThrowPageNotFound = true;
            Assert.Throws<PageNotFoundException>(() => PageFactory.FindPage("/index.html"));
            //try
            //{
            //    IPage page = PageFactory.FindPage("/index.html");
            //    Assert.Fail("PageFactory did not throw a PageNotFoundException");
            //}
            //catch (PageNotFoundException)
            //{
            //    Assert.True(true);
            //}
        }

        [Fact]
        public void CacheNullResult()
        {
            int configuredPageExpiration = TestConfiguration.OverridePageExpiration;
            TestConfiguration.OverridePageExpiration = 60;
            ((TridionPageProvider)PageFactory.PageProvider).ThrowPageNotFound = true;

            Assert.Throws<PageNotFoundException>(() => PageFactory.FindPage("/test-null-result.html"));
            Assert.Throws<PageNotFoundException>(() => PageFactory.FindPage("/test-null-result.html"));
            //try
            //{
            //    IPage page = PageFactory.FindPage("/test-null-result.html");
            //    Assert.Fail("PageFactory did not throw a PageNotFoundException");
            //}
            //catch (PageNotFoundException)
            //{
            //}
            //((TridionPageProvider)PageFactory.PageProvider).ThrowPageNotFound = false;
            //try
            //{
            //    IPage page = PageFactory.FindPage("/test-null-result.html");
            //    Assert.Fail("PageFactory did not throw a PageNotFoundException");
            //}
            //catch (PageNotFoundException)
            //{
            //    Assert.True(true);
            //}
            TestConfiguration.OverridePageExpiration = configuredPageExpiration;
        }
    }
}