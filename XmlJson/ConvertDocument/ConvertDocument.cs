using System;
using System.IO;
using System.Xml;

using Newtonsoft.Json;

namespace DocConversion
{
    public class ConvertDocument
    {
        //returning empty string in case of success, or error message otherwise
        public string DoConvertDocument( Stream inputStream, Stream outputStream )
        {
            string error = "";
            switch( GetContentType( inputStream ) )
            {
                case ContentType.None:
                    error = "Neither format could be recognized in the provided file.";
                    break;
                case ContentType.Json:
                {
                    try
                    {
                        StreamReader reader = new StreamReader( inputStream );
                        string content = reader.ReadToEnd();
                        XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode( content );
                        doc.Save( outputStream );
                    }
                    //JSON.NET doesn't properly document thrown exceptions so we'll catch the base class exception 
                    //instead of the more specialized JsonReaderException, JsonSerializationException, JsonWriterException
                    //JsonSchemaException
                    catch( JsonException e )
                    {
                        error = "The file was detected as JSON but it has incorrect format."
                            + " The following exception occured while parsing: \"" + e.Message + "\"";
                    }
                    //according to MSDN documentation, this exception would be thrown if 
                    //"The operation would not result in a well formed XML document(for example, no document element or duplicate XML declarations)."
                    catch( XmlException e )
                    {
                        error = "The file was detected as JSON and successfully parsed but serializing"
                            + " it to XML failed with the following exception: \"" + e.Message;
                    }
                    break;
                }

                case ContentType.Xml:
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load( inputStream );

                        //there is no JsonConvert.SerializeXmlNode to a stream, so will have 
                        //to take returned string and write it to the file ourselves.
                        string jsonText = JsonConvert.SerializeXmlNode( doc );

                        StreamWriter writer = new StreamWriter( outputStream );
                        writer.Write( jsonText );
                    }
                    catch( XmlException e )
                    {
                        error = "The file format was detected as XML but parsing it failed with exception: "
                            + "\"" + e.Message + "\"";
                    }
                    catch( JsonException e )
                    {
                        error = "The file format was detected as XML successfully parsed" +
                            " but serializing it to JSON failed with exception: \"" + e.Message + "\"";
                    }
                    catch( SystemException e )
                    {
                        error = "The file format was detected as XML successfully converted" +
                            " to JSON but writing it to the output stream" +
                            " failed with exception: \"" + e.Message + "\"";
                    }
                    break;
                }
                default:
                    System.Diagnostics.Debug.Assert( false );
                    error = "There was a failure on the server, we will take a look at it ASAP";
                    break;
            }
            return error;
        }

        private enum ContentType { None, Json, Xml };
        //StreamReader constructor will throw exception if the file can't be open, and 
        //that bubble all the way up and stop the program
        private ContentType GetContentType( Stream inputStream )
        {
            ContentType type = ContentType.None;
            //check the first non-space character to be either '<' for Xml or '{' or [' for JSon
            System.IO.StreamReader reader = new System.IO.StreamReader( inputStream );
            
            while( !reader.EndOfStream )
            {
                string line = reader.ReadLine();
                if( !string.IsNullOrWhiteSpace( line ) )
                {
                    //null param trims all whitespace
                    char first = line.TrimStart( null )[ 0 ];
                    if( first == '<' )
                    {
                        type = ContentType.Xml;
                    }
                    else if( first == '{' || first == '[' )
                    {
                        type = ContentType.Json;
                    }
                    break;
                }
            } //end while
            inputStream.Position = 0;
            return type;
        }
    }
}
