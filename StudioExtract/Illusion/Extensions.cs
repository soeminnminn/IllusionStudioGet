using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
#if CONSOLE
#else
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
#endif

namespace Illusion
{
    public static class Extensions
    {
        #region Methods
        /*
         * Copy from KKManager. Somethime wrong result position.
         */
        public static long FindPattern(this Stream stream, byte[] pattern)
        {
            const int bufferSize = 4096;
            long origPos = stream.Position;

            byte[] buffer = new byte[bufferSize];
            int read;

            byte scanByte = pattern[0];

            while ((read = stream.Read(buffer, 0, bufferSize)) > 0)
            {
                for (var i = 0; i < read; i++)
                {
                    if (buffer[i] != scanByte)
                        continue;

                    bool flag = true;

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
                        long result = (stream.Position + 1) - (bufferSize - i) - pattern.Length;
                        stream.Seek(origPos, SeekOrigin.Begin);
                        return result;
                    }
                }
            }
            return -1;
        }

        /*
         * Correct but slow.
         */
        public static long FindSequence(this Stream stream, byte[] byteSequence)
        {
            if (byteSequence.Length > stream.Length)
                return -1;

            int padLeftSequence(byte[] bytes, byte[] seqBytes) 
            {
                int i = 1;
                while (i < bytes.Length)
                {
                    int n = bytes.Length - i;
                    byte[] aux1 = new byte[n];
                    byte[] aux2 = new byte[n];
                    Buffer.BlockCopy(bytes, i, aux1, 0, n);
                    Buffer.BlockCopy(seqBytes, 0, aux2, 0, n);
                    if (aux1.SequenceEqual(aux2))
                        return i;
                    i++;
                }
                return i;
            };

            byte[] buffer = new byte[byteSequence.Length];

            BufferedStream bufStream = new BufferedStream(stream, byteSequence.Length);
            while (bufStream.Read(buffer, 0, byteSequence.Length) == byteSequence.Length)
            {
                if (byteSequence.SequenceEqual(buffer))
                    return bufStream.Position - byteSequence.Length;
                else
                    bufStream.Position -= byteSequence.Length - padLeftSequence(buffer, byteSequence);
            }

            return -1;
        }

        public static long IndexOf(this byte[] haystack, byte[] needle, long startOffset = 0)
        {
            unsafe
            {
                fixed (byte* h = haystack) fixed (byte* n = needle)
                {
                    for (byte* hNext = h + startOffset, hEnd = h + haystack.LongLength + 1 - needle.LongLength, nEnd = n + needle.LongLength; hNext < hEnd; hNext++)
                        for (byte* hInc = hNext, nInc = n; *nInc == *hInc; hInc++)
                            if (++nInc == nEnd)
                                return hNext - h;
                    return -1;
                }
            }
        }

        public static long Seek(this BinaryReader reader, long offset, SeekOrigin origin) => reader.BaseStream.Seek(offset, origin);

        public static long Position(this BinaryReader reader) => reader.BaseStream.Position;

        public static long Length(this BinaryReader reader) => reader.BaseStream.Length;

        public static bool IsEOF(this BinaryReader reader) => (reader.BaseStream.Position >= reader.BaseStream.Length);

        public static byte[] ReadToEnd(this Stream stream)
        {
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

#if CONSOLE
#else
        public static Bitmap ResizeBitmap(this Image image, int width, int height)
        {
            int sourceWidth = image.Width;
            int sourceHeight = image.Height;
            int destX = 0;
            int destY = 0;

            float nPercent;

            float nPercentW = ((float)width / (float)sourceWidth);
            float nPercentH = ((float)height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = Convert.ToInt16((width -
                            (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = Convert.ToInt16((height -
                            (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            using (Graphics graphic = Graphics.FromImage(bmPhoto))
            {
                graphic.Clear(Color.White);

                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = SmoothingMode.HighQuality;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.CompositingQuality = CompositingQuality.HighQuality;

                graphic.DrawImage(image,
                    new Rectangle(destX, destY, destWidth, destHeight),
                    new Rectangle(0, 0, sourceWidth, sourceHeight),
                    GraphicsUnit.Pixel);
            }

            return bmPhoto;
        }
#endif
        #endregion
    }
}
