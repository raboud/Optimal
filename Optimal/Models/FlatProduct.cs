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
		public string Vendor { get { return this.Product.Vendor; } }
		public string Name { get { return this.Product.Title; } }
		public string Option1 { get { return this.ProductVariant.Option1; } }
		public string Option2 { get { return this.ProductVariant.Option1; } }
		public string Option3 { get { return this.ProductVariant.Option1; } }

		public decimal? Price { get { return this.ProductVariant.Price; } set { this.ProductVariant.Price = value; } }
		public string SKU { get { return this.ProductVariant.SKU; } set { this.ProductVariant.SKU = value; } }
		public string Barcode { get { return this.ProductVariant.Barcode; } set { this.ProductVariant.Barcode = value; } }
		public int? InventoryQuantity { get { return this.ProductVariant.InventoryQuantity; } set { this.ProductVariant.InventoryQuantity = value; } }

		private Product Product { get; set; }
		private ProductVariant ProductVariant { get; set; }

		public FlatProduct(Product product, ProductVariant productVariant)
		{
			this.Product = product;
			this.ProductVariant = productVariant;
		}

		public ProductVariant GetProductVariant()
		{
			return this.ProductVariant;
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
