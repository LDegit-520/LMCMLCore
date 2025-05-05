using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.NBT
{
    public class NBTjc
    {
        public List<byte> bytes;
        public NBT_Set root;
        public NBTjc(string path)
        {
            bytes = File.ReadAllBytes(path).ToList();
            if (bytes[0] != 0x0A)
            {
                throw new Exception("没有根节点，请检查是否为NBT文件");
            }
            NBT_Set CNode = null;//当前节点
            Stack<(byte, int)> CidSize=new();//id堆栈
            byte? Cid = null;//当前id（给列表使用）
            int? Cidsize = 0;//当前长度
            for (int i = 0; i < bytes.Count; i++)
            {
                byte Lid = 0;
                ushort namesize = 0;
                string name = "";
                int Psize = 0;
                
                if (CidSize.Count > 0)
                {
                    (Cid,Cidsize)= CidSize.Pop();//当前堆顶id
                    
                    if (Cidsize == 0)
                    {
                        //CidSize.Pop();//移出堆顶
                        CNode = CNode.Parent;//当前节点转为父节点
                        i--;
                        continue;//结束本次循环
                    }
                    else
                    {
                        i--;
                        Lid = Cid ?? 0x00;
                        namesize = 0;
                        Psize = 0;
                        name = "";
                        CidSize.Push((Cid ?? 0x00, Cidsize - 1 ?? 0));//重新入栈
                    }
                }
                else
                {
                    if (bytes[i] == 0x00)//结束标记
                    {
                        if (CNode?.Parent != null)//父节点不为空,为空的代表为根节点
                        {
                            CNode = CNode.Parent;//当前节点转为父节点
                        }
                        continue;
                    }
                    Lid = bytes[i];
                    namesize = (ushort)((bytes[i + 1] << 8) | bytes[i + 2]);
                    name = DisModified_UTF_8(bytes.GetRange(i + 3, namesize));//名称
                    Psize = 2;
                }
                switch (Lid)
                {
                    case 0x01:
                        {
                            byte Lpayload = bytes[i + Psize + 1 + namesize];
                            i = i + Psize + namesize + 1;
                            CNode.payload.Add(new NBT_item
                            {
                                id = 0x01,
                                name = name,
                                payload = Lpayload,
                                Parent = CNode,
                            });
                        }
                        break;
                    case 0x02:
                        {
                            short Lpayload = (short)((bytes[i + Psize + namesize + 1] << 8) | bytes[i + Psize + namesize + 2]);
                            i = i + Psize + namesize + 2;
                            CNode.payload.Add(new NBT_item
                            {
                                id = 0x02,
                                name = name,
                                payload = Lpayload,
                                Parent = CNode,
                            });
                        }
                        break;
                    case 0x03:
                        {
                            int Lpayload = (bytes[i + Psize + 1 + namesize] << 24)
                                         | (bytes[i + Psize + 1 + namesize + 1] << 16)
                                         | (bytes[i + Psize + 1 + namesize + 2] << 8)
                                         | bytes[i + Psize + 1 + namesize + 3];
                            i = i + Psize + namesize + 4;
                            CNode.payload.Add(new NBT_item
                            {
                                id = 0x03,
                                name = name,
                                payload = Lpayload,
                                Parent = CNode,
                            });
                        }
                        break;
                    case 0x04:
                        {
                            long Lpayload = ((long)bytes[i + Psize + 1 + namesize] << 56)
                                          | ((long)bytes[i + Psize + 1 + namesize + 1] << 48)
                                          | ((long)bytes[i + Psize + 1 + namesize + 2] << 40)
                                          | ((long)bytes[i + Psize + 1 + namesize + 3] << 32)
                                          | ((long)bytes[i + Psize + 1 + namesize + 4] << 24)
                                          | ((long)bytes[i + Psize + 1 + namesize + 5] << 16)
                                          | ((long)bytes[i + Psize + 1 + namesize + 6] << 8)
                                          | bytes[i + Psize + 1 + namesize + 7];
                            i = i + Psize + namesize + 8;
                            CNode.payload.Add(new NBT_item
                            {
                                id = 0x04,
                                name = name,
                                payload = Lpayload,
                                Parent = CNode,
                            });
                        }
                        break;
                    case 0x05:
                        {
                            int intBits = (bytes[i + Psize + 1 + namesize] << 24)
                                        | (bytes[i + Psize + 1 + namesize + 1] << 16)
                                        | (bytes[i + Psize + 1 + namesize + 2] << 8)
                                        | bytes[i + Psize + 1 + namesize + 3];
                            float Lpayload = BitConverter.Int32BitsToSingle(intBits);
                            i = i + Psize + namesize + 4;
                            CNode.payload.Add(new NBT_item
                            {
                                id = 0x05,
                                name = name,
                                payload = Lpayload,
                                Parent = CNode,
                            });
                        }
                        break;
                    case 0x06:
                        {
                            long longBits = ((long)bytes[i + Psize + 1 + namesize] << 56)
                                          | ((long)bytes[i + Psize + 1 + namesize + 1] << 48)
                                          | ((long)bytes[i + Psize + 1 + namesize + 2] << 40)
                                          | ((long)bytes[i + Psize + 1 + namesize + 3] << 32)
                                          | ((long)bytes[i + Psize + 1 + namesize + 4] << 24)
                                          | ((long)bytes[i + Psize + 1 + namesize + 5] << 16)
                                          | ((long)bytes[i + Psize + 1 + namesize + 6] << 8)
                                          | bytes[i + Psize + 1 + namesize + 7];
                            double Lpayload = BitConverter.Int64BitsToDouble(longBits);
                            i = i + Psize + namesize + 8;
                            CNode.payload.Add(new NBT_item
                            {
                                id = 0x06,
                                name = name,
                                payload = Lpayload,
                                Parent = CNode,
                            });
                        }
                        break;
                    case 0x07:
                        {
                            int Lpayloadsize = (bytes[i + Psize + 1 + namesize] << 24)
                                             | (bytes[i + Psize + 1 + namesize + 1] << 16)
                                             | (bytes[i + Psize + 1 + namesize + 2] << 8)
                                             | bytes[i + Psize + 1 + namesize + 3];
                            List<byte> Lpayload = bytes.GetRange(i + Psize + 1 + namesize + 4, Lpayloadsize);
                            i = i + Psize + namesize + 4 + Lpayloadsize;
                            CNode.payload.Add(new NBT_item
                            {
                                id = 0x07,
                                name = name,
                                payload = Lpayload,
                                Parent = CNode,
                            });
                        }
                        break;
                    case 0x08:
                        {
                            ushort Lpayloadsize = (ushort)((bytes[i + Psize + 1 + namesize] << 8) | bytes[i + Psize + 1 + namesize + 1]);
                            string Lpayload = DisModified_UTF_8(bytes.GetRange(i + Psize + 1 + namesize + 2, Lpayloadsize));
                            i = i + Psize + namesize + 2 + Lpayloadsize;
                            CNode.payload.Add(new NBT_item
                            {
                                id = 0x08,
                                name = name,
                                payload = Lpayload,
                                Parent = CNode,
                            });
                        }
                        break;
                    case 0x09:
                        {
                            byte Lid9 = bytes[i + Psize + 1 + namesize];
                            int Lpayloadsize = (bytes[i + Psize + 1 + namesize + 1] << 24)
                                             | (bytes[i + Psize + 1 + namesize + 2] << 16)
                                             | (bytes[i + Psize + 1 + namesize + 3] << 8)
                                             | bytes[i + Psize + 1 + namesize + 4];
                            i = i + Psize + namesize + 1 + 4;
                            var Lnode = new NBT_Set
                            {
                                id = 0x09,
                                name = name,
                                payload = new List<object>(),
                                Parent = CNode,
                            };
                            CNode.payload.Add(Lnode);
                            CNode = Lnode;
                            CidSize.Push(new (Lid9, Lpayloadsize));
                            //Cid = Lid9;
                            //Cidsize = Lpayloadsize;
                        }
                        break;
                    case 0x0A:
                        {
                            if (i == 0)
                            {
                                root = new NBT_Set()
                                {
                                    id = 0x0A,
                                    name = name,
                                    payload = new List<object>()
                                };
                                CNode = root;
                                continue;
                            }
                            i = i + Psize + namesize;
                            var Lnode = new NBT_Set
                            {
                                id = 0x0A,
                                name = name,
                                payload = new List<object>(),
                                Parent = CNode,
                            };
                            CNode.payload.Add(Lnode);
                            CNode = Lnode;
                        }
                        break;
                    case 0x0B:
                        {
                            int Lpayloadsize = (bytes[i + Psize + 1 + namesize] << 24)
                                             | (bytes[i + Psize + 1 + namesize + 1] << 16)
                                             | (bytes[i + Psize + 1 + namesize + 2] << 8)
                                             | bytes[i + Psize + 1 + namesize + 3];
                            List<int> Lpayload = new List<int>();
                            for (int j = 0; j < Lpayloadsize; j++)
                            {
                                int value = (bytes[i + Psize + 1 + namesize + 4 + j * 4] << 24)
                                          | (bytes[i + Psize + 1 + namesize + 4 + j * 4 + 1] << 16)
                                          | (bytes[i + Psize + 1 + namesize + 4 + j * 4 + 2] << 8)
                                          | bytes[i + Psize + 1 + namesize + 4 + j * 4 + 3];
                                Lpayload.Add(value);
                            }
                            i = i + Psize + namesize + 4 + Lpayloadsize * 4;
                            CNode.payload.Add(new NBT_item
                            {
                                id = 0x0B,
                                name = name,
                                payload = Lpayload,
                                Parent = CNode,
                            });
                        }
                        break;
                    case 0x0C:
                        {
                            int Lpayloadsize = (bytes[i + Psize + 1 + namesize] << 24)
                                             | (bytes[i + Psize + 1 + namesize + 1] << 16)
                                             | (bytes[i + Psize + 1 + namesize + 2] << 8)
                                             | bytes[i + Psize + 1 + namesize + 3];
                            List<long> Lpayload = new List<long>();
                            for (int j = 0; j < Lpayloadsize; j++)
                            {
                                long value = ((long)bytes[i + Psize + 1 + namesize + 4 + j * 8] << 56)
                                           | ((long)bytes[i + Psize + 1 + namesize + 4 + j * 8 + 1] << 48)
                                           | ((long)bytes[i + Psize + 1 + namesize + 4 + j * 8 + 2] << 40)
                                           | ((long)bytes[i + Psize + 1 + namesize + 4 + j * 8 + 3] << 32)
                                           | ((long)bytes[i + Psize + 1 + namesize + 4 + j * 8 + 4] << 24)
                                           | ((long)bytes[i + Psize + 1 + namesize + 4 + j * 8 + 5] << 16)
                                           | ((long)bytes[i + Psize + 1 + namesize + 4 + j * 8 + 6] << 8)
                                           | bytes[i + Psize + 1 + namesize + 4 + j * 8 + 7];
                                Lpayload.Add(value);
                            }
                            i = i + Psize + namesize + 4 + Lpayloadsize * 8;
                            CNode.payload.Add(new NBT_item
                            {
                                id = 0x0C,
                                name = name,
                                payload = Lpayload,
                                Parent = CNode,
                            });
                        }
                        break;
                    default:
                        {
                            throw new Exception("未知类型");
                        }
                        break;
                }
            }
        }
        /// <summary>
        /// 解析修改过的UTF-8,也就是java特殊的Modified_UTF_8
        /// </summary>
        /// <param name="bytes">byte数组</param>
        /// <returns>字符串</returns>
        public string DisModified_UTF_8(List<byte> bytes)
        {
            if (bytes == null)
            {
                return "";
            }

            List<byte> decodedBytes = new List<byte>();
            int i = 0;

            while (i < bytes.Count)
            {
                if (i + 1 < bytes.Count &&
                    bytes[i] == 0xC0 &&
                    bytes[i + 1] == 0x80)
                {
                    // 遇到 C0 80，替换为 0x00
                    decodedBytes.Add(0x00);
                    i += 2;
                }
                else
                {
                    // 其他字节保持不变
                    decodedBytes.Add(bytes[i]);
                    i += 1;
                }
            }

            // 使用标准 UTF-8 解码为字符串
            return Encoding.UTF8.GetString(decodedBytes.ToArray());
        }
    }
    public class NBT_item
    {
        public byte id { get; set; }
        public string name { get; set; }
        public object payload { get; set; }
        public NBT_Set Parent { get; set; }
    }
    public class NBT_Set
    {
        public byte id { get; set; }
        public string name { get; set; }
        public List<object> payload { get; set; }
        public NBT_Set Parent { get; set; }
    }

    // nbt 结构
    // id 名称 负载 
    // id 名称长度 名称 负载
}
