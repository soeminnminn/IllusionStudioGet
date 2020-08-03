using System;
using System.Collections.Generic;
using System.ComponentModel;
using MessagePack;

namespace Illusion.Card
{
    public partial class KKCharaCard
    {
        #region Nested Types
        [MessagePackObject(true)]
        [TypeConverter(typeof(SimpleExpandTypeConverter<CharaParameter>))]
        public class CharaParameter
        {
            #region Variables
            [IgnoreMember]
            public const string BlockName = "Parameter";

            [IgnoreMember]
            public static readonly Version CurrentVersion = new Version("0.0.5");
            #endregion

            #region Constructor
            public CharaParameter()
            {
                version = (Version)CurrentVersion.Clone();
                sex = 0;
                callType = -1;
                personality = 0;
                bloodType = 0;
                birthMonth = 1;
                birthDay = 1;
                clubActivities = 0;
                voiceRate = 0.5f;
                awnser = new Awnser();
                weakPoint = -1;
                denial = new Denial();
                attribute = new Attribute();
                aggressive = 0;
                diligence = 0;
                kindness = 0;
            }
            #endregion

            #region Properties
            public Version version { get; set; }

            public byte sex { get; set; }

            public string lastname { get; set; }

            public string firstname { get; set; }

            public string nickname { get; set; }

            public int callType { get; set; }

            public int personality { get; set; }

            public byte bloodType { get; set; }

            public byte birthMonth { get; set; }

            public byte birthDay { get; set; }

            public byte clubActivities { get; set; }

            public float voiceRate { get; set; }

            public int weakPoint { get; set; }

            public Awnser awnser { get; set; }

            public Denial denial { get; set; }

            public Attribute attribute { get; set; }

            public int aggressive { get; set; }

            public int diligence { get; set; }

            public int kindness { get; set; }
            #endregion

            #region Methods
            public void ComplementWithVersion()
            {
                callType = -1;
                if (version.CompareTo(new Version("0.0.1")) == -1)
                {
                    awnser = new Awnser();
                }
                if (version.CompareTo(new Version("0.0.2")) == -1)
                {
                    denial = new Denial();
                }
                if (version.CompareTo(new Version("0.0.3")) == -1)
                {
                    attribute = new Attribute();
                }
                if (version.CompareTo(new Version("0.0.4")) == -1)
                {
                    voiceRate = 0.5f;
                }
                version = (Version)CurrentVersion.Clone();
            }
            #endregion

            #region Nested Types
            [MessagePackObject(true)]
            [TypeConverter(typeof(SimpleExpandTypeConverter<Awnser>))]
            public class Awnser
            {
                #region Properties
                public bool animal { get; set; }

                public bool eat { get; set; }

                public bool cook { get; set; }

                public bool exercise { get; set; }

                public bool study { get; set; }

                public bool fashionable { get; set; }

                public bool blackCoffee { get; set; }

                public bool spicy { get; set; }

                public bool sweet { get; set; }
                #endregion
            }

            [MessagePackObject(true)]
            [TypeConverter(typeof(SimpleExpandTypeConverter<Denial>))]
            public class Denial
            {
                #region Properties
                public bool kiss { get; set; }

                public bool aibu { get; set; }

                public bool anal { get; set; }

                public bool massage { get; set; }

                public bool notCondom { get; set; }
                #endregion
            }

            [MessagePackObject(true)]
            [TypeConverter(typeof(SimpleExpandTypeConverter<Attribute>))]
            public class Attribute
            {
                #region Properties
                public bool hinnyo { get; set; }

                public bool harapeko { get; set; }

                public bool donkan { get; set; }

                public bool choroi { get; set; }

                public bool bitch { get; set; }

                public bool mutturi { get; set; }

                public bool dokusyo { get; set; }

                public bool ongaku { get; set; }

                public bool kappatu { get; set; }

                public bool ukemi { get; set; }

                public bool friendly { get; set; }

                public bool kireizuki { get; set; }

                public bool taida { get; set; }

                public bool sinsyutu { get; set; }

                public bool hitori { get; set; }

                public bool undo { get; set; }

                public bool majime { get; set; }

                public bool likeGirls { get; set; }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
