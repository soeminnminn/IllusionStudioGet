using System;
using System.IO;

namespace Illusion.Card
{
    public partial class SBPRCharaCard
    {
        #region Nested Types
        public class ColorSetSBPR
        {
            #region Variables
            public HsvColor diffuseColor = new HsvColor(20f, 0.8f, 0.8f);
            public float alpha = 1f;
            public HsvColor specularColor = new HsvColor(0.0f, 0.0f, 0.8f);
            public float specularIntensity = 0.5f;
            public float specularSharpness = 3f;
            #endregion

            #region Methods
            public void Load(BinaryReader reader)
            {
                this.diffuseColor.H = (float)reader.ReadDouble();
                this.diffuseColor.S = (float)reader.ReadDouble();
                this.diffuseColor.V = (float)reader.ReadDouble();
                this.alpha = (float)reader.ReadDouble();
                this.specularColor.H = (float)reader.ReadDouble();
                this.specularColor.S = (float)reader.ReadDouble();
                this.specularColor.V = (float)reader.ReadDouble();
                this.specularIntensity = (float)reader.ReadDouble();
                this.specularSharpness = (float)reader.ReadDouble();
            }
            #endregion
        }

        public class Coordinate
        {
            #region Variables
            public int clothesTopId;
            public ColorSetSBPR clothesTopColor;
            public int clothesBotId;
            public ColorSetSBPR clothesBotColor;
            public int braId;
            public ColorSetSBPR braColor;
            public int shortsId;
            public ColorSetSBPR shortsColor;
            public int glovesId;
            public ColorSetSBPR glovesColor;
            public int panstId;
            public ColorSetSBPR panstColor;
            public int socksId;
            public ColorSetSBPR socksColor;
            public int shoesId;
            public ColorSetSBPR shoesColor;
            public int swimsuitId;
            public ColorSetSBPR swimsuitColor;
            public int swimTopId;
            public ColorSetSBPR swimTopColor;
            public int swimBotId;
            public ColorSetSBPR swimBotColor;
            #endregion
        }

        public class Accessory
        {
            #region Variables
            public int accessoryType = -1;
            public int accessoryId = -1;
            public string parentKey = string.Empty;
            public Vector3 plusPos = Vector3.zero;
            public Vector3 plusRot = Vector3.zero;
            public Vector3 plusScl = Vector3.one;
            #endregion

            #region Methods
            public void Load(BinaryReader reader)
            {
                this.accessoryType = reader.ReadInt32();
                this.accessoryId = reader.ReadInt32();
                this.parentKey = reader.ReadString();
                this.plusPos.x = (float)reader.ReadDouble();
                this.plusPos.y = (float)reader.ReadDouble();
                this.plusPos.z = (float)reader.ReadDouble();
                this.plusRot.x = (float)reader.ReadDouble();
                this.plusRot.y = (float)reader.ReadDouble();
                this.plusRot.z = (float)reader.ReadDouble();
                this.plusScl.x = (float)reader.ReadDouble();
                this.plusScl.y = (float)reader.ReadDouble();
                this.plusScl.z = (float)reader.ReadDouble();
            }
            #endregion
        }

        public class CharaCustomData
        {
            #region Variables
            public int personality;
            public string name;
            public int headId;
            public double[] sbpr_shapeFace;
            public double[] sbpr_shapeBody;
            public int[] hairId;
            public ColorSetSBPR[] hairColor;
            public ColorSetSBPR[] hairAcsColor;
            public int texFaceId;
            public ColorSetSBPR skinColor;

            public int texEyeshadowId;
            public ColorSetSBPR eyeshadowColor;
            public int texCheekId;
            public ColorSetSBPR cheekColor;
            public int texLipId;
            public ColorSetSBPR lipColor;

            public int texTattoo_fId;
            public ColorSetSBPR tattoo_fColor;

            public int texMoleId;
            public ColorSetSBPR moleColor;
            public int matEyebrowId;
            public ColorSetSBPR eyebrowColor;
            
            public int matEyelashesId;
            public ColorSetSBPR eyelashesColor;

            public int matEyeLId;
            public ColorSetSBPR eyeLColor;
            public int matEyeRId;
            public ColorSetSBPR eyeRColor;

            public int matEyeHiId;
            public ColorSetSBPR eyeHiColor;

            public ColorSetSBPR eyeWColor;

            public double faceDetailWeight;
            public int texBodyId;

            public int texSunburnId;
            public ColorSetSBPR sunburnColor;

            public int texTattoo_bId;
            public ColorSetSBPR tattoo_bColor;

            public int matNipId;
            public ColorSetSBPR nipColor;
            public int matUnderhairId;
            public ColorSetSBPR underhairColor;
            public ColorSetSBPR nailColor;
            public double nipSize;

            public double bodyDetailWeight;

            public int beardId;
            public ColorSetSBPR beardColor;

            public double bustSoftness;
            public double bustWeight;
            #endregion

            #region Constructor
            public CharaCustomData()
            { }
            #endregion

            #region Methods
            public void Load(byte[] bytes, int version, byte sex)
            {
                try
                {
                    using (var reader = new BinaryReader(new MemoryStream(bytes)))
                    {
                        byte b = reader.ReadByte();
                        this.personality = reader.ReadInt32();
                        this.name = reader.ReadString();
                        this.headId = reader.ReadInt32();

                        this.sbpr_shapeFace = new double[67];
                        if (version < 3)
                        {
                            for (int i = 0; i < 66; i++)
                                this.sbpr_shapeFace[i] = reader.ReadDouble();
                        }
                        else
                        {
                            for (int i = 0; i < this.sbpr_shapeFace.Length; i++)
                                this.sbpr_shapeFace[i] = reader.ReadDouble();
                        }

                        this.sbpr_shapeBody = new double[sex != 0 ? 32 : 21];
                        for (int i = 0; i < this.sbpr_shapeBody.Length; i++)
                            this.sbpr_shapeBody[i] = reader.ReadDouble();

                        this.hairId = new int[sex != 0 ? 4 : 1];
                        for (int i = 0; i < this.hairId.Length; i++)
                            this.hairId[i] = reader.ReadInt32();

                        this.hairColor = new ColorSetSBPR[sex != 0 ? 4 : 1];
                        for (int i = 0; i < this.hairColor.Length; i++)
                        {
                            this.hairColor[i] = new ColorSetSBPR();
                            this.hairColor[i].Load(reader);
                        }

                        this.hairAcsColor = new ColorSetSBPR[sex != 0 ? 4 : 1];
                        for (int i = 0; i < this.hairAcsColor.Length; i++)
                        {
                            this.hairAcsColor[i] = new ColorSetSBPR();
                            this.hairAcsColor[i].Load(reader);
                        }

                        this.texFaceId = reader.ReadInt32();

                        this.skinColor = new ColorSetSBPR();
                        this.skinColor.Load(reader);

                        if (sex == 1)
                        {
                            this.texEyeshadowId = reader.ReadInt32();
                            this.eyeshadowColor = new ColorSetSBPR();
                            this.eyeshadowColor.Load(reader);
                            this.texCheekId = reader.ReadInt32();
                            this.cheekColor = new ColorSetSBPR();
                            this.cheekColor.Load(reader);
                            this.texLipId = reader.ReadInt32();
                            this.lipColor = new ColorSetSBPR();
                            this.lipColor.Load(reader);
                        }

                        this.texTattoo_fId = reader.ReadInt32();
                        this.tattoo_fColor = new ColorSetSBPR();
                        this.tattoo_fColor.Load(reader);
                        
                        if (sex == 1)
                        {
                            this.texMoleId = reader.ReadInt32();
                            this.moleColor = new ColorSetSBPR();
                            this.moleColor.Load(reader);
                        }

                        this.matEyebrowId = reader.ReadInt32();
                        this.eyebrowColor = new ColorSetSBPR();
                        this.eyebrowColor.Load(reader);

                        if (sex == 1)
                        {
                            this.matEyelashesId = reader.ReadInt32();
                            this.eyelashesColor = new ColorSetSBPR();
                            this.eyelashesColor.Load(reader);
                        }

                        this.matEyeLId = reader.ReadInt32();
                        this.eyeLColor = new ColorSetSBPR();
                        this.eyeLColor.Load(reader);
                        this.matEyeRId = reader.ReadInt32();
                        this.eyeRColor = new ColorSetSBPR();
                        this.eyeRColor.Load(reader);

                        if (sex == 1)
                        {
                            this.matEyeHiId = reader.ReadInt32();
                            this.eyeHiColor = new ColorSetSBPR();
                            this.eyeHiColor.Load(reader);
                        }
                        
                        this.eyeWColor = new ColorSetSBPR();
                        this.eyeWColor.Load(reader);
                        
                        if (version >= 2)
                            this.faceDetailWeight = (float)reader.ReadDouble();
                        
                        this.texBodyId = reader.ReadInt32();
                        
                        if (sex == 1)
                        {
                            this.texSunburnId = reader.ReadInt32();
                            this.sunburnColor = new ColorSetSBPR();
                            this.sunburnColor.Load(reader);
                        }

                        this.texTattoo_bId = reader.ReadInt32();
                        this.tattoo_bColor = new ColorSetSBPR();
                        this.tattoo_bColor.Load(reader);
                        
                        if (sex == 1)
                        {
                            this.matNipId = reader.ReadInt32();
                            this.nipColor = new ColorSetSBPR();
                            this.nipColor.Load(reader);
                            this.matUnderhairId = reader.ReadInt32();
                            this.underhairColor = new ColorSetSBPR();
                            this.underhairColor.Load(reader);
                            this.nailColor = new ColorSetSBPR();
                            this.nailColor.Load(reader);
                            
                            if (version >= 1)
                                this.nipSize = (float)reader.ReadDouble();
                        }

                        if (version >= 2)
                        {
                            this.bodyDetailWeight = (float)reader.ReadDouble();
                        }

                        if (sex == 0)
                        {
                            this.beardId = reader.ReadInt32();
                            this.beardColor = new ColorSetSBPR();
                            this.beardColor.Load(reader);
                        }
                        else
                        {
                            if (version >= 4)
                                reader.BaseStream.Seek(4L, SeekOrigin.Current);
                            
                            if (version >= 5)
                            {
                                this.bustSoftness = (float)reader.ReadDouble();
                                this.bustWeight = (float)reader.ReadDouble();
                            }
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
                throw new NotImplementedException();
            }
            #endregion
        }

        public class CharaClothesData
        {
            #region Variables
            public Coordinate[] coord;
            public byte stateSwimOptTop;
            public byte stateSwimOptBot;
            public Accessory[,] accessory;
            public ColorSetSBPR[,] accessoryColor;
            #endregion

            #region Methods
            public void Load(byte[] bytes, int version, byte sex)
            {
                try
                {
                    using (var reader = new BinaryReader(new MemoryStream(bytes)))
                    {
                        reader.ReadByte();

                        this.coord = new Coordinate[2];
                        for (int i = 0; i < this.coord.Length; i++)
                        {
                            this.coord[i] = new Coordinate();
                            if (sex == 0)
                            {
                                this.coord[i].clothesTopId = reader.ReadInt32();
                                this.coord[i].shoesId = reader.ReadInt32();
                                if (version >= 2)
                                {
                                    this.coord[i].clothesTopColor = new ColorSetSBPR();
                                    this.coord[i].clothesTopColor.Load(reader);
                                    this.coord[i].shoesColor = new ColorSetSBPR();
                                    this.coord[i].shoesColor.Load(reader);
                                }
                            }
                            else
                            {
                                this.coord[i].clothesTopId = reader.ReadInt32();
                                this.coord[i].clothesBotId = reader.ReadInt32();
                                this.coord[i].braId = reader.ReadInt32();
                                this.coord[i].shortsId = reader.ReadInt32();
                                this.coord[i].glovesId = reader.ReadInt32();
                                this.coord[i].panstId = reader.ReadInt32();
                                this.coord[i].socksId = reader.ReadInt32();
                                this.coord[i].shoesId = reader.ReadInt32();
                                this.coord[i].swimsuitId = reader.ReadInt32();
                                this.coord[i].swimTopId = reader.ReadInt32();
                                this.coord[i].swimBotId = reader.ReadInt32();
                                
                                if (version >= 2)
                                {
                                    this.coord[i].clothesTopColor = new ColorSetSBPR();
                                    this.coord[i].clothesTopColor.Load(reader);
                                    this.coord[i].clothesBotColor = new ColorSetSBPR();
                                    this.coord[i].clothesBotColor.Load(reader);
                                    this.coord[i].braColor = new ColorSetSBPR();
                                    this.coord[i].braColor.Load(reader);
                                    this.coord[i].shortsColor = new ColorSetSBPR();
                                    this.coord[i].shortsColor.Load(reader);
                                    this.coord[i].glovesColor = new ColorSetSBPR();
                                    this.coord[i].glovesColor.Load(reader);
                                    this.coord[i].panstColor = new ColorSetSBPR();
                                    this.coord[i].panstColor.Load(reader);
                                    this.coord[i].socksColor = new ColorSetSBPR();
                                    this.coord[i].socksColor.Load(reader);
                                    this.coord[i].shoesColor = new ColorSetSBPR();
                                    this.coord[i].shoesColor.Load(reader);
                                    this.coord[i].swimsuitColor = new ColorSetSBPR();
                                    this.coord[i].swimsuitColor.Load(reader);
                                    this.coord[i].swimTopColor = new ColorSetSBPR();
                                    this.coord[i].swimTopColor.Load(reader);
                                    this.coord[i].swimBotColor = new ColorSetSBPR();
                                    this.coord[i].swimBotColor.Load(reader);
                                }
                            }
                        }

                        if (sex == 0)
                        {
                            if (version >= 2) reader.ReadByte();
                        }
                        else if (version >= 1)
                        {
                            this.stateSwimOptTop = reader.ReadByte();
                            this.stateSwimOptBot = reader.ReadByte();
                            reader.ReadByte();
                        }

                        this.accessory = new Accessory[2, 5];
                        this.accessoryColor = new ColorSetSBPR[2, 5];
                        for (int i = 0; i < 2; i++)
                        {
                            for (int j = 0; j < 5; j++)
                            {
                                this.accessory[i, j] = new Accessory();
                                this.accessory[i, j].Load(reader);
                                if (version >= 2)
                                {
                                    this.accessoryColor[i, j] = new ColorSetSBPR();
                                    this.accessoryColor[i, j].Load(reader);
                                }
                            }
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
                throw new NotImplementedException();
            }
            #endregion
        }
        #endregion
    }
}
