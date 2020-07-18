using System;
using System.Collections.Generic;
using System.IO;

namespace Illusion.Card
{
    public enum CardTypes
    {
        Unknown,
        Charater,
        Scene,
        Coordinate
    }

    public interface ICharaCard
    {
        #region Members
        string GenerateFileName(CardTypes cardType);

        bool Save(Stream stream);

        bool SaveCoordinate(Stream stream);
        #endregion
    }
}
