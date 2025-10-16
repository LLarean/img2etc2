# ![unity](https://img.shields.io/badge/Unity-100000?style=for-the-badge&logo=unity&logoColor=white) IMG2ETC2 

[![Releases](https://img.shields.io/github/v/release/llarean/img2etc2)](https://github.com/llarean/img2etc2/releases)
[![License: APACHE](https://img.shields.io/badge/License-APACHE-yellow.svg)](https://github.com/LLarean/img2etc2/blob/main/LICENSE.md)
![stability-stable](https://img.shields.io/badge/stability-stable-green.svg)
[![CodeFactor](https://www.codefactor.io/repository/github/llarean/img2etc2/badge)](https://www.codefactor.io/repository/github/llarean/img2etc2)
[![Support](https://img.shields.io/badge/support-maintenance_mode-orange)](https://github.com/llarean/img2etc2/graphs/commit-activity)
[![Downloads](https://img.shields.io/github/downloads/llarean/img2etc2/total)](https://github.com/LLarean/img2etc2/archive/refs/heads/main.zip)
[![Asset Store](https://img.shields.io/badge/Asset_Store-300+_downloads-blue)](https://assetstore.unity.com/preview/315139/1042556)

Automatically resize Unity images for optimal ETC2/Crunch compression. Ensures all textures have dimensions divisible by 4, enabling efficient mobile texture compression with transparent padding.

Perfect for mobile game developers who need maximum compression efficiency without manual image editing.

**Technical details**: ETC2 is a block-based compression format that processes images in 4x4 pixel blocks. Learn more in Unity's [documentation](https://docs.unity3d.com/2023.2/Documentation/Manual/class-TextureImporterOverride.html) or this [technical post](https://unity.com/ru/blog/engine-platform/crunch-compression-of-etc-textures).

## Quick Start
1. Install via Package Manager: `https://github.com/llarean/img2etc2.git`
2. Open `Tools > UnityIMG2ETC2`
3. Select your textures folder
4. Click "Resize images"

**Result**: All images resized to ETC2-compatible dimensions (multiples of 4)

## INSTALLATION

There are 4 ways to install this utility:

- import [UnityIMG2ETC2.unitypackage](https://github.com/llarean/img2etc2/releases) via *Assets-Import Package*
- clone/[download](https://github.com/llarean/img2etc2/archive/master.zip) this repository and move files to your Unity project's *Assets* folder
- import it from [Asset Store](https://assetstore.unity.com/preview/315139/1042556)
- *(via Package Manager)* Select Add package from git URL from the add menu. A text box and an Add button appear. Enter a valid Git URL in the text box:
  - `https://github.com/llarean/img2etc2.git`
- *(via Package Manager)* add the following line to *Packages/manifest.json*:
  - `"com.llarean.img2etc2": "https://github.com/llarean/img2etc2.git"`

## Warning

- The utility changes the size of images. Before applying it, make sure you make a backup copy or create a commit.
- **All images are converted to PNG format** to ensure proper alpha channel support for transparent padding pixels.
- Image resolution data may not be updated immediately in the image information in editor.

## HOW TO

1. Open the utility (context menu: `Tools > UnityIMG2ETC2`)
2. Select a folder (2 ways):
- Click on the "Select folder" button; or
- Specify "Folder path:" in the input field;
3. Check the list of files
4. Click on the "Resize images" button

![Window](https://github.com/LLarean/img2etc2/blob/main/Preview.png?raw=true)

## Project Status
This project is **feature-complete** and stable. It accomplishes its intended goals and is ready for production use.

**Current maintenance approach:**
- **Bug fixes**: Addressed as reported
- **Minor improvements**: Implemented based on community feedback  
- **New features**: Considered only if there's significant demand

**Need a feature?** Feel free to open an issue with your use case!

## Contributing
While this project is feature-complete, contributions are welcome:
- **Bug reports**: [Open an issue](https://github.com/llarean/img2etc2/issues)
- **Feature requests**: Describe your use case in an issue
- **Pull requests**: For bug fixes or minor improvements
## Special thanks

[Nintoryan](https://github.com/Nintoryan) (For your inspiration and main idea)

---

<div align="center">

**Made with ❤️ for the Unity community**  

⭐ If this project helped you, please consider giving it a star!
