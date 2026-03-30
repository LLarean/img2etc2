#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace LLarean.IMG2ETC2
{
    public class ImageProcessor
    {
        public void ResizeImages(List<ImageModel> imageModels, RoundingMode roundingMode)
        {
            for (int i = 0; i < imageModels.Count; i++)
            {
                var model = imageModels[i];
                if (model.ResolutionStatus == ResolutionStatus.Wrong)
                    ResizeImage(model, roundingMode);

                GUIUtils.UpdateResizeProgress(i, imageModels.Count, model.FilePath);
            }

            GUIUtils.ClearProgress();
        }

        private void ResizeImage(ImageModel model, RoundingMode roundingMode)
        {
            var original = ImageUtils.LoadTexture(model.FilePath);
            var newWidth = ImageUtils.RoundToMultipleOfFour(original.width, roundingMode);
            var newHeight = ImageUtils.RoundToMultipleOfFour(original.height, roundingMode);
            var resized = new Texture2D(newWidth, newHeight, TextureFormat.ARGB32, false);

            ImageUtils.ResizeTexture(original, resized);
            SaveAsPng(model, resized);
            UpdateModel(model, newWidth, newHeight);
            DestroyTextures(original, resized);
        }

        private void SaveAsPng(ImageModel model, Texture2D resized)
        {
            var pngPath = Path.ChangeExtension(model.FilePath, ".png");
            File.WriteAllBytes(pngPath, resized.EncodeToPNG());

            if (!string.Equals(model.FilePath, pngPath, System.StringComparison.OrdinalIgnoreCase))
            {
                File.Delete(model.FilePath);
                model.FilePath = pngPath;
            }
        }

        private void UpdateModel(ImageModel model, int newWidth, int newHeight)
        {
            model.PreviousResolution = model.CurrentResolution;
            model.CurrentResolution = $"({newWidth},{newHeight})";
            model.TargetResolution = model.CurrentResolution;
            model.ResolutionStatus = ResolutionStatus.Correct;
        }

        private void DestroyTextures(params Texture2D[] textures)
        {
            foreach (var texture in textures)
                if (texture != null) Object.DestroyImmediate(texture);
        }
    }
}
#endif
