using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.FILE
{
    /*
     * 哈希类
     * 用于对哈希值的检查生成判断
     */
    /// <summary>
    /// 哈希处理
    /// </summary>
    public class Hash
    {
        /// <summary>
        /// 计算文件的指定类型的哈希值，默认为SHA1
        /// </summary>
        /// <param name="path">文件的地址</param>
        /// <param name="hashinfo">哈希值类型默认为sha1</param>
        /// <returns>指定类型哈希值，如果出现其他情况会返回strin.Empty</returns>
        public static string FileHash(string path, HashInfo hashinfo = HashInfo.SHA1)
        {
            try
            {
                using (var stream = File.OpenRead(path))
                {

                    using HashAlgorithm algorithm = hashinfo switch
                    {
                        HashInfo.SHA256 => SHA256.Create(),
                        HashInfo.SHA384 => SHA384.Create(),
                        HashInfo.SHA512 => SHA512.Create(),
                        HashInfo.MD5 => MD5.Create(),
                        HashInfo.SHA1 => SHA1.Create(),
                        HashInfo.None => SHA1.Create(),
                        _ => SHA1.Create() // 默认使用SHA1
                    };

                    var hash = algorithm.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLower();
                }
            }
            catch (Exception e)
            {
                //Logger.Error(nameof(Filesha), $"{e.Message}");
                Console.WriteLine(e);
            }
            return string.Empty;
        }
    }
    public enum HashInfo
    {
        /// <summary>
        /// 没有
        /// </summary>
        None,
        /// <summary>
        /// MD5
        /// </summary>
        MD5,
        /// <summary>
        /// sha1//mc默认是这个
        /// </summary>
        SHA1,
        /// <summary>
        /// sha256
        /// </summary>
        SHA256,
        /// <summary>
        /// sha384
        /// </summary>
        SHA384,
        /// <summary>
        /// sha512
        /// </summary>
        SHA512,
    }
}
