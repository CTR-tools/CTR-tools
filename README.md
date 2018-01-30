# CTR tools

## Description
Various tools to operate Crash Team Racing game files.


## big_splitter
Splits BIGFILE.BIG. Will rename some files (like levels). Keep in mind it's for NTSC .BIG.

### Usage
big_splitter.exe C:\example\bigfile.big

This will generate additional BIGFILE folder, you will find the extracted files there.


## lng2txt
Converts lng files into text files.

### Usage
lng2txt.exe C:\example\somefile.lng


## model_reader
Potentially will be able to export models in a widespread 3D format like obj.
Currently only exports colored point cloud. Import tested in MeshLab and 3ds Max.
In 3ds Max you'll have to import as single mesh, convert to editable mesh and then pick vertex selection.
https://i.imgur.com/yQPvHgu.jpg

### Usage
model_reader.exe C:\example\somefile.lev


2016-2018, DCxDemo*.
