using Aliyun.OSS;
using Elderly_Canteen;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using Elderly_Canteen.Services.Interfaces;
using Elderly_Canteen.Data.Entities;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Elderly_Canteen.Services.Implements
{
    public class OSSService : IOssService
    {
        private readonly OssClient _ossClient;
        private readonly string _bucketName;

        public OSSService(string endpoint, string accessKeyId, string accessKeySecret, string bucketName)
        {
            _ossClient = new OssClient(endpoint, accessKeyId, accessKeySecret);
            if (_ossClient == null)
            {
                ;
            }
            _bucketName = bucketName;
        }

        public async Task<string> UploadFileAsync(IFormFile file,string fileName)
        {
            try
            {
                
                using (var stream = file.OpenReadStream())
                {
                    // 上传文件到指定的bucket和路径
                    var putObjectRequest = new PutObjectRequest(_bucketName, fileName, stream);
                    _ossClient.PutObject(putObjectRequest);
                    // 构建文件的访问URL
                    var imageUrl = _ossClient.GeneratePresignedUri(_bucketName, fileName, DateTime.Now.AddYears(1)).ToString();
                    return imageUrl;
                }
            }
            catch (Exception ex)
            {
                // 处理上传过程中的异常
                throw new Exception("图片上传到OSS失败", ex);
            }
        }

        public string GetDefaultImageUrl()
        {
            try
            {
                // 指向default-dish.jpg的签名URL，设置为一年过期
                var defaultImageKey = "default-dish.jpg";
                var imageUrl = _ossClient.GeneratePresignedUri(_bucketName, defaultImageKey, DateTime.Now.AddYears(1)).ToString();
                return imageUrl;
            }
            catch (Exception ex)
            {
                // 处理生成URL过程中的异常
                throw new Exception("获取默认图片URL失败", ex);
            }
        }


    }


}
