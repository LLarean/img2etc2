# UnityIMG2ETC2 ![unity](https://img.shields.io/badge/Unity-100000?style=for-the-badge&logo=unity&logoColor=white)

[![Releases](https://img.shields.io/github/v/release/llarean/img2etc2)](https://github.com/llarean/img2etc2/releases)
![stability-stable](https://img.shields.io/badge/stability-stable-green.svg)
[![License: APACHE](https://img.shields.io/badge/License-APACHE-yellow.svg)](https://opensource.org/license/apache-2-0)
[![Maintenance](https://img.shields.io/badge/Maintained%3F-yes-green.svg)](https://GitHub.com/Naereen/StrapDown.js/graphs/commit-activity)


A utility that allows you to bring the image resolution to a multiple of 4 for compression to work correctly.

ETC is a block-based texture compression format. 
The image is split up into 4x4 blocks, and each block is encoded using a fixed number of bits.
Only textures with width/height being multiple of 4 can be compressed to Crunch format.

You can get acquainted with the parameters on the Unity3D [documentation](https://docs.unity3d.com/2023.2/Documentation/Manual/class-TextureImporterOverride.html), or read this [post](https://unity.com/ru/blog/engine-platform/crunch-compression-of-etc-textures).

## INSTALLATION

There are 4 ways to install this utility:

- import [UnityIMG2ETC2.unitypackage](https://github.com/llarean/img2etc2/releases) via *Assets-Import Package*
- clone/[download](https://github.com/llarean/img2etc2/archive/master.zip) this repository and move files to your Unity project's *Assets* folder
- import it from [Asset Store](https://assetstore.unity.com/preview/315139/1042556)
- *(via Package Manager)* Select Add package from git URL from the add menu. A text box and an Add button appear. Enter a valid Git URL in the text box:
  - `https://github.com/llarean/img2etc2.git`
- *(via Package Manager)* add the following line to *Packages/manifest.json*:
  - `"com.llarean.img2etc2": "https://github.com/llarean/img2etc2.git",`

## Warning

- The utility changes the size of images. Before applying it, make sure you make a backup copy or create a commit.
- Image resolution data may not be updated immediately in the image information in editor.

## HOW TO

1. Open the utility (context menu: `Tools > UnityIMG2ETC2`)


2. Select a folder (2 ways):
- Click on the "Select folder" button; or
- Specify "Folder path:" in the input field;


3. Check the list of files


4. Click on the "Resize images" button

![Window](https://github.com/LLarean/img2etc2/blob/main/Preview.png?raw=true)

## Special thanks

[Nintoryan](https://github.com/Nintoryan) (For your inspiration and main idea )
