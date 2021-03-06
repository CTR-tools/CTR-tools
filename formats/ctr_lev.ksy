meta:
  id: ctr_lev
  application: Crash Team Racing
  title: Crash Team Racing (PS1) scene file
  file-extension: lev
  endian: le

seq:
  - id: data_size
    type: u4
  - id: data
    type: lev
    size: data_size
  - id: ptr_map_size
    type: u4
  - id: ptr_map
    type: u4
    repeat: expr
    repeat-expr: ptr_map_size / 4

types:
  lev:
    seq:
      - id: header
        type: scene_header
    
      - id: restart_main
        type: pose
        if: header.ptr_restart_main != 0
    
      - id: restart_pts
        type: pose
        repeat: expr
        repeat-expr: header.cnt_restart_pts
    
      - id: instances
        type: instance
        repeat: expr
        repeat-expr: header.num_instances
    
      - id: model_pointers
        type: u4
        repeat: expr
        repeat-expr: header.num_models
    
      - id: mesh_info_header
        type: mesh_info
    
      - id: quad_block_array
        type: quad_block
        repeat: expr
        repeat-expr: mesh_info_header.cnt_quad_block
    
      - id: vertex_array
        type: vertex
        repeat: expr
        repeat-expr: mesh_info_header.vertexnum
    
    instances:
      trial:
        pos: header.ptr_trial_data
        type: trial_data
        if: header.cnt_trial_data > 0
    
      skybox:
        pos: header.ptr_skybox
        type: skybox
        if: header.ptr_skybox != 0
    
      vis_data_array:
        pos: mesh_info_header.ptr_vis_data_array
        type: vis_data
        repeat: expr
        repeat-expr: mesh_info_header.cnt_vis_data
    
      unk_struct1_array:
        pos: header.ptr_tex_array
        type: unk_struct
    
      ai_nav:
        pos: header.ptr_ai_nav
        type: ai_paths
    
      water_data:
        pos: header.ptr_water
        type: water_packet
        repeat: expr
        repeat-expr: header.cnt_water

  vis_data:
    seq:
      - id: flag
        type: u2
      - id: id
        type: u2
      - id: bbox_min
        type: vector3s
      - id: bbox_max
        type: vector3s 
      - id: data
        type:
          switch-on: flag & 1
          cases:
            0: vis_data_branch
            1: vis_data_leaf

  vis_data_branch:
    seq:
      - id: axis_x
        type: u2
      - id: axis_y
        type: u2
      - id: axis_z
        type: u2
      - id: unk
        type: u2
      - id: left_child_id
        type: u2
      - id: right_child_id
        type: u2
      - id: unk0
        type: u4

  vis_data_leaf:
    seq:
      - id: unk
        type: u4
      - id: ptr_some_data
        type: u4
      - id: num_quads
        type: u4
      - id: ptr_quads
        type: u4

  skybox:
    seq:
      - id: num_vertex
        type : u4
      - id: ptr_vertex
        type : u4 
      - id: num8
        type: u2
        repeat: expr
        repeat-expr: 8
      - id: ptr8
        type: u4
        repeat: expr
        repeat-expr: 8
      - id: vertex_array
        type: skybox_vertex
        repeat: expr
        repeat-expr: num_vertex

  scene_header:
    seq:
      - id: ptr_mesh_info
        type: u4
      - id: ptr_skybox
        type: u4
      - id: ptr_tex_array
        type: u4
      - id: num_instances
        type: u4
      - id: ptr_instances
        type: u4
      - id: num_models
        type: u4
      - id: ptr_models_ptr
        type: u4 
      - id: unk_ptr3
        type: u4
      - id: unk_ptr4
        type: u4
      - id: ptr_instances_ptr_array
        type: u4
      - id: unk_ptr5
        type: u4
      - id: null1
        type: u4
      - id: null2
        type: u4 
      - id: cnt_water
        type: u4
      - id: ptr_water
        type: u4 
      - id: ptr_named_tex
        type: u4
      - id: ptr_named_tex_array
        type: u4
      - id: ptr_restart_main
        type: u4

      - id: some_data
        type: somedata
        repeat: expr
        repeat-expr: 3

      - id: start_grid
        type: pose
        repeat: expr
        repeat-expr: 8

      - id: unkptr1
        type: u4
      - id: unkptr2
        type: u4
      - id: ptr_low_tex_array
        type: u4

      - id: back_color
        type: color 

      - id: bg_mode
        type: u4

      - id: build
        type: build_info

      - id: skip
        size: 0x44
 
      - id: cnt_trial_data
        type: u4 
      - id: ptr_trial_data
        type: u4 

      - id: cnt_spawn_arrays2
        type: u4 
      - id: ptr_spawn_arrays2
        type: u4 

      - id: cnt_spawn_arrays
        type: u4 
      - id: ptr_spawn_arrays
        type: u4 

      - id: cnt_restart_pts
        type: u4 
      - id: ptr_restart_pts
        type: u4 

      - id: skip2
        size: 16
      - id: bg_color
        type: color
        repeat: expr
        repeat-expr: 4
      - id: skip2_unkptr
        type: u4
      - id: cnt_vcanim
        type: u4
      - id: ptr_vcanim
        type: u4
      - id: skip2_3
        size: 12

      - id: ptr_ai_nav
        type: u4 

      - id: skip3
        size: 0x24

  trial_data:
    seq:
      - id: cnt_pointers
        type: u4
      - id: ptr_map
        type: u4
      - id: ptr_null
        type: u4
      - id: ptr_post_cam
        type: u4
      - id: ptr_intro_cam
        type: u4
      - id: ptr_tropy_ghost
        type: u4
        if: cnt_pointers == 6
      - id: ptr_oxide_ghost
        type: u4
        if: cnt_pointers == 6
        
  somedata:
    seq:
      - id: s1
        type: u2
      - id: s2
        type: u2 
      - id: s3
        type: u4
      - id: s4
        type: u4

  pose:
    seq:
      - id: position
        type: vector3s
      - id: angle
        type: vector3s

  vector3u:
    seq:
      - id: x
        type: u2
      - id: y
        type: u2
      - id: z
        type: u2

  vector3s:
    seq:
      - id: x
        type: s2
      - id: y
        type: s2
      - id: z
        type: s2

  vector4s:
    seq:
      - id: x
        type: s2
      - id: y
        type: s2
      - id: z
        type: s2
      - id: w
        type: s2

  vector2b:
    seq:
      - id: x
        type: u1
      - id: y
        type: u1

  instance:
    seq:
      - id: name
        size: 0x10
        type: str
        encoding: ascii
      - id: model_ptr
        type: u4
      - id: scale
        type: vector3s
      - id: scale1
        type: u2
      - id: null1
        type: u4
      - id: unk1
        type: u4
      - id: skip
        size: 12
      - id: position
        type: vector3s
      - id: angle
        type: vector3s
      - id: event
        type: u4

  mesh_info:
    seq:
      - id: cnt_quad_block
        type: u4
      - id: vertexnum
        type: u4
      - id: unk1
        type: u4
      - id: ptr_quadblock_array
        type: u4
      - id: ptr_vert_array
        type: u4
      - id: unk2
        type: u4
      - id: ptr_vis_data_array
        type: u4
      - id: cnt_vis_data
        type: u4

  bounding_box:
    seq:
      - id: min
        type: vector3s
      - id: max
        type: vector3s
 
  ctr_tex:
    seq:
      - id: mid_tex
        type: texture_layout
        repeat: expr
        repeat-expr: 3
      - id: ptr_hi
        type: u4

  quad_block:
    seq:
      - id: indices
        type: u2
        repeat: expr
        repeat-expr: 9
      - id: quad_flags
        type: u2
      - id: draw_order_low
        type: u1
      - id: f1
        type: u1
      - id: f2
        type: u1
      - id: f3
        type: u1
      - id: draw_order_high
        size: 4
      - id: ptr_texture_mid
        type: u4
        repeat: expr
        repeat-expr: 4
      - id: bbox
        type: bounding_box
      - id: terrain_type
        type: u1
      - id: weather_intensity
        type: u1
      - id: weather_type
        type: u1
      - id: terrain_flag_unknown
        type: u1
      - id: block_id
        type: u2
      - id: progress_tracker
        type: u1
      - id: midflag_unk
        type: u1
      - id: ptr_texture_low
        type: u4
      - id: ptr_texture_high
        type: u4
      - id: unk_col_array
        type: u2
        repeat: expr
        repeat-expr: 10
    instances:
      midtex1:
        pos: ptr_texture_mid[0]
        type: ctr_tex
      midtex2:
        pos: ptr_texture_mid[1]
        type: ctr_tex
      midtex3:
        pos: ptr_texture_mid[2]
        type: ctr_tex
      midtex4:
        pos: ptr_texture_mid[3]
        type: ctr_tex

  vertex:
    seq: 
      - id: coord
        type: vector3s
      - id: nil
        type: u2
      - id: colorz
        type: color
      - id: color_morph
        type: color

  skybox_vertex:
    seq:
      - id: position
        type: vector4s
      - id: colorz
        type: color

  color:
    seq:
      - id: r
        type: u1
      - id: g
        type: u1
      - id: b
        type: u1
      - id: a
        type: u1


  unk_struct:
    seq:
      - id: self_ptr
        type: u4
      - id: cnt_entries
        type: u4
      - id: some_ptr2
        type: u4
      - id: nil
        type: u4
      - id: some_ptr3
        type: u4
      - id: entries
        type: unk_entry
        repeat: expr
        repeat-expr: cnt_entries

  unk_entry:
    seq:
      - id: name
        type: strz
        encoding: ascii
        size: 16
      - id: entry_type
        type: u4
      - id: layout
        type: texture_layout
        if: entry_type != 0x86

  ai_frame_header:
    seq:
      - id: unk1
        type: u2
      - id: num_frames
        type: u2
      - id: skip
        size: 0x48

  nav_frame:
    seq:
      - id: point
        type: pose
      - id: data
        size: 8

  ai_path:
    seq:
      - id: header
        type: ai_frame_header
      - id: start
        type: nav_frame
      - id: frames
        type: nav_frame
        repeat: expr
        repeat-expr: header.num_frames

  ai_paths:
    seq:
      - id: ptr
        type: u4
        repeat: expr
        repeat-expr: 3
      - id: paths
        type: ai_path
        repeat: expr
        repeat-expr: 3
        doc: rewrite it.
        if: ptr[0] != 0 and ptr[1] != 0 and ptr[2] != 0

  texture_layout:
    seq:
      - id: uv1
        type: vector2b
      - id: pal_x
        type: b6
      - id: pal_y
        type: b10
      - id: uv2
        type: vector2b
      - id: page_x
        type: b4
      - id: page_y
        type: b12
      - id: uv3
        type: vector2b
      - id: uv4
        type: vector2b

  water_packet: 
    seq:
      - id: ptr_vertex
        type: u4
      - id: ptr_anim
        type: u4

  build_info:
    seq:
      - id: ptr_build_start
        type: u4
      - id: ptr_build_end
        type: u4
      - id: ptr_build_type
        type: u4
    instances:
      build_start:
        type: strz
        encoding: ascii
        pos: ptr_build_start
      build_end:
        type: strz
        encoding: ascii
        pos: ptr_build_end
      build_type:
        type: strz
        encoding: ascii
        pos: ptr_build_type