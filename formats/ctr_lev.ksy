meta:
  id: ctr_lev
  application: Crash Team Racing
  title: Crash Team Racing (PS1) scene file
  file-extension: lev
  endian: le

doc-ref: https://github.com/CTR-tools/CTR-tools/blob/master/formats/ctr_lev.ksy

seq:
  - id: struct_level1_size
    type: u4
  - id: lev_file
    type: lev_file
    size: level_data_size
  - id: psx_patch_table
    type: psx_patch_table
    if: ext_ptr_map == 0

instances:
  ext_ptr_map:
    value: struct_level1_size >> 31
  level_data_size:
    value: struct_level1_size & ~(1 << 31)

types:
  
  model_ptrarray:
    doc: "this is to show model data giving it the model addres as parameter"
    params:
      - id: ptr
        type: u4
    instances:
       model:
         pos: ptr
         type: ctr_model
         if: ptr != 0
         
  icongroup_ptrarray:
     params:
       - id: ptr
         type: u4
     instances:
       icongroup:
         pos: ptr
         type: icon_group
         if: ptr != 0
         
  icongroup4_ptrarray:
     params:
       - id: ptr
         type: u4
     instances:
       icongroup4_array:
         type: icongroup4
         pos: ptr

  psx_patch_table:
    doc: | 
      an array of offsets that is used to convert relative pointers to
      absolute psx ram pointers
    seq:
      - id: size
        type: u4
      - id: entries
        type: u4
        repeat: expr
        repeat-expr: size / 4
  
  get_pointers_array:
    params:
      - id: array_ptr
        type: u4
      - id: index
        type: u4
    instances:
      curr_ptr:
        pos: array_ptr + (index * 4)
        type: u4
       

  lev_file:
    doc: |
      main scene struct
      contains all scene data
    seq:
      - id: struct_level
        type: level1
        
      - id: model_ptrs
        type: get_pointers_array(struct_level.ptr_models_ptr_array, _index)
        repeat: expr
        repeat-expr: struct_level.num_models
        
        
      - id: model_array
        type: model_ptrarray(model_ptrs[_index].curr_ptr)
        repeat: expr
        repeat-expr: struct_level.num_models

    
    
    instances:
      
      vertex_array:
        type: lev_vertex
        pos: struct_mesh_info.ptr_vertex_array
        repeat: expr
        repeat-expr: struct_mesh_info.num_vertices
        
      water_env_map:
        type: texture_layout
        pos: struct_level.ptr_tex_water_env_map
        if: struct_level.ptr_tex_water_env_map != 0
    
      restart_points_array:
        type: check_point_node
        pos: struct_level.ptr_restart_points
        repeat: expr
        repeat-expr: struct_level.cnt_restart_points
    
      inst_def_array:
        type: inst_def
        pos: struct_level.ptr_inst_defs
        repeat: expr
        repeat-expr: struct_level.num_instances

      quad_block_array:
        type: quad_block
        pos: struct_mesh_info.ptr_quad_block_array
        repeat: expr
        repeat-expr: struct_mesh_info.num_quad_blocks
        
  
        
    
      struct_mesh_info:
        type: mesh_info
        pos: struct_level.ptr_mesh_info
      st1:
        pos: struct_level.ptr_spawn_type1
        type: spawn_type1
        if: struct_level.ptr_spawn_type1 != 0
        
      st2:
        pos: struct_level.ptr_spawn_type2
        type: spawn_type2
        repeat: expr
        repeat-expr: struct_level.num_spawn_type2
        if: struct_level.ptr_spawn_type2 != 0

      struct_scvert:
        pos: struct_level.ptr_sc_vert
        type: scvert
        repeat: expr
        repeat-expr: struct_level.num_sc_vert
        if: struct_level.ptr_sc_vert != 0

      struct_skybox:
        pos: struct_level.ptr_skybox
        type: skybox
        if: struct_level.ptr_skybox != 0
        
      struct_animtex:
        pos: struct_level.ptr_anim_tex
        type: anim_tex
        if: struct_level.ptr_anim_tex != 0
      
        
      

            
       
      bsp_data_array:
        pos: struct_mesh_info.ptr_bsp_root
        type: bsp_root
        repeat: expr
        repeat-expr: struct_mesh_info.num_bsp_nodes
        if: struct_mesh_info.ptr_bsp_root != 0
    
      struct_lev_tex_lookup:
        pos: struct_level.ptr_tex_look_up
        type: lev_tex_lookup
        if: struct_level.ptr_tex_look_up != 0
    
      lev_ai_nav_table:
        pos: struct_level.ptr_lev_ai_nav_table_array
        type: ai_paths
        if: struct_level.ptr_lev_ai_nav_table_array != 0
    
      struct_water_vert:
        pos: struct_level.ptr_water
        type: water_vert
        repeat: expr
        repeat-expr: struct_level.num_water_vertices
        if: struct_level.ptr_water != 0
        
        


  bsp_root:
    doc: | 
      bsp tree used for level drawing
      simply an array of either leaf or branch
    seq:
      - id: flag
        type: u2
      - id: id
        type: u2
      - id: bbox_min
        type: s_vec3
      - id: bbox_max
        type: s_vec3 
      - id: data
        type:
          switch-on: flag & 1
          cases:
            0: vis_data_branch
            1: vis_data_leaf

  vis_data_branch:
    doc: |
      bsp tree branch node
      contains split axis info and children indices
    seq:
      - id: axis
        type: u2
        repeat: expr
        repeat-expr: 4
      - id: child_i_d
        type: s2
        repeat: expr
        repeat-expr: 4

  vis_data_leaf:
    doc: | 
      bsp tree branch termination node
      points to a set of quadblocks
    seq:
      - id: unk1
        type: u4
      - id: ptr_bsp_hitbox_array
        type: u4
      - id: num_quads
        type: u4
      - id: ptr_quad_block_array
        type: u4

  skybox:
    doc: | 
      skybox struct, contains 8 even segments
    seq:
      - id: num_vertex
        type: u4
      - id: ptr_vertex
        type: u4 
      - id: num_faces_per_segment_array
        type: u2
        repeat: expr
        repeat-expr: 8
      - id: ptr_segments
        type: u4
        repeat: expr
        repeat-expr: 8
      - id: vertex_array
        type: ptr_to_skybox_vertex(ptr_vertex, num_vertex)

      - id: skybox_segments
        type: skybox_segment(ptr_segments[_index], num_faces_per_segment_array[_index])
        repeat: expr
        repeat-expr: 8

  skybox_segment:
    params:
      - id: ptr
        type: u4
      - id: num_faces
        type: u4
    instances:
      faces_array:
        type: skybox_face
        pos: ptr
        repeat: expr
        repeat-expr: num_faces

  level1:
    doc: | 
      scene header, contains pointers to other data within the file and
      various global data like starting grid, background colors, etc. 
    seq:
      - id: ptr_mesh_info
        type: u4
      - id: ptr_skybox
        type: u4
      - id: ptr_anim_tex
        type: u4
      - id: num_instances
        type: u4
      - id: ptr_inst_defs
        type: u4
      - id: num_models
        type: u4
      - id: ptr_models_ptr_array
        type: u4
      - id: bsp_unk3
        type: u4
      - id: bsp_unk4
        type: u4
      - id: ptr_inst_def_ptr_array
        type: u4
      - id: water_bsp_unk5
        type: u4
      - id: null1
        type: u4
      - id: null2
        type: u4 
      - id: num_water_vertices
        type: u4
      - id: ptr_water
        type: u4 
      - id: ptr_tex_look_up
        type: u4
      - id: ptr_named_tex_array
        type: u4
      - id: ptr_tex_water_env_map # probably only if water is present, maybe shared
        type: u4

      - id: glow_gradient
        type: gradient
        repeat: expr
        repeat-expr: 3

      - id: driver_spawn
        type: pos_rot
        repeat: expr
        repeat-expr: 8

      - id: unk_ptr4
        type: u4
      - id: unk_ptr5
        type: u4
      - id: ptr_low_tex_array
        type: u4

      - id: clear_color_rgba
        type: color_r_g_b_cd

      - id: config_flags
        type: u4

      - id: build
        type: build_info
        
      - id: unk_ec
        type: s1
        repeat: expr
        repeat-expr: 0x18

      - id: struct_rain_buffer
        type: rain_buffer
 
      - id: ptr_spawn_type1
        type: u4 

      - id: num_spawn_type2
        type: u4 
      - id: ptr_spawn_type2
        type: u4 

      - id: num_spawn_type2_pos_rot
        type: u4 
      - id: ptr_spawn_type2_pos_rot
        type: u4 

      - id: cnt_restart_points
        type: u4 
      - id: ptr_restart_points
        type: u4 

      - id: unk_150
        type: s1
        repeat: expr
        repeat-expr: 16
        
      - id: clear_color
        type: clear_color
        repeat: expr
        repeat-expr: 3
        
      - id: unk16c # probably not
        type: s4
      - id: unk170
        type: s4
      - id: num_sc_vert
        type: u4
      - id: ptr_sc_vert
        type: u4
      - id: struct_stars
        type: stars
      - id: split_lines
        type: s2
        repeat: expr
        repeat-expr: 2
        
      - id: ptr_lev_ai_nav_table_array
        type: u4

      - id: unk18c
        type: u4
        
      - id: vismem_ptr
        type: u4
        
        
      - id: footer
        size: 0x1C
    
    instances:
      struct_icon_named_tex_array:
        pos: ptr_named_tex_array
        type: icon
        if: ptr_named_tex_array != 0
      first_instdef:
        pos: ptr_inst_defs
        type: inst_def
        if: num_instances != 0
      low_tex_first:
        pos: ptr_low_tex_array
        type: texture_layout
      vismem_struct:
        pos: vismem_ptr
        type: vismem_struct

  spawn_type1:
    seq:
      - id: count
        type: u4
      - id: ptr_minimap
        type: u4
        if: count >= 1
      - id: ptr_inst_meta_data
        type: u4
        if: count >= 2
      - id: ptr_camera_eor
        type: u4
        if: count >= 3
      - id: ptr_camera_path
        type: u4
        if: count >= 4
      - id: ptr_n_tropy
        type: u4
        if: count >= 5
      - id: ptr_noxide
        type: u4
        if: count >= 6
      - id: ptr_credits
        type: u4
        if: count >= 7
    instances:
      minimap:
        type: struct_minimap
        pos: ptr_minimap
        if: count >= 1 and ptr_minimap != 0
      inst_meta_data:
        size: 104
        pos: ptr_inst_meta_data
        if: count >= 2 and ptr_inst_meta_data != 0
      camera_eor:
        size: 252
        pos: ptr_camera_eor
        if: count >= 3 and ptr_camera_eor != 0
      camera_path:
        size: 1752
        pos: ptr_camera_path
        if: count >= 4 and ptr_camera_path != 0
      tropy_ghost:
        type: ghost_data
        pos: ptr_n_tropy
        if: count >= 5 and ptr_n_tropy != 0
      oxide_ghost:
        type: ghost_data
        pos: ptr_noxide
        if: count >= 6 and ptr_noxide != 0
      credits:
        type: credits_text
        pos: ptr_credits
        if: count >= 7 and ptr_credits != 0

  struct_minimap:
    seq:
      - id: world_end_x # ? some sort of scale
        type: s2
      - id: world_end_y # ? some sort of scale too, close to prev but negative
        type: s2
      - id: world_start_x # ? some sort of scale
        type: s2
      - id: world_start_y # ? some sort of scale too, close to prev but negative
        type: s2
      - id: icon_size_x
        type: s2
      - id: icon_size_y
        type: s2
      - id: icon_start_x
        type: s2
      - id: icon_start_y
        type: s2
      - id: mode # 0, 1, 2, 3 - no, hor, vert, both
        type: s2
      - id: unk # hides top half if not null, why?
        type: u2

  ghost_data:
    doc: | 
      ghost data used in time trial mode, same data saved to memcard
    seq:
      - id: version
        contents: [0xfc, 0xff]
      - id: data_size
        type: s2
      - id: level_i_d
        type: s2
        #enum: tracks
      - id: character_i_d
        type: s2
        #enum: chars
      - id: speed_approx
        type: s4
      - id: y_speed
        type: s4
      - id: time_elapsed_in_race
        type: s4
        #decomp says this is empty but track data says the opposite
      - id: empty_padding
        type: s1
        repeat: expr
        repeat-expr: 16
      - id: record_buffer
        size: data_size
        
  ghost_tape:
    seq:
      - id: ghost_header_ptr
        type: u4
      - id: ptr_start
        type: u4
      - id: ptr_end
        type: u4
      - id: ptr_curr
        type: u4
      - id: unk10
        type: s4
      - id: time_elapsed_in_race
        type: s4
      - id: time_in_packet32_backup
        type: s4
      - id: unk1c
        type: s4
      - id: unk20
        type: s4
      - id: unk1
        type: s2
        repeat: expr
        repeat-expr: 3
      - id: unk2
        type: s2
        repeat: expr
        repeat-expr: 3
      - id: unk3
        type: s2
        repeat: expr
        repeat-expr: 3
      - id: unk4
        type: s2
        repeat: expr
        repeat-expr: 3
      - id: time_in_packet01
        type: s4
      - id: time_between_packets
        type: s4
      - id: num_packets_in_array
        type: s4
      - id: packet_i_d
        type: s4
      - id: ghost_packets_array
        type: ghost_packet
        repeat: expr
        repeat-expr: 0x21
      - id: const_dead_c0ed
        type: s4
      - id: gh_ptr_again
        type: u4
   
  ghost_packet:
    seq:
    - id: pos
      type: s_vec3
    - id: time
      type: s2
    - id: rot
      type: s2
      repeat: expr
      repeat-expr: 2
    - id: buffer_packet_ptr
      type: u4
      
  gradient:
    seq:
      - id: point_from
        type: s2
      - id: point_to
        type: s2
      - id: color_from
        type: color_b_g_r_cd
      - id: color_to
        type: color_b_g_r_cd

  inst_def:
    doc: |
      describes a single instance of the model on the map
      used to spawn hazards, crates, etc.
    seq:
      - id: name
        size: 0x10
        type: str
        encoding: ascii
      - id: model_ptr
        type: u4
      - id: scale
        type: s_vec3
      - id: alpha_scale
        type: s2
      - id: color_rgba
        type: color_r_g_b_cd
      - id: flags
        type: u4
      - id: unk24
        type: s4
      - id: unk28
        type: s4
      - id: ptr_struct_instance
        type: u4
      - id: posrot
        type: pos_rot
      - id: model_i_d
        type: u4
        
    instances:
      inst_model:
        pos: model_ptr
        type: ctr_model
      

  mesh_info:
    doc: | 
      mesh header struct, contains pointer to vertex array, quadblock array 
      and visdata array
    seq:
      - id: num_quad_blocks
        type: u4
      - id: num_vertices
        type: u4
      - id: unk1
        type: u4
      - id: ptr_quad_block_array
        type: u4
      - id: ptr_vertex_array
        type: u4
      - id: unk2
        type: s4
      - id: ptr_bsp_root
        type: u4
      - id: num_bsp_nodes
        type: u4


  quad_block:
    doc: |
      describes the atomic entity of the level
      contains 4 quads and various per quad flag based info
    seq:
      - id: index
        type: u2
        repeat: expr
        repeat-expr: 9
      - id: quad_flags
        type: u2
      - id: draw_order_low
        type: u4
      - id: draw_order_high
        type : u4
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
      - id: weather_vanish_rate
        type: u1
      - id: speed_impact
        type: s1
      - id: block_id
        type: u2
      - id: checkpoint_index
        type: u1
      - id: tri_normal_vec_bit_shift
        type: u1
      - id: ptr_texture_low
        type: u4
      - id: pvs_ptr
        type: u4
      - id: tri_normal_vec_dividend
        type: u2
        repeat: expr
        repeat-expr: 10
    instances:
      texture_mid0:
        pos: ptr_texture_mid[0]
        type: icongroup4
      texture_mid1:
        pos: ptr_texture_mid[1]
        type: icongroup4
      texture_mid2:
        pos: ptr_texture_mid[2]
        type: icongroup4
      texture_mid3:
        pos: ptr_texture_mid[3]
        type: icongroup4
      struct_pvs:
        pos: pvs_ptr
        type: pvs
        if: pvs_ptr != 0

  pvs:
    doc: |
    seq:
      - id: vis_leaf_src_ptr
        type: u4
      - id: vis_face_src_ptr
        type: u4
      - id: vis_inst_src_ptr_array_ptr
        type: u4
      - id: vis_extra_src_ptr
        type: u4

        
  lev_vertex:
    doc: |
      describes a single vertex
      contains spacial coordinate, vertex color and additional color value,
      that is used when morphing between lod levels 
    seq:
      - id: pos
        type: s_vec3
      - id: flags
        type: u2
      - id: color_hi
        type: color_b_g_r_cd
      - id: color_lo
        type: color_b_g_r_cd

        
        


  clear_color:
    seq:
      - id: r
        type: u1
      - id: g
        type: u1
      - id: b
        type: u1
      - id: enable
        type: s1
        
  stars:
    seq:
    - id: num_stars
      type: s2
    - id: spread
      type: s2
    - id: seed
      type: s2
    - id: distance
      type: s2
  
  
  svector:
    seq:
      - id: vx
        type: s2
      - id: vy
        type: s2
      - id: vz
        type: s2
      - id: pad
        type: s2
        
        
     
  skybox_face:
    seq:
    - id: pos
      type: s_vec4
    
  icongroup4:
    seq:
      - id: far
        type: texture_layout
      - id: middle
        type: texture_layout
      - id: near
        type: texture_layout
      - id: mosaic
        type: texture_layout
        
        
  anim_tex:
    seq:
      - id: ptr_active_tex
        type: u4
      - id: num_frames
        type: s2
      - id: frame_offset
        type: s2
      - id: frame_skip
        type: s2
      - id: frame_curr
        type: s2
        
      - id: ptr_to_icongroup4_ptr_array
        type: u4
        repeat: expr
        repeat-expr: num_frames
        
      - id: icongroup4_array
        type: icongroup4_ptrarray(ptr_to_icongroup4_ptr_array[_index])
        repeat: expr
        repeat-expr: num_frames
        
    instances:
      active_tex:
        pos: ptr_active_tex
        type: icongroup4
        if: ptr_active_tex != 0
        
  
  model_struct:
    seq:
    - id: data_size
      type: u4
    - id: struct_model
      type: ctr_model
      size: data_size
    - id: ptr_map_size
      type: u4
    - id: ptr_map
      type: u4
      repeat: expr
      repeat-expr: ptr_map_size / 4
      
  ctr_model:
    seq:
      - id: name
        type: strz
        encoding: ascii
        size: 16
      - id: id # this is an internal number
        type: s2
      - id: num_headers
        type: u2
      - id: ptr_headers
        type: u4
    instances:
      model_headers:
        type: model_header
        pos: ptr_headers
        repeat: expr
        repeat-expr: num_headers
        
  model_header:
    seq:
      - id: name
        type: strz
        encoding: ascii
        size: 16
      - id: unk1
        type: u4
      - id: max_distance_l_o_d
        type: u2
      - id: flags
        type: u2
      - id: scale
        type: s_vec4

      - id: ptr_command_list
        type: u4
      - id: ptr_frame_data
        type: u4
      - id: ptr_tex_layout
        type: u4
      - id: ptr_colors
        type: u4
      - id: unk3
        type: u4
      - id: num_animations
        type: u4
      - id: ptr_animations
        type: u4
      - id: ptr_anim_tex
        type: u4
    instances:
      command_list:
        type: command
        pos: ptr_command_list
        repeat: until
        repeat-until: _.value == 0xFFFFFFFF
      model_anim_array:
        type: ctr_anim
        pos: ptr_animations
        repeat: expr
        repeat-expr: num_animations
        if: ptr_animations != 0
      model_frame:
        pos: ptr_frame_data
        type: ctr_model_frame(0)
        if: ptr_frame_data != 0
        
  command:
    seq:
      - id: value
        type: u4be
    instances:
      new_face_block:
        value: (value & (1 << 31)) >> 31
      swap_first_vertex:
        value: (value & (1 << 30)) >> 30
      flip_face_normal:
        value: (value & (1 << 29)) >> 29
      cull_backface:
        value: (value & (1 << 28)) >> 28
      color_scratchpad:
        value: (value & (1 << 27)) >> 27
      read_vertex_stack:
        value: (value & (1 << 26)) >> 26
      unk1:
        value: (value & (1 << 25)) >> 25
      unk2:
        value: (value & (1 << 24)) >> 24
      stack_write_index:
        value: (value >> 16) & 0b11111111
      color_coord_index:
        value: (value >> 9) & 0b1111111
      tex_coord_index:
        value: value & 0b111111111
        
  ctr_anim:
    seq:
      - id: ptr
        type: u4
    instances:
      struct_model_anim:
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
      - id: ptr_delta_array
        type: u4
      - id: struct_model_frame
        type: ctr_model_frame(frame_size)
        repeat: expr
        repeat-expr: num_frames_pack
    instances:
      interp:
        value: num_frames_pack & 0x8000 > 0
      num_frames:
        value: "interp ? (num_frames_pack & 0x7FFF) / 2 + 1 : num_frames_pack"
        
  ctr_model_frame:
    params:
      - id: frame_size
        type: u4
    seq:
      - id: position
        type: s_vec4

      - id: unk_16
        type: s1
        repeat: expr
        repeat-expr: 16
      - id: vertex_offset
        type: u4
      - id: verts
        size: frame_size - vertex_offset
        if: frame_size != 0
        
       
  
  vismem_struct:
    doc: "all the variables are supposed to be one per player screen"
    seq:
      - id: vis_leaf_list_ptr
        type: u4
        repeat: expr
        repeat-expr: 4
      - id: vis_face_list_ptr
        type: u4
        repeat: expr
        repeat-expr: 4
      - id: vis_o_vert_list_ptr
        type: u4
        repeat: expr
        repeat-expr: 4
      - id: vis_s_c_vert_list_ptr
        type: u4
        repeat: expr
        repeat-expr: 4
      - id: vis_leaf_src_ptr  
        type: u4
        repeat: expr
        repeat-expr: 4
      - id: vis_face_src_ptr
        type: u4
        repeat: expr
        repeat-expr: 4
      - id: vis_o_vert_src_ptr
        type: u4
        repeat: expr
        repeat-expr: 4
      - id: vis_s_c_vert_src_ptr
        type: u4
        repeat: expr
        repeat-expr: 4
      - id: bsp_list_ptr
        type: u4
        repeat: expr
        repeat-expr: 4
        
        
  rain_buffer:
    doc: |
      weather on all levels
    seq:
      - id: num_particles_curr
        type: s4
      - id: num_particles_max
        type: s2

      - id: vanish_rate
        type: s2
      - id: unk8_clock
        type: s1
        repeat: expr
        repeat-expr: 4
        

      - id: unkc_bool_pos
        type: s1

      - id: unkd_particle_pos
        type: s1
      - id: unke_unused
        type: s1
        repeat: expr
        repeat-expr: 2

      - id: fall_angle_x
        type: s1
      - id: speed_x
        type: s1
      - id: falling_speed
        type: s1
      - id: speed_y
        type: s1
      - id: fall_angle_z
        type: s1
      - id: speed_z
        type: s1
      - id: unk_visibility_bools
        type: s1
        repeat: expr
        repeat-expr: 2
        
      - id: camera_pos
        type: s_vec3
      - id: unk_22
        type: s2
      - id: color_rgbcd_top
        type: color_r_g_b_cd
      - id: color_rgbcd_bottom
        type: color_r_g_b_cd
      - id: fill_mode
        type: u4
      - id: offset_o_t
        type: s4
        
      
  skbox_vertex:
    seq:
      - id: position
        type: svector
      - id: color
        type: color_b_g_r_cd
  
  ptr_to_skybox_vertex:
    params:
      - id: ptr
        type: u4
      - id: num_vertex
        type: s4
    instances:
      skbox_vertex:
        type: skbox_vertex
        pos: ptr
        repeat: expr
        repeat-expr: num_vertex
        
  scvert:
    doc: |
      controls vertex animation, both color and position (roo's tubes best example).
    seq:
      - id: v
        type: u4
      - id: offset_pos_xy
        type: s4
      - id: offset_pos_zw
        type: s4
      - id: offset_color_rgba
        type: u4

  lev_tex_lookup:
    doc: |
      describes a set of icons
      used either in lev scene or mpk file
    seq:
      - id: num_icon
        type: s4
      - id: ptr_first_icon
        type: u4
      - id: num_icon_group
        type: s4
      - id: first_icon_group_ptr_array
        type: u4
        
      - id: icongroup_ptrs
        type: get_pointers_array(first_icon_group_ptr_array, _index)
        repeat: expr
        repeat-expr: num_icon_group
        
      - id: icongroup_array
        type: icongroup_ptrarray(icongroup_ptrs[_index].curr_ptr)
        repeat: expr
        repeat-expr: num_icon_group
        
    instances:
      icon_array:
        pos: ptr_first_icon
        type: icon
        repeat: expr
        repeat-expr: num_icon
        if: ptr_first_icon != 0
      

  icon:
    doc: |
      essentially, a tagged vram region
    seq:
      - id: name
        type: strz
        encoding: ascii
        size: 16
      - id: global_icon_array_index
        type: s4
      - id: layout
        type: texture_layout

  icon_group:
    doc: |
      some icons are grouped together (like all wheel sprites)
    seq:
      - id: name
        type: strz
        encoding: ascii
        size: 16
      - id: group_id
        type: s2
      - id: num_icons
        type: s2
      - id: ptr_icons_array
        type: u4
    instances:
      icons_on_this_group:
        pos: ptr_icons_array
        type: icon
        repeat: expr
        repeat-expr: num_icons
        if: ptr_icons_array != 0 
        

  nav_header:
    doc: |
      ai path header data
    seq:
      - id: version # assumed to be, game code tests against const value 
        type: u2
      - id: num_points
        type: u2
      - id: pos_y_first_node
        type: s4
      - id: nav_frame_last
        type: u4
      - id: ram_phys1
        type: s2
        repeat: expr
        repeat-expr: 16
      - id: ram_phys2
        type: s2
        repeat: expr
        repeat-expr: 16

  nav_frame:
    doc: |
      describes a single navigation point for bots
    seq:
      - id: pos
        type: s_vec3
      - id: rot
        type: u1
        repeat: expr
        repeat-expr: 4
      - id: unk2
        type: s2
        repeat: expr
        repeat-expr: 2
        
      - id: flags
        type: s2
      - id: path_change_opcode
        type: s2
      - id: go_back_count
        type: u1
      - id: special_bits
        type: u1

  ai_data:
    params:
      - id: ptr
        type: u4
    instances:
      bot:
        type: ai_path
        pos: ptr
        if: ptr != 0

        
  ai_path:
    doc: "describes a set of navigation points for bots to follow"
    seq:
      - id: nav_header
        type: nav_header
      - id: start_line 
        type: nav_frame
      - id: nav_frame_array
        type: nav_frame
        repeat: expr
        repeat-expr: nav_header.num_points
        
  ai_paths:
    doc: |
      contains 3 bots paths for each difficulty level
    seq:
      - id: ptr_to_ai_paths_ptr_array
        type: u4
        repeat: expr
        repeat-expr: 3

      - id: ai_paths
        type: ai_data(ptr_to_ai_paths_ptr_array[_index])
        repeat: expr
        repeat-expr: 3
        if: ptr_to_ai_paths_ptr_array[_index] != 0

          

  texture_layout:
    doc: |
      a struct to describe vram region.
      contains 4 UV coords, palette coord and texture page index
      as well as bpp flag, maybe more?
    seq:
      - id: u0
        type: u1
      - id: v0
        type: u1
      - id: clut
        type: clut
      - id: u1
        type: u1
      - id: v1
        type: u1
      - id: tpage
        type: poly_tpage
      - id: u2
        type: u1
      - id: v2
        type: u1
      - id: u3
        type: u1
      - id: v3
        type: u1

  water_vert:
    seq:
      - id: lev_vertex_v_ptr
        type: u4
      - id: overt_w_ptr
        type: u4
    instances:
      struct_lev_vertex:
        pos: lev_vertex_v_ptr
        type: lev_vertex
      struct_o_vert:
        pos: overt_w_ptr
        type: overt
        
  overt:
    seq:
    - id: data
      type: s2
      repeat: expr
      repeat-expr: 2
    

  build_info:
    doc: |
      contains pointer to strings, assumed to define vistree compilation times
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
        if: ptr_build_start != 0
      build_end:
        type: strz
        encoding: ascii
        pos: ptr_build_end
        if: ptr_build_end != 0
      build_type:
        type: strz
        encoding: ascii
        pos: ptr_build_type
        if: ptr_build_type != 0

  credits_text:
    doc: |
      used in crashdance credits file
    seq:
      - id: chunk_size
        type: u4
      - id: num_entries
        type: u4
      - id: ptr_text
        type: u4
        repeat: expr
        repeat-expr: num_entries
      - id: skip
        type: u4
      - id: text
        type: strz
        encoding: ascii
        repeat: expr
        repeat-expr: num_entries

  spawn_type2:
    seq:
      - id: num_coords
        type: s4
      - id: pos_coords_array_ptr
        type: u4
    instances:
      pos_coords:
        type: pos_rot
        pos: pos_coords_array_ptr

  vector2b:
    seq:
      - id: x
        type: u1
      - id: y
        type: u1
  clut:
    seq:
      - id: x
        type: b6
      - id: y
        type: b9
      - id: nop
        type: b1
        
  poly_tpage:
    seq:
      - id: x
        type: b4
      - id: y
        type: b1
      - id: semi_transparency
        type: b2
      - id: texpage_colors
        type: b2
      - id: unused
        type: b2
      - id: y_vram_exp
        type: b1
      - id: unused2
        type: b2
      - id: nop
        type: b2


        
  color_r_g_b_cd:
    seq:
      - id: r
        type: b8
      - id: g
        type: b8
      - id: b
        type: b8
      - id: cd
        type: b8
        
  color_b_g_r_cd:
    seq:
      - id: b
        type: b8
      - id: g
        type: b8
      - id: r
        type: b8
      - id: cd
        type: b8
        
        
  vec2:
    seq:
      - id: x
        type: s4
      - id: y
        type: s4

  vec3:
    seq:
      - id: x
        type: s4
      - id: y
        type: s4
      - id: z
        type: s4
        
  vec4:
    seq:
      - id: x
        type: s4
      - id: y
        type: s4
      - id: z
        type: s4
      - id: w
        type: s4

  s_vec2:
    seq:
      - id: x
        type: s2
      - id: y
        type: s2

  s_vec3:
    seq:
      - id: x
        type: s2
      - id: y
        type: s2
      - id: z
        type: s2

  s_vec4:
    seq:
      - id: x
        type: s2
      - id: y
        type: s2
      - id: z
        type: s2
      - id: w
        type: s2

  pos_rot:
  
    seq:
      - id: position
        type: s_vec3
      - id: rotation
        type: s_vec3

  check_point_node:
    seq:
      - id: position
        type: s_vec3
      - id: dist_to_finish
        type: u2
      - id: next_index_forward
        type: u1
      - id: next_index_left
        type: u1
      - id: next_index_backward
        type: u1
      - id: next_index_right
        type: u1
        
  bounding_box:
    seq:
      - id: min
        type: s_vec3
      - id: max
        type: s_vec3
