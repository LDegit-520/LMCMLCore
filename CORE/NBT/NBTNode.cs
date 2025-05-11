using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.NBT
{
    /// <summary>
    /// NBT节点 这个是NBT节点的基类，也定义了一些常用静态方法
    /// </summary>
    public class NBTNode
    {
        #region id常量
        /// <summary>
        /// 结束标签（和键值对标签成对应关系）
        /// </summary>
        public const byte END = 0x00;
        /// <summary>
        /// 字节
        /// </summary>
        public const byte BYTE = 0x01;
        /// <summary>
        /// 短整型
        /// </summary>
        public const byte SHORT = 0x02;
        /// <summary>
        /// 整型
        /// </summary>
        public const byte INT = 0x03;
        /// <summary>
        /// 长整型
        /// </summary>
        public const byte LONG = 0x04;
        /// <summary>
        /// 浮点型
        /// </summary>
        public const byte FLOAT = 0x05;
        /// <summary>
        /// 双精度浮点型
        /// </summary>
        public const byte DOUBLE = 0x06;
        /// <summary>
        /// 字节数组
        /// </summary>
        public const byte BYTE_ARRAY = 0x07;
        /// <summary>
        /// 字符串
        /// </summary>
        public const byte STRING = 0x08;
        /// <summary>
        /// 列表
        /// </summary>
        public const byte LIST = 0x09;
        /// <summary>
        /// 键值对标签
        /// </summary>
        public const byte COMPOUND = 0x0A;
        /// <summary>
        /// 整型数组
        /// </summary>
        public const byte INT_ARRAY = 0x0B;
        /// <summary>
        /// 长整型数组
        /// </summary>
        public const byte LONG_ARRAY = 0x0C;
        #endregion
        /// <summary>
        /// id
        /// </summary>
        public byte id { get; set; }
        /// <summary>
        /// 名称大小
        /// </summary>
        public ushort nameSize { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 父节点
        /// </summary>
        public NBTNode parent { get; set; }

        /// <summary>
        /// 生成根节点（也就是读取并生成整个NBT）
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static NBTcompound Bin_Root(List<byte> bytes)
        {
            if (bytes[0]!=COMPOUND)
            {
                throw new Exception("不是NBT文件");
            }
            NBTcompound root = null;//根节点
            NBTSet Current = null;//当前节点
            Stack<(byte,int)> stack = new Stack<(byte,int)>();//列表栈，用于处理列表节点
            //处理流程
            for(int i=0;i<bytes.Count;i++)
            {
                //定义变量
                byte _id = 0xff;//初始值设为0xff，防止被混淆为结束标记
                ushort _nameSize = 0;
                string _name = string.Empty;

                //处理列表栈
                if(stack.Count > 0&&Current is NBTlist)//列表栈不为空
                {
                    i--;//将因循环加上的1减掉，确保下次循环时i的值不变
                    var _stack = stack.Pop();//取出栈顶元素
                    _stack.Item2 -= 1;//列表项数量减1
                    if(_stack.Item2 >= 0)//检查是否再次入栈，注：列表项数量为0时，需要入栈,用于下次循环进行设置当前节点
                    {
                        _id= _stack.Item1;//设置列表项id
                        stack.Push(_stack);//入栈
                    }
                    else if (_stack.Item2 == -1)//列表项数量为-1时，表示当前节点为列表结束标记，无需要入栈，切换节点
                    {
                        Current = (NBTSet)Current.parent;//切换节点
                        continue;//结束本次循环
                    }
                }
                else//列表栈为空
                {
                    //处理id
                    _id = bytes[i];
                    //处理结束
                    if (_id == END)
                    {
                        if (root != Current)//如果当前节点不是根节点
                        {
                            Current = (NBTSet)Current.parent;//当前节点且换为父节点
                        }
                        continue;//结束本次循环
                    }
                    //处理name 名称长度
                    _nameSize = BinConversion.Bin_UShort_Big(bytes.GetRange(i + 1, 2));
                    i = i + 2;
                    //处理name 名称
                    _name = BinConversion.Bin_Modified_UTF_8(bytes.GetRange(i + 1, _nameSize));
                    i = i + _nameSize;
                }

                //处理payload
                switch (_id)
                {
                    case BYTE:
                        {
                            ((List<NBTNode>)Current.payload).Add(new NBTbyte()
                            {
                                id = BYTE,
                                nameSize = _nameSize,
                                name = _name,
                                parent = Current,
                                payload = BinConversion.Bin_Byte(bytes.GetRange(i + 1, 1))
                            });
                            i = i + 1;
                        }
                        break;
                    case SHORT:
                        {
                            ((List<NBTNode>)Current.payload).Add(new NBTshort()
                            {
                                id = SHORT,
                                nameSize = _nameSize,
                                name = _name,
                                parent = Current,
                                payload = BinConversion.Bin_Short_Big(bytes.GetRange(i + 1, 2))
                            });
                            i = i + 2;
                        }
                        break;
                    case INT:
                        {
                            ((List<NBTNode>)Current.payload).Add(new NBTint()
                            {
                                id = INT,
                                nameSize = _nameSize,
                                name = _name,
                                parent = Current,
                                payload = BinConversion.Bin_Int_Big(bytes.GetRange(i + 1, 4))
                            });
                            i = i + 4;
                        }
                        break;
                    case LONG:
                        {
                            ((List<NBTNode>)Current.payload).Add(new NBTlong()
                            {
                                id = LONG,
                                nameSize = _nameSize,
                                name = _name,
                                parent = Current,
                                payload = BinConversion.Bin_Long_Big(bytes.GetRange(i+1, 8))
                            });
                            i = i + 8;
                        }
                        break;
                    case FLOAT:
                        {
                            ((List<NBTNode>)Current.payload).Add(new NBTfloat()
                            {
                                id = FLOAT,
                                nameSize  = _nameSize,
                                name = _name,
                                parent = Current,
                                payload = BinConversion.Bin_Float_Big(bytes.GetRange(i+1, 4))
                            });
                            i = i + 4;
                        }
                        break;
                    case DOUBLE:
                        {
                            ((List<NBTNode>)Current.payload).Add(new NBTdouble()
                            {
                                id  = DOUBLE,
                                nameSize = _nameSize,
                                name = _name,
                                parent = Current,
                                payload = BinConversion.Bin_Double_Big(bytes.GetRange(i+1, 8))
                            });
                            i = i + 8;
                        }
                        break;
                    case BYTE_ARRAY:
                        {
                            var ls_list = new List<byte>(BinConversion.Bin_Int_Big(bytes.GetRange(i + 1, 4)));
                            i = i + 4;
                            for (int j = 0; j < ls_list.Capacity; j++)
                            {
                                ls_list.Add(bytes[i + 1 + j]);
                            }
                            ((List<NBTNode>)Current.payload).Add(new NBTbyteList()
                            {
                                id = BYTE_ARRAY,
                                nameSize = _nameSize,
                                name = _name,
                                parent = Current,
                                payload = ls_list
                            });
                            i = i + ls_list.Count;
                        }
                        break;
                    case STRING:
                        { 
                            string str =BinConversion.Bin_Modified_UTF_8(bytes.GetRange(i+3,BinConversion.Bin_UShort_Big(bytes.GetRange(i+1,2))));
                            ((List<NBTNode>)Current.payload).Add(new NBTstring()
                            {
                                id= STRING,
                                nameSize=_nameSize,
                                name = _name,
                                parent = Current,
                                payload  = str
                            });
                            i = i + 2 + BinConversion.Bin_UShort_Big(bytes.GetRange(i + 1, 2));
                        }
                        break;
                    case LIST:
                        {

                            i = i + 1;
                            var _Child_id = bytes[i];
                            var _Child_size = BinConversion.Bin_Int_Big(bytes.GetRange(i + 1, 4));
                            i = i + 4;
                            //负载
                            //Todo NBT列表负载处理
                            var node = new NBTlist()
                            {
                                id = LIST,
                                nameSize = _nameSize,
                                name = _name,
                                parent = Current,
                                listID = _Child_id,
                                payload = new List<NBTNode>()
                            };
                            ((List<NBTNode>)Current.payload).Add(node);//加入当前节点
                            Current = node;//当前节点设置为本节点
                            stack.Push((_Child_id,_Child_size));//压入列表栈
                        }
                        break;
                    case COMPOUND:
                        {
                            var node = new NBTcompound() { 
                                id=_id,
                                nameSize=_nameSize,
                                name=_name,
                                parent=Current,
                                payload=new List<NBTNode>()
                            };//初始化变量
                            if(i==_nameSize+2)//如果是第一个节点，则初始化根节点
                            {
                                root=node;//设置根节点
                                root.RootSize = bytes.Count;//存储总大小
                            }
                            else//否则设置为数据
                            {
                                ((List<NBTNode>)Current.payload).Add(node);//加入当前节点
                            }
                            node.parent = Current??node;//将当前节点设置为本节点的父节点;如果当前节点为空（也就是本节点为根节点），则将本节点设为当前节点（也就是根节点的父节点是自己本身）
                            Current = node;//将当前节点设为本节点
                        }
                        break;
                    case INT_ARRAY:
                        {
                            var ls_list = new List<int>(BinConversion.Bin_Int_Big(bytes.GetRange(i+1,4)));
                            i = i+4;
                            for (int j = 0; j < ls_list.Capacity; j++)
                            {
                                ls_list.Add(BinConversion.Bin_Int_Big(bytes.GetRange(i+1,4)));
                                i = i+4;
                            }
                            ((List<NBTNode>)Current.payload).Add(new NBTintList()
                            {
                                id=INT_ARRAY,
                                nameSize=_nameSize,
                                name = _name,
                                parent = Current,
                                payload = ls_list
                            });
                        }
                        break;

                    case LONG_ARRAY:
                        {
                            var ls_list = new List<long>(BinConversion.Bin_Int_Big(bytes.GetRange(i + 1, 4)));
                            i = i + 4;
                            for (int j = 0; j < ls_list.Count; j++)
                            {
                                ls_list.Add(BinConversion.Bin_Long_Big(bytes.GetRange(i + 1, 8)));
                                i = i + 8;
                            }
                            ((List<NBTNode>)Current.payload).Add(new NBTlongList()
                            {
                                id = LONG_ARRAY,
                                nameSize=_nameSize,
                                name = _name,
                                parent = Current,
                                payload = ls_list
                            });
                        }
                        break;

                }
            }
            return root;
        }

        /// <summary>
        /// 根节点转二进制
        /// </summary>
        /// <param name="root">根节点</param>
        /// <returns></returns>
        public static List<byte> Root_Bin(NBTcompound root)
        {
            List<byte> bytes = new List<byte>(root.RootSize + 1000);//创建一个字节列表,使用读取的大小加上1000字节
            Root_Bin_Node(root, ref bytes);
            bytes.TrimExcess();//尝试回收内存
            return bytes;//返回字节列表
        }
        /// <summary>
        /// 节点转二进制
        /// </summary>
        /// <param name="node"></param>
        /// <param name="bytes"></param>
        private static void Root_Bin_Node(NBTNode node, ref List<byte> bytes, bool islist = false)
        {
            //Console.Write($"[{node.id}][{node.name}][{bytes.Count}]    ");
            List<byte> name = new List<byte>();
            name.AddRange(BinConversion.UShort_Bin_Big(node.nameSize));
            name.AddRange(BinConversion.Modified_UTF_8_Bin(node.name));
            if (islist)
            {

            }
            else
            {
                bytes.Add(node.id);
                bytes.AddRange(name);
            }
            switch (node.id)
            {
                case BYTE:
                    {
                        bytes.AddRange(BinConversion.Byte_Bin(((NBTbyte)node).payload));
                    }
                    break;
                case SHORT:
                    {
                        bytes.AddRange(BinConversion.Short_Bin_Big(((NBTshort)node).payload));
                    }
                    break;
                case INT:
                    {
                        bytes.AddRange(BinConversion.Int_Bin_Big(((NBTint)node).payload));
                    }
                    break;
                case LONG:
                    {
                        bytes.AddRange(BinConversion.Long_Bin_Big(((NBTlong)node).payload));
                    }
                    break;
                case FLOAT:
                    {
                        bytes.AddRange(BinConversion.Float_Bin_Big(((NBTfloat)node).payload));
                    }
                    break;
                case DOUBLE:
                    {
                        bytes.AddRange(BinConversion.Double_Bin_Big(((NBTdouble)node).payload));
                    }
                    break;
                case BYTE_ARRAY:
                    {
                        var ls_list = (((NBTbyteList)node).payload);
                        bytes.AddRange(BinConversion.Int_Bin_Big(ls_list.Count));
                        for (int i = 0; i < ls_list.Count; i++)
                        {
                            bytes.AddRange(BinConversion.Byte_Bin(ls_list[i]));
                        }
                    }
                    break;
                case STRING:
                    {
                        var ls_list = BinConversion.Modified_UTF_8_Bin(((NBTstring)node).payload);
                        bytes.AddRange(BinConversion.UShort_Bin_Big((ushort)(ls_list.Count)));
                        bytes.AddRange(ls_list);
                    }
                    break;
                case LIST:
                    {
                        bytes.Add(((NBTlist)node).listID);
                        bytes.AddRange(BinConversion.Int_Bin_Big(((NBTlist)node).payload.Count));
                        for (int i = 0; i < ((NBTlist)node).payload.Count; i++)
                        {
                            Root_Bin_Node(((NBTlist)node).payload[i], ref bytes, true);
                        }
                    }
                    break;
                case COMPOUND:
                    {
                        for (int i = 0; i < ((NBTcompound)node).payload.Count; i++)
                        {
                            Root_Bin_Node(((NBTcompound)node).payload[i], ref bytes);
                        }
                        bytes.Add(END);
                    }
                    break;
                case INT_ARRAY:
                    {
                        var ls_list = (((NBTintList)node).payload);
                        bytes.AddRange(BinConversion.Int_Bin_Big(ls_list.Count));
                        for (int i = 0; i < ls_list.Count; i++)
                        {
                            bytes.AddRange(BinConversion.Int_Bin_Big(ls_list[i]));
                        }
                    }
                    break;
                case LONG_ARRAY:
                    {
                        var ls_list = ((NBTlongList)node).payload;
                        bytes.AddRange(BinConversion.Int_Bin_Big(ls_list.Count));
                        for (int i = 0; i < ls_list.Count; i++)
                        {
                            bytes.AddRange(BinConversion.Long_Bin_Big(ls_list[i]));
                        }
                    }
                    break;
            }
        }

        public static void ReadNBT(List<byte> bytes,string path)
        {
            File.WriteAllBytes(path, bytes.ToArray());
        }
    }
    /// <summary>
    /// 泛型 NBT 节点  这个推荐用于代替下面的NBTNode子类
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    public class NBTNode<T> : NBTNode
    {
        public new T payload { get; set; }
    }
    public class NBTItem:NBTNode
    { 
        public object payload { get; set; }
    }
    /// <summary>
    /// 字节
    /// </summary>
    public class NBTbyte : NBTNode
    {
        public new byte payload { get; set; }
    }
    /// <summary>
    /// 短整型
    /// </summary>
    public class NBTshort : NBTNode
    {
        public new short payload { get; set; }
    }
    /// <summary>
    /// 整型
    /// </summary>
    public class NBTint : NBTNode
    {
        public new int payload { get; set; }
    }
    /// <summary>
    /// 长整型
    /// </summary>
    public class NBTlong : NBTNode
    {
        public  new long payload { get; set; }
    }
    /// <summary>
    /// 浮点型
    /// </summary>
    public class NBTfloat : NBTNode
    {
        public new float payload { get; set; }
    }
    /// <summary>
    /// 双精度浮点型
    /// </summary>
    public class NBTdouble : NBTNode
    {
        public new double payload { get; set; }
    }
    /// <summary>
    /// 字节数组(内部使用List)
    /// </summary>
    public class NBTbyteList : NBTNode
    {
        public new List<byte> payload { get; set; }
    }
    /// <summary>
    /// 字符串
    /// </summary>
    public class NBTstring : NBTNode
    {
        public new string payload { get; set; }
    }

    public class NBTSet : NBTNode
    {
        public virtual List<NBTNode> payload { get; set; }
    }
    /// <summary>
    /// 列表
    /// </summary>
    public class NBTlist : NBTSet
    {
        public byte listID { get; set; }
        public override List<NBTNode> payload { get; set; }
    }
    /// <summary>
    /// 键值对
    /// </summary>
    public class NBTcompound : NBTSet
    {
        /// <summary>
        /// 存储根节点大小（适用于从文件读取的NBT）作用为在再次生成二进制NBT时减小性能开销
        /// </summary>
        public int RootSize { get; set; } = 0;
        public override List<NBTNode> payload { get; set; }
    }
    /// <summary>
    /// 整型数组(内部为列表)
    /// </summary>
    public class NBTintList : NBTNode
    {
        public new List<int> payload { get; set; }
    }
    /// <summary>
    /// 长整型数组(内部为列表)
    /// </summary>
    public class NBTlongList : NBTNode
    {
        public new List<long> payload { get; set; }
    }
}
