using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MessagePack;

namespace Illusion.Card
{
    public partial class AICharaCard : ICharaCard
    {
        #region Variables
        public const string marker = "【AIS_Chara】";

        public const string coordMarker = "【AIS_Clothes】";
        #endregion

        #region Properties
        public string Version { get; internal set; }

        public int ProductNo { get; internal set; }

        public int Language { get; internal set; }

        public string UserID { get; internal set; }

        public string DataID { get; internal set; }

        public byte[] PngData { get; set; }

        public long DataSize { get; internal set; }

        public BlockHeader BlocksInfo { get; internal set; }

        public Dictionary<string, byte[]> DataBlocks { get; internal set; }

        public CharaParameter Parameter { get; internal set; }

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
        public AICharaCard(string srcFileName)
        {
            this.SourceFileName = srcFileName;
            this.DataBlocks = new Dictionary<string, byte[]>();
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

        public string GenerateFileName(CardTypes cardType)
        {
            string fileName = null;
            switch (cardType)
            {
                case CardTypes.Coordinate:
                    fileName = Parameter.sex == AICharaCard.CharaSex.Male ? "HS2CoordM_" : "HS2CoordF_";
                    fileName += DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
                    break;
                case CardTypes.Charater:
                    fileName = Parameter.sex == AICharaCard.CharaSex.Male ? "HS2ChaM_" : "HS2ChaF_";
                    fileName += DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
                    break;
                default:
                    break;
            }

            return fileName;
        }

        public bool Save(Stream stream)
        {
            // Custom, Coordinate, Parameter, GameInfo, Status, Parameter2, GameInfo2, KKEx
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

                    writer.Write(saveData.ProductNo);
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

        public bool SaveCoordinate(Stream stream)
        {
            if (stream == null || !stream.CanWrite)
            {
                return false;
            }


            try
            {
                var info = BlocksInfo.FindInfo("Coordinate");
                if (info == null)
                {
                    return false;
                }

                var data = DataBlocks[info.name];
                if (data == null || data.Length == 0)
                {
                    return false;
                }

                using (var writer = new BinaryWriter(stream))
                {
                    // writer.Write(saveData.PngData);

                    writer.Write(ProductNo);
                    writer.Write(coordMarker);
                    writer.Write(info.version);
                    writer.Write(Language);
                    writer.Write(Parameter.fullname + "の服装");

                    writer.Write(data.Length);
                    writer.Write(data);

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
            if (Parameter == null)
            {
                Parameter = new CharaParameter()
                {
                    sex = CharaSex.Female
                };
            }

            var saveData = new SaveData()
            {
                Version = Version,
                ProductNo = ProductNo,
                Language = Language,
                UserID = UserID,
                DataID = DataID,
                PngData = PngData,
            };

            if (saveData.PngData == null || saveData.PngData.Length == 0)
            {
                var resName = (Parameter.sex == CharaSex.Male) ? "male.png" : "female.png";
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
                    BlockHeader.Info info = null;
                    if (DataBlocks.ContainsKey(key))
                    {
                        var data = DataBlocks[key];
                        if (data != null)
                        {
                            info = BlocksInfo.FindInfo(key);
                            info.pos = memoryStream.Position;
                            info.size = data.Length;

                            memoryStream.Write(data, 0, data.Length);
                        }
                    }

                    if (info == null)
                    {
                        var defData = (Parameter.sex == CharaSex.Male) ? DefaultDataMale.GetDataBlock(key) : DefaultDataFemale.GetDataBlock(key);
                        if (defData != null)
                        {
                            info = new BlockHeader.Info()
                            {
                                name = defData.name,
                                version = defData.version
                            };

                            info.pos = memoryStream.Position;
                            info.size = defData.data.Length;

                            memoryStream.Write(defData.data, 0, defData.data.Length);
                        }
                    }

                    if (info != null)
                    {
                        lstInfo.Add(info);
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
            #region Variables
            public string Marker = AICharaCard.marker;
            #endregion

            #region Properties
            public string Version { get; internal set; }

            public int ProductNo { get; internal set; }

            public int Language { get; internal set; }

            public string UserID { get; internal set; }

            public string DataID { get; internal set; }

            public byte[] PngData { get; internal set; }

            public byte[] InfoData { get; internal set; }

            public byte[] Data { get; internal set; }
            #endregion
        }

        public enum CharaSex : byte
        {
            Male = 0,
            Female = 1
        }
        #endregion
    }
}
