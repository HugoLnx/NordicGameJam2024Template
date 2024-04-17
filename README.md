# Nordic Game Jam Template
## Introduction
This is just a simple template made with [https://github.com/HugoLnx/unity-sensen-setup](SensenSetup) on `Unity 2022.3.24f1`.

In summary it has:
* Default project folders structure.
* Setup for working well with C# Analyzers and Unity Analyzers.
* A bunch of useful default packages for prototyping and jams.
* Integration with Git and Git LFS.
* Integration with [https://docs.unity3d.com/Manual/SmartMerge.html](UnityYAMLMerger).
* Pre-installed plugins/assets:
  * [https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676](DOTween)
  * [https://github.com/hugolnx/unity-sensen-toolkit](SensenToolkit)
  * [https://github.com/hugolnx/unity-sensen-components](SensenComponents)
  * [https://github.com/hugolnx/unity-lnx-arch](LnxArch)

## Adapt for 2D Games or mobile games
Originally the project doesn't have packages for 2d, or mobile games, but it can be easily arranged
by running the following commands:
```bash
$ git clone git@github.com:HugoLnx/unity-sensen-setup.git

# For 2d games
$ python ./unity-sensen-setup/setup.py push-manifest --2d --slim

# For mobile games
$ python ./unity-sensen-setup/setup.py push-manifest --mobile --slim
```

## VS Code Extensions
To use all the features of this template, you shall be using `VS Code` with the following extensions
installed:
- C# Dev Kit by Microsoft
    - C# by Microsoft
    - IntelliCode for C# Dev Kit
- Unity by Microsoft
- Editor Config
