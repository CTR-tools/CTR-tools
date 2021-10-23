meta:
  id: ctr_big
  application: Crash Team Racing
  title: Crash Team Racing (PS1) BIG container
  file-extension: big
  endian: le

doc-ref: https://github.com/CTR-tools/CTR-tools/blob/master/formats/ctr_big.ksy

seq:
  - id: magic
    contents: [0, 0, 0, 0]
  - id: num_entries
    type: u4
  - id: entries
    type: big_entry
    repeat: expr
    repeat-expr: num_entries

types:
  big_entry:
    seq:
      - id: offset
        type: u4
      - id: size
        type: u4
    instances:
      data:
        size: size
        pos: offset * 2048