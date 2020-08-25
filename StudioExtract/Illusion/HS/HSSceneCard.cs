using System;
using System.Collections.Generic;
using System.IO;

namespace Illusion.Card
{
    public class HSSceneCard
    {
        #region Variables
        public string Version { get; private set; }

        public string SourceFileName { get; }

        public List<HSCharaCard> CharaCards { get; }
        #endregion

        #region Constructor
        public HSSceneCard(string srcFileName)
        {
            this.SourceFileName = srcFileName;
            this.CharaCards = new List<HSCharaCard>();
        }
        #endregion

        #region Methods

        #region Read OI Info
        private void ReadObjectInfo(BinaryReader reader, int version, bool other)
        {
            // dicKey, changeAmount
            reader.ReadBytes(40);

            if (other)
            {
                if (version >= 101)
                {
                    // treeState
                    reader.ReadBytes(4);
                }
                
                if (version >= 102)
                {
                    // visible
                    reader.ReadBoolean();
                }
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

        private void ReadOICharInfo(BinaryReader reader, int version)
        {
            ReadObjectInfo(reader, version, true);

            var sex = reader.ReadInt32();
            var mark = reader.ReadString();

            HSCharaCard charaCard = new HSCharaCard(this.SourceFileName, mark, (short)sex);
            charaCard.Parse(reader, 0L);
            this.CharaCards.Add(charaCard);

            int countBones = reader.ReadInt32();
            for (int i = 0; i < countBones; i++)
            {
                reader.ReadBytes(4);
                ReadObjectInfo(reader, version, false);
            }

            int countIkTarget = reader.ReadInt32();
            for (int i = 0; i < countIkTarget; i++)
            {
                reader.ReadBytes(4);
                ReadObjectInfo(reader, version, false);
            }

            int countChild = reader.ReadInt32();
            for (int i = 0; i < countChild; i++)
            {
                reader.ReadBytes(4);
                ReadChild(reader, version);
            }

            // kinematicMode, animeInfo.group, animeInfo.category, animeInfo.no
            // handPtnL, handPtnR, skinRate nipple
            reader.ReadBytes(32);

            if (version >= 13)
            {
                reader.ReadBytes(4); // siru
            }

            if (version >= 11)
            {
                reader.ReadBytes(4); // mouthOpen
            }

            // lipSync
            reader.ReadByte();

            // lookAtTarget
            ReadObjectInfo(reader, version, false);

            // enableIK, activeIK, enableFK, activeFK, expression // 18
            // animeSpeed, animePattern
            reader.ReadBytes(26);

            if (version >= 11)
            {
                reader.ReadByte(); // animeOptionVisible
            }

            if (version >= 15)
            {
                reader.ReadByte(); // isAnimeForceLoop
            }

            // VoiceCtrl
            int voiceCount = reader.ReadInt32();
            for (int i = 0; i < voiceCount; i++)
            {
                // group, category, no
                reader.ReadBytes(12);
            }
            reader.ReadBytes(4); // repeat

            if (sex == 0)
            {
                // visibleSimple, colorType, simpleColor, visibleSon
                reader.ReadBytes(78);

                if (version >= 12)
                {
                    // animeOptionParam[0], animeOptionParam[1]
                    reader.ReadBytes(8);
                }
            }

            // neckByteData
            int countNeckByte = reader.ReadInt32();
            reader.ReadBytes(countNeckByte);

            if (version >= 14)
            {
                // eyesByteData
                int countEyesByte = reader.ReadInt32();
                reader.ReadBytes(countEyesByte);
            }

            // animeNormalizedTime
            reader.ReadBytes(4);

            if (version >= 12)
            {
                int accessGroup = reader.ReadInt32();
                if (accessGroup > 0)
                {
                    reader.ReadBytes(8 * accessGroup);
                }

                int accessNo = reader.ReadInt32();
                if (accessNo > 0)
                {
                    reader.ReadBytes(8 * accessNo);
                }
            }
        }

        private void ReadOIItemInfo(BinaryReader reader, int version)
        {
            ReadObjectInfo(reader, version, true);
            // no, animeSpeed, colortype, color, color2
            reader.ReadBytes(156);
            
            if (version >= 104)
            {
                // enableFK
                reader.ReadByte();
                int cbone = reader.ReadInt32();
                for (int i = 0; i < cbone; i++)
                {
                    reader.ReadString();
                    ReadObjectInfo(reader, version, false);
                }
            }

            if (version >= 16)
            {
                // animeNormalizedTime
                reader.ReadBytes(4);
            }
            ReadChild(reader, version);
        }

        private void ReadOILightInfo(BinaryReader reader, int version)
        {
            ReadObjectInfo(reader, version, true);
            // no, color, intensity, range, spotAngle, shadow, enable, drawTarget
            reader.ReadBytes(35);
        }

        private void ReadOIFolderInfo(BinaryReader reader, int version)
        {
            ReadObjectInfo(reader, version, true);
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
                switch (infoType)
                {
                    case 0:
                        ReadOICharInfo(reader, versionInt);
                        break;
                    case 1:
                        ReadOIItemInfo(reader, versionInt);
                        break;
                    case 2:
                        ReadOILightInfo(reader, versionInt);
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
