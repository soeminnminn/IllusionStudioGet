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

        #region Read OI Info
        private void ReadObjectInfo(BinaryReader reader, bool other)
        {
            // 133
            reader.ReadBytes(4); // dicKey
            reader.ReadString(); // ChangeAmount.pos
            reader.ReadString(); // ChangeAmount.rot
            reader.ReadString(); // ChangeAmount.scale

            if (other)
            {
                // treeState, visible
                reader.ReadBytes(5);
            }
        }

        private void ReadCharFileStatus(BinaryReader reader, int version, ref string name)
        {
            reader.ReadBytes(4); // coordinateType
            int countAccessory = reader.ReadInt32();
            reader.ReadBytes(countAccessory);

            // eyesPtn, eyesOpen, eyesOpenMin, eyesOpenMax, eyesFixed
            // mouthPtn, mouthOpen, mouthOpenMin, mouthOpenMax, mouthFixed
            // tongueState, eyesLookPtn, eyesTargetNo, eyesTargetRate 
            // neckLookPtn, neckTargetNo, neckTargetRate, eyesBlink, disableShapeMouth
            reader.ReadBytes(67);

            int countClothesState = reader.ReadInt32();
            reader.ReadBytes(countClothesState);

            int countSiruLv = reader.ReadInt32();
            reader.ReadBytes(countSiruLv);

            // nipStand, hohoAkaRate, tearsLv
            // disableShapeBustL, disableShapeBustR, disableShapeNipL
            // disableShapeNipR, hideEyesHighlight
            reader.ReadBytes(17);

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
                reader.ReadBytes(4);
                ReadObjectInfo(reader, false);
            }

            int countIkTarget = reader.ReadInt32();
            for (int i = 0; i < countIkTarget; i++)
            {
                reader.ReadBytes(4);
                ReadObjectInfo(reader, false);
            }

            int countChild = reader.ReadInt32();
            for (int i = 0; i < countChild; i++)
            {
                reader.ReadBytes(4);
                ReadChild(reader, version);
            }

            // kinematicMode, animeInfo.group, animeInfo.category, animeInfo.no
            // handPtnL, handPtnR, skinRate nipple, siru
            reader.ReadBytes(37);

            if (version >= 12)
            {
                // faceOption
                reader.ReadBytes(4); 
            }

            // mouthOpen, lipSync
            reader.ReadBytes(5);

            ReadObjectInfo(reader, false); // lookAtTarget

            // enableIK, activeIK, enableFK, activeFK
            // expression, animeSpeed
            reader.ReadBytes(22);

            if (version < 12) 
            {
                // animePattern[0]
                reader.ReadBytes(4);
            }
            else
            {
                // animePattern[0], animePattern[1]
                reader.ReadBytes(8);
            }

            // animeOptionVisible, isAnimeForceLoop
            reader.ReadBytes(2);

            int voiceCount = reader.ReadInt32();
            for (int i = 0; i < voiceCount; i++)
            {
                // group, category, no
                reader.ReadBytes(12);
            }
            reader.ReadBytes(4); // repeat

            if (sex == 0)
            {
                reader.ReadBoolean(); // visibleSimple
                reader.ReadString(); // simpleColor
                // visibleSon, animeOptionParam[0], animeOptionParam[1]
                reader.ReadBytes(9);
            }

            int countNeckByte = reader.ReadInt32();
            reader.ReadBytes(countNeckByte);

            int countEyesByte = reader.ReadInt32();
            reader.ReadBytes(countEyesByte);

            reader.ReadBytes(4); // animeNormalizedTime

            int countAccessGroup = reader.ReadInt32();
            for (int i = 0; i < countAccessGroup; i++)
            {
                // key, TreeState
                reader.ReadBytes(8);
            }

            int countAccessNo = reader.ReadInt32();
            for (int i = 0; i < countAccessNo; i++)
            {
                // key, TreeState
                reader.ReadBytes(8);
            }
        }

        private void ReadOIItemInfo(BinaryReader reader, int version)
        {
            ReadObjectInfo(reader, true);
            // no, animeSpeed, colortype, color, color2, enableFK
            reader.ReadBytes(157);  

            int cbone = reader.ReadInt32();
            for (int i = 0; i < cbone; i++)
            {
                reader.ReadString();
                ReadObjectInfo(reader, false);
            }

            reader.ReadBytes(4); // animeNormalizedTime
            ReadChild(reader, version);
        }

        private void ReadOILightInfo(BinaryReader reader)
        {
            ReadObjectInfo(reader, true);
            // no, color, intensity, range, spotAngle, shadow, enable, drawTarget
            reader.ReadBytes(35);
        }

        private void ReadOIFolderInfo(BinaryReader reader, int version)
        {
            ReadObjectInfo(reader, true);
            // name
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
