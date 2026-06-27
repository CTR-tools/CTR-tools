# iso-rebuild
This is a tool chain to rebuild Crash Team Racing ISO in 2 clicks for modding purposes.

Important! Source code doesn't contain mkpsxiso and bigfile binaries. Please make sure to download proper tools.
* mkpsxiso - https://github.com/Lameguy64/mkpsxiso/releases/latest
* ctr-tools - https://github.com/CTR-tools/CTR-tools/releases/latest

Usage:
- Drag-n-drop CTR BIN or ISO on _INIT.BAT. It will create ctr_rebuild folder. You'll find "bigfile" folder inside - these are the game files you can modify.
- Double click _BUILD.BAT. It will create ctr_rebuild.bin
- [Optional] You can specify path to the emulator in _BUILD.BAT to launch it automatically.

Full guide is available here: https://github.com/CTR-tools/CTR-tools/wiki/Rebuilding-the-ISO
