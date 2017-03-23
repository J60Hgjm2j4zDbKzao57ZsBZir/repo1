using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;

using Newtonsoft.Json;

namespace Console
{
    public class Program
    {
        
        static void Main( string[] args )
        {
            System.Console.Write( "Please enter the input file name: " );
            //string inputFile = System.Console.ReadLine();
            string inputFile = "aa1.txt";

            System.Console.Write( "Please enter the output file name: " );
            //string outputFile = System.Console.ReadLine();
            string outputFile = "out.txt";

            switch( BasicCheck( inputFile ) )
            {
                case FileType.None:
                    System.Console.WriteLine( "Neither format could be recognized in the provided file." );
                    break;
                case FileType.Json:
                {
                    //will throw exception if file can't be read
                    using( System.IO.StreamReader reader = new System.IO.StreamReader( inputFile ) )
                    {
                        try
                        {
                            string wholeFile = reader.ReadToEnd();
                            XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode( wholeFile );
                            doc.Save( outputFile );
                        }
                        //JSON.NET doesn't properly document thrown exceptions so we'll catch the base class exception 
                        //instead of the more specialized JsonReaderException, JsonSerializationException, JsonWriterException
                        //JsonSchemaException
                        catch( JsonException e )
                        {
                            System.Console.WriteLine( "The file was detected as JSON but it has incorrect format."
                                + " The following exception occured while parsing: \"" + e.Message + "\"" );
                        }
                        //according to MSDN documentation, this exception would be thrown if 
                        //"The operation would not result in a well formed XML document(for example, no document element or duplicate XML declarations)."
                        catch( XmlException e )
                        {
                            System.Console.WriteLine( "The file was detected as JSON and successfully parsed but converting"
                                + " it to XML failed with the following exception: \"" + e.Message );
                        }
                    }
                    break;
                }
                    
                case FileType.Xml:
                    {
                        try
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.Load( inputFile );

                            //there is no JsonConvert.SerializeXmlNode to a stream, so will have 
                            //to take returned string and write it to the file ourselves.

                            string jsonText = JsonConvert.SerializeXmlNode( doc );

                            using( System.IO.StreamWriter writer = new System.IO.StreamWriter( outputFile ) )
                            {
                                //documentation says it doesn't do any \n to \r\n conversation, so the string will be written unmodified
                                writer.Write( jsonText );
                            }
                        }
                        catch( XmlException e )
                        {
                            System.Console.WriteLine( "The file format was detected as XML but parsing it failed with exception: "
                                + "\"" + e.Message + "\"" );
                        }
                        catch( JsonException e )
                        {
                            System.Console.WriteLine( "The file format was detected as XML successfully parsed" +
                                " but serializing it to JSON failed with exception: \"" + e.Message + "\"" );
                        }
                        catch( SystemException e )
                        {
                            System.Console.WriteLine( "The file format was detected as XML successfully converted" +
                                " to JSON but writing it to file \"" + outputFile + 
    
                                "\"failed with exception: \"" + e.Message + "\"" );
                        }
                        break;
                    }
                    default:
                        System.Diagnostics.Debug.Assert( false );
                        break;
                    }

            /*
            FileType type = BasicCheck( fileName );

            XmlDocument doc = new XmlDocument();
            doc.Load( fileName );

            string jsonText = JsonConvert.SerializeXmlNode( doc );



            //back
            //XmlDocument doc2 = (XmlDocument)JsonConvert.DeserializeXmlNode( jsonText );
            XmlDocument doc2 = (XmlDocument)JsonConvert.DeserializeXmlNode( "invalid json" );
            doc.Save( "aaXmlOut.txt" );
            */

        }

        enum FileType { None, Json, Xml };
        //StreamReader constructor will throw exception if the file can't be open, and 
        //that bubble all the way up and stop the program
        static FileType BasicCheck( string fileName )
        {
            //check the first non-space character to be either '<' for Xml or '{' or [' for JSon

            using( System.IO.StreamReader reader = new System.IO.StreamReader( fileName ) )
            {
                while( !reader.EndOfStream )
                {
                    string line = reader.ReadLine();
                    if( !string.IsNullOrWhiteSpace( line ) )
                    {
                        //null param trims all whitespace
                        char first = line.TrimStart( null )[ 0 ];
                        if( first == '<' )
                            return FileType.Xml;
                        if( first == '{' || first == '[' )
                            return FileType.Json;
                    }
                } //end while
            }//end using
            return FileType.None;
        } //end BasicCheck


    }//end Program class


}