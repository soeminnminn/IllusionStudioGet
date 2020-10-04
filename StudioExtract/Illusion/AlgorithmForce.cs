using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Illusion
{
    /// <summary>
    /// Provides a set of extensions for searching specified collection in another collection.
    /// https://github.com/rvhuang/kmp-algorithm
    /// </summary>
    public static class KMPExtensions
    {
        #region TableBuilder
        private static IReadOnlyList<int> BuildTable<T>(IReadOnlyList<T> t, IEqualityComparer<T> comparer)
            where T : IEquatable<T>
        {
            var table = new int[t.Count];
            var i = 2;
            var j = 0;

            table[0] = -1;
            table[1] = 0;

            while (i < t.Count)
            {
                if (comparer.Equals(t[i - 1], t[j]))
                {
                    table[i] = j + 1;
                    i++;
                    j++;
                }
                else if (j > 0)
                {
                    j = table[j];
                }
                else
                {
                    table[i] = 0;
                    i++;
                }
            }
            return table;
        }
        #endregion

        #region IReadOnlyList(T) (IndexOf)

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified collection in this instance.
        /// </summary>
        /// <param name="s">The current collection.</param>
        /// <param name="t">The collection to seek.</param>
        /// <typeparam name="T">The type of element in the collection.</typeparam>
        /// <returns>
        /// The zero-based index position of value if <paramref name="t"/> is found, or -1 if it is not. 
        /// If <paramref name="t"/> is empty, the return value is 0.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        public static int IndexOf<T>(this IReadOnlyList<T> s, IReadOnlyList<T> t)
            where T : IEquatable<T>
        {
            return s.IndexOf(t, 0, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified collection in this instance
        /// and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="s">The current collection.</param>
        /// <param name="t">The collection to seek.</param>
        /// <param name="comparer">The specified <see cref="IEqualityComparer{T}"/> instance.</param>
        /// <typeparam name="T">The type of element in the collection.</typeparam>
        /// <returns>
        /// The zero-based index position of value if <paramref name="t"/> is found, or -1 if it is not. 
        /// If <paramref name="t"/> is empty, the return value is 0.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        public static int IndexOf<T>(this IReadOnlyList<T> s, IReadOnlyList<T> t, IEqualityComparer<T> comparer)
            where T : IEquatable<T>
        {
            return s.IndexOf(t, 0, comparer);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified collection in this instance.
        /// The search starts at a specified position.
        /// </summary>
        /// <param name="s">The current collection.</param>
        /// <param name="t">The collection to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <typeparam name="T">The type of element in the collection.</typeparam>
        /// <returns>
        /// The zero-based index position of value if <paramref name="t"/> is found, or -1 if it is not. 
        /// If <paramref name="t"/> is empty, the return value is 0.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero or greater than <see cref="IReadOnlyCollection{T}.Count">Count</see> of <paramref name="s"/>.
        /// </exception>
        public static int IndexOf<T>(this IReadOnlyList<T> s, IReadOnlyList<T> t, int startIndex)
            where T : IEquatable<T>
        {
            return s.IndexOf(t, startIndex, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified collection in this instance
        /// and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// The search starts at a specified position.
        /// </summary>
        /// <param name="s">The current collection.</param>
        /// <param name="t">The collection to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="comparer">The specified <see cref="IEqualityComparer{T}"/> instance.</param>
        /// <typeparam name="T">The type of element in the collection.</typeparam>
        /// <returns>
        /// The zero-based index position of value if <paramref name="t"/> is found, or -1 if it is not. 
        /// If <paramref name="t"/> is empty, the return value is 0.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero or greater than <see cref="IReadOnlyCollection{T}.Count">Count</see> of <paramref name="s"/>.
        /// </exception>
        public static int IndexOf<T>(this IReadOnlyList<T> s, IReadOnlyList<T> t, int startIndex, IEqualityComparer<T> comparer)
            where T : IEquatable<T>
        {
            Validate(s, t, startIndex);

            // Follow the behavior of string.IndexOf(string) method. 
            if (t.Count == 0) return 0;
            if (s.Count == 0 || s.Count < t.Count) return -1;

            if (comparer == null) comparer = EqualityComparer<T>.Default;
            if (t.Count == 1) return s.IndexOf(t[0], startIndex, comparer);

            var table = BuildTable(t, comparer);
            var i = 0;

            while (startIndex + i < s.Count)
            {
                if (comparer.Equals(t[i], s[startIndex + i]))
                {
                    if (i == t.Count - 1)
                        return startIndex;
                    i++;
                }
                else
                {
                    if (table[i] > -1)
                    {
                        startIndex += i;
                        i = table[i];
                    }
                    else
                    {
                        startIndex++;
                        i = 0;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified collection in this instance.
        /// </summary>
        /// <param name="s">The current collection.</param>
        /// <param name="t">The collection to seek.</param>
        /// <typeparam name="T">The type of element in the collection.</typeparam>
        /// <returns>
        /// The zero-based index position of value if <paramref name="t"/> is found, or -1 if it is not. 
        /// If <paramref name="t"/> is empty, the return value is 0.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        public static int IndexOf<T>(this IEnumerable<T> s, IReadOnlyList<T> t)
            where T : IEquatable<T>
        {
            return s.IndexOf(t, 0, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified collection in this instance
        /// and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="s">The current collection.</param>
        /// <param name="t">The collection to seek.</param>
        /// <param name="comparer">The specified <see cref="IEqualityComparer{T}"/> instance.</param>
        /// <typeparam name="T">The type of element in the collection.</typeparam>
        /// <returns>
        /// The zero-based index position of value if <paramref name="t"/> is found, or -1 if it is not. 
        /// If <paramref name="t"/> is empty, the return value is 0.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        public static int IndexOf<T>(this IEnumerable<T> s, IReadOnlyList<T> t, IEqualityComparer<T> comparer)
            where T : IEquatable<T>
        {
            return s.IndexOf(t, 0, comparer);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified collection in this instance.
        /// The search starts at a specified position.
        /// </summary>
        /// <param name="s">The current collection.</param>
        /// <param name="t">The collection to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <typeparam name="T">The type of element in the collection.</typeparam>
        /// <returns>
        /// The zero-based index position of value if <paramref name="t"/> is found, or -1 if it is not. 
        /// If <paramref name="t"/> is empty, the return value is 0.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero.
        /// </exception>
        public static int IndexOf<T>(this IEnumerable<T> s, IReadOnlyList<T> t, int startIndex)
            where T : IEquatable<T>
        {
            return s.IndexOf(t, startIndex, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified collection in this instance
        /// and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// The search starts at a specified position.
        /// </summary>
        /// <param name="s">The current collection.</param>
        /// <param name="t">The collection to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="comparer">The specified <see cref="IEqualityComparer{T}"/> instance.</param>
        /// <typeparam name="T">The type of element in the collection.</typeparam>
        /// <returns>
        /// The zero-based index position of value if <paramref name="t"/> is found, or -1 if it is not. 
        /// If <paramref name="t"/> is empty, the return value is 0.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero.
        /// </exception>
        public static int IndexOf<T>(this IEnumerable<T> s, IReadOnlyList<T> t, int startIndex, IEqualityComparer<T> comparer)
            where T : IEquatable<T>
        {
            Validate(s, t, startIndex);

            // Follow the behavior of string.IndexOf(string) method. 
            if (t.Count == 0) return 0;
            if (comparer == null) comparer = EqualityComparer<T>.Default;
            if (t.Count == 1) return s.IndexOf(t[0], startIndex, comparer);

            var table = BuildTable(t, comparer);
            var i = 0;
            var offset = startIndex + 1;

            using (var enumerator = s.GetEnumerator())
            {
                while (Skip(enumerator, offset) != null)
                {
                    if (comparer.Equals(t[i], enumerator.Current))
                    {
                        if (i == t.Count - 1)
                            return startIndex;
                        i++;
                    }
                    else
                    {
                        // We will extract this as a method GetOffset() in future version
                        // for upcoming APIs:
                        // 1. IndexOfAny(IReadOnlyList{T}, IEnumerable{IReadOnlyList{T}})
                        // 2. IndexesOfAll(IReadOnlyList{T}, IEnumerable{IReadOnlyList{T}}).
                        if (table[i] > -1)
                        {
                            startIndex += i;
                            offset = i;
                            i = table[i];
                        }
                        else
                        {
                            startIndex++;
                            offset = 1;
                            i = 0;
                        }
                    }
                }
            }
            return -1;
        }

        #endregion

        #region String (IndexOf)

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified character collection in this instance.
        /// </summary>
        /// <param name="s">The string instance.</param>
        /// <param name="t">The character collection to seek.</param>
        /// <returns>
        /// The zero-based index position of value if that string is found, or -1 if it is not. 
        /// If <paramref name="t"/> is empty, the return value is 0.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        public static int IndexOf(this string s, IReadOnlyList<char> t)
        {
            return s.AsReadOnlyList().IndexOf(t, 0, EqualityComparer<char>.Default);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified character collection in this instance
        /// and uses the specified <see cref="IEqualityComparer{Char}"/>.
        /// </summary>
        /// <param name="s">The string instance.</param>
        /// <param name="t">The character collection to seek.</param>
        /// <param name="comparer">The specified <see cref="IEqualityComparer{Char}"/> instance.</param>
        /// <returns>
        /// The zero-based index position of value if that string is found, or -1 if it is not. 
        /// If <paramref name="t"/> is empty, the return value is 0.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        public static int IndexOf(this string s, IReadOnlyList<char> t, EqualityComparer<char> comparer)
        {
            return s.AsReadOnlyList().IndexOf(t, 0, comparer);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified character collection in this instance. 
        /// The search starts at a specified character position.
        /// </summary>
        /// <param name="s">The string instance.</param>
        /// <param name="t">The character collection to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>
        /// The zero-based index position of value if that string is found, or -1 if it is not. 
        /// If <paramref name="t"/> is empty, the return value is 0.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than 0 or greater than the length of this string.</exception>
        public static int IndexOf(this string s, IReadOnlyList<char> t, int startIndex)
        {
            return s.AsReadOnlyList().IndexOf(t, startIndex, EqualityComparer<char>.Default);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified character collection in this instance 
        /// and uses the specified <see cref="IEqualityComparer{Char}"/>.
        /// The search starts at a specified character position.
        /// </summary>
        /// <param name="s">The string instance.</param>
        /// <param name="t">The character collection to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="comparer">The specified <see cref="IEqualityComparer{Char}"/> instance.</param>
        /// <returns>
        /// The zero-based index position of value if that string is found, or -1 if it is not. 
        /// If <paramref name="t"/> is empty, the return value is 0.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than 0 or greater than the length of this string.</exception>
        public static int IndexOf(this string s, IReadOnlyList<char> t, int startIndex, EqualityComparer<char> comparer)
        {
            return s.AsReadOnlyList().IndexOf(t, startIndex, comparer);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified collection in this instance.
        /// </summary>
        /// <param name="s">The current collection.</param>
        /// <param name="t">The collection to seek.</param>
        /// <returns>
        /// The zero-based index position of value if <paramref name="t"/> is found, or -1 if it is not. 
        /// If <paramref name="t"/> is empty, the return value is 0.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        public static int IndexOf(this IEnumerable<char> s, string t)
        {
            return s.IndexOf(t, 0, EqualityComparer<char>.Default);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified collection in this instance
        /// and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="s">The current collection.</param>
        /// <param name="t">The collection to seek.</param>
        /// <param name="comparer">The specified <see cref="IEqualityComparer{T}"/> instance.</param>
        /// <returns>
        /// The zero-based index position of value if <paramref name="t"/> is found, or -1 if it is not. 
        /// If <paramref name="t"/> is empty, the return value is 0.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        public static int IndexOf(this IEnumerable<char> s, string t, IEqualityComparer<char> comparer)
        {
            return s.IndexOf(t, 0, comparer);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified collection in this instance.
        /// The search starts at a specified position.
        /// </summary>
        /// <param name="s">The current collection.</param>
        /// <param name="t">The collection to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>
        /// The zero-based index position of value if <paramref name="t"/> is found, or -1 if it is not. 
        /// If <paramref name="t"/> is empty, the return value is 0.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero.
        /// </exception>
        public static int IndexOf(this IEnumerable<char> s, string t, int startIndex)
        {
            return s.IndexOf(t, startIndex, EqualityComparer<char>.Default);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string in this instance
        /// and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// The search starts at a specified position.
        /// </summary>
        /// <param name="s">The current collection.</param>
        /// <param name="t">The string to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="comparer">The specified <see cref="IEqualityComparer{T}"/> instance.</param>
        /// <returns>
        /// The zero-based index position of value if <paramref name="t"/> is found, or -1 if it is not. 
        /// If <paramref name="t"/> is empty, the return value is 0.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero.
        /// </exception>
        public static int IndexOf(this IEnumerable<char> s, string t, int startIndex, IEqualityComparer<char> comparer)
        {
            Validate(s, t, startIndex);

            return s.IndexOf(t.AsReadOnlyList(), startIndex, comparer);
        }

        #endregion

        #region IReadOnlyList(T) (LastIndexOf) 

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified collection in this instance.
        /// </summary>
        /// <param name="s">The current collection.</param>
        /// <param name="t">The collection to seek.</param>
        /// <typeparam name="T">The type of element in the collection.</typeparam>
        /// <returns>
        /// The zero-based index position of value if that <paramref name="t"/> is found, or -1 if it is not. 
        /// If <paramref name="t"/> is empty, the return value is the last index position in this instance.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        public static int LastIndexOf<T>(this IReadOnlyList<T> s, IReadOnlyList<T> t)
            where T : IEquatable<T>
        {
            return s.LastIndexOf(t, s == null ? -1 : s.Count - 1, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified collection in this instance
        /// and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="s">The current collection.</param>
        /// <param name="t">The collection to seek.</param>
        /// <param name="comparer">The specified <see cref="IEqualityComparer{T}"/> instance.</param>
        /// <typeparam name="T">The type of element in the collection.</typeparam>
        /// <returns>
        /// The zero-based index position of value if that string is found, or -1 if it is not. 
        /// If <paramref name="t"/> is empty, the return value is the last index position in this instance.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        public static int LastIndexOf<T>(this IReadOnlyList<T> s, IReadOnlyList<T> t, IEqualityComparer<T> comparer)
            where T : IEquatable<T>
        {
            return s.LastIndexOf(t, s == null ? -1 : s.Count - 1, comparer);
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified collection in this instance. 
        /// The search starts at a specified character position.
        /// </summary>
        /// <param name="s">The current collection.</param>
        /// <param name="t">The collection to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <typeparam name="T">The type of element in the collection.</typeparam>
        /// <returns>
        /// The zero-based index position of value if that string is found, or -1 if it is not. 
        /// If <paramref name="t"/> is empty, the return value is the last index position in this instance.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero or greater than <see cref="IReadOnlyCollection{T}.Count">Count</see> of <paramref name="s"/>.
        /// </exception>
        public static int LastIndexOf<T>(this IReadOnlyList<T> s, IReadOnlyList<T> t, int startIndex)
            where T : IEquatable<T>
        {
            return s.LastIndexOf(t, startIndex, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified collection in this instance 
        /// and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// The search starts at a specified character position.
        /// </summary>
        /// <param name="s">The current collection.</param>
        /// <param name="t">The collection to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="comparer">The specified <see cref="IEqualityComparer{T}"/> instance.</param>
        /// <typeparam name="T">The type of element in the collection.</typeparam>
        /// <returns>
        /// The zero-based index position of value if that string is found, or -1 if it is not. 
        /// If <paramref name="t"/> is empty, the return value is the last index position in this instance.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero or greater than <see cref="IReadOnlyCollection{T}.Count">Count</see> of <paramref name="s"/>.
        /// </exception>
        public static int LastIndexOf<T>(this IReadOnlyList<T> s, IReadOnlyList<T> t, int startIndex, IEqualityComparer<T> comparer)
            where T : IEquatable<T>
        {
            Validate(s, t, startIndex);

            // Follow the behavior of string.LastIndexOf(string) method. 
            if (t.Count == 0) return 0;
            if (s.Count == 0 || s.Count < t.Count) return -1;

            if (comparer == null) comparer = EqualityComparer<T>.Default;
            if (t.Count == 1) return LastIndexOf(s, t[0], startIndex, comparer);

            var table = BuildTable(t, comparer);
            var i = 0;

            while (startIndex - i >= 0)
            {
                if (comparer.Equals(t[t.Count - i - 1], s[startIndex - i]))
                {
                    if (i == t.Count - 1)
                        return startIndex - t.Count + 1;
                    i++;
                }
                else
                {
                    if (table[i] > -1)
                    {
                        startIndex -= i;
                        i = table[i];
                    }
                    else
                    {
                        startIndex--;
                        i = 0;
                    }
                }
            }
            return -1;
        }

        #endregion

        #region String (LastIndexOf)

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified character collection in this instance.
        /// </summary>
        /// <param name="s">The string instance.</param>
        /// <param name="t">The character collection to seek.</param>
        /// <returns>
        /// The zero-based index position of value if that string is found, or -1 if it is not. 
        /// If <paramref name="t"/> is empty, the return value is the last index position in this instance.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        public static int LastIndexOf(this string s, IReadOnlyList<char> t)
        {
            return s.AsReadOnlyList().LastIndexOf(t, s == null ? -1 : s.Length - 1, EqualityComparer<char>.Default);
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified character collection in this instance
        /// and uses the specified <see cref="IEqualityComparer{Char}"/>.
        /// </summary>
        /// <param name="s">The string instance.</param>
        /// <param name="t">The character collection to seek.</param>
        /// <param name="comparer">The specified <see cref="IEqualityComparer{Char}"/> instance.</param>
        /// <returns>
        /// The zero-based index position of value if that string is found, or -1 if it is not. 
        /// If <paramref name="t"/> is empty, the return value is the last index position in this instance.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        public static int LastIndexOf(this string s, IReadOnlyList<char> t, EqualityComparer<char> comparer)
        {
            return s.AsReadOnlyList().LastIndexOf(t, s == null ? -1 : s.Length - 1, comparer);
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified character collection in this instance. 
        /// The search starts at a specified character position.
        /// </summary>
        /// <param name="s">The string instance.</param>
        /// <param name="t">The character collection to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>
        /// The zero-based index position of value if that string is found, or -1 if it is not. 
        /// If <paramref name="t"/> is empty, the return value is the last index position in this instance.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than 0 or greater than the length of this string.</exception>
        public static int LastIndexOf(this string s, IReadOnlyList<char> t, int startIndex)
        {
            return s.AsReadOnlyList().LastIndexOf(t, startIndex, EqualityComparer<char>.Default);
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified character collection in this instance 
        /// and uses the specified <see cref="IEqualityComparer{Char}"/>.
        /// The search starts at a specified character position.
        /// </summary>
        /// <param name="s">The string instance.</param>
        /// <param name="t">The character collection to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="comparer">The specified <see cref="IEqualityComparer{Char}"/> instance.</param>
        /// <returns>
        /// The zero-based index position of value if that string is found, or -1 if it is not. 
        /// If <paramref name="t"/> is empty, the return value is the last index position in this instance.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than 0 or greater than the length of this string.</exception>
        public static int LastIndexOf(this string s, IReadOnlyList<char> t, int startIndex, EqualityComparer<char> comparer)
        {
            return s.AsReadOnlyList().LastIndexOf(t, startIndex, comparer);
        }

        #endregion

        #region IReadOnlyList(T) (IndexesOf)

        /// <summary>
        /// Enumerates each zero-based index of all occurrences of the specified collection in this instance.
        /// </summary>
        /// <param name="s">The current collection.</param>
        /// <param name="t">The collection to seek.</param>
        /// <typeparam name="T">The type of element in the collection.</typeparam>
        /// <returns>
        /// The zero-based index positions of value if one or more <paramref name="t"/> are found. 
        /// If <paramref name="t"/> is empty, no indexes will be enumerated.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        public static IEnumerable<int> IndexesOf<T>(this IReadOnlyList<T> s, IReadOnlyList<T> t)
            where T : IEquatable<T>
        {
            return s.IndexesOf(t, 0, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Enumerates each zero-based index of all occurrences of the specified collection in this instance
        /// and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="s">The current collection.</param>
        /// <param name="t">The collection to seek.</param>
        /// <param name="comparer">The specified <see cref="IEqualityComparer{T}"/> instance.</param>
        /// <typeparam name="T">The type of element in the collection.</typeparam>
        /// <returns>
        /// The zero-based index positions of value if one or more <paramref name="t"/> are found. 
        /// If <paramref name="t"/> is empty, no indexes will be enumerated.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        public static IEnumerable<int> IndexesOf<T>(this IReadOnlyList<T> s, IReadOnlyList<T> t, IEqualityComparer<T> comparer)
            where T : IEquatable<T>
        {
            return s.IndexesOf(t, 0, comparer);
        }

        /// <summary>
        /// Enumerates each zero-based index of all occurrences of the specified collection in this instance.
        /// The search starts at a specified position.
        /// </summary>
        /// <param name="s">The current collection.</param>
        /// <param name="t">The collection to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <typeparam name="T">The type of element in the collection.</typeparam>
        /// <returns>
        /// The zero-based index positions of value if one or more <paramref name="t"/> are found. 
        /// If <paramref name="t"/> is empty, no indexes will be enumerated.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero or greater than <see cref="IReadOnlyCollection{T}.Count">Count</see> of <paramref name="s"/>.
        /// </exception>
        public static IEnumerable<int> IndexesOf<T>(this IReadOnlyList<T> s, IReadOnlyList<T> t, int startIndex)
            where T : IEquatable<T>
        {
            return s.IndexesOf(t, startIndex, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Enumerates each zero-based index of all occurrences of the specified collection in this instance
        /// and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// The search starts at a specified position.
        /// </summary>
        /// <param name="s">The current collection.</param>
        /// <param name="t">The collection to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="comparer">The specified <see cref="IEqualityComparer{T}"/> instance.</param>
        /// <typeparam name="T">The type of element in the collection.</typeparam>
        /// <returns>
        /// The zero-based index positions of value if one or more <paramref name="t"/> are found. 
        /// If <paramref name="t"/> is empty, no indexes will be enumerated.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero or greater than <see cref="IReadOnlyCollection{T}.Count">Count</see> of <paramref name="s"/>.
        /// </exception>
        public static IEnumerable<int> IndexesOf<T>(this IReadOnlyList<T> s, IReadOnlyList<T> t, int startIndex, IEqualityComparer<T> comparer)
            where T : IEquatable<T>
        {
            Validate(s, t, startIndex);

            if (comparer == null) comparer = EqualityComparer<T>.Default;
            if (t.Count == 1)
                return EnumerateIndexes(s, t[0], startIndex, comparer);
            else
                return EnumerateIndexes(s, t, startIndex, comparer);
        }

        private static IEnumerable<int> EnumerateIndexes<T>(IReadOnlyList<T> s, IReadOnlyList<T> t, int startIndex, IEqualityComparer<T> comparer)
            where T : IEquatable<T>
        {
            var table = BuildTable(t, comparer);
            var i = 0;

            while (startIndex + i < s.Count)
            {
                if (comparer.Equals(t[i], s[startIndex + i]))
                {
                    if (i == t.Count - 1)
                    {
                        yield return startIndex;

                        startIndex++;
                        i = 0;
                    }
                    else
                    {
                        i++;
                    }
                }
                else
                {
                    if (table[i] > -1)
                    {
                        startIndex += i;
                        i = table[i];
                    }
                    else
                    {
                        startIndex++;
                        i = 0;
                    }
                }
            }
        }

        #endregion

        #region String (IndexesOf)

        /// <summary>
        /// Enumerates each zero-based index of all occurrences of the string in this instance.
        /// </summary>
        /// <param name="s">The string instance.</param>
        /// <param name="t">The character collection to seek.</param>
        /// <returns>
        /// The zero-based index positions of value if one or more <paramref name="t"/> are found. 
        /// If <paramref name="t"/> is empty, no indexes will be enumerated.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        public static IEnumerable<int> IndexesOf(this string s, IReadOnlyList<char> t)
        {
            return s.AsReadOnlyList().IndexesOf(t, 0, EqualityComparer<char>.Default);
        }

        /// <summary>
        /// Enumerates each zero-based index of all occurrences of the string in this instance
        /// and uses the specified <see cref="IEqualityComparer{Char}"/>.
        /// </summary>
        /// <param name="s">The string instance.</param>
        /// <param name="t">The character collection to seek.</param>
        /// <param name="comparer">The specified <see cref="IEqualityComparer{Char}"/> instance.</param>
        /// <returns>
        /// The zero-based index positions of value if one or more <paramref name="t"/> are found. 
        /// If <paramref name="t"/> is empty, no indexes will be enumerated.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        public static IEnumerable<int> IndexesOf(this string s, IReadOnlyList<char> t, IEqualityComparer<char> comparer)
        {
            return s.AsReadOnlyList().IndexesOf(t, 0, comparer);
        }

        /// <summary>
        /// Enumerates each zero-based index of all occurrences of the string in this instance.
        /// The search starts at a specified position.
        /// </summary>
        /// <param name="s">The string instance.</param>
        /// <param name="t">The character collection to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>
        /// The zero-based index positions of value if one or more <paramref name="t"/> are found. 
        /// If <paramref name="t"/> is empty, no indexes will be enumerated.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero or greater than <see cref="IReadOnlyCollection{T}.Count">Count</see> of <paramref name="s"/>.
        /// </exception>
        public static IEnumerable<int> IndexesOf(this string s, IReadOnlyList<char> t, int startIndex)
        {
            return s.AsReadOnlyList().IndexesOf(t, startIndex, EqualityComparer<char>.Default);
        }

        /// <summary>
        /// Enumerates each zero-based index of all occurrences of the string in this instance
        /// and uses the specified <see cref="IEqualityComparer{Char}"/>.
        /// The search starts at a specified position.
        /// </summary>
        /// <param name="s">The string instance.</param>
        /// <param name="t">The character collection to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="comparer">The specified <see cref="IEqualityComparer{Char}"/> instance.</param>
        /// <returns>
        /// The zero-based index positions of value if one or more <paramref name="t"/> are found. 
        /// If <paramref name="t"/> is empty, no indexes will be enumerated.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero or greater than <see cref="IReadOnlyCollection{T}.Count">Count</see> of <paramref name="s"/>.
        /// </exception>
        public static IEnumerable<int> IndexesOf(this string s, IReadOnlyList<char> t, int startIndex, IEqualityComparer<char> comparer)
        {
            return s.AsReadOnlyList().IndexesOf(t, startIndex, comparer);
        }

        #endregion

        #region IReadOnlyList(T) (LastIndexesOf)

        /// <summary>
        /// Reversely enumerates each zero-based index of all occurrences of the specified collection in this instance.
        /// </summary>
        /// <param name="s">The current collection.</param>
        /// <param name="t">The collection to seek.</param>
        /// <typeparam name="T">The type of element in the collection.</typeparam>
        /// <returns>
        /// The zero-based index positions of value in reverse order if one or more <paramref name="t"/> are found. 
        /// If <paramref name="t"/> is empty, no indexes will be enumerated.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        public static IEnumerable<int> LastIndexesOf<T>(this IReadOnlyList<T> s, IReadOnlyList<T> t)
            where T : IEquatable<T>
        {
            return s.LastIndexesOf(t, 0, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Reversely enumerates each zero-based index of all occurrences of the specified collection in this instance
        /// and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="s">The current collection.</param>
        /// <param name="t">The collection to seek.</param>
        /// <param name="comparer">The specified <see cref="IEqualityComparer{T}"/> instance.</param>
        /// <typeparam name="T">The type of element in the collection.</typeparam>
        /// <returns>
        /// The zero-based index positions of value in reverse order if one or more <paramref name="t"/> are found. 
        /// If <paramref name="t"/> is empty, no indexes will be enumerated.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        public static IEnumerable<int> LastIndexesOf<T>(this IReadOnlyList<T> s, IReadOnlyList<T> t, IEqualityComparer<T> comparer)
            where T : IEquatable<T>
        {
            return s.LastIndexesOf(t, 0, comparer);
        }

        /// <summary>
        /// Reversely enumerates each zero-based index of all occurrences of the specified collection in this instance.
        /// The search starts at a specified position.
        /// </summary>
        /// <param name="s">The current collection.</param>
        /// <param name="t">The collection to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <typeparam name="T">The type of element in the collection.</typeparam>
        /// <returns>
        /// The zero-based index positions of value in reverse order if one or more <paramref name="t"/> are found. 
        /// If <paramref name="t"/> is empty, no indexes will be enumerated.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero or greater than <see cref="IReadOnlyCollection{T}.Count">Count</see> of <paramref name="s"/>.
        /// </exception>
        public static IEnumerable<int> LastIndexesOf<T>(this IReadOnlyList<T> s, IReadOnlyList<T> t, int startIndex)
            where T : IEquatable<T>
        {
            return s.LastIndexesOf(t, startIndex, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Reversely enumerates each zero-based index of all occurrences of the specified collection in this instance
        /// and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// The search starts at a specified position.
        /// </summary>
        /// <param name="s">The current collection.</param>
        /// <param name="t">The collection to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="comparer">The specified <see cref="IEqualityComparer{T}"/> instance.</param>
        /// <typeparam name="T">The type of element in the collection.</typeparam>
        /// <returns>
        /// The zero-based index positions of value in reverse order if one or more <paramref name="t"/> are found. 
        /// If <paramref name="t"/> is empty, no indexes will be enumerated.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero or greater than <see cref="IReadOnlyCollection{T}.Count">Count</see> of <paramref name="s"/>.
        /// </exception>
        public static IEnumerable<int> LastIndexesOf<T>(this IReadOnlyList<T> s, IReadOnlyList<T> t, int startIndex, IEqualityComparer<T> comparer)
            where T : IEquatable<T>
        {
            Validate(s, t, startIndex);

            if (comparer == null) comparer = EqualityComparer<T>.Default;
            if (t.Count == 1)
                return EnumerateLastIndexes(s, t[0], startIndex, comparer);
            else
                return EnumerateLastIndexes(s, t, startIndex, comparer);
        }

        private static IEnumerable<int> EnumerateLastIndexes<T>(IReadOnlyList<T> s, IReadOnlyList<T> t, int startIndex, IEqualityComparer<T> comparer)
            where T : IEquatable<T>
        {
            var table = BuildTable(t, comparer);
            var i = 0;

            while (startIndex - i >= 0)
            {
                if (comparer.Equals(t[t.Count - i - 1], s[startIndex - i]))
                {
                    if (i == t.Count - 1)
                    {
                        yield return startIndex - t.Count + 1;

                        startIndex--;
                        i = 0;
                    }
                    else
                    {
                        i++;
                    }
                }
                else
                {
                    if (table[i] > -1)
                    {
                        startIndex -= i;
                        i = table[i];
                    }
                    else
                    {
                        startIndex--;
                        i = 0;
                    }
                }
            }
        }

        #endregion

        #region String (LastIndexesOf)

        /// <summary>
        /// Reversely enumerates each zero-based index of all occurrences of the string in this instance.
        /// </summary>
        /// <param name="s">The string instance.</param>
        /// <param name="t">The character collection to seek.</param>
        /// <returns>
        /// The zero-based index positions of value in reverse order if one or more <paramref name="t"/> are found. 
        /// If <paramref name="t"/> is empty, no indexes will be enumerated.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        public static IEnumerable<int> LastIndexesOf(this string s, IReadOnlyList<char> t)
        {
            return s.AsReadOnlyList().LastIndexesOf(t, s == null ? -1 : s.Length - 1, EqualityComparer<char>.Default);
        }

        /// <summary>
        /// Reversely enumerates each zero-based index of all occurrences of the string in this instance
        /// and uses the specified <see cref="IEqualityComparer{Char}"/>.
        /// </summary>
        /// <param name="s">The string instance.</param>
        /// <param name="t">The character collection to seek.</param>
        /// <param name="comparer">The specified <see cref="IEqualityComparer{Char}"/> instance.</param>
        /// <returns>
        /// The zero-based index positions of value in reverse order if one or more <paramref name="t"/> are found. 
        /// If <paramref name="t"/> is empty, no indexes will be enumerated.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        public static IEnumerable<int> LastIndexesOf(this string s, IReadOnlyList<char> t, IEqualityComparer<char> comparer)
        {
            return s.AsReadOnlyList().LastIndexesOf(t, s == null ? -1 : s.Length - 1, comparer);
        }

        /// <summary>
        /// Reversely enumerates each zero-based index of all occurrences of the string in this instance.
        /// The search starts at a specified position.
        /// </summary>
        /// <param name="s">The string instance.</param>
        /// <param name="t">The character collection to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>
        /// The zero-based index positions of value in reverse order if one or more <paramref name="t"/> are found. 
        /// If <paramref name="t"/> is empty, no indexes will be enumerated.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero or greater than <see cref="IReadOnlyCollection{T}.Count">Count</see> of <paramref name="s"/>.
        /// </exception>
        public static IEnumerable<int> LastIndexesOf(this string s, IReadOnlyList<char> t, int startIndex)
        {
            return s.AsReadOnlyList().LastIndexesOf(t, startIndex, EqualityComparer<char>.Default);
        }

        /// <summary>
        /// Reversely enumerates each zero-based index of all occurrences of the string in this instance
        /// and uses the specified <see cref="IEqualityComparer{Char}"/>.
        /// The search starts at a specified position.
        /// </summary>
        /// <param name="s">The string instance.</param>
        /// <param name="t">The character collection to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="comparer">The specified <see cref="IEqualityComparer{Char}"/> instance.</param>
        /// <returns>
        /// The zero-based index positions of value in reverse order if one or more <paramref name="t"/> are found. 
        /// If <paramref name="t"/> is empty, no indexes will be enumerated.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> or <paramref name="t"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero or greater than <see cref="IReadOnlyCollection{T}.Count">Count</see> of <paramref name="s"/>.
        /// </exception>
        public static IEnumerable<int> LastIndexesOf(this string s, IReadOnlyList<char> t, int startIndex, IEqualityComparer<char> comparer)
        {
            return s.AsReadOnlyList().LastIndexesOf(t, startIndex, comparer);
        }

        #endregion

        #region Wrapper

        /// <summary>
        /// Wrap a <see cref="IList{T}"/> instant as a read-only list.
        /// </summary>
        /// <typeparam name="T">The type of element in the collection.</typeparam>
        /// <param name="list">The instance to be wrapped.</param>
        /// <returns>A wrapped collection.</returns>
        public static IReadOnlyList<T> AsReadOnlyList<T>(this IList<T> list)
            where T : IEquatable<T>
        {
            return list == null ? default(IReadOnlyList<T>) : new ListWrapper<T>(list);
        }

        /// <summary>
        /// Wrap a string instance as a read-only character collection.
        /// </summary>
        /// <param name="str">The string to be wrapped.</param>
        /// <returns>A wrapped string.</returns>
        public static IReadOnlyList<char> AsReadOnlyList(this string str)
        {
            return str == null ? default(IReadOnlyList<char>) : new StringWrapper(str);
        }

        #endregion

        #region Others

        private static void Validate<T>(IReadOnlyList<T> s, IReadOnlyList<T> t, int startIndex)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            if (t == null) throw new ArgumentNullException(nameof(t));

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Value is less than zero.");

            if (startIndex >= s.Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Value is greater than the length of s.");
        }

        private static int IndexOf<T>(this IReadOnlyList<T> s, T t, int startIndex, IEqualityComparer<T> comparer)
            where T : IEquatable<T>
        {
            for (var i = startIndex; i < s.Count; i++)
            {
                if (comparer.Equals(s[i], t))
                    return i;
            }
            return -1;
        }

        private static int LastIndexOf<T>(IReadOnlyList<T> s, T t, int startIndex, IEqualityComparer<T> comparer)
            where T : IEquatable<T>
        {
            for (var i = startIndex; i >= 0; i--)
            {
                if (comparer.Equals(s[i], t))
                    return i;
            }
            return -1;
        }

        private static IEnumerable<int> EnumerateIndexes<T>(IReadOnlyList<T> s, T t, int startIndex, IEqualityComparer<T> comparer)
            where T : IEquatable<T>
        {
            for (var i = startIndex; i < s.Count; i++)
            {
                if (comparer.Equals(s[i], t))
                    yield return i;
            }
        }

        private static IEnumerable<int> EnumerateLastIndexes<T>(IReadOnlyList<T> s, T t, int startIndex, IEqualityComparer<T> comparer)
            where T : IEquatable<T>
        {
            for (var i = startIndex; i >= 0; i--)
            {
                if (comparer.Equals(s[i], t))
                    yield return i;
            }
        }

        private static int IndexOf<T>(this IEnumerable<T> s, T t, int startIndex, IEqualityComparer<T> comparer)
            where T : IEquatable<T>
        {
            var offset = startIndex + 1;

            using (var enumerator = s.GetEnumerator())
            {
                while (Skip(enumerator, offset) != null)
                {
                    if (comparer.Equals(t, enumerator.Current)) return startIndex;

                    startIndex++;
                    offset = 1;
                }
            }
            return -1;
        }

        private static IEnumerator<T> Skip<T>(IEnumerator<T> enumerator, int count)
        {
            var i = 0;

            do
            {
                if (enumerator.MoveNext())
                    i++;
                else
                    return null;
            }
            while (i < count);
            return enumerator;
        }

        private static void Validate<T>(IEnumerable<T> s, IEnumerable<T> t, int startIndex)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            if (t == null) throw new ArgumentNullException(nameof(t));

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Value is less than zero.");
        }

        #endregion
    }

    #region StringWrapper Class
    /// <summary>
    /// If one day string implements IReadOnlyList{T} we may dig out this class
    /// </summary>
    public class StringWrapper : IReadOnlyList<char>
    {
        #region Fields

        private readonly string _str;

        #endregion

        #region Properties

        char IReadOnlyList<char>.this[int index] => _str[index];

        int IReadOnlyCollection<char>.Count => _str.Length;

        #endregion

        #region Constructor

        public StringWrapper(string str)
        {
            _str = str;
        }

        #endregion

        #region Methods

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (_str as IEnumerable<char>).GetEnumerator();
        }

        IEnumerator<char> IEnumerable<char>.GetEnumerator()
        {
            return (_str as IEnumerable<char>).GetEnumerator();
        }

        #endregion

        #region Others

        public override string ToString()
        {
            return _str;
        }

        public override bool Equals(object obj)
        {
            if (obj is StringWrapper wrapper)
                return string.Equals(_str, wrapper._str);
            return false;
        }

        public override int GetHashCode()
        {
            return _str.GetHashCode();
        }

        #endregion
    }
    #endregion

    #region ListWrapper Class
    public class ListWrapper<T> : IReadOnlyList<T>
        where T : IEquatable<T>
    {
        #region Fields
        private readonly IList<T> _list;
        #endregion

        #region Properties
        public T this[int index] => _list[index];

        public int Count => _list.Count;

        public bool IsReadOnly => true;
        #endregion

        #region Constructor
        public ListWrapper(IList<T> list)
        {
            _list = list;
        }
        #endregion

        #region Others
        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
        #endregion
    }
    #endregion

    #region ReaderExtensions Class
    /// <summary>
    /// Provides a set of extension methods that wrap <see cref="TextReader" /> or <see cref="BinaryReader" />
    /// as <see cref="IEnumerable{T}" /> instance.
    /// </summary>
    public static class ReaderExtensions
    {
        /// <summary>
        /// Wraps <see cref="TextReader" /> as <see cref="IEnumerable{T}" /> instance.  
        /// </summary>
        /// <param name="reader">The instance of <see cref="TextReader" />.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> instance that enumerates <see cref="char"/> from the source.</returns>
        public static IEnumerable<char> AsCharEnumerable(this TextReader reader)
        {
            return EnumerateChars(reader.Read);
        }

        /// <summary>
        /// Wraps <see cref="TextReader" /> as <see cref="IEnumerable{T}" /> instance.  
        /// </summary>
        /// <param name="reader">The instance of <see cref="TextReader" />.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> instance that enumerates <see cref="string" /> from the source.</returns>
        public static IEnumerable<string> AsStringEnumerable(this TextReader reader)
        {
            return Enumerate(reader.ReadLine);
        }

        /// <summary>
        /// Wraps <see cref="BinaryReader" /> as <see cref="IEnumerable{T}" /> instance.  
        /// </summary>
        /// <param name="reader">The instance of <see cref="BinaryReader" />.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> instance that enumerates <see cref="char"/> from the source.</returns>
        public static IEnumerable<char> AsCharEnumerable(this BinaryReader reader)
        {
            return Enumerate(reader.ReadChar);
        }

        /// <summary>
        /// Wraps <see cref="BinaryReader" /> as <see cref="IEnumerable{T}" /> instance.  
        /// </summary>
        /// <param name="reader">The instance of <see cref="BinaryReader" />.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> instance that enumerates <see cref="byte"/> from the source.</returns>
        public static IEnumerable<byte> AsByteEnumerable(this BinaryReader reader)
        {
            return Enumerate(reader.ReadByte);
        }

        /// <summary>
        /// Wraps <see cref="BinaryReader" /> as <see cref="IEnumerable{T}" /> instance.  
        /// </summary>
        /// <param name="reader">The instance of <see cref="BinaryReader" />.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> instance that enumerates <see cref="short"/> from the source.</returns>
        public static IEnumerable<short> AsInt16Enumerable(this BinaryReader reader)
        {
            return Enumerate(reader.ReadInt16);
        }

        /// <summary>
        /// Wraps <see cref="BinaryReader" /> as <see cref="IEnumerable{T}" /> instance.  
        /// </summary>
        /// <param name="reader">The instance of <see cref="BinaryReader" />.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> instance that enumerates <see cref="int"/> from the source.</returns>
        public static IEnumerable<int> AsInt32Enumerable(this BinaryReader reader)
        {
            return Enumerate(reader.ReadInt32);
        }

        /// <summary>
        /// Wraps <see cref="BinaryReader" /> as <see cref="IEnumerable{T}" /> instance.  
        /// </summary>
        /// <param name="reader">The instance of <see cref="BinaryReader" />.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> instance that enumerates <see cref="long"/> from the source.</returns>        
        public static IEnumerable<long> AsInt64Enumerable(this BinaryReader reader)
        {
            return Enumerate(reader.ReadInt64);
        }

        /// <summary>
        /// Wraps <see cref="BinaryReader" /> as <see cref="IEnumerable{T}" /> instance.  
        /// </summary>
        /// <param name="reader">The instance of <see cref="BinaryReader" />.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> instance that enumerates <see cref="float"/> from the source.</returns>
        public static IEnumerable<float> AsSingleEnumerable(this BinaryReader reader)
        {
            return Enumerate(reader.ReadSingle);
        }

        /// <summary>
        /// Wraps <see cref="BinaryReader" /> as <see cref="IEnumerable{T}" /> instance.  
        /// </summary>
        /// <param name="reader">The instance of <see cref="BinaryReader" />.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> instance that enumerates <see cref="double"/> from the source.</returns>
        public static IEnumerable<double> AsDoubleEnumerable(this BinaryReader reader)
        {
            return Enumerate(reader.ReadDouble);
        }

        /// <summary>
        /// Wraps <see cref="BinaryReader" /> as <see cref="IEnumerable{T}" /> instance.  
        /// </summary>
        /// <param name="reader">The instance of <see cref="BinaryReader" />.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> instance that enumerates <see cref="decimal"/> from the source.</returns>
        public static IEnumerable<decimal> AsDecimalEnumerable(this BinaryReader reader)
        {
            return Enumerate(reader.ReadDecimal);
        }

        internal static IEnumerable<T> Enumerate<T>(Func<T> method) // We may need to customize this for different cases. 
        {
            var c = default(T);

            while (true)
            {
                try
                {
                    c = method();
                }
                catch (EndOfStreamException)
                {
                    yield break;
                }
                yield return c;
            }
        }

        internal static IEnumerable<char> EnumerateChars(Func<int> method)
        {
            var c = default(int);

            while (c != -1)
            {
                try
                {
                    c = method();
                }
                catch (EndOfStreamException)
                {
                    yield break;
                }
                yield return (char)c;
            }
        }
    }
    #endregion
}
