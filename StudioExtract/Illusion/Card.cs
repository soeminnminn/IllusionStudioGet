using System;
using System.IO;

namespace Illusion.Card
{
    public enum CardTypes
    {
        [CardMarker]
        Unknown,
        [CardMarker("【KoiKatuChara】")]
        KoiKatuChara,
        [CardMarker("【KoiKatuCharaS】")]
        KoiKatuCharaS,
        [CardMarker("【KoiKatuCharaSP】")]
        KoiKatuCharaSP,
        [CardMarker("【PremiumResortCharaMale】")]
        PremiumResortCharaMale,
        [CardMarker("【PremiumResortCharaFemale】")]
        PremiumResortCharaFemale,
        [CardMarker("【HoneySelectCharaMale】")]
        HoneySelectCharaMale,
        [CardMarker("【HoneySelectCharaFemale】")]
        HoneySelectCharaFemale,
        [CardMarker("【PlayHome_Male】")]
        PlayHome_Male,
        [CardMarker("【PlayHome_Female】")]
        PlayHome_Female,
        [CardMarker("【AIS_Chara】")]
        AIS_Chara,
        [CardMarker("【KStudio】")]
        KStudio,
        [CardMarker("【PHStudio】")]
        PHStudio,
        [CardMarker("【honey】")]
        HoneyStudio,
        [CardMarker("【-neo-】")]
        StudioNeo,
        [CardMarker("【StudioNEOV2】")]
        StudioNEOV2,
        [CardMarker("【voice】")]
        Voice,
        [CardMarker("【HoneySelectClothesFemale】")]
        HoneySelectClothesFemale,
        [CardMarker("【HoneySelectClothesMale】")]
        HoneySelectClothesMale,
        [CardMarker("【HoneySelectCustomFile】")]
        HoneySelectCustomFile,
        [CardMarker("【AIS_Clothes】")]
        AIS_Clothes
    }

    public enum CharaSex : byte
    {
        Male = 0,
        Female = 1
    }

    public interface ICard
    {
        #region Members
        string Game { get; }

        string Name { get; }

        int Sex { get; }

        byte[] PngData { get; set; }

        string SourceFileName { get; }

        bool Parse(BinaryReader reader, long pngEnd);

        string GenerateFileName();

        bool Save(Stream stream, SaveOptions options);
        #endregion
    }

    public interface ICharaCard : ICard
    {
    }

    public interface ICoordinateCard : ICard
    {
    }
}
