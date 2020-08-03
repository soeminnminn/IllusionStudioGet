using System;
using System.IO;
using System.Reflection;

namespace Illusion.Card
{
    public partial class PHCharaCard : ICharaCard
    {
        #region Variables
        public readonly string marker;
        #endregion

        #region Properties
        public string Game { get => "Play Home"; }

        public int DataVersion { get; internal set; }

        public string Name { get; private set; }

        public int Sex { get; private set; }

        public byte[] PngData { get; set; }

        public PHCard.HairParameter Hair { get; private set; }

        public PHCard.HeadParameter Head { get; private set; }

        public PHCard.BodyParameter Body { get; private set; }

        public PHCard.WearParameter Wear { get; private set; }

        public PHCard.AccessoryParameter Accessory { get; private set; }

        public string SourceFileName { get; }
        #endregion

        #region Constructor
        public PHCharaCard(string srcFileName, string marker, short sex)
        {
            this.SourceFileName = srcFileName;
            this.marker = marker;
            this.Name = Path.GetFileNameWithoutExtension(srcFileName);
            this.Sex = sex;

            this.Hair = new PHCard.HairParameter();
            this.Head = new PHCard.HeadParameter();
            this.Body = new PHCard.BodyParameter(sex == 0 ? PHCard.SEX.MALE : PHCard.SEX.FEMALE);
            this.Wear = new PHCard.WearParameter();
            this.Accessory = new PHCard.AccessoryParameter();
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
                PHCard.SEX sex = (PHCard.SEX)reader.ReadInt32();

                var version = (PHCard.CUSTOM_DATA_VERSION)DataVersion;

                this.Hair.Load(reader, sex, version);
                this.Head.Load(reader, sex, version);
                this.Body.Load(reader, sex, version);
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

                    writer.Write(10); // DataVersion
                    writer.Write((int)sex);
                    Hair.Save(writer, sex);
                    Head.Save(writer, sex);
                    Body.Save(writer, sex);
                    Wear.Save(writer, sex);
                    Accessory.Save(writer, sex);
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
        #endregion
    }
}
