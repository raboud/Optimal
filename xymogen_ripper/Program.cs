using ShopifySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using foxit.pdf;
using foxit.common.fxcrt;
using Newtonsoft.Json;
using ARSoft.Tools.Net.Dns;
using System.Globalization;
using ONWLibrary;

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

	public class InvSupplement
	{
		public string OptimalNutritionName { get; set; }
		public string Supplement { get; set; }
		public string Name { get; set; }
		public string Dosage { get; set; }
		public string Size { get; set; }
		public string Flavor { get; set; }
		public string Company { get; set; }
		public bool Active { get; set; }
		public decimal? Cost { get; set; }
		public decimal? Retail { get; set; }
		public string url { get; set; }
		public int XymogenId { get; set; }
		public string productNumber { get; set; }
		public bool Imported { get; set; }
		public string ImageUrl { get; set; }
	}

	public partial class Patient
	{
		[JsonProperty("Patient No")]
		public long PatientNo { get; set; }

		[JsonProperty("Last Name")]
		public string LastName { get; set; }

		[JsonProperty("First Name")]
		public string FirstName { get; set; }

		[JsonProperty("Address1", NullValueHandling = NullValueHandling.Ignore)]
		public string Address1 { get; set; }

		[JsonProperty("Address2", NullValueHandling = NullValueHandling.Ignore)]
		public string Address2 { get; set; }

		[JsonProperty("City", NullValueHandling = NullValueHandling.Ignore)]
		public string City { get; set; }

		[JsonProperty("State", NullValueHandling = NullValueHandling.Ignore)]
		public string State { get; set; }

		[JsonProperty("Zip", NullValueHandling = NullValueHandling.Ignore)]
		public string Zip { get; set; }

		[JsonProperty("Home Phone", NullValueHandling = NullValueHandling.Ignore)]
		public string HomePhone { get; set; }

		[JsonProperty("Cell Phone", NullValueHandling = NullValueHandling.Ignore)]
		public string CellPhone { get; set; }

		[JsonProperty("EMail", NullValueHandling = NullValueHandling.Ignore)]
		public string EMail { get; set; }

		[JsonProperty("Other Phone", NullValueHandling = NullValueHandling.Ignore)]
		public string OtherPhone { get; set; }

		[JsonProperty("Work Phone", NullValueHandling = NullValueHandling.Ignore)]
		public string WorkPhone { get; set; }
	}

	class Program
	{
		static void Main(string[] args)
		{
			Shopify.Init();
			List<Customer> c = Shopify.GetCustomers();
			List<Order> l = Shopify.GetOrders();

			string start = JsonConvert.SerializeObject(l, Formatting.Indented,
				new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

			Customer c1 = c.FirstOrDefault(cu => cu.FirstName == "Darren" && cu.LastName == "Fink");
			Order o1 = l.FirstOrDefault(o => o.Name == "ONW-1006");
			Order o2 = l.FirstOrDefault(o => o.Name == "ONW-1007");
			Order o3 = l.FirstOrDefault(o => o.Name == "ONW-1008");

			if (o2 != null && o1 != null && o3 != null && c1 != null)
			{
				c1.OrdersCount++;
				
				o1.Note = "";
				o1.Open();
				o1.Customer = c1;
				o1.Update();
				o1.Close();

				o2.Note = "";
				o2.Open();
				o2.Customer = c1;
				o2.Update();
				o2.Close();

				o3.Note = "";
				o3.Open();
				o3.Customer = c1;
				o3.Update();
				o3.Close();

				string end = JsonConvert.SerializeObject(l, Formatting.Indented,
					new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
				Order o4 = Shopify.Update(o1);
				Order o5 = Shopify.Update(o2);
				Order o6 = Shopify.Update(o3);
				Customer c2 = Shopify.UpdateCustomer(c1);
			}


			//			MasterScraper scraper = new MasterScraper();
			//			scraper.flatProducts();
			//scraper.Supplement();
			//scraper.Cleanup();
			//scraper.Supplement2();

			//List<OthoScraper.OrthoProduct>prods = scraper.GetProducts();
			//foreach (var prod in prods)
			//{
			//	foreach (var variant in prod.Variants)
			//	{
			//		if (variant.ProductNumber != null)
			//		{
			//			if (string.IsNullOrEmpty(variant.ImageUrl))
			//			{
			//				continue;
			//			}
			//			scraper.GetImage(variant.ImageUrl, variant.ProductNumber);

			//		}
			//	}

			//}



			//			BuildProduct(924);
			//			Supplement();
			//Supplement2();
		}

		private static void Cleanup()
		{
			IEnumerable<Customer> customers = Shopify.GetCustomers();
			int count = 0;
			foreach (Customer c in customers)
			{
				System.Console.Write($"{++count}\r");
				Address add = c.Addresses.FirstOrDefault();
				if (add?.City != null)
				{
					string city = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(add.City.ToLower());
					if (city != add.City)
					{
						System.Console.WriteLine($"\r{add.City} -  {city}");
						add.City = city;
						Shopify.UpdateCustomer(c);
						System.Threading.Thread.Sleep(500);
					}
				}
				//				string first = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(c.FirstName);
			}
		}

		private static void PopulatePatients()
		{
			List<Patient> patients = JsonConvert.DeserializeObject<List<Patient>>(File.ReadAllText(@"C:\Users\Robert\Documents\patient.json"));
			List<Patient> success = new List<Patient>();
			List<Patient> errors = new List<Patient>();
			List<Patient> skipped = new List<Patient>();
			int count = 0;

			List<string> emails = new List<string>();
			List<string> phones = new List<string>();
			IEnumerable<Customer> customers = Shopify.GetCustomers();
			foreach (Customer c in customers)
			{
				if (!string.IsNullOrEmpty(c.Email))
				{
					emails.Add(c.Email);
				}

				if (!string.IsNullOrEmpty(c.Phone))
				{
					phones.Add(c.Phone);
				}
			}


			Patient patient;
			while ((patient = patients.FirstOrDefault()) != null)
			{
				patients.Remove(patient);
				File.WriteAllText(@"C:\Users\Robert\Documents\patient.json", JsonConvert.SerializeObject(patients));
				//				patient.EMail = isValidEmail(patient.EMail) ? patient.EMail : null;
				if (emails.Contains(patient.EMail))
				{
					skipped.Add(patient);
					File.WriteAllText(@"C:\Users\Robert\Documents\skipped.json", JsonConvert.SerializeObject(skipped));
					System.Console.WriteLine($"\rSkipping Patient {patient.PatientNo} duplicate email");
					continue;
				}
				if (!string.IsNullOrEmpty(patient.EMail))
				{
					emails.Add(patient.EMail);
				}
				else
				{

				}
				ShopifySharp.Address address = new Address();
				address.Address1 = patient.Address1;
				address.Address2 = patient.Address2;
				address.City = patient.City;
				address.Default = true;
				address.Zip = patient.Zip;
				address.CountryCode = "US";
				address.ProvinceCode = patient.State;

				ShopifySharp.Customer customer = new ShopifySharp.Customer();
				customer.Addresses = new List<Address>() { address };
				customer.Email = patient.EMail;
				customer.FirstName = patient.FirstName;
				customer.LastName = patient.LastName;
				if (patient.CellPhone != null && IsValidUSPhoneNumber(patient.CellPhone))
				{
					customer.Phone = patient.CellPhone;
				}
				else if (patient.HomePhone != null && IsValidUSPhoneNumber(patient.HomePhone))
				{
					customer.Phone = patient.HomePhone;
				}
				else if (patient.WorkPhone != null && IsValidUSPhoneNumber(patient.WorkPhone))
				{
					customer.Phone = patient.WorkPhone;
				}
				else if (patient.OtherPhone != null && IsValidUSPhoneNumber(patient.OtherPhone))
				{
					customer.Phone = patient.OtherPhone;
				}
				if (!string.IsNullOrEmpty(customer.Phone))
				{
					if (phones.Contains(customer.Phone))
					{
						skipped.Add(patient);
						File.WriteAllText(@"C:\Users\Robert\Documents\skipped.json", JsonConvert.SerializeObject(skipped));
						System.Console.WriteLine($"\rSkipping Patient {patient.PatientNo} duplicate phone");
						continue;
					}
					phones.Add(customer.Phone);
				}

				try
				{
					Customer c = Shopify.CreateCustomer(customer);
					bool b = Shopify.CreateCustomerMetadata(c.Id.Value, "ownCustomer", "PatientNumber", patient.PatientNo);
					success.Add(patient);
					File.WriteAllText(@"C:\Users\Robert\Documents\success.json", JsonConvert.SerializeObject(success));
				}
				catch (Exception)
				{
					errors.Add(patient);
					File.WriteAllText(@"C:\Users\Robert\Documents\errors.json", JsonConvert.SerializeObject(errors));

				}
				count++;
				System.Console.Write($"{count}\r");
				System.Threading.Thread.Sleep(1100);
			}
		}

		static List<string> invalidDomains = new List<string>();
		static List<string> validDomains = new List<string>();

		public static bool isValidEmail(string email)
		{
			bool retVal = false;
			if (!string.IsNullOrEmpty(email))
			{
				int index = email.IndexOf('@');
				if (index > 0)
				{
					string domain = email.Substring(index + 1);

					if (validDomains.Contains(domain))
					{
						return true;
					}
					if (invalidDomains.Contains(domain))
					{
						return false;
					}

					DnsStubResolver resolver = new DnsStubResolver();
					List<MxRecord> records = resolver.Resolve<MxRecord>(domain, RecordType.Mx);
					retVal = records.Count > 0;
					if (retVal)
					{
						validDomains.Add(domain);
					}
					else
					{
						invalidDomains.Add(domain);
						Console.WriteLine($"Invalid email {email}");
					}

				}
			}
			return retVal;
		}

		public static bool IsValidUSPhoneNumber(string strPhone)
		{
			string regExPattern = @"^([2-9][0-8][0-9])[- .]?\d{3}[- .]?\d{4}$";
			bool b = MatchStringFromRegex(strPhone, regExPattern);
			if (!b)
			{
				System.Console.WriteLine($"\rInvalid Phone Number {strPhone}");
			}
			return b;
		}

		public static bool MatchStringFromRegex(string str, string regexstr)
		{
			str = str.Trim();
			System.Text.RegularExpressions.Regex pattern = new System.Text.RegularExpressions.Regex(regexstr);
			return pattern.IsMatch(str);
		}

		private static void CleanupPatients()
		{
			List<string> lines = File.ReadAllLines(@"C:\Users\Robert\Documents\patient.json").ToList();
			lines.RemoveAll(l => l.Contains("\"ChartNo\":"));
			lines.RemoveAll(l => l.Contains("\"Provider\":"));
			lines.RemoveAll(l => l.Contains("\"Intl Locale\":"));
			lines.RemoveAll(l => l.Contains("\"Intl Postal\":"));
			lines.RemoveAll(l => l.Contains("\"Prim. Insurance\":"));
			lines.RemoveAll(l => l.Contains("\"Sec. Insurance\":"));
			lines.RemoveAll(l => l.Contains("\"Portal Account\":"));
			lines.RemoveAll(l => l.Contains("\"Created\":"));
			lines.RemoveAll(l => l.Contains("\"Modified\":"));
			lines.RemoveAll(l => l.Contains("\"Referral\":"));
			lines.RemoveAll(l => l.Contains("\"P.Ins Policy #\":"));
			lines.RemoveAll(l => l.Contains("\"P.Ins Grp #\":"));
			lines.RemoveAll(l => l.Contains("\"S.Ins Policy #\":"));
			lines.RemoveAll(l => l.Contains("\"S.Ins Grp #\":"));
			lines.RemoveAll(l => l.Contains("\"StudentStatus\":"));
			lines.RemoveAll(l => l.Contains("\"Tert. Insurance\":"));
			lines.RemoveAll(l => l.Contains("\"T.Ins Grp #\":"));
			lines.RemoveAll(l => l.Contains("\"T.Ins Policy #\":"));
			lines.RemoveAll(l => l.Contains("\"PatientGroup\":"));
			lines.RemoveAll(l => l.Contains("\"Ethnicity\":"));
			lines.RemoveAll(l => l.Contains("\"Language\":"));
			lines.RemoveAll(l => l.Contains("\"Race\":"));
			lines.RemoveAll(l => l.Contains("\"Gender\":"));
			lines.RemoveAll(l => l.Contains("\"BirthDate\":"));
			lines.RemoveAll(l => l.Contains("\"MI\":"));
			lines.RemoveAll(l => l.Contains("\": \"\"\"\","));
			lines.RemoveAll(l => l.Contains("\": \"\","));
			lines.RemoveAll(l => l.Contains("\"\"0000000000\"\""));

			int index = 0;
			//while ((index = lines.FindIndex(index, l => l.Contains("\"Zip\":") || l.Contains("\"Home Phone\":") || l.Contains("\"Work Phone\":") || l.Contains("\"Cell Phone\":") || l.Contains("\"Other Phone\":"))) != -1)
			//{
			//	lines[index] = lines[index].Replace("\": \"\"\"", "\": \"");
			//	index++;
			//}

			index = 0;
			//while ((index = lines.FindIndex(index, l => l.Contains("\"Zip\":") || l.Contains("\"Home Phone\":") || l.Contains("\"Work Phone\":") || l.Contains("\"Cell Phone\":") || l.Contains("\"Other Phone\":"))) != -1)
			//{
			//	lines[index] = lines[index].Replace("\": ", "\": \"").Replace(",", "\",");
			//	index++;
			//}


			index = 0;
			while ((index = lines.FindIndex(index, l => l.Contains("}"))) != -1)
			{
				if (lines[index - 1].EndsWith(','))
				{
					lines[index - 1] = lines[index - 1].Substring(0, lines[index - 1].Length - 1);
				}
				index++;
			}
			File.WriteAllLines(@"C:\Users\Robert\Documents\patient.json", lines);
		}


		public static void testShopify()
		{
			//ShopifySharp.CustomCollectionService customCollectionService = new CustomCollectionService("https://optimalnw.myshopify.com", "3912f9da28ad92b17c79bf734b25246f");
			//IEnumerable<CustomCollection> customs = customCollectionService.ListAsync().Result;

			//foreach (var cc in customs.Where(c => c.PublishedScope != "global"))
			//{
			//	cc.PublishedScope = "global";
			//	customCollectionService.UpdateAsync(cc.Id.Value, cc).Wait();
			//}



			//CollectService collectService = new CollectService("https://optimalnw.myshopify.com", "3912f9da28ad92b17c79bf734b25246f");
			//IEnumerable<Collect> collects = collectService.ListAsync().Result;

			//ShopifySharp.MetaFieldService metaFieldService = new ShopifySharp.MetaFieldService("https://optimalnw.myshopify.com", "3912f9da28ad92b17c79bf734b25246f");
			//IEnumerable<MetaField> metas = metaFieldService.ListAsync().Result;

			ShopifySharp.ProductService productService = new ShopifySharp.ProductService("https://optimalnw.myshopify.com", "3912f9da28ad92b17c79bf734b25246f");
			IEnumerable<Product> products = productService.ListAsync().Result;

			//MetaField meta = new MetaField()
			//{
			//	Namespace = "ownProduct",
			//	Key = "shortDescription",
			//	Value = "Convenient Supra-Dose Vitamin D3",
			//	ValueType = "string",
			//	Description = "This is a test meta field. It is an string value."
			//};
			//var t = metaFieldService.CreateAsync(meta, products.First().Id.Value, "products").Result;
			//metas = metaFieldService.ListAsync(products.First().Id.Value, "products").Result;
			//products = productService.ListAsync().Result;
		}

	}




}
