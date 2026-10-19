using UnityAssetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    public class GradientAlphaKey
    {
        public float alpha;
        public float time;
    }
    public class GradientColorKey
    {
        public Color color = new();
        public float time;
    }
    public class Gradient : Object
    {
        public GradientColorKey[] colorKeys = [];
        public GradientAlphaKey[] alphaKeys = [];
        public override void LoadDataFrom(Dictionary<string, object?> data, SerializedFile _sourceSerializedFile)
        {
            base.LoadDataFrom(data, _sourceSerializedFile);
            colorKeys = new GradientColorKey[(byte)data["m_NumColorKeys"]!];
            alphaKeys = new GradientAlphaKey[(byte)data["m_NumAlphaKeys"]!];
            for (int i = 0; i < Math.Max(colorKeys.Length, alphaKeys.Length); i++)
            {
                var color = Create<Color>(data[$"key{i}"]!, _sourceSerializedFile);
                if (i < colorKeys.Length)
                {
                    colorKeys[i] = new() { color = color, time = (ushort)data[$"ctime{i}"]! / 65535.0f };
                }
                if (i < alphaKeys.Length)
                {
                    alphaKeys[i] = new() { alpha = color.a, time = (ushort)data[$"atime{i}"]! / 65535.0f };
                }
            }
        }
        public float EvaluateAlpha(float t)
        {
            float lastT = 0f;
            float? lastV = null;
            foreach (var gradKey in alphaKeys)
            {
                if (gradKey.time >= t)
                {
                    if (lastV == null)
                    {
                        return gradKey.alpha;
                    }
                    float prog = (t - lastT) / (gradKey.time - lastT);
                    return (float)lastV + (prog * (gradKey.alpha - (float)lastV));
                }
                lastV = gradKey.alpha;
                lastT = gradKey.time;
            }
            return lastV ?? 0f;
        }
        public Color EvaluateColor(float t)
        {
            float lastT = 0;
            Color? lastV = null;
            foreach (var gradKey in colorKeys)
            {
                if (gradKey.time >= t)
                {
                    if (lastV == null)
                    {
                        return new(gradKey.color);
                    }
                    float prog = (t - lastT) / (gradKey.time - lastT);
                    return Color.Lerp(lastV, gradKey.color, prog);
                }
                lastV = gradKey.color;
                lastT = gradKey.time;
            }
            return lastV == null ? new() : new(lastV);
        }
        public Color Evaluate(float t)
        {
            var col = EvaluateColor(t);
            col.a = EvaluateAlpha(t);
            return col;
        }
    }

}
