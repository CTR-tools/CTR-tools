meta:
  id: ctr_lng
  application: Crash Team Racing
  title: Crash Team Racing (PS1) localization file
  file-extension: lng
  endian: le

seq:
  - id: num_entries
    type: u4
  - id: ptr_offsets
    type: u4
    
instances:
  entries:
      type: entry
      pos: ptr_offsets
      repeat: expr
      repeat-expr: num_entries
      
  missingmsg:
    type: strz
    encoding: ascii
    pos: ptr_offsets + 4 * num_entries

types:
  entry:
    seq:
      - id: offset
        type: u4
    instances:
      text:
        type: strz
        encoding: ascii
        pos: offset