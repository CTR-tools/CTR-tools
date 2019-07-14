# CTR tools

## Description
Various tools to operate Crash Team Racing game files.

Join the CTR Tools Discord channel: https://discord.gg/56xm9Aj

Tools are developed in Visual Studio 2010, target platform is .NET 4.0.

Project dependencies (make sure to download latest dlls):
* NAudio - https://github.com/naudio/NAudio (used to export MIDI files)
* Json.NET - https://github.com/JamesNK/Newtonsoft.Json (used for json parsing support)

## File formats
BIG (bigfile.big, sampler.big) - main game container for all the data used. Doesn't contain any filenames.\
LEV - scene container. Can contain static level mesh, various dynamic models, scripts, AI paths, etc.\
CTR - models. Can be stored in LEV or as a standalone file.\
VRAM - textures in native PS1 TIM format.\
LNG - localization files, bascially, an ordered list of strings.\
HWL (kart.hwl) - sfx/music container for the CTR sound engine known as "howl". All the SFX and music is stored in this file.\
CSEQ - custom music sequences found in HWL files.\
BNK - sound bank, a labeled set of headerless VAG samples.

## CTRFramework
CTRFramework is a shared DLL you can use in your projects.

## bigtool
Extracts/builds BIGFILE.BIG.

Usage: you can basically drag'n'drop big and txt files on the tool's icon or you can use command line.

Extracting example: bigtool.exe C:\example\bigfile.big
* This will create a text file containing a list of all files and a folder with actual extracted contents. 
* Some files will be named based on the filelist.txt entries. Current list only fits NTSC-U and NTSC-J release versions of the game.
* Zero-byte files are not exported at all (helps with SAMPLER.BIG from demos).

Building: bigtool.exe C:\example\bigfile.txt
* Given the file list, it will generate BIGFILE.BIG. Please note: it will overwrite existing file.
* If the file listed doesn't exist, it will be written to BIG as a zero-byte entry (for example useful to remove STR thumbnails).

## lng2txt
Converts LNG files into text files and back. LNG files contain all the strings used in the game, thus allows to localize the game in any language based on latin alphabet.

Usage: lng2txt.exe C:\example\somefile.lng
* Note: | is considered a new line character

## model_reader
At this point exports vertex colored non-textured mesh of the level (\*.lev files). Import tested in MeshLab, Blender and 3ds Max. MeshLab is recommended as an intermediate converter as it allows to import vertex color data from custom OBJ. Also supports ctr files as an input, but only outputs some info.
https://i.imgur.com/RqWH93V.png

Usage: model_reader.exe C:\example\somefile.lev \[ply]

## howl
Extracts CSEQ and BNK files from KART.HWL.

Usage: howl.exe C:\example\kart.hwl
* This will create kart.hwl_data folder with all the sequences and sound banks.

## cseq
Reads CSEQ files and exports to MIDI. See project dependencies.

Usage: use File menu or drag-drop CSEQ file on the application window.
* Single CSEQ file may contain multiple sequences.
* Click sequence on the list to show its tracks/instruments.
* Double-clicking the track will bring MIDI file save dialog.
* Use "patch MIDI instruments" option in case if custom instrument mapping is available. You can also add own mappings, see ctrdata.json for the reference.

2016-2019, DCxDemo*.