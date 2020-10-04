using System;
using System.Collections.Generic;
using System.IO;

namespace Illusion.Card
{
    public class StudioSceneCard : ISceneCard
    {
        #region Variables
        public byte[] PngData { get; set; }

        public Version Version { get; private set; }

        public string SourceFileName { get; }

        public List<ICharaCard> CharaCards { get; }
        #endregion

        #region Constructor
        public StudioSceneCard(string srcFileName)
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
            reader.ReadBytes(36); // changeAmount

            if (other)
            {
                if (version >= VersionOf(1, 0, 1))
                {
                    reader.ReadBytes(4); // treeState
                }

                if (version >= VersionOf(1, 0, 2))
                {
                    reader.ReadByte(); // visible
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

            //
        }

        protected virtual void ReadOIItemInfo(BinaryReader reader, Version version)
        {
            ReadObjectInfo(reader, version, true);

            reader.ReadBytes(4); // group
            reader.ReadBytes(4); // category
            reader.ReadBytes(4); // no

            if (version >= VersionOf(1, 0, 1))
            {
                reader.ReadBytes(4); // animePattern
            }
            reader.ReadBytes(4); // animeSpeed

            //

            ReadChild(reader, version);
        }

        protected virtual void ReadOILightInfo(BinaryReader reader, Version version)
        {
            ReadObjectInfo(reader, version, true);
            
            reader.ReadBytes(4); // no
            reader.ReadBytes(16); // color
            reader.ReadBytes(4); // intensity
            reader.ReadBytes(4); // range
            reader.ReadBytes(4); // spotAngle
            reader.ReadByte(); // shadow
            reader.ReadByte(); // enable
            reader.ReadByte(); // drawTarget
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

                reader.ReadBytes(4); // speed
                reader.ReadBytes(4); // easeType
                
                if (version == VersionOf(1, 0, 3))
                {
                    reader.ReadByte();
                }

                if (version >= VersionOf(1, 0, 4, 1))
                {
                    reader.ReadBytes(4); // connection

                    // aidInfo
                    ReadObjectInfo(reader, version, false);
                    reader.ReadByte(); // isInit
                }

                if (version >= VersionOf(1, 0, 4, 2))
                {
                    reader.ReadByte(); // link
                }
            }

            if (version >= VersionOf(1, 0, 3))
            {
                reader.ReadByte(); // active
                reader.ReadByte(); // loop
                reader.ReadByte(); // visibleLine
            }

            if (version >= VersionOf(1, 0, 4))
            {
                reader.ReadBytes(4); // orient
            }

            if (version >= VersionOf(1, 0, 4, 1))
            {
                reader.ReadString(); // color
            }
        }

        protected virtual void ReadOICameraInfo(BinaryReader reader, Version version)
        {
            ReadObjectInfo(reader, version, true);
            reader.ReadString(); // name
            reader.ReadBoolean(); // active
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
