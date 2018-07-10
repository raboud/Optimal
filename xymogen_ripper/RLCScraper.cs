using Newtonsoft.Json;
using ONWLibrary;
using ShopifySharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace xymogen_ripper
{
	public class RLCScraper : Scraper
	{
		string master = @"C:\Users\Robert\Documents\Supplements Master List.json";
		string cleaned = @"C:\Users\Robert\Documents\Supplements Cleaned.json";
		string supplement = @"C:\Users\Robert\Documents\rlc-supplements.json";
		readonly string CacheFile = @"C:\Users\Robert\Documents\RLC.json";

		public void Supplement()
		{
			List<InvSupplement> inv = null;
			if (File.Exists(supplement))
			{
				inv = JsonConvert.DeserializeObject<List<InvSupplement>>(File.ReadAllText(supplement));
			}
			else
			{
				CleanupInventory(master, cleaned);
				inv = JsonConvert.DeserializeObject<List<InvSupplement>>(File.ReadAllText(cleaned)).Where(i => i.Company == "RLC Labs").ToList();

			}
			List<RLCProduct> prods = GetProducts();

			foreach (InvSupplement i in inv)
			{
				i.productNumber = "";
				if (i.Active)
				{
					i.Name = i.Supplement.Substring(0, i.Supplement.LastIndexOf(" - "));

					//					if (string.IsNullOrEmpty(i.Size))
					{
						if (i.Supplement.Contains("#1000"))
						{
							i.Size = "1000 CT";
						}
						else if (i.Supplement.Contains("#120"))
						{
							i.Size = "120 CT";
						}
						else if (i.Supplement.Contains("#100"))
						{
							i.Size = "100 CT";
						}
						else if (i.Supplement.Contains("#90"))
						{
							i.Size = "90 CT";
						}
						else if (i.Supplement.Contains("#60"))
						{
							i.Size = "60 CT";
						}
						else if (i.Supplement.Contains("#30"))
						{
							i.Size = "30 CT";
						}
					}

					if (i.Size != null)
					{

						if (i.Active)
						{

							RLCProduct prod = prods.FirstOrDefault(p => p.Name == i.Name);

							if (prod == null)
							{
								prod = new RLCProduct();
								prod.Name = i.Name;
								prods.Add(prod);
							}
							//							else
							{
								RLCProduct.Variant v;
								if (!prod.Sizes.Contains(i.Size))
								{
									prod.Sizes.Add(i.Size);
									v = new RLCProduct.Variant();
									v.ImageUrl = i.ImageUrl;
									v.Option1 = i.Size;
									v.Price = i.Retail;
									v.SKU = Path.GetFileNameWithoutExtension(v.ImageUrl);
									v.ImageUrl = i.ImageUrl;
									prod.Variants.Add(v);
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

			File.WriteAllText(CacheFile, JsonConvert.SerializeObject(prods, Formatting.Indented,
				new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
			File.WriteAllText(supplement, JsonConvert.SerializeObject(inv, Formatting.Indented,
				new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

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


		private void BuildProduct(RLCProduct p)
		{
			Product prod = new Product();
			prod.Options = new List<ProductOption>();
			prod.Variants = new List<ProductVariant>();
			prod.Images = new List<ProductImage>();

			prod.PublishedScope = "global";
			prod.Title = p.Name;
			prod.BodyHtml = p.Description;
			prod.Vendor = "RLC Labs";

			ProductOption po = new ProductOption();
			po.Name = "Size";
			foreach (var vt in p.Sizes)
			{
				po.Values = new List<string>(p.Sizes);
			}
			po.ProductId = prod.Id;
			(prod.Options as List<ProductOption>).Add(po);

			foreach (var v in p.Variants)
			{
				ProductVariant pv = new ProductVariant();
				pv.Barcode = v.BarCode;
				pv.Option1 = v.Option1;
				pv.Option2 = v.Option2;
				pv.Option3 = v.Option3;
				pv.SKU = "RLC-" + v.ImageUrl;
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
			foreach (ProductVariant pv in prod.Variants)
			{
				ProductImage pi = prod.Images.FirstOrDefault(pii => pii.Src.Contains($"/{pv.SKU}.png"));
				if (pi != null)
				{
					List<long> ids = new List<long>();
					ids.Add(pv.Id.Value);
					pi.VariantIds = ids;
				}
			}
			prod = Shopify.UpdateProduct(prod);

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

		public List<RLCProduct> GetProducts(bool refresh = false)
		{
			if (!refresh && File.Exists(CacheFile))
			{
				return JsonConvert.DeserializeObject<List<RLCProduct>>(File.ReadAllText(CacheFile),
					new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error, CheckAdditionalContent = true });
			}

			List<RLCProduct> prods = new List<RLCProduct>();
			//			File.WriteAllText(CacheFile, JsonConvert.SerializeObject(prods, Formatting.Indented,
			//				new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
			return prods;

		}

		public void GetImages()
		{
			for (int id = 1; id < 100; id++)
			{
				string pngFile = $"f:\\temp\\Images\\RCL Labs\\{id:D4}_back.png";

				Directory.CreateDirectory(Path.GetDirectoryName(pngFile));

				string url = $"https://assets.naturalpartners.com/data/product/images/xlarge/RL{id:D4}_back.jpg";
				if (!File.Exists(pngFile))
				{
					string ext = Path.GetExtension(url);

					Directory.CreateDirectory(Path.GetDirectoryName(pngFile));

					using (WebClient wc = new WebClient())
					{
						try
						{
							byte[] bytes = wc.DownloadData(url);
							MemoryStream ms = new MemoryStream(bytes);

							Image image = Image.FromStream(ms);
							image.Save(pngFile, ImageFormat.Png);
						}
						catch
						{

						}
					}
				}
			}

		}

		public class RLCProduct
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

			public List<string> Sizes { get; set; }
			public List<Variant> Variants { get; set; }

			public RLCProduct()
			{
				this.Categories = new List<string>();
				this.Variants = new List<Variant>();
				this.Sizes = new List<string>();
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
