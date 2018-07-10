using ONWLibrary;
using ShopifySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimal
{

	public class ShopifyRepository
	{
		public List<Product> Products { get; set; }

		public ShopifyRepository()
		{
			this.Products = Shopify.GetProducts().ToList();
		}
	}
}
