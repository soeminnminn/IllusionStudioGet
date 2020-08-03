using System;
using System.IO;
using System.Reflection;

namespace Illusion.Card
{
    public class AISCoordinateCard : ICoordinateCard
    {
        #region Variables
        private readonly string marker = "【AIS_Clothes】";
        private string fileNamePrefix = string.Empty;
        #endregion

        #region Constructor
        public AISCoordinateCard()
        { }

        public AISCoordinateCard(AISCharaCard charaCard)
        {
            var info = charaCard.BlocksInfo.FindInfo("Coordinate");
            if (info != null)
            {
                var data = charaCard.DataBlocks[info.name];
                if (data != null && data.Length > 0)
                {
                    SourceFileName = charaCard.SourceFileName;
                    DataVersion = charaCard.DataVersion;
                    Language = charaCard.Language;
                    Name = charaCard.Name + "の服装";
                    Sex = charaCard.Sex;
                    Version = info.version;
                    Data = data;
                    fileNamePrefix = Sex == 0 ? "HS2CoordM_" : "HS2CoordF_";
                }
            }
        }
        #endregion

        #region Properties
        public string Game { get => "AIS / HS2"; }

        public string Version { get; internal set; }

        public string Name { get; }

        public int Sex { get; }

        public int DataVersion { get; internal set; }

        public int Language { get; internal set; }

        public byte[] PngData { get; set; }

        public byte[] Data { get; internal set; }

        public string SourceFileName { get; }
        #endregion

        #region Methods
        public bool Parse(BinaryReader reader, long pngEnd)
        {
            throw new NotImplementedException();
        }

        public string GenerateFileName()
        {
            return fileNamePrefix + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
        }

        public bool Save(Stream stream, SaveOptions options)
        {
            if (stream == null || !stream.CanWrite)
            {
                return false;
            }

            try
            {
                if (Data == null || Data.Length == 0)
                {
                    return false;
                }

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

                    writer.Write(DataVersion);
                    writer.Write(marker);
                    writer.Write(Version);
                    writer.Write(Language);
                    writer.Write(Name);
                    writer.Write(Data.Length);
                    writer.Write(Data);

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
    }
}
