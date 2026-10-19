using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using P5XLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using UnityAssetLib;


using Matrix4 = OpenTK.Mathematics.Matrix4;


namespace P5XTool
{
    public class ImageLayer
    {
        public ImageLayer()
        {
        }
        public ImageLayer(string? _imgName = null, string? _matParam = null)
        {
            imgName = _imgName;
            matParam = _matParam;
        }
        public Vector2 GetSize()
        {
            if (size == Vector2.Zero && parent != null)
            {
                return parent.GetSize();
            }
            return size;
        }
        public void SetSizeFromTexture()
        {
            var tex = GetTexture();
            if (tex != null)
            {
                size = new Vector2(tex.m_Width, tex.m_Height);
            }
        }
        private (Vector2 min, Vector2 max) GetExtentImpl()
        {
            var min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            var max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
            if (visible && HasImage())
            {
                var s = GetSize();
                var rad1 = Vector2.Multiply(s, pivot);
                var rad2 = Vector2.Multiply(s, new Vector2(1, 1) - pivot);
                min = GetPosition() - rad1;
                max = GetPosition() + rad2;
            }
            foreach (var child in children)
            {
                var childExtent = child.GetExtentImpl();
                min = Vector2.Min(min, childExtent.min);
                max = Vector2.Max(max, childExtent.max);
            }
            return (min, max);
        }
        public (Vector2 min, Vector2 max) GetExtent()
        {
            // prevent min > max from being returned.
            var (min, max) = GetExtentImpl();
            if (max.X >= min.X && max.Y >= min.Y)
            {
                return (min, max);
            }
            return (Vector2.Zero, Vector2.Zero);
        }
        public Vector2 GetPosition()
        {
            if (parent != null)
            {
                return parent.GetPosition() + position;
            }
            return position;
        }
        public Texture2D? GetTexture()
        {
            if (imgName != null)
            {
                _texRef = AssetManager.LoadAsset<Texture2D>(imgName);
                imgName = null;
            }
            return _texRef?.Asset;
        }
        public UIMuitipleGradientParam? GetMaterial()
        {
            if (matParam != null)
            {
                _matRef = AssetManager.LoadAsset<UIMuitipleGradientParam>(matParam);
                matParam = null;
            }
            return _matRef?.Asset;
        }
        public void AddChild(ImageLayer child)
        {
            child.parent = this;
            children.Add(child);
        }
        public void Print(int depth = 0)
        {
            //Console.WriteLine($"{new string(' ', depth * 2)}Layer: img={_texRef?.Asset?.m_Name ?? imgName}, mat={_matRef?.Asset?.m_Name ?? matParam}, pos={position}, size={size}, color={color}");
            foreach (var child in children)
            {
                child.Print(depth + 1);
            }
        }
        public bool HasImage()
        {
            return _texRef != null || imgName != null;
        }
        public BundleAssetRef<Texture2D>? _texRef;
        public BundleAssetRef<UIMuitipleGradientParam>? _matRef;
        public bool visible = true;
        public string? imgName;
        public string? matParam;
        public Vector4 color = Vector4.One;
        public Vector2 size = Vector2.Zero;
        public Vector2 position = Vector2.Zero;
        public Vector2 pivot = new(0.5f, 0.5f);
        public ImageLayer? parent;
        public List<ImageLayer> children = [];
    }
    public class DialogHeadRenderer : IDisposable
    {
        private static GameWindow? _window;
        private static int _winCount;
        static int vao = 0;
        static GpuProgram gpuProgram = null!;
        //static GpuProgram gpuProgram2;
        public ImageLayer? root_layer;
        private static void OnDebugMessage(
            DebugSource source,
            DebugType type,
            int id,
            DebugSeverity severity,
            int length,
            IntPtr pMessage,
            IntPtr pUserParam)
        {
            string message = Marshal.PtrToStringUTF8(pMessage, length);
            Console.WriteLine($"[GL Error {id}] {type} from {source} (Severity: {severity}): {message}\n{Environment.StackTrace}");
        }
        public DialogHeadRenderer()
        {
            if (_winCount++ == 0)
            {
                var nativeSettings = new NativeWindowSettings()
                {
                    ClientSize = new OpenTK.Mathematics.Vector2i(1, 1),
                    Title = "Dummy",
                    Flags = ContextFlags.ForwardCompatible | ContextFlags.Debug,
                    StartVisible = false
                };
                _window = new GameWindow(GameWindowSettings.Default, nativeSettings);
                _window.MakeCurrent();
                GL.DebugMessageCallback(OnDebugMessage, IntPtr.Zero);
                GL.Enable(EnableCap.DebugOutput);
                GL.Enable(EnableCap.DebugOutputSynchronous);
                vao = GL.GenVertexArray();

                gpuProgram = new(
                    """
                    #version 330 core
                    uniform mat4 transform;
                    out vec2 v1;
                    out vec2 v2;
                    void main() {
                        vec2 pos = vec2((gl_VertexID & 1), ((gl_VertexID >> 1) & 1));
                        v1 = pos;
                        v1 = (transform * vec4(pos, 1.0, 1.0)).xy;
                        v2 = pos;
                        pos = pos * 2 - 1;
                        gl_Position = vec4(pos, 0.0, 1.0);
                    }
                    """,
                    """
                    #version 330 core
                    in vec2 v1;
                    in vec2 v2;
                    out vec4 FragColor;
                    uniform sampler2D _BgTex;
                    uniform sampler2D _MainTex;
                    uniform vec4 _Color;
                    uniform float _BlurStrength;
                    uniform vec2 _MainTex_TexelSize;
                    uniform float _Brightness;
                    uniform float _Contrast;
                    uniform float _Saturation;
                    uniform float _Angle;
                    uniform float _GraBrightness;
                    uniform float _GraContrast;
                    uniform float _GraSaturation;
                    uniform float _EffectFactor;
                    uniform vec2 _Ratio;
                    uniform vec2 _Center;
                    uniform vec4 _GradientColors[8];
                    uniform float _GradientPositions[8];
                    uniform float _GradientAlphaTop;
                    uniform float _GradientAlphaBottom;
                    uniform float _GradientAlphaTopPosition;
                    uniform float _GradientAlphaBottomPosition;
                    uniform int _ModeSwitch_Linear_Radial;
                    const vec3 LUMA = vec3(0.2125,0.7154,0.0721);
                    #define lum(x) dot(x, LUMA)
                    #define DBG(x) FragColor = vec4(x);return;
                    vec3 linearToSrgb(vec3 linearRGB) {
                        vec3 higher = vec3(1.055) * pow(linearRGB, vec3(1.0 / 2.4)) - vec3(0.055);
                        vec3 lower = linearRGB * vec3(12.92);
                        bvec3 cutoff = lessThan(linearRGB, vec3(0.0031308));
                        return mix(higher, lower, cutoff);
                    }
                    float sRGB_to_Linear(float c) {
                        if (c <= 0.04045) {
                            return c / 12.92;
                        } else {
                            return pow((c + 0.055) / 1.055, 2.4);
                        }
                    }
                    vec3 sRGBToLinear(vec3 sRGB) {
                        return vec3(sRGB_to_Linear(sRGB.r), sRGB_to_Linear(sRGB.g), sRGB_to_Linear(sRGB.b));
                    }
                    const float GAUSS[3] = float[](0.4566, 1.0, 0.4566);
                    vec4 getColor(vec2 uv) {
                        vec4 baseColor;
                        if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1) {
                            return vec4(0.0);
                        }
                        if (_BlurStrength < 0.01) {
                            baseColor = texture(_MainTex, uv);
                        }
                        else {
                            vec2 stepUV = _MainTex_TexelSize.xy * _BlurStrength * 2.0;
                            vec4 sum = vec4(0.0);
                            float weight = 0.0;
                            for (int y = -1; y <= 1; y++)
                            for (int x = -1; x <= 1; x++) {
                                float w = GAUSS[x+1] * GAUSS[y+1];
                                sum += texture(_MainTex, uv + vec2(x, y) * stepUV) * w;
                                weight += w;
                            }
                            baseColor = sum / weight;
                        }
                        float angle = radians(_Angle);
                        vec2 dir = vec2(-sin(angle), cos(angle));
                    
                        float linearT = dot(uv - 0.5, dir) + 0.5;
                    
                        vec2 radial = (uv - _Center) * _Ratio;
                        float radialT = length(radial) * 2.0;
                    
                        float samplePoint = mix(linearT, radialT, float(_ModeSwitch_Linear_Radial));
                        vec3 graColor = _GradientColors[0].xyz;
                        float lastT = 0;
                        for (int i = 0; i < 8; i++) {
                            if (_GradientPositions[i] < lastT) break;
                            if (_GradientPositions[i] >= samplePoint) {
                                graColor = mix(graColor, _GradientColors[i].xyz, (samplePoint - lastT) / max(0.01, _GradientPositions[i] - lastT));
                                break;
                            }
                            graColor = _GradientColors[i].xyz;
                            lastT = _GradientPositions[i];
                        }
                        float graAlphaSample = clamp((samplePoint - _GradientAlphaBottomPosition) / max(0.01, _GradientAlphaTopPosition - _GradientAlphaBottomPosition), 0, 1);
                        float graAlpha = mix(_GradientAlphaBottom, _GradientAlphaTop, graAlphaSample);
                        
                        float baseLum = lum(baseColor.xyz);
                        vec3 adjColor = (_Brightness * baseColor.xyz - baseLum) * _Saturation + baseLum;
                        adjColor = (_Contrast * (adjColor - 0.5)) + 0.5;
                
                        graColor *= _GraBrightness;
                        graColor = (graColor - lum(graColor)) * _GraSaturation + lum(graColor);
                        graColor = (_GraContrast * (graColor - 0.5)) + 0.5;
                
                        vec3 blended = mix(adjColor, adjColor * graColor, graAlpha);
                        return vec4(mix(baseColor.xyz, blended, _EffectFactor), baseColor.a);
                    }
                    void main() {
                        vec4 dest = texture(_BgTex, v2);
                        vec4 src = getColor(vec2(v1.x, 1 - v1.y)) * _Color;
                        if (src.a < 0.001) {
                            FragColor = dest;
                            return;
                        }

                        src.rgb = linearToSrgb(src.rgb);
                        if (dest.a < 0.001) {
                            FragColor = src;
                            return;
                        }

                        float outA = min(1, src.a + dest.a * (1.0 - src.a));
                        vec3 outRGB = (src.rgb * src.a + dest.rgb * dest.a * (1.0 - src.a)) / outA;

                        FragColor = vec4(outRGB, outA);
                    }
                    """
                );
            }
        }
        public void Clear()
        {
            root_layer = null;
        }
        [SupportedOSPlatform("windows6.1")]
        public Bitmap? Draw()
        {
            if (root_layer == null) return null;

            var (min, max) = root_layer.GetExtent();
            if (min == max) return null;

            var size = max - min;
            var center = (min + max) / 2;

            int width = (int)size.X;
            int height = (int)size.Y;

            Matrix4 view = Matrix4.Identity;

            _window!.MakeCurrent();

            int fbo = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);

            int textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f, width, height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.ClampToEdge);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, textureId, 0);
            GL.DrawBuffers(1, [DrawBuffersEnum.ColorAttachment0]);

            GL.Viewport(0, 0, width, height);

            GL.ClearColor(0, 0, 0, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            int fbo2 = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo2);

            int textureId2 = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureId2);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f, width, height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.ClampToEdge);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, textureId2, 0);
            GL.DrawBuffers(1, [DrawBuffersEnum.ColorAttachment0]);

            GL.Viewport(0, 0, width, height);

            GL.ClearColor(0, 0, 0, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            void swapBuffers()
            {
                (fbo, textureId, fbo2, textureId2) = (fbo2, textureId2, fbo, textureId);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
                GL.Viewport(0, 0, width, height);
            }

            Dictionary<string, BundleAssetRef<UIMuitipleGradientParam>?> matParamCache = [];
            void DrawLayer(ImageLayer layer)
            {
                var matParam = layer.matParam;
                BundleAssetRef<UIMuitipleGradientParam>? asset = null;
                if (matParam != null)
                {
                    if (!matParamCache.TryGetValue(matParam, out var cachedAsset))
                    {
                        cachedAsset = AssetManager.LoadAsset<UIMuitipleGradientParam>(matParam + ".asset");
                        matParamCache[matParam] = cachedAsset;
                    }
                    asset = cachedAsset;
                }
                var imgTex = layer.visible ? layer.GetTexture() : null;
                if (imgTex != null)
                {
                    swapBuffers();
                    GL.BindVertexArray(vao);
                    using GpuTexture tex = GpuUtils.LoadUnityTexture(imgTex);
                    gpuProgram.Bind();
                    GL.Disable(EnableCap.DepthTest);
                    GL.DepthMask(false);
                    GL.Disable(EnableCap.Blend);
                    //GL.Enable(EnableCap.FramebufferSrgb);

                    int locTex = GL.GetUniformLocation(gpuProgram.programID, "_MainTex");
                    tex.BindToUnit(0, locTex);
                    int bgTex = GL.GetUniformLocation(gpuProgram.programID, "_BgTex");
                    GL.ActiveTexture(TextureUnit.Texture1);
                    GL.BindTexture(TextureTarget.Texture2D, textureId2);
                    GL.Uniform1(bgTex, 1);
                    int locTransform = GL.GetUniformLocation(gpuProgram.programID, "transform");
                    int locColor = GL.GetUniformLocation(gpuProgram.programID, "_Color");

                    var layerSize = layer.GetSize();
                    var worldPos = layer.GetPosition() - center + size / 2 - layerSize * layer.pivot;
                    worldPos /= size;
                    var scale = Matrix4.CreateScale(layerSize.X / (float)width, layerSize.Y / (float)height, 1.0f);
                    var translate = Matrix4.CreateTranslation(worldPos.X, worldPos.Y, 0.0f);
                    var transform = scale * translate;
                    transform.Invert();

                    GL.UniformMatrix4(locTransform, false, ref transform);
                    var layerCol = new OpenTK.Mathematics.Vector4(layer.color.X, layer.color.Y, layer.color.Z, layer.color.W);
                    GL.Uniform4(locColor, layerCol);

                    if (asset != null)
                    {
                        var matData = asset.Asset;
                        gpuProgram.SetUniform("_MainTex_TexelSize", 1.0f / imgTex.m_Width, 1.0f / imgTex.m_Height);
                        gpuProgram.SetUniform("_BlurStrength", matData.EnableFastBlur != 0 ? matData.BlurStrength : 0.0f);
                        gpuProgram.SetUniform("_Brightness", matData.Brightness);
                        gpuProgram.SetUniform("_Contrast", matData.Contrast);
                        gpuProgram.SetUniform("_Saturation", matData.Saturation);
                        gpuProgram.SetUniform("_Angle", matData.Angle);
                        gpuProgram.SetUniform("_GraBrightness", matData.GraBrightness);
                        gpuProgram.SetUniform("_GraContrast", matData.GraContrast);
                        gpuProgram.SetUniform("_GraSaturation", matData.GraSaturation);
                        gpuProgram.SetUniform("_EffectFactor", matData.EffectFactor);
                        gpuProgram.SetUniform("_Ratio", matData.Ratio.X, matData.Ratio.Y);
                        gpuProgram.SetUniform("_Center", matData.Center.X, matData.Center.Y);
                        gpuProgram.SetUniform("_ModeSwitch_Linear_Radial", matData.gradientMode == UIMuitipleGradientParam.GradientMode.Linear ? 0 : 1);
                        int locGradientColors = GL.GetUniformLocation(gpuProgram.programID, "_GradientColors");
                        int locGradientPositions = GL.GetUniformLocation(gpuProgram.programID, "_GradientPositions");
                        var colors = matData.gradient.colorKeys.SelectMany(x => new[] { x.color.r, x.color.g, x.color.b, x.color.a }).ToArray();
                        //var colors = matData.gradient.colorKeys.SelectMany(x => new[] { linearToSrgb(x.color.r), linearToSrgb(x.color.g), linearToSrgb(x.color.b), 1.0f }).ToArray();
                        var stops = matData.gradient.colorKeys.Select(x => x.time).ToArray();
                        GL.Uniform4(locGradientColors, 8, colors);
                        GL.Uniform1(locGradientPositions, 8, stops);
                        var firstAlphaKey = matData.gradient.alphaKeys.First();
                        var lastAlphaKey = matData.gradient.alphaKeys.Last();
                        gpuProgram.SetUniform("_GradientAlphaBottom", firstAlphaKey.alpha);
                        gpuProgram.SetUniform("_GradientAlphaTop", lastAlphaKey.alpha);
                        gpuProgram.SetUniform("_GradientAlphaBottomPosition", firstAlphaKey.time);
                        gpuProgram.SetUniform("_GradientAlphaTopPosition", lastAlphaKey.time);
                    }
                    else
                    {
                        gpuProgram.SetUniform("_EffectFactor", 0.0f);
                    }

                    GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
                }
                foreach (var child in layer.children)
                {
                    DrawLayer(child);
                }
            }
            DrawLayer(root_layer);
            //{
            //    swapBuffers();
            //    gpuProgram2.Bind();
            //    int bgTex = GL.GetUniformLocation(gpuProgram2.programID, "_BgTex");
            //    GL.ActiveTexture(TextureUnit.Texture0);
            //    GL.BindTexture(TextureTarget.Texture2D, textureId2);
            //    GL.Uniform1(bgTex, 0);
            //    GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
            //}

            byte[] pixels = new byte[width * height * 4];
            GL.ReadPixels(0, 0, width, height, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, pixels);

            Bitmap? finalImg = new(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var data = finalImg.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, finalImg.PixelFormat);
            Marshal.Copy(pixels, 0, data.Scan0, pixels.Length);
            finalImg.UnlockBits(data);

            //finalImg.RotateFlip(RotateFlipType.RotateNoneFlipY);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.DeleteFramebuffer(fbo);
            GL.DeleteTexture(textureId);
            GL.DeleteFramebuffer(fbo2);
            GL.DeleteTexture(textureId2);
            GL.BindVertexArray(0);

            return finalImg;
        }
        public void Dispose()
        {
            if (--_winCount == 0)
            {
                GL.DeleteVertexArray(vao);
                _window!.Close();
                _window.Dispose();
                gpuProgram.Dispose();
                //gpuProgram2.Dispose();
            }
            GC.SuppressFinalize(this);
        }
        ~DialogHeadRenderer()
        {
            Console.WriteLine("Warning: GC on undisposed DialogHeadRenderer");
            Dispose();
        }
    }
}
