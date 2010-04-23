namespace SAUCE {
    
    using NUnit.Framework;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;
    
    [TestFixture]
    public class Test {
        private string filename = Path.Combine( Path.GetTempPath(), "W7-R666.ANS" );
        
        [Test (Description = "Test the Read() method" )]
        public void Read(){
            SAUCE.Record record = new SAUCE.Record();
            record.Read( filename );
            
            Assert.AreEqual( "SAUCE", record.ID );
            Assert.AreEqual( "00", record.Version );
            Assert.AreEqual( "Route 666", record.Title );
            Assert.AreEqual( "White Trash", record.Author );
            Assert.AreEqual( "ACiD Productions", record.Group );
            Assert.AreEqual( "19970401", record.Date );
            Assert.AreEqual( 42990, record.Filesize );
            Assert.AreEqual( 1, record.DatatypeID );
            Assert.AreEqual( 1, record.FiletypeID );
            Assert.AreEqual( 80, record.Tinfo1 );
            Assert.AreEqual( 180, record.Tinfo2 );
            Assert.AreEqual( 0, record.Tinfo3 );
            Assert.AreEqual( 0, record.Tinfo4 );
            Assert.AreEqual( 4, record.CommentCount );
            Assert.AreEqual( 0, record.Flags );
            Assert.AreEqual( "\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0", record.Filler );
            Assert.AreEqual( new string[4] {
                "To purchase your white trash ansi:  send cash/check to",
                "keith nadolny / 41 loretto drive / cheektowaga, ny / 14225",
                "make checks payable to keith nadolny/us funds only",
                "5 dollars = 100 lines - 10 dollars = 200 lines"
            }, record.Comments );
        }

        [Test (Description = "Test the Remove() method" )]
        public void Remove(){
            FileInfo info = new FileInfo( filename );
            long filesize_before = info.Length;
                
            SAUCE.Record record = new SAUCE.Record();
            record.Remove( filename );

            info.Refresh();

            Assert.AreEqual( filesize_before - 1 - 128 - 5 - (4 * 64), info.Length );
        }
            
        [SetUp()]
        public void Unpack()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            Stream s = a.GetManifestResourceStream( "etc.W7-R666.ANS" );
            StreamReader sr = new StreamReader( s, Encoding.GetEncoding( "cp437" ) );
            StreamWriter sw = new StreamWriter( File.Create( filename ), Encoding.GetEncoding( "cp437" ) );
            sw.Write( sr.ReadToEnd() );
            sw.Flush();
            sw.Close();
            sr.Close();
        }

        [TearDown()]
        public void CleanUp()
        {
            if( File.Exists( filename ) ) {
                File.Delete( filename );
            }
        }
    }
}
