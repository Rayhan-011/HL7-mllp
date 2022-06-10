<h1>MLLP.HL7Reader</h1>
<P>
 Minimal lower-layer protocol to communicate with servers that use minimal LLP. It's effort where readers and writer can be used to easily communicate with thes Message Sender Devices.
</P>

<P>
The MLLP block is framed by certain bytes. The characters that are sent within the MLLP block need to be encoded in a way that doesn't conflict with the framing bytes. Some multi-byte character encodings (like UTF-16 or UTF-32) might result in errors because their byte values are equal to the framing bytes or lower than 0x1F. These encodings aren't supported by MLLP. MLLP supports all single-byte character encodings (like iso-8859-x and cp1252), as well as UTF-8 and Shift_JIS. The byte values used by UTF-8 don't conflict with the MLP framing bytes.
  </P>
