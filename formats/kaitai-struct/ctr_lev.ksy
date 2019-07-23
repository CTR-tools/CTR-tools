meta:
  id: ctr_lev
  application: Crash Team Racing
  file-extension: lev
  endian: le
  
seq:
    
  - id: header
    type: scene_header
    
  - id: array1array
    type: array1
    repeat: expr
    repeat-expr: header.count_unknown_array1
        
  - id: pickup_headers
    type: pickup_header
    repeat: expr
    repeat-expr: header.num_pickup_headers
    
  - id: some_pointers
    type: u4
    repeat: expr
    repeat-expr: 9
    
  - id: mesh_info_header
    type: mesh_info
    
  - id: quad_block_array
    type: quad_block
    repeat: expr
    repeat-expr: mesh_info_header.facesnum

  - id: vertex_array
    type: vertex
    repeat: expr
    repeat-expr: mesh_info_header.vertexnum

instances:
  unk_struct:
    pos: header.ptr_unk_struct
    size: 1
    type: unk_struct
  
types:
  unk_struct:
    seq:
      - id: num_repeats
        type : u4
      - id: ptr_data_array
        type : u4   
      - id: num8
        type: u2
        repeat: expr
        repeat-expr: 8
      - id: ptr8
        type: u4
        repeat: expr
        repeat-expr: 8
      - id: data_array
        type: posang
        repeat: expr
        repeat-expr: num_repeats

  scene_header:
    seq:
      - id: ptr_mesh_info
        type: u4
      - id: ptr_unk_struct
        type: u4
      - id: unk_ptr2
        type: u4
      - id: num_pickup_headers
        type: u4
      - id: ptr_pickup_headers
        type: u4
      - id: num_pickup_models
        type: u4
      - id: ptr_faces_array
        type: u4     
      - id: unk_ptr3
        type: u4
      - id: unk_ptr4
        type: u4
      - id: ptr_pickup_headers_ptr_array
        type: u4
      - id: unk_ptr5
        type: u4
      - id: null1
        type: u4
      - id: null2
        type: u4   
      - id: some_count1
        type: u4
      - id: some_ptr1
        type: u4     
      - id: some_ptr2
        type: u4
      - id: some_ptr3
        type: u4
      - id: ptr_array1
        type: u4
        
      - id: some_data
        type: somedata
        repeat: expr
        repeat-expr: 3
        
      - id: start_grid
        type: posang
        repeat: expr
        repeat-expr: 8
        
      - id: skip
        size: 0x7C
        
      - id: count_unknown_array1
        type: u4 
          
      - id: ptr_unknown_array1
        type: u4 
        
      - id: skip2
        size: 0x38
        
      - id: ptr_ai_nav
        type: u4 
        
      - id: skip3
        size: 0x24

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
        
  posang:
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
        
  array1:
    seq:
      - id: unk1
        type: u2
      - id: unk2
        type: u2
      - id: unk3
        type: u2
      - id: unk4
        type: u2
      - id: unk5
        type: u2
      - id: unk6
        type: u2
        
  pickup_header:
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
      - id: facesnum
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
      - id: ptr_face_array
        type: u4
      - id: facenum
        type: u4
        
  bounding_box:
    seq:
      - id: min
        type: vector3s
      - id: max
        type: vector3s
    
  quad_block:
    seq:
      - id: indices
        type: u2
        repeat: expr
        repeat-expr: 9
      - id: flags
        size: 10
      - id: tex
        type: u4
        repeat: expr
        repeat-expr: 4
      - id: bbox
        type: bounding_box
      - id: unk2
        type: u4
      - id: block_id
        type: u2
      - id: midflags
        type: u2
      - id: offset1
        type: u4
      - id: offset2
        type: u4
      - id: unkarray
        type: u2
        repeat: expr
        repeat-expr: 10
        
  vertex:
    seq: 
      - id: coord
        type: vector3s
      - id: nil
        type: u2
      - id: color
        type: u4
      - id: color_morph
        type: u4