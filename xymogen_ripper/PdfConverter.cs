using System;
using System.Drawing;
using foxit.pdf;
using foxit.common.fxcrt;

namespace xymogen_ripper
{
    public class PdfConverter : IDisposable
	{
		public bool Initialize()
		{
			string sn = "XO2bTnKD7mk7ocivflLYfMRd7Qo3F2Zrsd4MrR7Rn0Nf9e0scxuR3Q==";
			string key = "8f0YFcONvRkN+ldwlpAFW0NF9QtjOhOBvj1eVQBq2RrKr+Xx9JCMyffRyr9miPWWZtsw0GDcbFCtsYwg9N46zzhFD4tH3TGe6JmEFuRvgiZ80ENo61EN+pIezaoNOTtQCuXU+UaXH6KkKckW7RN+NT4PU7Zn2eN03C/FVAPpAgUUQEaG0UNVOi6MtPSQjd2+ESr1vThJaTKxBmn0wsVmOs1l9S+92sg3R+lc1r8vIiISWv1lf3zT09glb3v5Jax4B3kEZ03/HvWa3+oYSoIsXrg4dKZElY/U1LkRhxhnOe6Er1wpNIpgNCpANvkU8SGMDv3Ija3xw6ng8JeNwKlgJwtzNdGHoCDbihfLBLRLhYwInxAnpbKm1kMyRduOETrhhRfN7mVxmWNfGPOoPxrc+JLiwuFfZPqjYrxW9g2hQicS+1WA1o9nRH7Yn+yN9McsiZbPDbnlS7XMjgVMT659J07nicrTVW5+ZPG4WPtpqY4s5xeBJfCakBhDRfDci2W1unD5hk++1y1z3DlFm3O734kmlJq8L6NaYPgVfHrtyfJ16NOwddUfSFFtarl1wQjOMrkdgKrnFJqo91RgQJflWk5HW4UwXzJ9kAkyZg5mDzIG+8iWNCaCjQtkAt2bhj947a1XZgFrt7YMMUW6uqoq4Jey4h4GgyAL3oCtq6AlTP6ZJLj1lJowQdOeyStoWji5cOMec60PrBR3lIm1x9ZQsny/Hd8d38TkqXI3wAWMH8a1hdq9iFkCRkVaD5TI2/x3TJ0lo5ymCwYz3hQkXpyfj68KKRMISD73LyFymipd8J8CtRaUYfulS0nJmm1kRi9+QksP3zy/mW+M2Zzd0A45vRKIYh5eeBNMHW0hbQCVZZxi2vB1ImjnXTvDRTdJ7XWimF6qKQVSO12rEE8tVW2R+guWnZTnD6JfGvlLH6hkVLDx4nN5Cb5uo9MAauUKZFWUATj63PZV+CqVAA4A0rAyrEqk4ZYJYpCDdVhatnqWXVduyiCkz5urKJqLm+lp5xqGkGCPMbYHk7H1xkr70T9N3s4IosHlwDKkUj6n7k1IIsAJzfeqnJ+zm85zJwKRM1noYQUl+4IJWKMHj6sNjsY5B/gmCzBvZj/6Ey2wt2u2B3zGxaaa9tGtvVFhAEx6LItE5w8UJrF/b06BmKhin7iEhSLpYAgL4flp9wl//5rI8NcoBw5ta3j2NyzUEGCuqhyhbfY2AOW4SgQfPifO4CvyDX5jlOFzskZ3aL1XMlReVIsSLsLXxYf3";
			foxit.common.ErrorCode error_code = foxit.common.Library.Initialize(sn, key);
			if (error_code != foxit.common.ErrorCode.e_ErrSuccess)
			{
				Console.WriteLine("Library Initialize Error: {0}\n", error_code);
				return false;
			}
			return true;
		}

		public Bitmap ToBitmap(string pdfFile)
		{
			Bitmap bitmap = null;

			using (PDFDoc doc = new PDFDoc(pdfFile))
			{
				foxit.common.ErrorCode error_code = doc.Load(null);
				if (error_code == foxit.common.ErrorCode.e_ErrSuccess)
				{
					//	Console.WriteLine("The PDFDoc [{0}] Error: {1}\n", pdfFile, error_code);
					//}
					//else
					//{

					int nPageCount = doc.GetPageCount();

					for (int i = 0; i < nPageCount; i++)
					{
						using (PDFPage page = doc.GetPage(i))
						{
							// Parse page.
							page.StartParse((int)foxit.pdf.PDFPage.ParseFlags.e_ParsePageNormal, null, false);

							int width = (int)(page.GetWidth()) * 600 / 96;
							int height = (int)(page.GetHeight()) * 600 / 96;
							Matrix2D matrix = page.GetDisplayMatrix(0, 0, width, height, page.GetRotation());

							// Prepare a bitmap for rendering.
							bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
							bitmap.SetResolution((float)600, (float)600);
							using (Graphics draw = Graphics.FromImage(bitmap))
							{
								draw.Clear(Color.White);

								// Render page
								foxit.common.Renderer render = new foxit.common.Renderer(bitmap, false);
								render.StartRender(page, matrix, null);
							}
						}
					}
				}
			}
			return bitmap;
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
				}

				foxit.common.Library.Release();
				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~PdfConverter() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion
	}




}
