meta:
  id: ctr_mcs_ghost
  application: Crash Team Racing
  title: CTR ghost save slot
  file-extension: mcs
  endian: le

seq:
  - id: skip
    size: 256
  
  - id: magic
    contents: [0xfc, 0xff]
  - id: data_size
    type: s2
  - id: track_index
    type: s2
    enum: tracks
  - id: char_index
    type: s2
    enum: chars
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

enums:
  chars:
    0: c0
    1: c1
    2: c2
    3: coco
    4: c4
    5: dingo
    8: pinstripe
  tracks:
    0: canyon
    1: mines
    2: bluff
    3: cove
    4: temple
    5: pyramid
    6: tube
    7: skyway
    8: sewer
    9: cave
    10: castle
    11: labs
    12: mines
    13: station
    14: park
    15: arena
    16: coliseum
    17: turbo