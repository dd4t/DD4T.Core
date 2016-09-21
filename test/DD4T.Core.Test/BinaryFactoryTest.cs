//using DD4T.ContentModel;
//using DD4T.ContentModel.Contracts.Caching;
//using DD4T.ContentModel.Contracts.Configuration;
//using DD4T.ContentModel.Contracts.Providers;
//using DD4T.ContentModel.Contracts.Serializing;
//using DD4T.ContentModel.Exceptions;
//using DD4T.ContentModel.Factories;
//using DD4T.Factories;
//using DD4T.Providers.Test;
//using DD4T.Serialization;
//using DD4T.Utils.Caching;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Ninject;
//using Ninject.Modules;
//using System;
//using System.Drawing;
//using System.IO;
//using System.Reflection;
//using Xunit;

//namespace DD4T.Core.Test
//{
//    public class BinaryFactoryTest : BaseFactoryTest
//    {
//        public static void Setup(TestContext context)
//        {
//            Initialize();

//            string p = Path.Combine(Path.GetTempPath(), "DD4TMockWebRoot");
//            if (Directory.Exists(p))
//            {
//                Directory.Delete(p, true);
//            }
//            Directory.CreateDirectory(p);

//            IDD4TConfiguration config = new TestConfiguration();
//            if (Directory.Exists(config.BinaryFileSystemCachePath))
//            {
//                Directory.Delete(config.BinaryFileSystemCachePath, true);
//            }
//            Directory.CreateDirectory(config.BinaryFileSystemCachePath);

//            TestConfiguration.OverrideBinaryExpiration = 60;
//        }

//        [Fact]
//        public void FindBinary()
//        {
//            IBinary binary = BinaryFactory.FindBinary("/media/image.png");
//            Assert.NotNull(binary);
//            Assert.True(binary.BinaryData.Length > 100, "byte array is too small, something went wrong");
//            Assert.False(string.IsNullOrEmpty(binary.Id), "binary.Id is missing");
//        }

//        [Fact]
//        public void FindAndStoreBinary()
//        {
//            string p = Path.Combine(Path.GetTempPath(), "DD4TMockWebRoot");
//            if (!Directory.Exists(p))
//            {
//                Directory.CreateDirectory(p);
//            }
//            bool result = BinaryFactory.FindAndStoreBinary("/media/image.png", Path.Combine(p, "image.png"));
//            Assert.True(result, "FindAndStoreBinary returned false");
//            Assert.True(File.Exists(Path.Combine(p, "image.png")), "binary file was not created properly");
//        }

//        [Fact]
//        public void FindAndStoreBinaryWithoutExtension()
//        {
//            string p = Path.Combine(Path.GetTempPath(), "DD4TMockWebRoot");
//            if (!Directory.Exists(p))
//            {
//                Directory.CreateDirectory(p);
//            }
//            bool result = BinaryFactory.FindAndStoreBinary("/media/image", Path.Combine(p, "image"));
//            Assert.True(result, "FindAndStoreBinary returned false");
//            Assert.True(File.Exists(Path.Combine(p, "image")), "binary file was not created properly");
//        }

//        [Fact]
//        public void ResizeImageToWidth()
//        {
//            IBinary binary = BinaryFactory.FindBinary("/media/image_w160.png");
//            Assert.NotNull(binary);
//            Assert.True(binary.BinaryData.Length > 100, "byte array is too small, something went wrong");
//            Assert.False(string.IsNullOrEmpty(binary.Id), "binary.Id is missing");
//            Image img = GetImageFromBytes(binary.BinaryData);
//            Assert.True(img.Width == 160);
//            Assert.True(img.Height == 55);
//        }

//        [Fact]
//        public void ResizeImageToHeight()
//        {
//            IBinary binary = BinaryFactory.FindBinary("/media/image_h55.png");
//            Assert.NotNull(binary);
//            Assert.True(binary.BinaryData.Length > 100, "byte array is too small, something went wrong");
//            Assert.False(string.IsNullOrEmpty(binary.Id), "binary.Id is missing");
//            Image img = GetImageFromBytes(binary.BinaryData);
//            Assert.True(img.Height == 55);
//            Assert.True(img.Width == 160);
//        }

//        [Fact]
//        public void FindBinaryPublishedNotStaleAndAlreadyOnFS()
//        {
//            ResetImageDimensions();
//            TestConfiguration.OverrideBinaryExpiration = 60;
//            IBinary binary = BinaryFactory.FindBinary("/media/image.png");
//            Assert.NotNull(binary);
//            Assert.True(binary.BinaryData.Length > 100, "byte array is too small, something went wrong");
//            Assert.False(string.IsNullOrEmpty(binary.Id), "binary.Id is missing");
//            Image img = GetImageFromBytes(binary.BinaryData);
//            Assert.True(img.Width == 320);
//            Assert.True(img.Height == 110);

//            // change the generated image dimensions in the provider
//            // this should NOT affect the results because the binary should be cached
//            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageWidth = 400;
//            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageHeight = 200;

//            binary = BinaryFactory.FindBinary("/media/image.png");
//            Assert.NotNull(binary);
//            Assert.True(binary.BinaryData.Length > 100, "byte array is too small, something went wrong");
//            Assert.False(string.IsNullOrEmpty(binary.Id), "binary.Id is missing");
//            img = GetImageFromBytes(binary.BinaryData);
//            Assert.True(img.Width == 320);
//            Assert.True(img.Height == 110);

//            ResetImageDimensions();
//        }

//        [Fact]
//        public void FindBinaryPublishedAndNotOnFS()
//        {
//        }

//        [Fact]
//        public void FindBinaryPublishedStaleAndAlreadyOnFS()
//        {
//            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPVEYOR_BUILD_FOLDER")))
//            {
//                return; // for some unknown reason, this test fails on AppVeyor (we need to look into this but it has a low prio)
//            }

//            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageWidth = 300;
//            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageHeight = 400;
//            int currentBinaryExpiration = TestConfiguration.OverrideBinaryExpiration;
//            TestConfiguration.OverrideBinaryExpiration = 0;

//            IBinary binary = BinaryFactory.FindBinary("/media/image2.png");
//            Assert.NotNull(binary);
//            Assert.True(binary.BinaryData.Length > 100, "byte array is too small, something went wrong");
//            Assert.False(string.IsNullOrEmpty(binary.Id), "binary.Id is missing");
//            Image img = GetImageFromBytes(binary.BinaryData);
//            Assert.True(img.Width == 300, "width is not 300 when image is retrieved for the first time");
//            Assert.True(img.Height == 400, "height is not 400 when image is retrieved for the first time");

//            // change the generated image dimensions in the provider
//            // this should NOT affect the results because the binary should be cached
//            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageWidth = 400;
//            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageHeight = 200;

//            binary = BinaryFactory.FindBinary("/media/image2.png");
//            Assert.NotNull(binary, "binary is null");
//            Assert.True(binary.BinaryData.Length > 100, "byte array is too small, something went wrong");
//            Assert.False(string.IsNullOrEmpty(binary.Id), "binary.Id is missing");
//            img = GetImageFromBytes(binary.BinaryData);
//            Assert.True(img.Width == 400, "width is not 400 when image is retrieved for the second time");
//            Assert.True(img.Height == 200, "height is not 200 when image is retrieved for the second time");

//            ResetImageDimensions();
//            TestConfiguration.OverrideBinaryExpiration = currentBinaryExpiration;
//        }

//        [Fact]
//        public void FindBinaryPublishedNoDateAndAlreadyOnFS()
//        {
//            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageWidth = 300;
//            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageHeight = 400;
//            int currentBinaryExpiration = TestConfiguration.OverrideBinaryExpiration;
//            TestConfiguration.OverrideBinaryExpiration = 0;

//            IBinary binary = BinaryFactory.FindBinary("nodate");
//            Assert.NotNull(binary);
//            Assert.True(binary.BinaryData.Length > 100, "byte array is too small, something went wrong");
//            Assert.False(string.IsNullOrEmpty(binary.Id), "binary.Id is missing");
//            Image img = GetImageFromBytes(binary.BinaryData);
//            Assert.True(img.Width == 300);
//            Assert.True(img.Height == 400);

//            // change the generated image dimensions in the provider
//            // this should NOT affect the results because the binary should be cached
//            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageWidth = 400;
//            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageHeight = 200;

//            binary = BinaryFactory.FindBinary("nodate");
//            Assert.NotNull(binary);
//            Assert.True(binary.BinaryData.Length > 100, "byte array is too small, something went wrong");
//            Assert.False(string.IsNullOrEmpty(binary.Id), "binary.Id is missing");
//            img = GetImageFromBytes(binary.BinaryData);
//            Assert.True(img.Width == 400);
//            Assert.True(img.Height == 200);

//            ResetImageDimensions();
//            TestConfiguration.OverrideBinaryExpiration = currentBinaryExpiration;
//        }

//        [Fact]
//        public void FindBinaryOnFSInCache()
//        {
//        }

//        [Fact]
//        public void BinaryCacheIsInvalidated()
//        {
//            MockMessageProvider messageProvider = new MockMessageProvider();
//            messageProvider.Start();
//            ((DefaultCacheAgent)((FactoryBase)BinaryFactory).CacheAgent).Subscribe(messageProvider);

//            ResetImageDimensions();
//            TestConfiguration.OverrideBinaryExpiration = 180;
//            IBinary binary = BinaryFactory.FindBinary("/media/image.png");
//            Assert.NotNull(binary);
//            Assert.True(binary.BinaryData.Length > 100, "byte array is too small, something went wrong");
//            Assert.False(string.IsNullOrEmpty(binary.Id), "binary.Id is missing");
//            Image img = GetImageFromBytes(binary.BinaryData);
//            Assert.True(img.Width == 320);
//            Assert.True(img.Height == 110);

//            ICacheEvent cacheEvent = new CacheEvent()
//            {
//                Key = "1:2",
//                RegionPath = "Binaries",
//                Type = 0
//            };
//            messageProvider.BroadcastCacheEvent(cacheEvent);

//            // change the generated image dimensions in the provider
//            // this should NOT affect the results because the binary should be cached
//            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageWidth = 400;
//            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageHeight = 200;

//            binary = BinaryFactory.FindBinary("/media/image.png");
//            Assert.NotNull(binary);
//            Assert.True(binary.BinaryData.Length > 100, "byte array is too small, something went wrong");
//            Assert.False(string.IsNullOrEmpty(binary.Id), "binary.Id is missing");
//            img = GetImageFromBytes(binary.BinaryData);
//            Assert.True(img.Width == 400);
//            Assert.True(img.Height == 200);

//            ResetImageDimensions();
//        }

//        [Fact]
//        public void FindBinaryWithoutExtension()
//        {
//            IBinary binary = BinaryFactory.FindBinary("/media/image");
//            Assert.NotNull(binary);
//            Assert.True(binary.BinaryData.Length > 100, "byte array is too small, something went wrong");
//        }

//        private Image GetImageFromBytes(byte[] bytes)
//        {
//            MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length);
//            ms.Position = 0; // this is important
//            return Image.FromStream(ms, true);
//        }

//        private void ResetImageDimensions()
//        {
//            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageWidth = 320;
//            ((TridionBinaryProvider)BinaryFactory.BinaryProvider).GeneratedImageHeight = 110;
//        }
//    }
//}