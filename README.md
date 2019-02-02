# CTR tools

## Description
Various tools to operate Crash Team Racing game files.

Join the CTR Tools Discord channel: https://discord.gg/56xm9Aj

## bigtool
Extracts/builds BIGFILE.BIG.

Usage: you can basically drag'n'drop big and txt files on the tool's icon or you can use command line.

Extracting example: bigtool.exe C:\example\bigfile.big
* This will create a text filem containing a list of all files and a folder with actual extracted contents. 
* Some files will be named based on the filelist.txt entries. Current list only fits NTSC-U and NTSC-J versions of the game.
* Zero bytes files are not exported.

Building: bigtool.exe C:\example\bigfile.txt
* Given the file list it will generate BIGFILE.BIG. It will overwrite existing file.
* If the file listed doesn't exist, it will be written to BIG as a zero byte entry (for example useful to remove STR thumbnails).

## lng2txt
Converts LNG files into text files and back. LNG files contains all the strings used in the game, thus allows to localize the game in any language based on latin alphabet.

Usage: lng2txt.exe C:\example\somefile.lng


## model_reader
At this point exports vertex colored non-textured mesh of the level (\*.lev files). Import tested in MeshLab, Blender and 3ds Max. MeshLab is recommended as an intermediate converter as it allows to import vertex color data from custom OBJ.
https://i.imgur.com/RqWH93V.png

Usage: model_reader.exe C:\example\somefile.lev \[ply]

## howl
Extracts CSEQ files from KART.HWL.

Usage: howl.exe C:\example\kart.hwl
* This will create kart.hwl_data folder with all the sequences.

## cseq
Reads CSEQ files and exports to MIDI.

Usage: use File menu or drag-drop CSEQ file on the application window.
* Single CSEQ file may contain multiple sequences.
* Click sequence on the list to show it's tracks/instruments.
* Double-clicking the track will bring MIDI file save dialog.

2016-2019, DCxDemo*.
