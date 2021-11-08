# Wearable-Multi-camera-System

This wearable camera system consists of HTC VIVE VR headset, trackers and Realsense RGB-D Cameras. It can be used for **human motion capture**, to **track human gaze input** (if you have VIVE Pro Eye), to **provide camera stream from various Realsense cameras**, or any combination of these modules.

## Setup

#### **Download**

Git clone this repository in your Unity project folder. In Unity hub, add this folder to be a new project and launch it. Please **ignore the errors and warnings** if it's your first time launching the project. You will still need to install some packages to get rid of them.

#### **Install packages**

Depending on the module you use, you will need different packages:

- For any of these modules, the basic package you need is "[SteamVR Plugin](https://assetstore.unity.com/packages/tools/integration/steamvr-plugin-32647)". Please make sure you put it in your Unity asset, import and download it in this project before running any scenes.
- To use Realsense camera in Unity, 
- For eye tracking, 

Open the corresponding scene in **Scenes** folder. Make sure your <u>SteamVR</u> is launched and all the components are found before running any scenes.


# Demo Presentation

This module includes three parts, which are **Eye tracking**, **Motion capture** and **Multi-Camera Feedback and Switching**, respectively. 

## 1. Eye Tracking
Eye tracking is provided by HTC VIVE VR headset which helps track the real-time gaze trajectory as well as facial expression of the experimenters as they wear the headset and look at the scene in it. This function is in "Eye Tracking" subitem under the scene 

![eye_tracking_demo](Demo/eye_track_demo.gif)

## 2. Motion Capture

- Open scene **MotionCapture**.
- In the game object hierarchy, for each tracker, enter the serial number in the inspector. Serial number can be found in your SteamVR floating window by `≡ -> Devices -> Manage Vive Trackers -> ManageVive Trackers`.
- Launch SteamVR and make sure the headset and all the controllers are properly paired. Do a room setup in steamVR if it's your first time or when the VIVE station changes position.

- Hit play in Unity, you should be able to see the trackers and the headset tracks properly.


![tracker_demo](Demo/tracker_demo.gif)

If you run into any error, it's very likely that the "SteamVR plugin" isn't installed properly. Please reimport and redownload it.


## 3. Multi-Camera Switch

In the RealSense Canvas item, the gameobject identifies each camera interface with the series number on it and all frames are collected by RealSense cameras into an all-in-one canvas. We implement camera switch by enabling the choosen camera object while blocking the rest in the meanwhile. And the goal is achieved by pressing the keyboard Q, W, E, R down.

![Multicamera_demo](Demo/MultiCamera_Switch.gif)
