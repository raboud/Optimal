using ShopifySharp;
using System.Collections.Generic;
using System.Linq;

namespace Optimal.Models
{
	public class ProductVariantDTO
	{
		private ProductVariant _data;

		public string Option1 { get { return this._data.Option1; } set { this._data.Option1 = value; } }
		public string Option2 { get { return this._data.Option2; } set { this._data.Option2 = value; } }
		public string Option3 { get { return this._data.Option3; } set { this._data.Option3 = value; } }
		public string SKU { get { return this._data.SKU; } set { this._data.SKU = value; } }
		public string Barcode { get { return this._data.Barcode; } set { this._data.Barcode = value; } }
		public int? InventoryQuantity { get { return this._data.InventoryQuantity; } set { this._data.InventoryQuantity = value; } }

		public decimal? Cost
		{
			get
			{
				decimal? retval = null;
				if (this._data.Metafields != null)
				{
					MetaField meta = this._data.Metafields.FirstOrDefault(m => m.Key == "Cost");
					if (meta != null)
					{
						retval = decimal.Parse(meta.Value as string);
					}
				}
				return retval;
			}
			set
			{
				if (this._data.Metafields == null)
				{
					this._data.Metafields = new List<MetaField>();
				}
				MetaField meta = this._data.Metafields.FirstOrDefault(m => m.Key == "Cost");
				if (meta == null)
				{
					meta = new MetaField()
					{
						Namespace = "ownProduct",
						Key = "Cost",
						ValueType = "string"
					};
					(this._data.Metafields as IList<MetaField>).Add(meta);
				}
				meta.Value = value.ToString();
			}
		}
		public decimal? Price { get { return this._data.Price; } set { this._data.Price = value; } }

		public ProductVariantDTO()
		{
			this._data = new ProductVariant();
		}

		public ProductVariantDTO(ProductVariant pv)
		{
			this._data = pv;
			if (pv.Metafields == null)
			{

			}
		}



		
	}
}
