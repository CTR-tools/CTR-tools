meta:
  id: ctr_xnf
  application: Crash Team Racing
  title: Crash Team Racing (PS1) XA mapping file
  file-extension: xnf
  endian: le

doc-ref: https://github.com/CTR-tools/CTR-tools/blob/master/formats/ctr_xnf.ksy

seq:
  - id: magic # XINF
    type: str
    size: 4
    encoding: ascii
  - id: version #static value, code checks against hardcoded int, assumed version
    type: u4
  - id: num_groups #seems to be always 3. basically 1 for every folder in XA. file halts parsing if not 3.
    type: u4
  - id: num_total_files
    type: u4
  - id: num_total_entries
    type: u4
  - id: num_files # all summed should be equal to num_total_files
    type: u4
    repeat: expr
    repeat-expr: num_groups
  - id: file_start_index
    type: u4
    repeat: expr
    repeat-expr: num_groups
  - id: num_entries # all summed should be equal to num_total_entries
    type: u4
    repeat: expr
    repeat-expr: num_groups
  - id: entry_start_index # tells where exactly the group starts in the array of entries
    type: u4
    repeat: expr
    repeat-expr: num_groups
  - id: ptr_files # zero, calculated at runtime
    size: num_total_files * 4
  - id: entries
    type: xa_entry
    repeat: expr
    repeat-expr: num_total_entries
    
types:
  xa_entry:
    seq:
      - id: entry_index
        type: u1
      - id: file_index
        type: u1
      - id: entry_length # in sectors?
        type: u2