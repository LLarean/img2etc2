#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace LLarean.IMG2ETC2
{
    /// <summary>
    /// Auxiliary class for rendering data in GUILayout
    /// </summary>
    public static class GUIUtils
    {
        /// <summary>
        /// Displays the image data
        /// </summary>
        /// <param name="imageModel">Image Data</param>
        /// <param name="imageNumber">The number of the image in the list (not the index!)</param>
        public static void DrawImageModel(ImageModel imageModel, int imageNumber)
        {
            string color = GetStatusColor(imageModel.ResolutionStatus);
            string resolutionText = GetResolutionText(imageModel.CurrentResolution, imageModel.PreviousResolution);

            GUILayout.Label(
                $"<color={color}>{imageModel.ResolutionStatus}</color> {imageNumber} - {imageModel.FilePath} {resolutionText}",
                new GUIStyle { richText = true }
            );
        }
        
        /// <summary>
        /// Displaying the progress of resizing images in the window
        /// </summary>
        /// <param name="imageIndex">The index of the current image being worked with</param>
        /// <param name="listCount">The count of the list of images that you are working with</param>
        /// <param name="windowTitle">The title of the window that displays the progress</param>
        public static void UpdateResizeProgress(int imageIndex, int listCount, string windowTitle)
        {
            string info = $"Progress: {imageIndex}/{listCount}";
            float progress = imageIndex / (float)listCount;
            EditorUtility.DisplayProgressBar(windowTitle, info, progress);
        }

        /// <summary>
        /// Removes the progress bar
        /// </summary>
        public static void ClearProgress() => EditorUtility.ClearProgressBar();
        
        /// <summary>
        /// Color to display the ready status of the image
        /// </summary>
        /// <param name="resolutionStatus"><see cref="ResolutionStatus"/></param>
        /// <returns>String color name</returns>
        private static string GetStatusColor(ResolutionStatus resolutionStatus)
        {
            string color = resolutionStatus switch
            {
                ResolutionStatus.Correct => "green",
                ResolutionStatus.Wrong => "yellow",
                _ => "white"
            };

            return color;
        }

        /// <summary>
        /// To format data related to image resolution
        /// </summary>
        /// <param name="currentResolution">Current image resolution</param>
        /// <param name="previousResolution">Previous image resolution</param>
        /// <returns>Returns a string with image resolution data</returns>
        private static string GetResolutionText(string currentResolution, string previousResolution)
        {
            string resolutionText = $"Resolution: {currentResolution}";

            if (currentResolution != previousResolution)
            {
                resolutionText = $"{resolutionText} => {currentResolution}";
            }

            resolutionText = $"<color=white>{resolutionText}</color>";
            return resolutionText;
        }
    }
}
#endif