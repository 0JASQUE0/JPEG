using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace JPEG
{
    class YCbCrColor
    {
        public float Y;
        public float Cb;
        public float Cr;

        public YCbCrColor() { }
        public YCbCrColor(float y, float cb, float cr)
        {
            Y = y;
            Cb = cb;
            Cr = cr;
        }

        public Color ToRgbColor()
        {
            int R = (int)(Y + 1.402 * (Cr - 128));
            int G = (int)(Y - 0.34414 * (Cb - 128) - 0.71414 * (Cr - 128));
            int B = (int)(Y + 1.772 * (Cb - 128));

            if (R > 255) R = 255;
            if (G > 255) G = 255;
            if (B > 255) B = 255;

            if (R < 0) R = 0;
            if (G < 0) G = 0;
            if (B < 0) B = 0;

            return Color.FromArgb(R, G, B);
        }

        public YCbCrColor FromRgbColor(Color color)
        {
            Y = (float)(0.2989 * color.R + 0.5866 * color.G + 0.1145 * color.B);
            Cb = (float)(-0.1687 * color.R - 0.3313 * color.G + 0.5000 * color.B + 128);
            Cr = (float)(0.5000 * color.R - 0.4184 * color.G - 0.0816 * color.B + 128);

            return new YCbCrColor(Y, Cb, Cr);
        }
    }
}
