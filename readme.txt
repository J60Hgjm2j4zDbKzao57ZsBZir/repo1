There are 4 projects:
- ConvertDocument - a library containing the conversion function
- Console - console application using the ConvertLibrary directly
- WcfService - WCF service calling into ConvertLibrary
- WcfClient -  WCF client calling remotely into WcfService (and not dependent on the ConvertDocument library)

The solution XmlJson contains all projects
The solution WcfServer has just WcfService and ConvertDocument.

How to run:
	- console - run the "Console" project: it will prompt for the names of the input and output file; if conversion is not possible, it will display the error on the console, otherwise will display "success" and write the contents into the output file
	
	- WCF: 
		1. open the WcfServerSolution and do Run (WcfService is set as startup project)
		2. In the XmlJson solution, set the project WcfClient as startup project and run it. The output, whether it's an error message or the converted contents, will be in the output file (nothing is displayed on the console)
		
Notes:
The library itself uses JSON.NET for conversion (I added the dependency in my library ConvertDocument via NuPackage). The conversion funtions that I was able to find in JSON.NET need to have the full contents in RAM, therefore even though ConvertDocument.DoConvertDocument() takes streams as parameters, it will read those streams fully into strings (in a way defeating the purpose of passing streams - I made it this way so that if the implementation is changed to handle streams properly, the signature of the function will stay the same). 
- likewise, the WCF client sends the entire contents as a string to the WCF server, rather than doing any streaming.

If being able to handle harge streams was an important requirement, you can let me know and I'll look at it in more detail. Thanks!
