using System;
using System.Collections.Generic;
using System.IO;

namespace Illusion.Card
{
    public class PHSceneCard
    {
        #region Variable
        public string Version { get; private set; }

        public string SourceFileName { get; }

        public List<PHCharaCard> CharaCards { get; }
        #endregion

        #region Constructor
        public PHSceneCard(string srcFileName)
        {
            this.SourceFileName = srcFileName;
            this.CharaCards = new List<PHCharaCard>();
        }
        #endregion

        #region Methods

        private byte[] ReadColorHsv(BinaryReader reader)
        {
            BinaryList color = new BinaryList();
            for (int i = 0; i < 9; i++)
            {
                color.Add(reader.ReadDouble());
            }

            return color.ToArray();
        }

        #region Read OI Info
        private void ReadObjectInfo(BinaryReader reader, bool other)
        {
            // 133
            reader.ReadInt32(); // dicKey
            reader.ReadString(); // ChangeAmount.pos
            reader.ReadString(); // ChangeAmount.rot
            reader.ReadString(); // ChangeAmount.scale

            if (other)
            {
                reader.ReadInt32(); // treeState
                reader.ReadBoolean(); // visible
            }
        }

        private void ReadCharFileStatus(BinaryReader reader, int version, ref string name)
        {
            reader.ReadInt32(); // coordinateType
            int countAccessory = reader.ReadInt32();
            reader.ReadBytes(countAccessory);
            reader.ReadInt32(); // eyesPtn
            reader.ReadSingle(); // eyesOpen
            reader.ReadSingle(); // eyesOpenMin
            reader.ReadSingle(); // eyesOpenMax
            reader.ReadSingle(); // eyesFixed
            reader.ReadInt32(); // mouthPtn
            reader.ReadSingle(); // mouthOpen
            reader.ReadSingle(); // mouthOpenMin
            reader.ReadSingle(); // mouthOpenMax
            reader.ReadSingle(); // mouthFixed
            reader.ReadByte(); // tongueState
            reader.ReadInt32(); // eyesLookPtn
            reader.ReadInt32(); // eyesTargetNo
            reader.ReadSingle(); // eyesTargetRate
            reader.ReadInt32(); // neckLookPtn
            reader.ReadInt32(); // neckTargetNo
            reader.ReadSingle(); // neckTargetRate
            reader.ReadBoolean(); // eyesBlink
            reader.ReadBoolean(); // disableShapeMouth

            int countClothesState = reader.ReadInt32();
            reader.ReadBytes(countClothesState);

            int countSiruLv = reader.ReadInt32();
            reader.ReadBytes(countSiruLv);

            reader.ReadSingle(); // nipStand
            reader.ReadSingle(); // hohoAkaRate
            reader.ReadSingle(); // tearsLv

            reader.ReadBoolean(); // disableShapeBustL
            reader.ReadBoolean(); // disableShapeBustR
            reader.ReadBoolean(); // disableShapeNipL
            reader.ReadBoolean(); // disableShapeNipR
            reader.ReadBoolean(); // hideEyesHighlight

            if (version >= 14)
            {
                name = reader.ReadString();
            }
            else
            {
                name = string.Empty;
            }
        }

        private void ReadChild(BinaryReader reader, int version)
        {
            int childCount = reader.ReadInt32();
            for (int i = 0; i < childCount; i++)
            {
                int infoType = reader.ReadInt32();
                switch (infoType)
                {
                    case 0:
                        ReadOICharInfo(reader, version);
                        break;
                    case 1:
                        ReadOIItemInfo(reader, version);
                        break;
                    case 2:
                        ReadOILightInfo(reader);
                        break;
                    case 3:
                        ReadOIFolderInfo(reader, version);
                        break;
                    default:
                        break;
                }
            }
        }

        private void ReadOICharInfo(BinaryReader reader, int version)
        {
            ReadObjectInfo(reader, true);

            var sex = reader.ReadInt32();

            PHCharaCard charaCard = new PHCharaCard(this.SourceFileName, "", (short)sex);
            charaCard.Parse(reader, 0L);

            string name = string.Empty;
            ReadCharFileStatus(reader, version, ref name);

            charaCard.Name = name;
            this.CharaCards.Add(charaCard);

            int countBones = reader.ReadInt32();
            for (int i = 0; i < countBones; i++)
            {
                reader.ReadInt32();
                ReadObjectInfo(reader, false);
            }

            int countIkTarget = reader.ReadInt32();
            for (int i = 0; i < countIkTarget; i++)
            {
                reader.ReadInt32();
                ReadObjectInfo(reader, false);
            }

            int countChild = reader.ReadInt32();
            for (int i = 0; i < countChild; i++)
            {
                reader.ReadInt32();
                ReadChild(reader, version);
            }

            reader.ReadInt32(); // kinematicMode
            reader.ReadInt32(); // animeInfo.group
            reader.ReadInt32(); // animeInfo.category
            reader.ReadInt32(); // animeInfo.no

            reader.ReadInt32(); // handPtnL
            reader.ReadInt32(); // handPtnR

            reader.ReadSingle(); // skinRate
            reader.ReadSingle(); // nipple
            reader.ReadBytes(5); // siru

            if (version >= 12)
                reader.ReadInt32(); // faceOption

            reader.ReadSingle(); // mouthOpen
            reader.ReadBoolean(); // lipSync

            ReadObjectInfo(reader, false); // lookAtTarget

            reader.ReadBoolean(); // enableIK
            reader.ReadBytes(5); // activeIK

            reader.ReadBoolean(); // enableFK
            reader.ReadBytes(7); // activeFK

            reader.ReadBytes(4); // expression
            reader.ReadSingle(); // animeSpeed

            if (version < 12)
                reader.ReadSingle(); // animePattern[0]
            else
            {
                reader.ReadSingle(); // animePattern[0]
                reader.ReadSingle(); // animePattern[1]
            }

            reader.ReadBoolean(); // animeOptionVisible
            reader.ReadBoolean(); // isAnimeForceLoop

            int voiceCount = reader.ReadInt32();
            for (int i = 0; i < voiceCount; i++)
            {
                reader.ReadInt32(); // group
                reader.ReadInt32(); // category
                reader.ReadInt32(); // no
            }
            reader.ReadInt32(); // repeat

            if (sex == 0)
            {
                reader.ReadBoolean(); // visibleSimple
                reader.ReadString(); // simpleColor
                reader.ReadBoolean(); // visibleSon
                reader.ReadSingle(); // animeOptionParam[0]
                reader.ReadSingle(); // animeOptionParam[1]
            }

            int countNeckByte = reader.ReadInt32();
            reader.ReadBytes(countNeckByte);

            int countEyesByte = reader.ReadInt32();
            reader.ReadBytes(countEyesByte);

            reader.ReadSingle(); // animeNormalizedTime
            
            int countAccessGroup = reader.ReadInt32();
            for (int i = 0; i < countAccessGroup; i++)
            {
                reader.ReadInt32(); // key
                reader.ReadInt32(); // TreeState
            }

            int countAccessNo = reader.ReadInt32();
            for (int i = 0; i < countAccessNo; i++)
            {
                reader.ReadInt32(); // key
                reader.ReadInt32(); // TreeState
            }
        }

        private void ReadOIItemInfo(BinaryReader reader, int version)
        {
            ReadObjectInfo(reader, true);
            reader.ReadInt32(); // no
            reader.ReadSingle(); // animeSpeed
            reader.ReadInt32(); // colortype
            ReadColorHsv(reader); // color
            ReadColorHsv(reader); // color2
            reader.ReadBoolean(); // enableFK

            int cbone = reader.ReadInt32();
            for (int i = 0; i < cbone; i++)
            {
                reader.ReadString();
                ReadObjectInfo(reader, false);
            }

            reader.ReadSingle(); // animeNormalizedTime
            ReadChild(reader, version);
        }

        private void ReadOILightInfo(BinaryReader reader)
        {
            ReadObjectInfo(reader, true);
            reader.ReadInt32(); // no
            reader.ReadBytes(16); // color
            reader.ReadSingle(); // intensity
            reader.ReadSingle(); // range
            reader.ReadSingle(); // spotAngle
            reader.ReadBoolean(); // shadow
            reader.ReadBoolean(); // enable
            reader.ReadBoolean(); // drawTarget
        }

        private void ReadOIFolderInfo(BinaryReader reader, int version)
        {
            ReadObjectInfo(reader, true);
            reader.ReadString();
            ReadChild(reader, version);
        }
        #endregion

        public bool Parse(BinaryReader reader, long pngEnd)
        {
            reader.Seek(pngEnd, SeekOrigin.Begin);

            Version = reader.ReadString();
            int versionInt = int.Parse(Version.Replace(".", ""));

            int infoCount = reader.ReadInt32();
            for (int i = 0; i < infoCount; i++)
            {
                reader.ReadInt32(); // key
                int infoType = reader.ReadInt32();
                switch(infoType)
                {
                    case 0:
                        ReadOICharInfo(reader, versionInt);
                        break;
                    case 1:
                        ReadOIItemInfo(reader, versionInt);
                        break;
                    case 2:
                        ReadOILightInfo(reader);
                        break;
                    case 3:
                        ReadOIFolderInfo(reader, versionInt);
                        break;
                    default:
                        break;
                }
            }

            return false;
        }
        #endregion
    }
}
