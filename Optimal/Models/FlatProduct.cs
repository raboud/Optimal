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
		public string Vendor { get; set; }
		public string Name { get; set; }
		[JsonProperty("barcode")]
		public string Barcode { get; set; }
		[JsonProperty("sku")]
		public string SKU { get; set; }
		[JsonProperty("product_id")]
		public long? ProductId { get; set; }
		[JsonProperty("id")]
		public long? Id { get; set; }
		[JsonProperty("option1")]
		public string Option1 { get; set; }
		[JsonProperty("option2")]
		public string Option2 { get; set; }
		[JsonProperty("option3")]
		public string Option3 { get; set; }
		[JsonProperty("price")]
		public decimal? Price { get; set; }
		[JsonProperty("inventory_quantity")]
		public int? InventoryQuantity { get; set; }

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
					FlatProduct flat = new FlatProduct()
					{
						Vendor = p.Vendor,
						Name = p.Title,
						SKU = pv.SKU,
						Barcode = pv.Barcode,
						Option1 = pv.Option1,
						Option2 = pv.Option2,
						Option3 = pv.Option3,
						//						Grams = pv.Grams,
						//						Weight = pv.Weight,
						Price = pv.Price,
						InventoryQuantity = pv.InventoryQuantity,

						Id = pv.Id,
						ProductId = pv.ProductId,
					};
					flats.Add(flat);

				}
			}

			return flats;
		}
	}
}
