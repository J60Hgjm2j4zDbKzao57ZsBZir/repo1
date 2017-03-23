using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;

using System.IO;

using DocConversion;

namespace Console
{
    public class Program
    {
        
        static void Main( string[] args )
        {
            System.Console.Write( "Please enter the input file name: " );
            string inputFile = System.Console.ReadLine();
            //string inputFile = "aa1.txt";

            System.Console.Write( "Please enter the output file name: " );
            string outputFile = System.Console.ReadLine();
            //string outputFile = "out.txt";

            FileStream inputStream = new FileStream(
                inputFile, FileMode.Open ); //non-writable

            FileStream outputStream = new FileStream( outputFile, FileMode.Create );
            ConvertDocument convert = new ConvertDocument();
            string error = convert.DoConvertDocument( inputStream, outputStream );

            if( error == "" ) //success
            {
                System.Console.WriteLine( "Conversion successful." );
            }
            else
            {
                System.Console.WriteLine( error );
            }
        }

    }
}