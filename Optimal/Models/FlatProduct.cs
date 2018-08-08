using Newtonsoft.Json;
using ShopifySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimal.Models
{
	public class FlatProduct
	{
		public string Vendor { get { return Product.Vendor; } }
		public string Name { get { return Product.Title; } }
		public string Option1 { get { return ProductVariant.Option1; } }
		public string Option2 { get { return ProductVariant.Option2; } }
		public string Option3 { get { return ProductVariant.Option3; } }

		public decimal? Price { get { return ProductVariant.Price; } set { ProductVariant.Price = value; } }
		public string SKU { get { return ProductVariant.SKU; } set { ProductVariant.SKU = value; } }
		public string Barcode { get { return ProductVariant.Barcode; } set { ProductVariant.Barcode = value; } }
		public int? InventoryQuantity { get { return ProductVariant.InventoryQuantity; } set { ProductVariant.InventoryQuantity = value; } }

		private Product Product { get; set; }
		private ProductVariant ProductVariant { get; set; }

		public FlatProduct(Product product, ProductVariant productVariant)
		{
			Product = product;
			ProductVariant = productVariant;
		}

		public ProductVariant GetProductVariant()
		{
			return ProductVariant;
		}

		//[JsonProperty("weight")]
		//public decimal? Weight { get; set; }


		//[JsonProperty("weight_unit")]
		//public string WeightUnit { get; set; }

		//[JsonProperty("inventory_management")]
		//public string InventoryManagement { get; set; }
		//[JsonProperty("inventory_item_id")]
		//public long? InventoryItemId { get; set; }
		//[JsonProperty("fulfillment_service")]
		//public string FulfillmentService { get; set; }
		//[JsonProperty("inventory_policy")]
		//public string InventoryPolicy { get; set; }

		//[JsonProperty("grams")]
		//public int? Grams { get; set; }

		static public List<FlatProduct> FromProducts(List<Product> products)
		{
			List<FlatProduct> flats = new List<FlatProduct>();
			foreach(Product p in products)
			{
				foreach (ProductVariant pv in p.Variants)
				{
					FlatProduct flat = new FlatProduct(p, pv);
					flats.Add(flat);

				}
			}

			return flats;
		}
	}
}
