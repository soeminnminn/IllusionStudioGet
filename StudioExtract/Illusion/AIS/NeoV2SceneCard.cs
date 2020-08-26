using System;
using System.Collections.Generic;
using System.IO;

namespace Illusion.Card
{
    public class NeoV2SceneCard : ISceneCard
    {
        #region Variables
        public byte[] PngData { get; set; }

        public Version Version { get; private set; }

        public string SourceFileName { get; }

        public List<ICharaCard> CharaCards { get; }
        #endregion

        #region Constructor
        public NeoV2SceneCard(string srcFileName)
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
                // treeState
                reader.ReadBytes(4);
                // visible
                reader.ReadBoolean();
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
                    case 4:
                        ReadOIRouteInfo(reader, version);
                        break;
                    case 5:
                        ReadOICameraInfo(reader, version);
                        break;
                    default:
                        break;
                }
            }
        }

        protected virtual void ReadOIPatternInfo(BinaryReader reader, Version version)
        {
            reader.ReadString(); // color
            reader.ReadBytes(4); // key
            reader.ReadString(); // filePath
            reader.ReadByte(); // clamp
            reader.ReadString(); // uv
            reader.ReadBytes(4); // rot
        }

        protected virtual void ReadOICharInfo(BinaryReader reader, Version version)
        {
            ReadObjectInfo(reader, version, true);
            reader.ReadBytes(4); // sex

            int productNo = reader.ReadInt32();
            if (productNo <= 100)
            {
                var mark = reader.ReadString();
                AISCharaCard charaCard = new AISCharaCard(this.SourceFileName, productNo, mark);
                charaCard.Parse(reader, 0L);
                this.CharaCards.Add(charaCard);
            }

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
            // handPtnL, handPtnR, nipple, siru, mouthOpen, lipSync
            reader.ReadBytes(38);

            // lookAtTarget
            ReadObjectInfo(reader, version, false);

            // enableIK, activeIK, enableFK, activeFK, expression
            // animeSpeed, animePattern, animeOptionVisible, isAnimeForceLoop
            reader.ReadBytes(32);

            // voiceCtrl
            int cVoice = reader.ReadInt32();
            for (int i = 0; i < cVoice; i++)
            {
                // group, category, no
                reader.ReadBytes(12);
            }
            reader.ReadBytes(4); // repeat

            // visibleSon, sonLength, visibleSimple
            reader.ReadBytes(6);
            reader.ReadString(); // simpleColor
            // animeOptionParam[0], animeOptionParam[1]
            reader.ReadBytes(8);

            // neckByteData
            int cNeckByte = reader.ReadInt32();
            reader.ReadBytes(cNeckByte);

            // eyesByteData
            int cEyesByte = reader.ReadInt32();
            reader.ReadBytes(cEyesByte);

            reader.ReadBytes(4); // animeNormalizedTime

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

        protected virtual void ReadOIItemInfo(BinaryReader reader, Version version)
        {
            ReadObjectInfo(reader, version, true);
            // group, category, no
            reader.ReadBytes(12);
            if (version >= VersionOf(1, 0, 1))
            {
                reader.ReadBytes(4); // animePattern
            }
            reader.ReadBytes(4); // animeSpeed

            // colors
            for (int i = 0; i < 4; i++)
            {
                // mainColor
                reader.ReadString();
                // metallic, glossiness
                reader.ReadBytes(8);

                // pattern
                ReadOIPatternInfo(reader, version);
            }

            reader.ReadBytes(4); // alpha
            reader.ReadString(); // emissionColor
            // emissionPower, lightCancel
            reader.ReadBytes(8);

            // panel
            ReadOIPatternInfo(reader, version);

            reader.ReadByte(); // enableFK

            // bones
            int cbone = reader.ReadInt32();
            for (int i = 0; i < cbone; i++)
            {
                reader.ReadString(); // key
                ReadObjectInfo(reader, version, false);
            }
            reader.ReadByte(); // enableDynamicBone

            // option
            int cOption = reader.ReadInt32();
            reader.ReadBytes(cOption);

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
            // name
            reader.ReadString();
            ReadChild(reader, version);
        }

        protected virtual void ReadOIRouteInfo(BinaryReader reader, Version version)
        {
            ReadObjectInfo(reader, version, true);
            reader.ReadString(); // name

            ReadChild(reader, version);

            // OIRoutePointInfo
            int cPoint = reader.ReadInt32();
            for (int i = 0; i < cPoint; i++)
            {
                ReadObjectInfo(reader, version, false);
                // speed, easeType, connection
                reader.ReadBytes(12);

                // aidInfo
                ReadObjectInfo(reader, version, false);
                reader.ReadByte(); // isInit

                reader.ReadByte(); // link
            }

            // active, loop, visibleLine, orient
            reader.ReadBytes(7);
            reader.ReadString(); // color
        }

        protected virtual void ReadOICameraInfo(BinaryReader reader, Version version)
        {
            ReadObjectInfo(reader, version, true);
            reader.ReadString(); // name
            reader.ReadBoolean(); // active
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
                    case 4:
                        ReadOIRouteInfo(reader, Version);
                        break;
                    case 5:
                        ReadOICameraInfo(reader, Version);
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
