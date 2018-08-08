using ONWLibrary;
using HtmlAgilityPack;
using Newtonsoft.Json;
using ShopifySharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace xymogen_ripper
{
	public class OrthoScraper : Scraper
	{
		readonly string CacheFile = @"C:\Users\Robert\Documents\OrthoMolecular.json";

		public async Task Supplement2()
		{
			List<InvSupplement> inv2 = JsonConvert.DeserializeObject<List<InvSupplement>>(File.ReadAllText(supplement));
			List<InvSupplement> inv = inv2.Where(i => i.Active && !i.Imported && !string.IsNullOrEmpty(i.productNumber)).ToList();
			List<OrthoProduct> prods = GetProducts();

			Dictionary<string, List<string>> toImport = new Dictionary<string, List<string>>();
			foreach (InvSupplement i in inv)
			{
				List<OrthoProduct> p = prods.Where(xp => xp.Variants.Any(xv => xv.ProductNumber == i.productNumber)).ToList();
				if (p.Count == 1)
				{
					if (toImport.ContainsKey(p[0].Name))
					{
						if (!toImport[p[0].Name].Contains(i.productNumber))
						{
							toImport[p[0].Name].Add(i.productNumber);
						}
						else
						{
							System.Console.WriteLine($"{i.Name} - {i.productNumber} - {i.Size} - {i.Flavor}");
						}
					}
					else
					{
						toImport.Add(p[0].Name, new List<string>() { i.productNumber });
					}
				}
				else
				{
					System.Console.WriteLine($"{i.Name} - {i.productNumber} - {i.Size} - {i.Flavor}");
				}
			}

			foreach (KeyValuePair<string, List<string>> vp in toImport)
			{
				OrthoProduct prod = prods.FirstOrDefault(p => p.Name == vp.Key);
				if (prod != null)
				{
					prod.Variants.RemoveAll(v => !vp.Value.Contains(v.ProductNumber));
					foreach (InvSupplement i in inv2.Where(iv => vp.Value.Contains(iv.productNumber)))
					{
						i.Imported = true;
						OrthoProduct.Variant v = prod.Variants.FirstOrDefault(pv => pv.ProductNumber == i.productNumber);
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
					await BuildProduct(prod);
					await File.WriteAllTextAsync(supplement, JsonConvert.SerializeObject(inv2, Formatting.Indented,
						new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
					System.Console.WriteLine($"completed");

				}
				else
				{

				}
			}



		}

		private readonly string master = @"C:\Users\Robert\Documents\Supplements Master List.json";
		private readonly string cleaned = @"C:\Users\Robert\Documents\Supplements Cleaned.json";
		private readonly string supplement = @"C:\Users\Robert\Documents\ortho-supplements.json";

		public void Supplement()
		{
			List<OrthoProduct> prods = GetProducts();
			List<string> names = new List<string>();
			foreach (OrthoProduct prod in prods)
			{
				names.Add(prod.Name);
				prod.Name = prod.Name.Replace("™", "").Replace("®", "");
			}
			names.Sort();
			List<InvSupplement> inv = null;
			if (File.Exists(supplement))
			{
				inv = JsonConvert.DeserializeObject<List<InvSupplement>>(File.ReadAllText(supplement));
			}
			else
			{
				CleanupInventory(master, cleaned);
				inv = JsonConvert.DeserializeObject<List<InvSupplement>>(File.ReadAllText(cleaned)).Where(i => i.Company == "Ortho Molecular").ToList();

			}
			foreach (InvSupplement i in inv)
			{
				i.productNumber = "";
				if (i.Company == "Ortho Molecular" && i.Active)
				{
					if (string.IsNullOrEmpty(i.Size) )
					{
						string[] s = i.Supplement.Split(" - ");
						i.Name = s[0];
						if (s.Length >= 3)
						{
							i.Dosage = s[1];
							i.Size = s[2];
							i.Size = i.Size.Replace("caps", "C").Replace("tabs", "T").Replace("serv", "Serv").Replace("pkts", "Serv").Replace("oz", " oz.");
						}
						if (s.Length == 2)
						{
							i.Size = s[1];
							i.Size = i.Size.Replace("caps", "C").Replace("tabs", "T").Replace("serv", "Serv").Replace("pkts", "Serv").Replace("oz", " oz.");
						}
					}

					if (i.Size != null)
					{

						if (i.Active)
						{

							OrthoProduct prod = prods.FirstOrDefault(p => p.Name == i.Name);

							if (prod == null)
							{
								System.Console.WriteLine($"No Matching Ortho Product - {i.Name}");
							}
							else
							{
								OrthoProduct.Variant v;
								if (i.Flavor == null)
								{
									OrthoProduct.Option op = prod.Options.FirstOrDefault(o => o.variantTypeName == "Flavor");
									if (op != null && op.values.Count == 1)
									{
										i.Flavor = op.values[0];
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
									System.Console.WriteLine($"No Matching OrthoProduct Variant - {i.Name} - {i.Size} - {i.Flavor}");
								}
								else
								{
									i.productNumber = v.ProductNumber;
								}

							}
						}
					}
					else
					{
						System.Console.WriteLine($"No Size OrthoProduct Product - {i.Name}");
					}
				}

			}
			File.WriteAllText(supplement, JsonConvert.SerializeObject(inv, Formatting.Indented,
				new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
		}

		private async Task BuildProduct(string name)
		{
			OrthoProduct p = GetProduct(name);
			await BuildProduct(p);
		}

		public void CheckProducts()
		{
			List<OrthoProduct> prods = GetProducts();
			List<string> ProductNumbers = new List<string>();
			List<string> Names = new List<string>();

			foreach (OrthoProduct prod in prods)
			{
				if (Names.Contains(prod.Name.ToLower()))
				{
					System.Console.WriteLine($"Duplicate Product Names - {prod.Name}");
				}
				else
				{
					Names.Add(prod.Name.ToLower());
				}
				if (prod.Variants.Count == 0)
				{
					System.Console.WriteLine($"Product Has No Varients - {prod.Name}");
				}
				foreach (OrthoProduct.Variant variant in prod.Variants)
				{
					if (ProductNumbers.Contains(variant.ProductNumber.ToLower()))
					{
						System.Console.WriteLine($"Duplicate Product Numbers - {prod.Name} - {variant.ProductNumber}");
					}
					else
					{
						ProductNumbers.Add(variant.ProductNumber.ToLower());
					}
				}
			}
		}

		public void GetImages()
		{
			List<OrthoScraper.OrthoProduct> prods = GetProducts();
			foreach (OrthoProduct prod in prods)
			{
				foreach (OrthoProduct.Variant variant in prod.Variants)
				{
					if (variant.ProductNumber != null)
					{
						if (string.IsNullOrEmpty(variant.ImageUrl))
						{
							continue;
						}
						GetImage(variant.ImageUrl, variant.ProductNumber);

					}
				}
			}
		}

		public void GetImage(string url, string id)
		{
			string pngFile = $"f:\\temp\\Images\\Ortho Molecular\\{id}.png";
			if (!File.Exists(pngFile))
			{
				string ext = Path.GetExtension(url);

				Directory.CreateDirectory(Path.GetDirectoryName(pngFile));

				using (WebClient wc = new WebClient())
				{
					if (ext == ".png")
					{
						try
						{
							wc.DownloadFile(url, pngFile);
						}
						catch (Exception)
						{
						}
					}
					else
					{
						byte[] bytes = wc.DownloadData(url);
						MemoryStream ms = new MemoryStream(bytes);

						Image image = Image.FromStream(ms);
						image.Save(pngFile, ImageFormat.Png);
					}
				}
			}
		}


		public OrthoProduct GetProduct(string name)
		{
			return GetProducts().FirstOrDefault(p => p.Name == name);
		}

		private async Task BuildProduct(OrthoProduct p)
		{
			Product prod = new Product
			{
				Options = new List<ProductOption>(),
				Variants = new List<ProductVariant>(),
				Images = new List<ProductImage>(),

				PublishedScope = "global",
				Title = p.Name,
				BodyHtml = p.Description,
				Vendor = "Ortho Molecular"
			};

			foreach (OrthoProduct.Option vt in p.Options)
			{
				ProductOption po = new ProductOption
				{
					Values = new List<string>(vt.values),
					Name = vt.variantTypeName,
					ProductId = prod.Id
				};
				(prod.Options as List<ProductOption>).Add(po);
			}

			foreach (OrthoProduct.Variant v in p.Variants)
			{
				ProductVariant pv = new ProductVariant
				{
					Barcode = v.BarCode,
					Option1 = v.Option1,
					Option2 = v.Option2,
					Option3 = v.Option3,
					SKU = "Ortho-" + v.ProductNumber,
					Taxable = true,
					InventoryManagement = "shopify",
					InventoryQuantity = 0,
					Price = v.Price
				};

				(prod.Variants as List<ProductVariant>).Add(pv);


				ProductImage image = new ProductImage();
				string pngFile = $"f:\\temp\\Images\\Ortho Molecular\\{v.ProductNumber}.png";
				image.Attachment = Convert.ToBase64String(File.ReadAllBytes(pngFile));
				image.Filename = $"{pv.SKU}.png";
				(prod.Images as List<ProductImage>).Add(image);
			}

			prod = await Shopify.CreateProductAsync(prod);
			foreach (ProductVariant pv in prod.Variants)
			{
				ProductImage pi = prod.Images.FirstOrDefault(pii => pii.Src.Contains($"/{pv.SKU}.png"));
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

		public List<OrthoProduct> GetProducts(bool refresh = false)
		{
			if (!refresh && File.Exists(CacheFile))
			{
				return JsonConvert.DeserializeObject<List<OrthoProduct>>(File.ReadAllText(CacheFile),
					new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error, CheckAdditionalContent = true });
			}

			List<OrthoProduct> prods = new List<OrthoProduct>();
			using (WebClient wc = new WebClient())
			{
				for (int page = 1; page <= 14; page++)
				{
					prods.AddRange(GetProducts(page, wc));
				}
			}
			File.WriteAllText(CacheFile, JsonConvert.SerializeObject(prods, Formatting.Indented,
				new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
			return prods;

		}

		private List<OrthoProduct> GetProducts(int page, WebClient wc = null)
		{
			List<OrthoProduct> prods = new List<OrthoProduct>();
			string url = $"https://www.orthomolecularproducts.com/store/view-all/?pg={page}";
			HtmlDocument doc = GetPage(url, wc);
			HtmlNodeCollection n2 = doc.DocumentNode.SelectNodes("//h3[@class='itemTitle']/a");
			if (n2 != null)
			{
				foreach (HtmlNode node in n2)
				{
					string path = node.Attributes["href"].Value;
					string name = node.InnerText;

					prods.Add(GetProduct(path, name, wc));
				}
			}
			return prods;
		}

		private OrthoProduct GetProduct(string path, string name, WebClient wc)
		{
			System.Console.Write(name);
			OrthoProduct product = new OrthoProduct();
			HtmlDocument doc = GetPage(path, wc);
			HtmlNode node;

			node = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']");
			product.ImageUrl = node.Attributes["content"].Value;
		

			product.Name = name;
			product.BriefDescription = doc.DocumentNode.SelectSingleNode("//p[@id='pShortDesc']")?.OuterHtml.Trim();

			node = doc.DocumentNode.SelectSingleNode("//h3[contains(., 'OVERVIEW')]/..");
			node?.RemoveChild(node.FirstChild);
			product.Description = node?.InnerHtml.Trim();

			node = doc.DocumentNode.SelectSingleNode("//h3[contains(., 'CLINICAL APPLICATIONS')]/..");
			node?.RemoveChild(node.FirstChild);
			product.Benefits =node?.InnerHtml.Trim();

			HtmlNode nc = doc.DocumentNode.SelectSingleNode("//script[contains(., 'IdevSelections(')]");
			if (nc != null)
			{
				OrthoProduct.Option op = new OrthoProduct.Option
				{
					variantTypeName = "Size"
				};
				string json = nc.InnerText;
				json = json.Substring(json.IndexOf('{'));
				json = json.Substring(0, json.LastIndexOf('}') + 1);
				Example ex = JsonConvert.DeserializeObject<Example>(json);

				if (ex.Classifications.Count == 0)
				{
					HtmlNodeCollection nc2 = doc.DocumentNode.SelectNodes("//span[@class='bottleSize']");
					if (nc2 != null)
					{
						foreach (HtmlNode n2 in nc2)
						{
							op.values.Add(n2.InnerText.Trim());
						}
					}
					else
					{
						System.Console.Write(" Error");
					}
				}

				if (ex.Classifications.Count > 1)
				{
					System.Console.Write(" Error2");
				}

				foreach (Classification c in ex.Classifications)
				{
					foreach (S s in c.s)
					{
						op.values.Add(s.s);
					}
				}
				product.Options.Add(op);
			}
			else
			{
				System.Console.Write(" Error3");
			}

			foreach (OrthoProduct.Option vt in product.Options)
			{
				foreach (string value in vt.values)
				{
					OrthoProduct.Variant v = new OrthoProduct.Variant
					{
						Option1 = value
					};
					product.Variants.Add(v);
				}
			}
			if (product.Variants.Count == 1)
			{
				product.Variants[0].ImageUrl = product.ImageUrl;
			}
			System.Console.WriteLine(" - Complete");
			return product;
		}

		public class OrthoProduct
		{
			public long Id { get; set; }
			public long MasterId { get; set; }
			public string Name { get; set; }
			public List<string> Categories { get; set; }
			public string BriefDescription { get; set; }
			public string Description { get; set; }
			public string Benefits { get; set; }
			public string ImageUrl { get; set; }

			public string Directions { get; set; }
			public string Contains { get; set; }
			public string DoesNotContain { get; set; }
			public string Ingredients { get; set; }
			public string Storage { get; set; }
			public string PatentInfo { get; set; }

			public List<Option> Options { get; set; }
			public List<Variant> Variants { get; set; }

			public OrthoProduct()
			{
				Categories = new List<string>();
				Variants = new List<Variant>();
				Options = new List<Option>();
			}

			public class Option
			{
				public Option()
				{
					values = new List<string>();
				}
				public string variantTypeName { get; set; }
				public List<string> values { get; set; }
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
				public string ImageUrl { get; set; }
			}

		}


		public class S
		{
			public int i { get; set; }
			public string s { get; set; }
			public string m { get; set; }
			public object a { get; set; }
		}

		public class Classification
		{
			public int i { get; set; }
			public string n { get; set; }
			public object a { get; set; }
			public string m { get; set; }
			public string t { get; set; }
			public bool u { get; set; }
			public IList<S> s { get; set; }
		}

		public class Item
		{
			public int q { get; set; }
			public string c { get; set; }
			public object b { get; set; }
			public int i { get; set; }
			public IList<int> s { get; set; }
			public bool subscription { get; set; }
		}

		public class Example
		{
			public string Variable { get; set; }
			public object Suffix { get; set; }
			public string Container { get; set; }
			public bool EnableInventory { get; set; }
			public string AddToCartBtn { get; set; }
			public string AddToWishlistBtn { get; set; }
			public string EmailContainer { get; set; }
			public string QuantityTextBox { get; set; }
			public string RecipientDropDown { get; set; }
			public string RecipientTextBox { get; set; }
			public int ValueId { get; set; }
			public int OrderItemId { get; set; }
			public IList<Classification> Classifications { get; set; }
			public IList<Item> Items { get; set; }
			public int SelectedItem { get; set; }
			public int SwatchWidth { get; set; }
			public int SwatchHeight { get; set; }
			public int PortalId { get; set; }
		}

	}
}
