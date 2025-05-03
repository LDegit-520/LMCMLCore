using LMCMLCore.CORE.data;
using LMCMLCore.CORE.Json.Mc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.Json
{
    /// <summary>
    /// 本类用于合并json文件（也就是把mod加载器的json与mc的json合并）
    /// </summary>
    public class MergeJson
    {
        /// <summary>
        /// 合并json文件
        /// </summary>
        /// <param name="path">mod加载器json路径</param>
        /// <param name="mcpath">mc启动json</param>
        /// <returns>mc启动json反序列化类</returns>
        public static ArgumentsJson Merge(string path ,string mcpath)
        {
            IModLoaderJson modLoaderJson = null;
            string json = File.ReadAllText(mcpath);
            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,//忽略null
                WriteIndented = true, // 格式化输出
                Converters = {
                    new ArgumentsJson.Arguments.EitherConverter<string, ArgumentsJson.Arguments.GJRules>(),
                    new ArgumentsJson.Arguments.EitherConverter<string, List<string>>(),
                }
            };
            ArgumentsJson argumentsJson = JsonSerializer.Deserialize<ArgumentsJson>(json, options);
            argumentsJson.mainClass=modLoaderJson.GetMainclass()??argumentsJson.mainClass;//替换mainclass
            argumentsJson.arguments.game.AddRange(modLoaderJson.GetArgumentsGame().Select(s => new ArgumentsJson.Arguments.Either<string,ArgumentsJson.Arguments.GJRules>(s)).ToList());//添加game
            argumentsJson.arguments.jvm.AddRange(modLoaderJson.GetArgumentsJvm().Select(s => new ArgumentsJson.Arguments.Either<string, ArgumentsJson.Arguments.GJRules>(s)).ToList());//添加jvm
            argumentsJson.libraries.AddRange(modLoaderJson.GetLibraries().Select(s => new ArgumentsJson.Librarie { downloads =new () { artifact=new() { 
                url = s.url, sha1 = s.hash, size = s.size, path = s.path.Replace(PATH.LIBRARIES+"/","").Replace(PATH.LIBRARIES+"\\","") } } }).ToList());//添加lib
            return argumentsJson;
        }
    }
}
