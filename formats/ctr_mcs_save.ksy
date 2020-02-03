meta:
  id: ctr_mcs_save
  application: Crash Team Racing
  title: CTR save slot
  file-extension: mcs
  endian: le

seq:
  - id: header
    size: 128
  
  - id: unk88
    type: u4
    
  - id: title
    type: strz
    encoding: sjis
    size: 16*6-4
    
  - id: icon
    type: icon
    
  - id: saves
    type: slot
    repeat: expr
    repeat-expr: 4
    
  - id: unk
    size: 16
    
  - id: levels
    type: level_times_slot
    repeat: expr
    repeat-expr: 18
    
  - id: unk2
    type: u2
    repeat: expr
    repeat-expr: 15
  
  - id: skip
    size: 0x88
    
  - id: checksum
    type: u2
    
types:
  icon:
    seq:
      - id: palette
        type: u2
        repeat: expr
        repeat-expr: 16
      - id: data
        size: 16*8

  slot:
    seq:
      - id: data
        size: 16 * 5
  
  level_times_slot:
    seq:
      - id: trial_slots
        type: level_time
        repeat: expr
        repeat-expr: 12
      - id: unk
        type: u4
        
  level_time:
    seq:
      - id: time
        type: u4
      - id: name
        type: strz
        encoding: ascii
        size: 0x12
      - id: char
        type: u2