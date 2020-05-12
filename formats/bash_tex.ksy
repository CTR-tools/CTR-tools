meta:
  id: bash_tex
  application: Crash Bash
  title: Crash Bash (PS1) texture file
  file-extension: tex
  endian: le

seq:
  - id: tex_pack
    type: tex_block

types:

  tex_block:
    seq:
      - id: magic
        type: u4
      - id: fsize
        type: u4
      - id: num_tex
        type: u2
      - id: num_pals
        type: u2
      - id: unk1
        type: u4
      - id: skip
        size: 0x10
      - id: palettes
        type: pal
        repeat: expr
        repeat-expr: num_pals
      - id: textures
        type: tex
        repeat: expr
        repeat-expr: num_tex
  pal:
    seq:
      - id: num_colors
        type: u4
      - id: color
        type: u2
        repeat: expr
        repeat-expr: num_colors
        
  tex:
    seq:
      - id: width
        type: u2
      - id: height
        type: u2
      - id: unk0
        type: u4
      - id: unk1
        type: u4
      - id: unk2
        type: u4
      - id: unk3
        type: u4
      - id: data
        size: width * height * 2
    