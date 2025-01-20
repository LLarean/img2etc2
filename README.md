# img2etc2

A simple utility for adjusting image size to correct compression in Unity 3D.

## INSTALLATION

There are 4 ways to install this utility:

- import [img2etc2.unitypackage](https://github.com/llarean/img2etc2/releases) via *Assets-Import Package*
- clone/[download](https://github.com/llarean/img2etc2/archive/master.zip) this repository and move files to your Unity project's *Assets* folder
- *(via Package Manager)* Select Add package from git URL from the add menu. A text box and an Add button appear. Enter a valid Git URL in the text box:
    - `https://github.com/llarean/img2etc2.git`
- *(via Package Manager)* add the following line to *Packages/manifest.json*:
    - `"com.llarean.screenshoter": "https://github.com/llarean/img2etc2.git",`

## Warning

- The utility changes the size of images. Before applying it, make sure you make a backup copy or create a commit.
- Image resolution data may not be updated immediately in the image information in editor.

## HOW TO

1. Open the utility (context menu: `Window > img2etc2`)  


2. Select a folder (2 ways):
  - Click on the "Select folder" button; or
  - Specify "Folder path:" in the input field;


3. Check the list of files


4. Click on the "Resize images" button

![Window](https://github.com/LLarean/img2etc2/blob/main/Preview.png?raw=true)

## Special thanks

[Nintoryan](https://github.com/Nintoryan) (For your inspiration and main idea )