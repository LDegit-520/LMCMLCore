using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.FILE
{
    class FileExtractor
    {
        // 生成随机字符串（只包含英文字母和数字）
        private static string GenerateRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }
            return result.ToString();
        }

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="compressedFilePath">压缩文件的地址</param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static string ExtractFile(string compressedFilePath)
        {
            // 检查文件是否存在
            if (!File.Exists(compressedFilePath))
            {
                throw new FileNotFoundException("压缩文件不存在！");
            }

            // 获取文件扩展名
            string extension = Path.GetExtension(compressedFilePath).ToLower();

            // 生成随机文件夹名称
            string randomFolderName = Path.GetFileNameWithoutExtension(compressedFilePath) + "_" + GenerateRandomString(8);
            string extractPath = Path.Combine(Path.GetDirectoryName(compressedFilePath), randomFolderName);

            // 创建解压目录
            Directory.CreateDirectory(extractPath);

            try
            {
                // 根据文件扩展名选择解压方式
                switch (extension)
                {
                    case ".zip":
                    case ".jar":
                        // 使用内置库解压zip和jar文件
                        ZipFile.ExtractToDirectory(compressedFilePath, extractPath);
                        break;
                    case ".gz":
                        // 解压.gz文件到目标文件夹
                        string outputFileName = Path.GetFileNameWithoutExtension(compressedFilePath);
                        string outputFilePath = Path.Combine(extractPath, outputFileName);
                        using (FileStream compressedFileStream = File.OpenRead(compressedFilePath))
                        using (GZipStream decompressionStream = new GZipStream(compressedFileStream, CompressionMode.Decompress))
                        using (FileStream outputFileStream = File.Create(outputFilePath))
                        {
                            decompressionStream.CopyTo(outputFileStream);
                        }
                        break;
                    default:
                        throw new NotSupportedException($"不支持的文件格式: {extension}");
                }

                return extractPath;
            }
            catch (Exception ex)
            {
                // 解压失败时删除已创建的文件夹
                if (Directory.Exists(extractPath))
                {
                    Directory.Delete(extractPath, true);
                }
                throw new InvalidOperationException($"解压失败: {ex.Message}");
            }
        }
        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="cFilePath">压缩文件</param>
        /// <param name="cExPath">解压地址（请注意这个地址为解压后的根地址）</param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static void ExtractFile(string cFilePath,string cExPath)
        {
            // 检查文件是否存在
            if (!File.Exists(cFilePath))
            {
                throw new FileNotFoundException("压缩文件不存在！");
            }

            // 获取文件扩展名
            string extension = Path.GetExtension(cFilePath).ToLower();

            string extractPath = Path.Combine(cExPath,Path.GetFileNameWithoutExtension(cFilePath));
            try
            {
                switch (extension)
                {
                    case ".zip":
                    case ".jar":
                        // 使用内置库解压zip和jar文件
                        ZipFile.ExtractToDirectory(cFilePath, extractPath);
                        break;
                    case ".gz":
                        // 解压.gz文件到目标文件夹
                        string outputFileName = Path.GetFileNameWithoutExtension(cFilePath);
                        string outputFilePath = Path.Combine(extractPath, outputFileName);
                        using (FileStream compressedFileStream = File.OpenRead(cFilePath))
                        using (GZipStream decompressionStream = new GZipStream(compressedFileStream, CompressionMode.Decompress))
                        using (FileStream outputFileStream = File.Create(outputFilePath))
                        {
                            decompressionStream.CopyTo(outputFileStream);
                        }
                        break;
                    default:
                        throw new NotSupportedException($"不支持的文件格式: {extension}");
                }
                return;
            }
            catch (Exception ex)
            {
                // 解压失败时删除已创建的文件夹
                if (Directory.Exists(extractPath))
                {
                    Directory.Delete(extractPath, true);
                }
                throw new InvalidOperationException($"解压失败: {ex.Message}");
            }
        }
    }
}
