using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityAssetLib;

namespace P5XLib
{
    public partial class UIMuitipleGradientParam : MonoBehaviour
    {
        public enum GradientMode
        {
            Linear = 0,
            Radial = 1,
            Angle = 2
        }
        [AutoOptional]
        public float EffectFactor = 0.5f;
        public float Brightness;
        public float Saturation;
        public float Contrast;
        public float GradientAlphaTop;
        public float GradientAlphaBottom;
        public float GraBrightness;
        public float GraSaturation;
        public float GraContrast;
        public byte EnableFastBlur;
        public float BlurStrength;
        public GradientMode gradientMode;
        public Gradient gradient = new();
        public float Angle;
        public Vector2 Center;
        public Vector2 Ratio;
    }
}
