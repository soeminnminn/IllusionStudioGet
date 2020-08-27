using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MessagePack;

namespace Illusion.Card
{
    public partial class AISCharaCard : ICharaCard
    {
        #region Variables
        public readonly string marker;
        #endregion

        #region Properties
        public string Game 
        { 
            get 
            {
                if (DataBlocks.ContainsKey("Parameter2") && DataBlocks.ContainsKey("GameInfo2"))
                    return "HS2";
                return "AIS"; 
            } 
        }

        public int DataVersion { get; }

        public string Version { get; private set; }

        public int Language { get; private set; }

        public string UserID { get; private set; }

        public string DataID { get; private set; }

        public byte[] PngData { get; set; }

        public BlockHeader BlocksInfo { get; private set; }

        public long DataSize { get; private set; }

        public Dictionary<string, byte[]> DataBlocks { get; private set; }

        public CharaParameter Parameter { get; private set; }

        public string SourceFileName { get; }

        public string Name 
        {
            get => Parameter.fullname;
        }

        public int Sex 
        {
            get => (int)Parameter.sex;
        }
        #endregion

        #region Constructor
        public AISCharaCard(string srcFileName, int productNo, string marker)
        {
            this.SourceFileName = srcFileName;
            this.DataVersion = productNo;
            this.marker = marker;

            this.DataBlocks = new Dictionary<string, byte[]>();
            
            this.Parameter = new CharaParameter()
            {
                sex = 0
            };
        }
        #endregion

        #region Methods
        public BlockHeader.Info SearchBlockInfo(string name)
        {
            return BlocksInfo.lstInfo.Find(x => x.name == name);
        }

        public void ParseBlock<T>(string blockName, Action<T> set)
        { 
            if (DataBlocks.ContainsKey(blockName))
            {
                var block = DataBlocks[blockName];
                if (block != null && block.Length > 0)
                {
                    var parameter = MessagePackSerializer.Deserialize<T>(block);
                    set(parameter);
                }
            }
        }

        public string GenerateFileName()
        {
            string fileName = Game;
            fileName += Sex == 0 ? "ChaM_" : "ChaF_";
            fileName += DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";

            return fileName;
        }

        public bool Parse(BinaryReader reader, long pngEnd)
        {
            try
            {
                if (pngEnd > 0)
                {
                    var position = reader.BaseStream.Position;

                    reader.Seek(0, SeekOrigin.Begin);
                    PngData = reader.ReadBytes((int)pngEnd);
                    reader.Seek(position, SeekOrigin.Begin);
                }

                Version = reader.ReadString();

                Language = reader.ReadInt32();
                UserID = reader.ReadString();
                DataID = reader.ReadString();

                var headerSize = reader.ReadInt32();
                var headerBytes = reader.ReadBytes(headerSize);

                var blockHeader = MessagePackSerializer.Deserialize<BlockHeader>(headerBytes);
                if (blockHeader != null)
                {
                    BlocksInfo = blockHeader;
                    DataSize = reader.ReadInt64();
                    var position = reader.BaseStream.Position;

                    foreach (var info in blockHeader.lstInfo)
                    {
                        long seekPos = reader.Seek(position + info.pos, SeekOrigin.Begin);
                        if (seekPos < reader.BaseStream.Length)
                        {
                            var dataBytes = reader.ReadBytes((int)info.size);

                            // var dataHex = "\"" + info.name + "\" : \"0x" + BitConverter.ToString(dataBytes).Replace("-", ", 0x") + "\"";
                            // System.Diagnostics.Debug.WriteLine(dataHex);

                            DataBlocks.Add(info.name, dataBytes);
                        }
                    }

                    ParseBlock<CharaParameter>(CharaParameter.BlockName, x => Parameter = x);
                    return true;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return false;
        }

        public bool Save(Stream stream, SaveOptions options)
        {
            if (stream == null || !stream.CanWrite)
            {
                return false;
            }

            try
            {
                var saveData = PrepareForSave();

                using(var writer = new BinaryWriter(stream))
                {
                    writer.Write(saveData.PngData);

                    writer.Write(saveData.DataVersion);
                    writer.Write(saveData.Marker);
                    writer.Write(saveData.Version);
                    writer.Write(saveData.Language);
                    writer.Write(saveData.UserID);
                    writer.Write(saveData.DataID);

                    writer.Write(saveData.InfoData.Length);
                    writer.Write(saveData.InfoData);

                    writer.Write((long)saveData.Data.Length);
                    writer.Write(saveData.Data);

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

        private SaveData PrepareForSave()
        { 
            var saveData = new SaveData()
            {
                Marker = marker,
                Version = Version,
                DataVersion = DataVersion,
                Language = Language,
                UserID = UserID,
                DataID = DataID,
                PngData = PngData,
            };

            if (saveData.PngData == null || saveData.PngData.Length == 0)
            {
                var resName = (this.Sex == 0) ? "card_male.png" : "card_female.png";
                using (var resStream = Assembly.GetExecutingAssembly().OpenManifestResourceStream(resName))
                {
                    saveData.PngData = resStream.ReadToEnd();
                }
            }

            var keyArr = new string[] { "Custom", "Coordinate", "Parameter", "GameInfo", "Status", "Parameter2", "GameInfo2" };
            var keyExtra = "KKEx";

            var lstInfo = new List<BlockHeader.Info>();
            var header = new BlockHeader();

            using (var memoryStream = new MemoryStream())
            {
                foreach (var key in keyArr)
                {
                    if (DataBlocks.ContainsKey(key))
                    {
                        BlockHeader.Info info = BlocksInfo.FindInfo(key);
                        if (info != null)
                        {
                            var data = DataBlocks[key];
                            if (data != null)
                            {
                                info.pos = memoryStream.Position;
                                info.size = data.Length;

                                memoryStream.Write(data, 0, data.Length);

                                lstInfo.Add(info);
                            }
                        }
                    }
                }

                var infoEx = BlocksInfo.FindInfo(keyExtra);
                if (infoEx != null)
                {
                    infoEx.pos = memoryStream.Position;
                    var data = DataBlocks[keyExtra];
                    if (data != null)
                    {
                        infoEx.size = data.Length;
                        memoryStream.Write(data, 0, data.Length);

                        header.lstInfo.Add(infoEx);
                    }
                }

                // Custom, Coordinate, Parameter, GameInfo, Status, Parameter2, GameInfo2, KKEx
                saveData.Data = memoryStream.ToArray();
            }

            header.lstInfo.AddRange(lstInfo);
            saveData.InfoData = MessagePackSerializer.Serialize(header);

            return saveData;
        }
        #endregion

        #region Nested Types
        [MessagePackObject(true)]
        public class BlockHeader
        {
            #region Properties
            public List<Info> lstInfo { get; set; } = new List<Info>();
            #endregion

            #region Methods
            public Info FindInfo(string name)
            {
                return lstInfo.FirstOrDefault(x => x.name == name);
            }
            #endregion

            #region Nested Types
            [MessagePackObject(true)]
            public class Info
            {
                #region Properties
                public string name { get; set; }

                public string version { get; set; }

                public long pos { get; set; }

                public long size { get; set; }
                #endregion

                #region Methods
                public override string ToString() => $"{name} {{ pos: {pos}, size: {size} }}";
                #endregion
            }
            #endregion
        }

        private class SaveData
        {
            #region Properties
            public byte[] PngData { get; internal set; }

            public int DataVersion { get; internal set; }

            public string Marker { get; internal set; }

            public string Version { get; internal set; }

            public int Language { get; internal set; }

            public string UserID { get; internal set; }

            public string DataID { get; internal set; }

            public byte[] InfoData { get; internal set; }

            public byte[] Data { get; internal set; }
            #endregion
        }
        #endregion
    }
}
