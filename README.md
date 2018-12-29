# CTR tools

## Description
Various tools to operate Crash Team Racing game files.

Join the CTR Tools Discord channel: https://discord.gg/56xm9Aj


## bigtool
Extracts/builds BIGFILE.BIG. Some files will be renamed based on the filelist.txt contents. Identified file additions are welcome.

Usage: you can basically drag'n'drop big and txt files on the tool's icon or you can use command line.

Extracting: bigtool.exe C:\example\bigfile.big
* This will create a BIGFILE folder with all contents and BIGFILE.TXT which is a list of all files.

Building: bigtool.exe C:\example\bigfile.txt
* Given the file list it will generate BIGFILE.BIG. It will overwrite existing file.


## lng2txt
Converts lng files into text files and back (warning, not tested propery as of r4).

Usage: lng2txt.exe C:\example\somefile.lng


## model_reader
At this point exports vertex colored non-textured mesh of the level (\*.lev files). Import tested in MeshLab, Blender and 3ds Max.
https://i.imgur.com/RqWH93V.png

Usage: model_reader.exe C:\example\somefile.lev \[ply]


## howl
Extracts CSEQ files from KART.HWL.

Usage: howl.exe C:\example\kart.hwl
* This will create kart.hwl_data folder with all the sequences.


## cseq
Reads CSEQ files and plays separate tracks. At this point MIDI/SEQ export is not available. Unlike the rest it's temporary WinForms application.

Usage: use File menu or drag-drop CSEQ file on the application window.
* Single CSEQ file may contain multiple sequences.
* Click sequence on the list to show it's tracks.
* Click on the track to play it.


2016-2018, DCxDemo*.
