using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.MessageBus
{
    public class MessageBus
    {
        private readonly Dictionary<string, List<MessageSub>> _subs = new Dictionary<string, List<MessageSub>>();
        public void Publish(MessageBase message)
        {
            if (_subs.ContainsKey(message.Type)) 
            {
                foreach (var sub in _subs[message.Type])
                {
                    sub.handler(message);
                }
            }
        }
        public void Subscribe(string type, Action<MessageBase> handler)
        {
            if (!_subs.ContainsKey(type))
            {
                _subs.Add(type, new List<MessageSub>());
            }
            _subs[type].Add(new MessageSub() { handler = handler });
        }
    }
    /// <summary>
    /// 消息基类
    /// </summary>
    public class MessageBase
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public string Type  { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }
    }
    public class MessageSub
    {
        public Action<MessageBase> handler { get; set; }
    }
}