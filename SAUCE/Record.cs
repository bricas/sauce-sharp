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
                reader.Close();
                throw new Exception( "Filesize too small to hold SAUCE record" );
            }

            reader.BaseStream.Seek( -128, SeekOrigin.End );
            this.ID = new string( reader.ReadChars( 5 ) );

            if( this.ID != "SAUCE" ) {
                reader.Close();
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
            SAUCE.Record record = new SAUCE.Record();
            try {
                record.Read( filename );
            }
            catch {
            }
            
            if( record.ID == "SAUCE" ) {
                this.Remove( filename );
            }

            FileStream fs = new FileStream( filename, FileMode.Open );
            BinaryWriter writer = new BinaryWriter( fs );
            writer.Write( "\x26".ToCharArray() );
            writer.Write( SAUCE_ID.ToCharArray() );
            writer.Write( this.Version.ToCharArray() );
            writer.Write( this.Title.PadRight( 35 ).ToCharArray() );
            writer.Write( this.Author.PadRight( 20 ).ToCharArray() );
            writer.Write( this.Group.PadRight( 20 ).ToCharArray() );
            writer.Write( this.Date.ToCharArray() );
            writer.Write( (UInt32) this.Filesize );
            writer.Write( this.DatatypeID );
            writer.Write( this.FiletypeID );
            writer.Write( (UInt16) this.Tinfo1 );
            writer.Write( (UInt16) this.Tinfo2 );
            writer.Write( (UInt16) this.Tinfo3 );
            writer.Write( (UInt16) this.Tinfo4 );
            writer.Write( (Byte) 0 ); // TODO: Comment Count
            writer.Write( this.Flags );
            writer.Write( this.Filler.ToCharArray() );
            writer.Close();
        }
        
        public void Remove( string filename ) {
            SAUCE.Record record = new SAUCE.Record();
            record.Read( filename );
            
            if( record.ID != "SAUCE" ) {
                return;
            }

            BinaryWriter writer = new BinaryWriter( new FileStream( filename, FileMode.Open ) );
            writer.BaseStream.SetLength( record.Filesize );
            writer.Close();
        }
    }
}
