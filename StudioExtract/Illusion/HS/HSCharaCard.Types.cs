using System;
using System.IO;
using System.Text;

namespace Illusion.Card
{
    public partial class HSCharaCard
    {
        #region Nested Types
        public class CharaInfoPreview
        {
            #region Variables
            public readonly string tagName = "プレビュー情報";
            public readonly int defVersion = 4;

            public int productNo = 11;
            public int sex = 0;
            public int personality = byte.MaxValue;
            public int nameLength = 0;
            public string name = string.Empty;
            public int height = byte.MaxValue;
            public int bustSize = byte.MaxValue;
            public int hairType = byte.MaxValue;
            public int state = byte.MaxValue;
            public int resistH = byte.MaxValue;
            public int resistPain = byte.MaxValue;
            public int resistAnal = byte.MaxValue;
            public int isConcierge = 0;
            #endregion

            #region Constructor
            public CharaInfoPreview()
            { }
            #endregion

            #region Methods
            public void Load(byte[] bytes, int version)
            { 
                try
                {
                    using (var reader = new BinaryReader(new MemoryStream(bytes)))
                    {
                        if (4 <= version)
                        {
                            productNo = reader.ReadInt32();
                        }
                        sex = reader.ReadInt32();
                        if (2 <= version)
                        {
                            personality = reader.ReadInt32();
                            nameLength = reader.ReadInt32();
                            name = reader.ReadString();
                            height = reader.ReadInt32();
                            bustSize = reader.ReadInt32();
                            hairType = reader.ReadInt32();
                        }
                        if (4 <= version)
                        {
                            state = reader.ReadInt32();
                            resistH = reader.ReadInt32();
                            resistPain = reader.ReadInt32();
                            resistAnal = reader.ReadInt32();
                        }
                        if (3 <= version)
                        {
                            isConcierge = reader.ReadInt32();
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }

            public void Save(BinaryWriter writer)
            {
                writer.Write(11); // productNo
                writer.Write(this.sex);
                writer.Write(this.personality);
                int byteCount = Encoding.UTF8.GetByteCount(this.name);
                writer.Write(byteCount);
                writer.Write(this.name);
                writer.Write(this.height);
                writer.Write(this.bustSize);
                writer.Write(this.hairType);
                writer.Write(this.state);
                writer.Write(this.resistH);
                writer.Write(this.resistPain);
                writer.Write(this.resistAnal);
                writer.Write(this.isConcierge);
            }
            #endregion
        }

        public class CharaInfoCustom
        {
            #region Variables
            public readonly string tagName = "カスタム情報";
            public readonly int defVersion = 4;
            public readonly int sex;

            public int productNo = 1;
            public float[] shapeValueFace;
            public float[] shapeValueBody;
            public Hair[] hair;
            public int hairType;
            public int headId = 0;
            public HSColorSet skinColor = HSColorSet.White;
            public int texFaceId = 0;
            public int texTattoo_fId = 0;
            public HSColorSet tattoo_fColor = HSColorSet.DiffuseWhite;
            public int matEyebrowId = 0;
            public HSColorSet eyebrowColor = HSColorSet.DiffuseWhite;
            public int matEyeLId = 0;
            public HSColorSet eyeLColor = HSColorSet.DiffuseWhite;
            public int matEyeRId = 0;
            public HSColorSet eyeRColor = HSColorSet.DiffuseWhite;
            public HSColorSet eyeWColor = HSColorSet.DiffuseWhite;
            public int texFaceDetailId = 0;
            public float faceDetailWeight = 0.5f;
            public int texBodyId = 0;
            public int texTattoo_bId = 0;
            public HSColorSet tattoo_bColor = HSColorSet.DiffuseWhite;
            public int texBodyDetailId = 0;
            public float bodyDetailWeight = 0.0f;
            public string name = string.Empty;
            public int personality;
            public float voicePitch = 1f;
            public bool isConcierge = false;

            // For Male
            public HSColorSet beardColor = HSColorSet.DiffuseWhite;
            public int matBeardId = 0;

            // For Female
            public int texEyeshadowId = 0;
            public HSColorSet eyeshadowColor = HSColorSet.DiffuseWhite;
            public int texCheekId = 0;
            public HSColorSet cheekColor = HSColorSet.DiffuseWhite;
            public int texLipId = 0;
            public HSColorSet lipColor = HSColorSet.DiffuseWhite;
            public int texMoleId = 0;
            public HSColorSet moleColor = HSColorSet.DiffuseWhite;
            public int matEyelashesId = 0;
            public HSColorSet eyelashesColor = HSColorSet.DiffuseWhite;
            public int matEyeHiId = 0;
            public HSColorSet eyeHiColor = HSColorSet.DiffuseWhite;
            public int texSunburnId = 0;
            public HSColorSet sunburnColor = HSColorSet.DiffuseWhite;
            public int matNipId = 0;
            public HSColorSet nipColor = HSColorSet.DiffuseWhite;
            public int matUnderhairId = 0;
            public HSColorSet underhairColor = HSColorSet.DiffuseWhite;
            public HSColorSet nailColor = HSColorSet.DiffuseWhite;
            public float areolaSize = 0.5f;
            public float bustSoftness = 0.5f;
            public float bustWeight = 0.5f;
            #endregion

            #region Constructor
            public CharaInfoCustom(short sex)
            {
                this.sex = sex;

                int[] hairArr;
                if (sex == 0)
                {
                    beardColor.hsvDiffuse.S = 1f;
                    personality = 99;
                    name = "俺";

                    shapeValueFace = new float[67];
                    shapeValueBody = new float[21];
                    hairArr = new int[] { 0 };
                }
                else
                {
                    eyelashesColor.hsvDiffuse.S = 1f;
                    nipColor.hsvDiffuse.S = 1f;
                    underhairColor.hsvDiffuse.S = 1f;
                    nailColor.hsvDiffuse.S = 1f;
                    name = "カスタム娘";
                    personality = 0;

                    shapeValueFace = new float[67];
                    shapeValueBody = new float[32];
                    hairArr = new int[] { 0, 1, 0, 0 };
                }

                for (int i = 0; i < shapeValueFace.Length; i++)
                {
                    shapeValueFace[i] = 0.5f;
                }

                for (int i = 0; i < shapeValueBody.Length; i++)
                {
                    shapeValueBody[i] = 0.5f;
                }

                hair = new Hair[hairArr.Length];
                for (int i = 0; i < hairArr.Length; i++)
                {
                    hair[i] = new Hair()
                    {
                        id = hairArr[i],
                        color = HSColorSet.HairColor,
                        acsColor = HSColorSet.HairAcsColor
                    };
                }
            }
            #endregion

            #region Methods
            public void Load(byte[] bytes, int version)
            {
                try
                {
                    using (var reader = new BinaryReader(new MemoryStream(bytes)))
                    {
                        productNo = reader.ReadInt32();

                        int faceShapeCount = reader.ReadInt32();
                        shapeValueFace = new float[faceShapeCount];
                        for (int i = 0; i < faceShapeCount; i++)
                        {
                            shapeValueFace[i] = reader.ReadSingle();
                        }

                        int bodyShapeCount = reader.ReadInt32();
                        shapeValueBody = new float[bodyShapeCount];
                        for (int i = 0; i < bodyShapeCount; i++)
                        {
                            shapeValueBody[i] = reader.ReadSingle();
                        }

                        int hairCount = reader.ReadInt32();
                        hair = new Hair[hairCount];
                        for (int i = 0; i < hairCount; i++)
                        {
                            hair[i] = new Hair();
                            hair[i].id = reader.ReadInt32();
                            hair[i].color.Load(reader);
                            hair[i].acsColor.Load(reader);
                        }

                        if (2 <= version)
                            hairType = reader.ReadInt32();

                        this.headId = reader.ReadInt32();
                        this.skinColor.Load(reader);
                        this.texFaceId = reader.ReadInt32();
                        this.texTattoo_fId = reader.ReadInt32();
                        this.tattoo_fColor.Load(reader);
                        this.matEyebrowId = reader.ReadInt32();
                        this.eyebrowColor.Load(reader);
                        this.matEyeLId = reader.ReadInt32();
                        this.eyeLColor.Load(reader);
                        this.matEyeRId = reader.ReadInt32();
                        this.eyeRColor.Load(reader);
                        this.eyeWColor.Load(reader);
                        this.texFaceDetailId = reader.ReadInt32();
                        this.faceDetailWeight = reader.ReadSingle();
                        this.texBodyId = reader.ReadInt32();
                        this.texTattoo_bId = reader.ReadInt32();
                        this.tattoo_bColor.Load(reader);
                        this.texBodyDetailId = reader.ReadInt32();
                        this.bodyDetailWeight = reader.ReadSingle();
                        this.name = reader.ReadString();
                        this.personality = reader.ReadInt32();
                        if (3 <= version)
                            this.voicePitch = reader.ReadSingle();
                        if (4 <= version)
                            this.isConcierge = reader.ReadBoolean();

                        if (this.sex == 0)
                        {
                            this.matBeardId = reader.ReadInt32();
                            this.beardColor.Load(reader);
                        }
                        else
                        {
                            this.texEyeshadowId = reader.ReadInt32();
                            this.eyeshadowColor.Load(reader);
                            this.texCheekId = reader.ReadInt32();
                            this.cheekColor.Load(reader);
                            this.texLipId = reader.ReadInt32();
                            this.lipColor.Load(reader);
                            this.texMoleId = reader.ReadInt32();
                            this.moleColor.Load(reader);
                            this.matEyelashesId = reader.ReadInt32();
                            this.eyelashesColor.Load(reader);
                            this.matEyeHiId = reader.ReadInt32();
                            this.eyeHiColor.Load(reader);
                            this.texSunburnId = reader.ReadInt32();
                            this.sunburnColor.Load(reader);
                            this.matNipId = reader.ReadInt32();
                            this.nipColor.Load(reader);
                            this.matUnderhairId = reader.ReadInt32();
                            this.underhairColor.Load(reader);
                            this.nailColor.Load(reader);
                            this.areolaSize = reader.ReadSingle();
                            this.bustSoftness = reader.ReadSingle();
                            this.bustWeight = reader.ReadSingle();
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }

            public void Save(BinaryWriter writer)
            {
                writer.Write(1);
                
                writer.Write(this.shapeValueFace.Length);
                for (int i = 0; i < this.shapeValueFace.Length; i++)
                {
                    writer.Write(this.shapeValueFace[i]);
                }

                writer.Write(this.shapeValueBody.Length);
                for (int i = 0; i < this.shapeValueBody.Length; i++)
                {
                    writer.Write(this.shapeValueBody[i]);
                }

                writer.Write(this.hair.Length);
                for (int i = 0; i < this.hair.Length; i++)
                {
                    writer.Write(this.hair[i].id);
                    this.hair[i].color.Save(writer);
                    this.hair[i].acsColor.Save(writer);
                }

                writer.Write(this.hairType);
                writer.Write(this.headId);
                this.skinColor.Save(writer);
                writer.Write(this.texFaceId);
                writer.Write(this.texTattoo_fId);
                this.tattoo_fColor.Save(writer);
                writer.Write(this.matEyebrowId);
                this.eyebrowColor.Save(writer);
                writer.Write(this.matEyeLId);
                this.eyeLColor.Save(writer);
                writer.Write(this.matEyeRId);
                this.eyeRColor.Save(writer);
                this.eyeWColor.Save(writer);
                writer.Write(this.texFaceDetailId);
                writer.Write(this.faceDetailWeight);
                writer.Write(this.texBodyId);
                writer.Write(this.texTattoo_bId);
                this.tattoo_bColor.Save(writer);
                writer.Write(this.texBodyDetailId);
                writer.Write(this.bodyDetailWeight);
                writer.Write(this.name);
                writer.Write(this.personality);
                writer.Write(this.voicePitch);
                writer.Write(this.isConcierge);

                if (sex == 0)
                {
                    writer.Write(this.matBeardId);
                    this.beardColor.Save(writer);
                }
                else
                {
                    writer.Write(this.texEyeshadowId);
                    this.eyeshadowColor.Save(writer);
                    writer.Write(this.texCheekId);
                    this.cheekColor.Save(writer);
                    writer.Write(this.texLipId);
                    this.lipColor.Save(writer);
                    writer.Write(this.texMoleId);
                    this.moleColor.Save(writer);
                    writer.Write(this.matEyelashesId);
                    this.eyelashesColor.Save(writer);
                    writer.Write(this.matEyeHiId);
                    this.eyeHiColor.Save(writer);
                    writer.Write(this.texSunburnId);
                    this.sunburnColor.Save(writer);
                    writer.Write(this.matNipId);
                    this.nipColor.Save(writer);
                    writer.Write(this.matUnderhairId);
                    this.underhairColor.Save(writer);
                    this.nailColor.Save(writer);
                    writer.Write(this.areolaSize);
                    writer.Write(this.bustSoftness);
                    writer.Write(this.bustWeight);
                }
            }
            #endregion

            #region Nested Types
            public class Hair
            {
                #region Variables
                public int id = 0;
                public HSColorSet color = new HSColorSet();
                public HSColorSet acsColor = new HSColorSet();
                #endregion
            }
            #endregion
        }
        
        public class CharaInfoCoordinate
        {
            #region Variables
            public readonly string tagName = "コーディネート情報";
            public readonly int defVersion = 3;
            public readonly int sex;

            public Clothes[] clothes;
            #endregion

            #region Constructor
            public CharaInfoCoordinate(short sex)
            {
                this.sex = sex;
            }
            #endregion

            #region Methods
            public void Load(byte[] bytes, int version)
            { 
                try
                {
                    using (var reader = new BinaryReader(new MemoryStream(bytes)))
                    {
                        int count = reader.ReadInt32();

                        clothes = new Clothes[count];
                        for (int i = 0; i < count; i++)
                        {
                            clothes[i] = new Clothes();
                            clothes[i].id = reader.ReadInt32();
                            clothes[i].clothes = new HSCoordinateCard("", (short)sex);
                            clothes[i].clothes.Parse(reader, 0);
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }

            public void Save(BinaryWriter writer)
            {
                writer.Write(clothes.Length);
                for (int i = 0; i < clothes.Length; i++)
                {
                    writer.Write(clothes[i].id);
                    clothes[i].clothes.Save(writer);
                }
            }
            #endregion

            #region Nested Types
            public class Clothes
            {
                #region Variables
                public int id;
                public HSCoordinateCard clothes;
                #endregion
            }
            #endregion
        }

        public class CharaInfoStatus
        {
            #region Variables
            public readonly string tagName = "ステータス情報";
            public readonly int defVersion = 4;
            public readonly int sex;

            public int coordinateType;
            public bool[] showAccessory;
            public int eyesPtn;
            public float eyesOpen = 1f;
            public float eyesOpenMin;
            public float eyesOpenMax = 1f;
            public float eyesFixed = -1f;
            public int mouthPtn;
            public float mouthOpen;
            public float mouthOpenMin;
            public float mouthOpenMax = 1f;
            public float mouthFixed = -1f;
            public byte tongueState;
            public int eyesLookPtn;
            public int eyesTargetNo;
            public float eyesTargetRate = 1f;
            public int neckLookPtn;
            public int neckTargetNo;
            public float neckTargetRate = 1f;
            public bool eyesBlink = true;
            public bool disableShapeMouth;
            public byte[] clothesState;

            // Male
            public bool visibleSon = true;
            public bool visibleSonAlways = true;
            public byte visibleBodyAlways = 1;
            public UnityColor simpleColor = new UnityColor(0.188f, 0.286f, 0.8f, 0.5f);
            public bool visibleSimple;

            // Female
            public byte[] siruLv;
            public float nipStand;
            public float hohoAkaRate;
            public byte tearsLv;
            public bool disableShapeBustL;
            public bool disableShapeBustR;
            public bool disableShapeNipL;
            public bool disableShapeNipR;
            public bool hideEyesHighlight;
            #endregion

            #region Constructor
            public CharaInfoStatus(short sex)
            {
                this.sex = sex;

                this.showAccessory = new bool[10];

                if (sex == 0)
                {
                    clothesState = new byte[2];
                }
                else
                {
                    clothesState = new byte[12];
                }
            }
            #endregion

            #region Methods
            public void Load(byte[] bytes, int version)
            {
                try
                {
                    using (var reader = new BinaryReader(new MemoryStream(bytes)))
                    {
                        this.coordinateType = reader.ReadInt32();
                        int accCount = reader.ReadInt32();
                        for (int i = 0; i < accCount; i++)
                            this.showAccessory[i] = reader.ReadBoolean();
                        this.eyesPtn = reader.ReadInt32();
                        this.eyesOpen = reader.ReadSingle();
                        if (4 <= version)
                        {
                            this.eyesOpenMin = reader.ReadSingle();
                            this.eyesOpenMax = reader.ReadSingle();
                        }
                        this.eyesFixed = reader.ReadSingle();
                        this.mouthPtn = reader.ReadInt32();
                        this.mouthOpen = reader.ReadSingle();
                        if (4 <= version)
                        {
                            this.mouthOpenMin = reader.ReadSingle();
                            this.mouthOpenMax = reader.ReadSingle();
                        }
                        this.mouthFixed = reader.ReadSingle();
                        this.tongueState = reader.ReadByte();
                        this.eyesLookPtn = reader.ReadInt32();
                        this.eyesTargetNo = reader.ReadInt32();
                        this.eyesTargetRate = reader.ReadSingle();
                        this.neckLookPtn = reader.ReadInt32();
                        this.neckTargetNo = reader.ReadInt32();
                        this.neckTargetRate = reader.ReadSingle();
                        this.eyesBlink = reader.ReadBoolean();
                        if (2 <= version)
                            this.disableShapeMouth = reader.ReadBoolean();

                        if (sex == 0)
                        {
                            this.visibleSon = reader.ReadBoolean();
                            this.visibleSonAlways = reader.ReadBoolean();
                            this.visibleBodyAlways = reader.ReadByte();

                            if (2 <= version)
                            {
                                this.visibleSimple = reader.ReadBoolean();
                                this.simpleColor.r = reader.ReadSingle();
                                this.simpleColor.g = reader.ReadSingle();
                                this.simpleColor.b = reader.ReadSingle();
                                this.simpleColor.a = reader.ReadSingle();
                            }

                            int stateLength = reader.ReadInt32();
                            this.clothesState = reader.ReadBytes(stateLength);
                        }
                        else
                        {
                            int stateLength = reader.ReadInt32();
                            this.clothesState = reader.ReadBytes(stateLength);

                            int siruLength = reader.ReadInt32();
                            this.siruLv = reader.ReadBytes(siruLength);

                            this.nipStand = reader.ReadSingle();
                            this.hohoAkaRate = reader.ReadSingle();
                            this.tearsLv = reader.ReadByte();
                            this.disableShapeBustL = reader.ReadBoolean();
                            if (3 <= version)
                            {
                                this.disableShapeBustR = reader.ReadBoolean();
                                this.disableShapeNipL = reader.ReadBoolean();
                                this.disableShapeNipR = reader.ReadBoolean();
                            }
                            this.hideEyesHighlight = reader.ReadBoolean();
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }

            public void Save(BinaryWriter writer)
            {
                writer.Write(this.coordinateType);
                writer.Write(this.showAccessory.Length);
                for (int i = 0; i < this.showAccessory.Length; i++)
                    writer.Write(this.showAccessory[i]);
                writer.Write(this.eyesPtn);
                writer.Write(this.eyesOpen);
                writer.Write(this.eyesOpenMin);
                writer.Write(this.eyesOpenMax);
                writer.Write(this.eyesFixed);
                writer.Write(this.mouthPtn);
                writer.Write(this.mouthOpen);
                writer.Write(this.mouthOpenMin);
                writer.Write(this.mouthOpenMax);
                writer.Write(this.mouthFixed);
                writer.Write(this.tongueState);
                writer.Write(this.eyesLookPtn);
                writer.Write(this.eyesTargetNo);
                writer.Write(this.eyesTargetRate);
                writer.Write(this.neckLookPtn);
                writer.Write(this.neckTargetNo);
                writer.Write(this.neckTargetRate);
                writer.Write(this.eyesBlink);
                writer.Write(this.disableShapeMouth);

                if (sex == 0)
                {
                    writer.Write(this.visibleSon);
                    writer.Write(this.visibleSonAlways);
                    writer.Write(this.visibleBodyAlways);
                    writer.Write(this.visibleSimple);
                    writer.Write(this.simpleColor.r);
                    writer.Write(this.simpleColor.g);
                    writer.Write(this.simpleColor.b);
                    writer.Write(this.simpleColor.a);
                    writer.Write(this.clothesState.Length);
                    writer.Write(this.clothesState);
                }
                else
                {
                    writer.Write(this.clothesState.Length);
                    writer.Write(this.clothesState);
                    writer.Write(this.siruLv.Length);
                    writer.Write(this.siruLv);
                    writer.Write(this.nipStand);
                    writer.Write(this.hohoAkaRate);
                    writer.Write(this.tearsLv);
                    writer.Write(this.disableShapeBustL);
                    writer.Write(this.disableShapeBustR);
                    writer.Write(this.disableShapeNipL);
                    writer.Write(this.disableShapeNipR);
                    writer.Write(this.hideEyesHighlight);
                }
            }
            #endregion
        }

        public class CharaInfoParameter
        {
            #region Variables
            public readonly string tagName = "パラメータ情報";
            public readonly int defVersion = 5;
            public readonly int sex;

            public int favor;
            public int lewdness;
            public int aversion;
            public int slavery;
            public int broken;
            public int nowState;
            public bool lockNowState;
            public bool lockBroken;
            public int dirty;
            public int tiredness;
            public int toilette;
            public int libido;
            public int alertness;
            public int calcState;
            public byte escapeFlag;
            public bool escapeExperienced;
            public bool firstHFlag;
            public int hCount;
            public int[] map;
            public int resistH;
            public int resistPain;
            public int resistAnal;
            public int usedItem;
            public int characteristic;
            public int impression;
            public int attribute;
            public bool[][] genericVoice = new bool[4][];
            public bool genericFlag = true;
            public bool genericBefore = true;
            public bool[] inviteVoice = new bool[5];
            public bool initParameter = true;
            #endregion

            #region Constructor
            public CharaInfoParameter(short sex)
            {
                this.sex = sex;

                for (int i = 0; i < this.genericVoice.Length; i++)
                {
                    this.genericVoice[i] = new bool[3];
                }
            }
            #endregion

            #region Methods
            public void Load(byte[] bytes, int version)
            {
                try
                {
                    using (var reader = new BinaryReader(new MemoryStream(bytes)))
                    {
                        if (sex == 1)
                        {
                            if (2 <= version)
                            {
                                this.favor = reader.ReadInt32();
                                this.lewdness = reader.ReadInt32();
                                this.aversion = reader.ReadInt32();
                                this.slavery = reader.ReadInt32();
                                this.broken = reader.ReadInt32();
                                this.nowState = reader.ReadInt32();
                                this.lockNowState = reader.ReadBoolean();
                                this.lockBroken = reader.ReadBoolean();
                                this.dirty = reader.ReadInt32();
                                this.tiredness = reader.ReadInt32();
                                this.toilette = reader.ReadInt32();
                                this.libido = reader.ReadInt32();
                                this.alertness = reader.ReadInt32();
                                this.calcState = reader.ReadInt32();
                                this.escapeFlag = reader.ReadByte();
                                if (4 <= version)
                                    this.escapeExperienced = reader.ReadBoolean();
                                this.firstHFlag = reader.ReadBoolean();
                                this.hCount = reader.ReadInt32();

                                int mapCount = reader.ReadInt32();
                                this.map = new int[mapCount];
                                for (int i = 0; i < mapCount; i++)
                                    this.map[i] = reader.ReadInt32();

                                this.resistH = reader.ReadInt32();
                                this.resistPain = reader.ReadInt32();
                                this.resistAnal = reader.ReadInt32();
                                this.usedItem = reader.ReadInt32();
                                this.characteristic = reader.ReadInt32();
                                this.impression = reader.ReadInt32();
                                this.attribute = reader.ReadInt32();
                            }
                            if (3 <= version)
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    for (int j = 0; j < 3; j++)
                                        this.genericVoice[i][j] = reader.ReadBoolean();
                                }
                                this.genericFlag = reader.ReadBoolean();
                            }
                            if (4 <= version)
                            {
                                this.genericBefore = reader.ReadBoolean();
                                for (int i = 0; i < 5; i++)
                                    this.inviteVoice[i] = reader.ReadBoolean();
                            }
                            if (5 <= version)
                                this.initParameter = reader.ReadBoolean();
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }

            public void Save(BinaryWriter writer)
            {
                if (sex == 1)
                {
                    writer.Write(this.favor);
                    writer.Write(this.lewdness);
                    writer.Write(this.aversion);
                    writer.Write(this.slavery);
                    writer.Write(this.broken);
                    writer.Write((int)this.nowState);
                    writer.Write(this.lockNowState);
                    writer.Write(this.lockBroken);
                    writer.Write(this.dirty);
                    writer.Write(this.tiredness);
                    writer.Write(this.toilette);
                    writer.Write(this.libido);
                    writer.Write(this.alertness);
                    writer.Write((int)this.calcState);
                    writer.Write(this.escapeFlag);
                    writer.Write(this.escapeExperienced);
                    writer.Write(this.firstHFlag);
                    writer.Write(this.hCount);
                    writer.Write(this.map.Length);
                    for (int i = 0; i < this.map.Length; i++)
                        writer.Write(this.map[i]);
                    writer.Write(this.resistH);
                    writer.Write(this.resistPain);
                    writer.Write(this.resistAnal);
                    writer.Write(this.usedItem);
                    writer.Write(this.characteristic);
                    writer.Write(this.impression);
                    writer.Write(this.attribute);
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 3; j++)
                            writer.Write(this.genericVoice[i][j]);
                    }
                    writer.Write(this.genericFlag);
                    writer.Write(this.genericBefore);
                    for (int i = 0; i < 5; i++)
                        writer.Write(this.inviteVoice[i]);
                    writer.Write(this.initParameter);
                }
            }
            #endregion
        }
        #endregion
    }
}
