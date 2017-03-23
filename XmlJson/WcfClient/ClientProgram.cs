
using System.IO;

namespace WcfService
{
    public class ClientProgram
    {
        static void Main( string[] args )
        {
            System.Console.Write( "Please enter the input file name: " );
            string inputFile = System.Console.ReadLine();
            //string inputFile = "aa.txt";

            System.Console.Write( "Please enter the output file name: " );
            string outputFile = System.Console.ReadLine();
            //string outputFile = "out.txt";

            using( System.IO.StreamReader reader = new System.IO.StreamReader( inputFile ) )
            {
                string wholeFile = reader.ReadToEnd();
                Service1Client client = new Service1Client();
                string result = client.Convert( wholeFile );
                using( System.IO.StreamWriter writer = new System.IO.StreamWriter( outputFile ) )
                {
                    //documentation says it doesn't do any \n to \r\n conversation, so the string will be written unmodified
                    writer.Write( result );
                }
            }
        }
    }
}