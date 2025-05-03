using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.MessageBus
{
    public class MessageBus
    {
        public void AddNotifyEvent(MessageTitleContent message)
        {
            
        }
        public bool AddReturnEvent(MessageTitleContent message)
        {

            return true;
        }
        public T AddReturnEvent<T>(MessageTitleContent message) where T : class,  new()
        {

            return new T();
        }
    }
    public class MessageTitleContent
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public MessageTitleContent(string content)
        {
            Title  = I18N.I18NString.MessageTitleContent_Title_moren;
            Content = content;
        }
        public MessageTitleContent(string title, string content)
        {
            Title = title;
            Content = content;
        }
    }
    /*
     * 消息总线
     * 应该为单例模式
     * 属性
     *      
     * 字段
     *      存储消息的字典
     * 方法
     *      添加通知事件
     *      添加控制事件
     * 事件
     *      通知事件，不需要返回值的事件
     *      控制事件，需要返回值的事件
     *
     */
}
