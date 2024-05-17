# CTRFramework data
This folder contains various data, used by the code. It is stored in XML or numbered list custom format (kind of like INI).
These files are compressed to zip archive, then the archive is added as an embedded resource to CTRFramework.dll.
Refer to Helpers class resource helpers region - GetStreamFromZip, GetLinesFromResource, GetTextFromResource, LoadNumberedList, LoadTagList, etc.

# Numbered list format
Basically, a limited subset of INI format without groups.
Every entry in such list should be defined as [decimal number]=[string without spaces] (i.e 249=levels\canyon\level.dat). Leading zeroes allowed for padding.
Hash symbol represents comment. You can either start with a hash or use hash after the numbered value.
Empty lines are ignored, you can have as many as you want.

# versions.xml
This XML file contains MD5 values for various releases, used to identify specific game versions.
For a broader version support, individual versions are also identified by the number of files (in case of modified content).

# big_*.txt
Numbered list containing file names for every entry in BIGILE.BIG/SAMPLER.BIG.

# xa_*.txt
Numbered list containing names for every individual XA file linked in XNF file.

# howlnames, banknames and samplehashes
Numbered list containing names for content in KART.HWL.
Labeled tag list for CRC32 sample hashes.

# cseq.xml
Contains MIDI instrument mappings based on NTSC-U tracks.

# bash_filelist.txt
List of filenames for CRASHBSH.DAT found in NTSC-U Crash Bash.
