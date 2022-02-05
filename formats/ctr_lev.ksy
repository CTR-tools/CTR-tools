meta:
  id: ctr_lev
  application: Crash Team Racing
  title: Crash Team Racing (PS1) scene file
  file-extension: lev
  endian: le

doc-ref: https://github.com/CTR-tools/CTR-tools/blob/master/formats/ctr_lev.ksy

seq:
  - id: header_value
    type: u4
  - id: scene
    type: scene
    size: data_size
  - id: patch_table
    type: t_patch_table
    if: ext_ptr_map == 0

instances:
  ext_ptr_map:
    value: header_value >> 31
  data_size:
    value: header_value & ~(1 << 31)

types:

  t_patch_table:
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

  scene:
    doc: |
      main scene struct
      contains all scene data
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
        repeat-expr: mesh_info_header.num_quad_blocks
    
      - id: vertex_array
        type: vertex
        repeat: expr
        repeat-expr: mesh_info_header.num_vertices
    
    instances:
      trial:
        pos: header.ptr_trial_data
        type: trial_data
        if: header.ptr_trial_data != 0
        
      spawn_inst:
        pos: header.ptr_spawn_arrays2
        type: spawn_type
        repeat: expr
        repeat-expr: header.cnt_spawn_arrays2
        if: header.ptr_spawn_arrays2 != 0

      vcolors:
        pos: header.ptr_vcanim
        type: vcolor
        repeat: expr
        repeat-expr: header.num_vcanim
        if: header.ptr_vcanim != 0

      skybox:
        pos: header.ptr_skybox
        type: skybox
        if: header.ptr_skybox != 0
    
      vis_data_array:
        pos: mesh_info_header.ptr_vis_data
        type: vis_data
        repeat: expr
        repeat-expr: mesh_info_header.num_vis_data
        if: mesh_info_header.ptr_vis_data != 0
    
      icons:
        pos: header.ptr_icons
        type: icon_pack
        if: header.ptr_icons != 0
    
      ai_nav:
        pos: header.ptr_ai_nav
        type: ai_paths
        if: header.ptr_ai_nav != 0
    
      water_data:
        pos: header.ptr_water
        type: water_packet
        repeat: expr
        repeat-expr: header.cnt_water
        if: header.ptr_water != 0

  vis_data:
    doc: | 
      bsp tree used for level drawing
      simply an array of either leaf or branch
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
    doc: |
      bsp tree branch node
      contains split axis info and children indices
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
    doc: | 
      bsp tree branch termination node
      points to a set of quadblocks
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
    doc: | 
      skybox struct, contains 8 even segments
    seq:
      - id: num_vertex
        type : u4
      - id: ptr_vertex
        type : u4 
      - id: num_faces
        type: u2
        repeat: expr
        repeat-expr: 8
      - id: ptr_faces
        type: u4
        repeat: expr
        repeat-expr: 8
      - id: vertex_array
        type: skybox_vertex
        repeat: expr
        repeat-expr: num_vertex
      - id: faces
        type: skybox_segment(num_faces[_index])
        repeat: expr
        repeat-expr: 8

  skybox_segment:
    params:
      - id: num_entries
        type: u4
    seq:
      - id: faces
        type: vector4s
        repeat: expr
        repeat-expr: num_entries

  scene_header:
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
      - id: ptr_instances
        type: u4
      - id: num_models
        type: u4
      - id: ptr_models_ptr
        type: u4 
      - id: unk_ptr1
        type: u4
      - id: unk_ptr2
        type: u4
      - id: ptr_instances_ptr
        type: u4
      - id: unk_ptr3_related_to_water_anim
        type: u4
      - id: null1
        type: u4
      - id: null2
        type: u4 
      - id: cnt_water
        type: u4
      - id: ptr_water
        type: u4 
      - id: ptr_icons
        type: u4
      - id: ptr_icons_array
        type: u4
      - id: ptr_restart_main
        type: u4

      - id: glow_gradient
        type: gradient
        repeat: expr
        repeat-expr: 3

      - id: start_grid
        type: pose
        repeat: expr
        repeat-expr: 8

      - id: unk_ptr4
        type: u4
      - id: unk_ptr5
        type: u4
      - id: ptr_low_tex_array
        type: u4

      - id: back_color
        type: color 

      - id: some_render_flags
        type: u4

      - id: build
        type: build_info

      - id: skip_possibly_partice_related
        size: 0x38
      - id: particle_color_top
        type: color
      - id: particle_color_bottom
        type: color
      - id: particle_render_mode
        type: u4
 
      - id: cnt_trial_data
        type: u4 
      - id: ptr_trial_data
        type: u4 

      - id: cnt_spawn_arrays2
        type: u4 
      - id: ptr_spawn_arrays2
        type: u4 

      - id: cnt_spawn_groups
        type: u4 
      - id: ptr_spawn_groups
        type: u4 

      - id: cnt_restart_pts
        type: u4 
      - id: ptr_restart_pts
        type: u4 

      - id: skip2
        size: 16
      - id: bg_color_top
        type: color
      - id: bg_color_bottom
        type: color
      - id: grad_color
        type: color
      - id: color4 # probably not
        type: u4
      - id: skip2_unkptr_related_to_vcol_anim
        type: u4
      - id: num_vcanim
        type: u4
      - id: ptr_vcanim
        type: u4
      - id: num_stars
        type: u2
      - id: unk_stars_bool
        type: u2
      - id: unk_stars_flags
        type: u2
      - id: stars_depth # or some OT order
        type: u2
      - id: unk_after_stars
        type: u2
      - id: water_level
        type: u2
      - id: ptr_ai_nav
        type: u4

      - id: skip_null
        type: u4
        
      - id: unk_ptr6
        type: u4
        
      - id: skip3
        size: 0x1C

  trial_data:
    seq:
      - id: cnt_pointers
        type: u4
      - id: ptr_map
        type: u4
        if: cnt_pointers >= 1
      - id: ptr_null
        type: u4
        if: cnt_pointers >= 2
      - id: ptr_post_cam
        type: u4
        if: cnt_pointers >= 3
      - id: ptr_intro_cam
        type: u4
        if: cnt_pointers >= 4
      - id: ptr_tropy_ghost
        type: u4
        if: cnt_pointers >= 5
      - id: ptr_oxide_ghost
        type: u4
        if: cnt_pointers >= 6
      - id: ptr_credits_text
        type: u4
        if: cnt_pointers >= 7
    instances:
      map:
        type: map_coords
        pos: ptr_map
        if: cnt_pointers >= 1 and ptr_map != 0
      post_cam:
        type: u4
        pos: ptr_post_cam
        if: cnt_pointers >= 3 and ptr_post_cam != 0
      intro_cam:
        type: u4
        pos: ptr_intro_cam
        if: cnt_pointers >= 4 and ptr_intro_cam != 0
      tropy_ghost:
        type: ghost_data
        pos: ptr_tropy_ghost
        if: cnt_pointers >= 5 and ptr_tropy_ghost != 0
      oxide_ghost:
        type: ghost_data
        pos: ptr_oxide_ghost
        if: cnt_pointers >= 6 and ptr_oxide_ghost != 0
      text:
        type: credits_text
        pos: ptr_credits_text
        if: cnt_pointers >= 7 and ptr_credits_text != 0

  map_coords:
    seq:
      - id: v1 # ? some sort of scale
        type: vector2s
      - id: v2 # ? some sort of scale too, close to prev but negative
        type: vector2s
      - id: scale
        type: vector2s
      - id: position
        type: vector2s
      - id: mirror # 0, 1, 2, 3 - no, hor, vert, both
        type: u2
      - id: unk # hides top half if not null, why?
        type: u2

  ghost_data:
    doc: | 
      ghost data used in time trial mode, same data saved to memcard
    seq:
      - id: magic
        contents: [0xfc, 0xff]
      - id: data_size
        type: s2
      - id: track_index
        type: s2
        #enum: tracks
      - id: char_index
        type: s2
        #enum: chars
      - id: unk2
        type: s4
      - id: unk3
        type: s4
      - id: unk4
        type: s4
      - id: ptrs
        type: u4
        repeat: expr
        repeat-expr: 5
      - id: data
        size: data_size
        
  gradient:
    seq:
      - id: point_from
        type: s2
      - id: point_to
        type: s2
      - id: color_from
        type: color
      - id: color_to
        type: color

  instance:
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
        type: vector4s
      - id: null1
        type: u4
      - id: unk1
        type: u4
      - id: skip
        size: 12
      - id: pose
        type: pose
      - id: event
        type: u4

  mesh_info:
    doc: | 
      mesh header struct, contains pointer to vertex array, quadblock array 
      and visdata array
    seq:
      - id: num_quad_blocks
        type: u4
      - id: num_vertices
        type: u4
      - id: num_unk
        type: u4
      - id: ptr_quadblocks
        type: u4
      - id: ptr_vertices
        type: u4
      - id: ptr_unk
        type: u4
      - id: ptr_vis_data
        type: u4
      - id: num_vis_data
        type: u4

  ctr_tex:
    seq:
      - id: mid_tex
        type: texture_layout
        repeat: expr
        repeat-expr: 3
      - id: ptr_hi
        type: u4

  quad_block:
    doc: |
      describes the atomic entity of the level
      contains 4 quads and various per quad flag based info
    seq:
      - id: indices
        type: u2
        repeat: expr
        repeat-expr: 9
      - id: quad_flags
        type: u2
      - id: face_data
        type: u4
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
      - id: ptr_add_vis
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
      add_tex:
        pos: ptr_add_vis
        type: add_tex
        if: ptr_add_vis != 0

  add_tex:
    doc: |
      points to 4 structs, one of them assumed to store mosaic texture def
      each pointer might include flag hidden in lsb
    seq:
      - id: ptr1
        type: u4
      - id: ptr2
        type: u4
      - id: ptr3
        type: u4
      - id: ptr4
        type: u4
    instances:
      ptr1data:
        pos: ptr1 & 0xFFFFFFFC
        size: 1
        if: ptr1 != 0
      ptr2data:
        pos: ptr2 & 0xFFFFFFFC
        size: 1
        if: ptr2 != 0
      ptr3data:
        pos: ptr3 & 0xFFFFFFFC
        size: 1
        if: ptr3 != 0
      ptr4data:
        pos: ptr4 & 0xFFFFFFFC
        type: u4
        if: ptr4 != 0
        
  vertex:
    doc: |
      describes a single vertex
      contains spacial coordinate, vertex color and additional color value,
      that is used when morphing between lod levels 
    seq:
      - id: coord
        type: vector4s
      - id: vcolor
        type: color
      - id: vcolor_morph
        type: color

  skybox_vertex:
    doc: |
      shorter vertex defintion without morph color
    seq:
      - id: position
        type: vector4s
      - id: colorz
        type: color
        
  vcolor:
    doc: |
      controls vertex animation, both color and position (roo's tubes best example).
    seq:
      - id: ptr_vertex
        type: u4
      - id: u1
        type: u4
      - id: u2
        type: u4
      - id: color
        type: color

  icon_pack:
    doc: |
      describes a set of icons
      used either in lev scene or mpk file
    seq:
      - id: num_icons
        type: u4
      - id: ptr_tex_array
        type: u4
      - id: num_groups
        type: u4
      - id: ptr_groups
        type: u4
      - id: entries
        type: icon
        repeat: expr
        repeat-expr: num_icons
      - id: dummy
        type: u4
      - id: groups_ptr
        type: u4
        repeat: expr
        repeat-expr: num_groups
      - id: groups
        type: icon_group
        repeat: expr
        repeat-expr: num_groups

  icon:
    doc: |
      essentially, a tagged vram region
    seq:
      - id: name
        type: strz
        encoding: ascii
        size: 16
      - id: index
        type: u4
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
      - id: unk1
        type: u2
      - id: num_icons
        type: u2
      - id: entries
        type: u4
        repeat: expr
        repeat-expr: num_icons

  ai_frame_header:
    doc: |
      ai path header data
    seq:
      - id: version # assumed to be, game code tests against const value 
        type: u2
      - id: num_frames
        type: u2
      - id: skip
        size: 0x48

  nav_frame:
    doc: |
      describes a single navigation point for bots
    seq:
      - id: point
        type: pose
      - id: data
        size: 8

  ai_path:
    doc: |
      describes a set of navigation points for bots to follow
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
    doc: |
      contains 3 bots paths for each difficulty level
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
    doc: |
      a struct to describe vram region.
      contains 4 UV coords, palette coord and texture page index
      as well as bpp flag, maybe more?
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
    instances:
      waterdata:
        pos: ptr_anim
        size: 0x38

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

  spawn_type:
    seq:
      - id: unk
        type: u4
      - id: ptr
        type: u4

  vector2b:
    seq:
      - id: x
        type: u1
      - id: y
        type: u1

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

  vector3u:
    seq:
      - id: x
        type: u2
      - id: y
        type: u2
      - id: z
        type: u2

  vector2s:
    seq:
      - id: x
        type: s2
      - id: y
        type: s2

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

  pose:
    seq:
      - id: position
        type: vector3s
      - id: angle
        type: vector3s

  bounding_box:
    seq:
      - id: min
        type: vector3s
      - id: max
        type: vector3s