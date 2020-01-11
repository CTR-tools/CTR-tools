meta:
  id: ctr_hwl
  application: Crash Team Racing
  title: Crash Team Racing (PS1) HOWL engine file
  file-extension: hwl
  endian: le

seq:
  - id: header
    type: howl_hdr
  - id: unk_array
    type: array1
    repeat: expr
    repeat-expr: header.cnt4
  - id: sample1
    type: sample
    repeat: expr
    repeat-expr: header.cnt81
  - id: sample2
    type: sample
    repeat: expr
    repeat-expr: header.cnt82
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
      - id: magic
        size: 4
        type: strz
        encoding: ascii
      - id: unk1
        type: u4
      - id: null1
        type: u4
      - id: null2
        type: u4
      - id: cnt4
        type: u4
      - id: cnt81
        type: u4
      - id: cnt82
        type: u4
      - id: cnt_banks
        type: u4
      - id: cnt_seqs
        type: u4 
      - id: sample_data_size
        type: u4

  array1: 
    seq:
      - id: val1
        type: u2
      - id: val2
        type: u2
        
  sample: 
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