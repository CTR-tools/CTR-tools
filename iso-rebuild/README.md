This is a tool chain to rebuild Crash Team Racing ISO in 2 clicks for modding purposes.

Tools used:
- ctr-tools - https://github.com/DCxDemo/CTR-tools
- mkpsxiso - https://github.com/Lameguy64/mkpsxiso
- [optional] psxlicense - http://www.psxdev.net/forum/viewtopic.php?f=69&t=704

Warning, tools are not included in this folder! You have to download them separately.

Steps to follow:
- Download the tools above and extract to corresponding folders in "tools"
- Extract all files from CTR ISO to ctr_source folder.
- run _INIT.BAT once. it will create bigfile folder. you can edit files in this folder now.
- run _BUILD.BAT to build ISO back. it will create ctr_rebuild.bin.

Minimum files needed to run the game are:
- SYSTEM.CNF (kind of autorun.inf)
- SCUS_944.26 (or similiar for other versions - main game executable)
- KART.HWL (sound effects and music)
- BIGFILE.BIG (game data container)


Notes:

- you can alter the toolchain to use mkpsxiso build in licensing capabilities, if you want to provide license file. you can also replace psxlicense with any other licensing tool, or omit this step entirely.

- BAT script launches ePSXe by default, change epsxe_path to your correct path. if your emulator supports command line start, you can use change it.

- BAT script is designed to generate ctr_source.xml only if it doesn't exist. This allows to keep manual changes in place, so you can control the way the iso is built.

- BAT script is meant for NTSC-U version. You'll have to slightly edit the script for other versions. This includes:
  - update region variable (not really that important)
  - update big_name variable (like sampler instead of bigfile)
  - optionally remove/rename ctr_source.xml file to generate a new one

- You can safely remove STR files from bigfile folder to make it even smaller (simply rename movies to movies1 or such). Those are track previews on level selection screen and they take away like half of the BIG file.

- XA files won't work with this setup automatically, hence you won't hear some voicelines and jingles, so you can remove it too. There is a way to rebuild iso with XA, but that requires to remove XA headers and manually edit the xml.
