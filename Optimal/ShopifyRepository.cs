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
			Products = Shopify.GetProducts().Result.ToList();
			BindingListProducts = new BindingList<Product>(Products);
			BindingSource = new BindingSource(BindingListProducts, null);
		}
	}
}
