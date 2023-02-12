meta:
  id: ctr_vrm
  application: Crash Team Racing
  title: Crash Team Racing TIM stream
  file-extension: vrm
  endian: le

doc-ref: https://github.com/CTR-tools/CTR-tools/blob/master/formats/ctr_vrm.ksy

doc: |
  includes TIM format ksy spec from http://formats.kaitai.io/psx_tim/

seq:
  # here's what's going on, ctr checks if first value == 0x20
  # if it is, it's a stream of tims. if not, it's a simple tim starting at 0
  # the implementation is a bit messy in kaitai
  
  - id: tim_stream
    type:
      switch-on: magic_check
      cases:
        0x20: tim_stream
        _: tim

instances:
  magic_check:
    pos: 0
    type: u4

types:

  tim_stream:
    seq:
      - id: magic # 0x20
        type: u4
      - id: entries
        type: entry
        repeat: until
        repeat-until: _.data_size == 0

  entry:
    seq:
      - id: data_size
        type: u4
      - id: data
        type: tim
        if: data_size > 0

  tim:
    seq:
      - id: magic
        contents: [0x10, 0, 0, 0]
      - id: flags
        type: u4
      - id: clut
        type: bitmap
        if: has_clut
      - id: image
        type: bitmap
    instances:
      has_clut:
        value: flags & 0b1000 != 0
      bpp:
        value: flags & 0b0011

  bitmap:
    seq:
      - id: len
        type: u4
      - id: origin_x
        type: u2
      - id: origin_y
        type: u2
      - id: width
        type: u2
      - id: height
        type: u2
      - id: pixel_data
        size: len - 12 # 4 + 4 * 2
    
enums:
  bpp_type:
    0: bpp_4
    1: bpp_8
    2: bpp_16
    3: bpp_24