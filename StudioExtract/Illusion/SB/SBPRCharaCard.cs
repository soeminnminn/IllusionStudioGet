using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Illusion.Card
{
    public partial class SBPRCharaCard : ICharaCard
    {
        #region Variables
        public readonly string marker;
        #endregion

        #region Properties
        public string Game { get => "Sexy Beach PR"; }

        public int DataVersion { get; internal set; }

        public string Name { get => CustomData.name; }

        public int Sex { get; }

        public byte[] PngData { get; set; }

        public List<BlockHeader> BlockHeaders { get; }

        public Dictionary<string, byte[]> DataBlocks { get; internal set; }

        public CharaCustomData CustomData { get; }

        public string SourceFileName { get; }
        #endregion

        #region Constructor
        public SBPRCharaCard(string srcFileName, string marker, short sex)
        {
            this.SourceFileName = srcFileName;
            this.marker = marker;
            this.Sex = sex;

            this.BlockHeaders = new List<BlockHeader>();
            this.DataBlocks = new Dictionary<string, byte[]>();
            this.CustomData = new CharaCustomData();
        }
        #endregion

        #region Methods
        public string GenerateFileName()
        {
            throw new NotImplementedException();
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
                if (DataVersion > 1)
                {
                    return false;
                }

                int headerCount = reader.ReadInt32();
                for (int i = 0; i < headerCount; i++)
                {
                    var blockHeader = new BlockHeader();
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

                if (BlockHeaders.Count > 2)
                {
                    var customInfo = BlockHeaders[1];
                    if (customInfo.version <= 5)
                    {
                        var bytes = DataBlocks[customInfo.tagName];
                        if (bytes != null && bytes.Length > 0)
                        {
                            CustomData.Load(bytes, customInfo.version, (byte)Sex);
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

        public bool Save(Stream stream, SaveOptions options)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Nested Types
        public class BlockHeader
        {
            #region Variables
            internal const int tagSize = 128;
            #endregion

            #region Properties
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
