using System;
using System.IO;
using System.Reflection;

namespace Illusion.Card
{
    public partial class PHCoordinateCard : ICoordinateCard
    {
        #region Variables
        #endregion

        #region Properties
        public string Game { get => "Play Home"; }

        public string Name { get; }

        public int Sex { get; }

        public int DataVersion { get; private set; }

        public byte[] PngData { get; set; }

        public PHCard.WearParameter Wear { get; private set; }

        public PHCard.AccessoryParameter Accessory { get; private set; }

        public string SourceFileName { get; }
        #endregion

        #region Constructor
        public PHCoordinateCard(string srcFileName, short sex)
        {
            this.SourceFileName = srcFileName;
            this.Name = Path.GetFileNameWithoutExtension(srcFileName);
            this.Sex = sex;

            this.Wear = new PHCard.WearParameter();
            this.Accessory = new PHCard.AccessoryParameter();
        }

        public PHCoordinateCard(PHCharaCard charaCard)
        {
            this.SourceFileName = charaCard.SourceFileName;
            this.Name = charaCard.Name + "の服装";
            this.Sex = charaCard.Sex;

            this.Wear = charaCard.Wear;
            this.Accessory = charaCard.Accessory;
        }
        #endregion

        #region Methods
        public string GenerateFileName()
        {
            return $"{this.Name}.png";
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

                PHCard.CUSTOM_DATA_VERSION version = (PHCard.CUSTOM_DATA_VERSION)DataVersion;

                PHCard.SEX sex = (PHCard.SEX)reader.ReadInt32();

                this.Wear.Load(reader, sex, version);
                this.Accessory.Load(reader, sex, version);

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

                PHCard.SEX sex = Sex == 0 ? PHCard.SEX.MALE : PHCard.SEX.FEMALE;

                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(pngData);

                    writer.Write((int)PHCard.CUSTOM_DATA_VERSION.DEBUG_10);
                    writer.Write((int)sex);

                    this.Wear.Save(writer, sex);
                    this.Accessory.Save(writer, sex);
                }
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
