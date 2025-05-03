using LMCMLCore.CORE.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.Model
{
    /*
     * 事件线，内核需要传递的事件均从这里传递
     * 例如，ui传递一个操作让内核执行，内核执行后发现出现问题【1】提示问题（也就是不需要ui返回）【2】选择问题（需要ui返回）
     * 那就添加进入这里，ui可以通过回调得知，并显示【1】【2】，然后返回一个答案给【2】
     */
    /// <summary>
    /// 事件主线通过Instance获取本类的唯一实例
    /// ui注册OnNotEvent事件获取事件回调
    /// 内核通过Add方法传递参数和获取返回
    /// </summary>
    class EventThread
    {
        // 静态实例容器，Lazy 保证线程安全和延迟初始化
        private static readonly Lazy<EventThread> _instance =
            new Lazy<EventThread>(() => new EventThread());

        // 公共访问点
        public static EventThread Instance => _instance.Value;


        // 私有构造函数防止外部实例化  
        private EventThread()
        {
            
        }
        public event EventHandler<NotEventArgs> OnNotEvent;
        public MRingBuffer<NotEventArgs> _Data = new MRingBuffer<NotEventArgs>(SETTING.EventThreadDataSize);
        public void Add(NotEventArgs args)
        {
            _Data.Add(args);
            OnNotEventArgs(args);
        }
        public (string s, int i, bool b, object o) Add(NotEventArgs args,bool isc=true)
        {
            _Data.Add(args);
            OnNotEventArgs(args);
            return args.Sreturn;
        }
        private void OnNotEventArgs(NotEventArgs args)
        {
            // 创建临时引用避免竞态条件
            var handlers = OnNotEvent;

            if (handlers != null)
            {
                handlers(this, args);
            }
        }
        /// <summary>
        /// 事件（默认使用这个，会传递字典进入（字典一定含有(text）可能含有）
        /// <para>如果需要传递非字符字典请使用本类的泛型类<seealso cref="NotEventArgs{T}"/></para>
        /// </summary>
        public class NotEventArgs : EventArgs
        {
            public NotEventArgs(Dictionary<string, string> dic)
            {
                SMessarg = dic;
            }
            public Dictionary<string, string> SMessarg { get; set; } = new()
            {
                { "title","" },
                { "text","" },
            };
            public bool Isreturn { get; set; } = false;
            /// <summary>
            /// 返回值 ,请同步修改上面的是否返回的布尔值
            /// </summary>
            public (string s, int i, bool b, object o) Sreturn { get; set; }
        }
        public class NotEventArgs<T> : NotEventArgs where T : class
        {
            public NotEventArgs(T obj,string name, Dictionary<string, string> dic=null): base(dic)
            {
                OMessarg = obj;
                OMessargName = name;
            }
            public T OMessarg { get; }
            public string OMessargName { get; }
        }
    }
}
