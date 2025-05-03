using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMCMLCore.CORE.Model
{
    /// <summary>
    /// 环形缓冲数据区
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MRingBuffer<T>
    {
        private readonly T[] _buffer;
        private int _head;       // 写入位置
        private int _tail;       // 读取位置
        private readonly object _lock = new object();

        public MRingBuffer(int capacity)
        {
            if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
            _buffer = new T[capacity];
            _head = 0;
            _tail = 0;
        }

        /// <summary>
        /// 添加元素，自动覆盖旧数据（当容量满时）。
        /// </summary>
        public void Add(T item)
        {
            lock (_lock)
            {
                // 写入新元素，自动覆盖旧数据
                _buffer[_head] = item;
                _head = (_head + 1) % _buffer.Length;

                // 如果写指针追上读指针，移动读指针（保持至少一个空位）
                if (_head == _tail)
                    _tail = (_tail + 1) % _buffer.Length;
            }
        }

        /// <summary>
        /// 读取元素（先进先出）。
        /// </summary>
        public bool TryRead(out T item)
        {
            lock (_lock)
            {
                if (_tail == _head)
                {
                    item = default;
                    return false;
                }

                item = _buffer[_tail];
                _tail = (_tail + 1) % _buffer.Length;
                return true;
            }
        }

        public int Capacity => _buffer.Length;
        public bool IsFull => (_head + 1) % _buffer.Length == _tail;
        public bool IsEmpty => _head == _tail;
    }
}
