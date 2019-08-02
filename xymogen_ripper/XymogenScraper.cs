using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.IO;
using System.Drawing;
using Newtonsoft.Json;
using ShopifySharp;
using ONWLibrary;
using System.Threading.Tasks;

namespace xymogen_ripper
{
	public class XymogenScraper : Scraper
	{
		public static readonly int startingId = 1;
		public static readonly int endingId = 999;

		private readonly List<int> idsFound = new List<int>();
		public XymogenScraper()
		{
		}

		private static async Task BuildProductAsync(int id)
		{
			XymogenProduct p = GetCacheProductInfo(id);
			await BuildProductAsync(p);
		}

		private static async Task Supplement2Async()
		{
			List<InvSupplement> inv2 = JsonConvert.DeserializeObject<List<InvSupplement>>(File.ReadAllText(@"C:\Users\Robert\Documents\supplements.json"));
			List<InvSupplement> inv = inv2.Where(i => i.Company == "Xymogen" && i.Active && !i.Imported && i.XymogenId != 0).ToList();
			Dictionary<long, XymogenScraper.XymogenProduct> prods = GetCacheProductInfo();

			Dictionary<long, List<int>> toImport = new Dictionary<long, List<int>>();
			foreach (InvSupplement i in inv)
			{
				List<XymogenScraper.XymogenProduct> p = prods.Values.Where(xp => xp.Variants.Any(xv => xv.ProductID == i.XymogenId)).ToList();
				if (p.Count == 1)
				{
					if (toImport.ContainsKey(p[0].MasterId))
					{
						if (!toImport[p[0].MasterId].Contains(i.XymogenId))
						{
							toImport[p[0].MasterId].Add(i.XymogenId);
						}
						else
						{
							System.Console.WriteLine($"{i.Name} - {i.XymogenId} - {i.Size} - {i.Flavor}");
						}
					}
					else
					{
						toImport.Add(p[0].MasterId, new List<int>() { i.XymogenId });
					}
				}
				else
				{
					System.Console.WriteLine($"{i.Name} - {i.XymogenId} - {i.Size} - {i.Flavor}");
				}
			}

			foreach (KeyValuePair<long, List<int>> vp in toImport)
			{
				XymogenProduct prod = prods.Values.FirstOrDefault(p => p.MasterId == vp.Key);
				if (prod != null)
				{
					prod.Variants.RemoveAll(v => !vp.Value.Contains(v.ProductID));
					foreach (InvSupplement i in inv2.Where(iv => vp.Value.Contains(iv.XymogenId)))
					{
						i.Imported = true;
						XymogenProduct.Variant v = prod.Variants.FirstOrDefault(pv => pv.ProductID == i.XymogenId);
						if (v != null)
						{
							if (i.Retail == 0.00m)
							{

							}
							v.Price = i.Retail;
						}
						else
						{

						}
					}
					System.Console.Write($"{prod.Name} ");
					await BuildProductAsync(prod);
					File.WriteAllText(@"C:\Users\Robert\Documents\supplements.json", JsonConvert.SerializeObject(inv2, Formatting.Indented,
						new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
					System.Console.WriteLine($"completed");

				}
				else
				{

				}
			}



		}

		private static void Supplement()
		{
			Dictionary<long, XymogenProduct> prods = GetCacheProductInfo();
			List<string> names = new List<string>();
			foreach (XymogenProduct prod in prods.Values)
			{
				names.Add(prod.Name);
				prod.Name = prod.Name.Replace("™", "").Replace("®", "");
			}
			names.Sort();

			List<InvSupplement> inv = JsonConvert.DeserializeObject<List<InvSupplement>>(File.ReadAllText(@"C:\Users\Robert\Documents\supplements.json"));
			foreach (InvSupplement i in inv)
			{
				i.XymogenId = 0;
				if (i.Company == "Xymogen" && i.Active)
				{
					//string[] s = i.Supplement.Split(" - ");
					//i.Name = s[0];
					//if (s.Length >= 3)
					//{
					//	i.Dosage = s[1];
					//	i.Size = s[2];
					//}
					//if (s.Length == 2)
					//{
					//	i.Size = s[1];
					//}

					if (i.Size != null)
					{
						//						i.Size = i.Size.Replace("caps", "C").Replace("tabs", "T").Replace("serv", "Serv").Replace("pkts", "Serv").Replace("oz", " oz.");

						if (i.Active)
						{

							XymogenProduct prod = prods.Values.FirstOrDefault(p => p.Name == i.Name);

							if (prod == null)
							{
								System.Console.WriteLine($"No Matching Xymogen Product - {i.Name}");
							}
							else
							{
								XymogenProduct.Variant v;
								if (i.Flavor == null)
								{
									XymogenProduct.Option op = prod.Options.FirstOrDefault(o => o.variantTypeName == "Flavor");
									if (op != null && op.values.Count == 1)
									{
										i.Flavor = op.values[0].variantTypeValueName;
									}
								}

								if (i.Flavor == null)
								{
									v = prod.Variants.FirstOrDefault(xv => xv.Option1 == i.Size && xv.Option2 == null);
								}
								else
								{
									v = prod.Variants.FirstOrDefault(xv => (xv.Option1 == i.Size || xv.Option2 == i.Size) && (xv.Option1 == i.Flavor || xv.Option2 == i.Flavor));
								}
								if (v == null)
								{
									System.Console.WriteLine($"No Matching Xymogen Variant - {i.Name} - {i.Size} - {i.Flavor}");
								}
								else
								{
									i.XymogenId = v.ProductID;
								}

							}
						}
					}
					else
					{
						System.Console.WriteLine($"No Size Xymogen Product - {i.Name}");
					}
				}

			}
			File.WriteAllText(@"C:\Users\Robert\Documents\supplements.json", JsonConvert.SerializeObject(inv, Formatting.Indented,
				new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
		}


		private static async Task BuildProductAsync(XymogenProduct p)
		{
			Product prod = new Product
			{
				Options = new List<ProductOption>(),
				Variants = new List<ProductVariant>(),
				Images = new List<ProductImage>(),

				PublishedScope = "global",
				Title = p.Name,
				BodyHtml = p.Description,
				Vendor = "Xymogen"
			};

			foreach (XymogenProduct.Option vt in p.Options)
			{
				ProductOption po = new ProductOption
				{
					Values = new List<string>(),
					Name = vt.variantTypeName,
					ProductId = prod.Id
				};
				foreach (XymogenProduct.Value vv in vt.values)
				{
					(po.Values as List<string>).Add(vv.variantTypeValueName);
				}
				(prod.Options as List<ProductOption>).Add(po);
			}

			foreach (XymogenProduct.Variant v in p.Variants)
			{
				ProductVariant pv = new ProductVariant
				{
					Barcode = v.BarCode,
					Option1 = v.Option1,
					Option2 = v.Option2,
					Option3 = v.Option3,
					SKU = v.SKU,
					Taxable = true,
					InventoryManagement = "shopify",
					InventoryQuantity = 0,
					Price = v.Price
				};

				(prod.Variants as List<ProductVariant>).Add(pv);


				ProductImage image = new ProductImage
				{
					Attachment = Convert.ToBase64String(File.ReadAllBytes($"f:\\temp\\Images\\{v.ProductID}.png")),
					Filename = $"{v.ProductID}.png"
				};
				(prod.Images as List<ProductImage>).Add(image);
			}

			prod = await Shopify.CreateProductAsync(prod);
			foreach (ProductVariant pv in prod.Variants)
			{
				string vid = pv.SKU.Split('-')[1];
				ProductImage pi = prod.Images.FirstOrDefault(pii => pii.Src.Contains($"/{vid}.png"));
				List<long> ids = new List<long>
				{
					pv.Id.Value
				};
				pi.VariantIds = ids;
			}
			prod = await Shopify.UpdateProductAsync(prod);

			Shopify.CreateProductMetadata(prod.Id.Value, "ownProduct", "ShortDescription", p.BriefDescription);
			Shopify.CreateProductMetadata(prod.Id.Value, "ownProduct", "Benefits", p.Benefits);
			Shopify.CreateProductMetadata(prod.Id.Value, "ownProduct", "Contains", p.Contains);
			Shopify.CreateProductMetadata(prod.Id.Value, "ownProduct", "Directions", p.Directions);
			Shopify.CreateProductMetadata(prod.Id.Value, "ownProduct", "DoesNotContain", p.DoesNotContain);
			Shopify.CreateProductMetadata(prod.Id.Value, "ownProduct", "Ingredients", p.Ingredients);
			Shopify.CreateProductMetadata(prod.Id.Value, "ownProduct", "PatentInfo", p.PatentInfo);
			Shopify.CreateProductMetadata(prod.Id.Value, "ownProduct", "Storage", p.Storage);

			foreach (string collect in p.Categories)
			{
				Shopify.AddProductToCollection(prod, collect);
			}
		}

		public static XymogenProduct GetCacheProductInfo(int id)
		{
			Dictionary<long, XymogenProduct> prods = new Dictionary<long, XymogenProduct>();
			if (File.Exists(path))
			{
				prods = JsonConvert.DeserializeObject<Dictionary<long, XymogenProduct>>(File.ReadAllText(path));
			}
			return prods.Values.FirstOrDefault(p => p.Variants.Any(v => v.ProductID == id));
		}

		public static Dictionary<long, XymogenProduct> GetCacheProductInfo()
		{
			Dictionary<long, XymogenProduct> prods = new Dictionary<long, XymogenProduct>();
			if (File.Exists(path))
			{
				prods = JsonConvert.DeserializeObject<Dictionary<long, XymogenProduct>>(File.ReadAllText(path));
			}
			return prods;
		}

		private readonly static string path = "Xymogen.json";

		public static void GetProductInfo()
		{

			XymogenScraper scraper = new XymogenScraper();
			Dictionary<long, XymogenProduct> prods = new Dictionary<long, XymogenProduct>();
			if (File.Exists(path))
			{
				prods = JsonConvert.DeserializeObject<Dictionary<long, XymogenProduct>>(File.ReadAllText(path));
			}

			using (WebClient wc = new WebClient())
			{
				for (int id = startingId; id < endingId; id++)
				{
					System.Console.Write($"{id}\r");
					if (!prods.Any(p => p.Value.Variants.Any(v => v.ProductID == id)))
					{
						XymogenProduct prod = scraper.ProcessPage(id);
						if (prod != null)
						{
							if (!prods.ContainsKey(prod.MasterId))
							{
								prods.Add(prod.MasterId, prod);

								string s = JsonConvert.SerializeObject(prods);
								File.WriteAllText(path, s);
							}
							else
							{

							}
						}
					}
				}
			}
		}

		public static void GetLablels()
		{
			using (WebClient wc = new WebClient())
			{
				for (int id = startingId; id < endingId; id++)
				{
					string pdfFile = $"f:\\temp\\Labels\\{id}.pdf";
					try
					{
						wc.DownloadFile($"https://www.xymogen.com/assets/imageDisplay.ashx?productID={id}&attachmentTypeID=5", pdfFile);
					}
					catch (Exception)
					{
					}
				}
			}
		}

		public static void GetUPCCodes()
		{
			using (PdfConverter convert = new PdfConverter())
			{
				if (convert.Initialize())
				{
					for (int id = XymogenScraper.startingId; id <= XymogenScraper.endingId; id++)
					{
						string pdfFile = $"f:\\temp\\Labels\\{id}.pdf";
						if (File.Exists(pdfFile))
						{
							Bitmap bm = convert.ToBitmap(pdfFile);
							if (bm != null)
							{
								string x = ONWLibrary.Extentions.UPCDecode2(bm);
								if (string.IsNullOrEmpty(x))
								{
									System.Console.WriteLine($"{pdfFile} - Could not determin UPC");
								}
								else
								{
									System.Console.WriteLine($"{pdfFile} - UPC = {x}");
								}
							}
							else
							{
								System.Console.WriteLine($"Could not convert {pdfFile} to bitmap");
								File.Delete(pdfFile);
							}
						}
					}
				}
			}
		}

		public static void GetImages()
		{
			using (WebClient wc = new WebClient())
			{
				for (int id = startingId; id < endingId; id++)
				{
					string pngFile = $"f:\\temp\\Images\\{id}.png";
					try
					{
						wc.DownloadFile($"https://www.xymogen.com/assets/imageDisplay.ashx?productID={id}&attachmentTypeID=2", pngFile);
					}
					catch (Exception)
					{
					}
				}
			}
		}

		public XymogenProduct ProcessPage(int id, WebClient wc = null)
		{
			string url = $"https://www.xymogen.com/formulas/products/{id}";

            HtmlDocument doc = GetPage(url, wc);
			//WebClient client = wc;

			//if (client == null)
			//{
			//	client = new WebClient();
			//}
			//rawHtml = client.DownloadString(new Uri(url));
			//if (wc == null)
			//{
			//	client.Dispose();
			//}
			//while (rawHtml.Contains("\n "))
			//{
			//	rawHtml = rawHtml.Replace("\n ", "\n");
			//}

			//rawHtml = rawHtml.Replace("\n", "\r\n");
			//rawHtml = rawHtml.Replace("\r\r\n", "\r\n");
			//rawHtml = rawHtml.Replace("\r\n\r\n", "\r\n");
			//rawHtml = rawHtml.Replace("\r\n", "\n");
			//HtmlWeb web = new HtmlWeb();
			//HtmlDocument doc = new HtmlDocument();
			//doc.LoadHtml(rawHtml);
			//sanitizeNode(doc.DocumentNode);

			HtmlNodeCollection n2 = doc.DocumentNode.SelectNodes("//div[@id='udpProductDetails']/div/div");
			if (n2 != null)
			{
				foreach (HtmlNode node in n2)
				{
					XymogenProduct p = processNode(node);
					if (p.Id == id)
					{
						p.MasterId = findMasterId(id);
						GetOptions(p);
						return p;
					}
				}
			}

			return null;

		}

		private void GetOptions(XymogenProduct p)
		{
			using (WebClient wc = new WebClient())
			{
				wc.Headers[HttpRequestHeader.ContentType] = "application/json";
				string response = wc.UploadString("https://www.xymogen.com/usercontrols/utilities/ProductVariants.asmx/LoadProductVariantOptions", $"{{MasterId: {p.MasterId}, variantSelections: null, ProductID: 0, Favorites: false}}");

				Wrapper o = JsonConvert.DeserializeObject<Wrapper>(response);

				string[] parts = o.d.Substring(1, o.d.Length - 2).Split("],");
				p.Options = JsonConvert.DeserializeObject<List<XymogenProduct.Option>>(parts[0].Trim() + "]");
				int index = 1;
				foreach (XymogenProduct.Option t in p.Options)
				{
					string s = parts[index].Trim() + "]";
					t.values = JsonConvert.DeserializeObject<List<XymogenProduct.Value>>(s);
					index++;
				}

				List<combos> vars = buildVariants(p.Options, 0);
				foreach (combos variant in vars)
				{
					wc.Headers[HttpRequestHeader.ContentType] = "application/json";
					response = wc.UploadString("https://www.xymogen.com/usercontrols/utilities/ProductVariants.asmx/LoadProductVariantOptions", $"{{MasterId: {p.MasterId}, variantSelections: [{string.Join(",", variant.ids)}], ProductID: 0, Favorites: false}}");

					o = JsonConvert.DeserializeObject<Wrapper>(response);
					if (!string.IsNullOrEmpty(o.d))
					{
						parts = o.d.Substring(1, o.d.Length - 2).Split("],");
						XymogenProduct.Variant ppp = JsonConvert.DeserializeObject<List<XymogenProduct.Variant>>(parts.Last())[0];
						ppp.Option1 = variant.values[0];
						if (variant.values.Count > 1)
						{
							ppp.Option2 = variant.values[1];
						}

						if (variant.values.Count > 2)
						{
							ppp.Option3 = variant.values[2];
						}

						ppp.SKU = $"XMOGEN-{ppp.ProductID}";
						ppp.BarCode = GetUPCCodes(ppp.ProductID);
						p.Variants.Add(ppp);
					}
				}
			}

		}

		public static string GetUPCCodes(int id)
		{
			using (PdfConverter convert = new PdfConverter())
			{
				if (convert.Initialize())
				{
					//					for (int id = XymogenScraper.startingId; id <= XymogenScraper.endingId; id++)
					{
						string pdfFile = $"f:\\temp\\Labels\\{id}.pdf";
						if (File.Exists(pdfFile))
						{
							Bitmap bm = convert.ToBitmap(pdfFile);
							if (bm != null)
							{
								string x = ONWLibrary.Extentions.UPCDecode2(bm);
								return x;
							}
						}
					}
				}
			}
			return null;
		}

		public class combos
		{
			public combos()
			{
				ids = new List<string>();
				values = new List<string>();
			}
			public List<string> ids;
			public List<string> values;

		}

		public List<combos> buildVariants(List<XymogenProduct.Option> types, int level)
		{
			List<combos> o = new List<combos>();
			if (level >= types.Count)
			{
				return null;
			}
			List<combos> lower = buildVariants(types, level + 1);

			foreach (XymogenProduct.Value t in types[level].values)
			{
				string thisLevel = t.variantTypeValueId.ToString();
				if (lower != null)
				{
					foreach (combos l in lower)
					{
						combos c = new combos();
						c.ids.Add(thisLevel);
						c.values.Add(t.variantTypeValueName);
						c.ids.AddRange(l.ids);
						c.values.AddRange(l.values);
						o.Add(c);
					}
				}
				else
				{
					combos c = new combos();
					c.ids.Add(thisLevel);
					c.values.Add(t.variantTypeValueName);
					o.Add(c);
				}
			}

			return o;
		}

		private int findMasterId(int id)
		{
			int index = 0;
			while (-1 != (index = rawHtml.IndexOf("displayVariants(", index)))
			{
				string[] split = rawHtml.Substring(index).Split(new char[] { '(', ',', ')' }, 5);
				if (split[2] == id.ToString())
				{
					return int.Parse(split[1]);
				}
				index++;
			}
			return -1;
		}

		public XymogenProduct processNode(HtmlNode node)
		{
			XymogenProduct product = new XymogenProduct();
			HtmlNode nameNode = node.SelectSingleNode(".//h2");
			HtmlNode miniNode = node.SelectSingleNode(".//h4[@class='pi-text-base pi-has-border pi-visible-xs']");
			HtmlNode categoryNode = node.SelectSingleNode(".//ul[@class='pi-meta']");
			HtmlNode briefNode = node.SelectSingleNode(".//ul[@class='pi-list-with-icons pi-list-icons-right-open']");
			HtmlNode descNode = node.SelectSingleNode(".//h4[contains(., 'Formula Details')]").NextSibling;

			product.Name = nameNode.InnerText.Trim();

			product.Benefits = briefNode?.InnerHtml;

			if (categoryNode != null)
			{
				string category = categoryNode?.InnerText.Trim();
				product.Categories = category.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
			}

			product.BriefDescription = miniNode.InnerText.Trim();
			product.Description = descNode.InnerHtml.Trim();


			HtmlNode directNode = node.SelectSingleNode(".//a[contains(., 'Directions:')]/../../div");
			product.Directions = directNode?.InnerText.Trim();

			HtmlNode containsNode = node.SelectSingleNode(".//a[contains(., 'Contains:')]/../../div");
			product.Contains = containsNode?.InnerText.Trim();

			HtmlNode dncNode = node.SelectSingleNode(".//a[contains(., 'Does Not Contain:')]/../../div");
			product.DoesNotContain = dncNode?.InnerText.Trim();

			HtmlNode ingNode = node.SelectSingleNode(".//a[contains(., 'Other Ingredients:')]/../../div");
			product.Ingredients = ingNode?.InnerText.Trim();

			HtmlNode storageNode = node.SelectSingleNode(".//a[contains(., 'Storage:')]/../../div");
			product.Storage = storageNode?.InnerText.Trim();

			HtmlNode patentNode = node.SelectSingleNode(".//a[contains(., 'Patent Info:')]/../../div");
			product.PatentInfo = patentNode?.InnerText.Trim();

			string imageNode = node.SelectSingleNode(".//img").Attributes["src"].Value.Split(new char[] { '=', '&' })[1];
			product.Id = Convert.ToInt64(imageNode);

			//https://www.xymogen.com/assets/imageDisplay.ashx?productID=92&attachmentTypeID=2

			return product;
		}

		public class XymogenProduct
		{
			public long Id { get; set; }
			public long MasterId { get; set; }
			public string Name { get; set; }
			public List<string> Categories { get; set; }
			public string BriefDescription { get; set; }
			public string Description { get; set; }
			public string Benefits { get; set; }

			public string Directions { get; set; }
			public string Contains { get; set; }
			public string DoesNotContain { get; set; }
			public string Ingredients { get; set; }
			public string Storage { get; set; }
			public string PatentInfo { get; set; }

			public List<Option> Options { get; set; }
			public List<Variant> Variants { get; set; }

			public XymogenProduct()
			{
				Categories = new List<string>();
				Variants = new List<Variant>();
			}

			public class Option
			{
				public int variantTypeId { get; set; }
				public string variantTypeName { get; set; }
				public List<Value> values;
			}

			public class Value
			{
				public int variantTypeValueId { get; set; }
				public string variantTypeValueName { get; set; }
				public bool variantValueSelected { get; set; }
			}

			public class Variant
			{
				public string ProductNumber { get; set; }
				public int ProductID { get; set; }
				public string Option1 { get; set; }
				public string Option2 { get; set; }
				public string Option3 { get; set; }
				public string SKU { get; set; }
				public string BarCode { get; set; }
				public decimal? Price { get; set; }
			}

		}

		public class Wrapper
		{
			public string d { get; set; }
		}

	}




}
