using System;
using System.Collections.Generic;
using System.IO;

namespace Illusion.Card
{
    public class PHSceneCard : ISceneCard
    {
        #region Variable
        public byte[] PngData { get; set; }

        public Version Version { get; private set; }

        public string SourceFileName { get; }

        public List<ICharaCard> CharaCards { get; }
        #endregion

        #region Constructor
        public PHSceneCard(string srcFileName)
        {
            this.SourceFileName = srcFileName;
            this.CharaCards = new List<ICharaCard>();
        }
        #endregion

        #region Methods

        private Version VersionOf(int major, int minor, int build, int revision) => new Version(major, minor, build, revision);

        private Version VersionOf(int major, int minor, int build) => new Version(major, minor, build);

        #region Read OI Info
        protected virtual void ReadObjectInfo(BinaryReader reader, Version version, bool other)
        {
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

        protected virtual void ReadCharFileStatus(BinaryReader reader, Version version, ref string name)
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

            if (version >= VersionOf(0, 1, 4))
            {
                name = reader.ReadString();
            }
            else
            {
                name = string.Empty;
            }
        }

        protected virtual void ReadChild(BinaryReader reader, Version version)
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
                        ReadOILightInfo(reader, version);
                        break;
                    case 3:
                        ReadOIFolderInfo(reader, version);
                        break;
                    default:
                        break;
                }
            }
        }

        protected virtual void ReadOICharInfo(BinaryReader reader, Version version)
        {
            ReadObjectInfo(reader, version, true);

            var sex = reader.ReadInt32();

            PHCharaCard charaCard = new PHCharaCard(this.SourceFileName, "", (short)sex);
            charaCard.Parse(reader, 0L);

            string name = string.Empty;
            ReadCharFileStatus(reader, version, ref name);

            charaCard.Name = name;
            this.CharaCards.Add(charaCard);

            // bones
            int countBones = reader.ReadInt32();
            for (int i = 0; i < countBones; i++)
            {
                reader.ReadBytes(4); // key
                ReadObjectInfo(reader, version, false);
            }

            // IkTarget
            int countIkTarget = reader.ReadInt32();
            for (int i = 0; i < countIkTarget; i++)
            {
                reader.ReadBytes(4); // key
                ReadObjectInfo(reader, version, false);
            }

            // child
            int countChild = reader.ReadInt32();
            for (int i = 0; i < countChild; i++)
            {
                reader.ReadBytes(4); // key
                ReadChild(reader, version);
            }

            // kinematicMode, animeInfo.group, animeInfo.category, animeInfo.no
            // handPtnL, handPtnR, skinRate, nipple, siru
            reader.ReadBytes(37);

            if (version >= VersionOf(0, 1, 2))
            {
                reader.ReadBytes(4); // faceOption
            }

            // mouthOpen, lipSync
            reader.ReadBytes(5);

            ReadObjectInfo(reader, version, false); // lookAtTarget

            // enableIK, activeIK, enableFK, activeFK
            // expression, animeSpeed
            reader.ReadBytes(22);

            if (version < VersionOf(0, 1, 2)) 
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

        protected virtual void ReadOIItemInfo(BinaryReader reader, Version version)
        {
            ReadObjectInfo(reader, version, true);
            // no, animeSpeed, colortype, color, color2, enableFK
            reader.ReadBytes(157);  

            int cbone = reader.ReadInt32();
            for (int i = 0; i < cbone; i++)
            {
                reader.ReadString(); // key
                ReadObjectInfo(reader, version, false);
            }

            reader.ReadBytes(4); // animeNormalizedTime
            ReadChild(reader, version);
        }

        protected virtual void ReadOILightInfo(BinaryReader reader, Version version)
        {
            ReadObjectInfo(reader, version, true);
            // no, color, intensity, range, spotAngle, shadow, enable, drawTarget
            reader.ReadBytes(35);
        }

        protected virtual void ReadOIFolderInfo(BinaryReader reader, Version version)
        {
            ReadObjectInfo(reader, version, true);
            reader.ReadString(); // name
            ReadChild(reader, version);
        }
        #endregion

        public virtual bool Parse(BinaryReader reader, long pngEnd)
        {
            if (pngEnd > 0)
            {
                reader.Seek(0, SeekOrigin.Begin);
                PngData = reader.ReadBytes((int)pngEnd);
            }
            reader.Seek(pngEnd, SeekOrigin.Begin);

            Version = new Version(reader.ReadString());

            int infoCount = reader.ReadInt32();
            for (int i = 0; i < infoCount; i++)
            {
                reader.ReadInt32(); // key
                int infoType = reader.ReadInt32();
                switch(infoType)
                {
                    case 0:
                        ReadOICharInfo(reader, Version);
                        break;
                    case 1:
                        ReadOIItemInfo(reader, Version);
                        break;
                    case 2:
                        ReadOILightInfo(reader, Version);
                        break;
                    case 3:
                        ReadOIFolderInfo(reader, Version);
                        break;
                    default:
                        break;
                }
            }

            return CharaCards.Count > 0;
        }
        #endregion
    }
}
