# museum4punkt0
Android and iOS project allowing German museums to create new apps using Wezit

## Wezit Entity:
* https://studio.wezit.io/
* "entityId": "005064"
* "appId": "97388ec0-d6b4-4c76-8a6f-497c4dd158fe"
* "inventoryId": "deded2a2-9612-4b93-b871-086029fe5c92"

## About

* Unity app built on iOS and Android
* Version Unity : 2021.3.25f1

## Förderhinweis
This application was developed in the joint project museum4punkt0 - Digital Strategies for the Museum of the Future,
Subproject "Zentrale wissenschaftliche Projektsteuerung". The project museum4punkt0 is funded by the German Federal Government Commissioner for Culture
and Media based on a decision of the German Bundestag. Further information:
www.museum4punkt0.de

![alt text](https://github.com/museum4punkt0/media_storage/blob/2c46af6cb625a2560f39b01ecb8c4c360733811c/BKM_Fz_2017_Web_de.gif) ![alt text](https://github.com/museum4punkt0/media_storage/blob/e87f37973c3d91e2762d74d51bed81de5026e06e/BKM_Neustart_Kultur_Wortmarke_pos_RGB_RZ_web.jpg)

## Build

To build the app, first [install Unity 2021.3.25f1](https://unity.com/releases/editor/whats-new/2021.3.25). You will need a Unity account, a pro one is not necessary as long as you are building a test app.

To build through command lines:

`[Unity Editor Path] -projectPath [Unity project Path] -batchmode -buildTarget [Android or iOS] -executeMethod AutomaticBuild.Perform [Main scene Path] -quit`

Scene path to add to the build:
`Assets/Scenes/Main.unity

When building for Android, you will get an .apk file that can be directly installed on phones (if you want to do it from a computer, look a [this tutorial](https://www.makeuseof.com/install-apps-via-adb-android/).

When building for iOS, you will get an Xcode project that will have to be compiled in Xcode > 12.
