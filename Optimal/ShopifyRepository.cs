using ONWLibrary;
using ShopifySharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Optimal
{

	public class ShopifyRepository
	{
		public List<Product> Products { get; set; }
		public BindingList<Product> BindingListProducts { get; set; }
		public BindingSource BindingSource { get; set; }

		public ShopifyRepository()
		{
			this.Products = Shopify.GetProducts().Result.ToList();
			this.BindingListProducts = new BindingList<Product>(this.Products);
			this.BindingSource = new BindingSource(this.BindingListProducts, null);
		}
	}
}
