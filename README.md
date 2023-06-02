# SPK Museum 4punkt0
Android and iOS project allowing German museums to create new apps using Wezit

## Wezit Entity:
* https://studio.wezit.io/
* "entityId": "005064"
* "appId": "97388ec0-d6b4-4c76-8a6f-497c4dd158fe"
* "inventoryId": "deded2a2-9612-4b93-b871-086029fe5c92"

## About

* Unity app built on iOS and Android
* Version Unity : 2021.3.24f1

## Build

Pour lancer une build en ligne de commande :
`[Path de Unity Editor] -projectPath [Path du projet Unity] -batchmode -buildTarget Standalone -executeMethod AutomaticBuild.Perform [Path de la scene à intégrer à la build] -quit`
`[Path de Unity Editor] -projectPath [Path du projet Unity] -batchmode -buildTarget WebGL -executeMethod AutomaticBuild.Perform [Path de la scene à intégrer à la build] -quit`

Scene path to add to the build:
`Assets/Scenes/Main.unity