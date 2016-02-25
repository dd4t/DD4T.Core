using System;
using System.Collections.Generic;
using System.IO;
using DD4T.ContentModel;
using DD4T.ContentModel.Contracts.Providers;
using DD4T.ContentModel.Exceptions;
using DD4T.Utils.Caching;
using DD4T.ContentModel.Factories;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;

namespace DD4T.Factories
{
    public class BinaryFactory : FactoryBase, IBinaryFactory
    {
        public const string CacheRegion = "Binary";
        const string CacheValueNullTitle = "DD4T-Special-Value-BinaryNotFound";
        private IBinary CacheValueNull;
        private static IDictionary<string, DateTime> lastPublishedDates = new Dictionary<string, DateTime>();
        public IBinaryProvider BinaryProvider { get; set; }


        public BinaryFactory(IBinaryProvider binaryProvider, IFactoryCommonServices factoryCommonServices)
            : base(factoryCommonServices) 
        {
            if (binaryProvider == null)
                throw new ArgumentNullException("binaryProvider");
            BinaryProvider = binaryProvider;
            CacheValueNull = new Binary() { Title = CacheValueNullTitle };
        }

        #region IBinaryFactory members
        public bool FindAndStoreBinary(string url, string physicalPath)
        {
            IBinary binary = new Binary();
            return TryFindBinary(url, physicalPath, false, out binary);
        }

       
        public bool TryFindBinary(string url, out IBinary binary)
        {
            return TryFindBinary(url, null, true, out binary);

         
        }

        public IBinary FindBinary(string url)
        {
            IBinary binary;
            if (!TryFindBinary(url, out binary))
            {
                throw new BinaryNotFoundException();
            }
            return binary;
        }


        
        public DateTime FindLastPublishedDate(string url)
        {
            return BinaryProvider.GetLastPublishedDateByUrl(url);
        }

        public bool TryGetBinary(string tcmUri, out IBinary binary)
        {
            binary = new Binary();
            if (LoadBinariesAsStream)
            {
                LoggerService.Information("retrieving binaries as a stream is obsolete; support will be dropped in future versions of DD4T");
                binary.BinaryStream = BinaryProvider.GetBinaryStreamByUri(tcmUri);
                if (binary.BinaryStream == null)
                    return false;
            }
            else
            {

                binary.BinaryData = BinaryProvider.GetBinaryByUri(tcmUri);
                if (binary.BinaryData == null || binary.BinaryData.Length == 0)
                    return false;
            }
            ((Binary)binary).Id = tcmUri;
            return true;
        }

        public IBinary GetBinary(string tcmUri)
        {
            IBinary binary;
            if (!TryGetBinary(tcmUri, out binary))
            {
                throw new BinaryNotFoundException();
            }
            return binary;
        }

        public bool TryFindBinaryContent(string url, out byte[] bytes)
        {
            bytes = null;
            try
            {
                bytes = BinaryProvider.GetBinaryByUrl(url);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public byte[] FindBinaryContent(string url)
        {
            byte[] bytes;
            if (!TryFindBinaryContent(url, out bytes))
            {
                throw new BinaryNotFoundException();
            }
            return bytes;
        }

        public bool TryGetBinaryContent(string uri, out byte[] bytes)
        {
            bytes = null;
            try
            {
                bytes = BinaryProvider.GetBinaryByUri(uri);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public byte[] GetBinaryContent(string uri)
        {
            byte[] bytes;
            if (!TryFindBinaryContent(uri, out bytes))
            {
                throw new BinaryNotFoundException();
            }
            return bytes;
        }

        public bool HasBinaryChanged(string url)
        {
            return true; // TODO: implement
        }

        public IBinaryMeta FindBinaryMeta(string url)
        {
            string cacheKey = CacheKeyFactory.GenerateKey(url);
            IBinaryMeta binaryMeta = CacheAgent.Load(cacheKey) as IBinaryMeta;
            if (binaryMeta != null)
            {
                return binaryMeta;
            }
            binaryMeta = BinaryProvider.GetBinaryMetaByUrl(url);
            if (binaryMeta == null) // TODO: cache null result
            {
                return null;
            }
            CacheAgent.Store(cacheKey, "Binary", binaryMeta, new List<string> { binaryMeta.Id });
            return binaryMeta;
        }

        public IBinaryMeta GetBinaryMeta(string uri)
        {
            return BinaryProvider.GetBinaryMetaByUri(uri);
        }



        #endregion

        #region private

        private bool TryFindBinary(string url, string localPath, bool retrieveData, out IBinary binary)
        {
            string physicalPath = localPath ?? Path.Combine(Configuration.BinaryFileSystemCachePath, Path.GetFileName(url));

            binary = new Binary();

            if (LoadBinariesAsStream)
            {
                LoggerService.Information("retrieving binaries as a stream is obsolete; support will be dropped in future versions of DD4T");
                binary.BinaryStream = BinaryProvider.GetBinaryStreamByUrl(url);
                if (binary.BinaryStream == null)
                    return false;
            }

            Dimensions dimensions = null;

            string urlWithoutDimensions = StripDimensions(url, out dimensions);
            string cacheKey = CacheKeyFactory.GenerateKey(urlWithoutDimensions);

            try
            {
                IBinaryMeta binaryMeta = CacheAgent.Load(cacheKey) as IBinaryMeta;
                bool metaWasInCache = true;
                if (binaryMeta == null)
                {
                    metaWasInCache = false;
                    binaryMeta = BinaryProvider.GetBinaryMetaByUrl(urlWithoutDimensions);
                    if (binaryMeta == null)
                    {
                        throw new BinaryNotFoundException();
                    }
                    CacheAgent.Store(cacheKey, "Binary", binaryMeta, new List<string> { binaryMeta.Id });
                }

                if (FileExistsAndIsNotEmpty(physicalPath))
                {
                    if (binaryMeta.HasLastPublishedDate || metaWasInCache)
                    {
                        DateTime fileModifiedDate = File.GetLastWriteTime(physicalPath);
                        if (fileModifiedDate.CompareTo(binaryMeta.LastPublishedDate) >= 0)
                        {
                            LoggerService.Debug("binary {0} is still up to date, no action required", urlWithoutDimensions);
                            // TODO: load bytes from file system into binary 
                            if (retrieveData)
                            {
                                FillBinaryFromLocalFS(binary, physicalPath);
                            }
                            CopyBinaryMetaToBinary(binaryMeta, binary);
                            return true;
                        }
                    }
                }


                // the normal situation (where a binary is still in Tridion and it is present on the file system already and it is up to date) is now covered
                // Let's handle the exception situations. 

                byte[] bytes = BinaryProvider.GetBinaryByUrl(urlWithoutDimensions);
                if (bytes == null || bytes.Length == 0)
                {
                    throw new BinaryNotFoundException();
                }


                bool fileIsCreated = WriteBinaryToFile(bytes, physicalPath, dimensions);
                if (! fileIsCreated)
                {
                    LoggerService.Warning($"file '{physicalPath}' could not be created, binary {binary.Id} cannot be returned");
                    return false;
                }
                if (retrieveData)
                {
                    if (dimensions == null)
                    {
                        binary.BinaryData = bytes;
                    }
                    else
                    {
                        FillBinaryFromLocalFS(binary, physicalPath);
                    }
                }
                CopyBinaryMetaToBinary(binaryMeta, binary);
                return true;

            }
            catch (BinaryNotFoundException)
            {
                LoggerService.Debug("Binary with url {0} not found", urlWithoutDimensions);
                // binary does not exist in Tridion, it should be removed from the local file system too
                if (File.Exists(physicalPath))
                {
                    DeleteFile(physicalPath);
                }
                return false;

            }
            catch(Exception e)
            {
                LoggerService.Warning($"Caught unexpected exception while retrieving binary with url {urlWithoutDimensions} (requested url: {url}. Error message: {e.Message}\r\n{e.StackTrace}");
                if (File.Exists(physicalPath))
                {
                    DeleteFile(physicalPath);
                }
                return false;
            }
        }

        private void CopyBinaryMetaToBinary(IBinaryMeta binaryMeta, IBinary binary)
        {
            ((Binary)binary).Id = binaryMeta.Id;
            ((Binary)binary).VariantId = binaryMeta.VariantId;
            ((Binary)binary).LastPublishedDate = binaryMeta.LastPublishedDate;
        }

        private void FillBinaryFromLocalFS(IBinary binary, string physicalPath)
        {
            binary.BinaryData = File.ReadAllBytes(physicalPath);
        }

        private void DeleteFile(string physicalPath)
        {
            if (File.Exists(physicalPath))
            {
                LoggerService.Debug("requested binary {0} no longer exists in broker. Removing...", physicalPath);
                File.Delete(physicalPath); // file got unpublished
                LoggerService.Debug("done ({0})", physicalPath);
            }
        }

        private bool FileExistsAndIsNotEmpty(string physicalPath)
        {
            if (File.Exists(physicalPath))
            {
                FileInfo fi = new FileInfo(physicalPath);
                if (fi.Length > 0)
                {

                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Perform actual write of binary content to file
        /// </summary>
        /// <param name="binaryMeta">BinaryMeta the binary meta to store</param>
        /// <param name="physicalPath">String the file path to write to</param>
        /// <returns></returns>
        private bool WriteBinaryToFile(byte[] bytes, String physicalPath, Dimensions dimensions)
        {
            bool result = true;
            FileStream fileStream = null;
            try
            {
                lock (physicalPath)
                {
                    if (File.Exists(physicalPath))
                    {
                        fileStream = new FileStream(physicalPath, FileMode.Create);
                    }
                    else
                    {
                        FileInfo fileInfo = new FileInfo(physicalPath);
                        if (!fileInfo.Directory.Exists)
                        {
                            fileInfo.Directory.Create();
                        }
                        fileStream = File.Create(physicalPath);
                    }

                    byte[] buffer = bytes;

                    if (dimensions != null)
                        buffer = ResizeImageFile(buffer, dimensions, GetImageFormat(physicalPath));
                    fileStream.Write(buffer, 0, buffer.Length);
                }
            }
            catch (Exception e)
            {
                LoggerService.Error("Exception occurred {0}\r\n{1}", e.Message, e.StackTrace);
                result = false;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }

            return result;
        }

        internal static byte[] ResizeImageFile(byte[] imageFile, Dimensions dimensions, ImageFormat imageFormat)
        {

            int targetH, targetW;
            Image original = Image.FromStream(new MemoryStream(imageFile));
            if (dimensions.Width > 0 && dimensions.Height > 0 && !(dimensions.Width == original.Width && dimensions.Height == original.Height))
            {
                targetW = dimensions.Width;
                targetH = dimensions.Height;
            }
            else if (dimensions.Width > 0 && dimensions.Width != original.Width)
            {
                targetW = dimensions.Width;
                targetH = (int)(original.Height * ((float)targetW / (float)original.Width));
            }
            else if (dimensions.Height > 0 && dimensions.Height != original.Height)
            {
                targetH = dimensions.Height;
                targetW = (int)(original.Width * ((float)targetH / (float)original.Height));
            }
            else
            {
                //No need to resize the image, return the original bytes.
                return imageFile;
            }

            Image imgPhoto = null;
            using (MemoryStream memoryStream = new MemoryStream(imageFile))
            {
                imgPhoto = Image.FromStream(memoryStream);
            }
            if (imgPhoto == null)
            {
                throw new Exception("cannot read image, binary data may not represent an image");
            }
            // Create a new blank canvas.  The resized image will be drawn on this canvas.
            Bitmap bmPhoto = new Bitmap(targetW, targetH, PixelFormat.Format24bppRgb);
            Bitmap bmOriginal = new Bitmap(original);
            bmPhoto.SetResolution(72, 72);
            bmPhoto.MakeTransparent(bmOriginal.GetPixel(0, 0));
            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;
            grPhoto.DrawImage(imgPhoto, new Rectangle(0, 0, targetW, targetH), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel);
            // Save out to memory and then to a file.  We dispose of all objects to make sure the files don't stay locked.
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bmPhoto.Save(memoryStream, imageFormat);
                original.Dispose();
                imgPhoto.Dispose();
                bmPhoto.Dispose();
                grPhoto.Dispose();
                return memoryStream.GetBuffer();
            }
        }
        private ImageFormat GetImageFormat(string path)
        {
            switch (Path.GetExtension(path).ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    return ImageFormat.Jpeg;
                case ".png":
                    return ImageFormat.Png;
                case ".gif":
                    return ImageFormat.Gif;
            }
            return ImageFormat.Png; // use png as default
        }
        private IBinary GetIBinaryObject(byte[] binaryContent, string url)
        {
            IBinary binary = new Binary();
            binary.BinaryData = binaryContent;
            binary.Url = url;
            return binary;
        }
        private IBinary GetIBinaryObject(Stream binaryStream, string url)
        {
            IBinary binary = new Binary();
            binary.BinaryStream = binaryStream;
            binary.Url = url;
            return binary;
        }
        #endregion


        public static bool DefaultLoadBinariesAsStream = false;
        private bool _loadBinariesAsStream = DefaultLoadBinariesAsStream;
        public bool LoadBinariesAsStream
        {
            get
            {
                return _loadBinariesAsStream;
            }
            set
            {
                _loadBinariesAsStream = value;
            }
        }

        [Obsolete]
        public override DateTime GetLastPublishedDateCallBack(string key, object cachedItem)
        {
            throw new NotImplementedException();
        }

        public string GetUrlForUri(string uri)
        {
            return BinaryProvider.GetUrlForUri(uri);
        }

        private string StripDimensions(string path, out Dimensions dimensions)
        {
            Regex re = new Regex(@"_([hw])(\d+)\.");
            if (re.IsMatch(path))
            {
                string d = re.Match(path).Groups[1].ToString();
                string v = re.Match(path).Groups[2].ToString();
                if (d == "h")
                    dimensions = new Dimensions() { Height = Convert.ToInt32(v) };
                else
                    dimensions = new Dimensions() { Width = Convert.ToInt32(v) };
                return re.Replace(path, ".");
            }
            dimensions = null;
            return path;
        }


        #region inner class Dimensions
        internal class Dimensions
        {
            internal int Width; internal int Height;
        }
        #endregion

    }
}
