using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MessagePack;

namespace Illusion.Card
{
    public class CardExtractor
    {
        #region Variables
        private static readonly byte[] pngEndChunk = { 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82 };
        private static readonly byte[] pngStartChunk = { 0x89, 0x50, 0x4E, 0x47, 0x0D };

        internal const string aiCharaMark = AICharaCard.marker;
        internal const string hsCharaMaleMark = "【HoneySelectCharaMale】";
        internal const string hsCharaFemaleMark = "【HoneySelectCharaFemale】";
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
            using (BinaryReader reader = new BinaryReader(file.OpenRead()))
            {
                var pngEnd = SearchForPngEnd(reader);

                if (pngEnd == -1 || pngEnd >= reader.BaseStream.Length)
                    return false;

                reader.Seek(pngEnd, SeekOrigin.Begin);

                try
                {
                    var loadProductNo = reader.ReadInt32();
                    if (loadProductNo < 0) // HS Chara Card
                    {
                        reader.Seek(pngEnd, SeekOrigin.Begin);
                    }
                    else if (loadProductNo > 100) // Scene Card
                    {
                        reader.Seek(pngEnd, SeekOrigin.Begin);
                        return ReadSceneCard(reader, pngEnd);
                    }

                    var marker = reader.ReadString();
                    switch (marker)
                    {
                        case aiCharaMark:
                            return ReadAiCharaCard(reader, pngEnd);

                        case hsCharaMaleMark:
                        case hsCharaFemaleMark:
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }
            return false;
        }

        private bool ReadSceneCard(BinaryReader reader, long pngEnd)
        {
            var version = reader.ReadString();
            Version loadVersion;
            if (!Version.TryParse(version, out loadVersion))
            {
                return false;
            }

            var charaMarkPattern = Encoding.UTF8.GetBytes(aiCharaMark);
            long charaPos = reader.FindPattern(charaMarkPattern);
            if (charaPos > -1)
            {
                try
                {
                    do
                    {
                        reader.Seek(charaPos - 1, SeekOrigin.Begin);
                        string marker = reader.ReadString();
                        switch (marker)
                        {
                            case aiCharaMark:
                                ReadAiCharaCard(reader, 0);
                                break;

                            case hsCharaMaleMark:
                            case hsCharaFemaleMark:
                                break;

                            default:
                                break;
                        }

                        charaPos = reader.FindPattern(charaMarkPattern);
                    } while (charaPos > -1);

                    return true;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }

            return false;
        }

        private bool ReadAiCharaCard(BinaryReader reader, long pngEnd)
        {
            AICharaCard card = new AICharaCard();

            try
            {
                if (pngEnd > 0)
                {
                    var position = reader.BaseStream.Position;

                    reader.Seek(0, SeekOrigin.Begin);
                    card.PngData = reader.ReadBytes((int)pngEnd);
                    reader.Seek(position, SeekOrigin.Begin);
                }

                card.Version = reader.ReadString();

                card.Language = reader.ReadInt32();
                card.UserID = reader.ReadString();
                card.DataID = reader.ReadString();

                var headerSize = reader.ReadInt32();
                var bytes = reader.ReadBytes(headerSize);

                var blockHeader = MessagePackSerializer.Deserialize<AICharaCard.BlockHeader>(bytes);
                if (blockHeader != null)
                {
                    card.BlocksInfo = blockHeader;
                    card.DataSize = reader.ReadInt64();
                    var position = reader.BaseStream.Position;

                    foreach (var info in blockHeader.lstInfo)
                    {
                        long seekPos = reader.Seek(position + info.pos, SeekOrigin.Begin);
                        if (seekPos < reader.BaseStream.Length)
                        {
                            var dataBytes = reader.ReadBytes((int)info.size);

                            // var dataHex = "\"" + info.name + "\" : \"0x" + BitConverter.ToString(dataBytes).Replace("-", ", 0x") + "\"";
                            // System.Diagnostics.Debug.WriteLine(dataHex);

                            card.DataBlocks.Add(info.name, dataBytes);
                        }
                    }

                    card.ParseBlock<AICharaCard.CharaParameter>("Parameter", x => card.Parameter = x);
                }

                Cards.Add(card);
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            return false;
        }

        private long SearchForPngEnd(BinaryReader reader)
        {
            var result = reader.FindPattern(pngEndChunk);
            if (result >= 0) result += pngEndChunk.Length;
            return result;
        }

        private long SearchForPngStart(BinaryReader reader)
        {
            return reader.FindPattern(pngStartChunk);
        }
        #endregion
    }
}
