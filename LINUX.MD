# Running CTR-Tools on linux

** tested on ubuntu 19.10 and 20.04 **

By the end of this tutorial you should be able of:

* Install Proper tools to get everything working
* Install a psx-emulator that runs smoothly
* Extracting ctr files from iso
* Run CTR-Tools
* Altenartive ways

## Installing Proper tools

First and most important is mono, this you enable linux to read .CS and everything related
ps: if you are using 19.10 you should install the 18.04 version

### Ubuntu 20.04 (amd64, armhf, arm64, ppc64el)
```
sudo apt install gnupg ca-certificates
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb https://download.mono-project.com/repo/ubuntu stable-focal main" | sudo tee /etc/apt/sources.list.d/mono-official-stable.list
sudo apt update
```

### Ubuntu 18.04 (i386, amd64, armhf, arm64, ppc64el)

```
sudo apt install gnupg ca-certificates
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb https://download.mono-project.com/repo/ubuntu stable-bionic main" | sudo tee /etc/apt/sources.list.d/mono-official-stable.list
sudo apt update
```

### Same for all OS

```
sudo apt install mono-devel
sudo apt install mono-complete
sudo apt install mono-dbg
sudo apt install referenceassemblies-pcl
sudo apt install mca-certificates-mono
sudo apt install mono-xsp4

```

### Optional

If you want to deal with the source code, you can use monodevelop:
https://www.monodevelop.com/
Or install mono from the website:
https://www.mono-project.com/download/stable/

## Installing PS1 Emulator

After some testing, the easiest and surprisingly the best emulator I've found for linux was **PCSXR**

```
sudo apt install pcsxr
```

This emulator is able to emulate the psone bios, so you dont need one, if you really want you can download,
I recommend **SCPH1001.bin**

Crash Team Racing 1999 can easily be found at some research, try to find one with the .cue file (for proper music playing)

### Emulator Configuration

the default one is ok, but I'll share mine here:

![a](https://i.imgur.com/1W3Szcy.png)
![a](https://i.imgur.com/IqXZfII.png)
![a](https://i.imgur.com/rgJas2h.png)
![a](https://i.imgur.com/x1NWZe8.png)
![a](https://i.imgur.com/XV8d7RV.png)
![a](https://i.imgur.com/tDScJUF.png)
![a](https://i.imgur.com/BGeVS5d.png)


## Extracting CTR Files

PowerISO is a free tool, and of course has a exellent support for linux: https://www.poweriso.com/download-poweriso-for-linux.htm

Download it, and follow this tutorial:

![a](https://i.imgur.com/lmwrVgH.gif)

## Running CTR Tools

This part should be done independently from what you are going to do with the files, I mean we are going to use BIGTOOL
to extract main CTR files.

Download ctr tools: https://github.com/DCxDemo/CTR-tools/releases
For this tutorial I'm going to use **R10**

![a](https://imgur.com/5g5LlkJ.gif)

## Alternative ways

Remember that ctr-tools does not offer an official-release for linux, this means that it does not work same way as windows does
so you can see on the gif above how to extract the BIGFILE.BIG, look at filenaming what should be directories are in the filename.
to solve this I'm creating a Interface tool with cross-platform support, the link to it is on the final of this page.

https://github.com/nonunknown/ctr-tools-interface

## Conclusion

From now on, you can hack ctr, you are only limited by what the CTR-Framework can do, be sure to read its documentation
if you are going to use the ctr-interface do the same.

