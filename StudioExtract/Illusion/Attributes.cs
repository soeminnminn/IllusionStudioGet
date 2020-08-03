using System;
using System.Text;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;

namespace Illusion.Card
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, Inherited = false)]
    public class CardMarkerAttribute : Attribute
    {
        #region Variables
        private string displayName = string.Empty;
        #endregion

        #region Constructor
        public CardMarkerAttribute()
        {
        }

        public CardMarkerAttribute(string marker)
        {
            Marker = marker;
        }

        public CardMarkerAttribute(string marker, string displayName)
            : this(marker)
        {
            this.displayName = displayName;
        }
        #endregion

        #region Properties
        public string Marker { get; }

        public byte[] MarkerPattern
        {
            get
            {
                if (string.IsNullOrEmpty(Marker)) return null;
                return Encoding.UTF8.GetBytes(Marker);
            }
        }

        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(this.displayName) && !string.IsNullOrEmpty(Marker))
                {
                    return Regex.Replace(Marker, @"[^0-9a-zA-Z\s]+", " ").Trim();
                }
                return displayName;
            }
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return DisplayName;
        }

        public static CardMarkerAttribute GetFrom<T>()
        {
            return typeof(T).GetCustomAttributes(typeof(CardMarkerAttribute), false).FirstOrDefault() as CardMarkerAttribute;
        }

        public static CardMarkerAttribute GetFrom<T>(string name)
        {
            PropertyInfo property = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(p => p.Name == name);
            if (property != null)
            {
                return property.GetCustomAttributes(typeof(CardMarkerAttribute), false).FirstOrDefault() as CardMarkerAttribute;
            }

            MemberInfo member = typeof(T).GetMember(name).FirstOrDefault();
            if (member != null)
            {
                return member.GetCustomAttributes(typeof(CardMarkerAttribute), false).FirstOrDefault() as CardMarkerAttribute;
            }

            return null;
        }
        #endregion
    }
}
