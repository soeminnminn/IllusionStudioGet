using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Illusion.Card
{
    public class CardExtractor
    {
        #region Variables
        private static readonly byte[] pngEndChunk = { 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82 };
        private static readonly byte[] pngStartChunk = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        #endregion

        #region Constructor
        public CardExtractor()
        {
            this.Cards = new List<ICharaCard>();
        }
        #endregion

        #region Properties
        public List<ICharaCard> Cards { get; private set; }
        #endregion

        #region Methods
        public bool TryParse(FileInfo file)
        {
            string fileName = file.FullName;
            using (BinaryReader reader = new BinaryReader(file.OpenRead()))
            {
                //long pngSize = SearchForPngEnd(reader);

                //if (pngSize == -1 || pngSize >= reader.BaseStream.Length)
                //    return false;

                //reader.Seek(pngSize, SeekOrigin.Begin);

                long pngSize = 0L;
                if (!CheckPngData(reader.BaseStream, ref pngSize, true))
                    return false;

                if (reader.BaseStream.Length - reader.BaseStream.Position == 0L)
                    return false;

                try
                {
                    int loadProductNo = reader.ReadInt32();
                    if (loadProductNo != 100)
                    {
                        if (CheckIfSceneCard(reader) != CardTypes.Unknown) // Scene Card
                        {
                            return ReadSceneCard(fileName, reader, pngSize);
                        }

                        reader.Seek(pngSize, SeekOrigin.Begin);
                    }

                    ICharaCard card = null;

                    string marker = reader.ReadString();
                    CardTypes cardType = CardTypeFinder.IsMarkerOf(marker);

                    switch (cardType)
                    {
                        case CardTypes.KoiKatuChara:
                        case CardTypes.KoiKatuCharaS:
                        case CardTypes.KoiKatuCharaSP:
                            card = new KKCharaCard(fileName, loadProductNo, marker);
                            break;

                        case CardTypes.AIS_Chara:
                            card = new AISCharaCard(fileName, loadProductNo, marker);
                            break;

                        case CardTypes.HoneySelectCharaMale:
                            card = new HSCharaCard(fileName, marker, 0);
                            break;

                        case CardTypes.HoneySelectCharaFemale:
                            card = new HSCharaCard(fileName, marker, 1);
                            break;

                        case CardTypes.PlayHome_Male:
                            card = new PHCharaCard(fileName, marker, 0);
                            break;

                        case CardTypes.PlayHome_Female:
                            card = new PHCharaCard(fileName, marker, 1);
                            break;

                        case CardTypes.PremiumResortCharaMale:
                            card = new SBPRCharaCard(fileName, marker, 0);
                            break;

                        case CardTypes.PremiumResortCharaFemale:
                            card = new SBPRCharaCard(fileName, marker, 1);
                            break;

                        default:
                            break;
                    }

                    if (card != null && card.Parse(reader, pngSize))
                    {
                        Cards.Add(card);
                        return true;
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }
            return false;
        }

        private CardTypes CheckIfSceneCard(BinaryReader reader)
        {
            var finderArr = new CardTypeFinder[] 
                {
                    new CardTypeFinder(CardTypes.KStudio),
                    new CardTypeFinder(CardTypes.HoneyStudio),
                    new CardTypeFinder(CardTypes.PHStudio),
                    new CardTypeFinder(CardTypes.StudioNeo),
                    new CardTypeFinder(CardTypes.StudioNEOV2)
                };

            try
            {
                long position = reader.BaseStream.Position;
                foreach (var finder in finderArr)
                {
                    if (finder.Find(reader, true))
                    {
                        reader.Seek(position, SeekOrigin.Begin);
                        return finder.CardType;
                    }
                }

                reader.Seek(position, SeekOrigin.Begin);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return CardTypes.Unknown;
        }

        private bool ReadSceneCard(string fileName, BinaryReader reader, long pngEnd)
        {
            var finderArr = new CardTypeFinder[]
                {
                    new CardTypeFinder(CardTypes.KoiKatuChara),
                    new CardTypeFinder(CardTypes.KoiKatuCharaS),
                    new CardTypeFinder(CardTypes.KoiKatuCharaSP),
                    new CardTypeFinder(CardTypes.AIS_Chara),
                    new CardTypeFinder(CardTypes.HoneySelectCharaMale),
                    new CardTypeFinder(CardTypes.HoneySelectCharaFemale)
                };

            foreach(var finder in finderArr)
            {
                reader.Seek(pngEnd, SeekOrigin.Begin);
                if (finder.Find(reader, false))
                {
                    try
                    {
                        do
                        {
                            long charaPos = finder.Position;
                            reader.Seek(charaPos + finder.MarkerPattern.Length, SeekOrigin.Begin);

                            ICharaCard card = null;
                            switch (finder.CardType)
                            {
                                case CardTypes.KoiKatuChara:
                                case CardTypes.KoiKatuCharaS:
                                case CardTypes.KoiKatuCharaSP:
                                    card = new KKCharaCard(fileName, 100, finder.Marker);
                                    break;

                                case CardTypes.AIS_Chara:
                                    card = new AISCharaCard(fileName, 100, finder.Marker);
                                    break;

                                case CardTypes.HoneySelectCharaMale:
                                    card = new HSCharaCard(fileName, finder.Marker, 0);
                                    break;

                                case CardTypes.HoneySelectCharaFemale:
                                    card = new HSCharaCard(fileName, finder.Marker, 1);
                                    break;

                                default:
                                    break;
                            }

                            if (card != null && card.Parse(reader, 0))
                            {
                                Cards.Add(card);
                            }

                            reader.Seek(charaPos + finder.MarkerPattern.Length + 1, SeekOrigin.Begin);

                        } while (finder.Find(reader, false));
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }
                }
            }

            return true;
        }

        private long SearchForPngEnd(BinaryReader reader)
        {
            long result = reader.BaseStream.FindSequence(pngEndChunk);
            if (result >= 0) result += pngEndChunk.Length;
            return result;
        }

        private long SearchForPngStart(BinaryReader reader)
        {
            return reader.BaseStream.FindSequence(pngStartChunk);
        }

        private bool CheckPngData(Stream stream, ref long size, bool skip)
        {
            size = 0L;
            long position = stream.Position;

            try
            {
                int startBufLen = pngStartChunk.Length;
                byte[] bufStart = new byte[startBufLen];

                var read = stream.Read(bufStart, 0, startBufLen);
                if (read != startBufLen)
                {
                    stream.Seek(position, SeekOrigin.Begin);
                    return false;
                }

                if (!pngStartChunk.SequenceEqual(bufStart))
                {
                    stream.Seek(position, SeekOrigin.Begin);
                    return false;
                }

                bool flag = true;
                while(flag)
                {
                    byte[] readBuf = new byte[4];
                    stream.Read(readBuf, 0, 4);

                    Array.Reverse(readBuf);
                    int first = BitConverter.ToInt32(readBuf, 0);

                    stream.Read(readBuf, 0, 4);

                    if (BitConverter.ToInt32(readBuf, 0) == 1145980233)
                        flag = false;

                    if ((first + 4) > stream.Length - stream.Position)
                    {
                        stream.Seek(position, SeekOrigin.Begin);
                        return false;
                    }

                    stream.Seek(first + 4, SeekOrigin.Current);
                }

                size = stream.Position - position;

                if (!skip) stream.Seek(position, SeekOrigin.Begin);
            }
            catch (EndOfStreamException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                stream.Seek(position, SeekOrigin.Begin);
                return false;
            }

            return true;
        }
        #endregion

        #region Nested Types
        private class CardTypeFinder
        {
            #region Members
            public string Marker { get; private set; }

            public byte[] MarkerPattern { get; private set; }

            public CardTypes CardType { get; }

            public long Position { get; private set; } = -1;
            #endregion

            #region Constructor
            public CardTypeFinder(CardTypes cardType)
            {
                CardType = cardType;
                MarkerPattern = null;

                var attr = CardMarkerAttribute.GetFrom<CardTypes>(cardType.ToString());
                if (attr != null)
                {
                    Marker = attr.Marker;
                    MarkerPattern = attr.MarkerPattern;
                }
            }
            #endregion

            #region Methods
            public bool Find(BinaryReader reader, bool seekToBegin)
            {
                if (MarkerPattern == null) return false;
                if (seekToBegin) reader.Seek(0, SeekOrigin.Begin);
                Position = reader.BaseStream.FindPattern(MarkerPattern);
                return (Position > -1);
            }
            
            public static CardTypes IsMarkerOf(string marker)
            {
                MemberInfo member = typeof(CardTypes).GetMembers().Where(x => 
                {
                    var attr = x.GetCustomAttributes(typeof(CardMarkerAttribute), false).FirstOrDefault() as CardMarkerAttribute;
                    return (attr != null && attr.Marker == marker);
                }).FirstOrDefault();

                if (member != null)
                {
                    var obj = Enum.Parse(typeof(CardTypes), member.Name);
                    return obj != null ? (CardTypes)obj : CardTypes.Unknown;
                }

                return CardTypes.Unknown;
            }
            #endregion
        }
        #endregion
    }
}
