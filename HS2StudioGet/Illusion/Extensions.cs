using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

namespace Illusion
{
    public static class Extensions
    {
        #region Methods
        public static long FindPattern(this BinaryReader reader, byte[] pattern)
        {
            var stream = reader.BaseStream;
            const int bufferSize = 4096;
            var origPos = stream.Position;

            var buffer = new byte[bufferSize];
            int read;

            var scanByte = pattern[0];

            while ((read = stream.Read(buffer, 0, bufferSize)) > 0)
            {
                for (var i = 0; i < read; i++)
                {
                    if (buffer[i] != scanByte)
                        continue;

                    var flag = true;

                    for (var x = 1; x < pattern.Length; x++)
                    {
                        i++;

                        if (i >= bufferSize)
                        {
                            if ((read = stream.Read(buffer, 0, bufferSize)) < bufferSize)
                                return -1;

                            i = 0;
                        }

                        if (buffer[i] != pattern[x])
                        {
                            flag = false;
                            break;
                        }
                    }

                    if (flag)
                    {
                        var result = (stream.Position + 1) - (bufferSize - i) - pattern.Length;
                        stream.Position = origPos;
                        return result;
                    }
                }
            }
            return -1;
        }

        public static long Seek(this BinaryReader reader, long offset, SeekOrigin origin) => reader.BaseStream.Seek(offset, origin);

        public static byte[] ReadToEnd(this Stream stream)
        {
            //byte[] temp = null;
            //using (var memoryStream = new MemoryStream())
            //{
            //    stream.CopyTo(memoryStream);
            //    var src = memoryStream.ToArray();
            //    temp = new byte[src.Length];
            //    Buffer.BlockCopy(src, 0, temp, 0, src.Length);
            //}
            //return temp;

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static string AssemblyDirectory(this Assembly assembly)
        {
#if CORE
            string codeBase = assembly.CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
#else
            return Path.GetDirectoryName(assembly.Location);
#endif
        }

        public static Stream OpenManifestResourceStream(this Assembly assembly, string name)
        {
            string resourcePath = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith("." + name));

            if (!string.IsNullOrEmpty(resourcePath))
            {
                return assembly.GetManifestResourceStream(resourcePath);
            }

            throw new KeyNotFoundException("`"+name+"` not found.");
        }
        #endregion
    }
}
