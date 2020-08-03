using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Illusion.Card
{
    public partial class HSCharaCard : ICharaCard
    {
        #region Variables
        public readonly string marker;
        #endregion

        #region Properties
        public string Game { get => "Honey Select"; }

        public int DataVersion { get; internal set; }

        public byte[] PngData { get; set; }

        public List<BlockHeader> BlockHeaders { get; }

        public Dictionary<string, byte[]> DataBlocks { get; internal set; }

        public string SourceFileName { get; }

        public string Name { get => PreviewInfo.name; }

        public int Sex { get; }

        public List<byte> ExtraBytes { get; private set; }

        public string ExtraXml { get; set; }

        public CharaInfoPreview PreviewInfo { get; private set; }

        #endregion

        #region Constructor
        public HSCharaCard(string srcFileName, string marker, short sex)
        {
            this.SourceFileName = srcFileName;
            this.marker = marker;
            this.Sex = sex;

            this.BlockHeaders = new List<BlockHeader>();
            this.DataBlocks = new Dictionary<string, byte[]>();
            this.ExtraBytes = new List<byte>();
            this.PreviewInfo = new CharaInfoPreview();
        }
        #endregion

        #region Methods
        private byte[] CreateSha256(byte[] data, string key)
        {
            return new HMACSHA256(Encoding.UTF8.GetBytes(key)).ComputeHash(data);
        }

        public bool Parse(BinaryReader reader, long pngEnd)
        {
            try
            {
                long position = 0L;

                if (pngEnd > 0)
                {
                    position = reader.BaseStream.Position;

                    reader.Seek(0, SeekOrigin.Begin);
                    PngData = reader.ReadBytes((int)pngEnd);
                    reader.Seek(position, SeekOrigin.Begin);
                }

                DataVersion = reader.ReadInt32();
                if (DataVersion > 2)
                {
                    return false;
                }

                int headerCount = reader.ReadInt32();
                for (int i = 0; i < headerCount; i++)
                {
                    var blockHeader = new HSCharaCard.BlockHeader();
                    blockHeader.Parse(reader);
                    BlockHeaders.Add(blockHeader);
                }

                position = reader.BaseStream.Position;
                foreach (var info in BlockHeaders)
                {
                    long seekPos = reader.Seek(position + info.pos, SeekOrigin.Begin);
                    if (seekPos < reader.BaseStream.Length)
                    {
                        var dataBytes = reader.ReadBytes((int)info.size);
                        DataBlocks.Add(info.tagName, dataBytes);
                    }
                }

                if (headerCount == 5)
                {
                    long offset = position + BlockHeaders[4].pos + BlockHeaders[4].size;
                    long seekPos = reader.Seek(offset, SeekOrigin.Begin);
                    if (DataVersion == 2)
                    {
                        var exBytes = reader.ReadBytes(32);
                        if (exBytes != null && exBytes.Length > 0)
                        {
                            ExtraBytes.AddRange(exBytes);
                        }
                    }

                    if (!reader.IsEOF())
                    {
                        var exBytes = reader.ReadBytes(16);
                        if (exBytes != null && exBytes.Length > 0)
                        {
                            ExtraBytes.AddRange(exBytes);
                        }
                    }

                    if (!reader.IsEOF())
                    {
                        var byteList = new List<byte>();
                        byte b;
                        while (!reader.IsEOF() && (b = reader.ReadByte()) != 0)
                        {
                            byteList.Add(b);
                        }
                        byteList.Add(0);

                        var exXml = Encoding.UTF8.GetString(byteList.ToArray());
                        if (!string.IsNullOrEmpty(exXml))
                        {
                            exXml = exXml.Trim('\0');

                            var regex = new Regex(@"(<charExtData[^>]*>(.|\s)+?<\/charExtData>)", RegexOptions.IgnoreCase);
                            var match = regex.Match(exXml);
                            if (match.Success)
                            {
                                ExtraXml = match.Groups[1].Value;
                            }
                        }
                    }

                    if (BlockHeaders.Count > 1)
                    {
                        var info = BlockHeaders[0];
                        var bytes = DataBlocks[info.tagName];
                        if (bytes != null && bytes.Length > 0)
                        {
                            PreviewInfo.Load(bytes, info.version);
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return false;
        }

        public BlockHeader FindHeader(string name)
        {
            return BlockHeaders.FirstOrDefault(x => x.tagName == name);
        }

        public string GenerateFileName()
        {
            string fileName = Sex == 0 ? "charaM_" : "charaF_";
            fileName += DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
            return fileName;
        }

        public bool Save(Stream stream, SaveOptions options)
        {
            if (stream == null || !stream.CanWrite)
            {
                return false;
            }

            try
            {
                byte[] pngData = this.PngData;
                if (pngData == null || pngData.Length == 0)
                {
                    var resName = (this.Sex == 0) ? "card_male.png" : "card_female.png";
                    using (var resStream = Assembly.GetExecutingAssembly().OpenManifestResourceStream(resName))
                    {
                        pngData = resStream.ReadToEnd();
                    }
                }

                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(pngData);

                    long position = writer.BaseStream.Position;
                    writer.Write(marker);
                    writer.Write(2); // DataVersion

                    writer.Write(BlockHeaders.Count);

                    long pos = 0;
                    List<byte[]> dataList = new List<byte[]>();
                    foreach (var info in BlockHeaders)
                    {
                        var key = info.tagName;
                        byte[] data = null;

                        if (DataBlocks.ContainsKey(key))
                            data = DataBlocks[key];

                        info.pos = pos;
                        info.size = (data != null) ? data.GetLength(0) : 0L;
                        pos += info.size;

                        byte[] nameBytes = new byte[BlockHeader.tagSize];
                        Array.Clear(nameBytes, 0, nameBytes.Length);

                        byte[] strBytes = Encoding.UTF8.GetBytes(info.tagName);
                        Buffer.BlockCopy(strBytes, 0, nameBytes, 0, strBytes.Length);

                        writer.Write(nameBytes);
                        writer.Write(info.version);
                        writer.Write(info.pos);
                        writer.Write(info.size);

                        if (data != null)
                            dataList.Add(data);
                    }

                    long headerSize = writer.BaseStream.Position - position;
                    foreach (var bytes in dataList)
                    {
                        writer.Write(bytes);
                    }

                    byte[] sha256 = CreateSha256(pngData, marker);
                    writer.Write(sha256);
                    writer.Write(position);
                    writer.Write(headerSize);

                    if (ExtraXml != null && !string.IsNullOrEmpty(ExtraXml))
                    {
                        writer.Write(ExtraXml);
                    }

                    writer.Flush();
                }

                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            return false;
        }
        #endregion

        #region Nested Types
        public class BlockHeader
        {
            #region Variables
            internal const int tagSize = 128;
            #endregion

            #region Properties
            /*
             * プレビュー情報		    - Preview information
             * カスタム情報		    - Custom information
             * コーディネート情報		- Coordination information
             * ステータス情報		    - Status information
             * パラメータ情報		    - Parameter information
             */
            public string tagName { get; set; }

            public int version { get; set; }

            public long pos { get; set; }

            public long size { get; set; }
            #endregion

            #region Constructor
            public BlockHeader()
            { }
            #endregion

            #region Methods
            public void Parse(BinaryReader reader)
            {
                byte[] nameBytes = reader.ReadBytes(tagSize);
                tagName = Encoding.UTF8.GetString(nameBytes).Trim('\0');
                version = reader.ReadInt32();
                pos = reader.ReadInt64();
                size = reader.ReadInt64();
            }

            public override string ToString() => $"{tagName} {{ pos: {pos}, size: {size} }}";
            #endregion
        }
        #endregion
    }
}
