using Newtonsoft.Json;
using ONWLibrary;
using ShopifySharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace xymogen_ripper
{
    public class MasterScraper : Scraper
    {
		string master = @"C:\Users\Robert\Documents\Supplements Master List.json";
		string cleaned = @"C:\Users\Robert\Documents\Supplements Cleaned.json";
		string supplement = @"C:\Users\Robert\Documents\Master-supplements.json";
		readonly string CacheFile = @"C:\Users\Robert\Documents\Master.json";
		readonly string dump = @"C:\Users\Robert\Documents\dump.json";


		public void Cleanup()
		{
			List<InvSupplement> inv = GetInventory();
			IEnumerable<Product> products = Shopify.GetProducts().Result;
			foreach (Product p in products )
			{
				p.Title = p.Title.Replace("®", "").Replace("™", "").Replace("®", "");
				List<InvSupplement> imported = inv.Where(i => i.Name == p.Title && p.Vendor == i.Company).ToList();
				foreach (InvSupplement i in imported)
				{
					i.Imported = true;
				}
				if (imported.Count == 0)
				{
					System.Console.WriteLine($"No Matches {p.Vendor} - {p.Title}");
				}
			}

			foreach (InvSupplement i in inv.Where(i2 => i2.Imported))
			{
				int count = products.Where(p => p.Title == i.Name && p.Vendor == i.Company).Count();

				if (products.Count(p => p.Title == i.Name && p.Vendor == i.Company) != 1)
				{
					System.Console.WriteLine($"No Matches {i.Company} - {i.Name}");
				}
			}

			File.WriteAllText(supplement, JsonConvert.SerializeObject(inv, Formatting.Indented,
				new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

		}

		public List<InvSupplement> GetInventory()
		{
			List<InvSupplement> inv = null;
			if (File.Exists(supplement))
			{
				inv = JsonConvert.DeserializeObject<List<InvSupplement>>(File.ReadAllText(supplement));
			}
			else
			{
				CleanupInventory(master, cleaned);
				inv = JsonConvert.DeserializeObject<List<InvSupplement>>(File.ReadAllText(cleaned));

			}
			return inv;
		}

		public void flatProducts()
		{
			IEnumerable<Product> prods = Shopify.GetProducts().Result;
			File.WriteAllText(dump, JsonConvert.SerializeObject(prods, Formatting.Indented,
				new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
			int count = 1;
			foreach(var prod in prods)
			{
				foreach (var pv in prod.Variants)
				{
					System.Console.WriteLine($"{count}\t{prod.Vendor}\t{prod.Title}\t{pv.Price}\t{pv.Option1}\t{pv.Option3}");
					count++;
				}
			}
		}


		public void Supplement()
		{
			List<InvSupplement> inv = GetInventory();

			List<MasterProduct> prods = GetProducts();

			foreach (InvSupplement i in inv)
			{
				if (i.Active)
				{
					if (string.IsNullOrEmpty(i.Name))
					{
						if (i.Supplement.Contains(" - "))
						{
							i.Name = i.Supplement.Substring(0, i.Supplement.IndexOf(" - "));
						}
						else
						{
							i.Name = i.Supplement;
						}
					}
					string t = i.Supplement.ToLower();
//					if (string.IsNullOrEmpty(i.Size))
					{
						if (t.Contains("#1000") || t.Contains("- 1000 caps") || t.Contains("- 1000 sg") || t.Contains("- 1000 pkts"))
						{
							i.Size = "1000 CT";
						}
						else if (t.Contains("#240") || t.Contains("- 240 caps") || t.Contains("- 240 tabs") || t.Contains("- 240 sg") || t.Contains("- 240 pkts"))
						{
							i.Size = "240 CT";
						}
						else if (t.Contains("#180") || t.Contains("- 180 caps") || t.Contains("- 180 tabs") || t.Contains("- 180 sg") || t.Contains("- 180 pkts"))
						{
							i.Size = "180 CT";
						}
						else if (t.Contains("#120") || t.Contains("- 120 caps") || t.Contains("- 120 tabs") || t.Contains("- 120 sg") || t.Contains("- 120 pkts"))
						{
							i.Size = "120 CT";
						}
						else if (t.Contains("#100") || t.Contains("- 100 caps") || t.Contains("- 100 tabs") || t.Contains("- 100 sg") || t.Contains("- 100 pkts"))
						{
							i.Size = "100 CT";
						}
						else if (t.Contains("#90") || t.Contains("- 90 caps") || t.Contains("- 90 tabs") || t.Contains("- 90 sg") || t.Contains("- 90 pkts"))
						{
							i.Size = "90 CT";
						}
						else if (t.Contains("#60") || t.Contains("- 60 caps") || t.Contains("- 60 tabs") || t.Contains("- 60 sg") || t.Contains("- 60 pkts") || t.Contains("- 60 chews"))
						{
							i.Size = "60 CT";
						}
						else if (t.Contains("#40") || t.Contains("- 40 caps") || t.Contains("- 40 tabs") || t.Contains("- 40 sg") || t.Contains("- 40 doses") || t.Contains("- 40 pkts"))
						{
							i.Size = "40 CT";
						}
						else if (t.Contains("#30") || t.Contains("- 30 caps") || t.Contains("- 30 tabs") || t.Contains("- 30 sg") || t.Contains("- 30 doses") || t.Contains("- 30 pkts") || t.Contains("- 30 ct"))
						{
							i.Size = "30 CT";
						}
						else if (t.Contains("#15") || t.Contains("- 15 caps") || t.Contains("- 15 tabs") || t.Contains("- 15 sg") || t.Contains("- 15 doses") || t.Contains("- 15 pkts"))
						{
							i.Size = "15 CT";
						}
						else if (t.Contains("#10") || t.Contains("- 10 caps") || t.Contains("- 10 tabs") || t.Contains("- 10 sg") || t.Contains("- 10 doses") || t.Contains("- 10 pkts"))
						{
							i.Size = "10 CT";
						}
						else if (t.Contains("#6") || t.Contains("- 6 caps") || t.Contains("- 6 doses"))
						{
							i.Size = "6 CT";
						}
						else if (t.Contains(" - 85 serv"))
						{
							i.Size = "85 Serv";
						}
						else if (t.Contains(" - 60 serv"))
						{
							i.Size = "60 Serv";
						}
						else if (t.Contains(" - 50 serv"))
						{
							i.Size = "50 Serv";
						}
						else if (t.Contains(" - 36 serv"))
						{
							i.Size = "36 Serv";
						}
						else if (t.Contains(" - 30 serv"))
						{
							i.Size = "30 Serv";
						}
						else if (t.Contains(" - 15 serv"))
						{
							i.Size = "15 Serv";
						}
						else if (t.Contains(" - 14 serv"))
						{
							i.Size = "14 Serv";
						}
						else if (t.Contains(" - 12 serv"))
						{
							i.Size = "12 Serv";
						}
						else if (t.Contains(" - 7 serv"))
						{
							i.Size = "7 Serv";
						}
						else if (t.Contains(" - 32oz") || t.Contains(" - 32 oz"))
						{
							i.Size = "16 Oz";
						}
						else if (t.Contains(" - 16oz") || t.Contains(" - 16 oz"))
						{
							i.Size = "16 Oz";
						}
						else if (t.Contains(" - 8oz") || t.Contains(" - 8 oz"))
						{
							i.Size = "8 Oz";
						}
						else if (t.Contains(" - 4oz") || t.Contains(" - 4 oz"))
						{
							i.Size = "4 Oz";
						}
						else if (t.Contains(" - 3oz") || t.Contains(" - 3 oz"))
						{
							i.Size = "3 Oz";
						}
						else if (t.Contains(" - 2oz") || t.Contains(" - 2 oz"))
						{
							i.Size = "2 Oz";
						}
						else if (t.Contains(" - 1oz") || t.Contains(" - 1 oz"))
						{
							i.Size = "1 Oz";
						}
					}

					if (i.Size != null)
					{

						if (i.Active && !i.Imported)
						{

							MasterProduct prod = prods.FirstOrDefault(p => p.Name == i.Name);

							if (prod == null)
							{
								prod = new MasterProduct();
								prod.Name = i.Name;
								if (!string.IsNullOrEmpty(i.Company))
								{
									prod.Vendor = i.Company;
								}
								prods.Add(prod);
							}
							//							else
							{
								if (!string.IsNullOrEmpty(i.Flavor) && !prod.Flavors.Contains(i.Flavor))
								{
									prod.Flavors.Add(i.Flavor);
								}
								if (!string.IsNullOrEmpty(i.Size) && !prod.Sizes.Contains(i.Size))
								{
									prod.Sizes.Add(i.Size);
								}
								MasterProduct.Variant v;
								if (prod.Variants.All(pv => pv.Option1 != i.Size || pv.Option2 != i.Flavor))
								{
									v = new MasterProduct.Variant();
//									v.ImageUrl = i.ImageUrl;
									v.Option1 = i.Size;
									v.Option2 = i.Flavor;
									v.Price = i.Retail;
//									v.SKU = Path.GetFileNameWithoutExtension(v.ImageUrl);
									v.ImageUrl = i.ImageUrl;
									prod.Variants.Add(v);
								}
							}
						}
					}
					else
					{
						System.Console.WriteLine($"No Size Product - {i.Supplement}");
					}
				}

			}

			File.WriteAllText(CacheFile, JsonConvert.SerializeObject(prods, Formatting.Indented,
				new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
			File.WriteAllText(supplement, JsonConvert.SerializeObject(inv, Formatting.Indented,
				new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

		}


		public List<MasterProduct> GetProducts(bool refresh = false)
		{
			if (!refresh && File.Exists(CacheFile))
			{
				return JsonConvert.DeserializeObject<List<MasterProduct>>(File.ReadAllText(CacheFile),
					new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error, CheckAdditionalContent = true });
			}

			List<MasterProduct> prods = new List<MasterProduct>();
			//			File.WriteAllText(CacheFile, JsonConvert.SerializeObject(prods, Formatting.Indented,
			//				new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
			return prods;

		}

		public void Supplement2()
		{
			List<InvSupplement> inv2 = null;
			if (File.Exists(supplement))
			{
				inv2 = JsonConvert.DeserializeObject<List<InvSupplement>>(File.ReadAllText(supplement));
			}
			var prods = GetProducts();

			foreach (var prod in prods)
			{
				if (prod != null && !inv2.Where(i => i.Name == prod.Name && i.Active).Any(i => i.Imported))
				{
					System.Console.Write($"{prod.Name} ");
					BuildProduct(prod);
					foreach (var inv in inv2.Where(i => i.Name == prod.Name && i.Active))
					{
						inv.Imported = true;
					}
					File.WriteAllText(supplement, JsonConvert.SerializeObject(inv2, Formatting.Indented,
						new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
					System.Console.WriteLine($"completed");

				}
				else
				{

				}
			}



		}

		public void BuildProduct(MasterProduct p)
		{
			Product prod = new Product();
			prod.Options = new List<ProductOption>();
			prod.Variants = new List<ProductVariant>();
			prod.Images = new List<ProductImage>();

			prod.PublishedScope = "global";
			prod.Title = p.Name;
			prod.BodyHtml = p.Description;
			prod.Vendor = p.Vendor;

			if (p.Sizes.Count > 0)
			{
				ProductOption po = new ProductOption();
				po.Name = "Size";
				foreach (var vt in p.Sizes)
				{
					po.Values = new List<string>(p.Sizes);
				}
				po.ProductId = prod.Id;
				(prod.Options as List<ProductOption>).Add(po);
			}

			if (p.Flavors.Count > 0)
			{
				ProductOption po = new ProductOption();
				po.Name = "Flavor";
				foreach (var vt in p.Flavors)
				{
					po.Values = new List<string>(p.Flavors);
				}
				po.ProductId = prod.Id;
				(prod.Options as List<ProductOption>).Add(po);
			}

			foreach (var v in p.Variants)
			{
				ProductVariant pv = new ProductVariant();
				pv.Barcode = v.BarCode;
				pv.Option1 = v.Option1;
				pv.Option2 = v.Option2;
				pv.Option3 = v.Option3;
				pv.Taxable = true;
				pv.InventoryManagement = "shopify";
				pv.InventoryQuantity = 0;
				pv.Price = v.Price;

				(prod.Variants as List<ProductVariant>).Add(pv);

				if (!string.IsNullOrEmpty(v.ImageUrl))
				{
					ProductImage image = new ProductImage();
					string pngFile = $"f:\\temp\\Images\\RLC Labs\\{v.ImageUrl}.png";
					image.Attachment = Convert.ToBase64String(File.ReadAllBytes(pngFile));
					image.Filename = $"{pv.SKU}.png";
					(prod.Images as List<ProductImage>).Add(image);
				}
			}

			prod = Shopify.CreateProduct(prod);
			//foreach (ProductVariant pv in prod.Variants)
			//{
			//	ProductImage pi = prod.Images.FirstOrDefault(pii => pii.Src.Contains($"/{pv.SKU}.png"));
			//	if (pi != null)
			//	{
			//		List<long> ids = new List<long>();
			//		ids.Add(pv.Id.Value);
			//		pi.VariantIds = ids;
			//	}
			//}
			//prod = Shopify.UpdateProduct(prod);

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

		public class MasterProduct
		{
			public long Id { get; set; }
			public long MasterId { get; set; }
			public string Name { get; set; }
			public string Vendor { get; set; }
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

			public List<string> Sizes { get; set; }
			public List<string> Flavors { get; set; }
			public List<Variant> Variants { get; set; }

			public MasterProduct()
			{
				this.Categories = new List<string>();
				this.Variants = new List<Variant>();
				this.Sizes = new List<string>();
				this.Flavors = new List<string>();
			}

			public class Option
			{
				public Option()
				{
					this.values = new List<string>();
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
	}


}
