using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    public partial class Color : Object
    {
        public float r; public float g; public float b; public float a;
        public float Luminance => r * 0.2125f + g * 0.7154f + b * 0.0721f;
        public Color() { }
        public Color(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
        public Color(Color c)
        {
            r = c.r;
            g = c.g;
            b = c.b;
            a = c.a;
        }

        public void Saturate()
        {
            r = Math.Clamp(r, 0.0f, 1.0f);
            g = Math.Clamp(g, 0.0f, 1.0f);
            b = Math.Clamp(b, 0.0f, 1.0f);
            a = Math.Clamp(a, 0.0f, 1.0f);
        }

        public static Color operator*(Color a, float b)
        {
            return new(a.r * b, a.g * b, a.b * b, a.a);
        }
        public static Color operator *(Color a, Color b)
        {
            return new(a.r * b.r, a.g * b.g, a.b * b.b, a.a);
        }
        public static Color operator +(Color a, Color b)
        {
            return new(a.r + b.r, a.g + b.g, a.b + b.b, a.a);
        }
        public static Color operator -(Color a, Color b)
        {
            return new(a.r - b.r, a.g - b.g, a.b - b.b, a.a);
        }
        public Color Mul(Color cb)
        {
            r *= cb.r;
            g *= cb.g;
            b *= cb.b;
            return this;
        }
        public Color Mul(float c)
        {
            r *= c;
            g *= c;
            b *= c;
            return this;
        }
        public Color Add(Color cb)
        {
            r += cb.r;
            g += cb.g;
            b += cb.b;
            return this;
        }
        public Color Add(float c)
        {
            r += c;
            g += c;
            b += c;
            return this;
        }
        public Color Sub(Color cb)
        {
            r -= cb.r;
            g -= cb.g;
            b -= cb.b;
            return this;
        }
        public Color Sub(float c)
        {
            r -= c;
            g -= c;
            b -= c;
            return this;
        }
        public Color Pow(float exp)
        {
            r = (float)Math.Pow(r, exp);
            g = (float)Math.Pow(g, exp);
            b = (float)Math.Pow(b, exp);
            return this;
        }
        private static float LinearToSrgb(float linear)
        {
            if (linear <= 0.0031308f)
                return linear * 12.92f;
            else
                return 1.055f * (float)Math.Pow(linear, 1.0 / 2.4) - 0.055f;
        }
        public static float SrgbToLinear(float c)
        {
            if (c <= 0.04045f)
                return c / 12.92f;
            else
                return (float)Math.Pow((c + 0.055f) / 1.055f, 2.4f);
        }
        public Color MakeSrgb()
        {
            r = LinearToSrgb(r);
            g = LinearToSrgb(g);
            b = LinearToSrgb(b);
            return this;
        }
        public Color MakeLinear()
        {
            r = SrgbToLinear(r);
            g = SrgbToLinear(g);
            b = SrgbToLinear(b);
            return this;
        }

        //public override void LoadDataFrom(Dictionary<string, object?> data, AssetBundle _sourceAssetBundle)
        //{
        //    base.LoadDataFrom(data, _sourceAssetBundle);
        //    r = (float)data["r"];
        //    g = (float)data["g"];
        //    b = (float)data["b"];
        //    a = (float)data["a"];
        //}

        public static Color Lerp(Color c1, Color c2, float time)
        {
            var res = new Color(
                c1.r + time * (c2.r - c1.r),
                c1.g + time * (c2.g - c1.g),
                c1.b + time * (c2.b - c1.b),
                c1.a + time * (c2.a - c1.a));
            return res;
        }
    }
}
