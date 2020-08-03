using System;
using System.IO;

namespace Illusion.Card
{
    public static class PHColor
    {
        #region Methods
        public static void ReadColor(BinaryReader reader, ref UnityColor color)
        {
            color.r = reader.ReadSingle();
            color.g = reader.ReadSingle();
            color.b = reader.ReadSingle();
            color.a = reader.ReadSingle();
        }

        public static void WriteColor(BinaryWriter writer, UnityColor color)
        {
            writer.Write(color.r);
            writer.Write(color.g);
            writer.Write(color.b);
            writer.Write(color.a);
        }
        #endregion

        #region Nested Types
        public enum ColorTypes : int
        {
            NONE,
            HAIR,
            PBR1,
            PBR2,
            ALLOY,
            ALLOY_HSV,
            EYE,
            EYEHIGHLIGHT
        }

        public class Hair
        {
            #region Variables
            public UnityColor mainColor = UnityColor.black;
            public UnityColor cuticleColor = new UnityColor(0.75f, 0.75f, 0.75f);
            public float cuticleExp = 6f;
            public UnityColor fresnelColor = new UnityColor(0.75f, 0.75f, 0.75f);
            public float fresnelExp = 0.3f;
            #endregion

            #region Methods
            public void Load(BinaryReader reader, PHCard.CUSTOM_DATA_VERSION version)
            {
                if (version < PHCard.CUSTOM_DATA_VERSION.DEBUG_04)
                {
                    ReadColor(reader, ref mainColor);
                    return;
                }

                ColorTypes colorType = (ColorTypes)reader.ReadInt32();
                if (colorType != ColorTypes.HAIR)
                {
                    System.Diagnostics.Debug.WriteLine("色タイプが違う");
                }

                ReadColor(reader, ref mainColor);
                ReadColor(reader, ref cuticleColor);
                this.cuticleExp = reader.ReadSingle();
                ReadColor(reader, ref fresnelColor);
                this.fresnelExp = reader.ReadSingle();
            }

            public void Save(BinaryWriter writer)
            {
                writer.Write((int)ColorTypes.HAIR);
                WriteColor(writer, mainColor);
                WriteColor(writer, cuticleColor);
                writer.Write(cuticleExp);
                WriteColor(writer, fresnelColor);
                writer.Write(fresnelExp);
            }
            #endregion
        }

        public class PBR1
        {
            #region Variables
            public UnityColor mainColor1 = UnityColor.white;
            public UnityColor specColor1 = UnityColor.white;
            public float specular1;
            public float smooth1;
            #endregion

            #region Methods
            public void Load(BinaryReader reader, PHCard.CUSTOM_DATA_VERSION version)
            {
                if (version < PHCard.CUSTOM_DATA_VERSION.DEBUG_04)
                {
                    ReadColor(reader, ref mainColor1);
                    return;
                }

                ColorTypes colorType = (ColorTypes)reader.ReadInt32();
                if (colorType == ColorTypes.NONE)
                    return;

                ReadColor(reader, ref mainColor1);
                ReadColor(reader, ref specColor1);
                this.specular1 = reader.ReadSingle();
                this.smooth1 = reader.ReadSingle();
            }

            public void Save(BinaryWriter writer)
            {
                writer.Write((int)ColorTypes.PBR1);
                WriteColor(writer, this.mainColor1);
                WriteColor(writer, this.specColor1);
                writer.Write(this.specular1);
                writer.Write(this.smooth1);
            }
            #endregion
        }

        public class PBR2
        {
            #region Variables
            public UnityColor mainColor1 = UnityColor.white;
            public UnityColor specColor1 = UnityColor.white;
            public UnityColor mainColor2 = UnityColor.white;
            public UnityColor specColor2 = UnityColor.white;
            public float specular1;
            public float smooth1;
            public float specular2;
            public float smooth2;
            #endregion

            #region Methods
            public void Load(BinaryReader reader, PHCard.CUSTOM_DATA_VERSION version)
            {
                if (version < PHCard.CUSTOM_DATA_VERSION.DEBUG_04)
                {
                    ReadColor(reader, ref mainColor1);
                    return;
                }

                ColorTypes colorType = (ColorTypes)reader.ReadInt32();
                if (colorType == ColorTypes.NONE)
                    return;

                ReadColor(reader, ref mainColor1);
                ReadColor(reader, ref this.specColor1);
                this.specular1 = reader.ReadSingle();
                this.smooth1 = reader.ReadSingle();
                ReadColor(reader, ref this.mainColor2);
                ReadColor(reader, ref this.specColor2);
                this.specular2 = version >= PHCard.CUSTOM_DATA_VERSION.DEBUG_05 ? reader.ReadSingle() : 0.0f;
                this.smooth2 = reader.ReadSingle();
            }

            public void Save(BinaryWriter writer)
            {
                writer.Write((int)ColorTypes.PBR2);
                WriteColor(writer, this.mainColor1);
                WriteColor(writer, this.specColor1);
                writer.Write(this.specular1);
                writer.Write(this.smooth1);
                WriteColor(writer, this.mainColor2);
                WriteColor(writer, this.specColor2);
                writer.Write(this.specular2);
                writer.Write(this.smooth2);
            }
            #endregion
        }

        public class Alloy
        {
            #region Variables
            public UnityColor mainColor = UnityColor.white;
            public float metallic;
            public float smooth;
            #endregion

            #region Methods
            public void Load(BinaryReader reader, PHCard.CUSTOM_DATA_VERSION version)
            {
                if (version < PHCard.CUSTOM_DATA_VERSION.DEBUG_04)
                {
                    ReadColor(reader, ref mainColor);
                    return;
                }

                ColorTypes colorType = (ColorTypes)reader.ReadInt32();
                if (colorType == ColorTypes.NONE)
                    return;

                ReadColor(reader, ref mainColor);
                this.metallic = reader.ReadSingle();
                this.smooth = reader.ReadSingle();
            }

            public void Save(BinaryWriter writer)
            {
                writer.Write((int)ColorTypes.ALLOY);
                WriteColor(writer, this.mainColor);
                writer.Write(this.metallic);
                writer.Write(this.smooth);
            }
            #endregion
        }

        public class AlloyHSVOffset
        {
            #region Variables
            public UnityColor mainColor = UnityColor.white;
            public float offset_s = 1f;
            public float offset_v = 1f;
            public float alpha = 1f;
            public float smooth = 0.562f;
            public float offset_h;
            public bool hasAlpha;
            public float metallic;
            #endregion

            #region Constructor
            public AlloyHSVOffset()
            { }

            public AlloyHSVOffset(bool hasAlpha, float alpha)
            {
                this.hasAlpha = hasAlpha;
                this.alpha = alpha;
            }
            #endregion

            #region Methods
            public void Load(BinaryReader reader, PHCard.CUSTOM_DATA_VERSION version)
            {
                if (version < PHCard.CUSTOM_DATA_VERSION.DEBUG_04)
                {
                    ReadColor(reader, ref mainColor);
                    return;
                }

                ColorTypes colorType = (ColorTypes)reader.ReadInt32();
                if (colorType == ColorTypes.NONE)
                    return;

                if (colorType != ColorTypes.ALLOY_HSV)
                {
                    if (version < PHCard.CUSTOM_DATA_VERSION.DEBUG_06 && colorType == ColorTypes.ALLOY)
                    { }
                }

                if (version < PHCard.CUSTOM_DATA_VERSION.DEBUG_06)
                {
                    ReadColor(reader, ref mainColor);
                }
                else
                {
                    this.offset_h = reader.ReadSingle();
                    this.offset_s = reader.ReadSingle();
                    this.offset_v = reader.ReadSingle();
                    if (version == PHCard.CUSTOM_DATA_VERSION.DEBUG_07)
                    {
                        reader.ReadBoolean();
                        this.alpha = reader.ReadSingle();
                    }
                    else if (version >= PHCard.CUSTOM_DATA_VERSION.TRIAL)
                        this.alpha = reader.ReadSingle();
                }

                this.metallic = reader.ReadSingle();
                this.smooth = reader.ReadSingle();
            }

            public void Save(BinaryWriter writer)
            {
                writer.Write((int)ColorTypes.ALLOY_HSV);
                writer.Write(this.offset_h);
                writer.Write(this.offset_s);
                writer.Write(this.offset_v);
                writer.Write(this.alpha);
                writer.Write(this.metallic);
                writer.Write(this.smooth);
            }
            #endregion
        }

        public class EyeHighlight
        {
            #region Variables
            public UnityColor mainColor1 = UnityColor.white;
            public UnityColor specColor1 = UnityColor.white;
            public float specular1 = 1f;
            public float smooth1 = 1f;
            #endregion

            #region Methods
            public void Load(BinaryReader reader, PHCard.CUSTOM_DATA_VERSION version)
            {
                if (version < PHCard.CUSTOM_DATA_VERSION.DEBUG_04)
                {
                    ReadColor(reader, ref mainColor1);
                    return;
                }

                ColorTypes colorType = (ColorTypes)reader.ReadInt32();
                if (colorType == ColorTypes.NONE)
                {
                    System.Diagnostics.Debug.WriteLine("色タイプが違う");
                }

                ReadColor(reader, ref this.mainColor1);
                ReadColor(reader, ref this.specColor1);
                this.specular1 = reader.ReadSingle();
                this.smooth1 = reader.ReadSingle();
            }

            public void Save(BinaryWriter writer)
            {
                writer.Write((int)ColorTypes.EYEHIGHLIGHT);
                WriteColor(writer, this.mainColor1);
                WriteColor(writer, this.specColor1);
                writer.Write(this.specular1);
                writer.Write(this.smooth1);
            }
            #endregion
        }
        #endregion
    }
}
