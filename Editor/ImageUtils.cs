#if UNITY_EDITOR
using UnityEngine;
using System.IO;

namespace LLarean.IMG2ETC2
{
    public static class ImageUtils
    {
        public static ImageModel GetModel(string filePath, RoundingMode roundingMode)
        {
            var texture = LoadTexture(filePath);
            var status = GetResolutionStatus(texture.width, texture.height);
            var current = FormatResolution(texture.width, texture.height);
            var targetWidth = RoundToMultipleOfFour(texture.width, roundingMode);
            var targetHeight = RoundToMultipleOfFour(texture.height, roundingMode);
            var target = FormatResolution(targetWidth, targetHeight);

            Object.DestroyImmediate(texture);

            return new ImageModel
            {
                FilePath = filePath,
                ResolutionStatus = status,
                CurrentResolution = current,
                PreviousResolution = current,
                TargetResolution = target,
            };
        }

        public static int RoundToMultipleOfFour(int value, RoundingMode mode) => mode switch
        {
            RoundingMode.Up   => (value + 3) / 4 * 4,
            RoundingMode.Down => value / 4 * 4,
            _                 => value,
        };

        public static void ResizeTexture(Texture2D source, Texture2D target)
        {
            var rt = RenderTexture.GetTemporary(target.width, target.height, 0, RenderTextureFormat.ARGB32);
            RenderTexture.active = rt;
            Graphics.Blit(source, rt);
            target.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0);
            target.Apply();
            RenderTexture.ReleaseTemporary(rt);
            RenderTexture.active = null;
        }

        internal static Texture2D LoadTexture(string path)
        {
            var texture = new Texture2D(2, 2);
            texture.LoadImage(File.ReadAllBytes(path));
            return texture;
        }

        private static string FormatResolution(int width, int height) => $"({width},{height})";

        private static ResolutionStatus GetResolutionStatus(int width, int height)
        {
            if (width % 4 != 0) return ResolutionStatus.Wrong;
            if (height % 4 != 0) return ResolutionStatus.Wrong;
            return ResolutionStatus.Correct;
        }
    }
}
#endif