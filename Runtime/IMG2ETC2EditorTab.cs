using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace LLarean.IMG2ETC2
{
    public class Img2ETC2EditorTab : EditorWindow
    {
        private const string UtilityName = "IMG2ETC2";

        private readonly List<ImageModel> _imageModels = new();

        private string _folderPath = string.Empty;

        #region Displayed GUI

        [MenuItem("Window/" + UtilityName)]
        private static void ShowWindow()
        {
            GetWindow<Img2ETC2EditorTab>(UtilityName);
        }

        private void OnEnable()
        {
            _folderPath = Application.dataPath;
        }

        private void OnGUI()
        {
            _folderPath = EditorGUILayout.TextField("Folder path:", _folderPath);

            if (Event.current.keyCode == KeyCode.Return)
            {
                LoadImagesFromFolder(_folderPath);
            }

            if (GUILayout.Button("Select folder") == true)
            {
                SelectFolder();
            }

            if (_imageModels.Count > 0)
            {
                GUILayout.Space(10);

                if (GUILayout.Button("Resize images") == true)
                {
                    ResizeImages();
                }

                GUILayout.Space(10);
                GUILayout.Label("Images in selected folder:", EditorStyles.boldLabel);
                DisplayImageModels();
            }
        }

        #endregion

        #region GUI Logic

        private void SelectFolder()
        {
            string folderPath = EditorUtility.OpenFolderPanel("Select folder", _folderPath, "");

            if (string.IsNullOrEmpty(folderPath) == false)
            {
                _folderPath = folderPath;
                LoadImagesFromFolder(folderPath);
            }
        }

        private void LoadImagesFromFolder(string path)
        {
            _imageModels.Clear();
            string[] filePaths = Directory.GetFiles(path);

            for (int i = 0; i < filePaths.Length; i++)
            {
                filePaths[i] = filePaths[i].Replace("\\", "/");
            }

            foreach (string filePath in filePaths)
            {
                if (filePath.EndsWith(".png") || filePath.EndsWith(".jpg") || filePath.EndsWith(".jpeg"))
                {
                    ImageModel imageModel = GetImageModel(filePath);
                    _imageModels.Add(imageModel);
                }
            }
        }

        private void DisplayImageModels()
        {
            foreach (var image in _imageModels)
            {
                var color = GetStatusColor(image.ImageStatus);
                var imageSize = string.Empty;


                if (image.CurrentSize == image.OldSize)
                {
                    imageSize = $"(Size: {image.CurrentSize})";
                }
                else
                {
                    imageSize = $"(Size: {image.OldSize} => {image.CurrentSize})";
                }

                GUILayout.Label($"<color={color}>{image.ImageStatus}</color> {image.FilePath} {imageSize}",
                    new GUIStyle { richText = true });
            }
        }

        #endregion

        #region Changing the size

        private void ResizeImages()
        {
            foreach (var imageModel in _imageModels)
            {
                if (imageModel.ImageStatus == ImageStatus.Wrong)
                {
                    Texture2D original = new Texture2D(2, 2);
                    byte[] imageData = File.ReadAllBytes(imageModel.FilePath);
                    original.LoadImage(imageData);

                    int newWidth = RoundToNearestFour(original.width);
                    int newHeight = RoundToNearestFour(original.height);

                    Texture2D resized = new Texture2D(newWidth, newHeight);
                    ResizeTexture(original, resized);

                    byte[] resizedData = resized.EncodeToPNG();
                    File.WriteAllBytes(imageModel.FilePath, resizedData);

                    var tempImageModel = GetImageModel(imageModel.FilePath);
                    imageModel.ImageStatus = tempImageModel.ImageStatus;
                    imageModel.CurrentSize = tempImageModel.CurrentSize;

                    DestroyImmediate(original);
                    DestroyImmediate(resized);
                }
            }

            EditorUtility.DisplayDialog("Action Complete", "Actions have been performed on the images", "OK");
        }

        private void ResizeTexture(Texture2D source, Texture2D target)
        {
            Color[] newPixels = new Color[target.width * target.height];

            float scaleX = (float)source.width / target.width;
            float scaleY = (float)source.height / target.height;

            for (int y = 0; y < target.height; y++)
            {
                for (int x = 0; x < target.width; x++)
                {
                    int srcX = Mathf.FloorToInt(x * scaleX);
                    int srcY = Mathf.FloorToInt(y * scaleY);
                    newPixels[y * target.width + x] = source.GetPixel(Mathf.Min(srcX, source.width - 1),
                        Mathf.Min(srcY, source.height - 1));
                }
            }

            target.SetPixels(newPixels);
            target.Apply();
        }

        #endregion

        #region Auxiliary methods

        private ImageModel GetImageModel(string filePath)
        {
            byte[] imageData = File.ReadAllBytes(filePath);
            Texture2D original = new Texture2D(2, 2);
            original.LoadImage(imageData);

            int originalWidth = original.width;
            int originalHeight = original.height;

            var currentSize = $"({originalWidth},{originalHeight})";
            ImageStatus imageStatus = GetImageStatus(originalWidth, originalHeight);

            ImageModel imageModel = new ImageModel
            {
                FilePath = string.Empty,
                ImageStatus = imageStatus,
                CurrentSize = currentSize,
                OldSize = currentSize,
            };

            DestroyImmediate(original);

            return imageModel;
        }

        private int RoundToNearestFour(int value)
        {
            return (value + 3) / 4 * 4;
        }

        private ImageStatus GetImageStatus(int width, int height)
        {
            if (width % 4 == 0 && height % 4 == 0)
            {
                return ImageStatus.Correct;
            }

            return ImageStatus.Wrong;
        }

        private string GetStatusColor(ImageStatus imageStatus)
        {
            return imageStatus switch
            {
                ImageStatus.Correct => "green",
                ImageStatus.Wrong => "yellow",
                ImageStatus.Error => "red",
                _ => "white",
            };
        }

        #endregion
    }
}