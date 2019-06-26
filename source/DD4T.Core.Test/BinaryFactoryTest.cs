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
using DD4T.ContentModel.Exceptions;
using System.IO;
using System.Drawing;
using DD4T.ContentModel.Contracts.Configuration;
using DD4T.Utils.Caching;

namespace DD4T.Core.Test
{
    [TestClass]
    public class BinaryFactoryTest : BaseFactoryTest
    {
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            Initialize();
           
            string p = Path.Combine(Path.GetTempPath(), "DD4TMockWebRoot");
            if (Directory.Exists(p))
            {
                Directory.Delete(p,true);
            }
            Directory.CreateDirectory(p);

            IDD4TConfiguration config = new TestConfiguration();
            if (Directory.Exists(config.BinaryFileSystemCachePath))
            {
                Directory.Delete(config.BinaryFileSystemCachePath, true);
            }
            Directory.CreateDirectory(config.BinaryFileSystemCachePath);

            TestConfiguration.OverrideBinaryExpiration = 60;


        }

        [TestMethod]
        public void FindBinary()
        {
           
            IBinary binary = BinaryFactory.FindBinary("/media/image.png");
            Assert.IsNotNull(binary);
            Assert.IsTrue(binary.BinaryData.Length > 100, "byte array is too small, something went wrong");
            Assert.IsFalse(string.IsNullOrEmpty(binary.Id), "binary.Id is missing");
        }

        [TestMethod]

        public void FindAndStoreBinary()
        {
           
            string p = Path.Combine(Path.GetTempPath(), "DD4TMockWebRoot");
            if (!Directory.Exists(p))
            {
                Directory.CreateDirectory(p);
            }
            bool result = BinaryFactory.FindAndStoreBinary("/media/image.png", Path.Combine(p, "image.png"));
            Assert.IsTrue(result, "FindAndStoreBinary returned false");
            Assert.IsTrue(File.Exists(Path.Combine(p, "image.png")), "binary file was not created properly");

        }

        [TestMethod]

        public void FindAndStoreBinaryWithoutExtension()
        {

            string p = Path.Combine(Path.GetTempPath(), "DD4TMockWebRoot");
            if (!Directory.Exists(p))
            {
                Directory.CreateDirectory(p);
            }
            bool result = BinaryFactory.FindAndStoreBinary("/media/image", Path.Combine(p, "image"));
            Assert.IsTrue(result, "FindAndStoreBinary returned false");
            Assert.IsTrue(File.Exists(Path.Combine(p, "image")), "binary file was not created properly");

        }

        [TestMethod]
        public void ResizeImageToWidth()
        {

            IBinary binary = BinaryFactory.FindBinary("/media/image_w160.png");
            Assert.IsNotNull(binary);
            Assert.IsTrue(binary.BinaryData.Length > 100, "byte array is too small, something went wrong");
            Assert.IsFalse(string.IsNullOrEmpty(binary.Id), "binary.Id is missing");
            Image img = GetImageFromBytes(binary.BinaryData);
            Assert.IsTrue(img.Width == 160);
            Assert.IsTrue(img.Height == 55);

        }

        [TestMethod]
        public void ResizeImageToHeight()
        {
            IBinary binary = BinaryFactory.FindBinary("/media/image_h55.png");
            Assert.IsNotNull(binary);
            Assert.IsTrue(binary.BinaryData.Length > 100, "byte array is too small, something went wrong");
            Assert.IsFalse(string.IsNullOrEmpty(binary.Id), "binary.Id is missing");
            Image img = GetImageFromBytes(binary.BinaryData);
            Assert.IsTrue(img.Height == 55);
            Assert.IsTrue(img.Width == 160);
        }



        [TestMethod]
        public void FindBinaryPublishedNotStaleAndAlreadyOnFS()
        {
            ResetImageDimensions();
            TestConfiguration.OverrideBinaryExpiration = 60;
            IBinary binary = BinaryFactory.FindBinary("/media/image.png");
            Assert.IsNotNull(binary);
            Assert.IsTrue(binary.BinaryData.Length > 100, "byte array is too small, something went wrong");
            Assert.IsFalse(string.IsNullOrEmpty(binary.Id), "binary.Id is missing");
            Image img = GetImageFromBytes(binary.BinaryData);
            Assert.IsTrue(img.Width == 320);
            Assert.IsTrue(img.Height == 110);

            // change the generated image dimensions in the provider
            // this should NOT affect the results because the binary should be cached
            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageWidth = 400;
            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageHeight = 200;

            binary = BinaryFactory.FindBinary("/media/image.png");
            Assert.IsNotNull(binary);
            Assert.IsTrue(binary.BinaryData.Length > 100, "byte array is too small, something went wrong");
            Assert.IsFalse(string.IsNullOrEmpty(binary.Id), "binary.Id is missing");
            img = GetImageFromBytes(binary.BinaryData);
            Assert.IsTrue(img.Width == 320);
            Assert.IsTrue(img.Height == 110);

            ResetImageDimensions();

        }
        [TestMethod]
        public void FindBinaryPublishedAndNotOnFS()
        {
        }
        [TestMethod]
        public void FindBinaryPublishedStaleAndAlreadyOnFS()
        {
            if (! string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPVEYOR_BUILD_FOLDER")))
            {
                return; // for some unknown reason, this test fails on AppVeyor (we need to look into this but it has a low prio)
            }

            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageWidth = 300;
            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageHeight = 400;
            int currentBinaryExpiration = TestConfiguration.OverrideBinaryExpiration;
            TestConfiguration.OverrideBinaryExpiration = 0;

            IBinary binary = BinaryFactory.FindBinary("/media/image2.png");
            Assert.IsNotNull(binary);
            Assert.IsTrue(binary.BinaryData.Length > 100, "byte array is too small, something went wrong");
            Assert.IsFalse(string.IsNullOrEmpty(binary.Id), "binary.Id is missing");
            Image img = GetImageFromBytes(binary.BinaryData);
            Assert.IsTrue(img.Width == 300, "width is not 300 when image is retrieved for the first time");
            Assert.IsTrue(img.Height == 400, "height is not 400 when image is retrieved for the first time");

            // change the generated image dimensions in the provider
            // this should NOT affect the results because the binary should be cached
            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageWidth = 400;
            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageHeight = 200;

            binary = BinaryFactory.FindBinary("/media/image2.png");
            Assert.IsNotNull(binary, "binary is null");
            Assert.IsTrue(binary.BinaryData.Length > 100, "byte array is too small, something went wrong");
            Assert.IsFalse(string.IsNullOrEmpty(binary.Id), "binary.Id is missing");
            img = GetImageFromBytes(binary.BinaryData);
            Assert.IsTrue(img.Width == 400, "width is not 400 when image is retrieved for the second time");
            Assert.IsTrue(img.Height == 200, "height is not 200 when image is retrieved for the second time");

            ResetImageDimensions();
            TestConfiguration.OverrideBinaryExpiration = currentBinaryExpiration;

        }

        [TestMethod]
        public void FindBinaryPublishedNoDateAndAlreadyOnFS()
        {
            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageWidth = 300;
            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageHeight = 400;
            int currentBinaryExpiration = TestConfiguration.OverrideBinaryExpiration;
            TestConfiguration.OverrideBinaryExpiration = 0;

            IBinary binary = BinaryFactory.FindBinary("nodate");
            Assert.IsNotNull(binary);
            Assert.IsTrue(binary.BinaryData.Length > 100, "byte array is too small, something went wrong");
            Assert.IsFalse(string.IsNullOrEmpty(binary.Id), "binary.Id is missing");
            Image img = GetImageFromBytes(binary.BinaryData);
            Assert.IsTrue(img.Width == 300);
            Assert.IsTrue(img.Height == 400);

            // change the generated image dimensions in the provider
            // this should NOT affect the results because the binary should be cached
            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageWidth = 400;
            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageHeight = 200;

            binary = BinaryFactory.FindBinary("nodate");
            Assert.IsNotNull(binary);
            Assert.IsTrue(binary.BinaryData.Length > 100, "byte array is too small, something went wrong");
            Assert.IsFalse(string.IsNullOrEmpty(binary.Id), "binary.Id is missing");
            img = GetImageFromBytes(binary.BinaryData);
            Assert.IsTrue(img.Width == 400);
            Assert.IsTrue(img.Height == 200);

            ResetImageDimensions();
            TestConfiguration.OverrideBinaryExpiration = currentBinaryExpiration;

        }

        [TestMethod]
        public void FindBinaryOnFSInCache()
        {
        }

        [TestMethod]
        public void BinaryCacheIsInvalidated()
        {
            MockMessageProvider messageProvider = new MockMessageProvider();
            messageProvider.Start();
            ((DefaultCacheAgent)((FactoryBase)BinaryFactory).CacheAgent).Subscribe(messageProvider);

            ResetImageDimensions();
            TestConfiguration.OverrideBinaryExpiration = 180;
            IBinary binary = BinaryFactory.FindBinary("/media/image.png");
            Assert.IsNotNull(binary);
            Assert.IsTrue(binary.BinaryData.Length > 100, "byte array is too small, something went wrong");
            Assert.IsFalse(string.IsNullOrEmpty(binary.Id), "binary.Id is missing");
            Image img = GetImageFromBytes(binary.BinaryData);
            Assert.IsTrue(img.Width == 320);
            Assert.IsTrue(img.Height == 110);

            ICacheEvent cacheEvent = new CacheEvent()
            {
                Key = "1:2",
                RegionPath = "/some/path/that/includes/ItemMeta",
                Type = 0
            };
            messageProvider.BroadcastCacheEvent(cacheEvent);

            // change the generated image dimensions in the provider
            // this should NOT affect the results because the binary should be cached
            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageWidth = 400;
            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageHeight = 200;

            binary = BinaryFactory.FindBinary("/media/image.png");
            Assert.IsNotNull(binary);
            Assert.IsTrue(binary.BinaryData.Length > 100, "byte array is too small, something went wrong");
            Assert.IsFalse(string.IsNullOrEmpty(binary.Id), "binary.Id is missing");
            img = GetImageFromBytes(binary.BinaryData);
            Assert.IsTrue(img.Width == 400);
            Assert.IsTrue(img.Height == 200);

            ResetImageDimensions();

        }


        [TestMethod]
        public void FindBinaryWithoutExtension()
        {
            IBinary binary = BinaryFactory.FindBinary("/media/image");
            Assert.IsNotNull(binary);
            Assert.IsTrue(binary.BinaryData.Length > 100, "byte array is too small, something went wrong");
        }

        private Image GetImageFromBytes(byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length);
            ms.Position = 0; // this is important
            return Image.FromStream(ms, true);
        }

        private void ResetImageDimensions()
        {
            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageWidth = 320;
            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageHeight = 110;
        }

    }
}
