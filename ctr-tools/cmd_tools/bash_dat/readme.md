# CTR-tools: Crash Bash tool

This tool is able to:
- split CRASHBSH.DAT file given the correct EXE file;
- extract most textures from TEX files;
- extract messed up models from MDL files;
- extract VH/VB/SEQ from SFX files.

1. copy both EXE and DAT file to the same folder.
2. drag drop EXE file on the tool
3. if EXE is identified as known, it will create data folder with extracted contents

## supported versions:

- OPSM 38 Demo
- Spyro Split / Winter Jampack 2000 / demo disc 1.3
- Spyro 3 NTSC Demo
- Euro Demo xx? (gotta compare rest of euro demos if they differ)
- Sep 14, preview prototype
- NTSC Release
- Oct 9 PAL prototype
- PAL Release
- JPN Trial
- JPN Release

*Important! File names are partially available only for NTSC-U release.*

Do not rely on extensions too much, it may fail, cause it's based on rough ntsc-u analysis. it may or may not match other versions partially or completely.

to extract textures, simply drag drop TEX file on the tool. it will create a folder by the same name with extracted textures. note that only 4bit textures are supported, some stuff like player icons doesn't work. also files with wrong palette are marked with "badpal" word. it's normal to have those at this point.
to extract models, drag drop MDL file on the tool. it will create several OBJ files, one for every model in the file.
sfx files split into VB/VH/SEQ

you can drag multiple files at the same time.


dcxdemo
