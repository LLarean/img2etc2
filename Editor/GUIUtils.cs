#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace LLarean.IMG2ETC2
{
    public static class GUIUtils
    {
        private static GUIStyle _richTextStyle;
        private static GUIStyle RichTextStyle =>
            _richTextStyle ??= new GUIStyle(GUI.skin.label) { richText = true };

        public static void DrawImageModel(ImageModel model, int imageNumber)
        {
            var color = GetStatusColor(model.ResolutionStatus);
            var resolutionText = GetResolutionText(model);

            GUILayout.Label(
                $"<color={color}>{model.ResolutionStatus}</color> {imageNumber} - {model.FilePath} {resolutionText}",
                RichTextStyle
            );
        }

        public static void UpdateResizeProgress(int imageIndex, int listCount, string windowTitle)
        {
            var info = $"Progress: {imageIndex}/{listCount}";
            var progress = imageIndex / (float)listCount;
            EditorUtility.DisplayProgressBar(windowTitle, info, progress);
        }

        public static void ClearProgress() => EditorUtility.ClearProgressBar();

        private static string GetStatusColor(ResolutionStatus status) => status switch
        {
            ResolutionStatus.Correct => "green",
            ResolutionStatus.Wrong   => "yellow",
            _                        => "white",
        };

        private static string GetResolutionText(ImageModel model)
        {
            if (model.ResolutionStatus == ResolutionStatus.Wrong)
                return $"<color=white>{model.CurrentResolution} → {model.TargetResolution}</color>";

            if (model.CurrentResolution != model.PreviousResolution)
                return $"<color=white>{model.PreviousResolution} → {model.CurrentResolution}</color>";

            return $"<color=white>{model.CurrentResolution}</color>";
        }
    }
}
#endif