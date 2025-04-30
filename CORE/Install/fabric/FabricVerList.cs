using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.Install.fabric
{
    /*
     * Fabric的Loader列表是不分Mc版本的
     * 所以直接调用即可
     */
    class FabricVerList
    {
        /// <summary>
        /// Fabric加载器列表
        /// </summary>
        public static List<string> FabricLoaderList {  get; set; } =new List<string>();
        public static async Task<List<string>> GetFabricLoaderList()
        {
            FabricLoaderList.Clear();
            await FabricCore.getFabricVerJson();
            foreach (var item in FabricCore.fabricVerJson.loader)
            {
                FabricLoaderList.Add(item.version.Replace("+build", ""));
            }
            return FabricLoaderList;
        }
    }
}
