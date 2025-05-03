using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.data
{
    /// <summary>
    /// 本类用于启动时初始化数据
    /// </summary>
    public class Start
    {
        public Start()
        {
            PATH.CPATH();//文件夹初始化
            //I18n简中文件生成和i18n配置生成
            if (!File.Exists(PATH.I18N_JSON)) { I18N.I18NString.I18NJSON(PATH.I18N_JSON);}
            if (!File.Exists(PATH.I18N_ZH_CN_JSON)) { I18N.I18NString.GetI18N(PATH.I18N_ZH_CN_JSON);}
            //i18n设置
            var path=JsonSerializer.Deserialize<Dictionary<string,string>>(File.ReadAllText(PATH.I18N_JSON));
            I18N.I18NString.SetI18N(path["I18N"].Replace(PATH._LMCML_STR,PATH.EXE));
            //i18n结束
            DATA.CDATA();//数据初始化
        }
    }
}
