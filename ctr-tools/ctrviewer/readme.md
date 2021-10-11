# CTR-tools: ctrviewer
Crash Team Racing (PS1) scene viewer powered by CTRFramework and MonoGame.

## How to use
- put bigfile.big in root folder *or* put lev/vrm files in "levels" folder
- launch viewer

## Texture replacement
Viewer supports texture replacements.
It checks "newtex" folder for existing PNG files before loading the texture from ps1 vram.
If replacement texture is found, it will be used instead the original. It also supports mip map generation, which may take some time if you load many textures.

- use model_reader command line tool to extract the data from lev/vram
- find your PNG texture in one of tex folders
- copy to viewer's newtex folder
- edit texture in newtex folder
- reload level

No limitations apply to this process, but if you want to make it look more like in-game, keep the same size (usually 64x64) as well as convert it to 4 bit.

## Controls
Notice: XInput gamepad is strongly recommended, but mouse+keyboards is supported too.
You can also use x360ce for DirectInput devices.

* move: left stick/wasd
* rotate: right stick/arrow keys/hold left mouse click and move mouse
* change speed: left/right trigger or mouse wheel while left button is held
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