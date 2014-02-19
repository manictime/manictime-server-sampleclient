using System;

namespace Finkit.ManicTime.Server.SampleClient.Ui
{
    public static class ColorHelper
    {
        public static int ArgbValuesToColor(byte a, byte r, byte g, byte b)
        {
            return a << 24 | r << 16 | g << 8 | b;
        }

        public static void GetArgbValues(this int color, out byte a, out byte r, out byte g, out byte b)
        {
            byte[] values = BitConverter.GetBytes(color);
            a = values[3];
            r = values[2];
            g = values[1];
            b = values[0];
        }

        public static void GetArgbValues(this string color, out byte a, out byte r, out byte g, out byte b)
        {
            int index = color.StartsWith("#") ? 1 : 0;
            if (color.Length - index != 6 && color.Length - index != 8)
                throw new ArgumentException("Invalid color string length: " + color, "color");
            if (color.Length - index == 8)
            {
                a = Convert.ToByte(color.Substring(index, 2), 16);
                index += 2;
            }
            else
            {
                a = 255;
            }
            r = Convert.ToByte(color.Substring(index, 2), 16);
            g = Convert.ToByte(color.Substring(index + 2, 2), 16);
            b = Convert.ToByte(color.Substring(index + 4, 2), 16);
        }

        public static string ToRgbString(this int color)
        {
            byte a;
            byte r;
            byte g;
            byte b;
            GetArgbValues(color, out a, out r, out g, out b);
            return string.Format("{0:X2}{1:X2}{2:X2}", r, g, b);
        }

        public static int ToRgb(this string color)
        {
            byte a;
            byte r;
            byte g;
            byte b;
            color.GetArgbValues(out a, out r, out g, out b);
            return ArgbValuesToColor(255, r, g, b);
        }
    }
}
