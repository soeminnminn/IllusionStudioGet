using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Illusion.Card
{
    public class BinaryList : IList<byte>
    {
        #region Variables
        private List<byte> list;
        private Encoding encoding;
        #endregion

        #region Constructor
        public BinaryList()
        {
            list = new List<byte>();
            encoding = new UTF8Encoding(false, true);
        }

        private BinaryList(byte[] value)
            : this()
        {
            this.list.AddRange(value);
        }
        #endregion

        #region Properties
        public byte this[int index] 
        {
            get => this.list[index];
            set
            {
                this.list[index] = value;
            }
        }

        public int Count => this.list.Count;

        public bool IsReadOnly => false;
        #endregion

        #region Methods
        public void Add(byte item)
        {
            this.list.Add(item);
        }

        public void Add(sbyte item)
        {
            this.list.Add((byte)item);
        }

        public void Add(bool item)
        {
            this.list.Add((byte)(item ? 1 : 0));
        }

        public void Add(char item)
        {
            byte[] bytes = encoding.GetBytes(new char[] { item });
            this.list.AddRange(bytes);
        }

        public void Add(short item)
        {
            byte[] buffer = new byte[2];
            buffer[0] = (byte)item;
            buffer[1] = (byte)(item >> 8);
            this.list.AddRange(buffer);
        }

        public void Add(int item)
        {
            byte[] buffer = new byte[4];
            buffer[0] = (byte)item;
            buffer[1] = (byte)(item >> 8);
            buffer[2] = (byte)(item >> 16);
            buffer[3] = (byte)(item >> 24);
            this.list.AddRange(buffer);
        }

        public void Add(long item)
        {
            byte[] buffer = new byte[8];
            buffer[0] = (byte)item;
            buffer[1] = (byte)(item >> 8);
            buffer[2] = (byte)(item >> 16);
            buffer[3] = (byte)(item >> 24);
            buffer[4] = (byte)(item >> 32);
            buffer[5] = (byte)(item >> 40);
            buffer[6] = (byte)(item >> 48);
            buffer[7] = (byte)(item >> 56);
            this.list.AddRange(buffer);
        }

        public void Add(ushort item)
        {
            byte[] buffer = new byte[2];
            buffer[0] = (byte)item;
            buffer[1] = (byte)(item >> 8);
            this.list.AddRange(buffer);
        }

        public void Add(uint item)
        {
            byte[] buffer = new byte[4];
            buffer[0] = (byte)item;
            buffer[1] = (byte)(item >> 8);
            buffer[2] = (byte)(item >> 16);
            buffer[3] = (byte)(item >> 24);
            this.list.AddRange(buffer);
        }

        public void Add(ulong item)
        {
            byte[] buffer = new byte[8];
            buffer[0] = (byte)item;
            buffer[1] = (byte)(item >> 8);
            buffer[2] = (byte)(item >> 16);
            buffer[3] = (byte)(item >> 24);
            buffer[4] = (byte)(item >> 32);
            buffer[5] = (byte)(item >> 40);
            buffer[6] = (byte)(item >> 48);
            buffer[7] = (byte)(item >> 56);
            this.list.AddRange(buffer);
        }

        public unsafe void Add(float item)
        {
            byte[] buffer = new byte[4];
            uint TmpValue = *(uint*)&item;

            buffer[0] = (byte)TmpValue;
            buffer[1] = (byte)(TmpValue >> 8);
            buffer[2] = (byte)(TmpValue >> 16);
            buffer[3] = (byte)(TmpValue >> 24);
            
            this.list.AddRange(buffer);
        }

        public unsafe void Add(double item)
        {
            byte[] buffer = new byte[8];
            ulong TmpValue = *(ulong*)&item;

            buffer[0] = (byte)TmpValue;
            buffer[1] = (byte)(TmpValue >> 8);
            buffer[2] = (byte)(TmpValue >> 16);
            buffer[3] = (byte)(TmpValue >> 24);
            buffer[4] = (byte)(TmpValue >> 32);
            buffer[5] = (byte)(TmpValue >> 40);
            buffer[6] = (byte)(TmpValue >> 48);
            buffer[7] = (byte)(TmpValue >> 56);

            this.list.AddRange(buffer);
        }

        public void Add(string item)
        {
            int len = encoding.GetByteCount(item);

            uint v = (uint)len;
            while (v >= 0x80)
            {
                this.list.Add((byte)(v | 0x80));
                v >>= 7;
            }
            this.list.Add((byte)v);
            this.list.AddRange(encoding.GetBytes(item));
        }

        public void Add(float r, float g, float b, float a)
        {
            this.list.AddRange(BitConverter.GetBytes(r));
            this.list.AddRange(BitConverter.GetBytes(g));
            this.list.AddRange(BitConverter.GetBytes(b));
            this.list.AddRange(BitConverter.GetBytes(a));
        }

        public void AddRange(IEnumerable<byte> collection)
        {
            this.list.AddRange(collection);
        }

        public void Clear()
        {
            this.list.Clear();
        }

        public bool Contains(byte item) => this.list.Contains(item);

        public void CopyTo(byte[] array, int arrayIndex)
        {
            this.list.CopyTo(array, arrayIndex);
        }

        public void CopyTo(int index, byte[] array, int arrayIndex, int count)
        {
            this.list.CopyTo(index, array, arrayIndex, count);
        }

        public void CopyTo(byte[] array)
        {
            this.list.CopyTo(array);
        }

        public void ForEach(Action<byte> action)
        {
            this.list.ForEach(action);
        }

        public IEnumerator<byte> GetEnumerator() => this.list.GetEnumerator();

        public List<byte> GetRange(int index, int count) => this.list.GetRange(index, count);

        public int IndexOf(byte item, int index, int count) => this.list.IndexOf(item, index, count);

        public int IndexOf(byte item, int index) => this.list.IndexOf(item, index);

        public int IndexOf(byte item) => this.list.IndexOf(item);

        public void Insert(int index, byte item)
        {
            this.list.Insert(index, item);
        }

        public void InsertRange(int index, IEnumerable<byte> collection)
        {
            this.list.InsertRange(index, collection);
        }

        public int LastIndexOf(byte item) => this.list.LastIndexOf(item);

        public int LastIndexOf(byte item, int index) => this.list.LastIndexOf(item, index);

        public int LastIndexOf(byte item, int index, int count) => this.list.LastIndexOf(item, index, count);

        public bool Remove(byte item) => this.list.Remove(item);

        public void RemoveAt(int index)
        {
            this.list.RemoveAt(index);
        }

        public void RemoveRange(int index, int count)
        {
            this.list.RemoveRange(index, count);
        }

        public void Reverse(int index, int count)
        {
            this.list.Reverse(index, count);
        }

        public void Reverse()
        {
            this.list.Reverse();
        }

        public void TrimExcess()
        {
            this.list.TrimExcess();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.list.GetEnumerator();

        public byte[] ToArray() => this.list.ToArray();

        public override string ToString()
        {
            byte[] ba = this.list.ToArray();
            string str = BitConverter.ToString(ba).Replace("-", ", 0x");
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            return "0x" + str;
        }
        #endregion

        #region Operators
        public static implicit operator BinaryList(byte[] value)
        {
            return new BinaryList(value);
        }

        public static explicit operator byte[](BinaryList value)
        {
            return value.ToArray();
        }

        public static implicit operator BinaryList(List<byte> value)
        {
            return new BinaryList(value.ToArray());
        }

        public static explicit operator List<byte>(BinaryList value)
        {
            return value.list;
        }
        #endregion
    }
}
