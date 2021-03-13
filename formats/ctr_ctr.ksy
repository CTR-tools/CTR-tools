meta:
  id: ctr_ctr
  application: Crash Team Racing
  title: Crash Team Racing (PS1) model file
  file-extension: ctr
  endian: le

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
      mesh_headers:
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
        type: vector4s
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
        type: s4
        pos: ptr_cmd
        repeat: until
        repeat-until: _ == -1
      anim_ptr_map:
        type: u4
        pos: ptr_anims
        repeat: expr
        repeat-expr: num_anims

  ctr_anim:
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
        
  vector4s:
    seq:      
      - id: x
        type: u2
      - id: y
        type: u2
      - id: z
        type: u2
      - id: w
        type: u2