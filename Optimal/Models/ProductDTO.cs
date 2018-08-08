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

		public string Name { get { return _data.Title; } set { _data.Title = value; } }
		public string Vendor { get { return _data.Vendor; } set { _data.Vendor = value; } }
		public string Description { get { return _data.BodyHtml; } set { _data.BodyHtml = value; } }

		public List<ProductVariantDTO> Variants {get;set;}
		public string ShortDesciption
		{
			get
			{
				if (_data.Metafields != null)
				{
					return _data.Metafields.FirstOrDefault(m => m.Key == "ShortDescription")?.Value.ToString();
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (_data.Metafields == null)
				{
					_data.Metafields = new List<MetaField>();
				}
				MetaField meta = _data.Metafields.FirstOrDefault(m => m.Key == "ShortDescription");
				if (meta == null)
				{
					meta = new MetaField()
					{
						Namespace = "ownProduct",
						Key = "ShortDescription",
						ValueType = "string"
					};
					_data.Metafields.ToList().Add(meta);
				}
				meta.Value = value;
			}
		}

		public ProductDTO()
		{
			_data = new Product();
			Variants = new List<ProductVariantDTO>();
		}

		public ProductDTO(Product product)
		{
			Variants = new List<ProductVariantDTO>();
			SetProduct(product);
		}

		public Product GetProduct()
		{
			return _data;
		}

		public void SetProduct(Product product)
		{
			_data = product;
			Variants.Clear();
			foreach (ProductVariant v in _data.Variants)
			{
				Variants.Add(new ProductVariantDTO(v));
			}
		}
	}
}
