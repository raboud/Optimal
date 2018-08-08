using ShopifySharp;
using System.Collections.Generic;
using System.Linq;

namespace Optimal.Models
{
	public class ProductVariantDTO
	{
		private ProductVariant _data;

		public string Option1 { get { return _data.Option1; } set { _data.Option1 = value; } }
		public string Option2 { get { return _data.Option2; } set { _data.Option2 = value; } }
		public string Option3 { get { return _data.Option3; } set { _data.Option3 = value; } }
		public string SKU { get { return _data.SKU; } set { _data.SKU = value; } }
		public string Barcode { get { return _data.Barcode; } set { _data.Barcode = value; } }
		public int? InventoryQuantity { get { return _data.InventoryQuantity; } set { _data.InventoryQuantity = value; } }

		public decimal? Cost
		{
			get
			{
				decimal? retval = null;
				if (_data.Metafields != null)
				{
					MetaField meta = _data.Metafields.FirstOrDefault(m => m.Key == "Cost");
					if (meta != null)
					{
						retval = decimal.Parse(meta.Value as string);
					}
				}
				return retval;
			}
			set
			{
				if (_data.Metafields == null)
				{
					_data.Metafields = new List<MetaField>();
				}
				MetaField meta = _data.Metafields.FirstOrDefault(m => m.Key == "Cost");
				if (meta == null)
				{
					meta = new MetaField()
					{
						Namespace = "ownProduct",
						Key = "Cost",
						ValueType = "string"
					};
					(_data.Metafields as IList<MetaField>).Add(meta);
				}
				meta.Value = value.ToString();
			}
		}
		public decimal? Price { get { return _data.Price; } set { _data.Price = value; } }

		public ProductVariantDTO()
		{
			_data = new ProductVariant();
		}

		public ProductVariantDTO(ProductVariant pv)
		{
			_data = pv;
			if (pv.Metafields == null)
			{

			}
		}



		
	}
}
