meta:
  id: ctr_hwl
  application: Crash Team Racing
  title: Crash Team Racing (PS1) HOWL engine file
  file-extension: hwl
  endian: le

doc-ref: https://github.com/CTR-tools/CTR-tools/blob/master/formats/ctr_hwl.ksy

seq:
  - id: header
    type: howl_header
  - id: spu_addr_table
    type: spu_addr # ?
    repeat: expr
    repeat-expr: header.num_spu_addr
  - id: sfx_instruments
    type: instrument
    repeat: expr
    repeat-expr: header.num_sfx_instruments
  - id: engine_instruments
    type: instrument
    repeat: expr
    repeat-expr: header.num_engine_instruments
  - id: banks
    type: bank_inst
    repeat: expr
    repeat-expr: header.cnt_banks
  - id: seqs
    type: cseq_inst
    repeat: expr
    repeat-expr: header.cnt_seqs

types:

  howl_header:
    seq:
      - id: magic # HOWL char string
        size: 4
        type: strz
        encoding: ascii
      - id: version # const in game code
        type: u4
      - id: reserved1 # null
        type: u4
      - id: reserved2 # null
        type: u4
      - id: num_spu_addr # ?
        type: u4
      - id: num_sfx_instruments
        type: u4
      - id: num_engine_instruments
        type: u4
      - id: cnt_banks
        type: u4
      - id: cnt_seqs
        type: u4 
      - id: sample_data_size
        type: u4

  spu_addr:
    seq:
      - id: ptr # null, allocated at runtime
        type: u2
      - id: size # vag data size / 8
        type: u2

  instrument:
    seq:
      - id: unk1
        type: u1
      - id: volume
        type: u1
      - id: pitch
        type: u2
      - id: sample_id # index in spuaddr table
        type: u2
      - id: unk2
        type: u2

  bank_inst:
    seq:
      - id: ptr
        type: u2
    instances:
      bank:
        type: bank
        pos: ptr * 0x800

  # be aware that bank sample data is not covered here
  # it's just a stream of headerless vag sample data
  # the format doesnt provide bank sizes, so it has to be calculated
  # using the individual sample sizes and it starts at 0x800 padding

  bank:
    seq:
      - id: cnt_samps
        type: u2
      - id: sample_id
        type: u2
        repeat: expr
        repeat-expr: cnt_samps

  cseq_inst:
    seq:
      - id: ptr
        type: u2
    instances:
      seq:
        type: cseq
        pos: ptr * 0x800

  cseq:
    seq:
      - id: size
        type: u4
      - id: data
        size: size-4