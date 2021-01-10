# CTR-tools: ctrviewer r11 [WIP]
Crash Team Racing (PS1) scene viewer powered by CTRFramework and MonoGame.

## How to use
- put bigfile.big in root folder *or* put lev/vrm files in "levels" folder
- launch viewer

## Texture replacement
Viewer supports texture replacements.
It checks "\levels\newtex" folder for existing PNG files before loading the texture from ps1 vram.
If texture found, PNG image is used instead. It also supports mip map generation, which may take some time if you load many textures.

!outdated instruction!
- load level
- find your PNG texture in tex folder
- copy to newtex folder
- edit texture in newtex folder
- reload level

you can of course use larger images, but if you want to make it look like in-game, keep the same size (usually 64x64) as well as convert it to 4 bit.

## Controls
Notice: XInput gamepad is strongly recommended, but mouse+keyboards is supported too.
You can also use x360ce for DirectInput devices.

* move: left stick/wasd
* rotate: right stick/arrow keys/hold left mouse click and move mouse
* speed up: right trigger/left shift
* menu: start/esc
* confirm/cancel: A/B or enter
* menu navigation: dpad/wasd/arrows
* force quit: select+start (back+start on xbox)/alt-f4
* fov change: +/-
* fullscreen toggle: alt+enter

## Stereoscopic 3D mode
You can enable side-by-side steropair mode in video settings.

If this mode is enabled:
* increase eye distance: LB/L1
* decrease eye distance: RB/R2
* press both to reset to default

2016-2021, DCxDemo*.