﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Illusion.Card
{
    public class HSSceneCard : ISceneCard
    {
        #region Variables
        public byte[] PngData { get; set; }

        public Version Version { get; private set; }

        public string SourceFileName { get; }

        public List<ICharaCard> CharaCards { get; }
        #endregion

        #region Constructor
        public HSSceneCard(string srcFileName)
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
            // dicKey, changeAmount
            reader.ReadBytes(40);

            if (other)
            {
                if (version >= VersionOf(1, 0, 1))
                {
                    // treeState
                    reader.ReadBytes(4);
                }
                
                if (version >= VersionOf(1, 0, 2))
                {
                    // visible
                    reader.ReadBoolean();
                }
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
            var mark = reader.ReadString();

            HSCharaCard charaCard = new HSCharaCard(this.SourceFileName, mark, (short)sex);
            charaCard.Parse(reader, 0L);
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

            int countChild = reader.ReadInt32();
            for (int i = 0; i < countChild; i++)
            {
                reader.ReadBytes(4);
                ReadChild(reader, version);
            }

            // kinematicMode, animeInfo.group, animeInfo.category, animeInfo.no
            // handPtnL, handPtnR, skinRate, nipple
            reader.ReadBytes(32);

            if (version >= VersionOf(0, 1, 3))
            {
                reader.ReadBytes(5); // siru
            }

            if (version >= VersionOf(0, 1, 1))
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

            if (version >= VersionOf(0, 1, 1))
            {
                reader.ReadByte(); // animeOptionVisible
            }

            if (version >= VersionOf(0, 1, 5))
            {
                reader.ReadByte(); // isAnimeForceLoop
            }

            // VoiceCtrl
            int cVoice = reader.ReadInt32();
            for (int i = 0; i < cVoice; i++)
            {
                // group, category, no
                reader.ReadBytes(12);
            }
            reader.ReadBytes(4); // repeat

            if (sex == 0)
            {
                // visibleSimple, colorType, simpleColor, visibleSon
                reader.ReadBytes(78);

                if (version >= VersionOf(0, 1, 2))
                {
                    // animeOptionParam[0], animeOptionParam[1]
                    reader.ReadBytes(8);
                }
            }

            // neckByteData
            int cNeckByte = reader.ReadInt32();
            reader.ReadBytes(cNeckByte);

            if (version >= VersionOf(0, 1, 4))
            {
                // eyesByteData
                int cEyesByte = reader.ReadInt32();
                reader.ReadBytes(cEyesByte);
            }

            // animeNormalizedTime
            reader.ReadBytes(4);

            if (version >= VersionOf(0, 1, 2))
            {
                // dicAccessGroup
                int accessGroup = reader.ReadInt32();
                if (accessGroup > 0)
                {
                    reader.ReadBytes(8 * accessGroup);
                }

                // dicAccessNo
                int accessNo = reader.ReadInt32();
                if (accessNo > 0)
                {
                    reader.ReadBytes(8 * accessNo);
                }
            }
        }

        protected virtual void ReadOIItemInfo(BinaryReader reader, Version version)
        {
            ReadObjectInfo(reader, version, true);
            // no, animeSpeed, colortype, color, color2
            reader.ReadBytes(156);
            
            if (version >= VersionOf(1, 0, 4))
            {
                // enableFK
                reader.ReadByte();

                // bones
                int cbone = reader.ReadInt32();
                for (int i = 0; i < cbone; i++)
                {
                    reader.ReadString(); // key
                    ReadObjectInfo(reader, version, false);
                }
            }

            if (version >= VersionOf(0, 1, 6))
            {
                // animeNormalizedTime
                reader.ReadBytes(4);
            }
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
            // name
            reader.ReadString();
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
                switch (infoType)
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
