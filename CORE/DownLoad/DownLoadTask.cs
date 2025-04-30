using LMCMLCore.CORE.FILE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.DownLoad
{
    /*
     * 这是下载任务类
     * 在创建下载任务式请务必看一下这里
     */
    /// <summary>
    /// 下载任务类。。。请注意本类重写了Equals方法，在Url相同时即判定为相同。还重写了GetHashCode方法，在Url相同时即判定为相同。
    /// </summary>
    public class DownLoadTask : IEquatable<DownLoadTask>
    {
        /// <summary>
        /// 下载地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 保存地址
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 文件大小，默认为3MB
        /// </summary>
        public long Size { get; set; } = 3 * 1024 * 1024;
        /// <summary>
        /// 文件哈希值，默认为 string.Empty
        /// </summary>
        public string Hash { get; set; } = string.Empty;
        /// <summary>
        /// 哈希值类型,默认为SHA1(因为mc默认提供的就是SHA1)
        /// </summary>
        public HashInfo HashInfo { get; set; } = HashInfo.SHA1;
        /// <summary>
        /// 任务id，不提供set方法，这个同时也会作为临时文件名使用
        /// <para>生存方式为对Url使用SHA256算法生成，理论上不会出现不同Url为同一id情况</para>
        /// </summary>
        public string Id { get; } = string.Empty;

        /// <summary>
        /// 基础构造函数，适合简单生成任务(id为自动生成)
        /// <para>Size 文件大小，默认为3MB</para>
        /// <para>Hash 文件哈希值，默认为 string.Empty</para>
        /// <para>HasjInfo 哈希值类型,默认为SHA1</para>
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="path">保存地址</param>
        public DownLoadTask(string url, string path)
        {
            Url = url;
            Path = path;
            Id = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Url))).Replace("-", "").ToLower();
        }
        /// <summary>
        /// 基础构造函数，适合简单生成任务(id为自动生成)
        /// <para>Size 文件大小，默认为3MB</para>
        /// <para>Hash 文件哈希值，默认为 string.Empty</para>
        /// <para>HasjInfo 哈希值类型,默认为SHA1</para>
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="path">保存地址</param>
        /// <param name="size">文件大小</param>
        public DownLoadTask(string url, string path, long size)
        {
            Url = url;
            Path = path;
            Size = size;
            Id = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Url))).Replace("-", "").ToLower();
        }
        /// <summary>
        /// 基础构造函数，适合简单生成任务(id为自动生成)
        /// <para>Size 文件大小，默认为3MB</para>
        /// <para>Hash 文件哈希值，默认为 string.Empty</para>
        /// <para>HasjInfo 哈希值类型,默认为SHA1</para>
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="path">保存地址</param>
        /// <param name="hash">哈希值</param>
        /// <param name="hashInfo">哈希值类型</param>
        public DownLoadTask(string url, string path, string hash, HashInfo hashInfo)
        {
            Url = url;
            Path = path;
            Hash = hash;
            HashInfo = hashInfo;
            Id = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Url))).Replace("-", "").ToLower();
        }
        /// <summary>
        /// 基础构造函数，适合简单生成任务(id为自动生成)
        /// <para>Size 文件大小，默认为3MB</para>
        /// <para>Hash 文件哈希值，默认为 string.Empty</para>
        /// <para>HasjInfo 哈希值类型,默认为SHA1</para>
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="path">保存地址</param>
        /// <param name="size">文件大小</param>
        /// <param name="hash">哈希值</param>
        /// <param name="hashInfo">哈希值类型</param>
        public DownLoadTask(string url, string path, long size, string hash, HashInfo hashInfo)
        {
            Url = url;
            Path = path;
            Size = size;
            Hash = hash;
            HashInfo = hashInfo;
            Id = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Url))).Replace("-", "").ToLower();
        }



        // 非泛型 Equals 方法（重写 Object.Equals）
        public override bool Equals(object obj)
        {
            // 委托给泛型 Equals 方法
            return Equals(obj as DownLoadTask);
        }

        // 泛型 Equals 方法（实现 IEquatable<T>）
        public bool Equals(DownLoadTask? other)
        {
            if (other == null) return false;
            return Url == other.Url;
        }

        public override int GetHashCode()
        {
            // 使用 C# 7.3+ 的 HashCode.Combine 方法（推荐）
            return HashCode.Combine(Url);
        }
    }
    public static class Extensions
    {
        /// <summary>
        /// 扩展方法：根据指定的键选择器去重
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<T> DistinctBy<T, TKey>(
            this IEnumerable<T> source,
            Func<T, TKey> keySelector)
        {
            var seen = new HashSet<TKey>(); // 用于存储已出现的键
            foreach (var element in source)
            {
                // 提取键，若键未被记录则添加到集合，并返回 true
                if (seen.Add(keySelector(element)))
                {
                    yield return element; // 仅返回第一个出现的元素
                }
            }
        }
    }
}
