# Third party code linked statically

| Project name | Author | License | Repo link |
| :--- | :--- | :--- | :--- |
| cSharpColourQuantization | bacowan | Unlicense | https://github.com/bacowan/cSharpColourQuantization |
| KaitaiStruct C# runtime | kaitai-io | MIT | https://github.com/kaitai-io/kaitai_struct_csharp_runtime |
| PlyImporter | 3DBear | MIT | https://github.com/3DBear/PlyImporter |

# Changes for CTR-tools
## PlyImporter
- converted unity types to ctrframework types
- converted debug calls to helper-panic
- converted color from floats to bytes
- renamed GetVerticesAndTriangles to FromFile
