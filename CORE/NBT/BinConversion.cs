using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.NBT
{
    /// <summary>
    /// 字节转换，这个可以看名称知道意思所以就没有xml注释
    /// </summary>
    public class BinConversion
    {
        #region sbyte
        public static sbyte Bin_SByte(List<byte> bytes)
        {
            if (bytes == null || bytes.Count < 1)
                throw new ArgumentException("字节数组长度不足1字节");
            return (sbyte)bytes[0];
        }

        public static List<byte> SByte_Bin(sbyte value)
        {
            return new List<byte> { (byte)value };
        }
        #endregion

        #region byte
        public static byte Bin_Byte(List<byte> bytes)
        {
            if (bytes == null || bytes.Count < 1)
                throw new ArgumentException("字节数组长度不足1字节");
            return bytes[0];
        }

        public static List<byte> Byte_Bin(byte value)
        {
            return new List<byte> { value };
        }
        #endregion

        #region short
        public static short Bin_Short_Big(List<byte> bytes)
        {
            if (bytes == null || bytes.Count < 2)
                throw new ArgumentException("字节数组长度不足2字节");
            return (short)((bytes[0] << 8) | bytes[1]);
        }

        public static short Bin_Short_Little(List<byte> bytes)
        {
            if (bytes == null || bytes.Count < 2)
                throw new ArgumentException("字节数组长度不足2字节");
            return (short)((bytes[1] << 8) | bytes[0]);
        }

        public static List<byte> Short_Bin_Big(short value)
        {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)(value >> 8);
            bytes[1] = (byte)(value & 0xFF);
            return new List<byte>(bytes);
        }

        public static List<byte> Short_Bin_Little(short value)
        {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)(value & 0xFF);
            bytes[1] = (byte)(value >> 8);
            return new List<byte>(bytes);
        }
        #endregion

        #region ushort
        public static ushort Bin_UShort_Big(List<byte> bytes)
        {
            if (bytes == null || bytes.Count < 2)
                throw new ArgumentException("字节数组长度不足2字节");
            return (ushort)((bytes[0] << 8) | bytes[1]);
        }

        public static ushort Bin_UShort_Little(List<byte> bytes)
        {
            if (bytes == null || bytes.Count < 2)
                throw new ArgumentException("字节数组长度不足2字节");
            return (ushort)((bytes[1] << 8) | bytes[0]);
        }

        public static List<byte> UShort_Bin_Big(ushort value)
        {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)(value >> 8);
            bytes[1] = (byte)(value & 0xFF);
            return new List<byte>(bytes);
        }

        public static List<byte> UShort_Bin_Little(ushort value)
        {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)(value & 0xFF);
            bytes[1] = (byte)(value >> 8);
            return new List<byte>(bytes);
        }
        #endregion

        #region int
        public static int Bin_Int_Big(List<byte> bytes)
        {
            if (bytes == null || bytes.Count < 4)
                throw new ArgumentException("字节数组长度不足4字节");
            return (bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3];
        }

        public static int Bin_Int_Little(List<byte> bytes)
        {
            if (bytes == null || bytes.Count < 4)
                throw new ArgumentException("字节数组长度不足4字节");
            return (bytes[3] << 24) | (bytes[2] << 16) | (bytes[1] << 8) | bytes[0];
        }

        public static List<byte> Int_Bin_Big(int value)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(value >> 24);
            bytes[1] = (byte)(value >> 16);
            bytes[2] = (byte)(value >> 8);
            bytes[3] = (byte)(value & 0xFF);
            return new List<byte>(bytes);
        }

        public static List<byte> Int_Bin_Little(int value)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(value & 0xFF);
            bytes[1] = (byte)(value >> 8);
            bytes[2] = (byte)(value >> 16);
            bytes[3] = (byte)(value >> 24);
            return new List<byte>(bytes);
        }
        #endregion

        #region uint
        public static uint Bin_UInt_Big(List<byte> bytes)
        {
            if (bytes == null || bytes.Count < 4)
                throw new ArgumentException("字节数组长度不足4字节");
            return (uint)((bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3]);
        }

        public static uint Bin_UInt_Little(List<byte> bytes)
        {
            if (bytes == null || bytes.Count < 4)
                throw new ArgumentException("字节数组长度不足4字节");
            return (uint)((bytes[3] << 24) | (bytes[2] << 16) | (bytes[1] << 8) | bytes[0]);
        }

        public static List<byte> UInt_Bin_Big(uint value)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(value >> 24);
            bytes[1] = (byte)(value >> 16);
            bytes[2] = (byte)(value >> 8);
            bytes[3] = (byte)(value & 0xFF);
            return new List<byte>(bytes);
        }

        public static List<byte> UInt_Bin_Little(uint value)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(value & 0xFF);
            bytes[1] = (byte)(value >> 8);
            bytes[2] = (byte)(value >> 16);
            bytes[3] = (byte)(value >> 24);
            return new List<byte>(bytes);
        }
        #endregion

        #region long
        public static long Bin_Long_Big(List<byte> bytes)
        {
            if (bytes == null || bytes.Count < 8)
                throw new ArgumentException("字节数组长度不足8字节");
            return (long)(
                (long)bytes[0] << 56 |
                (long)bytes[1] << 48 |
                (long)bytes[2] << 40 |
                (long)bytes[3] << 32 |
                (long)bytes[4] << 24 |
                (long)bytes[5] << 16 |
                (long)bytes[6] << 8 |
                (long)bytes[7]
            );
        }

        public static long Bin_Long_Little(List<byte> bytes)
        {
            if (bytes == null || bytes.Count < 8)
                throw new ArgumentException("字节数组长度不足8字节");
            return (long)(
                (long)bytes[7] << 56 |
                (long)bytes[6] << 48 |
                (long)bytes[5] << 40 |
                (long)bytes[4] << 32 |
                (long)bytes[3] << 24 |
                (long)bytes[2] << 16 |
                (long)bytes[1] << 8 |
                (long)bytes[0]
            );
        }

        public static List<byte> Long_Bin_Big(long value)
        {
            byte[] bytes = new byte[8];
            for (int i = 0; i < 8; i++)
                bytes[i] = (byte)(value >> (56 - i * 8));
            return new List<byte>(bytes);
        }

        public static List<byte> Long_Bin_Little(long value)
        {
            byte[] bytes = new byte[8];
            for (int i = 0; i < 8; i++)
                bytes[i] = (byte)(value >> (i * 8));
            return new List<byte>(bytes);
        }
        #endregion

        #region ulong
        public static ulong Bin_ULong_Big(List<byte> bytes)
        {
            if (bytes == null || bytes.Count < 8)
                throw new ArgumentException("字节数组长度不足8字节");
            return (
                (ulong)bytes[0] << 56 |
                (ulong)bytes[1] << 48 |
                (ulong)bytes[2] << 40 |
                (ulong)bytes[3] << 32 |
                (ulong)bytes[4] << 24 |
                (ulong)bytes[5] << 16 |
                (ulong)bytes[6] << 8|
                (ulong)bytes[7]
            );
        }

        public static ulong Bin_ULong_Little(List<byte> bytes)
        {
            if (bytes == null || bytes.Count < 8)
                throw new ArgumentException("字节数组长度不足8字节");
            return (
                (ulong)bytes[7] << 56 |
                (ulong)bytes[6] << 48 |
                (ulong)bytes[5] << 40 |
                (ulong)bytes[4] << 32 |
                (ulong)bytes[3] << 24 |
                (ulong)bytes[2] << 16 |
                (ulong)bytes[1] << 8|
                (ulong)bytes[0]
            );
        }

        public static List<byte> ULong_Bin_Big(ulong value)
        {
            byte[] bytes = new byte[8];
            for (int i = 0; i < 8; i++)
                bytes[i] = (byte)(value >> (56 - i * 8));
            return new List<byte>(bytes);
        }

        public static List<byte> ULong_Bin_Little(ulong value)
        {
            byte[] bytes = new byte[8];
            for (int i = 0; i < 8; i++)
                bytes[i] = (byte)(value >> (i * 8));
            return new List<byte>(bytes);
        }
        #endregion

        #region char
        public static char Bin_Char_Big(List<byte> bytes)
        {
            if (bytes == null || bytes.Count < 2)
                throw new ArgumentException("字节数组长度不足2字节");
            return (char)((bytes[0] << 8) | bytes[1]);
        }

        public static char Bin_Char_Little(List<byte> bytes)
        {
            if (bytes == null || bytes.Count < 2)
                throw new ArgumentException("字节数组长度不足2字节");
            return (char)((bytes[1] << 8) | bytes[0]);
        }

        public static List<byte> Char_Bin_Big(char value)
        {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)(value >> 8);
            bytes[1] = (byte)(value & 0xFF);
            return new List<byte>(bytes);
        }

        public static List<byte> Char_Bin_Little(char value)
        {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)(value & 0xFF);
            bytes[1] = (byte)(value >> 8);
            return new List<byte>(bytes);
        }
        #endregion

        #region float
        public static float Bin_Float_Big(List<byte> bytes)
        {
            if (bytes == null || bytes.Count < 4)
                throw new ArgumentException("字节数组长度不足4字节");
            int i = Bin_Int_Big(bytes);
            return BitConverter.Int32BitsToSingle(i);
        }

        public static float Bin_Float_Little(List<byte> bytes)
        {
            if (bytes == null || bytes.Count < 4)
                throw new ArgumentException("字节数组长度不足4字节");
            int i = Bin_Int_Little(bytes);
            return BitConverter.Int32BitsToSingle(i);
        }

        public static List<byte> Float_Bin_Big(float value)
        {
            int i = BitConverter.SingleToInt32Bits(value);
            return Int_Bin_Big(i);
        }

        public static List<byte> Float_Bin_Little(float value)
        {
            int i = BitConverter.SingleToInt32Bits(value);
            return Int_Bin_Little(i);
        }
        #endregion

        #region double
        public static double Bin_Double_Big(List<byte> bytes)
        {
            if (bytes == null || bytes.Count < 8)
                throw new ArgumentException("字节数组长度不足8字节");
            long l = Bin_Long_Big(bytes);
            return BitConverter.Int64BitsToDouble(l);
        }

        public static double Bin_Double_Little(List<byte> bytes)
        {
            if (bytes == null || bytes.Count < 8)
                throw new ArgumentException("字节数组长度不足8字节");
            long l = Bin_Long_Little(bytes);
            return BitConverter.Int64BitsToDouble(l);
        }

        public static List<byte> Double_Bin_Big(double value)
        {
            long l = BitConverter.DoubleToInt64Bits(value);
            return Long_Bin_Big(l);
        }

        public static List<byte> Double_Bin_Little(double value)
        {
            long l = BitConverter.DoubleToInt64Bits(value);
            return Long_Bin_Little(l);
        }
        #endregion

        #region string Modified UTF-8 (java的特殊UTF8格式)
        public static string Bin_Modified_UTF_8(List<byte> bytes)
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
        public static List<byte> Modified_UTF_8_Bin(string value)
        {
            List<byte> name = new();
            var s= string.Empty;
            var sname = Encoding.UTF8.GetBytes(value);
            for (int i = 0; i < sname.Length; i++)
            {
                if (sname[i] == 0x00)
                {
                    name.Add(0xC0);
                    name.Add(0x80);
                }
                else
                {
                    name.Add(sname[i]);
                }
            }//转为Mutf8格式
            return  name;
        }
        #endregion
    }
}
