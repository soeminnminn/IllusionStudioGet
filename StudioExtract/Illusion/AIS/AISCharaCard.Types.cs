using System;
using System.Collections.Generic;
using System.ComponentModel;
using MessagePack;

namespace Illusion.Card
{
    public partial class AISCharaCard
    {
        #region Nested Types
        [MessagePackObject(true)]
        [TypeConverter(typeof(SimpleExpandTypeConverter<CharaParameter>))]
        public class CharaParameter
        {
            #region Variables
            [IgnoreMember]
            public const string BlockName = "Parameter";
            #endregion

            #region Properties
            public string version { get; set; } = string.Empty;

            public byte sex { get; set; }

            public string fullname { get; set; } = string.Empty;

            public int personality { get; set; }

            public byte birthMonth { get; set; } = 1;

            public byte birthDay { get; set; } = 1;

            public float voiceRate { get; set; } = 0.5f;

            [TypeConverter(typeof(CollectionConverter))]
            public HashSet<int> hsWish { get; set; } = new HashSet<int>();

            public bool futanari { get; set; }
            #endregion

            #region Constructor
            public CharaParameter() { }
            #endregion

            #region Methods
            public override string ToString() => $"{fullname} {{ sex: {sex} }}";
            #endregion
        }

        [MessagePackObject(true)]
        [TypeConverter(typeof(SimpleExpandTypeConverter<CharaParameter2>))]
        public class CharaParameter2
        {
            #region Variables
            [IgnoreMember]
            public const string BlockName = "Parameter2";
            #endregion

            #region Properties
            public string version { get; set; } = string.Empty;

            public int personality { get; set; }

            public float voiceRate { get; set; } = 0.5f;

            public byte trait { get; set; }

            public byte mind { get; set; }

            public byte hAttribute { get; set; }
            #endregion
        }

		[MessagePackObject(true)]
		[TypeConverter(typeof(SimpleExpandTypeConverter<CharaGameInfo>))]
		public class CharaGameInfo
		{
            #region Variables
            [IgnoreMember]
			public const string BlockName = "GameInfo";
            #endregion

            #region Properties
            public string version { get; set; } = string.Empty;

			public bool gameRegistration { get; set; }

			public MinMaxInfo tempBound { get; set; } = new MinMaxInfo();

			public MinMaxInfo moodBound { get; set; } = new MinMaxInfo();

			//public Dictionary<int, int> flavorState { get; set; } = new Dictionary<int, int>();

			public int totalFlavor { get; set; }

			//public Dictionary<int, float> desireDefVal { get; set; } = new Dictionary<int, float>();

			//public Dictionary<int, float> desireBuffVal { get; set; } = new Dictionary<int, float>();

			public int phase { get; set; }

			//public Dictionary<int, int> normalSkill { get; set; } = new Dictionary<int, int>();

			//public Dictionary<int, int> hSkill { get; set; } = new Dictionary<int, int>();

			public int favoritePlace { get; set; } = -1;

			public int lifestyle { get; set; } = -1;

			public int morality { get; set; }

			public int motivation { get; set; }

			public int immoral { get; set; }

			public bool isHAddTaii0 { get; set; }

			public bool isHAddTaii1 { get; set; }
            #endregion

            #region Nested Types
            [MessagePackObject(true)]
			public class MinMaxInfo
			{
				public float lower { get; set; } = 20f;

				public float upper { get; set; } = 80f;

				public override string ToString() => $"From {lower} to {upper}";
			}
            #endregion
        }

        [MessagePackObject(true)]
		[TypeConverter(typeof(SimpleExpandTypeConverter<CharaGameInfo2>))]
		public class CharaGameInfo2
		{
            #region Variables
            [IgnoreMember]
			public const string BlockName = "GameInfo2";
            #endregion

            #region Properties
            public string version { get; set; } = string.Empty;

			public int Favor { get; set; }

			public int Enjoyment { get; set; }

			public int Aversion { get; set; }

			public int Slavery { get; set; }

			public int Broken { get; set; }

			public int Dependence { get; set; }

			public State nowState { get; set; }

			public State nowDrawState { get; set; }

			public bool lockNowState { get; set; }

			public bool lockBroken { get; set; }

			public bool lockDependence { get; set; }

			public int Dirty { get; set; }

			public int Tiredness { get; set; }

			public int Toilet { get; set; }

			public int Libido { get; set; }

			public int alertness { get; set; }

			public State calcState { get; set; }

			public byte escapeFlag { get; set; }

			public bool escapeExperienced { get; set; }

			public bool firstHFlag { get; set; }

			//public bool[][] genericVoice = new bool[2][]{ get; set; }

			public bool genericBrokenVoice { get; set; }

			public bool genericDependencepVoice { get; set; }

			public bool genericAnalVoice { get; set; }

			public bool genericPainVoice { get; set; }

			public bool genericFlag { get; set; }

			public bool genericBefore { get; set; }

			//public bool[] inviteVoice = new bool[5]{ get; set; }

			public int hCount { get; set; }

			//public HashSet<int> map = new HashSet<int>(){ get; set; }

			public bool arriveRoom50 { get; set; }

			public bool arriveRoom80 { get; set; }

			public bool arriveRoomHAfter { get; set; }

			public int resistH { get; set; }

			public int resistPain { get; set; }

			public int resistAnal { get; set; }

			public int usedItem { get; set; }

			public bool isChangeParameter { get; set; }

			public bool isConcierge { get; set; }
            #endregion

            #region Nested Types
            public enum State : byte
			{
				Blank,
				Favor,
				Enjoyment,
				Aversion,
				Slavery,
				Broken,
				Dependence
			}
            #endregion
        }
        #endregion
    }
}
