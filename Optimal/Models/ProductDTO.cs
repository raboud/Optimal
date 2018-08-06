using ONWLibrary;
using ShopifySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimal.Models
{
	public class ProductDTO
	{
		private Product _data;

		public string Name { get { return this._data.Title; } set { this._data.Title = value; } }
		public string Vendor { get { return this._data.Vendor; } set { this._data.Vendor = value; } }
		public string Description { get { return this._data.BodyHtml; } set { this._data.BodyHtml = value; } }

		public List<ProductVariantDTO> Variants {get;set;}
		public string ShortDesciption
		{
			get
			{
				if (this._data.Metafields != null)
				{
					return this._data.Metafields.FirstOrDefault(m => m.Key == "ShortDescription")?.Value.ToString();
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this._data.Metafields == null)
				{
					this._data.Metafields = new List<MetaField>();
				}
				MetaField meta = this._data.Metafields.FirstOrDefault(m => m.Key == "ShortDescription");
				if (meta == null)
				{
					meta = new MetaField()
					{
						Namespace = "ownProduct",
						Key = "ShortDescription",
						ValueType = "string"
					};
					this._data.Metafields.ToList().Add(meta);
				}
				meta.Value = value;
			}
		}

		public ProductDTO()
		{
			this._data = new Product();
			this.Variants = new List<ProductVariantDTO>();
		}

		public ProductDTO(Product product)
		{
			this.Variants = new List<ProductVariantDTO>();
			this.SetProduct(product);
		}

		public Product GetProduct()
		{
			return this._data;
		}

		public void SetProduct(Product product)
		{
			this._data = product;
			this.Variants.Clear();
			foreach (ProductVariant v in this._data.Variants)
			{
				this.Variants.Add(new ProductVariantDTO(v));
			}
		}
	}
}
