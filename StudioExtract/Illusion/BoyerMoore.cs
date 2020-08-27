using System;
using System.Collections.Generic;

namespace Illusion.Card
{
    // https://gist.github.com/mjs3339/0772431281093f1bca1fce2f2eca527d
    public class BoyerMoore
    {
        #region Variables
        private int[] _jumpTable;
        private byte[] _pattern;
        private int _patternLength;
        #endregion

        #region Constructor
        public BoyerMoore()
        {
        }

        public BoyerMoore(byte[] pattern)
        {
            _pattern = pattern;
            _jumpTable = new int[256];
            _patternLength = _pattern.Length;

            for (var index = 0; index < 256; index++)
                _jumpTable[index] = _patternLength;

            for (var index = 0; index < _patternLength - 1; index++)
                _jumpTable[_pattern[index]] = _patternLength - index - 1;
        }
        #endregion

        #region Methods
        public void SetPattern(byte[] pattern)
        {
            _pattern = pattern;
            _jumpTable = new int[256];
            _patternLength = _pattern.Length;

            for (var index = 0; index < 256; index++)
                _jumpTable[index] = _patternLength;

            for (var index = 0; index < _patternLength - 1; index++)
                _jumpTable[_pattern[index]] = _patternLength - index - 1;
        }

        public unsafe long Search(byte[] searchArray, long startIndex = 0)
        {
            if (_pattern == null)
                throw new Exception("Pattern has not been set.");

            if (_patternLength > searchArray.Length)
                throw new Exception("Search Pattern length exceeds search array length.");

            long index = startIndex;
            int limit = searchArray.Length - _patternLength;
            int patternLengthMinusOne = _patternLength - 1;

            fixed (byte* pointerToByteArray = searchArray)
            {
                byte* pointerToByteArrayStartingIndex = pointerToByteArray + startIndex;
                fixed (byte* pointerToPattern = _pattern)
                {
                    while (index <= limit)
                    {
                        int j = patternLengthMinusOne;
                        while (j >= 0 && pointerToPattern[j] == pointerToByteArrayStartingIndex[index + j])
                            j--;
                        if (j < 0)
                            return index;
                        index += Math.Max(_jumpTable[pointerToByteArrayStartingIndex[index + j]] - _patternLength + 1 + j, 1);
                    }
                }
            }

            return -1;
        }

        public unsafe List<long> SearchAll(byte[] searchArray, long startIndex = 0)
        {
            long index = startIndex;
            int limit = searchArray.Length - _patternLength;
            int patternLengthMinusOne = _patternLength - 1;
            List<long> list = new List<long>();

            fixed (byte* pointerToByteArray = searchArray)
            {
                byte* pointerToByteArrayStartingIndex = pointerToByteArray + startIndex;
                fixed (byte* pointerToPattern = _pattern)
                {
                    while (index <= limit)
                    {
                        int j = patternLengthMinusOne;
                        while (j >= 0 && pointerToPattern[j] == pointerToByteArrayStartingIndex[index + j])
                            j--;

                        if (j < 0)
                            list.Add(index);

                        index += Math.Max(_jumpTable[pointerToByteArrayStartingIndex[index + j]] - _patternLength + 1 + j, 1);
                    }
                }
            }
            return list;
        }

        public long SuperSearch(byte[] searchArray, int nth, long start = 0)
        {
            long e = start;
            long c = 0;

            do
            {
                e = Search(searchArray, e);
                if (e == -1)
                    return -1;

                c++;
                e++;
            } while (c < nth);

            return e - 1;
        }
        #endregion
    }
}
