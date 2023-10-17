# Changelog

All notable changes to this project will be documented in this file.
This project adheres to [Semantic Versioning](http://semver.org/).

## [1.3.0] - 2023.05.29

### Added
- API for async avatar loading [#76](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/76)
- added setup guide window [#85](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/85)

### Updated
- updates for core module refactor [#78](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/78)
- general cleanup and refactor [#79](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/79)
- refactor of multiple avatar renderer example [#83](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/83)
- refactor of avatar cache [#86](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/86)
- documentation UI improvements [#87](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/87)

### Fixed
- alignment issues and spacing in Core settings window [#88](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/88)

## [1.2.0] - 2023.04.18

### Added
- Mesh Optimization compression support [#74](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/74)
- QueryBuilder class for handling Avatar API parameter generation [#71](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/71)
- EyeAnimationHandler and VoiceHandler now logs if required blendshapes are missing [#66](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/66)
- added extra unit tests for better coverage [#68](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/68)
- AvatarMetdata now includes a color hex value for SkinTone [#63](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/63)

### Fixed
- an issue caused by avatar URL's that have a space at beginning or end [#73](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/73)

### Updated
- AvatarRenderLoader now uses latest Render API via URL query parameters [#64](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/64)
- refactor of WebRequestDispatcher [#67](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/67)
- model urls for sample scenes updated [#72](https://github.com/readyplayerme/rpm-unity-sdk-avatar-loader/pull/72)

## [1.1.0] - 2023.03.21

### Added
- quick start sample
- animation extract now supports extracting multiple files at once
- avatar loaded events
- avatar component

### Updated
- animation extractor path

### Fixed
- caching issue related to time zone differences

## [1.0.0] - 2023.02.20

### Added
- support for offline avatar loading from cache
- optional sdk logging
- glTF fast defer agent support
- texture channel support for avatar config

### Updated
- PartnerSubdomainSettings refactored to a CoreSettings scriptable object

### Fixed
- Added missing URP shader variant
- core settings asset now automatically created if it is missing.
- Various other bug fixes and improvements

## [0.2.0] - 2023.02.08

### Added
- support for offline avatar loading from cache
- optional sdk logging
- glTF fast defer agent support
- texture channel support for avatar config

### Updated
- PartnerSubdomainSettings refactored to a CoreSettings scriptable object

### Fixed
- Added missing URP shader variant
- Various other bug fixes and improvements

## [0.1.1] - 2023.01.22

### Added
- missing shader variant for URP shader variant collection

## [0.1.0] - 2023.01.12

### Added
- inline code documentation
- Contribution guide and code of conduct
- Added samples in optional samples folders
- GLTF/GLB files now use gltFast importer
- shader variant helper to check and import missing shaders

### Updated
- A big refactor of code and classes

### Fixed
- Various bug fixes and improvements
