meta:
  id: ctr_ctr
  application: Crash Team Racing
  title: Crash Team Racing (PS1) model file
  file-extension: ctr
  endian: le
  
doc-ref: https://github.com/CTR-tools/CTR-tools/blob/master/formats/ctr_ctr.ksy

seq:
  - id: data_size
    type: u4
  - id: data
    type: ctr_model
    size: data_size
  - id: ptr_map_size
    type: u4
  - id: ptr_map
    type: u4
    repeat: expr
    repeat-expr: ptr_map_size / 4

types:
  ctr_model:
    seq:
      - id: name
        type: strz
        encoding: ascii
        size: 16
      - id: event
        type: u2
      - id: num_meshes
        type: u2
      - id: ptr_meshes
        type: u2
    instances:
      meshes:
        type: ctr_mesh
        pos: ptr_meshes
        repeat: expr
        repeat-expr: num_meshes
        
  ctr_mesh:
    seq:
      - id: name
        type: strz
        encoding: ascii
        size: 16
      - id: unk
        type: u4
      - id: lod_distance
        type: u2
      - id: billboard
        type: u2
      - id: scale
        type: vector3s
      - id: padding
        type: u2
      - id: ptr_cmd
        type: u4
      - id: ptr_verts
        type: u4
      - id: ptr_tex
        type: u4
      - id: ptr_clut
        type: u4
      - id: unk2
        type: u4
      - id: num_anims
        type: u4
      - id: ptr_anims
        type: u4
      - id: unk3
        type: u4
    instances:
      commands:
        type: u4
        pos: ptr_cmd
        repeat: until
        repeat-until: _ == 0xFFFFFFFF
      anims:
        type: ctr_anim
        pos: ptr_anims
        repeat: expr
        repeat-expr: num_anims
        
  ctr_anim:
    seq:
      - id: ptr
        type: u4
    instances:
      data:
        type: ctr_anim_data
        pos: ptr

  ctr_anim_data:
    seq:
      - id: name
        type: strz
        encoding: ascii
        size: 16
      - id: num_frames
        type: s2
      - id: frame_size
        type: u2
      - id: ptr_unk
        type: u4
      - id: vdata
        type: ctr_anim_frame(frame_size)
        repeat: expr
        repeat-expr: num_frames2
    instances:
      num_frames2:
        value: num_frames & 0x8000 == 0 ? num_frames : (num_frames & 0x7FFF) / 2 + 1

  ctr_anim_frame:
    params:
      - id: frame_size
        type: u4
    seq:
      - id: position
        type: vector3s
      - id: padding
        type: u2
      - id: skip
        size: 16
      - id: unk_render # ? unknown, values other than the original often ruin the model. maybe some drawing mode.
        type: u4
      - id: data
        size: frame_size - 28

  vector3s:
    seq:      
      - id: x
        type: s2
      - id: y
        type: s2
      - id: z
        type: s2