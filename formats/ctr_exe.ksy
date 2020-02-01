meta:
  id: ctr_exe
  application: Crash Team Racing
  title: Crash Team Racing (PS1) executable
  file-extension: SCUS_944.26
  endian: le

seq:
  - id: psx
    type: str
    size: 16
    encoding: ascii
  - id: ptr_exec
    type: u4
  - id: unk_val
    type: u4
  - id: ptr_text
    type: u4
  - id: size_text
    type: u4
  - id: skip
    size: 16
  - id: ptr_stack
    type: u4
  - id: skip2
    size: 24
  - id: region_stamp
    type: strz
    encoding: ascii    
  - id: skip3
    size: 0x794
  - id: x000818_ptr_tbl1
    type: psx_ptr
    repeat: expr
    repeat-expr: 45
    
  - id: x0008cc_strings
    type: string_pad
    repeat: expr
    repeat-expr: 221
    
  - id: skip4
    size: 16
    
  - id: x001874_strings
    type: string_pad
    repeat: expr
    repeat-expr: 3
    
  - id: x00018a0_ptr_tbl
    type: psx_ptr
    repeat: expr
    repeat-expr: 11
    
  - id: x000018cc
    type: string_pad
    
  - id: x0018e0_ptr_tbl2
    type: psx_ptr
    repeat: expr
    repeat-expr: 37 
    
  - id: x00001974
    type: string_pad
    
    size: 0x3c
    
  - id: skip7
    size: 0xe8
    
  - id: x0001aa4_ptr_tbl
    type: psx_ptr
    repeat: expr
    repeat-expr: 16
    
  - id: x001ae4_strings2
    type: string_pad
    repeat: expr
    repeat-expr: 31 
    
  - id: nill1
    type: u4
    
  - id: x0001c8c_ptr_tbl
    type: psx_ptr
    repeat: expr
    repeat-expr: (0x194 / 4)
    
  - id: x001e20_strings2
    type: string_pad
    repeat: expr
    repeat-expr: 3   
    
  - id: nill2
    type: u4  
    
  - id: x0001e50_ptr_tbl
    type: psx_ptr
    repeat: expr
    repeat-expr: (0x60 / 4)    
    
  - id: x001eb0_strings2
    type: string_pad
    repeat: expr
    repeat-expr: 5 
    
  - id: x001eec_arr
    type: u2
    repeat: expr
    repeat-expr: 8
    
  - id: x001f08_strings2
    type: string_pad
    repeat: expr
    repeat-expr: 3 

  - id: x0001f18_ptr_tbl
    type: psx_ptr
    repeat: expr
    repeat-expr: (0x28 / 4)      
    
  - id: x0001f40_characters
    type: character
    repeat: expr
    repeat-expr: 16    
      
  - id: x0020b8_strings
    type: string_pad
    repeat: expr
    repeat-expr: 12
   
  - id: skip5
    size: 16
    
  - id: x002160_strings
    type: string_pad
    repeat: expr
    repeat-expr: 5

instances:

  x71888_triplets:
    pos: 0x71888
    type: triplet
    repeat: expr
    repeat-expr: 120
    
  x074280_level_slots:
    pos: 0x74280
    type: level_slot
    repeat: expr
    repeat-expr: 65
    
  x07cc40_ptr_tbl3:
    pos: 0x7CC40
    type: psx_ptr
    repeat: expr
    repeat-expr: 13 * 3 + 1
    
  x07ac08_ptr_tbl3:
    pos: 0x7ac08
    type: psx_ptr
    repeat: expr
    repeat-expr: 0x78 / 4

  x07dd94_more_strings:
    pos: 0x7DD94
    type: string_pad
    repeat: expr
    repeat-expr: 26
    
  x072340_colorboxes:
    pos: 0x72340
    type: colorbox
    repeat: expr
    repeat-expr: 35

  x072570_ptr_colorboxes:
    pos: 0x72570
    type: psx_ptr
    repeat: expr
    repeat-expr: 0x8c / 4
    
  x7ab84_wheelanim:
    pos: 0x7ab84
    size: 0x84
    
  x78ccc_terrains:
    pos: 0x78ccc
    type: terrain
    repeat: expr
    repeat-expr: 21
    
  x77584_chars:
    pos: 0x77584
    type: char_slot
    repeat: expr
    repeat-expr: 16
    
types:

  terrain:
    seq:
      - id: name
        type: string_ptr
      - id: flags
        type: u4
      - id: pushback
        type: u4
      - id: acceleration
        type: u4
      - id: turning_slowdown_maybe
        type: u4
      - id: turning_slowdown_maybe_too
        type: u4
        
      - id: ptr_part_def1
        type: psx_ptr  
      - id: ptr_part_def2
        type: psx_ptr   
        
      - id: val6
        type: u4
      - id: val7
        type: u4
      - id: val8
        type: u4  
      - id: val9_unknown_4bytes
        type: u4  
        
      - id: sval1
        type: u2
      - id: sval2
        type: u2
      - id: sval3
        type: u2
      - id: sval4
        type: u2
      - id: sval5
        type: u2
      - id: sval6
        type: u2
      - id: sval7
        type: u2
      - id: sval8
        type: u2
        
  colorbox:
    seq:
      - id: tl
        type: color
      - id: tr
        type: color
      - id: bl
        type: color
      - id: br
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
        
  character:
    seq:
      - id: name
        type: string_pad
      - id: empty
        size: 0x10

  triplet:
    seq:
      - id: x1
        type: string_ptr
      - id: x2
        type: string_ptr
      - id: x3
        type: string_ptr

  psx_ptr:
    seq:
      - id: value
        type: u4
    instances:
      converted:
        value: value - _root.ptr_text + 0x800
        if: value != 0

  string_pad:
    seq:
      - id: text
        type: strz
        encoding: ascii
      - id: pad
        size: (text.length / 4 + 1) * 4  - (text.length + 1)

  string_ptr:
    seq:
      - id: ptr
        type: psx_ptr
    instances:
      text:
        type: string_pad
        pos: ptr.converted

  char_slot:
    seq:
      - id: ptr_model_name
        type: string_ptr
      - id: name_index_long
        type: u2
      - id: name_index_short
        type: u2
      - id: char_icon_index
        type: u2
      - id: always0
        type: u2
      - id: difficulty
        type: u4
        enum: difficulty

  level_slot:
    seq:
      - id: unk1
        type: s2
      - id: unk2_null
        type: u2
      - id: filename_ptr
        type: string_ptr
      - id: title_id
        type: u4
      - id: some_size
        type: s4
      - id: unk3
        type: s2
      - id: unk4
        type: s2
      - id: boss_level
        type: s2
      - id: unk6
        type: s2

enums:
  difficulty:
    0: default_intermediate
    1: intermediate
    2: advanced
    3: begginer
    4: pal_penta