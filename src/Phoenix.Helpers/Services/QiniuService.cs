using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Phoenix.Configuration;
using Qiniu.Conf;
using Qiniu.IO;
using Qiniu.RS;
using SS.Toolkit.Helpers;
using SS.Toolkit.Http;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Phoenix.Services
{
    public class QiniuService
    {
        private readonly IOptions<QiniuSettings> _qiniuSettings;

        public QiniuService(IOptions<QiniuSettings> qiniuSettings)
        {
            _qiniuSettings = qiniuSettings;
            Config.ACCESS_KEY = _qiniuSettings.Value.AccessKey;
            Config.SECRET_KEY = _qiniuSettings.Value.SecretKey;
            Config.UP_HOST = _qiniuSettings.Value.UPHost;
        }

        public async Task<string> Upload(HttpContent httpContent)
        {
            string fileExtension = "png";
            switch (httpContent.Headers.ContentType.ToString())
            {
                case "image/gif":
                    fileExtension = "gif";
                    break;
                case "image/jpeg":
                    fileExtension = "jpg";
                    break;
                case "image/png":
                    fileExtension = "png";
                    break;
                case "image/bmp":
                    fileExtension = "bmp";
                    break;
            }
            string url = string.Empty;
            using (var stream = httpContent.ReadAsStreamAsync().Result)
            {
                string guid = GuidHelper.Gen().ToString();
                url = await Upload(guid.ToString() + "." + fileExtension, stream);
            }
            return url;
        }

        public async Task<string> Upload(IFormFile file)
        {

            string fileExtension = "png";
            switch (file.ContentType)
            {
                case "image/gif":
                    fileExtension = "gif";
                    break;
                case "image/jpeg":
                    fileExtension = "jpg";
                    break;
                case "image/png":
                    fileExtension = "png";
                    break;
                case "image/bmp":
                    fileExtension = "bmp";
                    break;
            }
            string url = string.Empty;
            using (var stream = file.OpenReadStream())
            {
                string guid = GuidHelper.Gen().ToString();
                url = await Upload(guid.ToString() + "." + fileExtension, stream);
            }
            return url;
        }

        public async Task<string> Upload(Uri uri)
        {
            string guid = GuidHelper.Gen().ToString();
            AsyncHttpClient asyncHttpClient = new AsyncHttpClient();
            var result = await asyncHttpClient.DefaultUserAgent().Referer(uri.AbsoluteUri).Uri(uri).Get();
            string fileExtension = "png";
            return await Upload(guid.ToString() + "." + fileExtension, result.GetBytes());
        }

        public async Task<string> Upload(string key, byte[] buffer)
        {
            using (Stream stream = new MemoryStream(buffer))
            {
                return await Upload(key, stream);
            }
        }


        public async Task<string> Upload(string key, Stream stream)
        {
            var target = new IOClient();
            var result = await target.PutAsync(new PutPolicy(_qiniuSettings.Value.Bucket).Token(), key, stream, null);
            var url = string.Format("{0}/{1}", _qiniuSettings.Value.Domain, result.key);
            return url;
        }

        public async Task<string> Upload(byte[] buffer)
        {
            string fileExtension = "png";
            string guid = GuidHelper.Gen().ToString();
            return await Upload(guid.ToString() + "." + fileExtension, buffer);
        }
    }
}
