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

        public string Name { get; set; }

        public int Sex { get; private set; }

        public byte[] PngData { get; set; }

        public string SourceFileName { get; }

        public CustomParam CharaParameter { get; private set; }
        #endregion

        #region Constructor
        public PHCharaCard(string srcFileName, string marker, short sex)
        {
            this.SourceFileName = srcFileName;
            if (string.IsNullOrEmpty(marker))
            {
                this.marker = sex == 0 ? "【PlayHome_Male】" : "【PlayHome_Female】";
            }
            else
            {
                this.marker = marker;
            }
            this.Name = Path.GetFileNameWithoutExtension(srcFileName);
            this.Sex = sex;

            this.CharaParameter = new CustomParam();
        }
        #endregion

        #region Methods

        #region Color Reader
        private byte[] ReadColorHair(BinaryReader reader, int version)
        {
            BinaryList color = new BinaryList();
            color.Add(1); // colorType

            if (version < 4)
            {
                color.AddRange(reader.ReadBytes(16)); // mainColor
                color.AddAll(0.75f, 0.75f, 0.75f, 1.0f); // cuticleColor
                color.Add(6f); // cuticleExp
                color.AddAll(0.75f, 0.75f, 0.75f, 1.0f); // fresnelColor
                color.Add(0.3f); // fresnelExp
            }
            else
            {
                reader.ReadInt32(); // colorType >> 1

                // mainColor, cuticleColor, cuticleExp, fresnelColor, fresnelExp
                color.AddRange(reader.ReadBytes(56)); 
            }

            return color.ToArray();
        }

        private byte[] ReadColorPBR1(BinaryReader reader, int version)
        {
            BinaryList color = new BinaryList();
            color.Add(2); // colorType

            if (version < 4)
            {
                color.AddRange(reader.ReadBytes(16)); // mainColor1
                color.AddAll(1f, 1f, 1f, 1f); // specColor1
                color.Add(0f); // specular1
                color.Add(0f); // smooth1
            }
            else
            {
                int colorType = reader.ReadInt32(); // colorType
                if (colorType != 0)
                {
                    // mainColor1, specColor1, specular1, smooth1
                    color.AddRange(reader.ReadBytes(40));
                }
                else
                {
                    color.AddAll(1f, 1f, 1f, 1f); // mainColor1
                    color.AddAll(1f, 1f, 1f, 1f); // specColor1
                    color.Add(0f); // specular1
                    color.Add(0f); // smooth1
                }
            }

            return color.ToArray();
        }

        private byte[] ReadColorPBR2(BinaryReader reader, int version)
        {
            BinaryList color = new BinaryList();
            color.Add(3); // colorType

            if (version < 4)
            {
                color.AddRange(reader.ReadBytes(16)); // mainColor1

                color.AddAll(1f, 1f, 1f, 1f); // specColor1
                color.Add(0f); // specular1
                color.Add(0f); // smooth1

                color.AddAll(1f, 1f, 1f, 1f); // mainColor2
                color.AddAll(1f, 1f, 1f, 1f); // specColor2
                color.Add(0f); // specular2
                color.Add(0f); // smooth2
            }
            else
            {
                int colorType = reader.ReadInt32(); // colorType
                if (colorType != 0)
                {
                    // mainColor1, specColor1, specular1
                    // smooth1, mainColor2, specColor2
                    color.AddRange(reader.ReadBytes(72));

                    if (version >= 5)
                        color.Add(reader.ReadSingle()); // specular2
                    else
                        color.Add(0f); // specular2

                    color.Add(reader.ReadSingle()); // smooth2
                }
                else
                {
                    color.AddAll(1f, 1f, 1f, 1f); // mainColor1
                    color.AddAll(1f, 1f, 1f, 1f); // specColor1
                    color.Add(0f); // specular1
                    color.Add(0f); // smooth1

                    color.AddAll(1f, 1f, 1f, 1f); // mainColor2
                    color.AddAll(1f, 1f, 1f, 1f); // specColor2
                    color.Add(0f); // specular2
                    color.Add(0f); // smooth2
                }
            }

            return color.ToArray();
        }

        private byte[] ReadColorAlloy(BinaryReader reader, int version)
        {
            BinaryList color = new BinaryList();
            color.Add(4); // colorType

            if (version < 4)
            {
                color.AddRange(reader.ReadBytes(16)); // mainColor

                color.Add(0f); // metallic
                color.Add(0f); // smooth
            }
            else
            {
                int colorType = reader.ReadInt32(); // colorType
                if (colorType != 0)
                {
                    // mainColor, metallic, smooth
                    color.AddRange(reader.ReadBytes(24));
                }
                else
                {
                    color.AddAll(1f, 1f, 1f, 1f); // mainColor
                    color.Add(0f); // metallic
                    color.Add(0f); // smooth
                }
            }

            return color.ToArray();
        }

        private byte[] ReadColorAlloyHSVOffset(BinaryReader reader, int version)
        {
            BinaryList color = new BinaryList();
            color.Add(5); // colorType

            if (version < 4)
            {
                reader.ReadBytes(16);

                color.AddAll(0f, 1f, 1f, 1f); // hsv offset + alpha
                color.Add(0f); // metallic
                color.Add(0.562f); // smooth
            }
            else
            {
                int colorType = reader.ReadInt32(); // colorType
                if (colorType != 0)
                {
                    if (version < 6)
                    {
                        reader.ReadBytes(16);
                        color.AddAll(0f, 1f, 1f, 1f); // hsv offset + alpha
                    }
                    else
                    {
                        // offset_h, offset_s, offset_v
                        color.AddRange(reader.ReadBytes(12));

                        if (version == 7)
                        {
                            reader.ReadBoolean();
                            color.Add(reader.ReadSingle()); // alpha
                        }
                        else if (version >= 8)
                        {
                            color.Add(reader.ReadSingle()); // alpha
                        }
                        else
                        {
                            color.Add(1f); // alpha
                        }
                    }

                    // metallic, smooth
                    color.AddRange(reader.ReadBytes(8));
                }
                else
                {
                    color.AddAll(0f, 1f, 1f, 1f); // hsv offset + alpha
                    color.Add(0f); // metallic
                    color.Add(0.562f); // smooth
                }
            }

            return color.ToArray();
        }

        private byte[] ReadColorEyeHighlight(BinaryReader reader, int version)
        {
            BinaryList color = new BinaryList();
            color.Add(7); // colorType

            if (version < 4)
            {
                color.AddRange(reader.ReadBytes(16)); // mainColor1

                color.AddAll(1f, 1f, 1f, 1f); // specColor1
                color.Add(0f); // specular1
                color.Add(0f); // smooth1
            }
            else
            {
                int colorType = reader.ReadInt32(); // colorType
                if (colorType != 0)
                {
                    // mainColor1, specColor1, specular1, smooth1
                    color.AddRange(reader.ReadBytes(40));
                }
                else
                {
                    color.AddAll(1f, 1f, 1f, 1f); // mainColor1
                    color.AddAll(1f, 1f, 1f, 1f); // specColor1
                    color.Add(0f); // specular1
                    color.Add(0f); // smooth1
                }
            }

            return color.ToArray();
        }
        #endregion

        #region CustomParameter
        private HairPart ReadHairPart(BinaryReader reader, int version)
        {
            HairPart part = new HairPart();
            part.id = reader.ReadInt32(); // id
            part.hairColor = ReadColorHair(reader, version); // hairColor

            if (version > 0)
            {
                part.accColor = ReadColorPBR1(reader, version); // acceColor
            }
            else
            {
                part.accColor = new byte[] { 0x0, 0x0, 0x0, 0x0 };
            }
            return part;
        }

        private void ReadHair(BinaryReader reader, CustomParam chara)
        {
            int hairPartsCount = reader.ReadInt32();
            chara.hairParts = new HairPart[hairPartsCount];

            for (int i = 0; i < hairPartsCount; i++)
            {
                HairPart part = ReadHairPart(reader, chara.version);
                chara.hairParts[i] = part;
            }
        }

        private void ReadHead(BinaryReader reader, CustomParam chara)
        {
            BinaryList list = new BinaryList();

            // headID, faceTexID, detailID, detailWeight, eyeBrowID
            list.AddRange(reader.ReadBytes(20));

            // eyeBrowColor
            list.AddRange(ReadColorPBR1(reader, chara.version)); 

            if (chara.version < 4)
            {
                // eyeScleraColor
                byte[] eyeScleraColor = reader.ReadBytes(16); 
                // eyeL
                list.Add(reader.ReadInt32()); // eyeID_L
                list.AddRange(eyeScleraColor); // eyeScleraColorL
                list.AddRange(reader.ReadBytes(16)); // eyeIrisColorL
                list.AddAll(0.0f, 0.5f); // eyePupilDilationL, eyeEmissiveL

                // eyeR
                list.Add(reader.ReadInt32()); // eyeID_R
                list.AddRange(eyeScleraColor); // eyeScleraColorR
                list.AddRange(reader.ReadBytes(16)); // eyeIrisColorR
                list.AddAll(0.0f, 0.5f); // eyePupilDilationR, eyeEmissiveR
            }
            else
            {
                // eyeID_L, eyeScleraColorL, eyeIrisColorL eyePupilDilationL
                list.AddRange(reader.ReadBytes(40));
                if (chara.version >= 10)
                {
                    list.Add(reader.ReadSingle()); // eyeEmissiveL
                }
                else
                {
                    list.Add(0.5f); // eyeEmissiveL
                }

                // eyeID_R, eyeScleraColorR, eyeIrisColorR, eyePupilDilationR
                list.AddRange(reader.ReadBytes(40));

                if (chara.version >= 10)
                {
                    list.Add(reader.ReadSingle()); // eyeEmissiveR
                }
                else
                {
                    list.Add(0.5f); // eyeEmissiveR
                }
            }

            // tattooID, tattooColor
            list.AddRange(reader.ReadBytes(20));

            // shapeVals
            int shapeCount = reader.ReadInt32();
            list.Add(shapeCount);
            list.AddRange(reader.ReadBytes(shapeCount * 4));

            if (chara.sex == 0)
            {
                // eyeLash
                list.Add(reader.ReadInt32());
                list.AddRange(ReadColorPBR1(reader, chara.version));
                // eyeshadow
                list.AddRange(reader.ReadBytes(20));
                // cheek
                list.AddRange(reader.ReadBytes(20));
                // lip
                list.AddRange(reader.ReadBytes(20));
                // mole
                list.AddRange(reader.ReadBytes(20));

                // eyeHighlight
                list.Add(reader.ReadInt32());
                list.AddRange(ReadColorEyeHighlight(reader, chara.version));
            }
            else
            {
                // beard
                list.AddRange(reader.ReadBytes(20));

                if (chara.version >= 2)
                {
                    // eyeHighlightColor
                    list.AddRange(ReadColorEyeHighlight(reader, chara.version)); 
                }
                else
                {
                    // eyeHighlightColor
                    list.Add(7); // colorType
                    list.AddAll(1f, 1f, 1f, 1f); // mainColor1
                    list.AddAll(1f, 1f, 1f, 1f); // specColor1
                    list.AddAll(0f, 0f); // specular1, smooth1
                }
            }

            chara.head = list.ToArray();
        }

        private void ReadBody(BinaryReader reader, CustomParam chara)
        {
            BinaryList list = new BinaryList();

            // bodyID
            list.Add(reader.ReadInt32());
            // skinColor
            list.AddRange(ReadColorAlloyHSVOffset(reader, chara.version)); 

            // detailID, detailWeight, underhairID
            list.AddRange(reader.ReadBytes(12));

            // underhairColor
            list.AddRange(ReadColorAlloy(reader, chara.version)); 

            // tattooID, tattooColor
            list.AddRange(reader.ReadBytes(20));

            // shapeVals
            int shapeCount = reader.ReadInt32();
            list.Add(shapeCount);
            list.AddRange(reader.ReadBytes(shapeCount * 4));

            if (chara.sex == 0)
            {
                // nipID
                list.Add(reader.ReadInt32());
                // nipColor
                list.AddRange(ReadColorAlloyHSVOffset(reader, chara.version)); 
                // sunburnID, sunburnColor
                list.AddRange(reader.ReadBytes(20));

                if (chara.version >= 3)
                {
                    // nailColor
                    list.AddRange(ReadColorAlloyHSVOffset(reader, chara.version)); 

                    if (chara.version >= 9)
                    {
                        // manicureColor
                        list.AddRange(ReadColorPBR1(reader, chara.version)); 
                    }
                    else
                    {
                        // manicureColor
                        list.Add(2); // colorType
                        list.AddAll(1f, 1f, 1f, 0f); // mainColor1
                        list.AddAll(1f, 1f, 1f, 1f); // specColor1
                        list.AddAll(0f, 0f); // specular1, smooth1
                    }

                    // areolaSize, bustSoftness, bustWeight
                    list.AddRange(reader.ReadBytes(12));
                }
                else
                {
                    // nailColor
                    list.Add(5); // colorType
                    list.AddAll(0f, 1f, 1f, 1f); // hsv offset + alpha
                    list.AddAll(0f, 0.562f); // metallic, smooth

                    // manicureColor
                    list.Add(2); // colorType
                    list.AddAll(1f, 1f, 1f, 0f); // mainColor1
                    list.AddAll(1f, 1f, 1f, 1f); // specColor1
                    list.AddAll(0f, 0f); // specular1, smooth1

                    // areolaSize, bustSoftness, bustWeight
                    list.AddAll(0.5f, 0.5f, 0.5f);
                }
            }

            chara.body = list.ToArray();
        }

        private void ReadWear(BinaryReader reader, CustomParam chara)
        {
            chara.wear = new byte[11][];

            for (int i = 0; i < 11; i++)
            {
                BinaryList list = new BinaryList();
                // WEAR_TYPE, id
                list.AddRange(reader.ReadBytes(8));

                // color
                list.AddRange(ReadColorPBR2(reader, chara.version)); 
                chara.wear[i] = list.ToArray();
            }

            if (chara.sex == 0)
            {
                chara.isSwimwear = reader.ReadBoolean(); // isSwimwear
                chara.swimOptTop = reader.ReadBoolean(); // swimOptTop
                chara.swimOptBtm = reader.ReadBoolean(); // swimOptBtm
            }
        }

        private void ReadAccessory(BinaryReader reader, CustomParam chara)
        {
            chara.accessories = new byte[10][];
            for (int i = 0; i < 10; i++)
            {
                BinaryList list = new BinaryList();

                // ACCESSORY_TYPE, id, nowAttach, addPos, addRot addScl
                list.AddRange(reader.ReadBytes(48));

                list.AddRange(ReadColorPBR2(reader, chara.version)); // color

                chara.accessories[i] = list.ToArray();
            }
        }

        private CustomParam ReadCustomParameter(BinaryReader reader)
        {
            CustomParam chara = new CustomParam();

            chara.version = reader.ReadInt32(); // 10
            chara.sex = reader.ReadInt32();

            ReadHair(reader, chara);
            ReadHead(reader, chara);
            ReadBody(reader, chara);
            ReadWear(reader, chara);
            ReadAccessory(reader, chara);

            return chara;
        }
        #endregion

        public string GenerateFileName()
        {
            string name = this.Name;
            if (string.IsNullOrEmpty(name))
            {
                name = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            }
            return $"{name}.png";
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

                this.CharaParameter = ReadCustomParameter(reader);

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
                    writer.Write(marker);

                    var chara = this.CharaParameter;
                    writer.Write(10);
                    writer.Write(chara.sex);

                    // Hair
                    writer.Write(chara.hairParts.Length);
                    for (int i = 0; i < chara.hairParts.Length; i++)
                    {
                        var part = chara.hairParts[i];
                        writer.Write(part.id);
                        writer.Write(part.hairColor);
                        writer.Write(part.accColor);
                    }

                    // Head
                    writer.Write(chara.head);

                    // Body
                    writer.Write(chara.body);

                    // Wear
                    for (int i = 0; i < chara.wear.Length; i++)
                    {
                        writer.Write(chara.wear[i]);
                    }
                    if (chara.sex == 0)
                    {
                        writer.Write(chara.isSwimwear);
                        writer.Write(chara.swimOptTop);
                        writer.Write(chara.swimOptBtm);
                    }

                    // Accessory
                    for (int i = 0; i < chara.accessories.Length; i++)
                    {
                        writer.Write(chara.accessories[i]);
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
        public class CustomParam
        {
            #region Variables
            public int version = 10;
            public int sex = 0;
            public string name = string.Empty;

            public HairPart[] hairParts = null;
            public byte[] head = null;
            public byte[] body = null;

            public byte[][] wear = null;
            public bool isSwimwear = false;
            public bool swimOptTop = false;
            public bool swimOptBtm = false;

            public byte[][] accessories = null;
            #endregion
        }

        public class HairPart
        {
            #region Variables
            public int id = 0;
            public byte[] hairColor = null;
            public byte[] accColor = null;
            #endregion
        }
        #endregion
    }
}
