using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace ONWLibrary
{
	public static class Extentions
	{
		public static string UPCDecode(this Bitmap bm)
		{
			string qrcode = null;

			Result r = null;

			IBarcodeReader reader = new BarcodeReader();
			reader.Options.TryHarder = true;
			reader.Options.PossibleFormats = new List<BarcodeFormat>
			{
				BarcodeFormat.UPC_A
			};
			int dpi = (int)bm.HorizontalResolution;
			int originalDpi = dpi;
			try
			{
				{
					{
						r = reader.Decode(bm);
					}
					if (r != null && r.BarcodeFormat == BarcodeFormat.UPC_A)
					{
						qrcode = r.Text;
					}
				}
			}
			catch (Exception )
			{
			}

			return qrcode;
		}

		public static string UPCDecode2(Bitmap bm)
		{
			return bm.UPCDecode();
		}
	}

	public class PDFDocument
	{
		public string FileName { get; set; }
		~PDFDocument()
		{
			Close();
		}

		public string UPCDecode()
		{
			return GetImage(0).UPCDecode();
		}
	

	public static void AddImage(Stream inputPdfStream, Stream outputPdfStream, Stream inputImageStream)
		{
			PdfReader reader = new PdfReader(inputPdfStream);
			iTextSharp.text.Rectangle size = reader.GetPageSize(1);

			iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(inputImageStream);
			image.SetAbsolutePosition(size.Width - 98, size.Height - 98);

			PdfStamper stamper = new PdfStamper(reader, outputPdfStream);

			int page = 1;

			{

				PdfContentByte pdfContentByte = stamper.GetOverContent(page);

				pdfContentByte.AddImage(image);
			}
			stamper.Close();
		}

		public void Open(string fileName)
		{
			reader = new PdfReader(fileName);
			parser = new PdfReaderContentParser(reader);
			listener = new MyImageRenderListener();

		}

		public int Count { get { return reader.NumberOfPages; } }

		public Bitmap GetImage(int index)
		{
			parser.ProcessContent(index + 1, listener);
			return listener.Image;
		}

		public Bitmap GetImage2(int index)
		{
			PdfDictionary page = reader.GetPageN(index + 1);
			return GetImagesFromPdfDict(page);
		}

		private PdfReaderContentParser parser = null;
		private MyImageRenderListener listener = null;

		private PdfReader reader = null;

		//private Bitmap parseDict(PdfDictionary dict)
		//{

		//}

		private Bitmap GetImagesFromPdfDict(PdfDictionary dict)
		{
			PdfDictionary res = (PdfDictionary)(PdfReader.GetPdfObject(dict.Get(PdfName.RESOURCES)));
			PdfDictionary xobj = (PdfDictionary)(PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT)));
			Bitmap bm = null;
			if (xobj != null)
			{
				foreach (PdfName name in xobj.Keys)
				{
					PdfObject obj = xobj.Get(name);
					if (obj.IsIndirect())
					{
						PdfDictionary tg = (PdfDictionary)(PdfReader.GetPdfObject(obj));
						PdfName subtype = (PdfName)(PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE)));
						if (PdfName.IMAGE.Equals(subtype))
						{
							int xrefIdx = ((PRIndirectReference)obj).Number;
							PdfObject pdfObj = reader.GetPdfObject(xrefIdx);
							PRStream str = (PRStream)(pdfObj);

							PdfArray decode = tg.GetAsArray(PdfName.DECODE);
							int width = tg.GetAsNumber(PdfName.WIDTH).IntValue;
							int height = tg.GetAsNumber(PdfName.HEIGHT).IntValue;
							int bpc = tg.GetAsNumber(PdfName.BITSPERCOMPONENT).IntValue;
							PdfObject filter = tg.Get(PdfName.FILTER);

							if (filter.Equals(PdfName.FLATEDECODE))
							{
								byte[] imageBytes = PdfReader.GetStreamBytesRaw(str);

								byte[] decodedBytes = PdfReader.FlateDecode(imageBytes); //decode the raw image
								byte[] streamBytes = PdfReader.DecodePredictor(decodedBytes, str.GetAsDict(PdfName.DECODEPARMS)); //decode predict to filter the bytes
								PixelFormat pixelFormat = PixelFormat.Format1bppIndexed;
								switch (bpc) //determine the BPC
								{
									case 1:
										pixelFormat = PixelFormat.Format1bppIndexed;
										break;
									case 8:
										pixelFormat = PixelFormat.Format8bppIndexed;
										break;
									case 24:
										pixelFormat = PixelFormat.Format24bppRgb;
										break;
								}

								bm = new Bitmap(width, height, pixelFormat);
								{
									BitmapData bmpData = bm.LockBits(new System.Drawing.Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, pixelFormat);
									int length = (int)Math.Ceiling(width * bpc / 8.0);
									for (int i = 0; i < height; i++)
									{
										int offset = i * length;
										int scanOffset = i * bmpData.Stride;
										Marshal.Copy(streamBytes, offset, new IntPtr(bmpData.Scan0.ToInt32() + scanOffset), length);
									}
									bm.UnlockBits(bmpData);
								}
							}
							else
							{
								iTextSharp.text.pdf.parser.PdfImageObject pdfImage = new iTextSharp.text.pdf.parser.PdfImageObject(str);

								bm = (System.Drawing.Bitmap)pdfImage.GetDrawingImage();
							}
							int yDPI = bm.Height / 11;
							int xDPI = (bm.Width * 2) / 17;

							xDPI = Math.Abs(xDPI - 300) < 10 ? 300 : xDPI;
							yDPI = Math.Abs(yDPI - 300) < 10 ? 300 : yDPI;
							xDPI = Math.Abs(xDPI - 600) < 10 ? 600 : xDPI;
							yDPI = Math.Abs(yDPI - 600) < 10 ? 600 : yDPI;

							if (xDPI == yDPI)
							{
								bm.SetResolution(xDPI, yDPI);
							}
							else
							{

							}
							break;
						}
						else if (PdfName.FORM.Equals(subtype) || PdfName.GROUP.Equals(subtype))
						{
							GetImagesFromPdfDict(tg);
						}
					}
				}
			}
			return bm;
		}

		public void Split(string fileName)
		{
			throw new NotImplementedException();
		}

		public void Save(System.Drawing.Bitmap bm, string filename)
		{
			Save(bm, filename, RotateFlipType.RotateNoneFlipNone);
		}

		const float PAGE_LEFT_MARGIN = 0;
		const float PAGE_RIGHT_MARGIN = 0;
		const float PAGE_TOP_MARGIN = 0;
		const float PAGE_BOTTOM_MARGIN = 0;

		public void Save(System.Drawing.Bitmap bm, string filename, System.Drawing.RotateFlipType rotate)
		{
			Bitmap image = bm;

			if (rotate != RotateFlipType.RotateNoneFlipNone)
			{
				image.RotateFlip(rotate);
			}

			using (FileStream stream = new FileStream(filename, FileMode.Create))
			{
				using (iTextSharp.text.Document pdfDocument = new iTextSharp.text.Document(PageSize.LETTER, PAGE_LEFT_MARGIN, PAGE_RIGHT_MARGIN, PAGE_TOP_MARGIN, PAGE_BOTTOM_MARGIN))
				{
					iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(pdfDocument, stream);
					pdfDocument.Open();

					MemoryStream ms = new MemoryStream();
					image.Save(ms, System.Drawing.Imaging.ImageFormat.Tiff);
					iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(ms);
					img.ScaleToFit(PageSize.LETTER.Width - (PAGE_LEFT_MARGIN + PAGE_RIGHT_MARGIN), PageSize.LETTER.Height - (PAGE_TOP_MARGIN + PAGE_BOTTOM_MARGIN));
					pdfDocument.Add(img);

					pdfDocument.Close();
					writer.Close();
				}
			}
		}

		public void Add(System.Drawing.Bitmap bm)
		{
			Add(bm, RotateFlipType.RotateNoneFlipNone);
		}

		FileStream stream;
		iTextSharp.text.Document pdfDocument;
		iTextSharp.text.pdf.PdfWriter writer;

		public void Add(System.Drawing.Bitmap bm, System.Drawing.RotateFlipType rotate)
		{
			if (stream == null)
			{
				stream = new FileStream(FileName, FileMode.Create);
				pdfDocument = new iTextSharp.text.Document(PageSize.LETTER, PAGE_LEFT_MARGIN, PAGE_RIGHT_MARGIN, PAGE_TOP_MARGIN, PAGE_BOTTOM_MARGIN);
				writer = iTextSharp.text.pdf.PdfWriter.GetInstance(pdfDocument, stream);
				pdfDocument.Open();
			}

			Bitmap image = bm;

			if (rotate != RotateFlipType.RotateNoneFlipNone)
			{
				image.RotateFlip(rotate);
			}

			{
				iTextSharp.text.Image img = GetImage(image);
				img.ScaleToFit(PageSize.LETTER.Width - (PAGE_LEFT_MARGIN + PAGE_RIGHT_MARGIN), PageSize.LETTER.Height - (PAGE_TOP_MARGIN + PAGE_BOTTOM_MARGIN));
				pdfDocument.Add(img);
			}
		}

		private iTextSharp.text.Image GetImage(Bitmap image)
		{
			if (image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format1bppIndexed)
			{
				int w = image.Width;
				int h = image.Height;

				System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);
				System.Drawing.Imaging.BitmapData bmpData =
					image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
					image.PixelFormat);

				// Get the address of the first line.
				IntPtr ptr = bmpData.Scan0;
				int bytes = Math.Abs(bmpData.Stride) * image.Height;
				byte[] rgbValues = new byte[bytes];

				int byteWidth = (w / 8) + ((w & 7) != 0 ? 1 : 0);
				byte[] pixelsByte = new byte[byteWidth * h];

				// Copy the RGB values into the array.
				for (int a = 0; a < h; a++)
				{
					System.Runtime.InteropServices.Marshal.Copy(ptr, pixelsByte, a * byteWidth, byteWidth);
					ptr = ptr + bmpData.Stride;
				}
				image.UnlockBits(bmpData);

				return iTextSharp.text.Image.GetInstance(w, h, 1, 1, pixelsByte, null);
			}

			else
			{
				return iTextSharp.text.Image.GetInstance(image, null, true);
			}
		}
		public void Close()
		{
			if (pdfDocument != null)
			{
				pdfDocument.Close();
				pdfDocument.Dispose();
				writer.Close();
				writer.Dispose();
				stream.Close();
				stream.Dispose();
				pdfDocument = null;
				writer = null;
				pdfDocument = null;
			}

			if (reader != null)
			{
				reader.Close();
			}

		}



		public void Tag(string oldFile, Stream fs, string text)
		{
			float x;
			float y;

			// open the reader
			PdfReader reader = new PdfReader(oldFile);
			iTextSharp.text.Rectangle size = reader.GetPageSizeWithRotation(1);
			float height = 60;
			float width = 150;
			x = size.Width - 40 - width;
			y = size.Height - height;
			Document document = new Document(size);

			// open the writer
			PdfWriter writer = PdfWriter.GetInstance(document, fs);
			document.Open();

			// the pdf content
			PdfContentByte cb = writer.DirectContent;

			// create the new page and add it to the pdf
			PdfImportedPage page = writer.GetImportedPage(reader, 1);
			cb.AddTemplate(page, 0, 0);

			cb.Rectangle(x, y, width, height);
			cb.SetColorFill(BaseColor.WHITE);
			cb.Fill();
			// close the streams and voilá the file should be changed :)


			// write the text in the pdf content
			cb.BeginText();
			// select the font properties
			BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
			cb.SetColorFill(BaseColor.BLACK);
			cb.SetFontAndSize(bf, 20);
			// put the alignment and coordinates here
			cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, text, x + 5, y + 10, 0);
			cb.EndText();

			writer.Flush();
			document.Close();
			reader.Close();
			//			fs.Seek(0, SeekOrigin.Begin);
		}
	}

	public class MyImageRenderListener : IRenderListener
	{
		public void RenderText(TextRenderInfo renderInfo) { }
		public void BeginTextBlock() { }
		public void EndTextBlock() { }

		public Bitmap Image = null;
		public void RenderImage(ImageRenderInfo renderInfo)
		{
			try
			{
				PdfImageObject image = renderInfo.GetImage();
				if (image == null)
				{
					return;
				}

				using (MemoryStream ms = new MemoryStream(image.GetImageAsBytes()))
				{
					Bitmap i = (System.Drawing.Bitmap)Bitmap.FromStream(ms);
					Image = (System.Drawing.Bitmap)i.Clone();
					i.Dispose();
					//                    int dpi = i.Height / 11;
					int yDPI = Image.Height / 11;
					int xDPI = (Image.Width * 2) / 17;

					xDPI = Math.Abs(xDPI - 300) < 10 ? 300 : xDPI;
					yDPI = Math.Abs(yDPI - 300) < 10 ? 300 : yDPI;
					xDPI = Math.Abs(xDPI - 600) < 10 ? 600 : xDPI;
					yDPI = Math.Abs(yDPI - 600) < 10 ? 600 : yDPI;

					if (xDPI == yDPI)
					{
						Image.SetResolution(xDPI, yDPI);
					}
					else
					{

					}
				}
			}
			catch (IOException)
			{
				/*
                 * pass-through; image type not supported by iText[Sharp]; e.g. jbig2
                */
			}
		}
	}

}
