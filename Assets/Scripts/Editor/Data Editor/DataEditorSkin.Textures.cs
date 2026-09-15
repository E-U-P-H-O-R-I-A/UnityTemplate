using System.Collections.Generic;
using UnityEngine;

namespace EditorTools.DataEditor
{
    public sealed partial class DataEditorSkin
    {
        private readonly List<Texture2D> textures = new();

        public bool IsValid => textures.Count == 0 || textures[0] != null;

        public void Dispose()
        {
            foreach (var texture in textures)
            {
                if (texture != null)
                    Object.DestroyImmediate(texture);
            }

            textures.Clear();
        }

        private Texture2D Solid(Color color)
        {
            var texture = CreateTexture(1);
            texture.SetPixel(0, 0, color);
            texture.Apply();

            return texture;
        }

        private Texture2D Rounded(Color fill, Color border, int radius)
        {
            const int size = 24;
            const float outline = 1.25f;

            var texture = CreateTexture(size);
            var pixels = new Color[size * size];

            float half = size * 0.5f;
            float clampedRadius = Mathf.Clamp(radius, 1, (int)half);
            bool hasBorder = border.a > 0.001f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Max(Mathf.Abs(x + 0.5f - half) - (half - clampedRadius), 0f);
                    float dy = Mathf.Max(Mathf.Abs(y + 0.5f - half) - (half - clampedRadius), 0f);
                    float distance = Mathf.Sqrt(dx * dx + dy * dy) - clampedRadius;

                    float coverage = Mathf.Clamp01(0.5f - distance);
                    float borderWeight = hasBorder ? Mathf.Clamp01(distance + outline) : 0f;

                    var color = Color.Lerp(fill, border, borderWeight);
                    color.a = Mathf.Lerp(fill.a, hasBorder ? border.a : fill.a, borderWeight) * coverage;

                    pixels[y * size + x] = color;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            return texture;
        }

        private Texture2D TrayArrowIcon(bool pointingUp, Color color)
        {
            const int size = 16;
            const int samples = 4;

            var texture = CreateTexture(size);
            var pixels = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int hits = 0;

                    for (int subY = 0; subY < samples; subY++)
                    {
                        for (int subX = 0; subX < samples; subX++)
                        {
                            float sampleX = x + (subX + 0.5f) / samples;
                            float sampleY = y + (subY + 0.5f) / samples;

                            if (IsInsideTrayArrow(sampleX, sampleY, pointingUp))
                                hits++;
                        }
                    }

                    var pixel = color;
                    pixel.a = color.a * hits / (samples * samples);
                    pixels[y * size + x] = pixel;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            return texture;
        }

        private static bool IsInsideTrayArrow(float x, float y, bool pointingUp)
        {
            if (IsInsideBox(x, y, 2f, 14f, 2f, 3.6f))
                return true;

            if (IsInsideBox(x, y, 2f, 3.6f, 2f, 7f) || IsInsideBox(x, y, 12.4f, 14f, 2f, 7f))
                return true;

            const float centerX = 8f;
            const float stemHalfWidth = 1.1f;
            const float headHalfWidth = 3.5f;

            return pointingUp
                ? IsInsideBox(x, y, centerX - stemHalfWidth, centerX + stemHalfWidth, 5.5f, 10.5f) ||
                  IsInsideArrowHead(x, y, centerX, 14.5f, 10.5f, headHalfWidth)
                : IsInsideBox(x, y, centerX - stemHalfWidth, centerX + stemHalfWidth, 9.5f, 14.5f) ||
                  IsInsideArrowHead(x, y, centerX, 5.5f, 9.5f, headHalfWidth);
        }

        private static bool IsInsideBox(float x, float y, float left, float right, float bottom, float top)
        {
            return x >= left && x <= right && y >= bottom && y <= top;
        }

        private static bool IsInsideArrowHead(float x, float y, float centerX, float apexY, float baseY,
            float halfWidth)
        {
            float progress = (apexY - y) / (apexY - baseY);

            if (progress < 0f || progress > 1f)
                return false;

            return Mathf.Abs(x - centerX) <= halfWidth * progress;
        }

        private Texture2D CreateTexture(int size)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                hideFlags = HideFlags.HideAndDontSave,
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            textures.Add(texture);
            return texture;
        }
    }
}
