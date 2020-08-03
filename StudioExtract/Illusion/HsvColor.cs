using System;

namespace Illusion.Card
{
    public class HsvColor
    {
        #region Constructor
        public HsvColor(float hue, float saturation, float brightness)
        {
            this.H = hue;
            this.S = saturation;
            this.V = brightness;
        }
        #endregion

        #region Properties
        public float H { get; set; }

        public float S { get; set; }

        public float V { get; set; }
        #endregion

        #region Methods
        public static HsvColor FromRGB(float r, float g, float b)
        {
            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));
            float hue = 0.0f;
            if ((double)max == (double)min)
                hue = 0.0f;
            else if ((double)max == (double)r)
                hue = (float)((60.0 * ((double)g - (double)b) / ((double)max - (double)min) + 360.0) % 360.0);
            else if ((double)max == (double)g)
                hue = (float)(60.0 * ((double)b - (double)r) / ((double)max - (double)min) + 120.0);
            else if ((double)max == (double)b)
                hue = (float)(60.0 * ((double)r - (double)g) / ((double)max - (double)min) + 240.0);
            float saturation = (double)max != 0.0 ? (max - min) / max : 0.0f;
            float brightness = max;
            return new HsvColor(hue, saturation, brightness);
        }
        #endregion
    }
}
