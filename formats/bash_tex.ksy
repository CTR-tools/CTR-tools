meta:
  id: bash_tex
  application: Crash Bash
  title: Crash Bash (PS1) texture file
  file-extension: tex
  endian: le

seq:
  - id: magic
    type: u4
  - id: fsize
    type: u4
  - id: num_tex
    type: u2
  - id: num_pals
    type: u2
  - id: skip_to_tex
    type: u4
  - id: skip_to_pal
    type: u4
  - id: unk3
    type: u4
  - id: ptr_animated
    type: u4
  - id: zero #null?
    type: u4
  - id: palettes
    type: pal
    repeat: expr
    repeat-expr: num_pals
  - id: textures
    type: tex
    repeat: expr
    repeat-expr: num_tex
  - id: magic2
    type: u4
  - id: fsize2
    type: u4
  - id: num_anim_tex
    type: u4
  - id: anim_heads
    type: anim_header
    repeat: expr
    repeat-expr: num_anim_tex

types:
  pal:
    seq:
      - id: num_colors
        type: u4
      - id: color
        type: u2
        repeat: expr
        repeat-expr: num_colors

  anim_header:
    seq:
      - id: skip_to_data
        type: u4
      - id: pal_index #maybe?
        type: u4
      - id: num_frames
        type: u4
      - id: unk4
        type: u4
      - id: zero #null?
        type: u4

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