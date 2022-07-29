# XR-based HRTF Measurement App
This Unity app is designed to be used as a part of the [XR-based HRTF Measurement System](https://trsonic.github.io/XR-HRTFs/).

## Supported Hardware
Meta Quest 2

## Getting Started
* Installation
    * Use eg. [SideQuest](https://sidequestvr.com/setup-howto/).
    * Download the latest APK file from the [Releases](../../releases/latest) page.
* Usage
    * Make sure that the Quest is on the same local network as the PC running the [Measurement Control App](https://github.com/trsonic/XR-HRTF-capture-juce/).
    * Run the App on your Quest (can be found in the Unknown Sources folder).
    * Note the displayed IP address (needed to initialize the [Measurement Control App](https://github.com/trsonic/XR-HRTF-capture-juce/)).
    * Tag the measurement loudspeaker position using the trigger button on your right controller.
    * Press joystick to disable the "loudspeaker tagging mode".
    * The app is ready to run measurements.

## Unity Project Information
* Unity Version: 2019.4.30f1
* Packages used:
    * Oculus XR Plugin 1.11.0
    * XR Plugin Management 4.1.0
* Main scene file: `Assets/Scenes/XR-HRTF-CAPTURE.unity`