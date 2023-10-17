# Ready Player Me Unity SDK Core

[![GitHub release (latest SemVer)](https://img.shields.io/github/v/release/readyplayerme/rpm-unity-sdk-core)](https://github.com/readyplayerme/rpm-unity-sdk-core/releases/latest) [![GitHub Discussions](https://img.shields.io/github/discussions/readyplayerme/rpm-unity-sdk-core)](https://github.com/readyplayerme/rpm-unity-sdk-core/discussions)

This is an open source Unity plugin that contains all the core functionality required for and manages all the plugin updates and dependencies including Ready Player Me Avatar Loader and glTFast. 

Please visit the online documentation and join our public `discord` community.

![](https://i.imgur.com/zGamwPM.png) **[Online Documentation]( https://readyplayer.me/docs )**

![](https://i.imgur.com/FgbNsPN.png) **[Discord Channel]( https://discord.gg/9veRUu2 )**

:octocat: **[GitHub Discussions]( https://github.com/readyplayerme/rpm-unity-sdk-core/discussions )**

## Quick Start 

### Requirements
- Unity Version 2020.3 or higher
- [Git](https://git-scm.com) needs to be installed to fetch the Unity package. [Download here](https://git-scm.com/downloads)

**1.** To add the new Ready Player Me Unity SDK to your project you can use the Unity Package Manager to import the package directly from the Git URL. 

**2.** With your Unity Project open open up the Package Manager window by going to `Window > Package Manager`.

![open-package-manager](https://user-images.githubusercontent.com/7085672/206432665-da233187-06ad-40b5-a25e-660c97d6726f.png)

**3.** In the **Package Manager** window click on the + icon in the top left corner and select Add Package From Git URL.

![add-package-from-ur;](https://user-images.githubusercontent.com/7085672/206432698-8ecde741-4259-486f-9c77-d63fbc9a6cde.png)

**4.** Paste in this url 

`https://github.com/readyplayerme/rpm-unity-sdk-core.git`

![paste-git-url](https://user-images.githubusercontent.com/7085672/206432731-f9e0d161-7843-4d6e-8851-47b1f3bfb3bc.png)

**5.** Click add and wait for the import process to finish.

After the process is complete you project will have imported these packages:

- **Ready Player Me Core**
- **Ready Player Me Avatar Loader**
- **Ready Player Me WebView**
- **glTFast**

![package-import-complete](https://user-images.githubusercontent.com/7085672/206432761-dc078cc7-23b0-4d83-a638-46db19faabd6.png)

## Alternate Installation

### Using Git URL

1. Navigate to your project's Packages folder and open the manifest.json file.
2. Add this line below the `"dependencies": {` line
    - ```json title="Packages/manifest.json"
      "com.readyplayerme.core": "https://github.com/readyplayerme/rpm-unity-sdk-core.git",
      ```
3. UPM should now install the package.

### OpenUPM (using command line)

1. The package is available on the [openupm registry](https://openupm.com).
2. Execute the openum command.
    - ```
      openupm add com.readyplayerme.core
      ```
### OpenUPM (using editor)

1. Open `Edit | Project Settings | Package Manager`
2. Add a new Scoped Registry (or edit the existing OpenUPM entry)
   - ```
     Name     package.openupm.com
     URL      https://package.openupm.com
     Scope(s) com.readyplayerme.core
     ```
3. Click Save (or Apply)
4. Open Window/Package Manager
5. Click +
6. Select Add package by name... or Add package from git URL...
7. Paste `com.readyplayerme.core` into name
8. Click Add

### OpenUPM (using manifest)

1. Add this to manifest 
   - ```{
     "scopedRegistries": [
        {
            "name": "package.openupm.com",
            "url": "https://package.openupm.com",
            "scopes": [
                "com.readyplayerme.core"
            ]
        }
     ],
     "dependencies": {
        "com.readyplayerme.core": "1.0.0"
       }
     }
     ```
   
