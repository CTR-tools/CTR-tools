meta:
  id: ctr_xnf
  application: Crash Team Racing
  title: Crash Team Racing (PS1) XA mapping file
  file-extension: xnf
  endian: le

seq:
  - id: magic
    type: str
    size: 4
    encoding: ascii
  - id: version #static value, code checks against hardcoded int, assumed version
    type: u4
  - id: num_groups #seems to be always 3. basically 1 for every folder in XA. file halts parsing if not 3.
    type: u4
  - id: num_skip_ints # wat?
    type: u4
  - id: num_total_entries
    type: u4
  - id: num_xa_files
    type: u4
    repeat: expr
    repeat-expr: num_groups
  - id: unk2_groups
    type: u4
    repeat: expr
    repeat-expr: num_groups
  - id: num_entries # all summed should be equal to num_total_entries
    type: u4
    repeat: expr
    repeat-expr: num_groups
  - id: start_index # tells where exactly the group starts in the array of entries
    type: u4
    repeat: expr
    repeat-expr: num_groups
  - id: skip_data
    size: num_skip_ints * 4
  - id: entry
    type: u4
    repeat: expr
    repeat-expr: num_total_entries