# Changes for CTR-tools
- converted unity types to ctrframework types
- converted debug calls to helper-panic
- converted color from floats to bytes
- renamed GetVerticesAndTriangles to FromFile

Source repo: https://github.com/3DBear/PlyImporter

# Original readme

# PlyImporter
PLY (Polygon File Format) importer for Unity.
Feel free to open an issue if you found a .ply that does not work with this script, I'll happily make it work.

## Installation
Put both .cs files into your project

## Usage
Call GetVerticesAndTriangles in PlyHandler. This will give you at minimum a list of vertices and triangle indices, optionally a list of colors if vertex colors are present

## Features

- Support for Binary Little Endian formatting with vertex colors
- Support for Ascii formatting

## Planned features

- Support for Binary Big Endian
- Support for vertex colors for Ascii formatting
- PLY exporting