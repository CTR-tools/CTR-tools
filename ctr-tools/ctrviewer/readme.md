# CTR-tools: ctrviewer r10 [WIP]

A MonoGame Crash Team Racing level viewer using CTRFramework.


## How to use
- put bigfile.big in root folder *or* put lev/vrm files in "levels" folder
- launch viewer

If you see incorrect textures, cleanup your tex folder. textures currently do not overwrite (speeds up loading).

WARNING! It will dump enormous amount of small files in tex folder. Do not use the viewer if it's unacceptable (i.e. SSD users)

## Texture replacement
- load level
- find your PNG texture in tex folder
- copy to newtex folder
- edit texture in newtex folder
- reload level

you can of course use larger images, but if you want to make it look like in-game, keep the same size as well as convert to 4 bit.

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
* alt+enter: fullscreen toggle


2016-2020, DCxDemo*.
