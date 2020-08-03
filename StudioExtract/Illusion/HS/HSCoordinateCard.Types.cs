using System;
using System.IO;

namespace Illusion.Card
{
    public partial class HSCoordinateCard
    {
        #region Nested Types
        public class Clothes
        {
            #region Variables
            public int id;
            public HSColorSet color;
            public HSColorSet color2;
            #endregion

            #region Constructor
            public Clothes()
            {
                color = new HSColorSet();
                color2 = new HSColorSet();
            }
            #endregion

            #region Methods
            public void Load(BinaryReader reader, int version)
            {
                id = reader.ReadInt32();
                color.Load(reader);
                if (version >= 3) color2.Load(reader);
            }

            public void Save(BinaryWriter writer)
            {
                writer.Write(id);
                color.Save(writer);
                color2.Save(writer);
            }
            #endregion
        }

        public class Accessory
        {
            #region Variables
            public int type = -1;
            public int id = -1;
            public string parentKey = string.Empty;
            public Vector3 addPos = Vector3.zero;
            public Vector3 addRot = Vector3.zero;
            public Vector3 addScl = Vector3.one;
            public HSColorSet color = new HSColorSet();
            public HSColorSet color2 = new HSColorSet();
            #endregion

            #region Constructor
            public Accessory()
            { }
            #endregion

            #region Methods
            public void Save(BinaryWriter writer)
            {
                writer.Write(this.type);
                writer.Write(this.id);
                writer.Write(this.parentKey);
                writer.Write(this.addPos.x);
                writer.Write(this.addPos.y);
                writer.Write(this.addPos.z);
                writer.Write(this.addRot.x);
                writer.Write(this.addRot.y);
                writer.Write(this.addRot.z);
                writer.Write(this.addScl.x);
                writer.Write(this.addScl.y);
                writer.Write(this.addScl.z);
                this.color.Save(writer);
                this.color2.Save(writer);
            }

            public void Load(BinaryReader reader, int productNo)
            {
                this.type = reader.ReadInt32();
                this.id = reader.ReadInt32();
                this.parentKey = reader.ReadString();
                this.addPos.x = reader.ReadSingle();
                this.addPos.y = reader.ReadSingle();
                this.addPos.z = reader.ReadSingle();
                this.addRot.x = reader.ReadSingle();
                this.addRot.y = reader.ReadSingle();
                this.addRot.z = reader.ReadSingle();
                this.addScl.x = reader.ReadSingle();
                this.addScl.y = reader.ReadSingle();
                this.addScl.z = reader.ReadSingle();
                this.color.Load(reader);
                if (productNo < 3)
                    return;
                this.color2.Load(reader);
            }
            #endregion
        }
        #endregion
    }
}
