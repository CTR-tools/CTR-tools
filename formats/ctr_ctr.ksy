meta:
  id: ctr_ctr
  application: Crash Team Racing
  title: Crash Team Racing (PS1) model file
  file-extension: ctr
  endian: le
  bit-endian: be

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
      - id: thread_id # this is an internal number
        type: s2
      - id: num_meshes
        type: u2
      - id: ptr_meshes
        type: u4
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
      - id: padding # assumed always 0?
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
        type: command
        pos: ptr_cmd
        repeat: until
        repeat-until: _.new_face_block and _.swap_first_vertex and _.flip_face_normal and _.cull_backface and _.color_scratchpad and _.read_vertex_stack and _.unk1 and _.unk2 and _.stack_write_index == 0b11111111 and _.color_coord_index == 0b1111111 and _.tex_coord_index == 0b111111111
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
      - id: num_frames_pack
        type: u2
      - id: frame_size
        type: u2
      - id: ptr_unk
        type: u4
      - id: vdata
        type: ctr_anim_frame(frame_size)
        repeat: expr
        repeat-expr: num_frames
    instances:
      interp:
        value: num_frames_pack & 0x8000 > 0
      num_frames:
        value: "interp ? (num_frames_pack & 0x7FFF) / 2 + 1 : num_frames_pack"

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
      - id: ptr_data
        type: u4
      - id: extra_unk
        size: ptr_data - 0x1C
      - id: data
        size: frame_size - ptr_data

  vector3s:
    seq:
      - id: x
        type: s2
      - id: y
        type: s2
      - id: z
        type: s2

  command:
    seq:
      - id: new_face_block
        type: b1
      - id: swap_first_vertex
        type: b1
      - id: flip_face_normal
        type: b1
      - id: cull_backface
        type: b1
      - id: color_scratchpad
        type: b1
      - id: read_vertex_stack
        type: b1
      - id: unk1
        type: b1
      - id: unk2
        type: b1
      - id: stack_write_index
        type: b8
      - id: color_coord_index
        type: b7
      - id: tex_coord_index
        type: b9