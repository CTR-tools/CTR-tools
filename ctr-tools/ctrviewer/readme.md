# CTR-tools: ctrviewer
Crash Team Racing (PS1) scene viewer powered by CTRFramework and MonoGame.
It is recommended to use either NTSC-U or NTSC-J versions. PAL will work too, but only for levels.
Demo samplers are not supported, you can extract those and load as custom levels.

## How to use
To load original levels:
- put **bigfile.big** from CTR CD in viewer's root folder (or mount a virtual CD, or copy to your any hard drive root)
- launch viewer
- press esc -> load level -> select cup -> select level to load
- check level options and video options menus for various settings

To load custom levels:
- in viewer's root find levels folder (create one if missing)
- create a folder for your level
- copy extracted level files (lev and vrm)
- create a text file called info.xml and put this:
	<CustomLevelInfo>
        <LevelName>Your level name</LevelName>
        <LevelFile>data.lev</LevelFile>
    </CustomLevelInfo>
- launch viewer
- press esc -> load levels -> custom levels
- choose your custom level

*You can speed up loading times by disabling mip map generation at the cost of texture flickering in the distance.*

## Controls
Notice: XInput gamepad is strongly recommended, but mouse+keyboards is supported too.
You can also use x360ce for DirectInput devices.

* move: left stick/wasd
* rotate: right stick/arrow keys/hold left mouse click and move mouse
* change speed: left/right trigger or mouse wheel while left button is held
* menu: start/esc
* confirm/cancel: A/B or enter/backspace
* menu navigation: dpad/wasd/arrows
* force quit: select+start (back+start on xbox)/alt-f4
* fov change: +/-
* fullscreen toggle: alt+enter
* take screenshot: print screen (screenshots folder)

## Kart mode
A simplistic kart mode is added for fun. Do not expect too much, it isn't intended to be a playable game or anything.

* move kart: WASD or A - accel, left stick - steer
* raise up/down, ignoring collisions: PageUp/PageDown or dpad up/down 
* change kart - O/P keys

*Can't go backwards, can clip through walls*

## Stereoscopic 3D mode
You can enable side-by-side steropair mode in video settings.

If this mode is enabled:
* increase eye distance: LB/L1
* decrease eye distance: RB/R2
* press both to reset to default

## Texture replacement
Viewer supports texture replacements. Original and replaced textures can be toggled on the fly via menu.
It checks "newtex" folder recursively for existing PNG files before loading the texture from ps1 vram. newtex folder can contain subfolders, all of them will be loaded.
If replacement texture is found, it will be used instead the original. It also supports mip map generation, which may take some time if you load many textures.

- use either ctr-tools-gui or model_reader command line tool to extract the data from lev/vram
- find your PNG texture in one of tex folders
- copy to viewer's newtex folder
- edit texture in newtex folder
- reload level

No limitations apply to this process, but if you want to make it look more like in-game, keep the same size (usually 64x64) as well as convert it to 4bit format (16 colors).


2016-2023, DCxDemo*.