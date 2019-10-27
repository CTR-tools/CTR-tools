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
    
    
instances:

  ptr_tbl1:
    pos: 0x818
    type: psx_ptr
    repeat: expr
    repeat-expr: 45

  strings:
    pos: 0x8CC
    type: string_pad
    repeat: expr
    repeat-expr: 221
    
  ptr_tbl2:
    pos: 0x18E0
    type: psx_ptr
    repeat: expr
    repeat-expr: 37
    
  strings2:
    pos: 0x1AE4
    type: string_pad
    repeat: expr
    repeat-expr: 31
    
  level_slots:
    pos: 0x74280
    type: level_slot
    repeat: expr
    repeat-expr: 65
    
types:

  psx_ptr:
    seq:
      - id: value
        type: u4
    instances:
      converted:
        value: value - _root.ptr_text + 0x800

  string_pad:
    seq:
      - id: text
        type: strz
        encoding: ascii
      - id: pad
        size: (text.length / 4 + 1) * 4  - (text.length + 1)

  level_slot:
    seq:
      - id: unk1
        type: s2
      - id: unk2_null
        type: u2
      - id: filename_ptr
        type: psx_ptr
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
    instances:
      filename:
        pos: filename_ptr.converted
        type: strz
        encoding: ascii