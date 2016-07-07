using System;
using DD4T.ContentModel.Contracts.Providers;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using DD4T.ContentModel;

namespace DD4T.Providers.Test
{
    /// <summary>
    /// Provide access to binaries in a Tridion broker instance
    /// </summary>
    public class TridionBinaryProvider : BaseProvider, IBinaryProvider
    {

        public int GeneratedImageWidth = 320;
        public int GeneratedImageHeight = 110;


        public byte[] GetBinaryByUri(string uri)
        {
            return GenerateImage();
        }

        public byte[] GetBinaryByUrl(string url)
        {
            return GenerateImage();
        }

       

        public System.IO.Stream GetBinaryStreamByUri(string uri)
        {
            throw new NotImplementedException();
        }

        public System.IO.Stream GetBinaryStreamByUrl(string url)
        {
            throw new NotImplementedException();
        }

        public DateTime GetLastPublishedDateByUrl(string url)
        {
            return DateTime.Now.AddHours(-8);
        }

        public DateTime GetLastPublishedDateByUri(string uri)
        {
            return DateTime.Now.AddHours(-8);
        }


        public string GetUrlForUri(string uri)
        {
            throw new NotImplementedException();
        }

        private byte[] GenerateImage()
        {
            using (var rectangleFont = new Font("Arial", 14, FontStyle.Bold))
            using (var bitmap = new Bitmap(GeneratedImageWidth, GeneratedImageHeight, PixelFormat.Format24bppRgb))
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var backgroundColor = Color.Bisque;
                g.Clear(backgroundColor);
                g.DrawString("This PNG was totally generated", rectangleFont, SystemBrushes.WindowText, new PointF(10, 40));
                using (var ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }


       

        public IBinaryMeta GetBinaryMetaByUri(string uri)
        {
            if (uri == "nodate")
            {
                return new BinaryMeta()
                {
                    Id = "tcm:1-2",
                    VariantId = "variant",
                    HasLastPublishedDate = false
                };
            }
            return new BinaryMeta()
            {
                Id = uri,
                LastPublishedDate = DateTime.Now,
                VariantId = "variant",
                HasLastPublishedDate = true
            };
        }

        public IBinaryMeta GetBinaryMetaByUrl(string url)
        {
            if (url == "nodate")
            {
                return new BinaryMeta()
                {
                    Id = "tcm:1-2",
                    VariantId = "variant",
                    HasLastPublishedDate = false
                };

            }
            return new BinaryMeta()
            {
                Id = "tcm:1-2",
                LastPublishedDate = DateTime.Now,
                VariantId = "variant",
                HasLastPublishedDate = true
            };
        }
    }
}
