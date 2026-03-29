#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;

namespace LLarean.IMG2ETC2
{
    public class ImageLoader
    {
        public List<ImageModel> LoadImages(string folderPath, bool includeSubfolders, RoundingMode roundingMode)
        {
            var imageModels = new List<ImageModel>();
            var searchOption = includeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            foreach (var filePath in Directory.GetFiles(folderPath, "*.*", searchOption))
            {
                var normalizedPath = filePath.Replace("\\", "/");
                if (IsSupportedExtension(normalizedPath))
                    imageModels.Add(ImageUtils.GetModel(normalizedPath, roundingMode));
            }

            return imageModels;
        }

        private bool IsSupportedExtension(string path) =>
            path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".jpeg");
    }
}
#endif
