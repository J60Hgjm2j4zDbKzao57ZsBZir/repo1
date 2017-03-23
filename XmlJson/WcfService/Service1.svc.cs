using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

using System.IO;

using DocConversion;

namespace WcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1: IService1
    {
        //returns the contents of the document or error
        public string Convert( string content )
        {
            //assuming it comes in UTF8
            MemoryStream inputStream = new MemoryStream( 
                Encoding.UTF8.GetBytes( content ), false ); //non-writable

            bool b = inputStream.CanSeek;

            MemoryStream outputStream = new MemoryStream();
            ConvertDocument convert = new ConvertDocument();
            string error = convert.DoConvertDocument( inputStream, outputStream );
            if( error == "" ) //success
            {
                return Encoding.UTF8.GetString( outputStream.ToArray() );
            }
            return error;
        }
    }
}
