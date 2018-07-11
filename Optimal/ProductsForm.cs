using ONWLibrary;
using Optimal.Models;
using ShopifySharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Optimal
{
	public partial class ProductsForm : Form
	{
		private List<Product> Products;
		private List<FlatProduct> Flats;
		private SortableBindingList<FlatProduct> BindingListProducts;
		private BindingSource BindingSource;

		private int newSortColumn = -1;
		ListSortDirection newColumnDirection = ListSortDirection.Ascending;

		public ProductsForm()
		{
			Shopify.Init();
			//this.Products = Shopify.GetProducts().ToList();
			//this.BindingListProducts = new BindingList<Product>(this.Products);
			//this.BindingSource = new BindingSource(BindingListProducts, null);

			InitializeComponent();
		}

		public void initGrid()
		{
			this.dataGridView1.DataSource = this.BindingSource;
		}

		private async void ProductsForm_Load(object sender, EventArgs e)
		{
			this.Products = (await Shopify.GetProducts()).ToList();
			this.Flats = FlatProduct.FromProducts(this.Products);
			int count = this.Products.Count(p => p.Variants?.Count() == 0);
			this.BindingListProducts = new SortableBindingList<FlatProduct>(this.Flats);
			this.BindingSource = new BindingSource(BindingListProducts, null);
			this.dataGridView1.DataSource = this.BindingSource;

			this.dataGridView1.Columns.Remove("Id");
			this.dataGridView1.Columns.Remove("ProductId");
		}

		//private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		//{
		//	if (this.dataGridView1.Columns[e.ColumnIndex].SortMode != DataGridViewColumnSortMode.NotSortable)
		//	{
		//		if (e.ColumnIndex == newSortColumn)
		//		{
		//			if (newColumnDirection == ListSortDirection.Ascending)
		//				newColumnDirection = ListSortDirection.Descending;
		//			else
		//				newColumnDirection = ListSortDirection.Ascending;
		//		}

		//		newSortColumn = e.ColumnIndex;

		//		switch (newColumnDirection)
		//		{
		//			case ListSortDirection.Ascending:
		//				dataGridView1.Sort(dataGridView1.Columns[newSortColumn], ListSortDirection.Ascending);
		//				break;
		//			case ListSortDirection.Descending:
		//				dataGridView1.Sort(dataGridView1.Columns[newSortColumn], ListSortDirection.Descending);
		//				break;
		//		}
		//	}
		//}
	}
}
