using System;
using System.Collections.Generic;
using System.IO;

namespace Illusion.Card
{
    public static class PHCard
    {
        #region Enum
        public enum SEX : int
        {
            FEMALE,
            MALE,
        }

        public enum CUSTOM_DATA_VERSION : int
        {
            UNKNOWN = -1, // 0xFFFFFFFF
            DEBUG_00 = 0,
            DEBUG_01 = 1,
            DEBUG_02 = 2,
            DEBUG_03 = 3,
            DEBUG_04 = 4,
            DEBUG_05 = 5,
            DEBUG_06 = 6,
            DEBUG_07 = 7,
            TRIAL = 8,
            DEBUG_09 = 9,
            DEBUG_10 = 10, // 0x0000000A
            NEW = 10, // 0x0000000A
            NEXT = 11, // 0x0000000B
        }

        public enum WEAR_TYPE : int
        {
            TOP,
            BOTTOM,
            BRA,
            SHORTS,
            SWIM,
            SWIM_TOP,
            SWIM_BOTTOM,
            GLOVE,
            PANST,
            SOCKS,
            SHOES,
            NUM,
        }

        public enum ACCESSORY_ATTACH : int
        {
            NONE = -1, // 0xFFFFFFFF
            AP_Head = 0,
            AP_Megane = 1,
            AP_Earring_L = 2,
            AP_Earring_R = 3,
            AP_Mouth = 4,
            AP_Nose = 5,
            AP_Neck = 6,
            AP_Chest = 7,
            AP_Wrist_L = 8,
            AP_Wrist_R = 9,
            AP_Arm_L = 10, // 0x0000000A
            AP_Arm_R = 11, // 0x0000000B
            AP_Index_L = 12, // 0x0000000C
            AP_Index_R = 13, // 0x0000000D
            AP_Middle_L = 14, // 0x0000000E
            AP_Middle_R = 15, // 0x0000000F
            AP_Ring_L = 16, // 0x00000010
            AP_Ring_R = 17, // 0x00000011
            AP_Leg_L = 18, // 0x00000012
            AP_Leg_R = 19, // 0x00000013
            AP_Ankle_L = 20, // 0x00000014
            AP_Ankle_R = 21, // 0x00000015
            AP_Tikubi_L = 22, // 0x00000016
            AP_Tikubi_R = 23, // 0x00000017
            AP_Waist = 24, // 0x00000018
            AP_Shoulder_L = 25, // 0x00000019
            AP_Shoulder_R = 26, // 0x0000001A
            AP_Hand_L = 27, // 0x0000001B
            AP_Hand_R = 28, // 0x0000001C
            SKINNING = 29, // 0x0000001D
            NUM = 30, // 0x0000001E
        }

        public enum ACCESSORY_TYPE : int
        {
            NONE = -1, // 0xFFFFFFFF
            HEAD = 0,
            EAR = 1,
            GLASSES = 2,
            FACE = 3,
            NECK = 4,
            SHOULDER = 5,
            CHEST = 6,
            WAIST = 7,
            BACK = 8,
            ARM = 9,
            HAND = 10, // 0x0000000A
            LEG = 11, // 0x0000000B
            NUM = 12, // 0x0000000C
        }
        #endregion

        #region Nested Types
        public class HairParameter
        {
            #region Variables
            public HairPartParameter[] parts;
            #endregion

            #region Constructor
            public HairParameter()
            {
                this.parts = new HairPartParameter[3];
                for (var i = 0; i < 3; i++)
                    this.parts[i] = new HairPartParameter();
            }
            #endregion

            #region Methods
            public void Save(BinaryWriter writer, SEX sex)
            {
                writer.Write(this.parts.Length);
                for (var i = 0; i < this.parts.Length; i++)
                {
                    this.parts[i].Save(writer, sex);
                }
            }

            public void Load(BinaryReader reader, SEX sex, CUSTOM_DATA_VERSION version)
            {
                int partsCount = reader.ReadInt32();
                this.parts = new HairPartParameter[partsCount];

                for (int i = 0; i < partsCount; i++)
                {
                    this.parts[i] = new HairPartParameter();
                    this.parts[i].Load(reader, sex, version);
                }
            }
            #endregion
        }

        public class HairPartParameter
        {
            #region Variables
            public int id;
            public PHColor.Hair hairColor = new PHColor.Hair();
            public PHColor.PBR1 acceColor;
            #endregion

            #region Constructor
            public HairPartParameter()
            { }
            #endregion

            #region Methods
            public void Save(BinaryWriter writer, SEX sex)
            {
                writer.Write(this.id);
                if (this.hairColor != null)
                    this.hairColor.Save(writer);
                else
                    writer.Write(0);
                if (this.acceColor != null)
                    this.acceColor.Save(writer);
                else
                    writer.Write(0);
            }

            public void Load(BinaryReader reader, SEX sex, CUSTOM_DATA_VERSION version)
            {
                this.id = reader.ReadInt32();
                if (version <= CUSTOM_DATA_VERSION.DEBUG_03)
                {
                    UnityColor color = UnityColor.white;
                    PHColor.ReadColor(reader, ref color);

                    if (version <= CUSTOM_DATA_VERSION.DEBUG_00)
                        return;
                    PHColor.ReadColor(reader, ref color);
                }
                else
                {
                    this.hairColor.Load(reader, version);
                    this.acceColor = new PHColor.PBR1();
                    this.acceColor.Load(reader, version);
                }
            }
            #endregion
        }

        public class HeadParameter
        {
            #region Variables
            public PHColor.PBR1 eyeBrowColor = new PHColor.PBR1();
            public UnityColor eyeScleraColorL = UnityColor.white;
            public UnityColor eyeIrisColorL = UnityColor.black;
            public float eyeEmissiveL = 0.5f;
            public UnityColor eyeScleraColorR = UnityColor.white;
            public UnityColor eyeIrisColorR = UnityColor.black;
            public float eyeEmissiveR = 0.5f;
            public UnityColor tattooColor = UnityColor.white;
            public PHColor.PBR1 eyeLashColor = new PHColor.PBR1();
            public UnityColor eyeshadowColor = UnityColor.black;
            public UnityColor cheekColor = UnityColor.white;
            public UnityColor lipColor = UnityColor.white;
            public UnityColor moleColor = UnityColor.white;
            public PHColor.EyeHighlight eyeHighlightColor = new PHColor.EyeHighlight();
            public UnityColor beardColor = UnityColor.black;
            public int headID;
            public int faceTexID;
            public int detailID;
            public float detailWeight;
            public int eyeBrowID;
            public int eyeID_L;
            public float eyePupilDilationL;
            public int eyeID_R;
            public float eyePupilDilationR;
            public int tattooID;
            public float[] shapeVals;
            public int eyeLashID;
            public int eyeshadowTexID;
            public int cheekTexID;
            public int lipTexID;
            public int moleTexID;
            public int eyeHighlightTexID;
            public int beardID;
            #endregion

            #region Constructor
            public HeadParameter()
            {
                this.shapeVals = new float[67];
                for (int i = 0; i < this.shapeVals.Length; i++)
                    this.shapeVals[i] = 0.5f;
            }
            #endregion

            #region Methods
            public void Save(BinaryWriter writer, SEX sex)
            {
                writer.Write(this.headID);
                writer.Write(this.faceTexID);
                writer.Write(this.detailID);
                writer.Write(this.detailWeight);
                writer.Write(this.eyeBrowID);
                this.eyeBrowColor.Save(writer);
                writer.Write(this.eyeID_L);
                PHColor.WriteColor(writer, this.eyeScleraColorL);
                PHColor.WriteColor(writer, this.eyeIrisColorL);
                writer.Write(this.eyePupilDilationL);
                writer.Write(this.eyeEmissiveL);
                writer.Write(this.eyeID_R);
                PHColor.WriteColor(writer, this.eyeScleraColorR);
                PHColor.WriteColor(writer, this.eyeIrisColorR);
                writer.Write(this.eyePupilDilationR);
                writer.Write(this.eyeEmissiveR);
                writer.Write(this.tattooID);
                PHColor.WriteColor(writer, this.tattooColor);

                writer.Write(this.shapeVals.Length);
                for (var i = 0; i < this.shapeVals.Length; i++)
                {
                    writer.Write(this.shapeVals[i]);
                }

                if (sex == SEX.FEMALE) // Female
                {
                    writer.Write(this.eyeLashID);
                    this.eyeLashColor.Save(writer);
                    writer.Write(this.eyeshadowTexID);
                    PHColor.WriteColor(writer, this.eyeshadowColor);
                    writer.Write(this.cheekTexID);
                    PHColor.WriteColor(writer, this.cheekColor);
                    writer.Write(this.lipTexID);
                    PHColor.WriteColor(writer, this.lipColor);
                    writer.Write(this.moleTexID);
                    PHColor.WriteColor(writer, this.moleColor);
                    writer.Write(this.eyeHighlightTexID);
                    this.eyeHighlightColor.Save(writer);
                }
                else
                {
                    writer.Write(this.beardID);
                    PHColor.WriteColor(writer, this.beardColor);
                    this.eyeHighlightColor.Save(writer);
                }
            }

            public void Load(BinaryReader reader, SEX sex, CUSTOM_DATA_VERSION version)
            {
                this.headID = reader.ReadInt32();
                this.faceTexID = reader.ReadInt32();
                this.detailID = reader.ReadInt32();
                this.detailWeight = reader.ReadSingle();
                this.eyeBrowID = reader.ReadInt32();
                this.eyeBrowColor.Load(reader, version);

                this.eyePupilDilationL = 0.0f;
                this.eyePupilDilationR = 0.0f;
                this.eyeEmissiveL = 0.5f;
                this.eyeEmissiveR = 0.5f;

                if (version < CUSTOM_DATA_VERSION.DEBUG_04)
                {
                    PHColor.ReadColor(reader, ref this.eyeScleraColorL);
                    this.eyeScleraColorR = this.eyeScleraColorL;
                    this.eyeID_L = reader.ReadInt32();
                    PHColor.ReadColor(reader, ref this.eyeIrisColorL);
                    this.eyeID_R = reader.ReadInt32();
                    PHColor.ReadColor(reader, ref this.eyeIrisColorR);
                }
                else
                {
                    this.eyeID_L = reader.ReadInt32();
                    PHColor.ReadColor(reader, ref this.eyeScleraColorL);
                    PHColor.ReadColor(reader, ref this.eyeIrisColorL);
                    this.eyePupilDilationL = reader.ReadSingle();
                    if (version >= CUSTOM_DATA_VERSION.DEBUG_10)
                        this.eyeEmissiveL = reader.ReadSingle();
                    this.eyeID_R = reader.ReadInt32();
                    PHColor.ReadColor(reader, ref this.eyeScleraColorR);
                    PHColor.ReadColor(reader, ref this.eyeIrisColorR);
                    this.eyePupilDilationR = reader.ReadSingle();
                    if (version >= CUSTOM_DATA_VERSION.DEBUG_10)
                        this.eyeEmissiveR = reader.ReadSingle();
                }
                this.tattooID = reader.ReadInt32();
                PHColor.ReadColor(reader, ref this.tattooColor);

                var shapeValsCount = reader.ReadInt32();
                this.shapeVals = new float[shapeValsCount];
                for (int i = 0; i < shapeValsCount; i++)
                {
                    this.shapeVals[i] = reader.ReadSingle();
                }

                if (sex == SEX.FEMALE) // Female
                {
                    this.eyeLashID = reader.ReadInt32();
                    this.eyeLashColor.Load(reader, version);
                    this.eyeshadowTexID = reader.ReadInt32();
                    PHColor.ReadColor(reader, ref this.eyeshadowColor);
                    this.cheekTexID = reader.ReadInt32();
                    PHColor.ReadColor(reader, ref this.cheekColor);
                    this.lipTexID = reader.ReadInt32();
                    PHColor.ReadColor(reader, ref this.lipColor);
                    this.moleTexID = reader.ReadInt32();
                    PHColor.ReadColor(reader, ref this.moleColor);
                    this.eyeHighlightTexID = reader.ReadInt32();
                    this.eyeHighlightColor.Load(reader, version);
                }
                else
                {
                    this.beardID = reader.ReadInt32();
                    PHColor.ReadColor(reader, ref this.beardColor);
                    if (version < CUSTOM_DATA_VERSION.DEBUG_02)
                        return;
                    this.eyeHighlightColor.Load(reader, version);
                }
            }
            #endregion
        }

        public class BodyParameter
        {
            #region Variables
            public PHColor.AlloyHSVOffset skinColor = new PHColor.AlloyHSVOffset();
            public PHColor.Alloy underhairColor = new PHColor.Alloy();
            public UnityColor tattooColor = UnityColor.white;
            public PHColor.AlloyHSVOffset nipColor = new PHColor.AlloyHSVOffset(true, 1f);
            public float sunburnColor_S = 1f;
            public float sunburnColor_V = 1f;
            public float sunburnColor_A = 1f;
            public PHColor.AlloyHSVOffset nailColor = new PHColor.AlloyHSVOffset();
            public PHColor.PBR1 manicureColor = new PHColor.PBR1();
            public float areolaSize = 0.7f;
            public float bustSoftness = 0.5f;
            public float bustWeight = 0.5f;
            public int bodyID;
            public int detailID;
            public float detailWeight;
            public int underhairID;
            public int tattooID;
            public float[] shapeVals;
            public int nipID;
            public int sunburnID;
            public float sunburnColor_H;
            #endregion

            #region Constructor
            public BodyParameter(SEX sex)
            {
                this.shapeVals = new float[sex == SEX.FEMALE ? 33 : 21];
                for (int i = 0; i < this.shapeVals.Length; i++)
                    this.shapeVals[i] = 0.5f;

                if (sex == SEX.FEMALE)
                    this.shapeVals[this.shapeVals.Length - 1] = 0.0f;
            }
            #endregion

            #region Methods
            public void Save(BinaryWriter writer, SEX sex)
            {
                writer.Write(this.bodyID);
                this.skinColor.Save(writer);
                writer.Write(this.detailID);
                writer.Write(this.detailWeight);
                writer.Write(this.underhairID);
                this.underhairColor.Save(writer);
                writer.Write(this.tattooID);
                PHColor.WriteColor(writer, this.tattooColor);
                writer.Write(this.shapeVals.Length);
                for (int i = 0; i < this.shapeVals.Length; i++)
                    writer.Write(this.shapeVals[i]);
                if (sex != SEX.FEMALE)
                    return;
                writer.Write(this.nipID);
                this.nipColor.Save(writer);
                writer.Write(this.sunburnID);
                writer.Write(this.sunburnColor_H);
                writer.Write(this.sunburnColor_S);
                writer.Write(this.sunburnColor_V);
                writer.Write(this.sunburnColor_A);
                this.nailColor.Save(writer);
                this.manicureColor.Save(writer);
                writer.Write(this.areolaSize);
                writer.Write(this.bustSoftness);
                writer.Write(this.bustWeight);
            }

            public void Load(BinaryReader reader, SEX sex, CUSTOM_DATA_VERSION version)
            {
                this.bodyID = reader.ReadInt32();
                this.skinColor.Load(reader, version);
                this.detailID = reader.ReadInt32();
                this.detailWeight = reader.ReadSingle();
                this.underhairID = reader.ReadInt32();
                this.underhairColor.Load(reader, version);
                this.tattooID = reader.ReadInt32();
                PHColor.ReadColor(reader, ref this.tattooColor);

                int shapeValsCount = reader.ReadInt32();
                this.shapeVals = new float[shapeValsCount];
                for (int i = 0; i < shapeValsCount; i++)
                    this.shapeVals[i] = reader.ReadSingle();

                if (sex != SEX.FEMALE)
                    return;

                this.nipID = reader.ReadInt32();
                this.nipColor.Load(reader, version);
                this.sunburnID = reader.ReadInt32();

                if (version < CUSTOM_DATA_VERSION.DEBUG_06)
                {
                    UnityColor color = UnityColor.white;
                    PHColor.ReadColor(reader, ref color);
                    this.sunburnColor_H = 0.0f;
                    this.sunburnColor_S = 1f;
                    this.sunburnColor_V = 1f;
                    this.sunburnColor_A = 1f;
                }
                else
                {
                    this.sunburnColor_H = reader.ReadSingle();
                    this.sunburnColor_S = reader.ReadSingle();
                    this.sunburnColor_V = reader.ReadSingle();
                    this.sunburnColor_A = reader.ReadSingle();
                }

                if (version < CUSTOM_DATA_VERSION.DEBUG_03)
                    return;

                this.nailColor.Load(reader, version);

                if (version >= CUSTOM_DATA_VERSION.DEBUG_09)
                {
                    this.manicureColor.Load(reader, version);
                }
                else
                {
                    this.manicureColor.mainColor1 = UnityColor.white;
                    this.manicureColor.mainColor1.a = 0.0f;
                    this.manicureColor.specColor1 = UnityColor.white;
                    this.manicureColor.specular1 = 0.0f;
                    this.manicureColor.smooth1 = 0.0f;
                }

                this.areolaSize = reader.ReadSingle();
                this.bustSoftness = reader.ReadSingle();
                this.bustWeight = reader.ReadSingle();
            }
            #endregion
        }

        public class WearParameter
        {
            #region Variables
            public Wear[] wears = new Wear[11];
            public bool swimOptTop = true;
            public bool swimOptBtm = true;
            public bool isSwimwear;
            #endregion

            #region Constructor
            public WearParameter()
            {
                for (int i = 0; i < this.wears.Length; i++)
                    this.wears[i] = new Wear((WEAR_TYPE)i, 0);
            }
            #endregion

            #region Methods
            public void Save(BinaryWriter writer, SEX sex)
            {
                for (int i = 0; i < this.wears.Length; i++)
                    this.wears[i].Save(writer);
                if (sex != SEX.FEMALE)
                    return;
                writer.Write(this.isSwimwear);
                writer.Write(this.swimOptTop);
                writer.Write(this.swimOptBtm);
            }

            public void Load(BinaryReader reader, SEX sex, CUSTOM_DATA_VERSION version)
            {
                for (int i = 0; i < this.wears.Length; i++)
                    this.wears[i].Load(reader, version);
                if (sex != 0)
                    return;
                this.isSwimwear = reader.ReadBoolean();
                this.swimOptTop = reader.ReadBoolean();
                this.swimOptBtm = reader.ReadBoolean();
            }
            #endregion

            #region Nested Types
            public class Wear
            {
                #region Variables
                public WEAR_TYPE wearType = WEAR_TYPE.NUM;
                public int id = -1;
                public PHColor.PBR2 color = new PHColor.PBR2();
                #endregion

                #region Constructor
                public Wear(WEAR_TYPE wearType, int id)
                {
                    this.wearType = wearType;
                    this.id = id;
                }
                #endregion

                #region Methods
                public void Save(BinaryWriter writer)
                {
                    writer.Write((int)this.wearType);
                    writer.Write(this.id);
                    if (this.color != null)
                        this.color.Save(writer);
                    else
                        writer.Write(0);
                }

                public void Load(BinaryReader reader, CUSTOM_DATA_VERSION version)
                {
                    this.wearType = (WEAR_TYPE)reader.ReadInt32();
                    this.id = reader.Read();
                    this.color.Load(reader, version);
                }
                #endregion
            }
            #endregion
        }

        public class AccessoryParameter
        {
            #region Variables
            public Accessory[] slot = new Accessory[10];
            #endregion

            #region Constructor
            public AccessoryParameter()
            {
                for (int i = 0; i < slot.Length; i++)
                    slot[i] = new Accessory();
            }
            #endregion

            #region Methods
            public void Save(BinaryWriter writer, SEX sex)
            {
                for (int i = 0; i < this.slot.Length; i++)
                    this.slot[i].Save(writer);
            }

            public void Load(BinaryReader reader, SEX sex, CUSTOM_DATA_VERSION version)
            {
                for (int i = 0; i < this.slot.Length; i++)
                    this.slot[i].Load(reader, version);
            }
            #endregion

            #region Nested Types
            public class Accessory
            {
                #region Variables
                public ACCESSORY_TYPE accType = ACCESSORY_TYPE.NONE;
                public int id = -1;
                public ACCESSORY_ATTACH nowAttach = ACCESSORY_ATTACH.NONE;
                public Vector3 addPos = Vector3.zero;
                public Vector3 addRot = Vector3.zero;
                public Vector3 addScl = Vector3.one;
                public PHColor.PBR2 color = new PHColor.PBR2();
                #endregion

                #region Constructor
                public Accessory()
                {
                }
                #endregion

                #region Methods
                public void Save(BinaryWriter writer)
                {
                    writer.Write((int)this.accType);
                    writer.Write(this.id);
                    writer.Write((int)this.nowAttach);
                    this.Write(writer, this.addPos);
                    this.Write(writer, this.addRot);
                    this.Write(writer, this.addScl);
                    if (this.color != null)
                        this.color.Save(writer);
                    else
                        writer.Write(0);
                }

                public void Load(BinaryReader reader, CUSTOM_DATA_VERSION version)
                {
                    this.accType = (ACCESSORY_TYPE)reader.ReadInt32();
                    this.id = reader.ReadInt32();
                    this.nowAttach = (ACCESSORY_ATTACH)reader.ReadInt32();
                    this.Read(reader, ref this.addPos);
                    this.Read(reader, ref this.addRot);
                    this.Read(reader, ref this.addScl);
                    this.color.Load(reader, version);
                }

                private void Write(BinaryWriter writer, Vector3 vec)
                {
                    writer.Write(vec.x);
                    writer.Write(vec.y);
                    writer.Write(vec.z);
                }

                private void Read(BinaryReader reader, ref Vector3 vec)
                {
                    vec.x = reader.ReadSingle();
                    vec.y = reader.ReadSingle();
                    vec.z = reader.ReadSingle();
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
