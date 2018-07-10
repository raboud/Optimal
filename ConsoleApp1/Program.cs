using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bytescout.BarCodeReader;

namespace ConsoleApp1
{
	class Program
	{
		const string ImageFile = "f:\\temp\\test.pdf";

		static void Main(string[] args)
		{
			Console.WriteLine("Reading barcode(s) from image {0}", System.IO.Path.GetFullPath(ImageFile));

			Reader reader = new Reader();
			reader.RegistrationName = "demo";
			reader.RegistrationKey = "demo";

			// Set barcode type to find
			reader.BarcodeTypesToFind.All = true;

			// Read barcodes
			FoundBarcode[] barcodes = reader.ReadFrom(ImageFile);

			foreach (FoundBarcode barcode in barcodes)
			{
				Console.WriteLine("Found barcode with type '{0}' and value '{1}'", barcode.Type, barcode.Value);
			}

			Console.WriteLine("Press any key to exit..");
			Console.ReadKey();
		}
	}


}
