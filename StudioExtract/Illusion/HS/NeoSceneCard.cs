using System;
using System.Collections.Generic;
using System.IO;

namespace Illusion.Card
{
    public class NeoSceneCard
    {
        #region Variables
        public string Version { get; private set; }

        public string SourceFileName { get; }

        public List<HSCharaCard> CharaCards { get; }
        #endregion

        #region Constructor
        public NeoSceneCard(string srcFileName)
        {
            this.SourceFileName = srcFileName;
            this.CharaCards = new List<HSCharaCard>();
        }
        #endregion

        #region Methods

        #region Read OI Info
        private void ReadObjectInfo(BinaryReader reader, bool other)
        { }

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
        { }

        private void ReadOIItemInfo(BinaryReader reader, int version)
        { }

        private void ReadOILightInfo(BinaryReader reader, int version)
        { }

        private void ReadOIFolderInfo(BinaryReader reader, int version)
        { }
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
