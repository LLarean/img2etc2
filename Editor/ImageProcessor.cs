#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace LLarean.IMG2ETC2
{
    public class ImageProcessor
    {
        public void ResizeImages(List<ImageModel> imageModels)
        {
            for (int i = 0; i < imageModels.Count; i++)
            {
                var imageModel = imageModels[i];
                if (imageModel.ResolutionStatus == ResolutionStatus.Wrong)
                {
                    var (original, resized) = ProcessImage(imageModel.FilePath);
                    SaveResizedImage(imageModel, resized);
                    UpdateModelStatus(imageModel);
                    DestroyTextures(original, resized);
                }

                var filePath = imageModels[i].FilePath;
                GUIUtils.UpdateResizeProgress(i, imageModels.Count, filePath);
            }

            GUIUtils.ClearProgress();
        }

        private (Texture2D, Texture2D) ProcessImage(string path)
        {
            var original = LoadTexture(path);
            var newWidth = ImageUtils.RoundToNearestFour(original.width);
            var newHeight = ImageUtils.RoundToNearestFour(original.height);
            var resized = new Texture2D(newWidth, newHeight, TextureFormat.ARGB32, false);
            ImageUtils.ResizeTexture(original, resized);
            return (original, resized);
        }

        private Texture2D LoadTexture(string path)
        {
            var texture = new Texture2D(2, 2);
            texture.LoadImage(File.ReadAllBytes(path));
            return texture;
        }

        private void SaveResizedImage(ImageModel model, Texture2D resized)
        {
            File.WriteAllBytes(model.FilePath, resized.EncodeToPNG());
        }

        private void UpdateModelStatus(ImageModel model)
        {
            var updatedModel = ImageUtils.GetModel(model.FilePath);
            model.ResolutionStatus = updatedModel.ResolutionStatus;
            model.CurrentResolution = updatedModel.CurrentResolution;
        }

        private void DestroyTextures(params Texture2D[] textures)
        {
            foreach (var texture in textures)
            {
                if (texture != null) Object.DestroyImmediate(texture);
            }
        }
    }
}
#endif