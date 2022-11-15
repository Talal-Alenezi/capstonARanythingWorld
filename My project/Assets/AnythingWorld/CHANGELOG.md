# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [3.1.20] - 2022-08-02
### Added
- Introduced batched loading for models.
## Changed
- Updated habitats.
- Better scaling for animation shaders.
- Improved cloning for models.
### Fixed
- Various bug fixes in creation process.


## [3.1.19.1] - 2022-04-12
- Bug fixes with auto recentering.
## [3.1.19] - 2022-04-12
### Added
- Added interface to define default shaders for AW model creation (Anything Settings)
- Added extended support for different types texture maps in material loader, shaders.
- Added session logging.
- Added model serialization.
- Added tiling offset support for basemaps.
- Error logs for missing URP/HDRP when opening Creator/trying to create model.
### Fixed
- Fixed bug that caused voice panel to break under certain conditions.
- Fixed problem with winged waddling creatures' wings to migrate away from the body.
- Patched Unity bug that prevents transparent materials from being instantiated at runtime correctly.
- Fixed model attribution generation to prevent duplicates and update more quickly.
- Fixed issue where Anything Setup would be duplicated on deletion and undoing action.
- Platform dependent compilation for Unity 2021 editor compatiblity.
- Fixed and improved flocking generation.
- Fixed habitat generation.
- Fixed bug with grid system.
- Fix bug when loading some OBJs with non-decimal scientific notation.
- Fixed voice example scene and AnythingSpeech, AnythingChat integrations.
- Fixed transparency handling in MTL loader to prefer D over Tr (3ds Max specific bug)
- Fixed scaling with snake animation shader.
## Changed
- Optimised and consolidated editor textures.
- Updated various interfaces in the Anything Creator, Settings and Login/Sign Up panels.
- Changed default shaders from HLSL to Shader Graph.
- Changed some joint parameters for quadrupeds and quadruped-ungulate.
- Default Anything World shaders now automatically load in when not defined in settings panel.
## Removed
- Removed scene view panel.
- Removed default AW canvas from Anything Settings prefab.

## [3.1.18] - 2022-1-13
### Fixed
- Fixed UnityWebRequest bugs when using 2019 versions of Unity.

## [3.1.17] - 2021-11-24
### Added
- New AnythingCreator panel UI look
### Fixed
- Minor grid bug fix.
- Fixed voice example scene.
- Fixed some animation bugs.

## [3.1.16] - 2021-11-11
### Added
- Added new cancel/clear search button in ObjectCreator panel.
### Changed
- Optimised search algorithm and thumbnail loading for object creator search panel.
- Migrated to new API endpoints.
- Tweaked car behaviour and prefab.
### Fixed
- Fixed issue with character encoding when requesting some objects.
- Fix issues with some prefab animations.
- Fix to attribution management.
- Fix to grid serialization.


## [3.1.15] - 2021-09-30
### Fixed
- Fixed animation scripts
- Fixed mesh stitching errors.
- Fixed flock animation movement.
- Fixed NavMeshScene.
- Fixed animation jitter.
- Fixed double EventManager in voice example.

## [3.1.14] - 2021-09-27
### Added
- Better initialization for AnythingSetup prefabs when using voice.
- Improved search results in AnythingCreator panel.
- Added attribution generation in AnythingSetup.
- Added AttributionTag utility to change Text UI on the fly.
- Sort by animated models in creator panel.
- Animated models have green thumbnail.
- Updated README
### Fixed
- Fixed error handling when spawning some habitats.
- Fixed AnythingSettings panel losing data when restarting the Unity editor.
- Fixed characters in certain texture filenames not decoding properly.
- Fixed attribution generation with the new API. 
- Fixed voice panel creation broken by new API changes.
- Fixed path example scene.
- Fixed navmesh example scene.
- Fixed vehicle wheel centering during animation.

## [3.1.13] - 2021-09-08
### Added
- Added handling for API changes around error handling.
### Fixed
- Fixed mesh stitcher.
- Fixed city habitat. 
- Fixed navmesh scene.

## [3.1.12] - 2021-08-25
### Added
- Added better encoding for edge case API requests.
- Added properties to anything settings for easy access.
### Fixed
- Fixed hopper prefab.
- Fixed OBJ loader issue that mirrored mesh on import.
- Fixed error thrown when rapidly deleting objects before completion. 

## [3.1.11] - 2021-08-23
### Fixed
- Fixed exception sometimes thrown when adding new behaviour.

## [3.1.10] - 2021-08-10
### Fixed
- Fixed issue with settings file not being found.

## [3.1.9] - 2021-07-30
### Changed
- Optimised async makeprocess

## [3.1.8] - 2021-07-30
### Fixed
- Fixed issues with various prefabs

## [3.1.7] - 2021-07-27
### Fixed
- Fixed voice panel bug.
- Fixed Creation Panel search result thumbnail bug.
- Fixed texture name loading to work with database changes.
### Added
- Added optional microphones in voice panel.

## [3.1.6] - 2021-07-22
### Fixed
- Fixed issues with structure of prefabs.
- Fixed hopper animation.
- Fixed various bugs.

## [3.1.5] - 2021-07-15
### Fixed
- Fixed compilation errors in AWBehaviourController.
- Fixed scriptable object singleton initalization bugs.

## [3.1.4] - 2021-07-14
### Fixed
- Fixed issue with winged prefabs.
- Fixed wing flap issue
### Removed
- Removed requirement for attribution UI element from AnythingSetup

## [3.1.3] - 2021-07-13
### Fixed
- Fixed bugs introduced by behaviour controller updates.
### Removed
- Removed certain exceptions thrown for animationless objects in behaviours.

## [3.1.2] - 2021-07-09
### Changed
- Optimised adding and removing behaviours using behaviour controller.
### Fixed
- Fixed bug that caused performance dips in editor while viewing Behaviour Controller.

## [3.1.1] - 2021-06-29
## Added
- Added further namespace encapsulation for AnythingSpeech
### Changed
- Reorganised directory to fit Unity UPM specification.
### Fixed
- Fixed issues with habitat spawning.
- Fixed security issues.
### Removed
- Removed obsolete scripts no longer in use.

## [3.1.0] - [YANKED]

## [3.0.2] - 2021-06-18
### Added
- API endpoint override in AWObjs for testing purposes.
### Changed
- Severity of some debug messages.

## [3.0.1] - 2021-06-18
### Fixed
- Fixed bug with error handling from API.

## [3.0.0] - 2021-06-17
### Added
- Namespace encapsulation for the AnythingWorld scripts and utilities
- Changelog to document version changes within the package.
- Timeouts to the make processes
- Timeouts to login and signup processes
- Further documentation to core scripts.
- Upgraded to 2020 LTS version.
- Platform dependent compilation for network requests.

### Fixed
- Fixed bugs when opening Settings window
- Fixed scaling bugs.
- Fixed obsolete network calls.

### Removed
- Obsolete editor folder for NavMesh examples.


