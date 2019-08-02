using ShopifySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using ARSoft.Tools.Net.Dns;
using System.Globalization;
using ONWLibrary;

namespace xymogen_ripper
{

    class Program
	{
		static void Main(string[] args)
		{
            XymogenScraper.GetProductInfo();

   //         Shopify.Init();
			//List<Customer> c = Shopify.GetCustomers();
			//List<Order> l = Shopify.GetOrdersAsync().Result;

			//string start = JsonConvert.SerializeObject(l, Formatting.Indented,
			//	new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

			//Customer c1 = c.FirstOrDefault(cu => cu.FirstName == "Darren" && cu.LastName == "Fink");
			//Order o1 = l.FirstOrDefault(o => o.Name == "ONW-1006");
			//Order o2 = l.FirstOrDefault(o => o.Name == "ONW-1007");
			//Order o3 = l.FirstOrDefault(o => o.Name == "ONW-1008");

			//if (o2 != null && o1 != null && o3 != null && c1 != null)
			//{
			//	c1.OrdersCount++;
				
			//	o1.Note = "";
			//	o1.OpenAsync().Wait();
			//	o1.Customer = c1;
			//	o1.UpdateAsync().Wait();
			//	o1.CloseAsync().Wait();

			//	o2.Note = "";
			//	o2.OpenAsync().Wait();
			//	o2.Customer = c1;
			//	o2.UpdateAsync().Wait();
			//	o2.CloseAsync().Wait();

			//	o3.Note = "";
			//	o3.OpenAsync().Wait();
			//	o3.Customer = c1;
			//	o3.UpdateAsync().Wait();
			//	o3.CloseAsync().Wait();

			//	string end = JsonConvert.SerializeObject(l, Formatting.Indented,
			//		new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			//	Order o4 = Shopify.UpdateAsync(o1).Result;
			//	Order o5 = Shopify.UpdateAsync(o2).Result;
			//	Order o6 = Shopify.UpdateAsync(o3).Result;
			//	Customer c2 = Shopify.UpdateCustomer(c1);
			//}


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
				ShopifySharp.Address address = new Address
				{
					Address1 = patient.Address1,
					Address2 = patient.Address2,
					City = patient.City,
					Default = true,
					Zip = patient.Zip,
					CountryCode = "US",
					ProvinceCode = patient.State
				};

				ShopifySharp.Customer customer = new ShopifySharp.Customer
				{
					Addresses = new List<Address>() { address },
					Email = patient.EMail,
					FirstName = patient.FirstName,
					LastName = patient.LastName
				};
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

		public static bool IsValidEmail(string email)
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


		public static void TestShopify()
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
