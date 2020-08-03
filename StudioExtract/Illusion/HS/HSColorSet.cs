using System;
using System.IO;

namespace Illusion.Card
{
    public class HSColorSet
    {
        #region Variables
        public HsvColor hsvDiffuse = new HsvColor(19f, 0.07f, 0.63f);
        public float alpha = 1f;
        public HsvColor hsvSpecular = new HsvColor(0.0f, 0.0f, 0.8f);
        public float specularIntensity = 0.1f;
        public float specularSharpness = 0.33f;
        public const int SaveVersion = 1;
        #endregion

        #region Constructor
        public HSColorSet()
        { }
        #endregion

        #region Properties
        public static HSColorSet White
        {
            get => new HSColorSet()
            {
                alpha = 1f,
                hsvDiffuse = HsvColor.FromRGB(1f, 1f, 1f),
                hsvSpecular = HsvColor.FromRGB(1f, 1f, 1f)
            };
        }

        public static HSColorSet DiffuseWhite
        {
            get => new HSColorSet()
            {
                alpha = 1f,
                hsvDiffuse = HsvColor.FromRGB(1f, 1f, 1f)
            };
        }

        public static HSColorSet SpecularWhite
        {
            get => new HSColorSet()
            {
                alpha = 1f,
                hsvSpecular = HsvColor.FromRGB(1f, 1f, 1f)
            };
        }

        public static HSColorSet HairColor
        {
            get => new HSColorSet() 
            {
                hsvDiffuse = new HsvColor(30f, 0.5f, 0.7f),
                hsvSpecular = new HsvColor(0.0f, 0.0f, 0.5f),
                specularIntensity = 0.4f,
                specularSharpness = 0.55f
            };
        }

        public static HSColorSet HairAcsColor
        {
            get => new HSColorSet() 
            {
                hsvDiffuse = new HsvColor(0.0f, 0.8f, 1f),
                hsvSpecular = new HsvColor(0.0f, 0.0f, 0.5f),
                specularIntensity = 0.4f,
                specularSharpness = 0.55f
            };
        }
        #endregion

        #region Methods
        public void Save(BinaryWriter writer)
        {
            writer.Write((double)this.hsvDiffuse.H);
            writer.Write((double)this.hsvDiffuse.S);
            writer.Write((double)this.hsvDiffuse.V);
            writer.Write((double)this.alpha);
            writer.Write((double)this.hsvSpecular.H);
            writer.Write((double)this.hsvSpecular.S);
            writer.Write((double)this.hsvSpecular.V);
            writer.Write((double)this.specularIntensity);
            writer.Write((double)this.specularSharpness);
        }

        public void Load(BinaryReader reader)
        {
            this.hsvDiffuse.H = (float)reader.ReadDouble();
            this.hsvDiffuse.S = (float)reader.ReadDouble();
            this.hsvDiffuse.V = (float)reader.ReadDouble();
            this.alpha = (float)reader.ReadDouble();
            this.hsvSpecular.H = (float)reader.ReadDouble();
            this.hsvSpecular.S = (float)reader.ReadDouble();
            this.hsvSpecular.V = (float)reader.ReadDouble();
            this.specularIntensity = (float)reader.ReadDouble();
            this.specularSharpness = (float)reader.ReadDouble();
        }
        #endregion
    }
}
