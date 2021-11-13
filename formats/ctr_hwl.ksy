meta:
  id: ctr_hwl
  application: Crash Team Racing
  title: Crash Team Racing (PS1) HOWL engine file
  file-extension: hwl
  endian: le

doc-ref: https://github.com/CTR-tools/CTR-tools/blob/master/formats/ctr_hwl.ksy

seq:
  - id: header
    type: howl_hdr
  - id: unk_array
    type: ptr_array # ?
    repeat: expr
    repeat-expr: header.num_ptr_array
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

  howl_hdr:
    seq:
      - id: magic # HOWL char string
        size: 4
        type: strz
        encoding: ascii
      - id: version # const in game code
        type: u4
      - id: reserved1
        type: u4
      - id: reserved2
        type: u4
      - id: num_ptr_array # ?
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

  ptr_array:
    seq:
      - id: val1
        type: u2
      - id: val2
        type: u2

  instrument:
    seq:
      - id: unk1
        type: u1
      - id: volume
        type: u1
      - id: pitch
        type: u2
      - id: sample_id
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