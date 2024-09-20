using System;
using System.Collections;
using System.Collections.Generic;

namespace SML.Common
{
    public struct BitArray : IEnumerable<bool>
    {
        public int Capacity { get; private set; }

        private int[] _bools;
        public BitArray(int capacity)
        {
            Capacity = capacity;
            _bools = new int[(capacity + 1) / sizeof(int)];
        }
        public bool this[int index]
        {
            get
            {
                return index < 0 || index > Capacity
                    ? throw new IndexOutOfRangeException()
                    : (_bools[index / sizeof(int)] & (1 << (index % sizeof(int)))) != 0;
            }
            set
            {
                if (index < 0 || index > Capacity)
                {
                    throw new IndexOutOfRangeException();
                }
                if (value)
                {
                    _bools[index / sizeof(int)] |= 1 << (index % sizeof(int));
                }
                else
                {
                    _bools[index / sizeof(int)] &= ~(1 << (index % sizeof(int)));
                }
            }
        }
        public int FindFirstFalseIndex(int start = 0)
        {
            for (int i = start; i < Capacity; i++)
            {
                if (_bools[i] == -1)
                {
                    i += sizeof(int) - 1;
                    continue;
                }
                if (!this[i])
                {
                    return i;
                }
            }
            return -1;
        }
        public int FindFirstTrueIndex(int start = 0)
        {
            for (int i = start; i < Capacity; i++)
            {
                if (_bools[i] == 0)
                {
                    i += sizeof(int) - 1;
                    continue;
                }
                if (this[i])
                {
                    return i;
                }
            }
            return -1;
        }
        public IEnumerator<bool> GetEnumerator()
        {
            for (int i = 0; i < Capacity; i++)
            {
                yield return this[i];
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public void CopyTo(bool[] array, int arrayIndex)
        {
            ArgumentNullException.ThrowIfNull(array);
            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }
            if (array.Length - arrayIndex < Capacity)
            {
                throw new ArgumentException("The destination array is not large enough to hold all the elements.");
            }
            for (int i = 0; i < Capacity; i++)
            {
                array[i + arrayIndex] = this[i];
            }
        }
        public void Resize(int newCapacity)
        {
            if (newCapacity < 0)
            {
                throw new ArgumentException("New capacity cannot be negative.");
            }
            int[] newBools = new int[(newCapacity + 1) / sizeof(int)];
            int copyLength = Math.Min(_bools.Length, newBools.Length);
            Array.Copy(_bools, newBools, copyLength);
            _bools = newBools;
            Capacity = newCapacity;
        }
    }
}