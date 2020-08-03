using System;
using System.IO;
using System.Reflection;

namespace Illusion.Card
{
    public partial class HSCoordinateCard : ICoordinateCard
    {
        #region Variables
        public const string markerMale = "【HoneySelectClothesMale】";
        public const string markerFemale = "【HoneySelectClothesFemale】";
        public readonly string marker;

        public Clothes[] clothes;
        public byte clothesTypeSex;
        public Accessory[] accessory;
        public string comment = "コーディネート名";

        public bool swimType = false;
        public bool hideSwimOptTop = false;
        public bool hideSwimOptBot = false;
        #endregion

        #region Constructor
        public HSCoordinateCard(string srcFileName, short sex)
        {
            SourceFileName = srcFileName;
            Sex = sex;

            marker = sex == 0 ? markerMale : markerFemale;
            clothesTypeSex = (byte)sex;
        }
        #endregion

        #region Properties
        public string Game { get => "Honey Select"; }

        public int DataVersion { get; private set; }

        public int Version { get; private set; }

        public string Name { get; }

        public int Sex { get; }

        public byte[] PngData { get; set; }

        public string SourceFileName { get; }
        #endregion

        #region Methods
        public string GenerateFileName()
        {
            string fileNamePrefix = Sex == 0 ? "coordM_" : "coordF_";
            return fileNamePrefix + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
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
                if (DataVersion > 3) return false;

                Version = reader.ReadInt32();

                int clothesCount = reader.ReadInt32();
                clothes = new Clothes[clothesCount];
                for (int i = 0; i < clothesCount; i++)
                {
                    clothes[i] = new Clothes();
                    clothes[i].Load(reader, DataVersion);
                }

                int accessoryCount = reader.ReadInt32();
                accessory = new Accessory[accessoryCount];

                for (int i = 0; i < accessoryCount; i++)
                {
                    accessory[i] = new Accessory();
                    accessory[i].Load(reader, DataVersion);
                }

                if (DataVersion >= 2)
                {
                    comment = reader.ReadString();
                    clothesTypeSex = reader.ReadByte();
                }

                if (Sex == 1)
                {
                    swimType = reader.ReadBoolean();
                    hideSwimOptTop = reader.ReadBoolean();
                    hideSwimOptBot = reader.ReadBoolean();
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
                    Save(writer);
                    writer.Write(position);
                }

                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return false;
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(marker);
            writer.Write(3); // Version
            writer.Write(1); // ProductNo

            writer.Write(clothes.Length);
            for (int i = 0; i < clothes.Length; i++)
            {
                clothes[i].Save(writer);
            }

            writer.Write(accessory.Length);
            for (int i = 0; i < accessory.Length; i++)
            {
                accessory[i].Save(writer);
            }

            writer.Write(comment);
            writer.Write(clothesTypeSex);

            if (Sex == 1)
            {
                writer.Write(swimType);
                writer.Write(hideSwimOptTop);
                writer.Write(hideSwimOptBot);
            }
        }
        #endregion
    }
}
