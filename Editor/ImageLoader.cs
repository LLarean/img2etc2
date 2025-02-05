#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;

namespace LLarean.IMG2ETC2
{
    /// <summary>
    /// To download images from the file system
    /// </summary>
    public class ImageLoader
    {
        /// <summary>
        /// Allows you to prepare images for work
        /// </summary>
        /// <param name="folderPath">The address of the folder from which the images will be uploaded</param>
        /// <param name="includeSubfolders">Will subfolders be taken into account when searching for files</param>
        /// <returns>A list with image data</returns>
        public List<ImageModel> LoadImages(string folderPath, bool includeSubfolders)
        {
            List<ImageModel> imageModels = new List<ImageModel>();
            SearchOption searchOption = includeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            string[] filePaths = Directory.GetFiles(folderPath, "*.*", searchOption);

            foreach (string filePath in filePaths)
            {
                string normalizedPath = filePath.Replace("\\", "/");
                
                if (GetFileExtensionSupportStatus(normalizedPath) == true)
                {
                    ImageModel imageModel = ImageUtils.GetModel(normalizedPath);
                    imageModels.Add(imageModel);
                }
            }

            return imageModels;
        }

        /// <summary>
        /// Supported image types
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool GetFileExtensionSupportStatus(string path)
        {
            return path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".jpeg");
        }
    }
}
#endif