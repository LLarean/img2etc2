#if UNITY_EDITOR
using UnityEngine;
using System.IO;

namespace LLarean.IMG2ETC2
{
    /// <summary>
    /// For working with image data
    /// </summary>
    public static class ImageUtils
    {
        /// <summary>
        /// Getting images and preparing data
        /// </summary>
        /// <param name="filePath">The path for getting the image</param>
        /// <returns>Image data for working with them</returns>
        public static ImageModel GetModel(string filePath)
        {
            var texture = LoadTexture(filePath);
            var status = GetResolutionStatus(texture.width, texture.height);
            var size = $"({texture.width},{texture.height})";

            Object.DestroyImmediate(texture);

            return new ImageModel
            {
                FilePath = filePath,
                ResolutionStatus = status,
                CurrentResolution = size,
                PreviousResolution = size,
            };
        }

        /// <summary>
        /// Get a number that will be a multiple of 4 after the specified
        /// </summary>
        /// <param name="value">The original number</param>
        /// <returns>Returns a multiple of 4</returns>
        public static int RoundToNearestFour(int value) => (value + 3) / 4 * 4;

        /// <summary>
        /// Changes the resolution of the texture size to the required size for compression to work correctly
        /// </summary>
        /// <param name="source">The original texture</param>
        /// <param name="target">The new texture is a multiple of four</param>
        public static void ResizeTexture(Texture2D source, Texture2D target)
        {
            var renderTexture = RenderTexture.GetTemporary(target.width, target.height, 0, RenderTextureFormat.ARGB32);
            RenderTexture.active = renderTexture;
            
            Graphics.Blit(source, renderTexture);
            target.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0);
            target.Apply();
            
            RenderTexture.ReleaseTemporary(renderTexture);
            RenderTexture.active = null;
        }
        
        private static Texture2D LoadTexture(string path)
        {
            var texture = new Texture2D(2, 2);
            texture.LoadImage(File.ReadAllBytes(path));
            return texture;
        }

        private static ResolutionStatus GetResolutionStatus(int width, int height)
        {
            if (width % 4 != 0) return ResolutionStatus.Wrong;
            if (height % 4 != 0) return ResolutionStatus.Wrong;
            return ResolutionStatus.Correct;
        }
    }
}
#endif