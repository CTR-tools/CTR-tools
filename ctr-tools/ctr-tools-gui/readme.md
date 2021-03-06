# CTR-tools-gui
A GUI interface for CTR-Tools meant to be an alternative for command-line tools.

# Workflow
There are various tabs for specific actions that loosely correspond to the existing command line tools. Some of them however provide additional functionality.

There are generally 2 ways to use the program:
1. open the tab you need, choose the open file dialog and select your file.
2. simply drag-drop your file to the tool, it will try to guess the correct tab automatically to load the file, if supported.

# Tabs
## Info
Just a welcome tab with some info about framework version and links to github/discord

## BIG archive
BIG files processing.

## VRAM textures
Used to extract textures from VRAM and write new textures back.

## CTR models
Converts original CTR models to OBJ and back.

## LEV scenes
Various tools for LEV files mostly used as a debug playground now.

## HOWL sound
Reads HOWL sound file and extracts all banks, samples and cseq music.

## CSEQ music
Reads cseq music sequences and exports to midi. Used to be a separate cseq.exe before.

## ePSXe
Emulation tab meant to read CTR data from emulator memory and apply patches on the fly.
Curent support is very limited.