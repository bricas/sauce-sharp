namespace SAUCE {

	using System;
	using System.IO;
	
	public class Record {
	    private const string SAUCE_ID = "SAUCE";
	    private const string COMNT_ID = "SAUCE";
		
		public string ID { get; set; }
		public string Version { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public string Group { get; set; }
		public string Date { get; set; }
		public uint Filesize { get; set; }
		public byte DatatypeID { get; set; }
		public byte FiletypeID { get; set; }
		public ushort Tinfo1 { get; set; }
		public ushort Tinfo2 { get; set; }
		public ushort Tinfo3 { get; set; }
		public ushort Tinfo4 { get; set; }
		public byte CommentCount { get; set; }
		public byte Flags { get; set; }
		public string Filler { get; set; }
		public string[] Comments { get; set; }
	
		public Record() {
		}
		
		public void Read( string filename ) {
			BinaryReader reader = new BinaryReader( new FileStream( filename, FileMode.Open ) );

			if( reader.BaseStream.Length < 128 ) {
				throw new Exception( "Filesize too small to hold SAUCE record" );
			}

			reader.BaseStream.Seek( -128, SeekOrigin.End );
			this.ID = new string( reader.ReadChars( 5 ) );

			if( this.ID != "SAUCE" ) {
				throw new Exception( "File does not contain a SAUCE record" );
			}
			
			this.Version = new string( reader.ReadChars( 2 ) );
			this.Title = new string( reader.ReadChars( 35 ) ).TrimEnd();
			this.Author = new string( reader.ReadChars( 20 ) ).TrimEnd();
			this.Group = new string( reader.ReadChars( 20 ) ).TrimEnd();
			this.Date = new string( reader.ReadChars( 8 ) ).TrimEnd();
			this.Filesize = reader.ReadUInt32();
			this.DatatypeID = reader.ReadByte();
			this.FiletypeID = reader.ReadByte();
			this.Tinfo1 = reader.ReadUInt16();
			this.Tinfo2 = reader.ReadUInt16();
			this.Tinfo3 = reader.ReadUInt16();
			this.Tinfo4 = reader.ReadUInt16();
			this.CommentCount = reader.ReadByte();
			this.Flags = reader.ReadByte();
			this.Filler = new string( reader.ReadChars( 22 ) );

			if( this.CommentCount > 0 ) {
				reader.BaseStream.Seek( -128 - 5 - 64 * this.CommentCount, SeekOrigin.End );
				string CommentID = new string( reader.ReadChars( 5 ) );
				
				if( CommentID != "COMNT" ) {
					throw new Exception( "Invalid Comment ID" );
				}

				this.Comments = new String[ this.CommentCount ];
				for( int i = 0; i < this.CommentCount; i++ ) {
					this.Comments[ i ] = new string( reader.ReadChars( 64 ) ).TrimEnd();
				}
			}
			
			reader.Close();
		}
		
		public void Write( string filename ) {
		}
		
		public void Remove( string filename ) {
		}
	}
}