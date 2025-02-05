#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace LLarean.IMG2ETC2
{
    public class IMG2ETC2Window : EditorWindow
    {
        private readonly ImageLoader _imageLoader = new();
        private readonly ImageProcessor _imageProcessor = new();
        
        private List<ImageModel> _imageModels = new();
        private string _folderPath = string.Empty;
        private bool _includeSubfolders = true;
        private Vector2 _scrollPosition;
        
        [MenuItem(GlobalStrings.Tools + GlobalStrings.UtilityName)]
        private static void ShowWindow() => GetWindow<IMG2ETC2Window>(GlobalStrings.UtilityName);

        private void OnEnable() => _folderPath = Application.dataPath;

        private void OnGUI()
        {
            DrawFolderSettings();
            DrawActionButtons();
            DrawImageList();
        }

        private void DrawFolderSettings()
        {
            _folderPath = EditorGUILayout.TextField(GlobalStrings.FolderPath, _folderPath);
            _includeSubfolders = EditorGUILayout.Toggle(GlobalStrings.IncludeSubfolders, _includeSubfolders);

            if (GUILayout.Button(GlobalStrings.SelectFolder) == true)
            {
                SelectFolder();
            }

            if (Event.current.keyCode == KeyCode.Return)
            {
                _folderPath = EditorGUILayout.TextField(GlobalStrings.FolderPath, _folderPath);
                LoadImages();
            }
        }

        private void DrawActionButtons()
        {
            if (_imageModels.Count == 0) return;
            
            GUILayout.Space(10);

            if (GUILayout.Button(GlobalStrings.ResizeImages) == true)
            {
                _imageProcessor.ResizeImages(_imageModels);
            }
        }

        private void DrawImageList()
        {
            if (_imageModels.Count == 0) return;
            
            GUILayout.Space(10);
            GUILayout.Label(GlobalStrings.ImagesIn, EditorStyles.boldLabel);
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            
            foreach (var model in _imageModels)
            {
                GUIUtils.DrawImageModel(model, _imageModels.IndexOf(model) + 1);
            }
            
            GUILayout.EndScrollView();
        }

        private void SelectFolder()
        {
            var path = EditorUtility.OpenFolderPanel(GlobalStrings.SelectFolder, _folderPath, "");
            
            if (string.IsNullOrEmpty(path) == false)
            {
                _folderPath = path;
                LoadImages();
            }
        }

        private void LoadImages() => _imageModels = _imageLoader.LoadImages(_folderPath, _includeSubfolders);
    }
}
#endif