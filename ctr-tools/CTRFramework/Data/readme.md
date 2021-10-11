# CTRFramework data
This folder contains various data, used by the code. It is stored in XML, JSON or numbered list custom format (kind of like INI).
These files are added as embedded resources to CTRFramework.dll. Refer to Helpers and Meta classes (GetTextFromResource, GetTextFromResource, LoadNumberedList, etc.)

# Numbered list format
Every entry in this file should be defined as [decimal number]=[string without spaces] (i.e 249=levels\canyon\level.dat)
Hash symbol represents comment. You can either start with a hash or use hash after the numbered value.
Empty lines are ignored, you can have as many as you want.

# Versions.xml
This XML file contains MD5 values for various releases, used to identify specific game versions.
For a broader version support, individual versions are also identified by the number of files (in case of modified content).

# big_*.txt
Numbered list containing file names for every entry in BIGILE.BIG/SAMPLER.BIG.

# xa_*.txt
Numbered list containing names for every individual XA file linked in XNF file.

# samplenames, howlnames and banknames
Numbered list containing names for content in KART.HWL.

# cseq.json
Contains MIDI mappings for NTSC-U tracks.

# bash_filelist.txt
List of filenames for CRASHBSH.DAT found in NTSC-U Crash Bash.
